using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace BadaoLeblanc
{
    // LeBlanc_Base_P_Tar_Mark.troy Leblanc_Base_P_Tar_Mark_Detonate.troy LeBlanc_Base_P_Timer.troy LeblancPMark
    class Program
    {
        public static Vector3 LastWPos = new Vector3();
        public static int LastW =0;
        public static Obj_AI_Base WaitQTarget = null;
        public static List<GameObject> PassiveObjects = new List<GameObject>();
        public static List<GameObject> PassiveCDObjects = new List<GameObject>();
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Orbwalking.Orbwalker Orbwalker;

        public static Spell Q, W, E, R;

        public static Menu Menu;

        public static int Rstate, Wstate, Ecol;
        public static string QName = "LeblancQ", WName = "LeblancW", EName = "LeblancE", RName = "LeblancRToggle", QRName = "",
            WRName = "", ERName = "", RRName = "LeblancR", W2Name = "LeblancWReturn", RW2Name = "LeblancRWReturn";
        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Leblanc")
                return;

            Q = new Spell(SpellSlot.Q, 710);
            W = new Spell(SpellSlot.W, 750);
            E = new Spell(SpellSlot.E, 950);
            R = new Spell(SpellSlot.R);

            //Q.SetSkillshot(300, 50, 2000, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0, 70, 1500, false, SkillshotType.SkillshotLine);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            spellMenu.AddItem(new MenuItem("Use Q Harass", "Use Q Harass").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use W Harass", "Use W Harass").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use W Back Harass", "Use W Back Harass").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use W Combo", "Use W Combo").SetValue(true));
            spellMenu.AddItem(new MenuItem("Rmode", "R mode").SetValue(new StringList (new string[] { "Q","W","E","Any"},3)));
            spellMenu.AddItem(new MenuItem("WaitPassive", " Wait for Passive Procs").SetValue(true));
            spellMenu.AddItem(new MenuItem("force focus selected", "force focus selected").SetValue(false));
            spellMenu.AddItem(new MenuItem("if selected in :", "if selected in :").SetValue(new Slider(1000, 1000, 1500)));
            spellMenu.AddItem(new MenuItem("QE Selected Target", "QE Selected Target").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press)));

            Menu.AddToMainMenu();

            //Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;


            Chat.Print("Leblanc by badao updated 10/11/2016!, updated for Lb REWORK!");

            Chat.Print("Visist forum for more information");

            Chat.Print("Leave an upvote in database if you like it <3!");
        }
        public static int Rmode { get{ return Menu.Item("Rmode").GetValue<StringList>().SelectedIndex; } }
        public static bool WgapCombo { get { return Menu.Item("Use W Combo Gap").GetValue<bool>(); } }
        public static bool WaitPassive => Menu.Item("WaitPassive").GetValue<bool>();
        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.W)
            {
                LastW = Environment.TickCount;
                LastWPos = args.StartPosition;
            }
            //if (!sender.Owner.IsMe || args.Slot != SpellSlot.Q || !(args.Target as Obj_AI_Base).IsValidTarget())
            //    return;
            //var target = args.Target as Obj_AI_Base;
            //if (GetPassiveState(target) == 0 && !PassiveCDObjects.Any(x => x.Position.Distance(target.Position) <= 20))
            //{
            //    WaitQTarget = target;
            //    LeagueSharp.Common.Utility.DelayAction.Add(500 + Game.Ping, () => WaitQTarget = null);
            //}
        }
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "LeBlanc_Base_P_Tar_Mark.troy")
                PassiveObjects.Add(sender);
            if (sender.Name == "LeBlanc_Base_P_Timer.troy")
                PassiveCDObjects.Add(sender);
        }
        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "LeBlanc_Base_P_Tar_Mark.troy")
                PassiveObjects.RemoveAll(x => x.NetworkId == sender.NetworkId);
            if (sender.Name == "LeBlanc_Base_P_Timer.troy")
                PassiveCDObjects.RemoveAll(x => x.NetworkId == sender.NetworkId);
        }
        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            //var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            //if (target != null)
            //{
            //    var state = GetPassiveState(target);
            //    if (state ==2)
            //    {
            //        W.Cast(Game.CursorPos);
            //    }
            //}
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                //string buffs = "";
                //var nearest = HeroManager.Enemies.Where(x => x.IsValidTarget()).MinOrDefault(x => x.Distance(Game.CursorPos));
                //if (nearest != null)
                //    foreach (var buff in nearest.Buffs.Where(x => x.Name.ToLower().Contains("leblanc")))
                //    {
                //        buffs += buff.Name + (Game.Time - buff.StartTime) + " ;";
                //    }
                //Chat.Print(buffs);
                if (Menu.Item("Use Q Harass").GetValue<bool>())
                {
                    useQ();
                }
                if (Menu.Item("Use W Harass").GetValue<bool>())
                {
                    useWH();
                }
                if (Menu.Item("Use W Back Harass").GetValue<bool>())
                {
                    useWBH();
                }

            }
            if (Menu.Item("QE Selected Target").GetValue<KeyBind>().Active)
            {
                useQE();
            }
        }

        public static bool Selected()
        {
            if (!Menu.Item("force focus selected").GetValue<bool>())
            {
                return false;
            }
            else
            {
                var target = TargetSelector.GetSelectedTarget();
                float a = Menu.Item("if selected in :").GetValue<Slider>().Value;
                if (target == null || target.IsDead || target.IsZombie)
                {
                    return false;
                }
                else
                {
                    if (Player.Distance(target.Position) > a)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public static void useQ()
        {
            if (Selected())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget(Q.Range) && CanRock(target))
                {
                    Q.Cast(target);
                    if (GetPassiveState(target) == 0 && !PassiveCDObjects.Any(x => x.Position.Distance(target.Position) <= 20))
                    {
                        WaitQTarget = target;
                        LeagueSharp.Common.Utility.DelayAction.Add(500 + Game.Ping, () => WaitQTarget = null);
                    }
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget(Q.Range) && CanRock(target))
                {
                    Q.Cast(target);
                    if (GetPassiveState(target) == 0 && !PassiveCDObjects.Any(x => x.Position.Distance(target.Position) <= 20))
                    {
                        WaitQTarget = target;
                        LeagueSharp.Common.Utility.DelayAction.Add(500 + Game.Ping, () => WaitQTarget = null);
                    }
                }
            }
        }

        public static void useE()
        {
            if (Selected())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical) ??
                             TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }
            }
        }
        public static void useW()
        {
            if (Menu.Item("Use W Combo").GetValue<bool>())
            {
                if (Selected())
                {
                    var target = TargetSelector.GetSelectedTarget();
                    if (target != null && target.IsValidTarget(W.Range) && CanRock(target))
                    {
                        CastW(target);
                    }

                }
                else
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                    if (target != null && target.IsValidTarget(W.Range) && CanRock(target))
                    {
                        CastW(target);
                    }
                }
            }
        }

        public static void useWH()
        {
            if (Selected())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget(W.Range) && CanRock(target))
                {
                    CastW(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget(W.Range) && CanRock(target))
                {
                    CastW(target);
                }
            }
        }

        public static void useWBH()
        {
            if (W.Instance.Name == W2Name)
                W.Cast();
        }

        public static void useR()
        {
            float range = Q.Range;
            if (Rmode == 2)
                range = W.Range;
            if (Rmode == 3)
                range = E.Range;
            if (Selected())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget(range) && (CanRock(target) || CanRockR()))
                {
                    CastR(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget(range) && (CanRock(target) || CanRockR()))
                {
                    CastR(target);
                }
            }
        }

        public static void CastR(Obj_AI_Base target)
        {
            if (R.Instance.Name == RName && R.IsReady())
            {
                if (Q.IsReady())
                {
                    return;
                }
                R.Cast();
            }
            if (R.Instance.Name == RRName)
            {
                if (Rmode == 0 && target != null && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
                if (Rmode == 1 && target != null && target.IsValidTarget(W.Range))
                {
                    CastW(target);
                }
                if (Rmode == 2 && target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }
                if (Rmode == 3 && target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                    CastW(target);
                    Q.Cast(target);
                }
            }
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (!W.IsReady() || W.Instance.Name == W2Name)
                return;
            var t = Prediction.GetPrediction(target, 400).CastPosition;
            float x = target.MoveSpeed;
            float y = x * 400 / 1000;
            var pos = target.Position;
            if (target.Distance(t) <= y)
            {
                pos = t;
            }
            if (target.Distance(t) > y)
            {
                pos = target.Position.Extend(t, y - 50);
            }
            if (Player.Distance(pos) <= 600)
            {
                W.Cast(pos);
            }
            if (Player.Distance(pos) > 600)
            {
                if (target.Distance(t) > y)
                {
                    var pos2 = target.Position.Extend(t, y);
                    if (Player.Distance(pos2) <= 600)
                    {
                        W.Cast(pos2);
                    }
                    else
                    {
                        var prediction = W.GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            var pos3 = prediction.CastPosition;
                            var pos4 = Player.Position.Extend(pos3, 600);
                            W.Cast(pos4);
                        }
                    }

                }
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (E.IsReady() && !Player.IsDashing())
            {
                E.Cast(target);
            }
        }
        public static void Combo()
        {
            if (R.Instance.Name != RRName || R.Instance.Level < 1)
            {
                useE();
                useQ();
                useW();
            }
            useR();
        }

        public static void useQE()
        {
            Orbwalking.MoveTo(Game.CursorPos);
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if( Player.Distance(target.Position) <= Q.Range)
                    {
                        Q.Cast(target);
                    }
                    if (Player.Distance(target.Position) <= E.Range)
                    {
                        E.Cast(target);
                    }
                } 
        }
        public static int GetPassiveState(Obj_AI_Base target)
        {
            if (!target.HasBuff("LeblancPMark"))
                return 0;
            var buff = target.GetBuff("LeblancPMark");
            if ((Game.Time - buff.StartTime) * 1000 < 1400)
                return 1;
            if ((Game.Time - buff.StartTime) * 1000 >= 1400 && (Game.Time - buff.StartTime) * 1000 <= 3800)
                return 2;
            return 0;
        }
        public static bool CanRock(Obj_AI_Base target)
        {
            int state = GetPassiveState(target);
            if (!WaitPassive)
                return true;
            if (state == 2)
                return true;
            if (GetDamage(target) >= target.Health)
                return true;
            if (R.Instance.Name == RRName && R.Instance.Level >= 1)
                return true;
            if (PassiveCDObjects.Any(x => x.Position.Distance(target.Position) <= 20))
                return true;
            if (target.HasBuff("LeblancE") && state == 1)
            {
                var buffE = target.GetBuff("LeblancE");
                var buffP = target.GetBuff("LeblancPMark");
                if (buffP.StartTime * 1000 + 1495 <= buffE.StartTime * 1000 + 1500)
                    return true;
            }
            if (WaitQTarget != null && WaitQTarget.NetworkId == target.NetworkId && state == 0)
            {
                return false;
            }
            if (LastWPos.IsValid() && LastWPos.Distance(target.Position) <= 350 && Environment.TickCount - LastW <= 1000 + Game.Ping && state == 0)
                return false;
            if (state == 1)
            {
                return false;
            }
            return true;
        }
        public static bool CanRockR()
        {
            return
                Q.IsReady()
                || (W.IsReady() && W.Instance.Name == WName)
                || E.IsReady();
        }
        public static double GetDamage(Obj_AI_Base target)
        {
            double damage = 0;
            if (R.Instance.Name != RRName || R.Instance.Level < 1)
            {
                if (Q.IsReady())
                    damage += Q.GetDamage(target);
                if (W.IsReady())
                    damage += W.GetDamage(target);
                if (Q.IsReady())
                    damage += E.GetDamage(target) * 2;
            }
            if (R.Instance.Name == RName && R.Instance.Level >= 1 && R.IsReady())
            {
                damage += Player.CalcDamage(target, Damage.DamageType.Magical, new double[] { 125, 225, 325 }[R.Instance.Level - 1] + 0.5 * Player.TotalMagicalDamage);
            }
            if (R.Instance.Name == RRName && R.Instance.Level  >=1)
            {
                damage += Player.CalcDamage(target, Damage.DamageType.Magical, new double[] { 125, 225, 325 }[R.Instance.Level - 1] + 0.5 * Player.TotalMagicalDamage);
            }
            return damage;
        }
    }
}
