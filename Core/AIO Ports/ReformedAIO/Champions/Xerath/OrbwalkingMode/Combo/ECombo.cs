using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Xerath.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ECombo(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Magical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Xerath.Combo.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            switch (Menu.Item("Xerath.Combo.E.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.CastIfHitchanceEquals(Target, HitChance.High);
                    break;
                case 1:
                    spell.Spell.CastIfHitchanceEquals(Target, HitChance.VeryHigh);
                    break;
                case 2:
                    spell.Spell.Cast(Target);
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

            Menu.AddItem(new MenuItem("Xerath.Combo.E.Hitchance", "Hitchance: ").SetValue(new StringList(new[] { "High", "Very High", "Position" }, 2)));

            Menu.AddItem(new MenuItem("Xerath.Combo.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
