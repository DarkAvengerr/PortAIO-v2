#region

using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Swiftly_Teemo.Menu
{
    internal class MenuConfig
    {
       
        private const string MenuName = "Swiftly Teemo";
        public static LeagueSharp.SDK.UI.Menu MainMenu { get; set; } = new LeagueSharp.SDK.UI.Menu(MenuName, MenuName, true);
        public static void Load()
        {
            ComboMenu = MainMenu.Add(new LeagueSharp.SDK.UI.Menu("ComboMenu", "Combo"));
            KillStealSummoner = ComboMenu.Add(new MenuBool("KillStealSummoner", "Killsteal Ignite", true));
            TowerCheck = ComboMenu.Add(new MenuBool("TowerCheck", "No R Under Turret", true));

            LaneMenu = MainMenu.Add(new LeagueSharp.SDK.UI.Menu("LaneMenu", "Lane"));
            LaneQ = LaneMenu.Add(new MenuBool("LaneQ", "Last Hit Q AA", true));

            DrawMenu = MainMenu.Add(new LeagueSharp.SDK.UI.Menu("Draw", "Draw"));
            Dind = DrawMenu.Add(new MenuBool("dind", "Damage Indicator", true));
            EngageDraw = DrawMenu.Add(new MenuBool("EngageDraw", "Draw Engage", true));
            DrawR = DrawMenu.Add(new MenuBool("DrawR", "Draw R Prediction"));

            SkinMenu = MainMenu.Add(new LeagueSharp.SDK.UI.Menu("SkinChanger", "SkinChanger"));
            UseSkin = SkinMenu.Add(new MenuBool("UseSkin", "Use SkinChanger"));
            SkinChanger = SkinMenu.Add(new MenuList<string>("Skins", "Skins", new[] { "Default", "Happy Elf Teemo", "Recon Teemo", "Badger Teemo", "Astronaut Teemo", "Cottontail Teemo", "Super Teemo", "Panda Teemo", "Omega Squad Teemo" }));
            Flee = MainMenu.Add(new MenuKeyBind("Flee", "Flee", System.Windows.Forms.Keys.A, KeyBindType.Press));

            MainMenu.Attach();
        }
        public static LeagueSharp.SDK.UI.Menu ComboMenu;
        public static LeagueSharp.SDK.UI.Menu DrawMenu;
        public static LeagueSharp.SDK.UI.Menu LaneMenu;
        public static LeagueSharp.SDK.UI.Menu SkinMenu;

        public static MenuKeyBind Flee;

        public static MenuList<string> SkinChanger;

        public static MenuBool TowerCheck;
        public static MenuBool DrawR;
        public static MenuBool LaneQ;
        public static MenuBool EngageDraw;
        public static MenuBool Dind;
        public static MenuBool UseSkin;
        public static MenuBool KillStealSummoner;
    }
}
