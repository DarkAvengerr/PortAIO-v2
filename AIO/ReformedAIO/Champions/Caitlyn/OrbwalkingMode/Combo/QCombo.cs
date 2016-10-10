using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QCombo  : OrbwalkingChild
    {
        public override string Name { get; set; } 

        private AIHeroClient Target => TargetSelector.GetTarget(Spells.Spell[SpellSlot.Q].Range, TargetSelector.DamageType.Physical);

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("QMana", "Mana %").SetValue(new Slider(30, 0, 100)));

            Menu.AddItem(new MenuItem("QImmobile", "Q On Immobile").SetValue(true));

            Menu.AddItem(new MenuItem("QHit", "Cast if 2 can be hit").SetValue(true));

            Menu.AddItem(new MenuItem("QHigh", "E + Q Cancel").SetValue(false));
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

            if (qPrediction.Hitchance >= HitChance.Immobile && Menu.Item("QImmobile").GetValue<bool>() || (qPrediction.Hitchance >= HitChance.VeryHigh && Menu.Item("QHigh").GetValue<bool>()))
            {
                Spells.Spell[SpellSlot.Q].Cast(qPrediction.CastPosition);
            }
        }
    }
}
