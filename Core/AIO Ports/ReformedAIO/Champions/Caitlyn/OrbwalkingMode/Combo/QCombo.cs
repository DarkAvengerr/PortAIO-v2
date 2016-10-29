using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QCombo  : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private AIHeroClient Target => TargetSelector.GetTarget(Spells.Spell[SpellSlot.Q].Range, TargetSelector.DamageType.Physical);

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

            Menu.AddItem(new MenuItem("QMana", "Mana %").SetValue(new Slider(10, 0, 100)));

            Menu.AddItem(new MenuItem("QImmobile", "Q On Immobile").SetValue(true));

            Menu.AddItem(new MenuItem("QHit", "Cast if 2 can be hit").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || Menu.Item("QMana").GetValue<Slider>().Value > Vars.Player.ManaPercent || !CheckGuardians())
            {
                return;
            }

            var qPrediction = Spells.Spell[SpellSlot.Q].GetPrediction(this.Target, true);

            if (Menu.Item("QHit").GetValue<bool>() && qPrediction.AoeTargetsHitCount >= 2)
            {
                Spells.Spell[SpellSlot.Q].Cast(Target);
            }

            if ((qPrediction.Hitchance >= HitChance.Immobile && Menu.Item("QImmobile").GetValue<bool>()) || qPrediction.Hitchance >= HitChance.VeryHigh)
            {
                Spells.Spell[SpellSlot.Q].Cast(qPrediction.CastPosition);
            }
        }
    }
}
