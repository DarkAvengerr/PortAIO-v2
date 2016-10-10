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

    internal sealed class Q : OrbwalkingChild
   {
       public override string Name { get; set; } = nameof(Q); // ty reth0ught

       private readonly QSpell qSpell;

       private readonly Q2Spell q2Spell;

        public Q(QSpell qSpell, Q2Spell q2Spell)
        {
            this.qSpell = qSpell;
            this.q2Spell = q2Spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(qSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (this.Target == null || this.Target.Health > qSpell.GetDamage(Target) || !CheckGuardians())
            {
                return;
            }

            if (this.Target.Distance(ObjectManager.Player) <= qSpell.Spell.Range)
            {
                qSpell.Spell.CastOnUnit(Target);
            }
           else if (this.Target.Distance(ObjectManager.Player) > ObjectManager.Player.AttackRange && q2Spell.QMinionExtend() && Menu.Item("Extend").GetValue<bool>())
            {
                var m = MinionManager.GetMinions(qSpell.Spell.Range).FirstOrDefault();

                qSpell.Spell.CastOnUnit(m);
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
            Menu.AddItem(new MenuItem("Extend", "Allow Extended Q").SetValue(true));
        }
    }
}
