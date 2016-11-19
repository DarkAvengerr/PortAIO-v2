using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core;
    using Drawings.Damage;
    using Drawings.SpellRange;
    using OrbwalkingMode.Combo;
    using OrbwalkingMode.Harass;
    using OrbwalkingMode.Jungle;
    using OrbwalkingMode.Lane;
    using PermaActive;
    using PermaActive.Killsteal;

    using ReformedAIO.Champions.Gnar.Utility;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

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

            var orbwalker = new OrbwalkerModule();
            orbwalker.Load();

            var comboParent = new OrbwalkingParent("Combo", orbwalker.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Combo);
            var harassParent = new OrbwalkingParent("Harass", orbwalker.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Mixed);
            var jungleParent = new OrbwalkingParent("Jungle", orbwalker.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);
            var laneParent = new OrbwalkingParent("Lane", orbwalker.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);
            var killstealParent = new Parent("Killsteal");
            var fleeParent = new Parent("Flee");
            var drawingParent = new Parent("Drawing");

            var reformedUtilityParent = new Parent("Reformed Utility");

            reformedUtilityParent.Add(new GnarSkinchanger());

            var mustNotBeWindingUp = new PlayerMustNotBeWindingUp();
            var qReady = new SpellMustBeReady(SpellSlot.Q);
            var w2Ready = new SpellMustBeReady(SpellSlot.W);
            var eReady = new SpellMustBeReady(SpellSlot.E);
            var rReady = new SpellMustBeReady(SpellSlot.R);

            comboParent.Add(new Base[]
            {
                new QCombo().Guardian(qReady).Guardian(mustNotBeWindingUp),
                new WCombo().Guardian(w2Ready).Guardian(mustNotBeWindingUp),
                new ECombo().Guardian(eReady).Guardian(mustNotBeWindingUp),
                new RCombo().Guardian(rReady)
            });

            harassParent.Add(new Base[]
            {
                new QHarass().Guardian(qReady).Guardian(mustNotBeWindingUp) 
            });

            laneParent.Add(new Base[]
            {
                new QLane().Guardian(qReady).Guardian(mustNotBeWindingUp),
                new W2Lane().Guardian(w2Ready)  
            });

            jungleParent.Add(new Base[]
            {
                new QJungle().Guardian(qReady).Guardian(mustNotBeWindingUp),
                new W2Jungle().Guardian(w2Ready).Guardian(mustNotBeWindingUp),
                new EJungle().Guardian(eReady).Guardian(mustNotBeWindingUp) 
            });

            killstealParent.Add(new Base[]
            {
               new QKillsteal("Q"),
               new WKillsteal("W")  
            });

            fleeParent.Add(new Base[]
            {
                new Flee("Flee")
            });

            drawingParent.Add(new Base[]
            {
                new GnarDamage("Damage Indicator"), 
                new QRange("Q"),
                new WRange("W"),
                new ERange("E"),
                new RRange("R")    
            });

            superParent.Add(new Base[]
            {
                reformedUtilityParent,
                orbwalker,
                comboParent,
                harassParent,
                laneParent,
                jungleParent,
                killstealParent,
                fleeParent,
                drawingParent
            });

            superParent.Load();


            reformedUtilityParent.Menu.Style = FontStyle.Bold;
            reformedUtilityParent.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Gnar!</font></b>");
        }
    }
}
