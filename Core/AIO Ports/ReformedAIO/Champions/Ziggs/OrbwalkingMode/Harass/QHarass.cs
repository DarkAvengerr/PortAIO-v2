using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SPrediction;

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

            switch (Menu.Item("Ziggs.Harass.Q.Prediction").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Common();
                    break;
                case 1:
                    SPrediction();
                    break;
                case 2:
                    OKTW();
                    break;
            }
        }

        private void SPrediction()
        {
            switch (Menu.Item("Ziggs.Harass.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.SPredictionCast(Target, HitChance.Medium);
                    break;

                case 1:
                    spell.Spell.SPredictionCast(Target, HitChance.High);
                    break;

                case 2:
                    spell.Spell.SPredictionCast(Target, HitChance.VeryHigh);
                    break;
            }
        }

        private void Common()
        {
            switch (Menu.Item("Ziggs.Harass.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.CastIfHitchanceEquals(Target, HitChance.Medium);
                    break;

                case 1:
                    spell.Spell.CastIfHitchanceEquals(Target, HitChance.High);
                    break;

                case 2:
                    spell.Spell.CastIfHitchanceEquals(Target, HitChance.VeryHigh);
                    break;
            }
        }

        private void OKTW()
        {
            if (spell.OKTW(Target) != null && spell.OKTW(Target).Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
            {
                spell.Spell.Cast(spell.OKTW(Target).CastPosition);
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

            Menu.AddItem(new MenuItem("Ziggs.Harass.Q.Prediction", "Prediction: ").SetValue(new StringList(new[] { "Common", "SPrediction", "OKTW" }, 1)));

            Menu.AddItem(new MenuItem("Ziggs.Harass.Q.Hitchance", "Hitchance: ").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));

            Menu.AddItem(new MenuItem("Ziggs.Harass.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
