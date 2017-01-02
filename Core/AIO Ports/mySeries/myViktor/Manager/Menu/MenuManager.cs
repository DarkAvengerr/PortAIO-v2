using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Menu
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
                comboMenu.AddItem(new MenuItem("ComboQGap", "Use Q Gapcloser Target", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboQGapLevel", "When Player Level >= x| Q Gapcloser", true).SetValue(
                        new Slider(9, 1, 18)));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboWLogic", "Use W| Logic", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboWTeam", "Use W| TeamFight", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboWTeamHit", "Use W| TeamFight Hit Count >= x", true).SetValue(new Slider(3, 1, 5)));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRalways", "Use R| Always", true).SetValue(false));
                comboMenu.AddItem(new MenuItem("ComboRKill", "Use R| KillSteal", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRTeam", "Use R| TeamFight", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboRTeamHit", "Use R| TeamFight Hit Count >= x", true).SetValue(new Slider(3, 1, 5)));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearECount", "Use E| Hit Count >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var lastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                lastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q", true).SetValue(true));
                lastHitMenu.AddItem(
                    new MenuItem("LastHitMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qSettings = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qSettings.AddItem(
                        new MenuItem("DisableAA", "When Q is Ready Disable AutoAttack", true).SetValue(true)
                            .SetTooltip("just Only Work in Combo Mode"));
                    qSettings.AddItem(
                        new MenuItem("DisableAALevel", "When Player Level >= x| Auto Disable", true).SetValue(
                            new Slider(9, 1, 18)));
                    qSettings.AddItem(new MenuItem("DisableAAALL", "Or All Level Disable Attack", true).SetValue(false));
                }

                var wSettings = miscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wSettings.AddItem(new MenuItem("AutoW", "If Enemy CC", true).SetValue(true));
                    wSettings.AddItem(new MenuItem("BrokenW", "Broken Spell", true).SetValue(true));
                    wSettings.AddItem(new MenuItem("GapW", "Anti Gapcloser", true).SetValue(true));
                    wSettings.AddItem(new MenuItem("IntW", "Interrupt Spell", true).SetValue(true));
                }

                var rSettings = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    rSettings.AddItem(new MenuItem("AutoR", "Auto R Follow Logic", true).SetValue(true));
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChange", "SkinChange"));
                {
                    SkinManager.AddToMenu(skinMenu, 3);
                }

                var autoLevelMenu = miscMenu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
                {
                    LevelsManager.AddToMenu(autoLevelMenu);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawEMax", "Draw EMax Range", true).SetValue(false));
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
