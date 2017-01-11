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

    internal sealed class EHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EHarass(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || (!Menu.Item("Gragas.Harass.E.Range").GetValue<bool>() && Target.Distance(ObjectManager.Player) > spell.Spell.Range - 150)
                || Menu.Item("Gragas.Harass.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            switch (Menu.Item("Gragas.Harass.E.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.OKTW(Target).Hitchance >= HitChance.High)
                    {
                        spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                    }
                    break;
                case 1:
                    if (spell.OKTW(Target).Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(spell.OKTW(Target).CastPosition);
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

            Menu.AddItem(new MenuItem("Gragas.Harass.E.Range", "Use At Max Range").SetValue(true));

            Menu.AddItem(new MenuItem("Gragas.Harass.E.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "High", "Very High" })));

            Menu.AddItem(new MenuItem("Gragas.Harass.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
