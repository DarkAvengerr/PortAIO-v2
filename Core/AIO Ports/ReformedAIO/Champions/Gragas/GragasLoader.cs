namespace ReformedAIO.Champions.Gragas
{
    #region Using Directives

    using System.Collections.Generic;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;
    using ReformedAIO.Champions.Gragas.Menus.Draw;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Jungle;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Mixed;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    using Prediction = SPrediction.Prediction;

    #endregion

    internal class GragasLoader : LoadableBase
    {
        #region Public Properties

        public override string DisplayName { get; set; } = "Reformed Gragas";

        public override string InternalName { get; set; } = "Gragas";

        public override IEnumerable<string> Tags { get; set; } = new List<string> { "Gragas" };

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
            var draw = new Parent("Drawings");

            var qLogic = new QLogic();
            qLogic.Load();

            comboParent.Add(new ChildBase[]
            {
                new QCombo(orbwalker), 
                new WCombo(orbwalker), 
                new ECombo(orbwalker), 
                new RCombo(orbwalker)
            });
           
            laneParent.Add(new ChildBase[]
            {
                new LaneQ(orbwalker), 
                new LaneW(orbwalker), 
                new LaneE(orbwalker) 
            });
          
            mixedParent.Add(new ChildBase[]
            {
                new QMixed(orbwalker)
            });
           
            jungleParent.Add(new ChildBase[]
            {
                new QJungle(orbwalker), 
                new WJungle(orbwalker), 
                new EJungle(orbwalker)
            });
            
            draw.Add(new ChildBase[]
            {
                new DrawIndicator(), 
                new DrawQ(), 
                new DrawE(), 
                new DrawR()
            });
          
            superParent.Add(new Base[]
            {
                comboParent, mixedParent, laneParent, jungleParent, draw
            });

            Prediction.Initialize(superParent.Menu);

            superParent.Load();

            if (superParent.Loaded)
            {
                Chat.Print("Reformed Gragas - Loaded");
            }
        }

        #endregion
    }
}