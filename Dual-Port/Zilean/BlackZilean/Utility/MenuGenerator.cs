using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BlackZilean.Utility
{
    public class MenuGenerator
    {
        public static void Load()
        {
            Zilean.Menu = new Menu("BlackZilean", "black_zilean", true);

            var owMenu = new Menu("[BZ] Orbwalker", "orbwalker");
            Zilean.Orbwalker = new Orbwalking.Orbwalker(owMenu);
            Zilean.Menu.AddSubMenu(owMenu);

            var tsMenu = new Menu("Target Selector", "Target.Selector");
            TargetSelector.AddToMenu(tsMenu);

            var comboMenu = Zilean.Menu.AddSubMenu(new Menu("[BZ] Combo", "zilean.combo"));
            {
                comboMenu.AddItem(new MenuItem("zilean.combo.useQ", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("zilean.combo.useW", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("zilean.combo.useE", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("zilean.combo.useR", "Use R").SetValue(true));

                var comboOptionsMenu = new Menu("Combo & Ult - Options", "zilean.combo.options");
                {
                    comboOptionsMenu.AddItem(new MenuItem("zilean.combo.options.ultAlly", "Use R on Ally").SetValue(true));
                    comboOptionsMenu.AddItem(new MenuItem("zilean.combo.options.ultAllyHp", "Health % to use R on Ally")).SetValue(new Slider(25, 1, 100));
                    comboOptionsMenu.AddItem(new MenuItem("sep1", ""));
                    foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe))
                        comboOptionsMenu.AddItem(new MenuItem("zilean.combo.options.ultCastAlly" + hero.CharData.BaseSkinName, hero.CharData.BaseSkinName).SetValue(true));
                    comboOptionsMenu.AddItem(new MenuItem("sep2", ""));
                    comboOptionsMenu.AddItem(new MenuItem("zilean.combo.options.ultSelf", "Use R on self").SetValue(true));
                    comboOptionsMenu.AddItem(new MenuItem("zilean.combo.options.ultSelfHp", "Health % to use R on self")).SetValue(new Slider(25, 1, 100));
                    comboOptionsMenu.AddItem(new MenuItem("zilean.combo.options.useIgnite", "Use Ignite").SetValue(true));

                    comboMenu.AddSubMenu(comboOptionsMenu);
                }
            }

            var harassMenu = Zilean.Menu.AddSubMenu(new Menu("[BZ] Harass", "zilean.harass"));
            {
                harassMenu.AddItem(new MenuItem("zilean.harass.useQ", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("zilean.harass.useE", "Use E").SetValue(false));
            }

            var waveclearMenu = Zilean.Menu.AddSubMenu(new Menu("[BZ] Waveclear", "zilean.waveclear"));
            {
                waveclearMenu.AddItem(new MenuItem("zilean.waveclear.useQ", "Use Q").SetValue(false));
                waveclearMenu.AddItem(new MenuItem("zilean.waveclear.useW", "Use W").SetValue(false));
            }

            var fleeMenu = Zilean.Menu.AddSubMenu(new Menu("[BZ] Flee", "zilean.flee"));
            {
                fleeMenu.AddItem(new MenuItem("zilean.flee.activated", "Flee Activated").SetValue(new KeyBind('G', KeyBindType.Press)));
            }

            var miscMenu = Zilean.Menu.AddSubMenu(new Menu("[BZ] Miscellaneous", "zilean.misc"));
            {
                miscMenu.AddItem(new MenuItem("zilean.misc.immobileQ", "Use Q on immobile enemys").SetValue(true));
                miscMenu.AddItem(new MenuItem("zilean.misc.gapcloseE", "Use E against gapclosers").SetValue(true));
                miscMenu.AddItem(new MenuItem("zilean.misc.hitchance", "Hitchance").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));
            }

            Zilean.ManaManager.AddToMenu(ref Zilean.Menu);

            var drawingMenu = Zilean.Menu.AddSubMenu(new Menu("[BZ] Drawings", "zilean.drawing"));
            {
                drawingMenu.AddItem(new MenuItem("zilean.drawing.drawQ", "Draw Q").SetValue(new Circle(false, Color.Crimson)));
                drawingMenu.AddItem(new MenuItem("zilean.drawing.drawW", "Draw W").SetValue(new Circle(false, Color.Crimson)));
                drawingMenu.AddItem(new MenuItem("zilean.drawing.drawE", "Draw E").SetValue(new Circle(false, Color.Crimson)));
                drawingMenu.AddItem(new MenuItem("zilean.drawing.drawR", "Draw R").SetValue(new Circle(false, Color.Crimson)));
                drawingMenu.AddItem(new MenuItem("zilean.drawing.drawDamage", "Draw Damage").SetValue(new Circle(true, Color.GreenYellow)));
                drawingMenu.AddItem(new MenuItem("zilean.drawing.drawingsOff", "Turn drawings off").SetValue(false));
            }

            Zilean.Menu.AddItem(new MenuItem("seperator", ""));
            Zilean.Menu.AddItem(new MenuItem("by.blacky", "Made by blacky"));

            Zilean.Menu.AddToMainMenu();
        }
    }
}
