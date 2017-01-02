using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Caitlyn.Drawings
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    internal sealed class DmgDraw : ChildBase
    {
        private HeroHealthBarIndicator drawDamage;

        private readonly ComboLogic logic;

        public DmgDraw(ComboLogic logic)
        {
            this.logic = logic;
        }

        public override string Name { get; set; } = "Damage";

        public void OnDraw(EventArgs args)
        {
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
            this.drawDamage = new HeroHealthBarIndicator();
            base.OnLoad(sender, eventArgs);
        }
    }
}
