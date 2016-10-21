using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
{

    using LeagueSharp;
    using LeagueSharp.Common;

    using System.Linq;
    using System.Collections.Generic;

    internal class SkyLv_Evelynn
    {

        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite = new Spell(SpellSlot.Unknown, 600);

        public static List<Spell> SpellList = new List<Spell>();

        public static List<AIHeroClient> Enemies = new List<AIHeroClient>(), Allies = new List<AIHeroClient>();

        public const string ChampionName = "Evelynn";

        public static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        public SkyLv_Evelynn()
        {

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 500f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 250f);
            R = new Spell(SpellSlot.R, 650f);

            R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

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

            Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));

            Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));

            Menu.AddSubMenu(new Menu("Drawings", "Drawings"));

            Menu.AddToMainMenu();

            new KillSteal();
            new JungleSteal();
            new OnUpdateFeatures();
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