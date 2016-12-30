using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace XDSharp.Champions.Cassiopeia
{
    class Main
    {
        public static AIHeroClient MainTarget;
        public static List<AIHeroClient> Targets = XDSharp.Utils.TargetSelector.Targets;
        public static Menu Option;
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static float Range = 100f;
        private static long LastQCast = 0;
        private static long LastECast = 0;
        public static AimMode AMode = AimMode.Normal;
        public static HitChance Chance = HitChance.VeryHigh;
        public static bool listed = true;
        public static bool Nopsntarget = true;
        public static bool aastatus;
        public static float testspell;

        public static int kills = 0;
        public static Random rand = new Random();

        public static List<string> Messages;

        public enum AimMode
        {
            Normal = 1,
            HitChance = 0
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

#region GetPsnTime
        private static float GetPoisonBuffEndTime(Obj_AI_Base target)
        {
            var buffEndTime = target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Type == BuffType.Poison)
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
            return buffEndTime;
        }
#endregion

#region Targetselect

        private static AIHeroClient GetQTarget()
        {
            if (MainTarget == null || MainTarget.IsDead || !MainTarget.IsVisible || MainTarget.HasBuffOfType(BuffType.Poison))
            {
                foreach (var target in Targets)
                {
                    if (target != null && target.IsVisible && !target.IsDead)
                    {
                        if (!target.HasBuffOfType(BuffType.Poison) || GetPoisonBuffEndTime(target) < (Game.Time + Q.Delay))
                        {
                            if (Player.ServerPosition.Distance(PreCastPos(target, Range, 0.6f)) < Q.Range)
                            {
                                return target;
                            }
                        }
                    }
                }
            }
            else
                return MainTarget;
            return null;
        }

        private static AIHeroClient GetWTarget()
        {
            if (MainTarget == null || MainTarget.IsDead || !MainTarget.IsVisible || MainTarget.HasBuffOfType(BuffType.Poison))
            {
                foreach (var target in Targets)
                {
                    if (target != null && target.IsVisible && !target.IsDead)
                    {
                        if (!target.HasBuffOfType(BuffType.Poison) || (Player.ServerPosition.Distance(Q.GetPrediction(target, true).CastPosition) > Q.Range))
                        {
                            if (Player.ServerPosition.Distance(PreCastPos(target, Range, Player.ServerPosition.Distance(target.ServerPosition) / W.Speed)) < W.Range)
                            {
                                return target;
                            }
                        }
                    }
                }
            }
            else
                return MainTarget;
            return null;
        }

        private static AIHeroClient GetETarget()
        {
            if (MainTarget == null || MainTarget.IsDead || !MainTarget.IsVisible || ((!MainTarget.HasBuffOfType(BuffType.Poison) && GetPoisonBuffEndTime(MainTarget) < (Game.Time + E.Delay)) || Player.GetSpellDamage(MainTarget, SpellSlot.E) < MainTarget.Health))
            {
                foreach (var target in Targets)
                {
                    if (target != null && target.IsVisible && !target.IsDead)
                    {
                        if ((target.HasBuffOfType(BuffType.Poison) && GetPoisonBuffEndTime(target) > (Game.Time + E.Delay)) || Player.GetSpellDamage(target, SpellSlot.E) > target.Health)
                        {
                            if (target.IsValidTarget(E.Range))
                            {
                                return target;
                            }
                        }
                    }
                }
            }
            else
                return MainTarget;
            return null;
        }

        private static AIHeroClient GetRFaceTarget()
        {
            var FaceEnemy = ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && enemy.IsFacing(Player) && R.WillHit(enemy, R.GetPrediction(enemy, true).CastPosition)).ToList();
            foreach (var target in Targets)
            {
                if (target != null && target.IsVisible && !target.IsDead)
                {
                    foreach (var fenemy in FaceEnemy)
                        if (fenemy != null && fenemy.IsVisible && !fenemy.IsDead)
                        {
                            if (target.BaseSkinName == fenemy.BaseSkinName)
                                return fenemy;
                        }
                }
            }
            return null;

        }

        private static AIHeroClient GetRTarget()
        {
            var Enemy = ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && R.WillHit(enemy, R.GetPrediction(enemy, true).CastPosition)).ToList();

            foreach (var target in Targets)
            {
                if (target != null && target.IsVisible && !target.IsDead)
                {
                    foreach (var enemy in Enemy)
                        if (enemy != null && enemy.IsVisible && !enemy.IsDead)
                        {
                            if (target.BaseSkinName == enemy.BaseSkinName)
                                return enemy;
                        }
                }
            }
            return null;
        }

