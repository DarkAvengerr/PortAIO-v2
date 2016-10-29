using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BlackFeeder
{

    public class InitializeMenu
    {
        public static void Load()
        {
            Entry.Menu = new Menu("BlackFeeder 2.0", "BlackFeeder", true);

            Entry.Menu.AddItem(new MenuItem("Feeding.Activated", "Feeding Activated").SetValue(true));
            Entry.Menu.AddItem(new MenuItem("Feeding.FeedMode", "Feeding Mode:").SetValue(new StringList(new[] { "Middle Lane", "Bottom Lane", "Top Lane", "Random Lane" })));

            var feedingMenu = Entry.Menu.AddSubMenu(new Menu("Feeding Options", "FeedingMenu"));
            {
                feedingMenu.AddItem(new MenuItem("Spells.Activated", "Spells Activated").SetValue(true));
                feedingMenu.AddItem(new MenuItem("Messages.Activated", "Messages Activated").SetValue(true));
                feedingMenu.AddItem(new MenuItem("Laugh.Activated", "Laugh Activated").SetValue(true));
                feedingMenu.AddItem(new MenuItem("Items.Activated", "Items Activated").SetValue(true));
                feedingMenu.AddItem(new MenuItem("Attacks.Disabled", "Disable auto attacks").SetValue(true));
            }

            var miscMenu = Entry.Menu.AddSubMenu(new Menu("Misc Options", "MiscMenu"));
            {
                miscMenu.AddItem(new MenuItem("Quit.Activated", "Quit on Game End").SetValue(true));
                miscMenu.AddItem(new MenuItem("Surrender.Activated", "Auto Surrender Activated").SetValue(true));
            }

            Entry.Menu.AddItem(new MenuItem("seperator", ""));
            Entry.Menu.AddItem(new MenuItem("by.blacky", "Made by blacky"));

            Entry.Menu.AddToMainMenu();
        }
    }
}