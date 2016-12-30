using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SPrediction;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ECombo(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Ziggs.Combo.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            switch (Menu.Item("Ziggs.Combo.E.Prediction").GetValue<StringList>().SelectedIndex)
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
            spell.Spell.CastIfWillHit(Target, 1);

            spell.Spell.SPredictionCastAoe(2);

            spell.Spell.SPredictionCast(Target, HitChance.VeryHigh);
        }

        private void Common()
        {
            switch (Menu.Item("Ziggs.Combo.E.Hitchance").GetValue<StringList>().SelectedIndex)
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

            Menu.AddItem(new MenuItem("Ziggs.Combo.E.Prediction", "Prediction: ").SetValue(new StringList(new[] { "Common", "SPrediction" }, 1)));

            Menu.AddItem(new MenuItem("Ziggs.Combo.E.Hitchance", "Hitchance: ").SetValue(new StringList(new[] { "Target", "High", "Very High" })));

            Menu.AddItem(new MenuItem("Ziggs.Combo.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
