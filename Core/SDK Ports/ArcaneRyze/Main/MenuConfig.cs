#region

using LeagueSharp.SDK.UI;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Arcane_Ryze.Main
{
    internal class MenuConfig
    {
        public Menu TargetSelectorMenu;
        private const string MenuName = "Arcane Ryze";
        public static Menu MainMenu { get; set; } = new Menu(MenuName, MenuName, true);
        public static void Load()
        {
            // Combo
            ComboMenu = MainMenu.Add(new Menu("ComboMenu", "Combo"));
            KillStealSummoner = ComboMenu.Add(new MenuBool("KillStealSummoner", "KillSteal Ignite", true));
          //  QCollision = ComboMenu.Add(new MenuBool("QCollision", "Ignore Q Collision", true));

            // Lane
            LaneMenu = MainMenu.Add(new Menu("LaneMenu", "Lane"));
            LaneR = LaneMenu.Add(new MenuBool("LaneR", "Use R"));
            LaneQ = LaneMenu.Add(new MenuBool("LaneQ", "Last Hit Q AA", true));
            LaneMana = LaneMenu.Add(new MenuSliderButton("LaneMana", "Mana Slider", 50, 0, 100, true));

            JungleMenu = MainMenu.Add(new Menu("JungleMenu", "Jungle"));
            JungleR = JungleMenu.Add(new MenuBool("JungleR", "Use R"));

            // Misc
            MiscMenu = MainMenu.Add(new Menu("MiscMenu", "Misc"));
         //   AutoQ = MiscMenu.Add(new MenuBool("AutoQ", "Auto Stack", true));
            UseItems = MiscMenu.Add(new MenuBool("UseItems", "Use Items", true));

            // Draw
            DrawMenu = MainMenu.Add(new Menu("Draw", "Draw"));
            dind = DrawMenu.Add(new MenuBool("dind", "Damage Indicator", true));

            MainMenu.Attach();
        }
        public static Menu ComboMenu;
        public static Menu DrawMenu;
        public static Menu LaneMenu;
        public static Menu JungleMenu;
        public static Menu MiscMenu;
        

        public static MenuKeyBind Flee;

        public static MenuList<string> SkinChanger;

        public static MenuSliderButton LaneMana;

        public static MenuBool JungleR;
        public static MenuBool LaneR;
        public static MenuBool UseItems;
        public static MenuBool LaneQ;
    //    public static MenuBool QCollision;
        public static MenuBool dind;
    //    public static MenuBool AutoQ;
        public static MenuBool UseSkin;
        public static MenuBool KillStealSummoner;
    }
}