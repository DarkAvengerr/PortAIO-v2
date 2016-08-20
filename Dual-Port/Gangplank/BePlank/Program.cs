/**
 * Coded by Brikovich
 * Assembly Skeleton by GoldenGates
 * Lot of code copied from baballev & Soresu GP assemblies (please don't sue me KappaHD)
 * Shouts to Trees, Asuna, jQuery and others who helped me :)
 * First assembly don't judge KappaPride
 * The code is (very) dirty will clean up later
 * I'm aware of range correction bugs and i'll try to fix them
 **/
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
namespace BePlank
{
    class Program
    {

        #region Declaration
        static Spell Q, W, E, R;
        public static List<Barrel> savedBarrels = new List<Barrel>();
        static double mouseToClosestBarrel;
        public static double maxSearchRange = 900;
        public static int correctionRange = 1000;
        public static int potentielRange = 1000;
        public const int BarrelConnectionRange = 340;
        public const int UltRadius = 500;
        public const int DeathDaughterRadius = 200;
        static Vector3 blockPos;
        static Orbwalking.Orbwalker Orbwalker;
        static Menu Menu;
        private static Vector2 PingLocation;
        static bool isEQ = false;
        private static int LastPingT = 0;
        public static bool ECasted;
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static string CHAMPION_NAME = "Gangplank";
        #endregion

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != CHAMPION_NAME)
                return;
            Chat.Print("BePlank by Brikovich loaded - Credits to baballev & Soresu");

            #region Spells
            Q = new Spell(SpellSlot.Q, 610);
            Q.SetTargetted(0.25f, 2150f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 980);
            R = new Spell(SpellSlot.R);
            R.SetSkillshot(0.9f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
            #endregion



            #region Menu
            Menu = new Menu("BePlank", Player.ChampionName, true);

            Menu OrbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);

            Menu TargetSelectorMenu = Menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
            TargetSelector.AddToMenu(TargetSelectorMenu);


            Menu DrawingMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawing"));
            DrawingMenu.AddItem(new MenuItem("DrawAA", "Draw AA Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawERadius", "Draw E Raduis").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawEConnection", "Draw E Connection Line").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawEConnectionRadius", "Draw E Connection Circles").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawEPrediction", "Draw E Predicted Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawR", "Draw R Radius").SetValue(false));
            DrawingMenu.AddItem(new MenuItem("OnReady", "Draw only if Ready").SetValue(true));


            Menu.AddItem(new MenuItem("Corrector", "Connection correction [BETA]").SetTooltip("If E connection miss will try to cast E on the lastest succesfull position").SetValue(true));
            Menu.AddItem(new MenuItem("CastQ", "Quick Q detonate nearest (health decay support)").SetValue(new KeyBind('A', KeyBindType.Press, false)));
            Menu.AddItem(new MenuItem("CastEQ", "Quick cast EQ at mouse (first barrel manual)").SetValue(new KeyBind('T', KeyBindType.Press, false)));

            Menu.AddItem(new MenuItem("Ping", "Ping on low hp (local)").SetValue(true));
            Menu.AddItem(new MenuItem("KS", "Q KillSecure").SetValue(true));
            Menu.AddItem(new MenuItem("Qlasthit", "Q last hit toggle").SetValue(new KeyBind('K', KeyBindType.Toggle, false)));

