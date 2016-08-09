using EloBuddy; namespace RethoughtLib.LogicProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Geometry = LeagueSharp.Common.Geometry;
    using MinionTypes = LeagueSharp.Common.MinionTypes;

    /// <summary>
    ///     Logic Provider being responsible for Turret specific logics
    /// </summary>
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

        #region Public Methods and Operators

        // TODO
        /// <summary>
        ///     Determines whether the specified position is safe.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public bool IsSafePosition(Vector3 position)
        {
            try
            {
                if (!position.UnderTurret(true))
                {
                    return true;
                }

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

                    if (LeagueSharp.Common.Utility.IsValidTarget(target, float.MaxValue, false))
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
        ///     Called when [target].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private void OnTarget(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Turret)) return;
                
            if (args.Target != null)
            {
                //this.turretTarget[sender.NetworkId] = args.Target;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Counts the attackable units in range.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public int CountAttackableUnitsInRange(Vector3 position, float range)
        {
            var minions =
                MinionManager.GetMinions(position, range, MinionTypes.All, MinionTeam.NotAllyForEnemy)
                    .Where(minion => minion.IsValidTarget());
            var heroes =
                HeroManager.Allies.Where(
                    ally =>
                    Geometry.Distance(ally, position) <= range + ally.BoundingRadius && LeagueSharp.Common.Utility.IsValidTarget(ally)
                    && !ally.IsMe);

            return minions.Count() + heroes.Count();
        }

        /// <summary>
        ///     Initializes the cache.
        /// </summary>
        private void InitializeCache()
        {
            foreach (var obj in ObjectManager.Get<Obj_AI_Turret>().Where(turret => !turret.IsAlly && turret.Health > 0))
            {
                if (!this.turretCache.ContainsKey(obj.NetworkId))
                {
                    this.turretCache.Add(obj.NetworkId, obj);
                    this.turretTarget.Add(obj.NetworkId, null);
                }
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:Update" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
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

                if (value.IsDead || (int)value.Health == 0 || !value.IsValid)
                {
                    this.turretCache.Remove(entry.Key);
                }
            }

            //foreach (var obj in ObjectManager.Get<Obj_AI_Turret>().Where(turret => !turret.IsAlly && turret.Health > 0))
            //{
            //    if (!turretCache.ContainsKey(obj.NetworkId))
            //    {
            //        turretCache.Add(obj.NetworkId, obj);
            //        turretTarget.Add(obj.NetworkId, null);
            //    }
            //}
        }

        #endregion
    }
}