#endregion

#region OnGameLoad
        public void OGLoad()
        {
            Game.OnUpdate += OnTick;
            Drawing.OnDraw += OnDraw;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Game.OnWndProc += Game_OnWndProc;

            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;

            XDSharp.Utils.TargetSelector.Targetlist(XDSharp.Utils.TargetSelector.TargetingMode.AutoPriority);
            
            Q = new Spell(SpellSlot.Q, 850f);
            Q.SetSkillshot(0.75f, Q.Instance.SData.CastRadius, int.MaxValue, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 850f);
            W.SetSkillshot(0.5f, W.Instance.SData.CastRadius, W.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 700f);
            E.SetTargetted(0.2f, float.MaxValue);

            R = new Spell(SpellSlot.R, 825f);
            R.SetSkillshot(0.3f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Option = new Menu("XD-Crew", "XD-Crew Cassio", true);
            Option.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Option.SubMenu("Orbwalking"));

            Option.AddItem(new MenuItem("TargetingMode", "Target Mode").SetValue(new StringList(Enum.GetNames(typeof(XDSharp.Utils.TargetSelector.TargetingMode)))));
            Option.SubMenu("Aiming").AddItem(new MenuItem("AimMode", "Aim Mode").SetValue(new StringList(Enum.GetNames(typeof(AimMode)))));
            Option.SubMenu("Aiming").AddItem(new MenuItem("Hitchance", "Hitchance Mode").SetValue(new StringList(Enum.GetNames(typeof(HitChance)))));
            Option.AddItem(new MenuItem("Edelay", "Ecombo delay").SetValue(new Slider(0, 0, 5)));
            Option.SubMenu("Farming").AddItem(new MenuItem("Qlaneclear", "Q Lane Clear").SetValue(true));
            Option.SubMenu("Farming").AddItem(new MenuItem("Wlaneclear", "W Lane Clear").SetValue(true));
            Option.SubMenu("Farming").AddItem(new MenuItem("Elasthit", "E Lasthit no psn").SetValue(true));
            Option.SubMenu("Farming").AddItem(new MenuItem("LaneClearMana", "Lane Clear Mana").SetValue(new Slider(70, 0, 100)));
            Option.SubMenu("Ultimate").AddItem(new MenuItem("BlockR", "BlockR").SetValue(true));
            Option.SubMenu("Ultimate").AddItem(new MenuItem("AutoUlt", "AutoUltimate").SetValue(true));
            Option.SubMenu("Ultimate").AddItem(new MenuItem("AutoUltF", "AutoUlt facing").SetValue(new Slider(3, 1, 5)));
            Option.SubMenu("Ultimate").AddItem(new MenuItem("AutoUltnF", "AutoUlt not facing").SetValue(new Slider(5, 1, 5)));
            Option.SubMenu("Ultimate").AddItem(new MenuItem("AssistedUltKey", "Assisted Ult Key").SetValue((new KeyBind("R".ToCharArray()[0], KeyBindType.Press))));
            Option.SubMenu("Drawing").AddItem(new MenuItem("DrawQ", "DrawQ").SetValue(true));
            Option.SubMenu("Drawing").AddItem(new MenuItem("DrawP", "Draw Prediction").SetValue(true));
            Option.SubMenu("Drawing").AddItem(new MenuItem("DrawPsn", "Draw Poison").SetValue(true));
            /*
            Option.SubMenu("Advanced").AddItem(new MenuItem("QsRadius", "Q Spell Radius").SetValue(new Slider((int)Q.Instance.SData.CastRadius, 0, (int)Q.Instance.SData.CastRadius)));
            Option.SubMenu("Advanced").AddItem(new MenuItem("QsDelay", "Q Spell Delay").SetValue(new Slider(70, 0, 100)));
            Option.SubMenu("Advanced").AddItem(new MenuItem("WsRadius", "W Spell Radius").SetValue(new Slider((int)W.Instance.SData.CastRadius, 0, (int)W.Instance.SData.CastRadius)));
            Option.SubMenu("Advanced").AddItem(new MenuItem("WsDelay", "W Spell Delay").SetValue(new Slider(70, 0, 100)));
            */
            Option.AddToMainMenu();

        }
