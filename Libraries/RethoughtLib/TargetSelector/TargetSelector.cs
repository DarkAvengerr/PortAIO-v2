using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; namespace RethoughtLib.TargetSelector
{
    using global::RethoughtLib.TargetSelector.Abstract_Classes;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class TargetSelector : TargetSelectorBase
    {
        public AIHeroClient LastSelectedTarget { get; set; }

        public AIHeroClient LastManuallySelectedTarget { get; set; }

        public AIHeroClient SelectedTarget { get; set; }

        public AIHeroClient ManuallySelectedTarget { get; set; }

        public TargetSelector(Menu menu)
            : base(menu)
        {
        }
    }
}
