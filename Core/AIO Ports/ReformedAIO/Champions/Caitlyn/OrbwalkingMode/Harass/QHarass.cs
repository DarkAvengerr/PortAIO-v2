using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        public QHarass(QSpell qSpell)
        {
            this.qSpell = qSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(qSpell.Spell.Range, TargetSelector.DamageType.Physical);

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Mana", "Mana %").SetValue(new Slider(10, 0, 100)));

            Menu.AddItem(new MenuItem("Immobile", "Q On Immobile").SetValue(true));

            Menu.AddItem(new MenuItem("Hit", "Cast if 2 can be hit").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || !CheckGuardians()
                || Target.Distance(ObjectManager.Player) < ObjectManager.Player.AttackRange + 50)
            {
                return;
            }

            var qPrediction = qSpell.Spell.GetPrediction(Target, true);

            if (Menu.Item("Hit").GetValue<bool>())
            {
                qSpell.Spell.CastIfWillHit(Target, 2);
            }

            if (qPrediction.Hitchance >= HitChance.Immobile && Menu.Item("Immobile").GetValue<bool>())
            {
                qSpell.Spell.Cast(qPrediction.CastPosition);
            }
        }
    }
}
