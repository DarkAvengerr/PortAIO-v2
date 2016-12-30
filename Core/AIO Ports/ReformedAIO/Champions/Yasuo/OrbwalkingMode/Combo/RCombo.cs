using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Damage;

    using Yasuo.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        private readonly YasuoDamage damage;

        public RCombo(RSpell spell, YasuoDamage damage)
        {
            this.spell = spell;
            this.damage = damage;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null 
                || !CheckGuardians()
                || (Menu.Item("Combo.R.Turret").GetValue<bool>() && Target.UnderTurret(true))
                || (Menu.Item("Combo.R.Enemies").GetValue<Slider>().Value < ObjectManager.Player.CountEnemiesInRange(spell.Spell.Range))
                || (Menu.Item("Combo.R.Killable").GetValue<bool>() && Target.Health > damage.GetComboDamage(Target)))
            {
                return;
            }

            if (spell.IsAirbone(Target) && spell.RemainingAirboneTime(Target) < Game.Ping + 10)
            {
                spell.Spell.Cast();
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

            Menu.AddItem(new MenuItem("Combo.R.Killable", "Only When Killable").SetValue(false));

            Menu.AddItem(new MenuItem("Combo.R.Turret", "Don't R Into Turret").SetValue(true));

            Menu.AddItem(new MenuItem("Combo.R.Enemies", "Use When X Enemies").SetValue(new Slider(2, 1, 5)));

            //Menu.AddItem(new MenuItem("Combo.R.Delay", "Combo Delay").SetValue(new Slider(200, 50, 400)));
        }
    }
}