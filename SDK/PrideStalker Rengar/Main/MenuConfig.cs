#region

using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace PrideStalker_Rengar.Main
{
   public class MenuConfig
    {
        public Menu TargetSelectorMenu;
        private const string MenuName = "PrideStalker Rengar";
        public static Menu MainMenu { get; set; } = new Menu(MenuName, MenuName, true);

        public static void Load()
        {
            // Combo
            ComboMenu = MainMenu.Add(new Menu("ComboMenu", "Combo"));
            ComboMode = ComboMenu.Add(new MenuList<string>("ComboMode", "Combo Mode", new[] { "Gank", "Triple Q", "Ap Combo", "OneShot" }));

            // Misc
            Misc = MainMenu.Add(new Menu("Misc", "Misc"));
            KillStealSummoner = Misc.Add(new MenuBool("KillStealSummoner", "KillSteal Summoner", true));
            UseItem = Misc.Add(new MenuBool("UseItem", "Use Items", true));
            StackLastHit = Misc.Add(new MenuBool("StackLastHit", "Stack In Lasthit", true));
            ChangeComboMode = Misc.Add(new MenuKeyBind("ChangeComboMode", "Change ComboMode", System.Windows.Forms.Keys.L, KeyBindType.Press));
            Passive = Misc.Add(new MenuKeyBind("Passive", "Passive", System.Windows.Forms.Keys.G, KeyBindType.Toggle));
                
            // Draw
            Draw = MainMenu.Add(new Menu("Draw", "Draw"));
            DrawCombo = Draw.Add(new MenuBool("DrawCombo", "Draw ComboMode", true));
            DrawAnim = Draw.Add(new MenuBool("DrawAnim", "Draw Animation", true));
            DrawHelp = Draw.Add(new MenuBool("DrawHelp", "Draw Tips", true));
            Dind = Draw.Add(new MenuBool("dind", "Damage Indicator", true));
            EngageDraw = Draw.Add(new MenuBool("EngageDraw", "Draw Engage", true));

            // Skin
            Skin = MainMenu.Add(new Menu("SkinChanger", "SkinChanger"));
            UseSkin = Skin.Add(new MenuBool("UseSkin", "Use SkinChanger"));
            SkinChanger = Skin.Add(new MenuList<string>("Skins", "Skins", new[] { "Default", "Headhunter Rengar", "Night Hunter Rengar", "SSW Rengar" }));

            MainMenu.Attach();
        }
        public static Menu ComboMenu;
        public static Menu Misc;
        public static Menu Draw;
        public static Menu Skin;

        public static MenuList<string> ComboMode;
        public static MenuList<string> SkinChanger;

        public static MenuKeyBind Passive;
        public static MenuKeyBind ChangeComboMode;

        public static MenuBool StackLastHit;
        public static MenuBool UseSkin;
        public static MenuBool KillStealSummoner;
        public static MenuBool UseItem;
        public static MenuBool DrawCombo;
        public static MenuBool DrawAnim;
        public static MenuBool DrawHelp;
        public static MenuBool Dind;
        public static MenuBool EngageDraw;
    }
}
