using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;
using System.Windows.Forms;
using Menu = LeagueSharp.SDK.UI.Menu;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Preserved_Kassadin.Cores
{
    class MenuConfig
    {
        private const string MenuName = "Preserved Kassadin";
        private static Menu MainMenu { get; set; } = new Menu(MenuName, MenuName, true);

        public static void Load()
        {
            #region Combo
            ComboMenu = MainMenu.Add(new Menu("ComboMenu", "Combo"));
            SafeR = ComboMenu.Add(new MenuSlider("SafeR", "Don't R Into x Enemies", 3, 0, 5));

            #endregion

            #region Harass
            HarassMenu = MainMenu.Add(new Menu("HarassMenu", "Harass"));
            AutoHarass = HarassMenu.Add(new MenuKeyBind("AutoHarass", "Auto Harass", Keys.T, KeyBindType.Toggle));
            HarassQ = HarassMenu.Add(new MenuBool("HarassQ", "Harass Q"));
            #endregion

            #region Lane
            LaneMenu = MainMenu.Add(new Menu("LaneMenu", "Lane"));
            StackQ = LaneMenu.Add(new MenuBool("StackQ", "Stack Tear With Q"));
            StackMana = LaneMenu.Add(new MenuSlider("StackMana", "Stack Minimum Mana %", 50, 0, 100));
            LaneW = LaneMenu.Add(new MenuBool("LaneW", "Laneclear W"));
            LaneE = LaneMenu.Add(new MenuBool("LaneE", "Laneclear E"));
            LaneR = LaneMenu.Add(new MenuBool("LaneR", "Laneclear R"));
            LaneMana = LaneMenu.Add(new MenuSlider("LaneMana", "Lane Minimum Mana %", 50, 0, 100));
            #endregion

            #region Jungle
            JungleMenu = MainMenu.Add(new Menu("JungleMenu", "Jungle"));
            JungleQ = JungleMenu.Add(new MenuBool("JungleQ", "Jungle Q"));
            JungleW = JungleMenu.Add(new MenuBool("JungleW", "Jungle W"));
            JungleE = JungleMenu.Add(new MenuBool("JungleE", "Jungle E"));
            JungleR = JungleMenu.Add(new MenuBool("JungleR", "Jungle R"));
            #endregion

            #region Draw
            DrawMenu = MainMenu.Add(new Menu("DrawMenu", "Draw"));
            DrawDmg = DrawMenu.Add(new MenuBool("DrawDmg", "Draw Damage", true));
            DisableDraw = DrawMenu.Add(new MenuBool("DisableDraw", "Don't Draw"));
            DrawQ = DrawMenu.Add(new MenuBool("DrawQ", "Q Range", true));
            DrawE = DrawMenu.Add(new MenuBool("DrawE", "E Range"));
            DrawR = DrawMenu.Add(new MenuBool("DrawR", "R Range", true));
            #endregion

            #region Killsteal
            KillstealMenu = MainMenu.Add(new Menu("KillstealMenu", "Killsteal"));
            KsQ = KillstealMenu.Add(new MenuBool("KsQ", "Killsteal Q", true));
            KsW = KillstealMenu.Add(new MenuBool("KsW", "Killsteal W", true));
            KsE = KillstealMenu.Add(new MenuBool("KsE", "Killsteal E", true));
            KsR = KillstealMenu.Add(new MenuBool("KsR", "Killsteal R", true));
            #endregion

            #region Trinket
            TrinketMenu = MainMenu.Add(new Menu("TrinketMenu", "Trinket"));
            BuyTrinket = TrinketMenu.Add(new MenuBool("BuyTrinket", "Buy Trinket"));
            TrinketList = TrinketMenu.Add(new MenuList<string>("TrinketList", "Choose Trinket", new[] { "Oracle Alternation", "Farsight Alternation" }));
            #endregion

            #region Skin
            SkinMenu = MainMenu.Add(new Menu("SkinMenu", "Skin"));
            SkinList = SkinMenu.Add(new MenuList<string>("Skins", "Skins", new[] { "Default", "Festival Kassadin", "Deep One Kassadin", "Pre-Void Kassadin", "Harbinger Kassadin", "Cosmic Reaver Kassadin"}));
            #endregion

            MainMenu.Attach();
        }

        // Menu
        public static Menu ComboMenu;
        public static Menu HarassMenu;
        public static Menu LaneMenu;
        public static Menu JungleMenu;
        public static Menu DrawMenu;
        public static Menu KillstealMenu;
        public static Menu TrinketMenu;
        public static Menu SkinMenu;

        // Bools
        public static MenuBool BuyTrinket;
        public static MenuBool DrawDmg;
        public static MenuBool DisableDraw;
        public static MenuBool DrawQ;
        public static MenuBool DrawE;
        public static MenuBool DrawR;
        public static MenuBool KsQ;
        public static MenuBool KsW;
        public static MenuBool KsE;
        public static MenuBool KsR;
        public static MenuBool KsIgnite;
        public static MenuBool StackQ;
        public static MenuBool LaneW;
        public static MenuBool LaneE;
        public static MenuBool LaneR;
        public static MenuBool JungleQ;
        public static MenuBool JungleW;
        public static MenuBool JungleE;
        public static MenuBool JungleR;
        public static MenuBool HarassQ;
        

        // List
        public static MenuList<string> SkinList;
        public static MenuList<string> TrinketList;

        // Slider
        public static MenuSlider SafeR;
        public static MenuSlider StackMana;
        public static MenuSlider LaneMana;

        // Keybind
        public static MenuKeyBind AutoHarass;
    }
}
