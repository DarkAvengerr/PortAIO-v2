using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using OrbwalkingMode.Combo;
    using OrbwalkingMode.Flee;
    using OrbwalkingMode.Jungleclear;
    using OrbwalkingMode.Laneclear;
    using OrbwalkingMode.Misaya;
    using OrbwalkingMode.Mixed;

    using Diana.Draw;
    using Diana.Killsteal;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Utility;

    using Color = SharpDX.Color;

    #endregion

    internal class DianaLoader : LoadableBase
    {
        #region Public Properties

        public override string DisplayName { get; set; } = String.ToTitleCase("Reformed Diana");

        public override string InternalName { get; set; } = "Diana";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Diana" };

        #endregion

        #region Public Methods and Operators

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);
            superParent.Initialize();

            var orbwalker = new Orbwalking.Orbwalker(superParent.Menu.SubMenu("Orbwalker"));

            // Parents
            var comboParent = new OrbwalkingParent("Combo", orbwalker, Orbwalking.OrbwalkingMode.Combo);
            var misayaParent = new Parent("Misaya");
            var laneParent = new OrbwalkingParent("Lane", orbwalker, Orbwalking.OrbwalkingMode.LaneClear);
            var jungleParent = new OrbwalkingParent("Jungle", orbwalker, Orbwalking.OrbwalkingMode.LaneClear);
            var mixedParent = new OrbwalkingParent("Mixed", orbwalker, Orbwalking.OrbwalkingMode.Mixed);
            var ksParent = new Parent("Killsteal");
            var drawParent = new Parent("Drawings");
            var fleeParent = new Parent("Flee");

            var qReady = new SpellMustBeReady(SpellSlot.Q);
            var wReady = new SpellMustBeReady(SpellSlot.W);
            var eReady = new SpellMustBeReady(SpellSlot.E);
            var rReady = new SpellMustBeReady(SpellSlot.R);
            var rMustNotBeReady = new SpellMustBeReady(SpellSlot.R) {Negated = true};

            comboParent.Add(new Base[]
            {
                new CrescentStrike().Guardian(qReady).Guardian(rMustNotBeReady), 
                new LunarRush().Guardian(wReady),
                new Moonfall().Guardian(eReady),
                new PaleCascade().Guardian(rReady), 
            });

            misayaParent.Add(new MisayaCombo());

            mixedParent.Add(new Base[]
            {
                new MixedCrescentStrike().Guardian(qReady)
            });
            
            laneParent.Add(new Base[]
            {
                new LaneCrescentStrike().Guardian(qReady), 
                new LaneLunarRush().Guardian(wReady) 
            });
            
            jungleParent.Add(new Base[]
            {
                new JungleCrescentStrike().Guardian(qReady), 
                new JungleLunarRush().Guardian(wReady), 
                new JungleMoonfall().Guardian(eReady), 
                new JunglePaleCascade().Guardian(rReady) 
            });
         
            ksParent.Add(new Base[]
            {
                new KsPaleCascade(), 
                new KsCrescentStrike() 
            });
            
            drawParent.Add(new Base[]
            {
                new DrawQ(), 
                new DrawE(), 
                new DrawDmg(), 
                new DrawPred() 
            });
            
            fleeParent.Add(new Base[]
            {
                new FleeMode() 
            });

            superParent.Add(new Base[] {

                comboParent,
                misayaParent,
                mixedParent,
                laneParent,
                jungleParent,
                ksParent,
                drawParent,
                fleeParent
            });

            superParent.Load();

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;
        }
        #endregion
    }
}