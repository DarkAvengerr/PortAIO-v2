using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Caitlyn.Drawings;
    using Caitlyn.Killsteal;
    using Caitlyn.Logic;
    using Caitlyn.OrbwalkingMode.Combo;
    using Caitlyn.OrbwalkingMode.Jungle;
    using Caitlyn.OrbwalkingMode.Lane;

    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

    internal sealed class CaitlynLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Caitlyn";

        public override string InternalName { get; set; } = "Caitlyn";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Caitlyn" };

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);
            superParent.Initialize();

            var spells = new Spells();
            spells.OnLoad();

            var orbwalkerModule = new OrbwalkerModule();
            orbwalkerModule.Load();

            var comboParent = new OrbwalkingParent("Combo", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Combo);
            var harassParent = new OrbwalkingParent("Harass", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Mixed);
            var laneParent = new OrbwalkingParent("Lane", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);
            var jungleParent = new OrbwalkingParent("Jungle", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);

            var killstealParent = new Parent("Killsteal");
            var drawParent = new Parent("Drawings");

            comboParent.Add(new List<Base>()
            {
                new QCombo().Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                new WCombo().Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.W)),
                new ECombo().Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E))
            });

            laneParent.Add(new List<Base>
            {
                new QLane().Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q))
            });

            jungleParent.Add(new List<Base>
            {
                new QJungle().Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)).Guardian(new SpellMustBeReady(SpellSlot.E) {Negated = true}),
                new EJungle().Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E))
            });

            killstealParent.Add(new List<Base>
            {
                new QKillsteal().Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                new RKillsteal().Guardian(new SpellMustBeReady(SpellSlot.R))
            });

            drawParent.Add(new List<Base>
            {
                new DmgDraw(),
                new QDraw(),
                new RDraw()
            });

            superParent.Add(new List<Base> {
                orbwalkerModule,
                comboParent,
                laneParent,
                jungleParent,
                killstealParent,
                drawParent
           });

            superParent.Load();

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;
        }
    }
}
