using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Damage;
    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        private readonly LeeSinStatistisks statistisks;

        public RCombo(RSpell spell, LeeSinStatistisks statistisks)
        {
            this.spell = spell;
            this.statistisks = statistisks;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null)
            {
                return;
            }

            spell.Rectangle = new Geometry.Polygon.Rectangle(Target.Position, ObjectManager.Player.Position.Extend(Target.Position, 1200), 100);

            var target = HeroManager.Enemies.FirstOrDefault(x => spell.Rectangle.IsInside(x));

            var hitCount = HeroManager.Enemies.Count(x => spell.Rectangle.IsInside(x));

            if (target != null && (hitCount >= Menu.Item("LeeSin.Combo.R.Hit").GetValue<Slider>().Value || target.Health < spell.GetDamage(Target)))
            {
                spell.Spell.CastOnUnit(target);
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

            Menu.AddItem(new MenuItem("LeeSin.Combo.R.Hit", "Use If X Hit:").SetValue(new Slider(2, 2, 5)));
        }
    }
}
