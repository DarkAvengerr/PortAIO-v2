namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class Moonfall : ChildBase
    {
        #region Fields

        private LogicAll logic;

        #endregion

        private readonly Orbwalking.Orbwalker orbwalker;

        public Moonfall(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        #region Public Properties

        public override string Name { get; set; } = "[E] Moonfall";

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
        //    logic = new LogicAll();

        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

        protected override sealed void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem(Name + "EInterrupt", "Interrupt").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "EGapcloser", "Anti-Gapcloser").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "EKillable", "Use Only If Killable By Combo").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "ERange", "E Range").SetValue(new Slider(300, 350)));

            Menu.AddItem(
                new MenuItem(Name + "MinTargets", "Min Targets In Range").SetValue(new Slider(2, 0, 5)));

            Menu.AddItem(new MenuItem(Name + "EMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            logic = new LogicAll();
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item(Menu.Name + "EGapcloser").GetValue<bool>()) return;

            var target = gapcloser.Sender;

            if (target == null) return;

            if (!target.IsEnemy || !Variables.Spells[SpellSlot.E].IsReady() || !target.IsValidTarget()
                || target.IsZombie) return;

            if (
                target.IsValidTarget(
                    Variables.Spells[SpellSlot.E].Range + Variables.Player.BoundingRadius + target.BoundingRadius)) Variables.Spells[SpellSlot.E].Cast();
        }

        private void Interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Menu.Item(Menu.Name + "EInterrupt").GetValue<bool>()) return;

            if (!sender.IsEnemy || !Variables.Spells[SpellSlot.E].IsReady() || !sender.IsValidTarget()
                || sender.IsZombie) return;

            if (sender.IsValidTarget(Variables.Spells[SpellSlot.E].Range)) Variables.Spells[SpellSlot.E].Cast();
        }

        private void moonfall()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item(Menu.Name + "ERange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (Menu.Item(Menu.Name + "MinTargets").GetValue<Slider>().Value
                > target.CountEnemiesInRange(Menu.Item(Menu.Name + "ERange").GetValue<Slider>().Value)) return;

            if (Menu.Item(Menu.Name + "EKillable").GetValue<bool>()
                && logic.ComboDmg(target) * 1.2 < target.Health)
            {
                return;
            }

            Variables.Spells[SpellSlot.E].Cast();
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variables.Spells[SpellSlot.E].IsReady()) return;

            // if (Menu.Item(Menu.Name + "EMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            moonfall();
        }

        #endregion
    }
}