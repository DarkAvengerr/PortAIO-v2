using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EHarass(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Ziggs.Harass.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            switch (Menu.Item("Ziggs.Harass.E.Prediction").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Common();
                    break;
                case 1:
                    SPrediction();
                    break;
            }
        }

        private void Common()
        {
            switch (Menu.Item("Ziggs.Harass.E.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast(Target.ServerPosition);
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

        private void SPrediction()
        {
            switch (Menu.Item("Ziggs.Harass.E.Hitchance").GetValue<StringList>().SelectedIndex)
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

            Menu.AddItem(new MenuItem("Ziggs.Harass.E.Prediction", "Prediction: ").SetValue(new StringList(new[] { "Common", "SPrediction" })));

            Menu.AddItem(new MenuItem("Ziggs.Harass.E.Hitchance", "Hitchance: ").SetValue(new StringList(new[] { "Target", "High", "Very High" })));

            Menu.AddItem(new MenuItem("Ziggs.Harass.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