#endregion

#region OnTick
        private static void OnTick(EventArgs args)
        {

            try
            {
                var menuItem = Option.Item("TargetingMode").GetValue<StringList>();
                Enum.TryParse(menuItem.SList[menuItem.SelectedIndex], out XDSharp.Utils.TargetSelector.TMode);


                var AutoUlt = Option.Item("AutoUlt").GetValue<bool>();
                /*
                var QsRadius = Option.Item("QsRadius").GetValue<Slider>().Value;
                var QsDelay = Option.Item("QsDelay").GetValue<Slider>().Value;

                Q.SetSkillshot((float)QsDelay/100, (float)QsRadius, int.MaxValue, false, SkillshotType.SkillshotCircle);
                */
                if (AutoUlt)
                    CastAutoUltimate();

                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        Harass();
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        JungleClear();
                        WaveClear();
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        Freeze();
                        break;
                    default:
                        break;
                }
                switch (XDSharp.Utils.TargetSelector.TMode)
                {
                    case XDSharp.Utils.TargetSelector.TargetingMode.AutoPriority:
                        if (listed == false)
                            XDSharp.Utils.TargetSelector.Targetlist(XDSharp.Utils.TargetSelector.TargetingMode.AutoPriority);
                        listed = true;
                        break;
                    case XDSharp.Utils.TargetSelector.TargetingMode.FastKill:
                        XDSharp.Utils.TargetSelector.Targetlist(XDSharp.Utils.TargetSelector.TargetingMode.FastKill);
                        listed = false;
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
#endregion

#region BeforeAttack
        static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if ((Player.Mana < E.Instance.SData.Mana) || (E.Instance.Level == 0) || ((E.Instance.CooldownExpires - Game.Time) > 0.7) || Player.HasBuffOfType(BuffType.Silence))
                {
                    args.Process = true;
                    aastatus = true;
                }
                else
                {
                    args.Process = false;
                    aastatus = false;
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var LaneClearMana = Option.Item("LaneClearMana").GetValue<Slider>().Value;
                if ((Player.ManaPercentage() < LaneClearMana) || (E.Instance.Level == 0) || ((E.Instance.CooldownExpires - Game.Time) > 0.7) || Nopsntarget || Player.HasBuffOfType(BuffType.Silence))
                {
                    args.Process = true;
                    aastatus = true;
                }
                else
                {
                    args.Process = false;
                    aastatus = false;
                }
            }
        }
#endregion

#region Combo

        public static void Combo()
        {
            var menuItem3 = Option.Item("AimMode").GetValue<StringList>();
            Enum.TryParse(menuItem3.SList[menuItem3.SelectedIndex], out AMode);

            var menuItem2 = Option.Item("Hitchance").GetValue<StringList>();
            Enum.TryParse(menuItem2.SList[menuItem2.SelectedIndex], out Chance);
            var EDelay = Option.Item("Edelay").GetValue<Slider>().Value;

            if (E.IsReady() && GetETarget() != null)
            {
                if (Environment.TickCount >= LastECast + (EDelay * 100))
                    E.Cast(GetETarget());
            }

            if (Q.IsReady())
            {
                switch (AMode)
                {
                    case AimMode.HitChance:
                        Q.CastIfHitchanceEquals(GetQTarget(), Chance, false);
                        break;
                    case AimMode.Normal:
                        Q.Cast(PreCastPos(GetQTarget(), Range, 0.6f));
                        break;

                }
            }
            if (W.IsReady() && Environment.TickCount > LastQCast + Q.Delay * 1000)
            {
                switch (AMode)
                {
                    case AimMode.HitChance:
                        W.CastIfHitchanceEquals(GetWTarget(), Chance, false);
                        break;
                    case AimMode.Normal:
                        W.Cast(PreCastPos(GetWTarget(), Range, Player.ServerPosition.Distance(GetWTarget().ServerPosition) / W.Speed));
                        break;
                }
            }

        }
#endregion

#region Harras

        public static void Harass()
        {
            var menuItem3 = Option.Item("AimMode").GetValue<StringList>();
            Enum.TryParse(menuItem3.SList[menuItem3.SelectedIndex], out AMode);

            var menuItem2 = Option.Item("Hitchance").GetValue<StringList>();
            Enum.TryParse(menuItem2.SList[menuItem2.SelectedIndex], out Chance);

            if (E.IsReady() && GetETarget() != null)
            {
                E.Cast(GetETarget());
            }
            if (Q.IsReady() && (Player.ServerPosition.Distance(Q.GetPrediction(GetQTarget(), true).CastPosition) < Q.Range))
            {

                Q.CastIfHitchanceEquals(GetQTarget(), HitChance.VeryHigh, false);

            }
        }

#endregion

#region Jungle

        public static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (!mobs.Any())
                return;

            var mob = mobs.First();

            if (Q.IsReady() && mob.IsValidTarget(Q.Range))
            {
                Q.Cast(mob.ServerPosition);
            }

            if (E.IsReady() && mob.HasBuffOfType(BuffType.Poison) && mob.IsValidTarget(E.Range))
            {
                E.Cast(mob);
            }

            if (W.IsReady() && mob.IsValidTarget(W.Range))
            {
                W.Cast(mob.ServerPosition);
            }

        }

