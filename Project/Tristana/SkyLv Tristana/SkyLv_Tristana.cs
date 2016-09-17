using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{

    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    using System.Linq;
    using System.Collections.Generic;

    internal class SkyLv_Tristana
    {

        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite = new Spell(SpellSlot.Unknown, 600);
        public static SpellSlot FlashSlot;

        public static int lastQ, lastW, lastE, lastR, lastAA;
        public static int lastInsec;
        public static Vector3 REndPosition;

        public static bool ERKSState = false;
        public static bool InsecState = false;

        public static List<Spell> SpellList = new List<Spell>();

        public const string ChampionName = "Tristana";

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

        public SkyLv_Tristana()
        {

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 620);
            R = new Spell(SpellSlot.R, 620);

            W.SetSkillshot(0.35f, 250f, 1400f, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            FlashSlot = Player.GetSpellSlot("summonerflash");

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                Ignite.Slot = ignite.Slot;

            Menu = new Menu("SkyLv " + ChampionName + " By LuNi", "SkyLv " + ChampionName + " By LuNi", true);

            Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            Menu.AddSubMenu(new Menu("Combo", "Combo"));

            Menu.AddSubMenu(new Menu("Insec", "Insec"));

            Menu.AddSubMenu(new Menu("Harass", "Harass"));

            Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));

            Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));

            Menu.AddSubMenu(new Menu("Drawings", "Drawings"));

            Menu.AddToMainMenu();

            new SpellTimer();
            new KillSteal();
            new JungleSteal();
            new Interrupter();
            new AfterAttack();
            new AntiGapCLoser();
            new Insec();
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