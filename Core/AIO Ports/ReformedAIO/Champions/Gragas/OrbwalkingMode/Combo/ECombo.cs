namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SPrediction;

    #endregion

    internal class ECombo : ChildBase
    {
        #region Fields

        private LogicAll logic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[E] Body Slam";

        #endregion

        #region Methods

        private readonly Orbwalking.Orbwalker orbwalker;

        public ECombo(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }


        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    logic = new LogicAll();
        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

        protected override sealed void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem(Name + "EKillable", "Only If Killable").SetValue(false));

            Menu.AddItem(new MenuItem(Menu.Name + "ERange", "E Range ").SetValue(new Slider(835, 0, 850)));

            Menu.AddItem(new MenuItem(Menu.Name + "EMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            logic = new LogicAll();
        }

        private void BodySlam()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item(Menu.Name + "ERange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (Menu.Item(Menu.Name + "EKillable").GetValue<bool>()
                && logic.ComboDmg(target) < target.Health) return;

            var ePred = Variable.Spells[SpellSlot.E].GetSPrediction(target);

            if (target.HasBuffOfType(BuffType.Knockback)) return;

            if (ePred.HitChance < HitChance.Medium) return;

            Variable.Spells[SpellSlot.E].Cast(ePred.CastPosition);
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variable.Spells[SpellSlot.E].IsReady()) return;

            if (Menu.Item(Menu.Name + "EMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            BodySlam();
        }

        #endregion
    }
}