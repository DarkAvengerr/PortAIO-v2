using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Movement.HitChance;

    internal sealed class QHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QHarass(QSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || (Target.UnderTurret(true) && !spell.IsQ1))
            {
                return;
            }

            if (spell.IsQ1)
            {
                var prediction = spell.Prediction(Target);

                switch (Menu.Item("LeeSin.Harass.Q.Hitchance").GetValue<StringList>().SelectedIndex)
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

            if (spell.HasQ2(Target) && Menu.Item("LeeSin.Harass.Q.Q2").GetValue<bool>())
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

            Menu.AddItem(new MenuItem("LeeSin.Harass.Q.Q2", "Use Q2?").SetValue(false));

            Menu.AddItem(new MenuItem("LeeSin.Harass.Q.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "High", "Very High" })));
        }
    }
}
