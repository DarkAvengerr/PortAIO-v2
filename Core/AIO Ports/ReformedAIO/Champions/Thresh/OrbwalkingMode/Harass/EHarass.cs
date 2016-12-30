using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Thresh.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Thresh.Core.Damage;
    using ReformedAIO.Champions.Thresh.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        private readonly ThreshDamage damage;

        public EHarass(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private static bool Dangerous(Obj_AI_Base target)
        {
            return (ObjectManager.Player.CountAlliesInRange(1350)
                == 0
                && !target.UnderTurret(true))
                || target.HealthPercent > 76;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Thresh.Harass.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Menu.Item("Thresh.Harass.E.Push").GetValue<bool>() && Dangerous(Target))
            {
                spell.Spell.Cast(Target.Position);
            }

            if (Menu.Item("Thresh.Harass.E.Health").GetValue<Slider>().Value <= Target.HealthPercent || Target.UnderTurret(true))
            {
                spell.Spell.Cast(ObjectManager.Player.Position.Extend(Target.Position, ObjectManager.Player.Distance(Target.Position) + 400));
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

            Menu.AddItem(new MenuItem("Thresh.Harass.E.Push", "Push If Dangerous").SetValue(true));

            Menu.AddItem(new MenuItem("Thresh.Harass.E.Health", "Pull If X Health < ").SetValue(new Slider(60, 0, 100)));

            Menu.AddItem(new MenuItem("Thresh.Harass.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
