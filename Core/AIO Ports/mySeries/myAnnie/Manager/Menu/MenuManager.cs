using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Menu
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
                comboMenu.AddItem(new MenuItem("ComboRKill", "Use R| KillSteal", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRCount", "Use R| Hit Count >= x", true).SetValue(new Slider(3, 1, 5)));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassQLH", "Use Q|Last Hit", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearQLH", "Use Q|Only Last Hit", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearWCount", "Use W| Hit Count >= x", true).SetValue(new Slider(3, 1, 10)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var antiGapcloserMenu = miscMenu.AddSubMenu(new Menu("AntiGapcloser", "AntiGapcloser"));
                {
                    antiGapcloserMenu.AddItem(new MenuItem("AntiGapcloserQ", "Use Q", true).SetValue(true));
                    antiGapcloserMenu.AddItem(new MenuItem("AntiGapcloserW", "Use W", true).SetValue(true));
                    antiGapcloserMenu.AddItem(new MenuItem("AntiGapcloserE", "Use E", true).SetValue(true));
                }

                var interruptMenu = miscMenu.AddSubMenu(new Menu("Interrupt", "Interrupt"));
                {
                    interruptMenu.AddItem(new MenuItem("InterruptQ", "Use Q", true).SetValue(true));
                    interruptMenu.AddItem(new MenuItem("InterruptW", "Use W", true).SetValue(true));
                    interruptMenu.AddItem(new MenuItem("InterruptE", "Use E", true).SetValue(true));
                }

                var flashRMenu = miscMenu.AddSubMenu(new Menu("Flash R", "Flash R"));
                {
                    flashRMenu.AddItem(
                        new MenuItem("EnableFlashR", "FlashR Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
                    flashRMenu.AddItem(
                        new MenuItem("FlashRCountsEnemiesinRange", "Min Hit Enemies Counts >= ", true).SetValue(
                            new Slider(3, 1, 5)));
                    flashRMenu.AddItem(
                        new MenuItem("FlashRCountsAlliesinRange", "And Min Allies Counts >= (0 = off)", true).SetValue(
                            new Slider(2, 0, 5)));
                    flashRMenu.AddItem(new MenuItem("FlashRCanKillEnemy", "Or Can Kill", true).SetValue(true));
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChange", "SkinChange"));
                {
                    SkinManager.AddToMenu(skinMenu, 10);
                }

                var autoLevelMenu = miscMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }

                miscMenu.AddItem(new MenuItem("SupportMode", "Support Mode", true).SetValue(false));
                miscMenu.AddItem(new MenuItem("AutoFollow", "Auto Follow Bear Logic", true).SetValue(true));
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawFlashR", "Draw FlashR Range", true).SetValue(false));
                ManaManager.AddDrawFarm(drawMenu);
                DamageIndicator.AddToMenu(drawMenu, DamageCalculate.GetComboDamage);
            }

            Menu.AddItem(new MenuItem("asd ad asd ", " ", true));
            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();
        }
    }
}