#endregion

#region Farm
        public static void WaveClear()
        {
            if (!Orbwalking.CanMove(40)) return;

            var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All, MinionTeam.Enemy).ToList();
            var allMinionsW = MinionManager.GetMinions(Player.ServerPosition, W.Range + W.Width, MinionTypes.All, MinionTeam.Enemy).ToList();
            var allMinionsQnopsn = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All, MinionTeam.Enemy).Where(x => !x.HasBuffOfType(BuffType.Poison) || GetPoisonBuffEndTime(x) <= (Game.Time + Q.Delay)).ToList();
            var rangedMinionsQnopsn = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width, MinionTypes.Ranged, MinionTeam.Enemy).Where(x => !x.HasBuffOfType(BuffType.Poison) || GetPoisonBuffEndTime(x) <= (Game.Time + Q.Delay)).ToList();
            var allMinionsWnopsn = MinionManager.GetMinions(Player.ServerPosition, W.Range + W.Width, MinionTypes.All, MinionTeam.Enemy).Where(x => !x.HasBuffOfType(BuffType.Poison) || GetPoisonBuffEndTime(x) <= (Game.Time + W.Delay)).ToList();
            var rangedMinionsWnopsn = MinionManager.GetMinions(Player.ServerPosition, W.Range + W.Width, MinionTypes.Ranged, MinionTeam.Enemy).Where(x => !x.HasBuffOfType(BuffType.Poison) || GetPoisonBuffEndTime(x) <= (Game.Time + W.Delay)).ToList();

            var Qlaneclear = Option.Item("Qlaneclear").GetValue<bool>();
            var Wlaneclear = Option.Item("Wlaneclear").GetValue<bool>();
            var LaneClearMana = Option.Item("LaneClearMana").GetValue<Slider>().Value;

            if (allMinionsQnopsn.Count() == allMinionsQ.Count())
                Nopsntarget = true;
            else
                Nopsntarget = false;

            if (Q.IsReady() && allMinionsQnopsn.Count() == allMinionsQ.Count() && Qlaneclear)
            {
                var FLr = Q.GetCircularFarmLocation(rangedMinionsQnopsn, Q.Width);
                var FLa = Q.GetCircularFarmLocation(allMinionsQnopsn, Q.Width);

                if (FLr.MinionsHit >= 3 && Player.Distance(FLr.Position) < (Q.Range + Q.Width))
                {
                    Q.Cast(FLr.Position);
                    return;
                }
                else
                    if (FLa.MinionsHit >= 2 || allMinionsQnopsn.Count() == 1 && Player.Distance(FLr.Position) < (Q.Range + Q.Width))
                    {
                        Q.Cast(FLa.Position);
                        return;
                    }
            }

            if (W.IsReady() && allMinionsWnopsn.Count() == allMinionsW.Count() && Wlaneclear && Environment.TickCount > (LastQCast + Q.Delay * 1000))
            {
                var FLr = W.GetCircularFarmLocation(rangedMinionsWnopsn, W.Width);
                var FLa = W.GetCircularFarmLocation(allMinionsWnopsn, W.Width);

                if (FLr.MinionsHit >= 3 && Player.Distance(FLr.Position) < (W.Range + W.Width))
                {
                    W.Cast(FLr.Position);
                    return;
                }
                else
                    if (FLa.MinionsHit >= 2 || allMinionsWnopsn.Count() == 1 && Player.Distance(FLr.Position) < (W.Range + W.Width))
                    {
                        W.Cast(FLa.Position);
                        return;
                    }
            }

            if (E.IsReady())
            {
                var MinionList = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

                foreach (var minion in MinionList.Where(x => x.HasBuffOfType(BuffType.Poison)))
                {
                    var buffEndTime = GetPoisonBuffEndTime(minion);
                    if (buffEndTime > Game.Time + E.Delay)
                    {
                        if (Player.GetSpellDamage(minion, SpellSlot.E) > minion.Health || Player.ManaPercentage() > LaneClearMana)
                        {
                            E.Cast(minion);
                        }
                    }
                }
            }

        }

