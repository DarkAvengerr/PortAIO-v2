// TODO: Add Dash End Positions as list. Maybe think about positive things when I change Obj_AI_Base to YasuoConnection or Point.
// TODO: Rework Calculations based on Dash End Positions.

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.CommonEx.Extensions;
    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class LastBreath
    {
        #region Fields

        /// <summary>
        ///     The affected enemies
        /// </summary>
        public List<AIHeroClient> AffectedEnemies = new List<AIHeroClient>();

        /// <summary>
        ///     The R logicprovider
        /// </summary>
        public LastBreathLogicProvider ProviderR = new LastBreathLogicProvider();

        /// <summary>
        ///     The turret logicprovider
        /// </summary>
        public TurretLogicProvider ProviderTurret = new TurretLogicProvider();

        /// <summary>
        ///     The target
        /// </summary>
        public AIHeroClient Target;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastBreath" /> class.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public LastBreath(AIHeroClient unit)
        {
            if (unit != null && unit.IsValidTarget())
            {
                this.Target = unit;
            }

            if (this.Target != null)
            {
                this.AffectedEnemies.Add(this.Target);

                this.StartPosition = GlobalVariables.Player.ServerPosition;

                this.SetEndPosition();
                this.SetAffectedEnemies();
                this.SetTravelDistance();
                this.SetDangerValue();
                this.SetMinRemainingAirboneTime();
                this.SetKnockUpAmount();
                this.SetDamageDealt();
                this.SetPriority();
            }
        }

        public LastBreath() { }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the damage dealt.
        /// </summary>
        /// <value>
        ///     The damage dealt.
        /// </value>
        public float DamageDealt { get; private set; }

        /// <summary>
        ///     Gets the danger value.
        /// </summary>
        /// <value>
        ///     The danger value.
        /// </value>
        public int DangerValue { get; private set; }

        /// <summary>
        ///     Gets the end position.
        /// </summary>
        /// <value>
        ///     The end position.
        /// </value>
        public Vector3 EndPosition { get; private set; }

        /// <summary>
        ///     Gets the enemies in ult.
        /// </summary>
        /// <value>
        ///     The enemies in ult.
        /// </value>
        public int EnemiesInUlt { get; private set; }

        // TODO: Priority High
        /// <summary>
        ///     Gets a value indicating whether this execution is overkill.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this execution is overkill; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverkill { get; private set; }

        // TODO
        /// <summary>
        ///     Gets the mean health.
        /// </summary>
        /// <value>
        ///     The mean health.
        /// </value>
        public float MeanHealth { get; private set; }

        // TODO
        /// <summary>
        ///     Gets the mean health percentage.
        /// </summary>
        /// <value>
        ///     The mean health percentage.
        /// </value>
        public float MeanHealthPercentage { get; private set; }

        /// <summary>
        ///     Gets the minimum remaining airbone time.
        /// </summary>
        /// <value>
        ///     The minimum remaining airbone time.
        /// </value>
        public float MinRemainingAirboneTime { get; private set; }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        /// <value>
        ///     The priority.
        /// </value>
        public int Priority { get; private set; }

        /// <summary>
        ///     Gets the start position.
        /// </summary>
        /// <value>
        ///     The start position.
        /// </value>
        public Vector3 StartPosition { get; }

        /// <summary>
        ///     Gets the travel distance.
        /// </summary>
        /// <value>
        ///     The travel distance.
        /// </value>
        public float TravelDistance { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        public void Draw()
        {
            var color = Color.DeepSkyBlue;

            Drawing.DrawLine(
                Drawing.WorldToScreen(this.StartPosition),
                Drawing.WorldToScreen(this.EndPosition.Extend(GlobalVariables.Player.ServerPosition, 200)),
                1f,
                Color.White);

            Drawing.DrawCircle(this.EndPosition, 200, Color.White);

            foreach (var enemy in this.AffectedEnemies)
            {
                Drawing.DrawCircle(enemy.Position, enemy.BoundingRadius, color);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sets the affected enemies.
        /// </summary>
        private void SetAffectedEnemies()
        {
            try
            {
                if (this.EndPosition == Vector3.Zero || this.EndPosition.CountEnemiesInRange(475) <= 0)
                {
                    return;
                }

                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x => x != this.Target && x.IsAirbone() && !x.IsZombie && x.Distance(this.EndPosition) <= 400))
                {
                    this.AffectedEnemies.Add(enemy);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        ///     Sets the damage dealt.
        /// </summary>
        private void SetDamageDealt()
        {
            if (this.AffectedEnemies != null)
            {
                foreach (var enemy in this.AffectedEnemies)
                {
                    this.DamageDealt += GlobalVariables.Spells[SpellSlot.R].GetDamage(enemy);
                }
            }
        }

        /// <summary>
        ///     Sets the danger value.
        /// </summary>
        private void SetDangerValue()
        {
            try
            {
                foreach (var x in HeroManager.Enemies.Where(x => !x.IsAirbone() && x.Distance(this.EndPosition) <= 750))
                {
                    this.DangerValue += 1;
                }

                if (!this.ProviderTurret.IsSafePosition(this.EndPosition))
                {
                    this.DangerValue += 5;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[SetDangerValue:] " + ex);
            }

            // TODO: Add Skillshots
        }

        /// <summary>
        ///     Sets the end position.
        /// </summary>
        private void SetEndPosition()
        {
            var endPosition = Vector3.Zero;

            if (this.Target == null || !this.Target.IsValid)
            {
                return;
            }

            if (this.Target.UnderTurret(true))
            {
                var turret =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(x => !x.IsAlly && x.Health > 0)
                        .MinOrDefault(x => x.Distance(GlobalVariables.Player));

                if (turret != null && turret.IsValid)
                {
                    var y = this.Target.Distance(turret);

                    endPosition = turret.ServerPosition.Extend(this.Target.ServerPosition,
                        turret.AttackRange + this.Target.BoundingRadius - y);
                }
            }
            else
            {
                endPosition = this.Target.ServerPosition;
            }
            this.EndPosition = endPosition;
        }

        /// <summary>
        ///     Sets the knock up amount.
        /// </summary>
        private void SetKnockUpAmount()
        {
            if (this.AffectedEnemies != null)
            {
                this.EnemiesInUlt = this.AffectedEnemies.Count;
            }
        }

        /// <summary>
        ///     Sets the minimum remaining airbone time.
        /// </summary>
        private void SetMinRemainingAirboneTime()
        {
            if (this.AffectedEnemies != null)
            {
                this.MinRemainingAirboneTime = this.AffectedEnemies.MinOrDefault(x => x.RemainingAirboneTime()).RemainingAirboneTime();
            }
        }

        /// <summary>
        ///     Sets the priority.
        /// </summary>
        private void SetPriority()
        {
            foreach (var enemy in this.AffectedEnemies)
            {
                this.Priority += (int)TargetSelector.GetPriority(enemy);
            }
        }

        /// <summary>
        ///     Sets the travel distance.
        /// </summary>
        private void SetTravelDistance()
        {
            this.TravelDistance = this.StartPosition.Distance(this.EndPosition);
        }

        #endregion
    }
}