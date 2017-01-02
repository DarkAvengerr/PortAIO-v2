using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.SDK.Utils;

    using ReformedAIO.Champions.Gragas.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WLane(WSpell spell)
        {
            this.spell = spell;
        }

        private static IEnumerable<Obj_AI_Base> Minion => MinionManager.GetMinions(ObjectManager.Player.Position, ObjectManager.Player.GetRealAutoAttackRange() + 65);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
               || Minion == null
               || Menu.Item("Gragas.Lane.W.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
               || (Menu.Item("Gragas.Lane.W.Enemies").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(1400) >= 1))
            {
                return;
            }
            foreach (var m in Minion)
            {
                if(m == null) return;

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

            Menu.AddItem(new MenuItem("Gragas.Lane.W.Enemies", "Return if nearby enemies").SetValue(true));

            Menu.AddItem(new MenuItem("Gragas.Lane.W.Mana", "Min Mana %").SetValue(new Slider(70, 0, 100)));
        }
    }
}
