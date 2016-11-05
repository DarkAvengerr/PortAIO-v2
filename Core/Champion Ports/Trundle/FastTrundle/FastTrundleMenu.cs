using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FastTrundle
{
    public class FastTrundleMenu
    {
        #region Data

        public static Menu Menu;

        #endregion

        #region Methods

        public static void Initialize()
        {
            Menu = new Menu("FastTrundle", "menu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "orbwalker");
            Trundle.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("FastTrundle.Combo.Q", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("FastTrundle.Combo.W", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("FastTrundle.Combo.E", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("FastTrundle.Combo.R", "Use R").SetValue(false));
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
                {
                    comboMenu.SubMenu("Use R on")
                        .AddItem(
                            new MenuItem("FastTrundle.R.On" + hero.CharData.BaseSkinName, hero.CharData.BaseSkinName)
                                .SetValue(true));
                }

                comboMenu.AddItem(new MenuItem("FastTrundle.Combo.Ignite", "Use Ignite").SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("FastTrundle.Harass.Q", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("FastTrundle.Harass.W", "Use W").SetValue(true));
                harassMenu.AddItem(new MenuItem("FastTrundle.Harass.E", "Use E").SetValue(false));
                harassMenu.AddItem(new MenuItem("FastTrundle.Harass.Mana", "Minimum mana")).SetValue(new Slider(25));
            }

            var lastHitMenu = Menu.AddSubMenu(new Menu("Last Hit", "Lasthit"));
            {
                lastHitMenu.AddItem(new MenuItem("FastTrundle.LastHit.Q", "Use Q").SetValue(true));
                lastHitMenu.AddItem(new MenuItem("FastTrundle.LastHit.Mana", "Minimum mana")).SetValue(new Slider(25));
            }

            var laneClearMenu = Menu.AddSubMenu(new Menu("Lane Clear", "Laneclear"));
            {
                laneClearMenu.AddItem(new MenuItem("FastTrundle.LaneClear.Q", "Use Q").SetValue(true));
                laneClearMenu.AddItem(new MenuItem("FastTrundle.LaneClear.Q.Lasthit", "Only lasthit with Q").SetValue(false));
                laneClearMenu.AddItem(new MenuItem("FastTrundle.LaneClear.W", "Use W").SetValue(true));
                laneClearMenu.AddItem(new MenuItem("FastTrundle.LaneClear.Mana", "Minimum mana")).SetValue(new Slider(25));
            }

            var jungleClearMenu = Menu.AddSubMenu(new Menu("Jungle Clear", "Jungleclear"));
            {
                jungleClearMenu.AddItem(new MenuItem("FastTrundle.JungleClear.Q", "Use Q").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("FastTrundle.JungleClear.W", "Use W").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("FastTrundle.JungleClear.Mana", "Minimum mana"))
                    .SetValue(new Slider(25));
            }

            var itemMenu = Menu.AddSubMenu(new Menu("Items", "Items"));
            {
                itemMenu.AddItem(new MenuItem("FastTrundle.Items.Hydra", "Use Tiamat / Ravenous Hydra").SetValue(true));
                itemMenu.AddItem(new MenuItem("FastTrundle.Items.Titanic", "Use Titanic Hydra").SetValue(true));
                itemMenu.AddItem(new MenuItem("FastTrundle.Items.Youmuu", "Use Youmuu's Ghostblade").SetValue(true));
                itemMenu.AddItem(new MenuItem("FastTrundle.Items.Blade", "Use Cutlass / BOTRK").SetValue(true));
                itemMenu.AddItem(
                    new MenuItem("FastTrundle.Items.Blade.MyHP", "When my HP % <").SetValue(new Slider(50, 0, 100)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                miscMenu.AddItem(new MenuItem("FastTrundle.Draw.off", "Turn drawings off").SetValue(false));
                miscMenu.AddItem(new MenuItem("FastTrundle.Draw.Q", "Draw Q").SetValue(new Circle(false, Color.White)));
                miscMenu.AddItem(new MenuItem("FastTrundle.Draw.W", "Draw W").SetValue(new Circle(false, Color.White)));
                miscMenu.AddItem(new MenuItem("FastTrundle.Draw.E", "Draw E").SetValue(new Circle(true, Color.White)));
                miscMenu.AddItem(new MenuItem("FastTrundle.Draw.R", "Draw R").SetValue(new Circle(false, Color.White)));
                miscMenu.AddItem(new MenuItem("FastTrundle.Draw.Pillar", "Draw Pillar").SetValue(new Circle(false, Color.DeepPink)));
                miscMenu.AddItem(new MenuItem("FastTrundle.Antigapcloser", "Antigapcloser").SetValue(false));
                miscMenu.AddItem(new MenuItem("FastTrundle.Interrupter", "Interrupter").SetValue(true));
            }

            Menu.AddItem(new MenuItem("FastTrundle.Version", "FastTrundle v" + Trundle.ScriptVersion));

            Menu.AddToMainMenu();
        }

        #endregion
    }
}