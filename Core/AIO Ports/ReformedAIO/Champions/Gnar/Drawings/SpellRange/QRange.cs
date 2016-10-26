namespace ReformedAIO.Champions.Gnar.Drawings.SpellRange
{
    using System;
    using System.Drawing;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class QRange : ChildBase
    {
        public override string Name { get; set; }

        public QRange(string name)
        {
            Name = name;
        }

        private GnarState gnarState;

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            if (gnarState.Mini)
            {
                Render.Circle.DrawCircle(
                   ObjectManager.Player.Position,
                   Spells.Q.Range,
                   Spells.Q.IsReady()
                   ? Color.OrangeRed
                   : Color.DarkSlateGray, 
                   4, 
                   true);
            }

            if (gnarState.Mega)
            {
                Render.Circle.DrawCircle(
                   ObjectManager.Player.Position,
                   Spells.Q2.Range,
                   Spells.Q2.IsReady()
                   ? Color.Red
                   : Color.DarkSlateGray);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            gnarState = new GnarState();
        }
    }
}
