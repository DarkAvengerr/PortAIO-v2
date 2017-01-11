using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Olaf.OrbwalkingMode.Mixed
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using ReformedAIO.Library.Spell_Information;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QMixed : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QMixed(QSpell spell)
        {
            this.spell = spell;
        }

        private SpellInformation spellInfo;

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range - 70, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent))
            {
                return;
            }

            var pred = spell.Spell.GetPrediction(Target);

            switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        spell.Spell.Cast(pred.CastPosition + 70);
                    }
                    break;
                case 1:
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spell.Spell.Cast(pred.CastPosition + 70);
                    }
                    break;
                case 2:
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(pred.CastPosition + 70);
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

            Menu.AddItem(
                new MenuItem("Hitchance", "Hitchance").SetValue(
                    new StringList(new[] { "Medium", "High", "Very High" }, 1)));

            // Menu.AddItem(new MenuItem("Distance", "Shortened Throw Distance").SetValue(new Slider(10, 0, 50)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));

            spellInfo = new SpellInformation();
        }
    }
}