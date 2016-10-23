namespace ElVarusRevamped.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElVarusRevamped.Components.Spells;
    using ElVarusRevamped.Enumerations;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    using Collision = LeagueSharp.Common.Collision;

    /// <summary>
    ///     The misc.
    /// </summary>
    internal static class Misc
    {
        #region Methods

        /// <summary>
        ///     Spell Q.
        /// </summary>
        public static SpellQ SpellQ;

        /// <summary>
        ///     Spell E.
        /// </summary>
        public static SpellE SpellE;

        /// <summary>
        ///     Spell R.
        /// </summary>
        public static SpellR SpellR;

        /// <summary>
        ///     The Passive.
        /// </summary>
        public static Spell BlightedQuiver { get; set; }

        /// <summary>
        ///     
        /// </summary>
        public static int LastQ, LastE;

        /// <summary>
        ///     Gets the target W stacks
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static int GetWStacks(Obj_AI_Base target)
        {
            try
            {
                return target.GetBuffCount("varuswdebuff");
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@Misc.cs: Can not return target - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     Gets the Q Collision count.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="castPos"></param>
        /// <returns></returns>
        internal static int GetQCollisionsCount(AIHeroClient target, Vector3 castPos)
        {
            var input = new PredictionInput
            {
                Unit = target,
                Radius = SpellQ.SpellObject.Width,
                Delay = SpellQ.SpellObject.Delay,
                Speed = SpellQ.SpellObject.Speed,
                CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions }
            };
            return
                Collision.GetCollision(
                    new List<Vector3> { ObjectManager.Player.Position.Extend(castPos, SpellQ.Range + SpellQ.Width) }, input).Count;
        }
        /// <summary>
        ///     Gets if Q is killable.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="collisions"></param>
        /// <returns></returns>
        internal static bool QIsKillable(AIHeroClient target, int collisions)
        {
            return target.Health + target.HPRegenRate / 2f < GetQDamage(target, collisions);
        }

        /// <summary>
        ///     Gets the Q damage.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="collisions"></param>
        /// <returns></returns>
        internal static float GetQDamage(AIHeroClient target, int collisions)
        {
            if (Misc.SpellQ.SpellObject.Level == 0)
            {
                return 0;
            }
            var chargePercentage = Misc.SpellQ.Range / Misc.SpellQ.MaxRange;
            var damage =
                (float)
                    (new float[] { 10, 46, 83, 120, 156 }[Misc.SpellQ.SpellObject.Level - 1] +
                     new float[] { 5, 23, 41, 60, 78 }[Misc.SpellQ.SpellObject.Level - 1] * chargePercentage +
                     chargePercentage * (ObjectManager.Player.TotalAttackDamage + ObjectManager.Player.TotalAttackDamage * .6));

            var minimum = damage / 100f * 33f;
            for (var i = 0; i < collisions; i++)
            {
                var reduce = damage / 100f * 15f;
                if (damage - reduce < minimum)
                {
                    damage = minimum;
                    break;
                }
                damage -= reduce;
            }
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, damage);
        }

        /// <summary>
        ///     Gets a target from the common target selector.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="damageType">
        ///     The damage type.
        /// </param>
        /// <returns>
        ///     <see cref="Obj_AI_Hero" />
        /// </returns>
        internal static AIHeroClient GetTarget(float range, TargetSelector.DamageType damageType)
        {
            try
            {
                return TargetSelector.GetTarget(range, damageType);
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@Misc.cs: Can not return target - {0}", e);
                throw;
            }
        }

        #endregion
    }
}