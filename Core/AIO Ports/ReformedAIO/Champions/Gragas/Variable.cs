namespace ReformedAIO.Champions.Gragas
{
    #region Using Directives

    using System.Collections.Generic;

    using EloBuddy;
    using LeagueSharp.Common;

    #endregion

    internal class Variable
    {
        #region Static Fields

        public static Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell> {
                                                                    {
                                                                        SpellSlot.Q,
                                                                        new Spell(
                                                                        SpellSlot.Q,
                                                                        775f)
                                                                    },
                                                                    {
                                                                        SpellSlot.W,
                                                                        new Spell(
                                                                        SpellSlot.W)
                                                                    },
                                                                    {
                                                                        SpellSlot.E,
                                                                        new Spell(
                                                                        SpellSlot.E,
                                                                        600f)
                                                                    },
                                                                    {
                                                                        SpellSlot.R,
                                                                        new Spell(
                                                                        SpellSlot.R,
                                                                        1050f)
                                                                    }
                                                                };

        #endregion

        #region Public Properties

        public static Orbwalking.Orbwalker Orbwalker { get; internal set; }

        public static AIHeroClient Player => ObjectManager.Player;

        #endregion
    }
}