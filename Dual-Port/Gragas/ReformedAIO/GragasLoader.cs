using System.Collections.Generic;

using EloBuddy; namespace ReformedAIO.Champions.Gragas
{
    #region Using Directives

    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;
    using ReformedAIO.Champions.Gragas.Menus.Draw;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Jungle;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Mixed;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    using Prediction = SPrediction.Prediction;

    #endregion

    internal class GragasLoader : LoadableBase
    {
        #region Public Properties

        public override string DisplayName { get; set; } = "Reformed Gragas";

        public override string InternalName { get; set; } = "Gragas";

        public override IEnumerable<string> Tags { get; set; } = new List<string>() { "Gragas" };

        #endregion

        #region Public Methods and Operators

        public override void Load()
        {
            var superParent = new SuperParent(this.DisplayName);

            var combo = new Parent("Combo");
            var mixed = new Parent("Mixed");
            var lane = new Parent("LaneClear");
            var jungle = new Parent("JungleClear");
            var draw = new Parent("Drawings");

            var qLogic = new QLogic();
            qLogic.Load();

            superParent.AddChildren(new[]
            {
                combo, mixed, lane, jungle, draw
            });

            combo.AddChild(new QCombo());
            combo.AddChild(new WCombo());
            combo.AddChild(new ECombo());
            combo.AddChild(new RCombo());

            lane.AddChild(new LaneQ());
            lane.AddChild(new LaneW());
            lane.AddChild(new LaneE());

            mixed.AddChild(new QMixed());

            jungle.AddChild(new QJungle());
            jungle.AddChild(new WJungle());
            jungle.AddChild(new EJungle());

            draw.AddChild(new DrawIndicator());
            draw.AddChild(new DrawQ());
            draw.AddChild(new DrawE());
            draw.AddChild(new DrawR());

            superParent.OnLoadInvoker();

            Prediction.Initialize(superParent.Menu);

            var orbWalkingMenu = new Menu("Orbwalker", "Orbwalking");
            Variable.Orbwalker = new Orbwalking.Orbwalker(orbWalkingMenu);
            superParent.Menu.AddSubMenu(orbWalkingMenu);
        }

        #endregion
    }
}