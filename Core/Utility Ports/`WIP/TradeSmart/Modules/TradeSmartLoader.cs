using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TradeSmart.Modules
{
    using System.Collections.Generic;
    using System.Drawing;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    class TradeSmartLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "TradeSmart";

        public override string InternalName { get; set; } = "TradeSmart";

        public override IEnumerable<string> Tags { get; set; } = new[] { "TradeSmart" };

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);

            superParent.Add(new List<Base>()
                                {
                                    new AllyMinion.AllyMinion(),
                                    new TradeEnemy.TradeEnemy()
                                });

            superParent.Load();

            superParent.Menu.Style = FontStyle.Bold;

            RootMenu = superParent.Menu;
        }
    }
}
