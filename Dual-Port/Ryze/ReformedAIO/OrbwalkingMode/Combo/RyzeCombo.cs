using EloBuddy; namespace ReformedAIO.Champions.Ryze.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ryze.Logic;

    using RethoughtLib.Events;
    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class RyzeCombo : ChildBase
    {
        #region Fields

        private ELogic eLogic;

        #endregion

        #region Public Properties

        public sealed override string Name { get; set; }

        #endregion

        #region Methods

        public RyzeCombo(string name)
        {
            this.Name = name;
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += this.OnUpdate;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.eLogic = new ELogic();

            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            //base.OnLoad(sender, featureBaseEventArgs);

            this.Menu.AddItem(
                new MenuItem(this.Name + "Mode", "Mode").SetValue(
                    new StringList(new[] { "Burst", "Safe", "Automatic" })));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "QMana", "Q Mana %").SetValue(new Slider(0, 0, 50)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "WMana", "W Mana %").SetValue(new Slider(0, 0, 50)));

            this.Menu.AddItem(new MenuItem(this.Menu.Name + "EMana", "E Mana %").SetValue(new Slider(0, 0, 50)));
        }

        private void Burst()
        {
            var target = TargetSelector.GetTarget(975, TargetSelector.DamageType.Magical);

            if (target == null) return;

            if (Variable.Spells[SpellSlot.Q].LSIsReady())
            {
                if (target.IsValid
                    && this.Menu.Item(this.Menu.Name + "QMana").GetValue<Slider>().Value < Variable.Player.ManaPercent)
                {
                    var qpred = Variable.Spells[SpellSlot.Q].GetPrediction(target);
                    if (qpred.Hitchance >= HitChance.Medium)
                    {
                        Variable.Spells[SpellSlot.Q].Cast(qpred.CastPosition);
                    }
                }
            }

            if (Variable.Spells[SpellSlot.E].LSIsReady() && !Variable.Spells[SpellSlot.Q].LSIsReady())
            {
                if (target.LSIsValidTarget(Variable.Spells[SpellSlot.E].Range)
                    && this.Menu.Item(this.Menu.Name + "EMana").GetValue<Slider>().Value < Variable.Player.ManaPercent)
                {
                    Variable.Spells[SpellSlot.E].Cast(target);
                }
            }

            if (!Variable.Spells[SpellSlot.W].LSIsReady() || this.eLogic.RyzeE(target)) return;

            if (!target.LSIsValidTarget(Variable.Spells[SpellSlot.W].Range)
                || !(this.Menu.Item(this.Menu.Name + "WMana").GetValue<Slider>().Value < Variable.Player.ManaPercent)) return;

            Variable.Spells[SpellSlot.W].Cast(target);
        }

        private void OnUpdate(EventArgs args)
        {
            if (Variable.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;

            switch (this.Menu.Item(this.Menu.Name + "Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    {
                        this.Burst();
                        break;
                    }
                case 1:
                    {
                        this.Safe();
                        break;
                    }
                case 2:
                    {
                        var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);

                        if (target == null) return;

                        if (Variable.Player.HealthPercent <= 15 && target.HealthPercent >= 20)
                        {
                            goto case 1;
                        }
                        goto case 0;
                    }
            }
        }

        private void Safe()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);

            if (target == null) return;

            if (Variable.Spells[SpellSlot.E].LSIsReady())
            {
                Variable.Spells[SpellSlot.E].Cast(target);
            }

            if (this.eLogic.RyzeE(target) && Variable.Spells[SpellSlot.W].LSIsReady()
                && target.LSIsValidTarget(Variable.Spells[SpellSlot.W].Range))
            {
                Variable.Spells[SpellSlot.W].Cast(target);
            }

            if (Variable.Spells[SpellSlot.Q].LSIsReady() && !this.eLogic.RyzeE(target))
            {
                Variable.Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
            }
        }

        #endregion
    }
}