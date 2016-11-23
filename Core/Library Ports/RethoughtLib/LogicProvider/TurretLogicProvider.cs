//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.LogicProvider
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    /// <summary>
    ///     Logic Provider being responsible for Turret specific logics
    /// </summary>
    /// // TODO UPDATE (ChildBase)
    internal class TurretLogicProvider
    {
        #region Fields

        /// <summary>
        ///     The turret cache
        /// </summary>
        private readonly Dictionary<int, Obj_AI_Turret> turretCache = new Dictionary<int, Obj_AI_Turret>();

        /// <summary>
        ///     The turret target
        /// </summary>
        private readonly Dictionary<int, AttackableUnit> turretTarget = new Dictionary<int, AttackableUnit>();

        #endregion

        #region Constructors and Destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TurretLogicProvider" /> class.
        /// </summary>
        public TurretLogicProvider()
        {
            this.InitializeCache();

            Game.OnUpdate += this.OnUpdate;
            //Obj_AI_Base.OnTarget += this.OnTarget;
        }

        #endregion

        #endregion

        #region Public Methods and Operators

        // TODO
        /// <summary>
        ///     Determines whether the specified position is safe.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsSafePosition(Vector3 position)
        {
            try
            {
                if (!position.UnderTurret(true)) return true;

                /*
                var smallestDistance = float.MaxValue;
                var turret = new Obj_AI_Turret();

                foreach (var entry in turretCache)
                {
                    if (entry.Value.Distance(Variables.Player) <= smallestDistance)
                    {
                        smallestDistance = entry.Value.Distance(Variables.Player);
                        turret = entry.Value;
                    }
                }

                if (position.Distance(turret.ServerPosition) >= turret.AttackRange)
                {
                    return true;
                }

                if (turretTarget.Any())
                {
                    var target = turretTarget[turret.NetworkId];

                    if (Utility.IsValidTarget(target, float.MaxValue, false))
                    {
                        // We can onehit the turret, there are not much enemies near and we won't die from the next turret shot
                        if (turret.Health + turret.AttackShield <= Variables.Player.GetAutoAttackDamage(turret)
                            && turret.CountEnemiesInRange(turret.AttackRange) < 2
                            && Variables.Player.Health > turret.GetAutoAttackDamage(Variables.Player) * 2
                            && Geometry.Distance(position, turret.ServerPosition) <= Variables.Player.AttackRange)
                        {
                            return true;
                        }

                        // Turret is focusing something else and there are new targets that are not we in range
                        if (target != null && !target.IsMe
                            && this.CountAttackableUnitsInRange(turret.Position, turret.AttackRange) > 1
                            && target.Health >= turret.GetAutoAttackDamage((Obj_AI_Base)target))
                        {
                            return true;
                        }
                    }
                }
                */
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        ///     Counts the attackable units in range.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int CountAttackableUnitsInRange(Vector3 position, float range)
        {
            var minions =
                MinionManager.GetMinions(position, range, MinionTypes.All, MinionTeam.NotAllyForEnemy)
                    .Where(minion => minion.IsValidTarget());
            var heroes =
                HeroManager.Allies.Where(
                    ally =>
                        (Geometry.Distance(ally, position) <= range + ally.BoundingRadius)
                        && Utility.IsValidTarget(ally) && !ally.IsMe);

            return minions.Count() + heroes.Count();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the cache.
        /// </summary>
        private void InitializeCache()
        {
            foreach (
                var obj in ObjectManager.Get<Obj_AI_Turret>().Where(turret => !turret.IsAlly && (turret.Health > 0)))
                if (!this.turretCache.ContainsKey(obj.NetworkId))
                {
                    this.turretCache.Add(obj.NetworkId, obj);
                    this.turretTarget.Add(obj.NetworkId, null);
                }
        }

        /// <summary>
        ///     Called when [target].
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.
        /// </param>
        private void OnTarget(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Turret)) return;

            //if (args.Target != null) this.turretTarget[sender.NetworkId] = args.Target;
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnUpdate(EventArgs args)
        {
            this.UpdateCache();
        }

        /// <summary>
        ///     Updates the cache.
        /// </summary>
        private void UpdateCache()
        {
            foreach (var entry in this.turretCache.ToList())
            {
                var value = entry.Value;

                if (value.IsDead || ((int)value.Health == 0) || !value.IsValid) this.turretCache.Remove(entry.Key);
            }

            // foreach (var obj in ObjectManager.Get<Obj_AI_Turret>().Where(turret => !turret.IsAlly && turret.Health > 0))
            // {
            // if (!turretCache.ContainsKey(obj.NetworkId))
            // {
            // turretCache.Add(obj.NetworkId, obj);
            // turretTarget.Add(obj.NetworkId, null);
            // }
            // }
        }

        #endregion
    }
}