using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lux.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Prediction.HitChance;

    internal sealed class QCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QCombo(QSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || !spell.Collision(Target)
                || Menu.Item("Lux.Combo.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            switch (Menu.Item("Lux.Combo.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.Prediction(Target).Hitchance >= HitChance.High)
                    {
                        spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                    }
                    break;
                case 1:
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

            Menu.AddItem(new MenuItem("Lux.Combo.Q.Hitchance", "Hitchance:").SetValue(new StringList(new[] {"High", "Very High"})));
         
            Menu.AddItem(new MenuItem("Lux.Combo.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
