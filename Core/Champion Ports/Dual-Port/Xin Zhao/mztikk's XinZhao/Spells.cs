using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Spells
    {
        #region Static Fields

        internal static Spell Smite;

        internal static SpellSlot SmiteSlot = SpellSlot.Unknown;

        #endregion

        #region Properties

        internal static Spell E { get; private set; }

        internal static SpellDataInst Flash { get; private set; }

        internal static float FlashRange { get; } = 425;

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
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 180);

            Flash = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(args => args.SData.Name == "SummonerFlash");

            Tiamat = new Items.Item((int)ItemId.Tiamat_Melee_Only, 325);
            RavHydra = new Items.Item((int)ItemId.Ravenous_Hydra_Melee_Only, 325);
            TitHydra = new Items.Item(3748, 325);
            var firstOrDefault =
                ObjectManager.Player.Spellbook.Spells.FirstOrDefault(
                    x => x.Name.Contains("smite") || x.Name.Contains("Smite"));
            if (firstOrDefault != null)
            {
                SmiteSlot = firstOrDefault.Slot;
                Smite = new Spell(SmiteSlot, 700);
            }
        }

        #endregion
    }
}