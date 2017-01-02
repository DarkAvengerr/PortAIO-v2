using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Menu
{
    using myCommon;
    using LeagueSharp.Common;
    

    internal class MenuManager : Logic
    {
        internal static void Init()
        {
            Menu = new Menu("mySeries: " + Me.ChampionName, "mySeries: " + Me.ChampionName, true);

            var targetSelectMenu = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            {
                TargetSelector.AddToMenu(targetSelectMenu);
            }

            var orbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            }

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboMode", "Combo Mode: ", true).SetValue(new StringList(new[] {"QEW", "EQW"})));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRalways", "Use R| Always Cast", true).SetValue(false));
                comboMenu.AddItem(new MenuItem("ComboRcanKill", "Use R| Can Killable Target", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRhitCount", "Use R| Hit Count Ememies (AOE)", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRhitCountcount", "Use R| Hit Count Ememies >= x", true).SetValue(new Slider(3, 1,
                        5)));
                comboMenu.AddItem(new MenuItem("ComboDot", "Use Ignite", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMode", "Harass Mode: ", true).SetValue(new StringList(new[] {"QEW", "EQW"})));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearQLH", "Use Q|Only Last Hit", true).SetValue(false));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearWCount", "Use W| Hit Count >= x", true).SetValue(new Slider(3, 1, 10)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var lastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                lastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q", true).SetValue(true));
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealCancel", "Cancel Ult", true).SetValue(true));
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var eSettings = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eSettings.AddItem(new MenuItem("LogicE", "Enabled Logic E(Not work in EQW Mode)", true).SetValue(true));
                    eSettings.AddItem(new MenuItem("Humanizer", "Enabled Humanizer", true).SetValue(false));
                    eSettings.AddItem(new MenuItem("HumanizerD", "Humanizer Delay", true).SetValue(new Slider(0, 0, 1000)));
                    eSettings.AddItem(
                        new MenuItem("Eturret", "Dont Use E to Turret", true).SetValue(
                            new StringList(new[] {"Always", "Smart", "Off"}, 1)));
                    eSettings.AddItem(
                        new MenuItem("EturretHP", "Smart Mode| When Player HealthPercent", true).SetValue(new Slider(50))
                            .SetTooltip(
                                "just when player HealthPercent <= Settings, will enabled my setting, if High than percent it will can dash to turret"));
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChange", "SkinChange"));
                {
                    SkinManager.AddToMenu(skinMenu, 9);
                }

                var autoLevelMenu = miscMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }

                var itemsMeun = miscMenu.AddSubMenu(new Menu("Items", "Items"));
                {
                    ItemManager.AddToMenu(itemsMeun);
                }

                miscMenu.AddItem(
                    new MenuItem("AutoCancel", "Auto Cancel Ult", true).SetValue(true)
                        .SetTooltip("When No Ememies in R range auto cancel r"));
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                //DamageIndicator.AddToMenu(drawMenu, DamageCalculate.GetComboDamage);
            }

            Menu.AddItem(new MenuItem("asd ad asd ", " ", true));
            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();
        }
    }
}