#endregion

#region Lasthit

        public static void Freeze()
        {
            var elasthit = Option.Item("Elasthit").GetValue<bool>();
            if (!Orbwalking.CanMove(40)) return;

            if (E.IsReady())
            {
                var MinionListQ = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                var MinionListE = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                /*
                foreach (var minion in MinionListQ.Where(x => x.Health < (Player.GetSpellDamage(x, SpellSlot.Q)/3)))
                {
                    if (Q.IsReady() && Player.Distance(minion,true) > Player.AttackRange)
                        Q.Cast(minion);
                }*/

                foreach (var minion in MinionListE.Where(x => Player.GetSpellDamage(x, SpellSlot.E) > x.Health))
                {
                    if ((minion.HasBuffOfType(BuffType.Poison) && (GetPoisonBuffEndTime(minion) > Game.Time + E.Delay)) || elasthit)
                    {
                        E.Cast(minion);
                    }

                }
            }

        }

#endregion

        static void Game_OnWndProc(WndEventArgs args)
        {
            if (MenuGUI.IsChatOpen)
                return;

            var AssistedUltKey = Option.Item("AssistedUltKey").GetValue<KeyBind>().Key;

            if (args.WParam == AssistedUltKey)
            {
                args.Process = false;
                CastAssistedUlt();
            }

            if (args.Msg == (uint)WindowsMessages.WM_LBUTTONDOWN)
            {

                MainTarget =
                    HeroManager.Enemies
                        .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000)
                        .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault(); ;
            }

        }
