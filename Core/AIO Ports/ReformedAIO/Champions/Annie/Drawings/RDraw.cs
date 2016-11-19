using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Annie.Drawings
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Annie.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class RDraw : ChildBase
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        public RDraw(RSpell spell)
        {
            this.spell = spell;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Spell.Range, spell.Spell.IsReady()
                ? Color.BlueViolet
                : Color.DarkSlateGray,
                5,
                true);
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
        }
    }
}
