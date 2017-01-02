using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Menu
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
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRUnder", "Use R| Under Turret", true).SetValue(false));
                comboMenu.AddItem(new MenuItem("ComboSecondR", "Use Second R", true).SetValue(true))
                    .SetTooltip("Only Can Kill Target");
                comboMenu.AddItem(new MenuItem("ComboDot", "Use Ignite", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboMode", "Combo Mode?", true).SetValue(new StringList(new[] { "Q->R", "R->Q" })));
                comboMenu.AddItem(
                    new MenuItem("MisayaRange", "Min RQ Range >= x", true).SetValue(new Slider(500, 150, 825)));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("Harassmana", "When Player ManaPercent >= x%", true)
                    .SetValue(new Slider(50)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearQCount", "Use Q| Min Hit Count >= x", true)
                        .SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearWCount", "Use W| Min Hit Count >= x", true)
                        .SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(new MenuItem("LaneClearmana", "When Player ManaPercent >= x%", true)
                        .SetValue(new Slider(50)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearR", "Use R", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearmana", "When Player ManaPercent >= x%", true)
                        .SetValue(new Slider(30)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealRtarget", "Use R List", true));
                foreach (var target in HeroManager.Enemies)
                {
                    killStealMenu.AddItem(
                        new MenuItem("KillStealR" + target.ChampionName.ToLower(), target.ChampionName, true).SetValue(true));
                }
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var eSettings = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eSettings.AddItem(new MenuItem("EGap", "Use E Anti Gapcloser", true).SetValue(true));
                    eSettings.AddItem(new MenuItem("EInt", "Use E Interrupt Spell", true).SetValue(true));
                    eSettings.AddItem(
                        new MenuItem("EFlash", "E Flash Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChange", "SkinChange"));
                {
                    SkinManager.AddToMenu(skinMenu, 10);
                }

                var autoLevelMenu = miscMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }
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
