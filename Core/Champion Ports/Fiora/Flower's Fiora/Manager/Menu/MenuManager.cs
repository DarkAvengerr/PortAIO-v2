using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Fiora.Manager.Menu
{
    using LeagueSharp.Common;

    internal class MenuManager : Logic
    {
        internal static void Init()
        {
            Menu = new Menu("Flowers' Fiora", "Flowers_Fiora", true);

            var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            }

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRSolo", "Use R| Solo R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRTeam", "Use R| Team Fight", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboPassive", "Forcus Attack Passive", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboYoumuu", "Use Youmuu", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboTiamat", "Use Tiamat", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboHydra", "Use Hydra", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassPassive", "Forcus Attack Passive", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassTiamat", "Use Tiamat", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassHydra", "Use Hydra", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var laneclearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                laneclearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                laneclearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                laneclearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var jungleclearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                jungleclearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                jungleclearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                jungleclearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeQ", "Use Q", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            Evade.Program.InitEvade();

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qSettings = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qSettings.AddItem(new MenuItem("QUnder", "Dont Cast Q If UnderTurret", true).SetValue(true));
                    qSettings.AddItem(new MenuItem("KillStealQ", "Use Q KillSteal", true).SetValue(true));
                }

                var wSettings = miscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wSettings.AddItem(new MenuItem("KillStealW", "Use W KillSteal", true).SetValue(true));
                }
            }

            var drawingMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawingMenu.AddItem(new MenuItem("DrawingQ", "Draw Q Range", true).SetValue(false));
                drawingMenu.AddItem(new MenuItem("DrawingW", "Draw W Range", true).SetValue(false));
                drawingMenu.AddItem(new MenuItem("DrawingR", "Draw R Range", true).SetValue(false));
                drawingMenu.AddItem(new MenuItem("DrawingDamage", "Draw Combo Damage", true).SetValue(true));
            }

            Menu.AddToMainMenu();
        }
    }
}