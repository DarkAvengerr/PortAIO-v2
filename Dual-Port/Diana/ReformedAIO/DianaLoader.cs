using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic.Killsteal;
    using ReformedAIO.Champions.Diana.Menus.Draw;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Flee;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Jungleclear;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Laneclear;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Misaya;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Mixed;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Utility;

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

            superParent.Add(new Base[]
            {
                comboParent, misayaParent, mixedParent, laneParent, jungleParent, ksParent, drawParent, fleeParent
            });

            comboParent.Add(new ChildBase[]
            {
                new CrescentStrike(orbwalker), 
                new Moonfall(orbwalker), 
                new LunarRush(orbwalker), 
                new PaleCascade(orbwalker), 
                new MisayaCombo(orbwalker) 
            });

            mixedParent.Add(new ChildBase[]
            {
                new MixedCrescentStrike(orbwalker)
            });
            
            laneParent.Add(new ChildBase[]
            {
                new LaneCrescentStrike(orbwalker), 
                new LaneLunarRush(orbwalker) 
            });
            
            jungleParent.Add(new ChildBase[]
            {
                new JungleCrescentStrike(orbwalker), 
                new JungleLunarRush(orbwalker), 
                new JungleMoonfall(orbwalker), 
                new JunglePaleCascade(orbwalker) 
            });
         
            ksParent.Add(new ChildBase[]
            {
                new KsPaleCascade(), 
                new KsCrescentStrike() 
            });
            
            drawParent.Add(new ChildBase[]
            {
                new DrawQ(), 
                new DrawE(), 
                new DrawDmg(), 
                new DrawPred() 
            });
            
            fleeParent.Add(new ChildBase[]
            {
                new FleeMode() 
            });
            
            superParent.Load();

            if (superParent.Loaded)
            {
                Chat.Print("Reformed Diana - Loaded");
            }
        }
        #endregion
    }
}