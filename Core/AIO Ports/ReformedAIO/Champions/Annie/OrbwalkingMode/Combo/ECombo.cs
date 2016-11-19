using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Annie.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using ReformedAIO.Champions.Annie.Core;
    using ReformedAIO.Library.Spell_Information;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private PassiveCount count;

        private readonly ESpell spell;

        public ECombo(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(Menu.Item("Range").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
                || count.StunCount == 4)
            {
                return;
            }

            spell.Spell.Cast();
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

            Menu.AddItem(new MenuItem("Range", "Enemy Search Range").SetValue(new Slider(625, 0, 800)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(5, 0, 100)));

            count = new PassiveCount();
        }
    }
}
