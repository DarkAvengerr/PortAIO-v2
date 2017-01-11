using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Xerath.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QLane(QSpell spell)
        {
            this.spell = spell;
        }

        private IEnumerable<Obj_AI_Base> Minion => MinionManager.GetMinions(ObjectManager.Player.Position, spell.Spell.ChargedMinRange);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
               || Minion == null
               || (Menu.Item("Xerath.Lane.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent && !spell.Charging)
               || (Menu.Item("Xerath.Lane.Q.Enemies").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(1400) >= 1))
            {
                return;
            }

            //Hacks.DisableCastIndicator = spell.Charging;

            foreach (var m in Minion)
            {
                if (m.Health > spell.GetDamage(m) && Menu.Item("Xerath.Lane.Q.Killable").GetValue<bool>())
                {
                    return;
                }

                if (!spell.Charging)
                {
                    spell.Spell.StartCharging();
                    return;
                }

                if(spell.SDK(m) == null) return;

                spell.Spell.Cast(spell.SDK(m).CastPosition);
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

           // Menu.AddItem(new MenuItem("Xerath.Lane.Q.Count", "Min Predicted Hit Count").SetValue(new Slider(1, 1, 6)));

            Menu.AddItem(new MenuItem("Xerath.Lane.Q.Killable", "Only Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Xerath.Lane.Q.Enemies", "Return if nearby enemies").SetValue(true));

            Menu.AddItem(new MenuItem("Xerath.Lane.Q.Mana", "Min Mana %").SetValue(new Slider(70, 0, 100)));
        }
    }
}
