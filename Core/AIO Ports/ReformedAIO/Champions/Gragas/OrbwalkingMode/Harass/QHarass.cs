using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Prediction.HitChance;

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
                || !spell.Spell.IsReady()
                || Menu.Item("Gragas.Harass.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            spell.ExplodeHandler(Target);

            switch (Menu.Item("Gragas.Harass.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.OKTW(Target).Hitchance >= HitChance.High)
                    {
                        spell.Handle(spell.OKTW(Target).CastPosition);
                    }
                    break;
                case 1:
                    if (spell.OKTW(Target).Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Handle(spell.OKTW(Target).CastPosition);
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

            Menu.AddItem(new MenuItem("Gragas.Harass.Q.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "High", "Very High" })));

            Menu.AddItem(new MenuItem("Gragas.Harass.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
