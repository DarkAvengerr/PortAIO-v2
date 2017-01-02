using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Prediction.HitChance;

    internal sealed class RKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        public RKillsteal(RSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null 
                || Target.Health > spell.Spell.GetDamage(Target)
                || (Menu.Item("Ziggs.Killsteal.R.Safety").GetValue<bool>() && Target.CountEnemiesInRange(900) >= 1)
                || Menu.Item("Ziggs.Killsteal.R.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

           // spell.Delay(Target);

            if (spell.OKTW(Target).Hitchance >= HitChance.VeryHigh)
            {
                spell.Spell.Cast(spell.OKTW(Target).CastPosition);
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

            Menu.AddItem(new MenuItem("Ziggs.Killsteal.R.Safety", "Check Allies").SetValue(true));

            Menu.AddItem(new MenuItem("Ziggs.Killsteal.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
