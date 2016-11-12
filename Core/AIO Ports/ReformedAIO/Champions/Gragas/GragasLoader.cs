using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

   using Draw;
   using Logic;
   using OrbwalkingMode.Combo;
   using OrbwalkingMode.Jungle;
   using OrbwalkingMode.Lane;
   using OrbwalkingMode.Mixed;

    using ReformedAIO.Champions.Gragas.Utility;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;
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

            var orbwalker = new OrbwalkerModule();
            orbwalker.Load();

            var comboParent = new OrbwalkingParent("Combo", orbwalker.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Combo);
            var laneParent = new OrbwalkingParent("Lane", orbwalker.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);
            var jungleParent = new OrbwalkingParent("Jungle", orbwalker.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);
            var mixedParent = new OrbwalkingParent("Mixed", orbwalker.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Mixed);
            var draw = new Parent("Drawings");

            var reformedUtilityParent = new Parent("Reformed Utility");

            reformedUtilityParent.Add(new GragasSkinchanger());

            var qLogic = new QLogic();
            qLogic.Load();

            var qReady = new SpellMustBeReady(SpellSlot.Q);
            var wReady = new SpellMustBeReady(SpellSlot.W);
            var eReady = new SpellMustBeReady(SpellSlot.E);
            var rReady = new SpellMustBeReady(SpellSlot.R);

            comboParent.Add(new ChildBase[]
            {
                new QCombo().Guardian(qReady).Guardian(new SpellMustBeReady(SpellSlot.R) { Negated = true }), 
                new WCombo().Guardian(wReady), 
                new ECombo().Guardian(eReady), 
                new RCombo().Guardian(rReady)
            });
           
            laneParent.Add(new ChildBase[]
            {
                new LaneQ().Guardian(qReady), 
                new LaneW().Guardian(wReady), 
                new LaneE().Guardian(eReady) 
            });
          
            mixedParent.Add(new ChildBase[]
            {
                new QMixed().Guardian(qReady)
            });
           
            jungleParent.Add(new ChildBase[]
            {
                new QJungle().Guardian(qReady), 
                new WJungle().Guardian(wReady), 
                new EJungle().Guardian(eReady)
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
                reformedUtilityParent,
                orbwalker,
                comboParent,
                mixedParent,
                laneParent,
                jungleParent,
                draw
            });

            Prediction.Initialize(superParent.Menu);

            superParent.Load();

            reformedUtilityParent.Menu.Style = FontStyle.Bold;
            reformedUtilityParent.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Gragas!</font></b>");
        }

        #endregion
    }
}