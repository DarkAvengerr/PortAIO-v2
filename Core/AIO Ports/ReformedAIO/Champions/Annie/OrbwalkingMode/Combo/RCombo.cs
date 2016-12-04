using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Annie.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using ReformedAIO.Champions.Annie.Core.Damage;
    using ReformedAIO.Library.Spell_Information;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        private readonly AnnieDamage dmg;

        public RCombo(RSpell spell, AnnieDamage dmg)
        {
            this.spell = spell;
            this.dmg = dmg;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Magical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
                || (Menu.Item("Killable").GetValue<bool>() && dmg.GetComboDamage(Target) < Target.Health))
            {
                return;
            }

            if (ObjectManager.Player.GetBuffCount("pyromania") == -1)
            {
                spell.Spell.CastIfWillHit(Target, Menu.Item("Flash").GetValue<Slider>().Value);
            }

            spell.Spell.Cast(Target);
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

            Menu.AddItem(new MenuItem("Flash", "Force If X Hit").SetValue(new Slider(3, 0, 5)));

            Menu.AddItem(new MenuItem("Killable", "Only When Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));

        }
    }
}
