using GeassLib.Events;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Menus
{
    class Trinket
    {
        // ReSharper disable once NotAccessedField.Local
        private Events.Trinket _trinket;
        public Menu GetMenu()
        {
            var menu = new Menu(Names.TrinketNameBase, "trinketOptions");
            menu.AddItem(new MenuItem(Names.TrinketItemBase + "Boolean.BuyOrb", "Auto Buy Orb At Level >= 9").SetValue(true));
            return menu;

        }

        public Trinket()
        {
            if (!DelayHandler.Loaded) DelayHandler.Load();

            Globals.Objects.Logger.WriteLog("Create Trinket Menu");
            Globals.Objects.GeassLibMenu.AddSubMenu(GetMenu());
            _trinket = new Events.Trinket();
        }
    }
}
