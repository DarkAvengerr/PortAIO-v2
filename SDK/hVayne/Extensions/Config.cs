using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.SDK;
using LeagueSharp.SDK.UI;
using Menu = LeagueSharp.SDK.UI.Menu;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace hVayne.Extensions
{
    class Config
    {
        public static Menu Menu;
        public static MenuList<string> MethodQ, ComboMethod, CondemnMethod, HarassMenu;
        public static MenuSlider PushDistance;

        public static void ExecuteMenu()
        {
            Menu = new Menu("hVayne [SDK]", "hVayne [SDK]", true);
            {
                var combomenu = Menu.Add(new Menu("combo.settings", "Combo Settings"));
                {
                    combomenu.Add(new MenuBool("combo.q", "Use (Q)", true));
                    combomenu.Add(new MenuBool("combo.e", "Use (E)", true));
                    combomenu.Add(new MenuBool("combo.r", "Use (R)", true));
                    combomenu.Add(new MenuSlider("combo.r.count", "Min. Enemy Count (R)", 3,1,5));
                }

                var harassmenu = Menu.Add(new Menu("harass.settings", "Harass Settings"));
                {
                    harassmenu.Add(new MenuBool("harass.q", "Use (Q)", true));
                    harassmenu.Add(new MenuBool("harass.e", "Use (E)", true));
                    harassmenu.Add(new MenuSlider("harass.mana", "Min. Mana", 50, 1, 99));
                }

                var junglemenu = Menu.Add(new Menu("jungle.settings", "Jungle Settings"));
                {
                    junglemenu.Add(new MenuBool("jungle.q", "Use (Q)", true));
                    junglemenu.Add(new MenuBool("jungle.e", "Use (E)", true));
                    junglemenu.Add(new MenuSlider("jungle.mana", "Min. Mana", 50, 1, 99));
                }

                var condemnmenu = Menu.Add(new Menu("condemn.settings", "Condemn Settings"));
                {
                    foreach (var enemy in GameObjects.EnemyHeroes)
                    {
                        condemnmenu.Add(new MenuBool("condemn." + enemy.ChampionName, "(Codenmn): "+enemy.ChampionName, true));
                    }
                }

                var miscmenu = Menu.Add(new Menu("misc.settings", "Miscellaneous"));
                {
                    miscmenu.Add(new MenuBool("interrupter.e", "Interrupter (E)", true));

                    miscmenu.Add(new MenuSeparator("auto.orb.seperator", "Scrying Orb Settings"));
                    miscmenu.Add(new MenuBool("auto.orb.buy", "Auto Orb. Buy", true));
                    miscmenu.Add(new MenuSlider("orb.level", "Orb. Level", 9, 9, 18));

                }

                var activator = Menu.Add(new Menu("activator.settings", "Activator Settings"));
                {
                    activator.Add(new MenuSeparator("qss.seperator", "Quicksilver Sash Settings"));
                    activator.Add(new MenuBool("use.qss", "Use QSS", true));

                    activator.Add(new MenuSeparator("qss.seperator2", "Quicksilver Sash Debuffs"));
                    activator.Add(new MenuBool("qss.charm", "Charm", true));
                    activator.Add(new MenuBool("qss.snare", "Snare", true));
                    activator.Add(new MenuBool("qss.polymorph", "Polymorph", true));
                    activator.Add(new MenuBool("qss.stun", "Stun", true));
                    activator.Add(new MenuBool("qss.suppression", "Suppression", true));
                    activator.Add(new MenuBool("qss.taunt", "Taunt", true));

                    activator.Add(new MenuSeparator("botrk.seperator", "BOTRK Settings"));
                    activator.Add(new MenuBool("use.botrk", "Use BOTRK", true));
                    activator.Add(new MenuSlider("botrk.vayne.hp", "Min. Vayne HP", 20, 1, 99));
                    activator.Add(new MenuSlider("botrk.enemy.hp", "Min. Enemy HP", 20, 1, 99));

                    activator.Add(new MenuSeparator("youmuu.seperator", "Youmuu Settings"));
                    activator.Add(new MenuBool("use.youmuu", "Use Youmuu", true));

                }

                Menu.Add(new MenuSeparator("genel.seperator1", "hVayne Modes"));

                PushDistance = Menu.Add(new MenuSlider("condemn.push.distance", "Condemn Push Distance", 410, 350, 420));
                CondemnMethod = Menu.Add(new MenuList<string>("condemn.style", "Condemn Mode : ", new[] {"Shine", "Asuna", "360"}));
                MethodQ = Menu.Add(new MenuList<string>("q.style", "(Q) Mode : ", new[] { "Cursor Position", "Safe Position" }));
                ComboMethod = Menu.Add(new MenuList<string>("combo.type", "Combo Mode : ", new[] { "Normal", "Burst" }));
                HarassMenu = Menu.Add(new MenuList<string>("harass.type", "Harass Mode : ", new[] { "2W + Q", "2W + E" }));
                Menu.Attach();
            }
        }
    }
}
