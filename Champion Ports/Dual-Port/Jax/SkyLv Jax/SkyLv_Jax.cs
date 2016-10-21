using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{

    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    using System.Linq;
    using System.Collections.Generic;

    internal class SkyLv_Jax
    {

        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite = new Spell(SpellSlot.Unknown, 600);

        public static Vector3 WardPos;

        public static List<Spell> SpellList = new List<Spell>();

        public static List<AIHeroClient> Enemies = new List<AIHeroClient>(), Allies = new List<AIHeroClient>();

        public const string ChampionName = "Jax";

        public static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        public static readonly string[] Monsters =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", "TT_NGolem5",
            "TT_NGolem2", "TT_NWolf6", "TT_NWolf3","TT_NWraith1", "TT_Spider"
        };

        public SkyLv_Jax()
        {

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
            E = new Spell(SpellSlot.E, 375);
            R = new Spell(SpellSlot.R);

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                Ignite.Slot = ignite.Slot;

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menu = new Menu("SkyLv " + ChampionName + " By LuNi", "SkyLv " + ChampionName + " By LuNi", true);

            Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            Menu.AddSubMenu(new Menu("Combo", "Combo"));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddSubMenu(new Menu("Q Settings Harass", "Q Settings Harass"));
            Menu.SubMenu("Harass").AddSubMenu(new Menu("W Settings Harass", "W Settings Harass"));
            Menu.SubMenu("Harass").AddSubMenu(new Menu("E Settings Harass", "E Settings Harass"));

            Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Menu.SubMenu("LaneClear").AddSubMenu(new Menu("Q Settings LaneClear", "Q Settings LaneClear"));
            Menu.SubMenu("LaneClear").AddSubMenu(new Menu("W Settings LaneClear", "W Settings LaneClear"));
            Menu.SubMenu("LaneClear").AddSubMenu(new Menu("E Settings LaneClear", "E Settings LaneClear"));

            Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Menu.SubMenu("JungleClear").AddSubMenu(new Menu("Q Settings JungleClear", "Q Settings JungleClear"));
            Menu.SubMenu("JungleClear").AddSubMenu(new Menu("W Settings JungleClear", "W Settings JungleClear"));
            Menu.SubMenu("JungleClear").AddSubMenu(new Menu("E Settings JungleClear", "E Settings JungleClear"));

            Menu.AddSubMenu(new Menu("Flee", "Flee"));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Jax.UsePacketCast", "Use PacketCast").SetValue(false));

            Menu.AddSubMenu(new Menu("Drawings", "Drawings"));

            Menu.AddToMainMenu();

            new AfterAttack();
            new KillSteal();
            new JungleSteal();
            new OnUpdateFeatures();
            new Flee();
            new WardTrick();
            new Combo();
            new Harass();
            new JungleClear();
            new LaneClear();
            new PotionManager();
            new SpellLeveler();
            new Draw();
            new SkinChanger();
            new FountainMoves();
        }
    }
}