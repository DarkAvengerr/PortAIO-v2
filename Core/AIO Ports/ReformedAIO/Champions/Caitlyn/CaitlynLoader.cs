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

    using ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Harass;
    using ReformedAIO.Champions.Caitlyn.Spells;
    using ReformedAIO.Champions.Caitlyn.Utility;
    using ReformedAIO.Library.SpellParent;

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

            var qSpell = new QSpell();
            var wSpell = new WSpell();
            var eSpell = new ESpell();
            var rSpell = new RSpell();

            var spellParent = new SpellParent();
            spellParent.Add(new List<Base>
                                  {
                                     qSpell,
                                     wSpell,
                                     eSpell,
                                     rSpell
                                  });
            spellParent.Load();

            var orbwalkerModule = new OrbwalkerModule();
            orbwalkerModule.Load();

            var comboParent = new OrbwalkingParent("Combo", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Combo);
            var harassParent = new OrbwalkingParent("Harass", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Mixed);
            var laneParent = new OrbwalkingParent("Lane", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);
            var jungleParent = new OrbwalkingParent("Jungle", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);

            var killstealParent = new Parent("Killsteal");
            var drawParent = new Parent("Drawings");

            var utilityParent = new Parent("Reformed Utility");

            utilityParent.Add(new CaitlynSkinchanger());

            var logic = new ComboLogic(eSpell, wSpell, qSpell, rSpell);

            comboParent.Add(new List<Base>()
            {
                new QCombo(qSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                new WCombo(wSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.W)),
                new ECombo(eSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E))
            });

            harassParent.Add(new List<Base>()
            {
                new QHarass(qSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                new WHarass(wSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.W)),
                new EHarass(eSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E))
            });

            laneParent.Add(new List<Base>
            {
                new QLane(qSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q))
            });

            jungleParent.Add(new List<Base>
            {
                new QJungle(qSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)).Guardian(new SpellMustBeReady(SpellSlot.E) {Negated = true}),
                new EJungle(eSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E))
            });

            killstealParent.Add(new List<Base>
            {
                new QKillsteal(qSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                new RKillsteal(rSpell).Guardian(new SpellMustBeReady(SpellSlot.R))
            });

            drawParent.Add(new List<Base>
            {
                new DmgDraw(logic),
                new QDraw(qSpell),
                new RDraw(rSpell)
            });

            superParent.Add(new List<Base> {
                utilityParent,
                orbwalkerModule,
                comboParent,
                harassParent,
                laneParent,
                jungleParent,
                killstealParent,
                drawParent
           });

            superParent.Load();

            utilityParent.Menu.Style = FontStyle.Bold;
            utilityParent.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Caitlyn!</font></b>");
        }
    }
}
