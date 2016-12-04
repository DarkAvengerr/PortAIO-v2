using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Olaf.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WLane(WSpell spell)
        {
            this.spell = spell;
        }

        private static List<Obj_AI_Base> Mob
            => MinionManager.GetMinions(ObjectManager.Player.Position, 600);

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
                || (Menu.Item("Enemy").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(2000) > 0))
            {
                return;
            }

            if (Mob.Count >= Menu.Item("Count").GetValue<Slider>().Value)
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

            Menu.AddItem(new MenuItem("Enemy", "Return If Nearby Enemy").SetValue(true));

            Menu.AddItem(new MenuItem("Count", "Min Minion Count").SetValue(new Slider(4, 1, 7)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(50, 0, 100)));
        }
    }
}