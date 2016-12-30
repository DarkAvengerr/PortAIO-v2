using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        private DashPosition dashPos;

        public QLane(QSpell spell)
        {
            this.spell = spell;
        }

        private float Range => spell.Spell.Range;

        private List<Obj_AI_Base> Minion => MinionManager.GetMinions(ObjectManager.Player.Position, Range);

        private void OnUpdate(EventArgs args)
        {
            if (Minion == null || !CheckGuardians())
            {
                return;
            }

            foreach (var m in Minion)
            {
                switch (spell.Spellstate)
                {
                    case QSpell.SpellState.Whirlwind:
                        Whirlwind();
                        break;

                    case QSpell.SpellState.Standard:
                        spell.Spell.Cast(m);
                        break;

                    case QSpell.SpellState.DashQ:

                        var pred = spell.Spell.GetCircularFarmLocation(Minion);

                        if (spell.CanEQ(m) && pred.MinionsHit >= Menu.Item("Lane.Q.Hit").GetValue<Slider>().Value && m.Distance(ObjectManager.Player) < 220)
                        {
                            spell.Spell.Cast(m);
                        }
                        break;
                }
            }
        }

        private void Whirlwind()
        {
            var pred = spell.Spell.GetLineFarmLocation(Minion);

            if (pred.MinionsHit >= Menu.Item("Lane.Q.Hit").GetValue<Slider>().Value)
            {
                spell.Spell.Cast(pred.Position);
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

            dashPos = new DashPosition();

            Menu.AddItem(new MenuItem("Lane.Q.Hit", "Use Q3 If X Hit Count").SetValue(new Slider(4, 0, 7)));
        }
    }
}