            var cleanserManagerMenu = new Menu("W cleanser - By baballev", "cleanserManager");
            cleanserManagerMenu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("separation1", ""));
            cleanserManagerMenu.AddItem(new MenuItem("separation2", "Buff Types: "));
            cleanserManagerMenu.AddItem(new MenuItem("charm", "Charm").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("flee", "Flee").SetTooltip("Fear").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("polymorph", "Polymorph").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("snare", "Snare").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("stun", "Stun").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("taunt", "Taunt").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("exhaust", "Exhaust").SetTooltip("Will only remove Slow").SetValue(false));
            cleanserManagerMenu.AddItem(new MenuItem("suppression", "Supression").SetValue(true));

            Menu.AddSubMenu(cleanserManagerMenu);
            Menu.AddToMainMenu();
            #endregion



            #region Subscriptions
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += Game_OnCreate;
            Spellbook.OnCastSpell += Game_OnCastSpell;
            #endregion

        }

        static void Game_OnUpdate(EventArgs args)
        {

            //Remove barrelss, onDelete have huge delay
            for (int i = 0; i < savedBarrels.Count; i++)
            {
                if (savedBarrels[i].barrel.Health != 1 && savedBarrels[i].barrel.Health != 2 && savedBarrels[i].barrel.Health != 3)
                {
                    savedBarrels.RemoveAt(i);
                    return;
                }
            }


            if (Menu.SubMenu("cleanserManager").Item("enabled").GetValue<bool>())
            {
                CleanserManager();
            }

            if (Menu.Item("CastEQ").GetValue<KeyBind>().Active || Menu.Item("CastQ").GetValue<KeyBind>().Active)
            {

                if (Menu.Item("CastEQ").GetValue<KeyBind>().Active) isEQ = true;
                else isEQ = false;
                Barrel myQTarget = NearestExpBarrelToMouse();

                //Kreygasm
                if (!ECasted)
                {
                    if (Player.Level >= 7 && Player.Level < 13)
                    {
                        var time = 2f * 1000;
                        var kappaHD = Environment.TickCount - myQTarget.time + (Player.Distance(myQTarget.barrel) / 2800f + Q.Delay) * 1000;

                        if (time < kappaHD)
                        {
                            if (isEQ)
                            {
                                ECasted = true;
                                if (myQTarget.barrel.Distance(Game.CursorPos) > BarrelConnectionRange * 2) E.Cast(correctThisPosition(Game.CursorPos.To2D(), myQTarget));
                                else E.Cast(Game.CursorPos);
                                Q.CastOnUnit(myQTarget.barrel);

                            }
                            else Q.CastOnUnit(myQTarget.barrel);



                        }

                    }
                    else if (Player.Level >= 13)
                    {

                        var time = 1f * 1000;
                        var kappaHD = Environment.TickCount - myQTarget.time + (Player.Distance(myQTarget.barrel) / 2800f + Q.Delay) * 1000;

                        if (time < kappaHD)
                        {
                            if (isEQ)
                            {
                                ECasted = true;
                                if (myQTarget.barrel.Distance(Game.CursorPos) > BarrelConnectionRange * 2) E.Cast(correctThisPosition(Game.CursorPos.To2D(), myQTarget));
                                else E.Cast(Game.CursorPos);
                                Q.CastOnUnit(myQTarget.barrel);

                            }
                            else Q.CastOnUnit(myQTarget.barrel);


                        }
                    }
                    else
                    {
                        var time = 4f * 1000;
                        var kappaHD = Environment.TickCount - myQTarget.time + (Player.Distance(myQTarget.barrel) / 2800f + Q.Delay) * 1000;

                        if (time < kappaHD)
                        {
                            if (isEQ)
                            {
                                ECasted = true;
                                if (myQTarget.barrel.Distance(Game.CursorPos) > BarrelConnectionRange * 2) E.Cast(correctThisPosition(Game.CursorPos.To2D(), myQTarget));
                                else E.Cast(Game.CursorPos);
                                Q.CastOnUnit(myQTarget.barrel);

                            }
                            else Q.CastOnUnit(myQTarget.barrel);


                        }
                    }
                }


            }
            else
            {
                ECasted = false;
                isEQ = false;

            }

            //Last hit
            if (Menu.Item("Qlasthit").GetValue<KeyBind>().Active && !Menu.Item("CastQ").GetValue<KeyBind>().Active && !Menu.Item("CastEQ").GetValue<KeyBind>().Active && Q.IsReady())
            {

                var mini =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(m => m.Health < Q.GetDamage(m) && m.BaseSkinName != "GangplankBarrel")
                        .OrderByDescending(m => m.MaxHealth)
                        .ThenByDescending(m => m.Distance(Player))
                        .FirstOrDefault();

                Q.CastOnUnit(mini);
            }

            //KS
            if (Menu.Item("KS").GetValue<bool>())
            {
                var kstarget = HeroManager.Enemies;
                if (kstarget != null)
                {
                    foreach (var ks in kstarget)
                    {
                        if (ks != null)
                        {
                            if (ks.Health <= Player.GetSpellDamage(ks, SpellSlot.Q) && ks.Health > 0 && Q.IsInRange(ks))
                            {

                                Q.CastOnUnit(ks);
                            }
                        }
                    }
                }
            }

            if (Menu.Item("Ping").GetValue<bool>())
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready &&
                                    h.IsValidTarget() && h.HealthPercent <= 20))
                {
                    Ping(enemy.Position.To2D());
                }


        }



        static void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("DrawAA").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Player.AttackRange, Color.MediumSeaGreen);

            if (!Menu.Item("OnReady").GetValue<bool>())
            {
                if (Menu.Item("DrawQ").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color.BlueViolet);
                if (Menu.Item("DrawE").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.IndianRed);
                    if (Menu.Item("DrawERadius").GetValue<bool>())
                        Render.Circle.DrawCircle(Game.CursorPos, BarrelConnectionRange, Color.Gray);
                }

                if (Menu.Item("DrawR").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Game.CursorPos, UltRadius, Color.Black);
                    Render.Circle.DrawCircle(Game.CursorPos, DeathDaughterRadius, Color.DarkKhaki);
                }
            }
            else
            {
                if (Menu.Item("DrawQ").GetValue<bool>() && Q.IsReady())
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color.BlueViolet);
                if (Menu.Item("DrawE").GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.IndianRed);
                    if (Menu.Item("DrawERadius").GetValue<bool>())
                        Render.Circle.DrawCircle(Game.CursorPos, BarrelConnectionRange, Color.Gray);
                }
                if (Menu.Item("DrawR").GetValue<bool>() && R.IsReady())
                {
                    Render.Circle.DrawCircle(Game.CursorPos, UltRadius, Color.Black);
                    Render.Circle.DrawCircle(Game.CursorPos, DeathDaughterRadius, Color.DarkKhaki);
                }
            }

            //Draw connection line
            mouseToClosestBarrel = NearestBarrelToMouse().barrel.Distance(Game.CursorPos);
            //In connection range


            if (mouseToClosestBarrel <= BarrelConnectionRange * 2)
            {

                if (E.IsReady())
                {
                    if (Menu.Item("DrawEConnection").GetValue<bool>())
                        Drawing.DrawLine(Drawing.WorldToScreen(Game.CursorPos), Drawing.WorldToScreen(NearestBarrelToMouse().barrel.Position), 4, Color.DarkGreen);
                    if (Menu.Item("DrawEConnectionRadius").GetValue<bool>())
                        Render.Circle.DrawCircle(NearestBarrelToMouse().barrel.Position, BarrelConnectionRange, Color.DarkGreen);
                    if (Menu.Item("DrawEPrediction").GetValue<bool>())
                        Render.Circle.DrawCircle(NearestBarrelToMouse().barrel.Position, potentielRange, Color.BlanchedAlmond);
                    Render.Circle.DrawCircle(Game.CursorPos, BarrelConnectionRange, Color.Green);
                }
            }
            //Out of connection range & in search range
            else if (mouseToClosestBarrel > BarrelConnectionRange && mouseToClosestBarrel < maxSearchRange && E.IsReady())
            {
                if (Menu.Item("DrawEConnection").GetValue<bool>())
                    Drawing.DrawLine(Drawing.WorldToScreen(Game.CursorPos), Drawing.WorldToScreen(NearestBarrelToMouse().barrel.Position), 4, Color.Red);
                if (Menu.Item("DrawEConnectionRadius").GetValue<bool>())
                    Render.Circle.DrawCircle(NearestBarrelToMouse().barrel.Position, BarrelConnectionRange, Color.Red);
                if (Menu.Item("DrawEPrediction").GetValue<bool>())
                    Render.Circle.DrawCircle(NearestBarrelToMouse().barrel.Position, potentielRange, Color.BlanchedAlmond);
            }


        }

        static void Game_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                savedBarrels.Add(new Barrel(sender as Obj_AI_Minion, System.Environment.TickCount));
            }
        }
        static void Game_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.E && Menu.Item("Corrector").GetValue<bool>())
            {
                if (mouseToClosestBarrel > BarrelConnectionRange * 2 && mouseToClosestBarrel < maxSearchRange && correctThisPosition(Game.CursorPos.To2D(), closestToPosition(Game.CursorPos)).Distance(Game.CursorPos) <= correctionRange && savedBarrels.Count > 0 && !isEQ)
                {
                    args.Process = false;
                    Spellbook.OnCastSpell -= Game_OnCastSpell;
                    E.Cast(correctThisPosition(Game.CursorPos.To2D(), closestToPosition(Game.CursorPos)));
                    Spellbook.OnCastSpell += Game_OnCastSpell;

                }
            }
        }
        private static bool GetBool(string name)
        {
            return Menu.Item(name).GetValue<bool>();
        }
        private static void CleanserManager()
        {
            // List of disable buffs
            if
                (W.IsReady() && (
                (Player.HasBuffOfType(BuffType.Charm) && Menu.SubMenu("cleanserManager").Item("charm").GetValue<bool>())
                || (Player.HasBuffOfType(BuffType.Flee) && Menu.SubMenu("cleanserManager").Item("flee").GetValue<bool>())
                || (Player.HasBuffOfType(BuffType.Polymorph) && Menu.SubMenu("cleanserManager").Item("polymorph").GetValue<bool>())
                || (Player.HasBuffOfType(BuffType.Snare) && Menu.SubMenu("cleanserManager").Item("snare").GetValue<bool>())
                || (Player.HasBuffOfType(BuffType.Stun))
                || (Player.HasBuffOfType(BuffType.Taunt) && Menu.SubMenu("cleanserManager").Item("taunt").GetValue<bool>())
                || (Player.HasBuff("summonerexhaust") && Menu.SubMenu("cleanserManager").Item("exhaust").GetValue<bool>())
                || (Player.HasBuffOfType(BuffType.Suppression) && Menu.SubMenu("cleanserManager").Item("suppression").GetValue<bool>())
                ))

            {
                W.Cast();
            }
        }

        #region BarrelsSection
        internal class Barrel
        {
            public Obj_AI_Minion barrel;
            public float time;

            public Barrel(Obj_AI_Minion objAiBase, int tickCount)
            {
                barrel = objAiBase;
                time = tickCount;
            }
        }
        //return nearest barrel to mouse
        private static Barrel NearestBarrelToMouse()
        {
            var pos = Game.CursorPos.To2D();
            if (savedBarrels.Count == 0)
            {
                return null;
            }
            return savedBarrels.OrderBy(k => k.barrel.ServerPosition.Distance(pos.To3D())).FirstOrDefault();
        }

        private static Barrel NearestExpBarrelToMouse()
        {
            var pos = Game.CursorPos.To2D();
            if (savedBarrels.Count == 0)
            {
                return null;
            }
            savedBarrels.OrderBy(k => k.barrel.ServerPosition.Distance(pos.To3D()));

            return savedBarrels.OrderBy(k => k.barrel.Health).Where(k => k.barrel.Distance(Player) <= Q.Range).FirstOrDefault();

        }

        #endregion BarrelSection

        public static void Main()
        {
            Game_OnGameLoad();
        }

        //ping
        private static void Ping(Vector2 position)
        {
            if (Utils.TickCount - LastPingT < 30 * 1000)
            {
                return;
            }

            LastPingT = Utils.TickCount;
            PingLocation = position;
            SimplePing();

            LeagueSharp.Common.Utility.DelayAction.Add(150, SimplePing);
            LeagueSharp.Common.Utility.DelayAction.Add(300, SimplePing);

        }

        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.Fallback, PingLocation, true);
        }

        //Correct given position so it will connect to barrel to that position at max range

        public static Vector2 correctThisPosition(Vector2 position, Barrel barrelToConnect)
        {
            double vX = position.X - barrelToConnect.barrel.Position.X;
            double vY = position.Y - barrelToConnect.barrel.Position.Y;
            double magV = Math.Sqrt(vX * vX + vY * vY);
            double aX = Math.Round(barrelToConnect.barrel.Position.X + vX / magV * 680);
            double aY = Math.Round(barrelToConnect.barrel.Position.Y + vY / magV * 680);
            Vector2 newPosition = new Vector2(Convert.ToInt32(aX), Convert.ToInt32(aY));
            return newPosition;
        }


        //Return closest barrel to a position
        public static Barrel closestToPosition(Vector3 position)
        {
            if (savedBarrels.Count() == 0)
                return null;
            Barrel closest = null;
            float bestSoFar = -1;


            for (int i = 0; i < savedBarrels.Count; i++)
            {
                if (bestSoFar == -1 || savedBarrels[i].barrel.Distance(position) < bestSoFar)
                {
                    bestSoFar = savedBarrels[i].barrel.Distance(position);
                    closest = savedBarrels[i];
                }
            }
            return closest;
        }

    }




}