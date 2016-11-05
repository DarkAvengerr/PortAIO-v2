using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal class ModeSettings
    {
        public static Menu MenuLocal { get; private set; }
        public static Menu MenuSkins { get; private set; }
        public static Menu MenuSettingQ { get; private set; }
        public static Menu MenuSettingE { get; private set; }
        public static Menu MenuFlame { get; private set; }
        public static Menu MenuSpellR { get; private set; }

        public static void Init(Menu MenuParent)
        {
            MenuLocal = new Menu("Settings", "Settings");
            MenuParent.AddSubMenu(MenuLocal);

            MenuFlame = new Menu("Flame", "Flame");
            MenuFlame.AddItem(new MenuItem("Flame.Laugh", "After Kill:").SetValue(new StringList(new[] {"Off", "Joke", "Taunt", "Laugh", "Random"}, 4)));
            MenuFlame.AddItem(new MenuItem("Flame.Ctrl6", "After Kill: Show Champion Point Icon (Ctrl + 6)").SetValue(new StringList(new[] { "Off", "On" })));
        }
    }
}
