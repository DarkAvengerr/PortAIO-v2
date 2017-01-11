using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Movement.HitChance;

    internal sealed class QKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QKillsteal(QSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null 
                || Target.Health > spell.GetDamage(Target)
                || (!Menu.Item("LeeSin.Killsteal.Q.Mode").GetValue<bool>() && spell.HasQ2(Target))
                || (Menu.Item("LeeSin.Killsteal.Q.Safety").GetValue<bool>() && spell.HasQ2(Target) && ObjectManager.Player.CountEnemiesInRange(2000) >= 2)
                || spell.Prediction(Target).Hitchance < HitChance.High)
            {
                return;
            }

            spell.Spell.Cast(spell.Prediction(Target).CastPosition);
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

            Menu.AddItem(new MenuItem("LeeSin.Killsteal.Q.Mode", "Use Q2").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Killsteal.Q.Safety", "Safety Check").SetValue(true));
        }
    }
}
