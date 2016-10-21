using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.Perma_Active.Killsteal
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Core.Spells;

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
                || (Menu.Item("Safety").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(rSpell.Spell.Range) > 1))
            {               // Soz for lazy 'safety' check xd cba
                return;
            }

            var wPred = rSpell.Spell.GetPrediction(Target);

            if (wPred.Hitchance > HitChance.Medium)
            {
                rSpell.Spell.Cast(Target);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= this.OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += this.OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem("Safety", "Safety Check").SetValue(true));
        }
    }
}
