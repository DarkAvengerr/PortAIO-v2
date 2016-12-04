using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Annie.OrbwalkingMode.Laneclear
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

        private List<Obj_AI_Base> Minion => MinionManager.GetMinions(ObjectManager.Player.Position, spell.Spell.Range);

        private void OnUpdate(EventArgs args)
        {
            if (Minion == null
                || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
                || (ObjectManager.Player.CountEnemiesInRange(1750) >= 1 && Menu.Item("Enemy").GetValue<bool>()))
            {
                return;
            }

            foreach (var m in Minion)
            {
                if (m.Health > spell.Spell.GetDamage(m) && Menu.Item("Killable").GetValue<bool>())
                {
                    continue;
                }

                var farmPosition = spell.Spell.GetLineFarmLocation(Minion).Position.To3D();

                spell.Spell.Cast(farmPosition);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Killable", "Only if Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Enemy", "Return If Nearby Enemies").SetValue(false));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(60, 0, 100)));
        }
    }
}
