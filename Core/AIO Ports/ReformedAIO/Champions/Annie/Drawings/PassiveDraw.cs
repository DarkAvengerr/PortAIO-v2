using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Annie.Drawings
{
    using System;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Annie.Core;
    using ReformedAIO.Champions.Annie.Core.Spells;
    using ReformedAIO.Library.Spell_Information;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class PassiveDraw : ChildBase
    {
        public override string Name { get; set; } = "Passive";

        private PassiveCount count;

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            Drawing.DrawText(
                pos.X - 20,
                pos.Y + 20,
                count.StunCount == 4
                ? Color.MediumSpringGreen
                : Color.White,
                "Count: " + count.StunCount + "/4");
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
            count = new PassiveCount();
        }
    }
}
