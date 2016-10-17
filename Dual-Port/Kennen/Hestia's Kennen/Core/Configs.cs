using System;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
namespace Kennen.Core
{
    internal class Configs
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static bool SpredLoaded;

        public static void Initialize()
        {
            try
            {
                config = new Menu(ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);

                var orbwalkerMenu = config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                orbwalker.RegisterCustomMode("flee", "Flee", (uint) Keys.Z);

                var combo = config.AddSubMenu(new Menu("Combo Settings", "Combo"));
                var comboQ = combo.AddSubMenu(new Menu("Q Settings", "Q"));
                comboQ.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));

                var comboW = combo.AddSubMenu(new Menu("W Settings", "W"));
                comboW.AddItem(new MenuItem("useW", "Use W").SetValue(true));
                comboW.AddItem(
                    new MenuItem("useWmodeCombo", "W Mode").SetValue(new StringList(new[] {"Always", "Only Stunnable"})));
                //var comboR = combo.AddSubMenu(new Menu("R Settings", "R"));
                //comboR.AddItem(new MenuItem("useR", "Use R").SetValue(false));

                var harass = config.AddSubMenu(new Menu("Harass Settings", "Harass"));
                var harassQ = harass.AddSubMenu(new Menu("Q Settings", "Q"));
                harassQ.AddItem(new MenuItem("useQHarass", "Use Q").SetValue(true));
                harassQ.AddItem(new MenuItem("useQHarassMana", "Min energy to use Q:").SetValue(new Slider(30, 1)));
                var harassW = harass.AddSubMenu(new Menu("W Settings", "W"));
                harassW.AddItem(new MenuItem("useWHarass", "Use W").SetValue(true));
                harassW.AddItem(
                    new MenuItem("useWmodeHarass", "W Mode").SetValue(new StringList(new[] {"Always", "Only Stunnable"})));
                harassW.AddItem(new MenuItem("useWHarassMana", "Min energy to use W:").SetValue(new Slider(30, 1)));

                var misc = config.AddSubMenu(new Menu("Misc Settings", "Misc"));
                var miscQ = misc.AddSubMenu(new Menu("Q Settings", "Qmisc"));
                miscQ.AddItem(
                    new MenuItem("autoQ", "Auto Q on enemies").SetValue(
                        new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle)));
                miscQ.AddItem(
                    new MenuItem("hitchanceQ", "Global Q Hitchance").SetValue(
                        new StringList(
                            new[]
                            {
                                HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(),
                                HitChance.VeryHigh.ToString()
                            }, 3)));
                miscQ.AddItem(
                    new MenuItem("predMode", "Used Prediction").SetValue(
                        new StringList(new[] {"Common", "OKTW©"})));

                //var miscR = misc.AddSubMenu(new Menu("R Settings", "R"));
                //miscR.AddItem(new MenuItem("useRmul", "Use R for multiple targets").SetValue(true));
                //miscR.AddItem(new MenuItem("useRmulti", "Use R on min X targets").SetValue(new Slider(3, 1, 5)));

                var miscItems = misc.AddSubMenu(new Menu("Items Settings", "Items"));
                miscItems.AddItem(new MenuItem("useProto", "Use Hextech Protobelt-01").SetValue(true));

                var lastHitMenu = config.AddSubMenu(new Menu("LastHit", "LastHit"));
                lastHitMenu.AddItem(new MenuItem("useQlh", "Use Q to Last Hit minions").SetValue(true));
                lastHitMenu.AddItem(new MenuItem("qRange", "Only use Q if far from minions").SetValue(true));
                lastHitMenu.AddItem(new MenuItem("useQlhMana", "Min energy to use Q:").SetValue(new Slider(30, 1)));

                var laneClearMenu = config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                laneClearMenu.AddItem(new MenuItem("useQlc", "Q to LH in lane clear").SetValue(true));
                laneClearMenu.AddItem(new MenuItem("useQlcMana", "Min energy to use Q:").SetValue(new Slider(30, 1)));

                var jungleClearMenu = config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                jungleClearMenu.AddItem(new MenuItem("useQj", "Q in jungle clear").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("useWj", "Use W in jungle clear").SetValue(true));

                var killsteal = config.AddSubMenu(new Menu("KillSteal Settings", "KillSteal"));
                killsteal.AddItem(new MenuItem("killsteal", "Activate Killsteal").SetValue(true));
                killsteal.AddItem(new MenuItem("useQks", "Use Q to KillSteal").SetValue(true));
                killsteal.AddItem(new MenuItem("useWks", "Use W to KillSteal").SetValue(true));
                killsteal.AddItem(new MenuItem("useIks", "Use Ignite to KillSteal").SetValue(true));

                var flee = config.AddSubMenu(new Menu("Flee Settings", "Flee"));
                flee.AddItem(new MenuItem("qFlee", "Q while fleeing").SetValue(true));
                flee.AddItem(new MenuItem("wFlee", "W to stun during flee").SetValue(true));
                flee.AddItem(new MenuItem("eFlee", "E to flee").SetValue(true));

                var drawingMenu = config.AddSubMenu(new Menu("Drawings", "Drawings"));
                drawingMenu.AddItem(new MenuItem("disableDraw", "Disable all drawings").SetValue(true));
                drawingMenu.AddItem(
                    new MenuItem("drawQ", "Draw Q").SetValue(new Circle(false, Color.DarkOrange, Spells.Q.Range)));
                drawingMenu.AddItem(
                    new MenuItem("drawW", "Draw W").SetValue(new Circle(false, Color.DarkOrange, Spells.W.Range)));
                drawingMenu.AddItem(
                    new MenuItem("drawR", "Draw R").SetValue(new Circle(false, Color.DarkOrange, Spells.R.Range)));
                drawingMenu.AddItem(new MenuItem("width", "Drawings width").SetValue(new Slider(2, 1, 5)));

                config.AddItem(new MenuItem("spacer", ""));
                config.AddItem(new MenuItem("author", "Author: Hestia"));

                config.AddToMainMenu();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the menu - {0}", exception);
            }
        }
    }
}
