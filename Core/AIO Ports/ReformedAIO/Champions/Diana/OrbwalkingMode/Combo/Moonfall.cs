using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class Moonfall : OrbwalkingChild
    {
        #region Fields

        private LogicAll logic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[E] Moonfall";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Interrupter2.OnInterruptableTarget -= Interrupt;

            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Interrupter2.OnInterruptableTarget += Interrupt;

            AntiGapcloser.OnEnemyGapcloser += Gapcloser;

            Game.OnUpdate += OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            Menu.AddItem(new MenuItem("EInterrupt", "Interrupt").SetValue(true));

            Menu.AddItem(new MenuItem("EGapcloser", "Anti-Gapcloser").SetValue(true));

            Menu.AddItem(new MenuItem("EKillable", "Use Only If Killable By Combo").SetValue(true));

            Menu.AddItem(new MenuItem("ERange", "E Range").SetValue(new Slider(300, 350)));

            Menu.AddItem(
                new MenuItem("MinTargets", "Min Targets In Range").SetValue(new Slider(2, 0, 5)));

            Menu.AddItem(new MenuItem("EMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            logic = new LogicAll();
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("EGapcloser").GetValue<bool>()) return;

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
            if (!Menu.Item("EInterrupt").GetValue<bool>()) return;

            if (!sender.IsEnemy || !Variables.Spells[SpellSlot.E].IsReady() || !sender.IsValidTarget()
                || sender.IsZombie) return;

            if (sender.IsValidTarget(Variables.Spells[SpellSlot.E].Range)) Variables.Spells[SpellSlot.E].Cast();
        }

        private void moonfall()
        {
            var target = TargetSelector.GetTarget(
                Menu.Item("ERange").GetValue<Slider>().Value,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (Menu.Item("MinTargets").GetValue<Slider>().Value
                > target.CountEnemiesInRange(Menu.Item("ERange").GetValue<Slider>().Value)) return;

            if (Menu.Item("EKillable").GetValue<bool>()
                && logic.ComboDmg(target) * 1.2 < target.Health)
            {
                return;
            }

            Variables.Spells[SpellSlot.E].Cast();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()) return;

            moonfall();
        }

        #endregion
    }
}