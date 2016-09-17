using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iKalistaReborn.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Collision = LeagueSharp.Common.Collision;
    using Color = System.Drawing.Color;

    /// <summary>
    ///     The Helper class
    /// </summary>
    internal static class Helper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the list of minions currently between the source and target
        /// </summary>
        /// <param name="source">
        ///     The Source
        /// </param>
        /// <param name="targetPosition">
        ///     The Target Position
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        public static List<Obj_AI_Base> GetCollisionMinions(AIHeroClient source, Vector3 targetPosition)
        {
            var input = new PredictionInput
                            {
                                Unit = source, Radius = SpellManager.Spell[SpellSlot.Q].Width, 
                                Delay = SpellManager.Spell[SpellSlot.Q].Delay, 
                                Speed = SpellManager.Spell[SpellSlot.Q].Speed, 
                                CollisionObjects = new[] { CollisionableObjects.Minions }
                            };

            return
                Collision.GetCollision(new List<Vector3> { targetPosition }, input)
                    .OrderBy(x => x.Distance(source))
                    .ToList();
        }

        public static ColorBGRA ToSharpDxColor(this Color c)
        {
            return new ColorBGRA(c.R, c.G, c.B, c.A);
        }

        /// <summary>
        ///     Gets the targets current health including shield damage
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetHealthWithShield(this Obj_AI_Base target)
            => target.Health + target.AttackShield > 0 ? target.AttackShield : 0; // TODO shield when fixed.

        /// <summary>
        ///     Gets the rend buff
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="BuffInstance" />.
        /// </returns>
        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
            =>
                target.Buffs.Find(
                    b => b.Caster.IsMe && b.IsValid && b.DisplayName.ToLowerInvariant() == "kalistaexpungemarker");

        /// <summary>
        ///     Gets the current <see cref="BuffInstance" /> Count of Expunge
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GetRendBuffCount(this Obj_AI_Base target)
            => target.Buffs.Count(x => x.Name == "kalistaexpungemarker");

        public static float GetRendDamage(Obj_AI_Base target)
            =>
                Kalista.Menu.Item("com.ikalista.misc.damage").GetValue<StringList>().SelectedIndex == 0
                    ? SpellManager.Spell[SpellSlot.E].GetDamage(target)
                    : Damages.GetRendDamage(target);

        /// <summary>
        ///     Checks if a target has the Expunge <see cref="BuffInstance" />
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasRendBuff(this Obj_AI_Base target) => target?.GetRendBuff() != null;

        /// <summary>
        ///     Checks if the given target has an invulnerable buff
        /// </summary>
        /// <param name="target1">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasUndyingBuff(this Obj_AI_Base target1)
        {
            var target = target1 as AIHeroClient;

            if (target == null) return false;

            // Tryndamere R
            if (target.ChampionName == "Tryndamere"
                && target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValid && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return true;
            }

            // TODO poppy
            return false;
        }

        /// <summary>
        ///     TODO The is mob killable.
        /// </summary>
        /// <param name="target">
        ///     TODO The target.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsMobKillable(this Obj_AI_Base target) => IsRendKillable(target);

        /// <summary>
        ///     Checks if the given target is killable
        /// </summary>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            if (target.IsInvulnerable || !target.HasBuff("kalistaexpungemarker"))
            {
                return false;
            }

            double baseDamage = Kalista.Menu.Item("com.ikalista.misc.damage").GetValue<StringList>().SelectedIndex == 0
                                    ? SpellManager.Spell[SpellSlot.E].GetDamage(target)
                                    : Damages.GetRendDamage(target);

            return (float)baseDamage >= target.GetHealthWithShield();
        }

        #endregion
    }
}