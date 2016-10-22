using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core;
    using Drawings;
    using OrbwalkingMode.Combo;
    using OrbwalkingMode.JungleClear;
    using OrbwalkingMode.LaneClear;
    using OrbwalkingMode.Mixed;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Utility;

    #endregion

    internal class AsheLoader : LoadableBase
    {
        #region Public Properties

        public override string DisplayName { get; set; } = String.ToTitleCase("Reformed Ashe");

        public override string InternalName { get; set; } = "Ashe";

        public override IEnumerable<string> Tags { get; set; } = new List<string> { "Ashe" };

        #endregion

        #region Public Methods and Operators

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);
            superParent.Initialize();

            var orbwalker = new Orbwalking.Orbwalker(superParent.Menu.SubMenu("Orbwalker"));

            var comboParent = new OrbwalkingParent("Combo", orbwalker, Orbwalking.OrbwalkingMode.Combo);
            var laneParent = new OrbwalkingParent("Lane", orbwalker, Orbwalking.OrbwalkingMode.LaneClear);
            var jungleParent = new OrbwalkingParent("Jungle", orbwalker, Orbwalking.OrbwalkingMode.LaneClear);
            var mixedParent = new OrbwalkingParent("Mixed", orbwalker, Orbwalking.OrbwalkingMode.Mixed);
            var drawingParent = new Parent("Drawings");

            var setSpell = new SetSpells();
            setSpell.Load();

            comboParent.Add(new ChildBase[]
            {
                new QCombo("[Q]", orbwalker),
                new WCombo("[W]", orbwalker),
                new ECombo("[E]", orbwalker),
                new RCombo("[R]", orbwalker)
            });

            mixedParent.Add(new ChildBase[]
            {
                new QMixed("[Q]", orbwalker),
                new WMixed("[W]", orbwalker) 
            });

            jungleParent.Add(new ChildBase[]
            {
                new QJungle("[Q]", orbwalker),
                new WJungle("[W]", orbwalker)
            });

           laneParent.Add(new ChildBase[]
           {
               new QLane("[Q]", orbwalker),
               new WLane("[W]", orbwalker)  
           });

            drawingParent.Add(new ChildBase[]
            {
               new WDraw("[W]"),
               new DmgDraw("Damage Indicator") 
            });
           
            superParent.Add(new Base[] {
                comboParent,
                mixedParent,
                laneParent,
                jungleParent,
                drawingParent
            });

            superParent.Load();

            if (superParent.Loaded)
            {
                Chat.Print(DisplayName + " - Loaded");
            }
        }
        #endregion
    }
}