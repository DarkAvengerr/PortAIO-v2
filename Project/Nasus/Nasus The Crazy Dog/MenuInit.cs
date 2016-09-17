using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nasus
{
    using LeagueSharp.Common;


    public class MenuInit
    {
        public static Menu Menu;


        public static void Initialize()
        {
            Menu                = new Menu("Nasus - The Crazy Dog", "L# Nasus", true);
            var orbwalkerMenu   = new Menu("Orbwalker", "orbwalker");
            Standards.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            TargetSelector.AddToMenu(TargetSelectorMenu());

            #region Combo Menu
            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "MenuCombo"));
            {
                comboMenu
                    .AddItem(new MenuItem("Combo.Use.Q", "Use Q").SetValue(true));
                comboMenu
                    .AddItem(new MenuItem("Combo.Use.W", "Use W").SetValue(true));
                comboMenu
                    .AddItem(new MenuItem("Combo.Use.E", "Use E").SetValue(true));
                comboMenu
                    .AddItem(new MenuItem("Combo.Use.R", "Use R").SetValue(true));
                comboMenu
                    .AddItem(new MenuItem("Combo.Min.HP.Use.R", "HP to use R").SetValue(new Slider(35)));
            }
            #endregion

            #region Harass Menu
            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "MenuHarass"));
            {
                harassMenu
                    .AddItem(new MenuItem("Harass.Use.Q", "Use Q").SetValue(true));
                harassMenu
                    .AddItem(new MenuItem("Harass.Use.W", "Use W").SetValue(true));
                harassMenu
                    .AddItem(new MenuItem("Harass.Use.E", "Use E").SetValue(true));
            }
            #endregion

            #region Lane Clear
            var laneClearMenu = Menu.AddSubMenu(new Menu("Lane Clear", "MenuLaneClear"));
            {
                laneClearMenu
                    .AddItem(new MenuItem("LaneClear.Use.Q", "Use Q").SetValue(true));
                laneClearMenu
                    .AddItem(new MenuItem("LaneClear.Use.E", "Use E").SetValue(true));
            }

            #endregion

            #region Last Hit
            var lastHitMenu = Menu.AddSubMenu(new Menu("Stack Siphoning Strike", "MenuStackQ"));
            {
                lastHitMenu
                    .AddItem(new MenuItem("Use.StackQ", "Stack").SetValue(true));
            }
            #endregion

            Menu.AddItem(new MenuItem("devCredits", "Dev by @ TwoHam"));
            Menu.AddToMainMenu();
        }

        private static Menu TargetSelectorMenu()
        {
            return Menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
        }
    }
}
