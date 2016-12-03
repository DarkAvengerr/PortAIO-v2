
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Utilities
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    /// <summary>
    ///     The Vars class.
    /// </summary>
    internal class Vars
    {
        #region Static Fields

        /// <summary>
        ///     The last tick.
        /// </summary>
        public static int LastTick = 0;

        /// <summary>
        ///     The jungle HP bar offset list.
        /// </summary>
        internal static readonly string[] JungleList =
            {
                "SRU_Dragon_Air", "SRU_Dragon_Fire", "SRU_Dragon_Water",
                "SRU_Dragon_Earth", "SRU_Dragon_Elder", "SRU_Baron",
                "SRU_RiftHerald", "SRU_Red", "SRU_Blue", "SRU_Gromp",
                "Sru_Crab", "SRU_Krug", "SRU_Razorbeak", "SRU_Murkwolf"
            };

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Drawings menu.
        /// </summary>
        public static Menu DrawingsMenu { get; set; }

        /// <summary>
        ///     Gets or sets the E Spell.
        /// </summary>
        public static Spell E { get; set; }

        /// <summary>
        ///     Gets or sets the E2 Spell.
        /// </summary>
        public static Spell E2 { get; set; }

        /// <summary>
        ///     Gets or sets the E Spell menu.
        /// </summary>
        public static Menu EMenu { get; set; }

        /// <summary>
        ///     Gets or sets the loaded champion.
        /// </summary>
        public static bool IsLoaded { get; set; } = true;

        /// <summary>
        ///     Gets or sets the assembly menu.
        /// </summary>
        public static Menu Menu { get; set; } = new Menu(
            $"aio.{GameObjects.Player.ChampionName.ToLower()}",
            $"[ExorAIO]: {GameObjects.Player.ChampionName}",
            true);

        /// <summary>
        ///     Gets or sets the Miscellaneous menu.
        /// </summary>
        public static Menu MiscMenu { get; set; }

        /// <summary>
        ///     Gets or sets the PowPow Range.
        /// </summary>
        public static Spell PowPow { get; set; }

        /// <summary>
        ///     Gets or sets the Q Spell.
        /// </summary>
        public static Spell Q { get; set; }

        /// <summary>
        ///     Gets or sets the 2nd stage of the Q Spell.
        /// </summary>
        public static Spell Q2 { get; set; }

        /// <summary>
        ///     Gets or sets the Q2 Spell menu.
        /// </summary>
        public static Menu Q2Menu { get; set; }

        /// <summary>
        ///     Gets or sets the Q Spell menu.
        /// </summary>
        public static Menu QMenu { get; set; }

        /// <summary>
        ///     Gets or sets the R Spell.
        /// </summary>
        public static Spell R { get; set; }

        /// <summary>
        ///     Gets or sets the R2 Spell.
        /// </summary>
        public static Spell R2 { get; set; }

        /// <summary>
        ///     Gets or sets the R Spell menu.
        /// </summary>
        public static Menu RMenu { get; set; }

        /// <summary>
        ///     Gets or sets the settings menu.
        /// </summary>
        public static Menu SpellsMenu { get; set; }

        /// <summary>
        ///     Gets or sets the W Spell.
        /// </summary>
        public static Spell W { get; set; }

        /// <summary>
        ///     Gets or sets the W2 Spell menu.
        /// </summary>
        public static Menu W2Menu { get; set; }

        /// <summary>
        ///     Gets or sets the second Whitelist menu.
        /// </summary>
        public static Menu WhiteList2Menu { get; set; }

        /// <summary>
        ///     Gets or sets the first Whitelist menu.
        /// </summary>
        public static Menu WhiteListMenu { get; set; }

        /// <summary>
        ///     Gets or sets the W Spell menu.
        /// </summary>
        public static Menu WMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns true if there is a Wall between X pos and Y pos.
        /// </summary>
        public static bool AnyWallInBetween(Vector3 startPos, Vector3 endPos)
        {
            for (var i = 0; i < startPos.Distance(endPos); i++)
            {
                if (NavMesh.GetCollisionFlags(startPos.Extend(endPos, i)) == CollisionFlags.Wall)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Gets the health with Blitzcrank's Shield support.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The target Health with Blitzcrank's Shield support.
        /// </returns>
        public static float GetRealHealth(Obj_AI_Base target)
        {
            var debuffer = 0f;

            /// <summary>
            ///     Gets the predicted reduction from Blitzcrank Shield.
            /// </summary>
            var hero = target as AIHeroClient;
            if (hero != null)
            {
                if (hero.ChampionName.Equals("Blitzcrank") && !hero.HasBuff("BlitzcrankManaBarrierCD"))
                {
                    debuffer += hero.Mana / 2;
                }
            }
            return target.Health + target.AttackShield + target.HPRegenRate + debuffer;
        }

        /// <summary>
        ///     Gets a value indicating whether BuffType is worth cleansing.
        /// </summary>
        public static bool ShouldCleanse(AIHeroClient target)
            =>
                GameObjects.EnemyHeroes.Any(t => t.IsValidTarget(1500f))
                && !Invulnerable.Check(GameObjects.Player, DamageType.Magical, false)
                && (target.HasBuffOfType(BuffType.Flee) || target.HasBuffOfType(BuffType.Charm)
                    || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Knockup)
                    || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Polymorph)
                    || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Stun));

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the time the unit is immobile untill.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>System.Double.</returns>
        internal static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                    buff.IsActive && Game.Time <= buff.EndTime
                    && (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun
                        || buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        #endregion
    }
}