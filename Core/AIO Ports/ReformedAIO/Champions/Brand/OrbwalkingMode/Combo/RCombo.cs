using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Brand.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Brand.Core.Damage;
    using ReformedAIO.Champions.Brand.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        private readonly BrandDamage damage;

        public RCombo(RSpell spell, BrandDamage brandDamage)
        {
            this.spell = spell;
            this.damage = brandDamage;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Brand.Combo.R.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (ObjectManager.Player.CountEnemiesInRange(430) >= Menu.Item("Brand.Combo.R.Hit").GetValue<Slider>().Value 
                || Menu.Item("Brand.Combo.R.Killable").GetValue<bool>() && Target.Health < damage.GetComboDamage(Target))
            {
                spell.Spell.CastOnUnit(Target);
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

            Menu.AddItem(new MenuItem("Brand.Combo.R.Killable", "Use If Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Brand.Combo.R.Hit", "Use If X Hit: ").SetValue(new Slider(3, 1, 5)));

            Menu.AddItem(new MenuItem("Brand.Combo.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