#region Ultimate
        public static void CastAssistedUlt()
        {
            var faceEnemy = ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && enemy.IsFacing(Player) && R.WillHit(enemy, R.GetPrediction(enemy, true).CastPosition)).ToList();
            var Enemy = ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && R.WillHit(enemy, R.GetPrediction(enemy, true).CastPosition)).ToList();

            if (faceEnemy.Count() >= 1 && GetRFaceTarget() != null)
            {
                R.Cast(R.GetPrediction(GetRFaceTarget(), true).CastPosition);
            }
            else
                if (Enemy.Count >= 1 && GetRTarget() != null)
                {
                    R.Cast(R.GetPrediction(GetRTarget(), true).CastPosition);
                }
        }

        public static void CastAutoUltimate()
        {
            var faceEnemy = ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && enemy.IsFacing(Player) && R.WillHit(enemy, R.GetPrediction(enemy, true).CastPosition)).ToList();
            var Enemy = ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && R.WillHit(enemy, R.GetPrediction(enemy, true).CastPosition)).ToList();
            
            var AutoUltF = Option.Item("AutoUltF").GetValue<Slider>().Value;
            var AutoUltnF = Option.Item("AutoUltnF").GetValue<Slider>().Value;

            if (faceEnemy.Count() >= AutoUltF && GetRFaceTarget() != null)
            {
                R.Cast(R.GetPrediction(GetRFaceTarget(), true).CastPosition);
            }
            else
                if (Enemy.Count >= AutoUltnF && GetRTarget() != null)
                {
                    R.Cast(R.GetPrediction(GetRTarget(), true).CastPosition);
                }

        }

        public static Tuple<int, List<AIHeroClient>> GetHits(Spell spell)
        {
            var GetenemysHit = ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && spell.WillHit(enemy, R.GetPrediction(enemy, true).CastPosition)).ToList();

            return new Tuple<int, List<AIHeroClient>>(GetenemysHit.Count, GetenemysHit);
        }
