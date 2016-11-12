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
 namespace XDSharp.Champions.Karthus
{
    class Main
    {
        public static AIHeroClient MainTarget;
        public static List<AIHeroClient> Targets = XDSharp.Utils.TargetSelector.Targets;
        public static Menu Option;
        public static float varRange = 50f;
        public static Single QsDelay = 0.625f;
        public static Vector3 LastQCastpos;
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static bool Ecasted = false;
        public static AimMode AMode = AimMode.Normal;
        public static HitChance Chance = HitChance.VeryHigh;
        public static bool listed = true;


        public enum AimMode
        {
            Normal = 1,
            HitChance = 0
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

#region Targetselect

        private static AIHeroClient GetQTarget()
        {
            if (MainTarget == null || MainTarget.IsDead || !MainTarget.IsVisible)
            {
                foreach (var target in Targets)
                {
                    if (target != null && target.IsVisible && !target.IsDead)
                    {
                        if (Player.ServerPosition.Distance(PreCastPos(target, (QsDelay / 1000), Q.Width, varRange)) < Q.Range)
                        {
                            return target;
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
            if (MainTarget == null || MainTarget.IsDead || !MainTarget.IsVisible)
            {
                foreach (var target in Targets)
                {
                    if (target != null && target.IsVisible && !target.IsDead)
                    {
                        if (Player.ServerPosition.Distance(PreCastPos(target, 0.0f, 1, 0)) < W.Range)
                        {
                            return target;
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
            if (MainTarget == null || MainTarget.IsDead || !MainTarget.IsVisible)
            {
                foreach (var target in Targets)
                {
                    if (target != null && target.IsVisible && !target.IsDead)
                    {

                        if (target.IsValidTarget(E.Range))
                        {
                            return target;
                        }
                    }
                }
            }
            else
                return MainTarget;
            return null;
        }

        private static List<AIHeroClient> GetRTarget()
        {
            var Enemy = ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget() && Player.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health && !enemy.IsDead).ToList();
            //Player.GetSpellDamage(MainTarget, SpellSlot.E) < MainTarget.Health
            if (Enemy.Count() != 0)
            {
                return Enemy;
            }
            else
                return null;
        }

#endregion

#region OnGameLoad
        public void OGLoad()
        {
            Game.OnUpdate += OnTick;
            Drawing.OnDraw += OnDraw;
            Game.OnWndProc += Game_OnWndProc;

            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;

            XDSharp.Utils.TargetSelector.Targetlist(XDSharp.Utils.TargetSelector.TargetingMode.AutoPriority);

            Q = new Spell(SpellSlot.Q, 875f);
            Q.SetSkillshot(0.625f, Q.Instance.SData.CastRadius, float.MaxValue, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 1000f);
            W.SetSkillshot(0.5f, W.Instance.SData.CastRadius, W.Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 550f);
            E.SetSkillshot(0.5f, E.Instance.SData.CastRange, float.MaxValue, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R);
            //R.SetSkillshot(3.0f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);

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
           // Option.SubMenu("Farming").AddItem(new MenuItem("Qlaneclear", "Q Lane Clear").SetValue(true));
            Option.SubMenu("Combo").AddItem(new MenuItem("WCombo", "W Combo").SetValue(true));
            Option.SubMenu("Combo").AddItem(new MenuItem("ECombo", "E Combo").SetValue(true));
            //Option.SubMenu("Farming").AddItem(new MenuItem("LaneClearMana", "Lane Clear Mana").SetValue(new Slider(70, 0, 100)));
            Option.SubMenu("Ultimate").AddItem(new MenuItem("AutoUlt", "AutoUltimate").SetValue(true));
            Option.SubMenu("Drawing").AddItem(new MenuItem("DrawQ", "DrawQ").SetValue(true));
            Option.SubMenu("Drawing").AddItem(new MenuItem("DrawR", "DrawR").SetValue(true));
            Option.SubMenu("Drawing").AddItem(new MenuItem("DrawP", "Draw Prediction").SetValue(true));

            Option.SubMenu("Advanced").AddItem(new MenuItem("QsvarRange", "Q Spell value").SetValue(new Slider((int)Q.Instance.SData.CastRadius, (int)Q.Instance.SData.CastRadius*-1, (int)Q.Instance.SData.CastRadius)));
            Option.SubMenu("Advanced").AddItem(new MenuItem("QsDelay", "Q Spell Delay").SetValue(new Slider(625, 0, 1000)));
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

               // Q.SetSkillshot(QsDelay, Q.Instance.SData.CastRadius, float.MaxValue, false, SkillshotType.SkillshotCircle);


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
           
        }
#endregion

#region Combo

        public static void Combo()
        {
            var menuItem3 = Option.Item("AimMode").GetValue<StringList>();
            Enum.TryParse(menuItem3.SList[menuItem3.SelectedIndex], out AMode);

            var menuItem2 = Option.Item("Hitchance").GetValue<StringList>();
            Enum.TryParse(menuItem2.SList[menuItem2.SelectedIndex], out Chance);
            var WCombo = Option.Item("WCombo").GetValue<bool>();
            var ECombo = Option.Item("ECombo").GetValue<bool>();


            if (Q.IsReady())
            {
                switch (AMode)
                {
                    case AimMode.HitChance:
                        Q.CastIfHitchanceEquals(GetQTarget(), Chance, false);
                        break;
                    case AimMode.Normal:
                        Q.Cast(PreCastPos(GetQTarget(), (QsDelay / 1000), Q.Width, varRange));
                        break;
                }
            }

            if (WCombo && W.IsReady())
            {
                switch (AMode)
                {
                    case AimMode.HitChance:
                        W.CastIfHitchanceEquals(GetWTarget(), Chance, false);
                        break;
                    case AimMode.Normal:
                        W.Cast(PreCastPos(GetWTarget(), 0.1f, 0 , 0));
                        break;
                }
            }

            if (ECombo && E.IsReady())
            {
                if (GetETarget() != null && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                {
                    E.Cast();
                }
                else
                {
                    if (GetETarget() == null && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2)
                    {
                        E.Cast();
                    }
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

            if (Q.IsReady())
            {
                Q.Cast(PreCastPos(GetQTarget(), (QsDelay / 1000), Q.Width, varRange));
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
            if (E.IsReady())
            {
                if (mob.IsValidTarget(E.Range) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                {
                    E.Cast();
                }
                else
                {
                    if (!mob.IsValidTarget(E.Range) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2)
                    {
                        E.Cast();
                    }
                }
            }

        }

#endregion

#region Farm
        public static void WaveClear()
        {
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

            if (!mobs.Any())
                return;

            var mob = mobs.First();

            if (Q.IsReady() && mob.IsValidTarget(Q.Range))
            {
                Q.Cast(mob.ServerPosition);
            }
            /*
            if (E.IsReady())
            {
                if (mob.IsValidTarget(E.Range) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1)
                {
                    E.Cast();
                }
                else
                {
                    if (!mob.IsValidTarget(E.Range) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).ToggleState == 2)
                    {
                        E.Cast();
                    }
                }
            }

            /*
            var allMinionQ1 = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All, MinionTeam.Enemy).Where(x => x.Health <= Player.GetSpellDamage(x, SpellSlot.Q)).ToList();
            var allMinionQ2 = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All, MinionTeam.Enemy).Where(x => x.Health <= (Player.GetSpellDamage(x, SpellSlot.Q) * 2)).ToList();
            if (!Orbwalking.CanMove(40)) return;

            if (!allMinionQ1.Any() || !allMinionQ2.Any())
                return;

            if (Q.IsReady())
            {

                var FLQ1 = Q.GetCircularFarmLocation(allMinionQ1, Q.Width);
                var FLQ2 = Q.GetCircularFarmLocation(allMinionQ2, Q.Width);

                if (FLQ2.MinionsHit == 1 && Player.Distance(FLQ2.Position) < (Q.Range + Q.Width))
                {
                    Q.Cast(FLQ2.Position);
                    return;
                }
                else
                    if (FLQ1.MinionsHit >= 2 && Player.Distance(FLQ1.Position) < (Q.Range + Q.Width))
                    {
                        Q.Cast(FLQ1.Position);
                        return;
                    }
            }*/

        }

#endregion

#region Lasthit

        public static void Freeze()
        {

            var allMinionQ1 = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All, MinionTeam.Enemy).Where(x => x.Health <= Player.GetSpellDamage(x, SpellSlot.Q)).ToList();
            var allMinionQ2 = MinionManager.GetMinions(Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All, MinionTeam.Enemy).Where(x => x.Health <= (Player.GetSpellDamage(x, SpellSlot.Q) * 2)).ToList();

            if (!Orbwalking.CanMove(40)) return;

            if (!allMinionQ1.Any() || !allMinionQ2.Any())
                return;

            if (Q.IsReady())
            {

                var FLQ1 = Q.GetCircularFarmLocation(allMinionQ1, Q.Width);
                var FLQ2 = Q.GetCircularFarmLocation(allMinionQ2, Q.Width);

                if (FLQ2.MinionsHit == 1 && Player.Distance(FLQ2.Position) < (Q.Range + Q.Width))
                {
                    Q.Cast(FLQ2.Position);
                    return;
                }
                else
                    if (FLQ1.MinionsHit >= 2 && Player.Distance(FLQ1.Position) < (Q.Range + Q.Width))
                    {
                        Q.Cast(FLQ1.Position);
                        return;
                    }
            }

        }

#endregion
        static void Game_OnWndProc(WndEventArgs args)
        {
            if (MenuGUI.IsChatOpen)
                return;

            if (args.Msg == (uint)WindowsMessages.WM_LBUTTONDOWN)
            {

                MainTarget =
                    HeroManager.Enemies
                        .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000)
                        .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault(); ;
            }

        }


        public static Vector2 PositionAfter(Obj_AI_Base unit, float t, float speed = float.MaxValue)
        {
            var distance = t * speed;
            var path = unit.GetWaypoints();

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

        public static Vector3 PreCastPos(AIHeroClient Hero, float Delay, float Range, float varRange)
        {
            float value = 0f;
            if (Hero.IsFacing(Player))
            {
                value = (Range - varRange - Hero.BoundingRadius);
            }
            else
            {
                value = (Range - varRange - Hero.BoundingRadius);
            }
            var distance = Delay * Hero.MoveSpeed + value;
            var path = Hero.GetWaypoints();

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

        public static void Interceptiontest(AIHeroClient Enemy, float delay, float Range, float varRange)
        {
            Geometry.Polygon.Circle Qspellpoly = new Geometry.Polygon.Circle(LastQCastpos, Q.Width);
            Qspellpoly.Draw(System.Drawing.Color.Khaki);

            Paths subjs = new Paths();
            foreach (var Waypoint in WPPolygon(Enemy, delay).ToPolygons())
            {
                subjs.Add(Waypoint.ToClipperPath());
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

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
                LastQCastpos = args.StartPosition;
        }

#region Draw

        private static void OnDraw(EventArgs args)
        {
            var DrawQ = Option.Item("DrawQ").GetValue<bool>();
            var DrawP = Option.Item("DrawP").GetValue<bool>();
            var DrawR = Option.Item("DrawR").GetValue<bool>();
            varRange = Option.Item("QsvarRange").GetValue<Slider>().Value;
            QsDelay = (Option.Item("QsDelay").GetValue<Slider>().Value);

            try
            {
                if (DrawP)
                {
                    foreach (var enemy in Targets)
                    {
                        if (enemy.IsVisible && !enemy.IsDead)
                        {
                            Render.Circle.DrawCircle(PreCastPos(enemy, (QsDelay / 1000), Q.Width, varRange), Q.Width, System.Drawing.Color.Red);
                            Render.Circle.DrawCircle(PreCastPos(enemy, 0.625f - (QsDelay / 1000), Q.Width, varRange), Q.Width, System.Drawing.Color.Green);
                            foreach (var Waypoint in WPPolygon(enemy, (QsDelay / 1000)).ToPolygons())
                            {
                                Waypoint.Draw(System.Drawing.Color.White);
                            }
                        }
                    }
                    Drawing.DrawText(100, 210, System.Drawing.Color.White, "Q Delay:" + (QsDelay / 1000).ToString("N3"));
                    Drawing.DrawText(100, 220, System.Drawing.Color.White, "Q Width:" + Q.Width.ToString());
                    Drawing.DrawText(100, 230, System.Drawing.Color.Red, "Q Castposition Color");
                    Drawing.DrawText(100, 240, System.Drawing.Color.Green, "Q Explode Color");
                    Drawing.DrawText(100, 250, System.Drawing.Color.White, "Enemy Waypoint + Predicted Position Color");

                }
                if (MainTarget != null && MainTarget.IsVisible)
                {
                    Render.Circle.DrawCircle(MainTarget.ServerPosition, 100, System.Drawing.Color.Red);
                }

                if (DrawQ)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Khaki);
                }
                if (DrawR && R.IsReady() && GetRTarget() != null)
                {
                    for (var i = 0; i <= GetRTarget().Count(); i++)
                        Drawing.DrawText(100, 100 + 10 * i, System.Drawing.Color.White, GetRTarget().ElementAt(i).CharData.BaseSkinName);
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
