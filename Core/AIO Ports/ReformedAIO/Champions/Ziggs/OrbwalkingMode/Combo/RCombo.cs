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
                || (Menu.Item("Ziggs.Combo.R.Killable").GetValue<bool>() && damage.GetComboDamage(Target) < Target.Health))
            {
                return;
            }

            spell.Spell.SPredictionCastAoe(Menu.Item("Ziggs.Combo.R.Hit").GetValue<Slider>().Value);

            spell.Spell.CastIfWillHit(Target, Menu.Item("Ziggs.Combo.R.Hit").GetValue<Slider>().Value);

            switch (Menu.Item("Ziggs.Combo.R.Prediction").GetValue<StringList>().SelectedIndex)
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

        private void OKTW()
        {
            switch (Menu.Item("Ziggs.Combo.R.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast(Target.ServerPosition);
                    break;
                case 1:
                    if (spell.OKTW(Target).Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                    }
                    break;
                case 2:
                    if (spell.OKTW(Target).Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                    }
                    break;
                    
            }
        }

        private void SPrediction()
        {
            if (spell.SPredictionOutput(Target).HitCount >= 1)
            {
                spell.Spell.Cast(spell.SPredictionOutput(Target).CastPosition);
            }
        }

        private void Common()
        {
            switch (Menu.Item("Ziggs.Combo.R.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast(Target.ServerPosition);
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

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Hit", "Use if X hit: ").SetValue(new Slider(3, 2, 5)));

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Killable", "Use when target killable").SetValue(true));

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Prediction", "Prediction: ").SetValue(new StringList(new[] { "Common", "SPrediction", "OKTW" }, 2)));

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Hitchance", "Hitchance: ").SetValue(new StringList(new[] {"Target Position", "High", "Very High" })));

            Menu.AddItem(new MenuItem("Ziggs.Combo.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));

            
        }
    }
}
