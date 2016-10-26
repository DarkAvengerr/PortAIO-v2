namespace ReformedAIO.Champions.Gnar
{
    using System.Collections.Generic;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;
    using ReformedAIO.Champions.Gnar.Drawings.Damage;
    using ReformedAIO.Champions.Gnar.Drawings.SpellRange;
    using ReformedAIO.Champions.Gnar.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Gnar.OrbwalkingMode.Harass;
    using ReformedAIO.Champions.Gnar.OrbwalkingMode.Jungle;
    using ReformedAIO.Champions.Gnar.OrbwalkingMode.Lane;
    using ReformedAIO.Champions.Gnar.PermaActive;
    using ReformedAIO.Champions.Gnar.PermaActive.Killsteal;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class GnarLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Gnar";

        public override string InternalName { get; set; } = "Gnar";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Gnar" };

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);
            superParent.Initialize();

            var spells = new Spells(); // - RIP CLEAN CODE
            spells.Initialize();

            var orbwalker = new Orbwalking.Orbwalker(superParent.Menu.SubMenu("Orbwalker"));

            var comboParent = new OrbwalkingParent("Combo", orbwalker, Orbwalking.OrbwalkingMode.Combo);
            var harassParent = new OrbwalkingParent("Harass", orbwalker, Orbwalking.OrbwalkingMode.Mixed);
            var jungleParent = new OrbwalkingParent("Jungle", orbwalker, Orbwalking.OrbwalkingMode.LaneClear);
            var laneParent = new OrbwalkingParent("Lane", orbwalker, Orbwalking.OrbwalkingMode.LaneClear);
            var killstealParent = new Parent("Killsteal");
            var fleeParent = new Parent("Flee");
            var drawingParent = new Parent("Drawing");

            comboParent.Add(new ChildBase[]
            {
                new QCombo(orbwalker),
                new WCombo(orbwalker),
                new ECombo(orbwalker),
                new RCombo(orbwalker)
            });

            harassParent.Add(new ChildBase[]
            {
                new QHarass(orbwalker) 
            });

            laneParent.Add(new ChildBase[]
            {
                new QLane(orbwalker),
                new W2Lane(orbwalker)  
            });

            jungleParent.Add(new ChildBase[]
            {
                new QJungle(orbwalker),
                new W2Jungle(orbwalker),
                new EJungle(orbwalker) 
            });

            killstealParent.Add(new ChildBase[]
            {
               new QKillsteal("Q"),
               new WKillsteal("W")  
            });

            fleeParent.Add(new ChildBase[]
            {
                new Flee("Flee")
            });

            drawingParent.Add(new ChildBase[]
            {
                new GnarDamage("Damage Indicator"), 
                new QRange("Q"),
                new WRange("W"),
                new ERange("E"),
                new RRange("R")    
            });

            superParent.Add(new Base[]
            {
                comboParent,
                harassParent,
                laneParent,
                jungleParent,
                killstealParent,
                fleeParent,
                drawingParent
            });

            superParent.Load();

            if (superParent.Loaded)
            {
                Chat.Print(DisplayName + " - Loaded");
            }
        }
    }
}
