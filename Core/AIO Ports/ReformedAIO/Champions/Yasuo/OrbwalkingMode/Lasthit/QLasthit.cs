using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Lasthit
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QLasthit : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        private DashPosition dashPos;

        public QLasthit(QSpell spell)
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

                        var pred = spell.Spell.GetLineFarmLocation(Minion);

                        if (pred.MinionsHit >= Menu.Item("Lasthit.Q.Hit").GetValue<Slider>().Value)
                        {
                            spell.Spell.Cast(pred.Position);
                        }
                        break;

                    case QSpell.SpellState.Standard:
                        spell.Spell.Cast(m);
                        break;
                }
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

            Menu.AddItem(new MenuItem("Lasthit.Q.Hit", "Use Q3 If X Hit Count").SetValue(new Slider(4, 0, 7)));
        }
    }
}