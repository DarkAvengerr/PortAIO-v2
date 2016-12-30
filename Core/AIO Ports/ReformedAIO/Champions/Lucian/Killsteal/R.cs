using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lucian.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class R : OrbwalkingChild
    {
        public override string Name { get; set; } = nameof(R);

        private readonly RSpell rSpell;

        public R(RSpell rSpell)
        {
            this.rSpell = rSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(rSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {                                                                               
            if (Target == null 
                || Target.Health > rSpell.GetDamage(Target)
                || !CheckGuardians()
                || Target.Distance(ObjectManager.Player) > rSpell.Spell.Range
                || (Menu.Item("Killsteal.R.Safety").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(rSpell.Spell.Range) > 1))
            {            
                return;
            }

            var wPred = rSpell.Spell.GetPrediction(Target);

            if (wPred.Hitchance > HitChance.Medium)
            {
                rSpell.Spell.Cast(Target);
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

            Menu.AddItem(new MenuItem("Killsteal.R.Safety", "Safety Check").SetValue(true));
        }
    }
}
