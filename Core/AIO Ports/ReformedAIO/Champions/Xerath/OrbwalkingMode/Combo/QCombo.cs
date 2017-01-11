using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Xerath.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Movement.HitChance;

    internal sealed class QCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QCombo(QSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Magical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || (Menu.Item("Xerath.Combo.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent && !spell.Charging))
            {
                return;
            }

            //Hacks.DisableCastIndicator = spell.Charging;

            if (!spell.Charging)
            {
                spell.Spell.StartCharging();
                return;
            }

            if (Menu.Item("Xerath.Combo.Q.Legit").GetValue<bool>() && !spell.IsInsideRect(Target))
            {
                return;
            }

            switch (Menu.Item("Xerath.Combo.Q.Prediction").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.SDK(Target).Hitchance >= HitChance.High)
                    {
                        spell.Spell.Cast(spell.SDK(Target).CastPosition);
                    }
                    break;
                case 1:

                    if (spell.OKTW(Target).Hitchance >= SebbyLib.Prediction.HitChance.High)
                    {
                        spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                    }
                    break;
                case 2:
                    spell.Spell.Cast(Target.ServerPosition);
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

            Menu.AddItem(new MenuItem("Xerath.Combo.Q.Legit", "Legit Mode").SetValue(false).SetTooltip("Will Q near cursor only"));

            Menu.AddItem(new MenuItem("Xerath.Combo.Q.Prediction", "Prediction: ").SetValue(new StringList(new[] { "SDK", "OKTW", "Target Position" })));

            Menu.AddItem(new MenuItem("Xerath.Combo.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
