using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WLane(WSpell spell)
        {
            this.spell = spell;
        }

        private IEnumerable<Obj_AI_Base> Minion => MinionManager.GetMinions(ObjectManager.Player.Position, 200);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
               || Minion == null
               || (!spell.W1 && ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic"))
               || (Menu.Item("LeeSin.Lane.W.Enemies").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(1400) >= 1))
            {
                return;
            }

            foreach (var m in Minion)
            {
                if (m == null)
                {
                    return;
                }

                spell.Spell.Cast(ObjectManager.Player);
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

            Menu.AddItem(new MenuItem("LeeSin.Lane.W.Enemies", "Return if nearby enemies").SetValue(true));
        }
    }
}
