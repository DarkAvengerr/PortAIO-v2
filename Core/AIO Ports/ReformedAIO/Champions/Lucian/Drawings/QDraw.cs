using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.Drawings
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class QDraw : ChildBase
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        private readonly Q2Spell q2Spell;

        public QDraw(QSpell qSpell, Q2Spell q2Spell)
        {
            this.qSpell = qSpell;
            this.q2Spell = q2Spell;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Menu.Item("Extended").GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, q2Spell.Spell.Range, q2Spell.Spell.IsReady()
                ? Color.DodgerBlue
                : Color.DarkSlateGray,
                4, true);
            }

            if (!Menu.Item("Prediction").GetValue<bool>())
            {
                return;
            }

            var minions = MinionManager.GetMinions(qSpell.Spell.Range);

            foreach (var m in minions)
            {
                if (q2Spell.QMinionExtend(m))
                {
                    Render.Circle.DrawCircle(m.Position, 70, Color.DodgerBlue, 4, true);
                }
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Extended", "Draw Extended Q").SetValue(true));

            Menu.AddItem(new MenuItem("Prediction", "Draw Prediction").SetValue(true));
        }
    }
}
