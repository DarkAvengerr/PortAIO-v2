using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Thresh.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Thresh.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QCombo(QSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private bool Safe()
        {
            return ObjectManager.Player.CountEnemiesInRange(2000) < ObjectManager.Player.CountAlliesInRange(2000)
                || Target.HealthPercent <= 20;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Thresh.Combo.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || (Menu.Item("Thresh.Combo.Q.Pull").GetValue<bool>() && !Safe() && Target.HasBuff("ThreshQ")))
            {
                return;
            }


            switch (Menu.Item("Thresh.Combo.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.Prediction(Target).Hitchance >= HitChance.High)
                    {
                        Prediction();
                    }
                    break;
                case 1:
                    if (spell.Prediction(Target).Hitchance >= HitChance.VeryHigh)
                    {
                        Prediction();
                    }
                    break;
                case 2:
                    if (spell.Prediction(Target).Hitchance >= HitChance.Immobile)
                    {
                        Prediction();
                    }
                    break;
            }
        }

        private void Prediction()
        {
            switch (Menu.Item("Thresh.Combo.Q.Prediction").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.SDK(Target) != null)
                    {
                        spell.Spell.Cast(spell.SDK(Target).CastPosition);
                    }
                    break;
                case 1:
                    if (spell.OKTW(Target) != null)
                    {
                        spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                    }
                    break;
                case 2:
                    if (Target.IsStunned || spell.Prediction(Target).Hitchance >= HitChance.Immobile)
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

            Menu.AddItem(new MenuItem("Thresh.Combo.Q.Pull", "Q2 If Safe").SetValue(true));

            Menu.AddItem(new MenuItem("Thresh.Combo.Q.Prediction", "Prediction: ").SetValue(new StringList(new[] { "SDK", "OKTW", "Common" })));

            Menu.AddItem(new MenuItem("Thresh.Combo.Q.Hitchance", "Hitchance: ").SetValue(new StringList(new[] { "High", "Very High", "Immobile" }, 1)));
         
            Menu.AddItem(new MenuItem("Thresh.Combo.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
