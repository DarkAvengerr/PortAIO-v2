using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ExorAIO.Utilities
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The Bools class.
    /// </summary>
    internal class Bools
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Defines whether the player has a deadly mark.
        /// </summary>
        public static bool HasDeadlyMark()
            =>
                !Invulnerable.Check(GameObjects.Player, DamageType.True, false)
                && GameObjects.Player.HasBuff("zedrtargetmark") || GameObjects.Player.HasBuff("summonerexhaust")
                || GameObjects.Player.HasBuff("fizzmarinerdoombomb") || GameObjects.Player.HasBuff("vladimirhemoplague")
                || GameObjects.Player.HasBuff("mordekaiserchildrenofthegrave");

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
            return target.MoveSpeed < 150 || target.HasBuff("rebirth") || target.HasBuff("chronorevive")
                   || target.HasBuff("lissandrarself") || target.HasBuff("teleport_target")
                   || target.HasBuff("woogletswitchcap") || target.HasBuff("zhonyasringshield")
                   || target.HasBuff("aatroxpassivedeath") || IsValidStun(target as AIHeroClient)
                   || IsValidSnare(target as AIHeroClient) || target.HasBuffOfType(BuffType.Flee)
                   || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Charm)
                   || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Suppression)
                   || (target as AIHeroClient).IsCastingInterruptableSpell(true);
        }

        /// <summary>
        ///     Returns true if the target is a perfectly valid rend target.
        /// </summary>
        public static bool IsPerfectRendTarget(Obj_AI_Base target)
        {
            var hero = target as AIHeroClient;
            if (hero != null && Invulnerable.Check(hero))
            {
                return false;
            }

            return target.IsValidTarget(Vars.E.Range) && target.HasBuff("kalistaexpungemarker");
        }

        /// <summary>
        ///     Gets a value indicating whether a determined root is worth cleansing.
        /// </summary>
        public static bool IsValidSnare(AIHeroClient target)
        {
            return
                target.Buffs.Any(
                    b =>
                    b.Type == BuffType.Snare && !Vars.InvalidSnareCasters.Contains(((AIHeroClient)b.Caster).ChampionName));
        }

        /// <summary>
        ///     Gets a value indicating whether a determined stun is worth cleansing.
        /// </summary>
        public static bool IsValidStun(AIHeroClient target)
        {
            return
                target.Buffs.Any(
                    b =>
                    b.Type == BuffType.Stun && !Vars.InvalidStunCasters.Contains(((AIHeroClient)b.Caster).ChampionName));
        }

        /// <summary>
        ///     Gets a value indicating whether BuffType is worth cleansing.
        /// </summary>
        public static bool ShouldCleanse(AIHeroClient target)
            =>
                GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(1500f))
                && !Invulnerable.Check(GameObjects.Player, DamageType.True, false)
                && (target.HasBuffOfType(BuffType.Flee) || target.HasBuffOfType(BuffType.Charm)
                    || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Knockup)
                    || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Polymorph)
                    || target.HasBuffOfType(BuffType.Suppression));

        #endregion
    }
}