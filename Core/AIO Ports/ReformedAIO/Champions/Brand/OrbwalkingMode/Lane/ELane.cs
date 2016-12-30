using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Brand.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Brand.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ELane : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ELane(ESpell spell)
        {
            this.spell = spell;
        }

        private IEnumerable<Obj_AI_Base> Minion => MinionManager.GetMinions(ObjectManager.Player.Position, spell.Spell.Range).Where(m => spell.Stunnable(m));

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
               || Minion == null
               || Menu.Item("Brand.Lane.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
               || (Menu.Item("Brand.Lane.E.Enemies").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(1400) >= 1))
            {
                return;
            }

            foreach (var m in Minion)
            {
                if (m.Health > spell.GetDamage(m) && Menu.Item("Brand.Lane.E.Killable").GetValue<bool>())
                {
                    continue;
                }

                spell.Spell.Cast(m);
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

            Menu.AddItem(new MenuItem("Brand.Lane.E.Killable", "Only Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Brand.Lane.E.Enemies", "Return if nearby enemies").SetValue(true));

            Menu.AddItem(new MenuItem("Brand.Lane.E.Mana", "Min Mana %").SetValue(new Slider(70, 0, 100)));
        }
    }
}
