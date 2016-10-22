using LeagueSharp.Common;
using _Project_Geass.Functions;
using _Project_Geass.Global.Data;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Core.Items.Menus
{

    internal sealed class Item
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Item" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        /// <param name="enabled">
        ///     if set to <c> true </c> [enabled].
        /// </param>
        /// <param name="orbwalker">
        ///     The orbwalker.
        /// </param>
        public Item(Menu menu, bool enabled, Orbwalking.Orbwalker orbwalker)
        {
            if (!enabled)
                return;

            StaticObjects.ProjectLogger.WriteLog("Item Menu and events loaded.");
            menu.AddSubMenu(Menu());
            // ReSharper disable once UnusedVariable
            var items=new Events.Item(orbwalker);
        }

        #endregion Public Constructors

        #region Private Methods

        private Menu Menu()
        {
            var menu=new Menu(Names.Menu.ItemMenuBase, "itemMenu");

            var offensiveMenu=new Menu(Names.Menu.MenuOffensiveItemBase, "offensiveMenu");
            offensiveMenu.AddItem(new MenuItem(Names.Menu.MenuOffensiveItemBase+"Boolean.Bork", "Use BotRK/Cutlass").SetValue(true));
            offensiveMenu.AddItem(new MenuItem(Names.Menu.MenuOffensiveItemBase+"Boolean.Youmuu", "Use Youmuu's").SetValue(true));
            offensiveMenu.AddItem(new MenuItem(Names.Menu.MenuOffensiveItemBase+"Slider.Bork.MinHp", "(BotRK/Cutlass) Min% HP Remaining(Target)").SetValue(new Slider(20)));
            offensiveMenu.AddItem(new MenuItem(Names.Menu.MenuOffensiveItemBase+"Slider.Bork.MaxHp", "(BotRK/Cutlass) Max% HP Remaining(Target)").SetValue(new Slider(55)));
            offensiveMenu.AddItem(new MenuItem(Names.Menu.MenuOffensiveItemBase+"Slider.Bork.MinHp.Player", "(BotRK/Cutlass) Min% HP Remaining(Player)").SetValue(new Slider(20)));
            offensiveMenu.AddItem(new MenuItem(Names.Menu.MenuOffensiveItemBase+"Boolean.ComboOnly", "Only use offensive items in combo").SetValue(true));

            var defensiveMenu=new Menu(Names.Menu.MenuDefensiveNameBase, "defensiveMenu");

            var qssMenu=new Menu(".QSS Menu", "qssMenu");

            qssMenu.AddItem(new MenuItem(Names.Menu.MenuDefensiveItemBase+"Boolean.QSS", "Use QSS").SetValue(true));
            qssMenu.AddItem(new MenuItem(Names.Menu.MenuDefensiveItemBase+"Slider.QSS.Delay", "QSS Delay").SetValue(new Slider(300, 250, 1500)));

            foreach (var buff in Buffs.GetTypes)
                qssMenu.AddItem(new MenuItem(Names.Menu.MenuDefensiveItemBase+"Boolean.QSS."+buff, "On "+buff).SetValue(true));

            var mercMenu=new Menu(".Merc Menu", "MercMenu");

            mercMenu.AddItem(new MenuItem(Names.Menu.MenuDefensiveItemBase+"Boolean.Merc", "Use Merc").SetValue(true));
            mercMenu.AddItem(new MenuItem(Names.Menu.MenuDefensiveItemBase+"Slider.Merc.Delay", "Merc Delay").SetValue(new Slider(300, 250, 1500)));

            foreach (var buff in Buffs.GetTypes)
                mercMenu.AddItem(new MenuItem(Names.Menu.MenuDefensiveItemBase+"Boolean.Merc."+buff, "On "+buff).SetValue(true));

            defensiveMenu.AddSubMenu(qssMenu);
            defensiveMenu.AddSubMenu(mercMenu);
            defensiveMenu.AddItem(new MenuItem(Names.Menu.MenuDefensiveItemBase+"Boolean.ComboOnly", "Only use offensive items in combo").SetValue(true));

            menu.AddSubMenu(offensiveMenu);
            menu.AddSubMenu(defensiveMenu);
            return menu;
        }

        #endregion Private Methods
    }

}