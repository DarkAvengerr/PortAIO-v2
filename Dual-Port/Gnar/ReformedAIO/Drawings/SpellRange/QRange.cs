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

    internal class QRange : ChildBase
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
                   ? Color.Cyan
                   : Color.DarkSlateGray);
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

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            gnarState = new GnarState();
        }
    }
}
