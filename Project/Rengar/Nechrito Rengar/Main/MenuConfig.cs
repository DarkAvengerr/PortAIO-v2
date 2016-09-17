using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Rengar.Main
{
    class MenuConfig : Core
    {
        public static Menu TargetSelectorMenu;
        private const string MenuName = "Nechrito Rengar";
        public static Menu Menu { get; set; } = new Menu(MenuName, MenuName, true);
        public static void Load()
        {
            var orbwalker = new Menu("Orbwalker", "rorb");
            var draw = new Menu("Draw", "Draw");
            var misc = new Menu("Misc", "Misc");
            var combo = new Menu("ComboMode", "ComboMode");
            var skin = new Menu("SkinChanger", "SkinChanger");

            TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Menu.AddSubMenu(TargetSelectorMenu);

            Orb = new Orbwalking.Orbwalker(orbwalker);
            Menu.AddSubMenu(orbwalker);

            combo.AddItem(new MenuItem("ComboMode", "Combo Mode").SetValue(new StringList(new[] { "Gank", "Triple Q", "Ap Combo" })));
            Menu.AddSubMenu(combo);

           
            misc.AddItem(new MenuItem("KillStealSummoner", "Killsteal Ignite/Smite")).SetValue(true);
            misc.AddItem(new MenuItem("UseItem", "Use Items")).SetValue(true);
            misc.AddItem(new MenuItem("Passive", "Save Passive")).SetValue(new KeyBind('G', KeyBindType.Toggle));
            Menu.AddSubMenu(misc);
            
            draw.AddItem(new MenuItem("dind", "Damage Indicator")).SetValue(true);
            draw.AddItem(new MenuItem("EngageDraw", "Draw Engage Range")).SetValue(true);
            Menu.AddSubMenu(draw);

            skin.AddItem(new MenuItem("UseSkin", "Use Skinchanger")).SetValue(true).SetTooltip("Toggles Skinchanger");
            skin.AddItem(new MenuItem("Skin", "Choose A Skin!").SetValue(new StringList(new[] { "Default", "Headhunter Rengar", "Night Hunter Rengar", "SSW Rengar" })));
            Menu.AddSubMenu(skin);

            Menu.AddToMainMenu();
        }
        public static bool Passive => Menu.Item("Passive").GetValue<KeyBind>().Active;
        public static bool UseSkin => Menu.Item("UseSkin").GetValue<bool>();
        public static bool KillStealSummoner => Menu.Item("KillStealSummoner").GetValue<bool>();
        public static bool UseItem => Menu.Item("UseItem").GetValue<bool>();
        public static bool dind => Menu.Item("dind").GetValue<bool>();
        public static bool EngageDraw => Menu.Item("EngageDraw").GetValue<bool>();

    }
}