using EloBuddy; namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ashe.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class RCombo : ChildBase
    {
        #region Fields

        private RLogic rLogic;

        #endregion

        #region Constructors and Destructors

        public RCombo(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Public Properties

        public override string Name { get; set; }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Interrupter2.OnInterruptableTarget -= this.Interrupt;
            AntiGapcloser.OnEnemyGapcloser -= this.Gapcloser;
            Events.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Interrupter2.OnInterruptableTarget += this.Interrupt;
            AntiGapcloser.OnEnemyGapcloser += this.Gapcloser;
            Events.OnUpdate += this.OnUpdate;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.rLogic = new RLogic();
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddItem(
                new MenuItem(this.Menu.Name + "RDistance", "Max Distance").SetValue(new Slider(1100, 0, 1500))
                    .SetTooltip("Too Much And You Might Not Get The Kill"));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "RMana", "Mana %").SetValue(new Slider(10, 0, 100)));

            this.Menu.AddItem(
                new MenuItem(this.Name + "SemiR", "Semi-Auto R Key").SetValue(new KeyBind('A', KeyBindType.Press))
                    .SetTooltip("Select Your Target First"));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "RKillable", "Only When Killable").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "RSafety", "Safety Check").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "Interrupt", "Interrupt").SetValue(true));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "Gapclose", "Gapcloser").SetValue(true));
        }

        private void CrystalArrow()
        {
            var target = TargetSelector.GetTarget(
                this.Menu.Item(this.Menu.Name + "RDistance").GetValue<Slider>().Value,
                TargetSelector.DamageType.Physical,
                false);

            if (target == null || !target.IsValid || target.IsInvulnerable || target.IsDashing()) return;

            if (this.Menu.Item(this.Menu.Name + "RSafety").GetValue<bool>() && !this.rLogic.SafeR(target)) return;

            if (this.Menu.Item(this.Menu.Name + "RKillable").GetValue<bool>() && !this.rLogic.Killable(target)) return;

            Variable.Spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.High);
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!this.Menu.Item(this.Menu.Name + "Gapclose").GetValue<bool>()) return;

            var target = gapcloser.Sender;

            if (target == null) return;

            if (!target.IsEnemy || !Variable.Spells[SpellSlot.R].IsReady() || !target.IsValidTarget() || target.IsZombie) return;

            if (target.IsValidTarget(800)) Variable.Spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        private void Interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!this.Menu.Item(this.Menu.Name + "Interrupt").GetValue<bool>()) return;

            if (!sender.IsEnemy || !Variable.Spells[SpellSlot.R].IsReady() || sender.IsZombie) return;

            if (sender.IsValidTarget(1200)) Variable.Spells[SpellSlot.R].CastIfHitchanceEquals(sender, HitChance.VeryHigh);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.R].IsReady()) return;

            this.SemiR();

            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;

            if (this.Menu.Item(this.Menu.Name + "RMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            this.CrystalArrow();
        }

        private void SemiR()
        {
            if (!this.Menu.Item(this.Menu.Name + "SemiR").GetValue<KeyBind>().Active) return;

            if (Variable.Player.CountEnemiesInRange(1500) == 0) return;

            var target = TargetSelector.GetSelectedTarget();
            if (target == null) return;

            var pred = Variable.Spells[SpellSlot.R].GetPrediction(target);
            if (pred.Hitchance >= HitChance.High)
            {
                Variable.Spells[SpellSlot.R].Cast(pred.CastPosition);
            }
        }

        #endregion
    }
}