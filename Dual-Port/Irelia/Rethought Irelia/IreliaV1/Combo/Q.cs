using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1.Combo
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;

    using Rethought_Irelia.IreliaV1.DamageCalculator;
    using Rethought_Irelia.IreliaV1.Spells;

    using SharpDX;

    #endregion

    internal class Q : OrbwalkingChild
    {
        #region Constants

        /// <summary>
        ///     The baitrange
        /// </summary>
        private const float Baitrange = 100f;

        #endregion

        #region Fields

        private readonly IDamageCalculator damageCalculator;

        /// <summary>
        ///     Gets or sets the last rites logic provider.
        /// </summary>
        /// <value>
        ///     The logic provider.
        /// </value>
        private readonly IreliaQ ireliaQ;

        /// <summary>
        ///     The target
        /// </summary>
        private AIHeroClient target;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Q" /> class.
        /// </summary>
        /// <param name="ireliaQ">The Q logic</param>
        /// <param name="damageCalculator">The damage calculator</param>
        public Q(IreliaQ ireliaQ, IDamageCalculator damageCalculator)
        {
            this.ireliaQ = ireliaQ;
            this.damageCalculator = damageCalculator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Name { get; set; } = "Q";

        /// <summary>
        ///     Gets or sets the spell priority.
        /// </summary>
        /// <value>
        ///     The spell priority.
        /// </value>
        public int SpellPriority { get; set; } = 2;

        #endregion

        #region Methods

        /// <summary>
        ///     Called when [disable].
        /// </summary>
        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= this.OnGameUpdate;
        }

        /// <summary>
        ///     Called when [enable]
        /// </summary>
        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += this.OnGameUpdate;
        }

        /// <summary>
        ///     Called when [load].
        /// </summary>
        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddItem(new MenuItem(this.Path + "." + "pathfinding", "Gapclosing / Pathfinding").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Path + "." + "movementprediction", "Movement Prediction").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Path + "." + "oneversusone", "One versus one logic").SetValue(true));

            this.Menu.AddItem(
                new MenuItem(this.Path + "." + "noturretdive", "Don't dive turrets").SetValue(
                    new KeyBind('G', KeyBindType.Toggle)));

            this.Menu.AddItem(
                new MenuItem(this.Path + "." + "baitenemy", "Bait target to use escaping ability").SetValue(true)
                    .SetTooltip(
                        "Tries to gapclose very close to the target first, so the target uses a gapclosing spell too, then you dash onto your enemy."));

            this.Menu.AddItem(
                new MenuItem(this.Path + "." + "minrangetogapclose", "Don't gapclose if closer than X units").SetValue(
                    new Slider((int)ObjectManager.Player.AttackRange, 0, (int)this.ireliaQ.Spell.Range)));
        }

        /// <summary>
        ///     Gets the movement prediction.
        /// </summary>
        private Vector3 GetMovementPrediction()
        {
            if (!this.Menu.Item(this.Path + "." + "movementprediction").GetValue<bool>() || this.target == null) return Vector3.Zero;

            var gapclosePath = this.ireliaQ.GetPath(ObjectManager.Player.ServerPosition, this.target.ServerPosition);

            var expectedTime = 0f;

            if (gapclosePath == null || gapclosePath.Any()) return Vector3.Zero;

            for (var i = 0; i < gapclosePath.Count - 1; i++)
            {
                if (gapclosePath[i] == null || gapclosePath[i + 1] == null) continue;

                expectedTime += gapclosePath[i].Distance(gapclosePath[i + 1]) / this.ireliaQ.Spell.Speed;
            }

            var pred = Prediction.GetPrediction(this.target, expectedTime);

            if (pred != null)
            {
                return pred.CastPosition;
            }

            return Vector3.Zero;
        }

        /// <summary>
        ///     Baits the enemy
        /// </summary>
        private void LogicBaitEnemy()
        {
            if (!this.Menu.Item(this.Path + "." + "baitenemy").GetValue<bool>() || this.target == null
                || ObjectManager.Player.ServerPosition.Distance(this.target.ServerPosition)
                <= this.ireliaQ.Spell.Range - Baitrange) return;

            var possibleUnits =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        x =>
                        this.ireliaQ.WillReset(x)
                        && x.Distance(this.target) <= ObjectManager.Player.Distance(this.target));

            this.ireliaQ.Spell.Cast(possibleUnits.MinOrDefault(x => x.Distance(this.target)));
        }

        /// <summary>
        ///     Logic to finish enemies with Q for the reset.
        /// </summary>
        private void LogicFinisher()
        {
            if (this.ireliaQ.WillReset(this.target))
            {
                this.ireliaQ.Spell.Cast(this.target);
            }
        }

        /// <summary>
        ///     Logic one versus one
        /// </summary>
        private void LogicOneVersusOne()
        {
            if (!this.Menu.Item(this.Path + "." + "oneversusone").GetValue<bool>()
                || this.target.GetEnemiesInRange(1000).Count > 1) return;

            if (this.damageCalculator.GetDamage(this.target) > this.target.Health
                && ObjectManager.Player.Distance(this.target) >= ObjectManager.Player.AttackRange)
            {
                this.ireliaQ.Spell.Cast(this.target);
            }
        }

        /// <summary>
        ///     Pathfinding
        /// </summary>
        private void LogicPathfinding()
        {
            if (!this.Menu.Item(this.Path + "." + "pathfinding").GetValue<bool>()) return;

            var end = this.GetMovementPrediction();

            if (end == Vector3.Zero)
            {
                end = Game.CursorPos;
            }

            var path = this.ireliaQ.GetPath(ObjectManager.Player.ServerPosition, end);

            if (path == null || !path.Any()) return;

            this.ireliaQ.Spell.Cast(path.FirstOrDefault());
        }

        /// <summary>
        ///     Raises the <see cref="E:GameUpdate" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnGameUpdate(EventArgs args)
        {
            if (!this.CheckGuardians()) return;

            this.target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical, false);

            if (this.target == null) return;

            if (this.Menu.Item(this.Path + "." + "noturretdive").GetValue<KeyBind>().Active && this.target.UnderTurret())
            {
                this.target = null;
            }

            if (this.target == null) return;

            this.LogicOneVersusOne();

            this.LogicFinisher();

            this.LogicBaitEnemy();

            if (ObjectManager.Player.Distance(this.target)
                <= this.Menu.Item(this.Path + "." + "minrangetogapclose").GetValue<Slider>().Value) return;

            this.LogicPathfinding();
        }

        #endregion
    }
}