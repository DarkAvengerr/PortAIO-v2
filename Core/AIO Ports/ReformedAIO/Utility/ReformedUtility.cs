using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Utility
{
    using System.Collections.Generic;
    using System.Drawing;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    using Color = SharpDX.Color;

    internal class ReformedUtlity : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Utility";

        public override string InternalName { get; set; } = "Reformed Utility";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Reformed Utility" };

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);

            superParent.Add(new List<Base>()
                                {
                                 
                                });


            superParent.Load();


            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;
        }
    }
}