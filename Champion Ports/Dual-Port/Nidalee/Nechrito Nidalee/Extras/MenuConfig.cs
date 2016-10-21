using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Nidalee
{
    class MenuConfig : Core
    {
        public static Menu TargetSelectorMenu;
        private const string MenuName = "Nechrito Nidalee";
        public static Menu Menu { get; set; } = new Menu(MenuName, MenuName, true);
        public static void Load()
        {
            var orbwalker = new Menu("Orbwalker", "rorb");
            var ks = new Menu("KillSteal", "KillSteal");
            var jngl = new Menu("Jungle", "Jungle");
            var heal = new Menu("Heal", "Heal Manager");
            var flee = new Menu("Flee", "Flee");
            var draw = new Menu("Draw", "Draw");
            var misc = new Menu("Misc", "Misc");

            // Targetselector
            TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Menu.AddSubMenu(TargetSelectorMenu);
            // Orbwalker
            
            Orb = new Orbwalking.Orbwalker(orbwalker);
            Menu.AddSubMenu(orbwalker);

            ks.AddItem(new MenuItem("SpellsKS", "KS Spells").SetValue(true)).SetTooltip("Uses Any spell to killsteal");
            ks.AddItem(new MenuItem("ComboSmite", "Smite").SetValue(true)).SetTooltip("Uses Smite when killable");
            ks.AddItem(new MenuItem("ComboIgnite", "Ignite").SetValue(true)).SetTooltip("Uses Smite when killable");
            Menu.AddSubMenu(ks);

            
            jngl.AddItem(new MenuItem("jnglQ", "Use Javelin Mana %").SetValue(new Slider(15, 0, 100))).SetTooltip("Cast Q if mana% more than slider value");                  // Human
            jngl.AddItem(new MenuItem("jnglHeal", "Auto Heal").SetValue(new Slider(15, 0, 95))).SetTooltip("Heals, good for faster jungle clear");
            Menu.AddSubMenu(jngl);

            heal.AddItem(new MenuItem("allyHeal", "Heal Allies Hp <= %").SetValue(new Slider(45, 0, 80))).SetTooltip("Heals Nearby Allies");                  // Human
            heal.AddItem(new MenuItem("SelfHeal", "Self Heal Hp <= %").SetValue(new Slider(80, 0, 90))).SetTooltip("Put Higher value here than on allies");
            heal.AddItem(new MenuItem("ManaHeal", "Mana <= %").SetValue(new Slider(20, 0, 100))).SetTooltip("Mana percent");
            Menu.AddSubMenu(heal);

           
            misc.AddItem(new MenuItem("Gapcloser", "Gapcloser").SetValue(true));
            Menu.AddSubMenu(misc);

            draw.AddItem(new MenuItem("dind", "Draw damage indicator").SetValue(true)).SetTooltip("Draws damage");
            draw.AddItem(new MenuItem("EngageDraw", "Engage Range").SetValue(true)).SetTooltip("Draws QR Range");
            draw.AddItem(new MenuItem("fleeDraw", "Draw Flee Spots").SetValue(true)).SetTooltip("Draws Flee Positions");
            Menu.AddSubMenu(draw);

          flee.AddItem(new MenuItem("FleeMouse", "Flee").SetValue(new KeyBind('A', KeyBindType.Press))).SetTooltip("BETA!!");
          Menu.AddSubMenu(flee);

            Menu.AddToMainMenu();
        }
        public static bool ComboSmite => Menu.Item("ComboSmite").GetValue<bool>();
        public static bool ComboIgnite => Menu.Item("ComboIgnite").GetValue<bool>();
        public static bool dind => Menu.Item("dind").GetValue<bool>();
        public static bool fleeDraw => Menu.Item("fleeDraw").GetValue<bool>();
        public static bool SpellsKS => Menu.Item("SpellsKS").GetValue<bool>();
        public static bool EngageDraw => Menu.Item("EngageDraw").GetValue<bool>();
        public static bool Gapcloser => Menu.Item("Gapcloser").GetValue<bool>();

        public static bool FleeMouse => Menu.Item("FleeMouse").GetValue<KeyBind>().Active;

        public static Slider ManaHeal => Menu.Item("ManaHeal").GetValue<Slider>();
        public static Slider SelfHeal => Menu.Item("SelfHeal").GetValue<Slider>();
        public static Slider allyHeal => Menu.Item("allyHeal").GetValue<Slider>();
        public static Slider jnglQ => Menu.Item("jnglQ").GetValue<Slider>();
        public static Slider jnglHeal => Menu.Item("jnglHeal").GetValue<Slider>();
    }
}
