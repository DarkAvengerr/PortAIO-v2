using System.Windows.Forms;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;
using Menu = LeagueSharp.SDK.UI.Menu;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace hJhin.Extensions
{
    class Config
    {
        public static Menu Menu;
        public static MenuKeyBind SemiManualUlt;
        public static MenuList<string> HitChance;

        public static void ExecuteMenu()
        {
            Menu = new Menu("hJhin", "hJhin", true);
            {
                var combomenu = Menu.Add(new Menu("combo.settings", "Combo Settings"));
                {
                    combomenu.Add(new MenuSeparator("combo.q.sep", "(Q) Settings"));
                    combomenu.Add(new MenuBool("combo.q", "Use (Q)", true));

                    combomenu.Add(new MenuSeparator("combo.w.sep", "(W) Settings"));
                    combomenu.Add(new MenuBool("combo.w", "Use (W)", true));
                    combomenu.Add(new MenuSlider("combo.w.min", "Min. Distance", 400, 1, 2500));
                    combomenu.Add(new MenuSlider("combo.w.max", "Max. Distance", 1000, 1, 2500));
                    combomenu.Add(new MenuBool("combo.w.mark", "Use (W) If Enemy is Marked", true));

                    combomenu.Add(new MenuSeparator("combo.e.sep", "(E) Settings"));
                    combomenu.Add(new MenuBool("combo.e", "Use (E)", true));
                }

                var harassmenu = Menu.Add(new Menu("harass.settings", "Harass Settings"));
                {
                    harassmenu.Add(new MenuSeparator("harass.q.sep", "(Q) Settings"));
                    harassmenu.Add(new MenuBool("harass.q", "Use (Q)", true));

                    harassmenu.Add(new MenuSeparator("harass.w.sep", "(W) Settings"));
                    harassmenu.Add(new MenuBool("harass.w", "Use (W)", true));

                    harassmenu.Add(new MenuSeparator("harass.mana.manager", "Mana Settings"));
                    harassmenu.Add(new MenuSlider("harass.mana", "Min. Mana", 50, 1, 99));
                }

                var lanemenu = Menu.Add(new Menu("laneclear.settings", "WaveClear Settings"));
                {

                    lanemenu.Add(new MenuSeparator("lane.q.sep", "(Q) Settings"));
                    lanemenu.Add(new MenuBool("lane.q", "Use (Q)", true));

                    lanemenu.Add(new MenuSeparator("lane.w.sep", "(W) Settings"));
                    lanemenu.Add(new MenuBool("lane.w", "Use (W)", true));
                    lanemenu.Add(new MenuSlider("lane.w.min.count", "Min. Minion", 4,1,5));

                    lanemenu.Add(new MenuSeparator("lane.mana.manager", "Mana Settings"));
                    lanemenu.Add(new MenuSlider("lane.mana", "Min. Mana", 50, 1, 99));
                }


                var junglemenu = Menu.Add(new Menu("jungle.settings", "Jungle Settings"));
                {
                    junglemenu.Add(new MenuSeparator("jungle.q.sep", "(Q) Settings"));
                    junglemenu.Add(new MenuBool("jungle.q", "Use (Q)", true));

                    junglemenu.Add(new MenuSeparator("jungle.w.sep", "(W) Settings"));
                    junglemenu.Add(new MenuBool("jungle.w", "Use (W)", true));

                    junglemenu.Add(new MenuSeparator("jungle.mana.manager", "Mana Settings"));
                    junglemenu.Add(new MenuSlider("jungle.mana", "Min. Mana", 50, 1, 99));
                }

                var miscmenu = Menu.Add(new Menu("misc.settings", "Miscellaneous"));
                {
                    miscmenu.Add(new MenuBool("auto.e.immobile", "Auto Cast (E) Immobile Target", true));

                    miscmenu.Add(new MenuSeparator("auto.orb.seperator", "Scrying Orb Settings"));
                    miscmenu.Add(new MenuBool("auto.orb.buy", "Auto Orb. Buy", true));
                    miscmenu.Add(new MenuSlider("orb.level", "Orb. Level", 9, 9, 18));

                }

                var ultimate = Menu.Add(new Menu("ultimate.settings", "Ultimate Settings"));
                {
                    ultimate.Add(new MenuSeparator("ultimate.white.sep", "Ultimate Whitelist"));

                    foreach (var enemy in GameObjects.EnemyHeroes)
                    {
                        ultimate.Add(new MenuBool("combo.r." + enemy.ChampionName, "(R): " + enemy.ChampionName, true));
                    }

                    ultimate.Add(new MenuSeparator("ultimate.sep", "Ultimate Settings"));
                    ultimate.Add(new MenuBool("combo.r", "Use (R)", true));
                    ultimate.Add(new MenuBool("auto.shoot.bullets", "If Jhin Casting (R) Auto Cast Bullets", true));
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

                Menu.Add(new MenuSeparator("genel.seperator1", "hJhin Keys"));
                SemiManualUlt = Menu.Add(new MenuKeyBind("semi.manual.ult", "Semi-Manual (R)!", Keys.A, KeyBindType.Press));

                Menu.Add(new MenuSeparator("hit.seperator1", "hJhin HitChance"));
                HitChance = Menu.Add(new MenuList<string>("hitchance", "HitChance ?", new[] { "Medium", "High", "Very High" }));

                Menu.Attach();
            }
        }
    }
}