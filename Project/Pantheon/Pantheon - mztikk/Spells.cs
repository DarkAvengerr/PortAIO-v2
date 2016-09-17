using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal static class Spells
    {
        #region Properties

        internal static Spell E { get; private set; }

        internal static Spell Q { get; private set; }

        internal static Spell R { get; private set; }

        internal static Items.Item RavHydra { get; private set; }

        internal static Items.Item Tiamat { get; private set; }

        internal static Items.Item TitHydra { get; private set; }

        internal static Spell W { get; private set; }

        #endregion

        #region Methods

        internal static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E, 620f);
            R = new Spell(SpellSlot.R, 5500f);
            Q.SetTargetted(0.2f, 1500f);
            W.SetTargetted(0.2f, 1500f);
            E.SetSkillshot(0.25f, (float)(35 * Math.PI / 180), 800f, false, SkillshotType.SkillshotCone);

            Tiamat = new Items.Item(ItemData.Tiamat_Melee_Only.Id, 325f);
            RavHydra = new Items.Item(ItemData.Ravenous_Hydra_Melee_Only.Id, 325f);
            TitHydra = new Items.Item(ItemData.Titanic_Hydra_Melee_Only.Id, 325f);
        }

        #endregion
    }
}