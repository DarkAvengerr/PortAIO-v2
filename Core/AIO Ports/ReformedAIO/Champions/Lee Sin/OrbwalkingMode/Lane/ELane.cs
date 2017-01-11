using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ELane : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ELane(ESpell spell)
        {
            this.spell = spell;
        }

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position, spell.Spell.Range).FirstOrDefault();

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
               || Minion == null
               || !spell.E1
               || (Menu.Item("LeeSin.Lane.E.Enemies").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(1400) >= 1)
               || (Minion.Health > spell.GetDamage(Minion) && spell.E1))
            {
                return;
            }

            spell.CastItem();
            spell.Spell.Cast(Minion);
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

            Menu.AddItem(new MenuItem("LeeSin.Lane.E.Enemies", "Return if nearby enemies").SetValue(true));
        }
    }
}
