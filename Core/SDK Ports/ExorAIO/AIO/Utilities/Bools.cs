using EloBuddy; 
using LeagueSharp.SDK; 
namespace ExorAIO.Utilities
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;

    /// <summary>
    ///     The Bools class.
    /// </summary>
    internal class Bools
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets a value indicating whether the player has a sheen-like buff.
        /// </summary>
        public static bool HasSheenBuff()
            =>
                GameObjects.Player.HasBuff("sheen") || GameObjects.Player.HasBuff("LichBane")
                || GameObjects.Player.HasBuff("dianaarcready") || GameObjects.Player.HasBuff("ItemFrozenFist")
                || GameObjects.Player.HasBuff("sonapassiveattack");

        /// <summary>
        ///     Gets a value indicating whether a determined champion has a stackable item.
        /// </summary>
        public static bool HasTear(AIHeroClient target)
            =>
                target.InventoryItems.Any(
                    item =>
                    item.Id.Equals(ItemId.Tear_of_the_Goddess) || item.Id.Equals(ItemId.Archangels_Staff)
                    || item.Id.Equals(ItemId.Manamune) || item.Id.Equals(ItemId.Tear_of_the_Goddess_Quick_Charge)
                    || item.Id.Equals(ItemId.Archangels_Staff_Quick_Charge)
                    || item.Id.Equals(ItemId.Manamune_Quick_Charge));

        /// <summary>
        ///     Gets a value indicating whether a determined champion can move or not.
        /// </summary>
        public static bool IsImmobile(Obj_AI_Base target)
        {
            return !target.IsDashing() && !target.IsMoving && target.MoveSpeed < 150 || target.HasBuff("rebirth")
                   || target.HasBuff("chronorevive") || target.HasBuff("lissandrarself")
                   || target.HasBuff("teleport_target") || target.HasBuff("woogletswitchcap")
                   || target.HasBuff("zhonyasringshield") || target.HasBuff("aatroxpassivedeath")
                   || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare)
                   || target.HasBuffOfType(BuffType.Flee) || target.HasBuffOfType(BuffType.Taunt)
                   || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockup)
                   || target.HasBuffOfType(BuffType.Suppression)
                   || (target as AIHeroClient).IsCastingInterruptableSpell(true);
        }

        #endregion
    }
}