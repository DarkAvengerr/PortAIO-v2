using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;
using static LeagueSharp.Common.Items;
using System.Drawing;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LordsCamille
{
    class Program
    {
        public static void Main()
        {
            Game_OnGameUpdate(new EventArgs());
        }

        public static AIHeroClient p;

        private static string News = "Welcome to Lord's Camille";

        public static AIHeroClient Player = ObjectManager.Player;

        public static Spell Q, W, E, R;

        public static Menu CamilleMenu;

        public static Orbwalking.Orbwalker orbwalker;

        internal static bool OnWall => Player.HasBuff("camilleedashtoggle") || E.Instance.Name != "CamilleE";

        internal static bool IsDashing => Player.HasBuff("camilleedash" + "1") || Player.HasBuff("camilleedash" + "2");

        public static void Game_OnGameUpdate(EventArgs args)

        {
            if (Player.ChampionName != "Camille")
            {
                return;
            }
            //Spells
            Q = new Spell(SpellSlot.Q, 135f);
            W = new Spell(SpellSlot.W, 625f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 375f);

            E.SetSkillshot(0.3f, 30, 500, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.195f, 100, 1750, false, SkillshotType.SkillshotCone);

            Chat.Print("<font size='30'>Lord's Camille</font> <font color='#b756c5'>by LordZEDith</font>");
            Chat.Print("<font color='#b756c5'>NEWS: </font>" + Program.News);

            //Events
            MainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_Update;
        }

        public static void Game_Update(EventArgs args)
        {
            //  if (Player.IsDead) return;
            //Activates Combo
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:

                    Combos();

                    break;


                //Activates Laneclear
                case Orbwalking.OrbwalkingMode.LaneClear:

                    Laneclear();

                    break;


            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            //Draw W
            if (CamilleMenu.Item("DrawW").GetValue<bool>() && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, 630, Color.Black);
            }

            //Draw E
            if (CamilleMenu.Item("DrawE").GetValue<bool>() && E.IsReady() && !Player.HasBuff("CamilleEDash1") && !Player.HasBuff("CamilleEOnWall") && !Player.HasBuff("camilleedashtoggle"))
            {
                Render.Circle.DrawCircle(Player.Position, 900, Color.BlueViolet);
            }

            if (CamilleMenu.Item("DrawR").GetValue<bool>() && R.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, 475, Color.Red);
            }
        }





        public static void Combos()
        {
            //Target
            var Target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            //Q Combo
            if (CamilleMenu.Item("Q").GetValue<bool>() && Q.IsReady() && Target.IsValidTarget() && CamilleMenu.Item("Q2").GetValue<bool>() && !Player.HasBuff("CamilleQPrimingStart"))
            {
                Q.Cast(Target);
            }

            if (CamilleMenu.Item("Q").GetValue<bool>() && Q.IsReady() && Target.IsValidTarget() && !CamilleMenu.Item("Q2").GetValue<bool>())
            {
                Q.Cast(Target);
            }

            //W Combo
            if (CamilleMenu.Item("W").GetValue<bool>() && W.IsReady() && Target.IsValidTarget() && !CamilleMenu.Item("W2").GetValue<bool>() && !Player.HasBuff("CamilleEOnWall") && !Player.HasBuff("CamilleEDash1") && !Player.HasBuff("camilleedashtoggle") && !Player.HasBuff("CamilleEDash2"))
            {
                W.Cast(Target);
            }
            if (CamilleMenu.Item("W").GetValue<bool>() && W.IsReady() && Target.IsValidTarget() && CamilleMenu.Item("W2").GetValue<bool>() && Player.Distance(Target) <= 630 && Player.Distance(Target) >= 350 && !Player.HasBuff("CamilleEOnWall") && !Player.HasBuff("camilleedashtoggle") && !Player.HasBuff("CamilleEDash2"))
            {
                W.Cast(Target);
            }

            //E Combo                
            if (E.IsReady() && CamilleMenu.Item("E").GetValue<bool>() && Target.IsValidTarget(E.Range))
            {
                switch (CamilleMenu.Item("EMode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        {
                            var polygon = new Geometry.Polygon.Circle(Target.Position, 600).Points.FirstOrDefault(x => NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Building));

                            if (new Vector2(polygon.X, polygon.Y).Distance(ObjectManager.Player.Position) < E.Range)
                            {
                                E.Cast(new Vector2(polygon.X, polygon.Y));
                                if (E.Cast(new Vector2(polygon.X, polygon.Y)))
                                {
                                    E.Cast(Target);
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            var polygon = new Geometry.Polygon.Circle(Target.Position, 600).Points.FirstOrDefault(x => NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Building));

                            if (new Vector2(polygon.X, polygon.Y).Distance(ObjectManager.Player.Position) < E.Range)
                            {
                                E.Cast(new Vector2(polygon.X, polygon.Y));

                            }
                        }
                        break;
                    case 2:
                        {
                            var target = TargetSelector.GetTarget(E.IsReady() ? E.Range * 2 : W.Range, TargetSelector.DamageType.Physical);
                            if (target.IsValidTarget() && !target.IsZombie)
                            {
                                if (CamilleMenu.Item("E").GetValue<bool>())
                                    UseE(target.ServerPosition);
                            }
                        }
                        break;
                }
            }

            //R Combo
            if (CamilleMenu.Item("R").GetValue<bool>() && R.IsReady() && Target.IsValidTarget() && !Player.HasBuff("CamilleEOnWall") && !Player.HasBuff("CamilleEDash1") && !Player.HasBuff("camilleedashtoggle") && !Player.HasBuff("CamilleEDash2") && Target.HealthPercent < CamilleMenu.Item("RHP").GetValue<Slider>().Value && CamilleMenu.Item("r" + Target.ChampionName.ToLower(), true).GetValue<bool>())
            {
                R.Cast(Target);
            }

        }

        public static void Laneclear()
        {

            if (Q.IsReady() && CamilleMenu.Item("WLane").GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, W.Range, MinionTypes.All,
                MinionTeam.NotAlly);

                var minioncount = W.GetLineFarmLocation(minions);
                if (minions == null || minions.Count == 0)
                {
                    return;
                }

                if (minioncount.MinionsHit >= CamilleMenu.Item("Min").GetValue<Slider>().Value)
                {
                    W.Cast(minioncount.Position);
                }
            }
        }



        public static void MainMenu()
        {
            //MainMenu
            CamilleMenu = new Menu("Lord's Camille", "Lord's Camille", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.SkyBlue);
            CamilleMenu.AddToMainMenu();

            //Orbwalker Menu
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            CamilleMenu.AddSubMenu(orbwalkerMenu);

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            CamilleMenu.AddSubMenu(targetSelectorMenu);

            //Combo
            var Combo = new Menu("Combo", "Combo");
            {
                Combo.AddItem(new MenuItem("Combo Settings:", "Combo Settings:"));
                Combo.AddItem(new MenuItem("Q", "Use Q in Combo")).SetValue(true);
                Combo.AddItem(new MenuItem("Q2", "Use Q2 true DMG")).SetValue(true);
                Combo.AddItem(new MenuItem("W", "Use W in Combo")).SetValue(true);
                Combo.AddItem(new MenuItem("W2", "Use W Outer Range")).SetValue(true);
                Combo.AddItem(new MenuItem("EMode", "Use E Mode:", true).SetValue(new StringList(new[] { "Hiki Edited", "Hiki" })));
                Combo.AddItem(new MenuItem("E", "Use E in Combo")).SetValue(true);
                Combo.AddItem(new MenuItem("R", "Use R in Combo")).SetValue(true);
                Combo.AddItem(new MenuItem("RHP", "Use R if Enemy % HP").SetValue(new Slider(50)));

            }

            CamilleMenu.AddSubMenu(Combo);

            //Ultimate Menu
            var whitelist = new Menu("Ultimate Whitelist", "Ultimate Whitelist");
            {
                foreach (var hero in HeroManager.Enemies)
                {
                    whitelist.AddItem(new MenuItem("r" + hero.ChampionName.ToLower(), "Use [R]:  " + hero.ChampionName, true).SetValue(
                            true));
                }

            }
            CamilleMenu.AddSubMenu(whitelist);
            //Clear
            var Clear = new Menu("Lane Clear", "Lane Clear");
            {
                Clear.AddItem(new MenuItem("Laneclear Settings:", "Laneclear Settings:"));
                //Clear.AddItem(new MenuItem("QLane", "Use Q in Laneclear")).SetValue(false);
                // Clear.AddItem(new MenuItem("QLane2", "Use Q2 in Laneclear")).SetValue(false);
                Clear.AddItem(new MenuItem("WLane", "Use W in Laneclear")).SetValue(true);
                Clear.AddItem(new MenuItem("Min", "[W] Min. Minion Count").SetValue(new Slider(3, 1, 5)));
            }
            CamilleMenu.AddSubMenu(Clear);



            //DrawMenu
            var DrawMenu = new Menu("Drawings", "Drawings");
            {
                DrawMenu.AddItem(new MenuItem("Draw Settings:", "Draw Settings:"));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range")).SetValue(true);
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range")).SetValue(true);
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range")).SetValue(true);

            }
            CamilleMenu.AddSubMenu(DrawMenu);

        }
        static void UseE(Vector3 p, bool combo = true)
        {
            try
            {
                if (IsDashing || OnWall || !E.IsReady())
                {
                    return;
                }

                
                var posChecked = 0;
                var maxPosChecked = 80;
                var posRadius = 145;
                var radiusIndex = 0;

                var candidatePos = new List<Vector2>();

                while (posChecked < maxPosChecked)
                {
                    radiusIndex++;

                    var curRadius = radiusIndex * (0x2 * posRadius);
                    var curCurcleChecks = (int)Math.Ceiling((2 * Math.PI * curRadius) / (2 * (double)posRadius));

                    for (var i = 1; i < curCurcleChecks; i++)
                    {
                        posChecked++;

                        var cRadians = (0x2 * Math.PI / (curCurcleChecks - 1)) * i;
                        var xPos = (float)Math.Floor(p.X + curRadius * Math.Cos(cRadians));
                        var yPos = (float)Math.Floor(p.Y + curRadius * Math.Sin(cRadians));

                        var desiredPos = new Vector2(xPos, yPos);



                        if (desiredPos.IsWall())
                        {
                            candidatePos.Add(desiredPos);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
