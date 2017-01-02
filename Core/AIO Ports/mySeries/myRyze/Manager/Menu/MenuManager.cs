using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Menu
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
                comboMenu.AddItem(
                    new MenuItem("ComboQSmart", "Use Q| Logic", true).SetValue(true)
                        .SetTooltip("Smart Ignore Collsion Q Logic"));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboMode", "Combo Mode: ", true).SetValue(
                            new StringList(new[] {"Normal", "Shield", "Burst(HuangXiaoMing Mode)"}))
                        .SetTooltip("Burst Mode No More SHIELD!!!"));
                comboMenu.AddItem(
                    new MenuItem("ComboModeSwitch", "Combo Mode Switch Key", true).SetValue(
                        new KeyBind('T', KeyBindType.Press))).ValueChanged += ChangedComboMode;
                comboMenu.AddItem(
                    new MenuItem("ComboShieldHP", "When Player HealthPercent <= x%| Active Shield", true).SetValue(
                        new Slider(60)));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboDisableAA", "Disable Attack Mode: ", true).SetValue(
                        new StringList(new[] {"Smart", "All", "Off"})));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(false));
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
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
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var interruptMenu = miscMenu.AddSubMenu(new Menu("Interrupt Settings", "Interrupt Settings"));
                {
                    interruptMenu.AddItem(new MenuItem("Interrupt", "Interrupt Danger Spells", true).SetValue(true));
                    interruptMenu.AddItem(new MenuItem("AntiAlistar", "Interrupt Alistar W", true).SetValue(true));
                    interruptMenu.AddItem(new MenuItem("AntiRengar", "Interrupt Rengar Jump", true).SetValue(true));
                    interruptMenu.AddItem(new MenuItem("AntiKhazix", "Interrupt Khazix R", true).SetValue(true));
                }

                var antigapcloserMenu =
                    miscMenu.AddSubMenu(new Menu("AntiGapcloser Settings", "AntiGapcloser Settings"));
                {
                    antigapcloserMenu.AddItem(new MenuItem("Gapcloser", "Anti Gapcloser", true).SetValue(true));
                    foreach (var target in HeroManager.Enemies)
                    {
                        antigapcloserMenu.AddItem(
                            new MenuItem("AntiGapcloser" + target.ChampionName.ToLower(), target.ChampionName, true)
                                .SetValue(true));
                    }
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChange", "SkinChange"));
                {
                    SkinManager.AddToMenu(skinMenu, 10);
                }

                var autoLevelMenu = miscMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }

                var autoStackMenu = miscMenu.AddSubMenu(new Menu("Auto Stack", "Auto Stack"));
                {
                    StackManager.AddToMenu(autoStackMenu, true, false, false);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawRMin", "Draw R Range(MinMap)", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawMode", "Draw Combo Mode Status", true).SetValue(true));
                ManaManager.AddDrawFarm(drawMenu);
                //DamageIndicator.AddToMenu(drawMenu, DamageCalculate.GetComboDamage);
            }

            Menu.AddItem(new MenuItem("asd ad asd ", " ", true));
            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();
        }

        private static void ChangedComboMode(object obj, OnValueChangeEventArgs Args)
        {
            if (Args.GetNewValue<KeyBind>().Active)
            {
                switch (Menu.Item("ComboMode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        Menu.Item("ComboMode", true)
                            .SetValue(new StringList(new[] {"Normal", "Shield", "Burst(HuangXiaoMing Mode)"}, 1));
                        break;
                    case 1:
                        Menu.Item("ComboMode", true)
                            .SetValue(new StringList(new[] {"Normal", "Shield", "Burst(HuangXiaoMing Mode)"}, 2));
                        break;
                    case 2:
                        Menu.Item("ComboMode", true)
                            .SetValue(new StringList(new[] {"Normal", "Shield", "Burst(HuangXiaoMing Mode)"}));
                        break;
                }
            }
        }
    }
}
