using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using Prediction = SPrediction.Prediction;

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
                || Menu.Item("Ziggs.Harass.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            switch (Menu.Item("Ziggs.Harass.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Common();
                    break;
                case 1:
                    SPrediction();
                    break;
            }
        }

        private void SPrediction()
        {
            switch (Menu.Item("Ziggs.Harass.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.SPredictionOutput(Target).HitChance >= HitChance.Medium)
                    {
                        spell.Spell.Cast(spell.SPredictionOutput(Target).CastPosition);
                    }
                    break;

                case 1:
                    if (spell.SPredictionOutput(Target).HitChance >= HitChance.High)
                    {
                        spell.Spell.Cast(spell.SPredictionOutput(Target).CastPosition);
                    }
                    break;

                case 2:
                    if (spell.SPredictionOutput(Target).HitChance >= HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(spell.SPredictionOutput(Target).CastPosition);
                    }
                    break;
            }
        }

        private void Common()
        {
            switch (Menu.Item("Ziggs.Harass.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.Prediction(Target).Hitchance >= HitChance.Medium)
                    {
                        spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                    }
                    break;

                case 1:
                    if (spell.Prediction(Target).Hitchance >= HitChance.High)
                    {
                        spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                    }
                    break;

                case 2:
                    if (spell.Prediction(Target).Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                    }
                    break;
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

            Menu.AddItem(new MenuItem("Ziggs.Harass.Q.Prediction", "Prediction: ").SetValue(new StringList(new[] { "Common", "SPrediction" })));

            Menu.AddItem(new MenuItem("Ziggs.Harass.Q.Hitchance", "Hitchance: ").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));

            Menu.AddItem(new MenuItem("Ziggs.Harass.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
