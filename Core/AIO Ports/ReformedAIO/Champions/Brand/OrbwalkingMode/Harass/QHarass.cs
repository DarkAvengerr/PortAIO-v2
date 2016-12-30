using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Brand.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Brand.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using SPrediction;

    internal sealed class QHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        private readonly Orbwalking.Orbwalker orbwalker;

        public QHarass(QSpell spell, Orbwalking.Orbwalker orbwalker)
        {
            this.spell = spell;
            this.orbwalker = orbwalker;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Brand.Harass.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || (Menu.Item("Brand.Harass.Q.Blaze").GetValue<bool>() && !spell.Stunnable(Target)))
            {
                return;
            }

            spell.Spell.SPredictionCast(Target, HitChance.High);
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

            Menu.AddItem(new MenuItem("Brand.Harass.Q.Blaze", "Only Q To Stun").SetValue(true));

            Menu.AddItem(new MenuItem("Brand.Harass.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
