using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EKillsteal(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null 
                || Target.Health + 45 > spell.GetDamage(Target) * .85
                || Menu.Item("Vayne.Killsteal.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent 
                || (Menu.Item("Vayne.Killsteal.E.Proc").GetValue<bool>() && !spell.WStack(Target))
                || !CheckGuardians())
            {
                return;
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

            Menu.AddItem(new MenuItem("Vayne.Killsteal.E.Proc", "Only Proc W").SetValue(true));

            Menu.AddItem(new MenuItem("Vayne.Killsteal.E.Mana", "Min Mana %").SetValue(new Slider(25, 0, 100)));
        }
    }
}
