using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lux.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Prediction.HitChance;

    internal sealed class RKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        public RKillsteal(RSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || (Menu.Item("Lux.Killsteal.R.Immobile").GetValue<bool>() && spell.Prediction(Target).Hitchance < SebbyLib.Movement.HitChance.Immobile)
                || Menu.Item("Lux.Killsteal.R.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var predictionHealth = HealthPrediction.GetHealthPrediction(Target, (int)spell.Spell.Delay + Game.Ping / 2);

            if (predictionHealth > spell.GetDamage(Target))
            {
                return;
            }

            switch (Menu.Item("Lux.Killsteal.R.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.Prediction(Target).Hitchance >= SebbyLib.Movement.HitChance.High)
                    {
                        spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                    }
                    break;
                case 1:
                    if (spell.Prediction(Target).Hitchance >= SebbyLib.Movement.HitChance.VeryHigh)
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

            Menu.AddItem(new MenuItem("Lux.Killsteal.R.Immobile", "Only If Can Hit").SetValue(false));

            Menu.AddItem(new MenuItem("Lux.Killsteal.R.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "High", "Very High" })));

            Menu.AddItem(new MenuItem("Lux.Killsteal.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
