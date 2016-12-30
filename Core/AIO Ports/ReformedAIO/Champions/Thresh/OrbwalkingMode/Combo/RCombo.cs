using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Thresh.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Thresh.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        public RCombo(RSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Thresh.Combo.R.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || (ObjectManager.Player.CountEnemiesInRange(spell.Spell.Range - 35) < Menu.Item("Thresh.Combo.R.Hit").GetValue<Slider>().Value
                && (Menu.Item("Thresh.Combo.R.Hooked").GetValue<bool>() && !Target.HasBuff("ThreshQ"))))
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

            Menu.AddItem(new MenuItem("Thresh.Combo.R.Hooked", "Use If Hooked").SetValue(true));

            Menu.AddItem(new MenuItem("Thresh.Combo.R.Hit", "Use If X Enemies:").SetValue(new Slider(2, 1, 5)));

            Menu.AddItem(new MenuItem("Thresh.Combo.R.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
