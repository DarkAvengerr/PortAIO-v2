using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Insec
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EInsec : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EInsec(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(280, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() 
                || Target == null 
                || !spell.E1 && (ObjectManager.Player.Mana < 80 || Target.Distance(ObjectManager.Player) < 150)
                || (spell.E1 && ObjectManager.Player.IsDashing()))
            {
                return;
            }

            spell.CastItem();
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
        }
    }
}
