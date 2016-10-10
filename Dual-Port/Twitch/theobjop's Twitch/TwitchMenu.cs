using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Twiitch {
    class TwitchMenu {

        public static Menu _config;

        public static void Init() {
            // Twiitch
            // 
            
            // New Menu
            _config = new Menu("Twiitch", "Twitch", true);

            // Orbwalker
            Twitch._orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            // Target selector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);

            // Combo menu
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            _config.SubMenu("Combo").AddItem(new MenuItem("WAntigap", "Use W to anti-gapclose").SetValue(true));

            _config.SubMenu("Combo").AddItem(new MenuItem("QKill", "Auto Q after Kill").SetValue(true));
            _config.SubMenu("Combo").AddItem(new MenuItem("QFleeCount", "Auto Q after Kill when X additional enemies").SetValue(new Slider(0,0,4)));
            _config.SubMenu("Combo").AddItem(new MenuItem("QFleeNote", "Set above to 0 to auto Q after all kills"));

            _config.SubMenu("Combo").AddItem(new MenuItem("RToKS", "Use R to KS").SetValue(false));

            // Lane Clear
            _config.SubMenu("LaneClear").AddItem(new MenuItem("ELaneClear", "Use E").SetValue(true));
            _config.SubMenu("LaneClear").AddItem(new MenuItem("ELaneMinionCount", "Min Mobs for E").SetValue(new Slider(3,1,6)));
            _config.SubMenu("LaneClear").AddItem(new MenuItem("WLaneClear", "Use W").SetValue(true));
            _config.SubMenu("LaneClear").AddItem(new MenuItem("WLaneMinionCount", "Min Mobs for W").SetValue(new Slider(3,1,6)));
            _config.SubMenu("LaneClear").AddItem(new MenuItem("LaneMana", "Min Mana for Abilities").SetValue(new Slider(5)));

            // JG Clear
            _config.SubMenu("JungleClear").AddItem(new MenuItem("EJGClear", "Use E").SetValue(true));
            _config.SubMenu("JungleClear").AddItem(new MenuItem("WJGClear", "Use W").SetValue(true));
            _config.SubMenu("JungleClear").AddItem(new MenuItem("WJGMinionCount", "Minion Count for W").SetValue(new Slider(2,1,4)));
            _config.SubMenu("JungleClear").AddItem(new MenuItem("JGMana", "Min Mana for Abilities").SetValue(new Slider(5)));
            
            // KS Mobs
            _config.AddItem(new MenuItem("KSMonsters", "KS Monsters with E?").SetValue(true));

            // Misc
            _config.SubMenu("Misc").AddItem(new MenuItem("EDamage", "E damage on healthbar").SetValue(new Circle(true, Color.Green)));
            _config.SubMenu("Misc").AddItem(new MenuItem("EKillsteal", "Killsteal with E").SetValue(true));
            _config.SubMenu("Misc").AddItem(new MenuItem("EBeforeDeath", "Use E before death if possible").SetValue(true));
            _config.SubMenu("Misc").AddItem(new MenuItem("DrawQRange", "Draw Q Range").SetValue(true));

            // Attach to main menu
            _config.AddToMainMenu();
        }

        public static MenuItem Item(string item) {
            return _config.Item(item);
        }
    }
}
