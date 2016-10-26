namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ashe.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal sealed class RCombo : ChildBase
    {
        #region Fields

        private RLogic rLogic;

        #endregion

        #region Constructors and Destructors

        private readonly Orbwalking.Orbwalker orbwalker;

        public RCombo(string name, Orbwalking.Orbwalker orbwalker)
        {
            Name = name;
            this.orbwalker = orbwalker;
        }

        #endregion

        #region Public Properties

        public override string Name { get; set; }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Interrupter2.OnInterruptableTarget -= Interrupt;
            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Interrupter2.OnInterruptableTarget += Interrupt;
            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
            Game.OnUpdate += OnUpdate;
        }

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    rLogic = new RLogic();
        //}

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(
                new MenuItem(Menu.Name + "RDistance", "Max Distance").SetValue(new Slider(1100, 0, 1500))
                    .SetTooltip("Too Much And You Might Not Get The Kill"));

            Menu.AddItem(new MenuItem(Menu.Name + "RMana", "Mana %").SetValue(new Slider(10, 0, 100)));

            Menu.AddItem(
                new MenuItem(Name + "SemiR", "Semi-Auto R Key").SetValue(new KeyBind('A', KeyBindType.Press))
                    .SetTooltip("Select Your Target First"));

            Menu.AddItem(new MenuItem(Menu.Name + "RKillable", "Only When Killable").SetValue(true));

            Menu.AddItem(new MenuItem(Menu.Name + "RSafety", "Safety Check").SetValue(true));

            Menu.AddItem(new MenuItem(Menu.Name + "Interrupt", "Interrupt").SetValue(true));

            Menu.AddItem(new MenuItem(Menu.Name + "Gapclose", "Gapcloser").SetValue(true));

            rLogic = new RLogic();
        }

        private void CrystalArrow()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item(Menu.Name + "RDistance").GetValue<Slider>().Value,
                TargetSelector.DamageType.Physical,
                false);

            if (target == null || !target.IsValid || target.IsInvulnerable || target.IsDashing()) return;

            if (Menu.Item(Menu.Name + "RSafety").GetValue<bool>() && !rLogic.SafeR(target)) return;

            if (Menu.Item(Menu.Name + "RKillable").GetValue<bool>() && !rLogic.Killable(target)) return;

            Variable.Spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.High);
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item(Menu.Name + "Gapclose").GetValue<bool>()) return;

            var target = gapcloser.Sender;

            if (target == null) return;

            if (!target.IsEnemy || !Variable.Spells[SpellSlot.R].IsReady() || !target.IsValidTarget() || target.IsZombie) return;

            if (target.IsValidTarget(800)) Variable.Spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        private void Interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Menu.Item(Menu.Name + "Interrupt").GetValue<bool>()) return;

            if (!sender.IsEnemy || !Variable.Spells[SpellSlot.R].IsReady() || sender.IsZombie) return;

            if (sender.IsValidTarget(1200)) Variable.Spells[SpellSlot.R].CastIfHitchanceEquals(sender, HitChance.VeryHigh);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.R].IsReady()) return;

            SemiR();

            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;

            if (Menu.Item(Menu.Name + "RMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            CrystalArrow();
        }

        private void SemiR()
        {
            if (!Menu.Item(Menu.Name + "SemiR").GetValue<KeyBind>().Active) return;

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