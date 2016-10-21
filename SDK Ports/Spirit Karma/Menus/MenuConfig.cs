#region

using LeagueSharp;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma.Menus
{
    internal class MenuConfig
    {
        public Menu TargetSelectorMenu;
        private const string MenuName = "Spirit Karma";
        public static Menu MainMenu { get; set; } = new Menu(MenuName, MenuName, true);

        public static void Load()
        {
            ComboMenu = MainMenu.Add(new Menu("ComboMenu", "Combo")); // Combo Main Menu
            MantraMode= ComboMenu.Add(new MenuList<string>("MantraMode", "R Prio", new[] { "Q", "W", "E", "Auto" }));
            Mantra = ComboMenu.Add(new MenuKeyBind("Mantra", "Change Prio Keybind", System.Windows.Forms.Keys.G, KeyBindType.Press));

            HarassMenu= MainMenu.Add(new Menu("HarassMenu", "Harass"));
            HarassR = HarassMenu.Add(new MenuBool("HarassR", "Use R"));
            HarassQ = HarassMenu.Add(new MenuSliderButton("HarassQ", "Q Mana", 70, 0, 100, true));
            HarassW = HarassMenu.Add(new MenuSliderButton("HarassW", "W Mana", 70, 0, 100, true));
            HarassE = HarassMenu.Add(new MenuSliderButton("HarassE", "E Mana", 70, 0, 100, true));

            LaneMenu = MainMenu.Add(new Menu("LaneMenu", "Lane"));
            LaneR = LaneMenu.Add(new MenuBool("LaneR", "Use R"));
            LaneQ = LaneMenu.Add(new MenuSliderButton("LaneQ", "Q Mana", 70, 0, 100, true));
            LaneE = LaneMenu.Add(new MenuSliderButton("LaneE", "E Mana", 70, 0, 100, true));

            // Items
            ItemsMenu = MainMenu.Add(new Menu("ItemsMenu", "Items"));
            UseItems = ItemsMenu.Add(new MenuBool("UseItems", "Use Items"));
            ItemLocket = ItemsMenu.Add(new MenuBool("ItemLocket", "Locket of the Iron Solari"));
            ItemProtoBelt = ItemsMenu.Add(new MenuBool("ItemProtoBelt", "ProtoBelt"));
         // ItemSeraph = ItemsMenu.Add(new MenuBool("ItemSeraph", "Seraph's Embrace"));
            ItemFrostQueen = ItemsMenu.Add(new MenuBool("ItemFrostQueen", "Frost Queen's Claim"));
            
            // Draw
            DrawMenu = MainMenu.Add(new Menu("DrawMenu", "Draw"));
            UseDrawings = DrawMenu.Add(new MenuBool("UseDrawings", "Enable Drawings", true));
            Dind = DrawMenu.Add(new MenuBool("Dind", "Damage Indicator (Fps Heavy)"));
            QRange = DrawMenu.Add(new MenuBool("QRange", "Engage Range (Q)", true));
            MantraDraw = DrawMenu.Add(new MenuBool("MantraDraw", "Draw Selected Prio", true));
            
            // Skins
            SkinMenu = MainMenu.Add(new Menu("SkinChanger", "SkinChanger"));
            UseSkin = SkinMenu.Add(new MenuBool("UseSkin", "Use SkinChanger"));
            SkinChanger = SkinMenu.Add(new MenuList<string>("Skins", "Skins", new[] { "Default", "Sun Godess Karma", "Sakura Karma", "Traditional Karma", "Order Of Lotus Karma", "Warden Karma" }));
            

            TrinketMenu = MainMenu.Add(new Menu("TrinketMenu", "Trinket"));
            Trinket = TrinketMenu.Add(new MenuBool("Trinket", "Auto Buy Advanced Trinket"));
            TrinketList = TrinketMenu.Add(new MenuList<string>("TrinketList", "Choose Trinket", new[] { "Oracle Alternation", "Farsight Alternation" }));

            FleeKey = MainMenu.Add(new MenuKeyBind("FleeKey", "Flee", System.Windows.Forms.Keys.A, KeyBindType.Press));

            MainMenu.Attach();
        }
        // Main Menu
        public static Menu ComboMenu;
        public static Menu LaneMenu;
        public static Menu JungleMenu;
        public static Menu HarassMenu;
        public static Menu ItemsMenu;
        public static Menu InterruptMenu;
        public static Menu MiscMenu;
        public static Menu DrawMenu;
        public static Menu SkinMenu;
        public static Menu TrinketMenu;

        // Lists
        public static MenuList<string> MantraMode;
        public static MenuList<string> SkinChanger;
        public static MenuList<string> TrinketList;

        // Slider
        public static MenuSliderButton HarassQ;
        public static MenuSliderButton HarassW;
        public static MenuSliderButton HarassE;

        public static MenuSliderButton LaneQ;
        public static MenuSliderButton LaneE;

        // Keybind
        public static MenuKeyBind Mantra;
        public static MenuKeyBind FleeKey;

        // Menu Bool
        public static MenuBool ItemFrostQueen;
      //  public static MenuBool ItemSeraph;
        public static MenuBool ItemLocket;
        public static MenuBool ItemProtoBelt;

        public static MenuBool UseDrawings;
        public static MenuBool MantraDraw;
        public static MenuBool QRange;
        public static MenuBool Dind;

        public static MenuBool HarassR;
        public static MenuBool LaneR;

        public static MenuBool UseSkin;
        public static MenuBool UseItems;
        public static MenuBool Trinket;
        public static MenuBool UseInterruptTarget;
        public static MenuBool UseInterrupt;
    }
}
