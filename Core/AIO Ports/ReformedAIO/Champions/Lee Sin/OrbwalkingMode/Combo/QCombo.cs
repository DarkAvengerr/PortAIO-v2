using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Movement.HitChance;

    internal sealed class QCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QCombo(QSpell spell)
        {
            this.spell = spell;
        }

        private static AIHeroClient Target => TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Target == null)
            {
                return;
            }

            Cast();
        }

        private void Cast()
        {
            if (spell.IsQ1)
            {
                spell.SmiteCollision(Target);

                var prediction = spell.Prediction(Target);

                switch (Menu.Item("LeeSin.Combo.Q.Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            spell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
                    case 1:
                        if (prediction.Hitchance >= HitChance.VeryHigh)
                        {
                            spell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
                }
            }

            if (spell.HasQ2(Target) && !(Target.UnderTurret(true) && Menu.Item("LeeSin.Combo.Q.Turret").GetValue<bool>()))
            {
                spell.Spell.Cast();
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("LeeSin.Combo.Q.Turret", "Don't Q2 Into Turret").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Combo.Q.Hitchance", "Hitchance:").SetValue(new StringList(new []{ "High", "Very High"}, 1)));
        }
    }
}
