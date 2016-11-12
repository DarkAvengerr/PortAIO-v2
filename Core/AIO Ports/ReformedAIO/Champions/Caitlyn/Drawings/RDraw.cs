using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.Drawings
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;
    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class RDraw : ChildBase
    {
        public override string Name { get; set; } = "R";

        public readonly RSpell rSpell;

        public RDraw(RSpell rSpell)
        {
            this.rSpell = rSpell;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            Render.Circle.DrawCircle(
                 ObjectManager.Player.Position,
                rSpell.Spell.Range,
                rSpell.Spell.IsReady()
                ? Color.SlateBlue
                : Color.DarkSlateGray
                , 4);
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
