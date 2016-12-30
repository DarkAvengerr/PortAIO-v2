using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QCombo  : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QCombo(QSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

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

            Menu.AddItem(new MenuItem("Caitlyn.Combo.Q.Mana", "Mana %").SetValue(new Slider(10, 0, 100)));

            Menu.AddItem(new MenuItem("Caitlyn.Combo.Q.Immobile", "Q On Immobile").SetValue(true));

            Menu.AddItem(new MenuItem("Caitlyn.Combo.Q.Hit", "Cast if 2 can be hit").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || Menu.Item("Caitlyn.Combo.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || !CheckGuardians() 
                || Target.Distance(ObjectManager.Player) < ObjectManager.Player.AttackRange + 50)
            {
                return;
            }

            var qPrediction = spell.Spell.GetPrediction(Target, true);

            if (Menu.Item("Caitlyn.Combo.Q.Hit").GetValue<bool>())
            {
                spell.Spell.CastIfWillHit(Target, 2);
            }

            if (qPrediction.Hitchance >= HitChance.Immobile && Menu.Item("Caitlyn.Combo.Q.Immobile").GetValue<bool>())
            {
                spell.Spell.Cast(qPrediction.CastPosition);
            }
        }
    }
}
