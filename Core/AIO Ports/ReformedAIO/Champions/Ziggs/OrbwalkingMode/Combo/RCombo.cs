using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Damage;
    using ReformedAIO.Champions.Ziggs.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SPrediction;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        private readonly ZiggsDamage damage;

        public RCombo(RSpell spell, ZiggsDamage damage)
        {
            this.spell = spell;
            this.damage = damage;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Ziggs.Combo.R.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || (Menu.Item("Ziggs.Combo.R.Killable").GetValue<bool>() && damage.GetComboDamage(Target) * 1.33 < Target.Health))
            {
                return;
            }


            spell.Spell.CastIfWillHit(Target, Menu.Item("Ziggs.Combo.R.Hit").GetValue<Slider>().Value);

            switch (Menu.Item("Ziggs.Combo.R.Prediction").GetValue<StringList>().SelectedIndex)
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
            switch (Menu.Item("Ziggs.Combo.R.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.CastIfWillHit(Target, 1);
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

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Hit", "Use if X hit: ").SetValue(new Slider(3, 1, 5)));

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Killable", "Use when target killable").SetValue(true));

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Prediction", "Prediction: ").SetValue(new StringList(new[] { "Common", "SPrediction" }, 1)));

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Hitchance", "Hitchance: ").SetValue(new StringList(new[] {"Target Position", "High", "Very High" })));

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));

            
        }
    }
}
