using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengar
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Standards
    {
        #region Static Fields

        protected static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                                         {
                                                                             {
                                                                                 Spells.Q,
                                                                                 new Spell(
                                                                                 SpellSlot.Q,
                                                                                 Orbwalking.GetRealAutoAttackRange(
                                                                                     Player) + 100)
                                                                             },
                                                                             {
                                                                                 Spells.W,
                                                                                 new Spell(
                                                                                 SpellSlot.W,
                                                                                 400 + Player.BoundingRadius)
                                                                             },
                                                                             {
                                                                                 Spells.E,
                                                                                 new Spell(
                                                                                 SpellSlot.E,
                                                                                 1000 + Player.BoundingRadius)
                                                                             },
                                                                             { Spells.R, new Spell(SpellSlot.R, 2000) }
                                                                         };

        public static int LastSwitch;

        public static Orbwalking.Orbwalker Orbwalker;

        protected static SpellSlot Ignite;

        #endregion

        #region Public Properties

        public static int Ferocity => (int)ObjectManager.Player.Mana;

        public static bool HasPassive => Player.Buffs.Any(x => x.Name.ToLower().Contains("rengarpassivebuff"));

        public static AIHeroClient Player => ObjectManager.Player;

        public static bool RengarR => Player.Buffs.Any(x => x.Name.ToLower().Contains("rengarrbuff"));

        public static string ScriptVersion => typeof(Rengar).Assembly.GetName().Version.ToString();

        #endregion

        #region Public Methods and Operators

        public static float IgniteDamage(AIHeroClient target)
        {
            return Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready
                       ? 0f
                       : (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public static bool IsActive(string menuItem) => MenuInit.Menu.Item(menuItem).IsActive();

        #endregion

        #region Methods

        public static StringList IsListActive(string menuItem) => MenuInit.Menu.Item(menuItem).GetValue<StringList>();

        #endregion
    }
}