using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Prediction.HitChance;

    internal sealed class EKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EKillsteal(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Target.Health > spell.GetDamage(Target)
                || spell.OKTW(Target).Hitchance < HitChance.High
                || Menu.Item("Gragas.Killsteal.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            spell.Spell.Cast(spell.OKTW(Target).CastPosition);
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

            Menu.AddItem(new MenuItem("Gragas.Killsteal.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