#endregion

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var BlockR = Option.Item("BlockR").GetValue<bool>();

            if (args.Slot == SpellSlot.R && GetHits(R).Item1 == 0 && BlockR)
                args.Process = false;
            if (args.Slot == SpellSlot.Q)
                LastQCast = Environment.TickCount;
            if (args.Slot == SpellSlot.E)
                LastECast = Environment.TickCount;
        }


        public static Vector2 PositionAfter(Obj_AI_Base unit, float t, float speed = float.MaxValue)
        {
            var distance = t * speed;
            var path = unit.Path.ToList().To2D();

            for (var i = 0; i < path.Count - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];
                var d = a.Distance(b);

                if (d < distance)
                {
                    distance -= d;
                }
                else
                {
                    return a + distance * (b - a).Normalized();
                }
            }


            return path[path.Count - 1];
        }

        public static Vector3 PreCastPos(AIHeroClient Hero, float Delay, float Range)
        {
            float value = 0f;
            if (Hero.IsFacing(Player))
            {
                value = (Range - Hero.BoundingRadius);
            }
            else
            {
                value = -(Range - Hero.BoundingRadius);
            }
            var distance = Delay * Hero.MoveSpeed + value;
            var path = Hero.Path.ToList().To2D();

            for (var i = 0; i < path.Count - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];
                var d = a.Distance(b);

                if (d < distance)
                {
                    distance -= d;
                }
                else
                {
                    return (a + distance * (b - a).Normalized()).To3D();
                }
            }


            return (path[path.Count - 1]).To3D();
        }

        public static Paths WPPolygon(AIHeroClient Hero, float delay)
        {
            List<Vector2Time> HeroPath = Hero.GetWaypointsWithTime();
            Vector2 myPath;
            Paths WPPaths = new Paths();
            for (var i = 0; i < HeroPath.Count() - 1; i++)
            {
                if (HeroPath.ElementAt<Vector2Time>(i + 1).Time <= delay)
                {
                    Geometry.Polygon.Rectangle WPRectangle = new Geometry.Polygon.Rectangle(HeroPath.ElementAt<Vector2Time>(i).Position, HeroPath.ElementAt<Vector2Time>(i + 1).Position, Hero.BoundingRadius);
                    Geometry.Polygon.Circle Box = new Geometry.Polygon.Circle(HeroPath.ElementAt<Vector2Time>(i).Position, Hero.BoundingRadius);
                    WPPaths.Add(Box.ToClipperPath());
                    WPPaths.Add(WPRectangle.ToClipperPath());
                }
                else
                {
                    myPath = PositionAfter(Hero, delay, Hero.MoveSpeed);
                    Geometry.Polygon.Rectangle WPRectangle = new Geometry.Polygon.Rectangle(HeroPath.ElementAt<Vector2Time>(i).Position, myPath, Hero.BoundingRadius);
                    Geometry.Polygon.Circle Box = new Geometry.Polygon.Circle(myPath, Hero.BoundingRadius);
                    WPPaths.Add(Box.ToClipperPath());
                    WPPaths.Add(WPRectangle.ToClipperPath());
                    break;
                }
            }
            Geometry.Polygon.Circle WPFirstBox = new Geometry.Polygon.Circle(HeroPath.First<Vector2Time>().Position, Hero.BoundingRadius);
            WPPaths.Add(WPFirstBox.ToClipperPath());
            return WPPaths;
        }

        public static void Interceptiontest(AIHeroClient Enemy, float delay, float Range)
        {
            Geometry.Polygon.Circle Qspellpoly = new Geometry.Polygon.Circle(PreCastPos(Enemy, Range, delay), 130f);
            Qspellpoly.Draw(System.Drawing.Color.Khaki);

            Paths subjs = new Paths();
            foreach (var bla in WPPolygon(Enemy, delay).ToPolygons())
            {
                subjs.Add(bla.ToClipperPath());
            }

            Paths clips = new Paths(1);
            clips.Add(Qspellpoly.ToClipperPath());

            Paths solution = new Paths();
            Clipper c = new Clipper();
            c.AddPaths(subjs, PolyType.ptSubject, true);
            c.AddPaths(clips, PolyType.ptClip, true);
            c.Execute(ClipType.ctIntersection, solution);

            foreach (var bli in solution.ToPolygons())
            {
                bli.Draw(System.Drawing.Color.Blue);
            }
        }

#region Draw

        private static void OnDraw(EventArgs args)
        {
            var DrawQ = Option.Item("DrawQ").GetValue<bool>();
            var DrawP = Option.Item("DrawP").GetValue<bool>();
            var DrawPsn = Option.Item("DrawPsn").GetValue<bool>();
            var QsRadius = Option.Item("QsRadius").GetValue<Slider>().Value;
            List<Vector2Time> pathT = Player.GetWaypointsWithTime();

            try
            {
                if (DrawP)
                {
                    foreach (var enemy in Targets)
                    {
                        if (enemy.IsVisible && !enemy.IsDead)
                        {
                            Render.Circle.DrawCircle(PreCastPos(enemy, Range, 0.6f), Q.Width, System.Drawing.Color.Green);

                            foreach (var bla in WPPolygon(enemy,0.6f).ToPolygons())
                            {
                                bla.Draw(System.Drawing.Color.White);
                            }
                        }
                    }
                }

                if (DrawPsn)
                {
                    foreach (var enemy in Targets)
                    {
                        if (enemy.IsVisible && !enemy.IsDead && enemy.HasBuffOfType(BuffType.Poison))
                        {
                            Drawing.DrawLine(Drawing.WorldToScreen(Player.ServerPosition), Drawing.WorldToScreen(enemy.ServerPosition), 1, System.Drawing.Color.Green);
                        }
                    }
                }

                if (MainTarget != null && MainTarget.IsVisible)
                {
                    Render.Circle.DrawCircle(MainTarget.ServerPosition, 100, System.Drawing.Color.Red);
                }

                if (DrawQ)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Khaki);
                }
            }
            catch (Exception ex)
            {
                Chat.Print(ex.ToString());
            }
        }
#endregion


    }
}
