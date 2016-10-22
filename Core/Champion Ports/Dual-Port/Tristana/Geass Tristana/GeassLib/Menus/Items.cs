using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Menus
{
    class Items
    {
        // ReSharper disable once NotAccessedField.Local
        Events.Items _items;
        public Menu GetMenu()
        {
            var menu = new Menu(Names.ItemMenuBase, "itemMenu");

            var offensiveMenu = new Menu(Names.MenuOffensiveNameBase, "offensiveMenu");
            offensiveMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Boolean.Bork", "Use BotRK/Cutlass").SetValue(true));
            offensiveMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Boolean.Youmuu", "Use Youmuu's").SetValue(true));
            offensiveMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Slider.Bork.MinHp", "(BotRK/Cutlass) Min% HP Remaining(Target)").SetValue(new Slider(20)));
            offensiveMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Slider.Bork.MaxHp", "(BotRK/Cutlass) Max% HP Remaining(Target)").SetValue(new Slider(55)));
            offensiveMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Slider.Bork.MinHp.Player", "(BotRK/Cutlass) Min% HP Remaining(Player)").SetValue(new Slider(20)));
            offensiveMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Boolean.ComboOnly", "Only use offensive items in combo").SetValue(true));

            var defensiveMenu = new Menu(Names.MenuDefensiveNameBase, "defensiveMenu");

            var qssMenu = new Menu(".QSS Menu", "qssMenu");

            qssMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Boolean.QSS", "Use QSS").SetValue(true));
            qssMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Slider.QSS.Delay", "QSS Delay").SetValue(new Slider(300, 250, 1500)));

            foreach (var buff in Data.Buffs.GetTypes)
            {
                qssMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Boolean.QSS." + buff, "Use QSS On" + buff).SetValue(true));
            }

            var mercMenu = new Menu(".Merc Menu", "MercMenu");

            mercMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Boolean.Merc", "Use Merc").SetValue(true));
            mercMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Slider.Merc.Delay", "Merc Delay").SetValue(new Slider(300, 250, 1500)));

            foreach (var buff in Data.Buffs.GetTypes)
            {
                mercMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Boolean.Merc." + buff, "Use Merc On" + buff).SetValue(true));
            }

            defensiveMenu.AddSubMenu(qssMenu);
            defensiveMenu.AddSubMenu(mercMenu);
            defensiveMenu.AddItem(new MenuItem(Names.MenuDefensiveItemBase + "Boolean.ComboOnly", "Only use offensive items in combo").SetValue(true));

            menu.AddSubMenu(offensiveMenu);
            menu.AddSubMenu(defensiveMenu);
            return menu;

        }

        public Items()
        {
            Globals.Objects.Logger.WriteLog("Create Items Menu");
            Globals.Objects.GeassLibMenu.AddSubMenu(GetMenu());
            _items = new Events.Items();
        }

    }
}
