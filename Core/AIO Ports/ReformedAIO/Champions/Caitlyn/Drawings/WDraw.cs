using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Caitlyn.Drawings
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal sealed class WDraw : ChildBase
    {
        public override string Name { get; set; } = "W";

        // WIP
        private readonly List<Vector3> Positions = new List<Vector3>()
                                                       {
                                                  new Vector3(8996, 8767, 54),
                                                  new Vector3(13884, 4774, 53),
                                                  new Vector3(13350, 8479, 52),
                                                  new Vector3(13630, 10852, 91),
                                                  new Vector3(7964, 13655, 53),
                                                  new Vector3(10487, 13906, 94)
                                                       };
                                              

        private readonly WSpell WSpell;

        public WDraw(WSpell WSpell)
        {
            this.WSpell = WSpell;
        }

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

           // Console.WriteLine("Vector3 Position: " + Game.CursorPos);
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
