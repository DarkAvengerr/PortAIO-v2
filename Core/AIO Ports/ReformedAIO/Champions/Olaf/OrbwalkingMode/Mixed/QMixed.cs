using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Olaf.OrbwalkingMode.Mixed
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QMixed : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QMixed(QSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent))
            {
                return;
            }

            var prediction = spell.Spell.GetPrediction(Target, true);

            switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (prediction.Hitchance >= HitChance.Medium)
                    {
                        spell.Spell.Cast(prediction.CastPosition.Extend(ObjectManager.Player.Position, -Menu.Item("Distance").GetValue<Slider>().Value));
                    }
                    break;
                case 1:
                    if (prediction.Hitchance >= HitChance.High)
                    {
                        spell.Spell.Cast(prediction.CastPosition.Extend(ObjectManager.Player.Position, -Menu.Item("Distance").GetValue<Slider>().Value));
                    }
                    break;
                case 2:
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(prediction.CastPosition.Extend(ObjectManager.Player.Position, -Menu.Item("Distance").GetValue<Slider>().Value));
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

            Menu.AddItem(new MenuItem("Hitchance", "Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 2)));

            Menu.AddItem(new MenuItem("Distance", "Shortened Throw Distance").SetValue(new Slider(20, 0, 100)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(40, 0, 100)));
        }
    }
}
