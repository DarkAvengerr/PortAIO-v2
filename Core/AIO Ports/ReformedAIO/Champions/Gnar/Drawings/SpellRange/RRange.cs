using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.Drawings.SpellRange
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class RRange : ChildBase
    {
        public override string Name { get; set; }

        public RRange(string name)
        {
            Name = name;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || !Spells.R2.IsReady()) return;

            Render.Circle.DrawCircle(
                ObjectManager.Player.Position,
                Spells.R2.Range,
                Color.Red,
                5,
                true);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
           
        }
    }
}
