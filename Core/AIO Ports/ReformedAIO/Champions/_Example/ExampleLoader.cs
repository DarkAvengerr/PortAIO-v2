using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions._Example
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions._Example.Core.Damage;
    using ReformedAIO.Champions._Example.Core.Spells;
    using ReformedAIO.Champions._Example.Drawings.Damage;
    using ReformedAIO.Champions._Example.Drawings.Spells;
    using ReformedAIO.Champions._Example.Killsteal;
    using ReformedAIO.Champions._Example.Misc;
    using ReformedAIO.Champions._Example.OrbwalkingMode.Combo;
    using ReformedAIO.Champions._Example.OrbwalkingMode.Harass;
    using ReformedAIO.Champions._Example.OrbwalkingMode.Jungle;
    using ReformedAIO.Champions._Example.OrbwalkingMode.Lane;
    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

    internal sealed class ExampleLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Example";

        public override string InternalName { get; set; } = "Example";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Example" };

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);
            superParent.Initialize();

            var qSpell = new QSpell();
            var wSpell = new WSpell();
            var eSpell = new ESpell();
            var rSpell = new RSpell();

            var spellParent = new SpellParent();
            spellParent.Add(new List<Base> { qSpell, wSpell, eSpell, rSpell });
            spellParent.Load();

            var orbwalkerModule = new OrbwalkerModule();
            orbwalkerModule.Load();

            var comboParent = new OrbwalkingParent(
                "Combo",
                orbwalkerModule.OrbwalkerInstance,
                Orbwalking.OrbwalkingMode.Combo);
            var harassParent = new OrbwalkingParent(
                "Harass",
                orbwalkerModule.OrbwalkerInstance,
                Orbwalking.OrbwalkingMode.Mixed);
            var laneParent = new OrbwalkingParent(
                "Lane",
                orbwalkerModule.OrbwalkerInstance,
                Orbwalking.OrbwalkingMode.LaneClear);
            var jungleParent = new OrbwalkingParent(
                "Jungle",
                orbwalkerModule.OrbwalkerInstance,
                Orbwalking.OrbwalkingMode.LaneClear);

            var killstealParnet = new Parent("Killsteal");
            var drawingParent = new Parent("Drawings");

            var mustNotBeWindingUpGuardian = new PlayerMustNotBeWindingUp();
            var qReadyGuardian = new SpellMustBeReady(SpellSlot.Q);
            var wReadyGuardian = new SpellMustBeReady(SpellSlot.W);
            var eReadyGuardian = new SpellMustBeReady(SpellSlot.E);
            var rReadyGuardian = new SpellMustBeReady(SpellSlot.R);

            var ExampleDmg = new ExampleDamage(qSpell, wSpell, eSpell, rSpell);

            comboParent.Add(new List<Base>()
                                {
                                    new QCombo(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                    new WCombo(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                    new ECombo(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                    new RCombo(rSpell).Guardian(rReadyGuardian)
                                });

            harassParent.Add(new List<Base>()
                                 {
                                     new QHarass(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new WHarass(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                     new EHarass(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                                 });
            laneParent.Add(new List<Base>()
                               {
                                   new QLane(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                   new WLane(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                   new ELane(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                               });

            jungleParent.Add(new List<Base>()
                                 {
                                     new QJungle(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new WJungle(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                     new EJungle(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                                 });

            killstealParnet.Add(new List<Base>()
                                    {
                                        new QKillsteal(qSpell).Guardian(qReadyGuardian),
                                        new WKillsteal(wSpell).Guardian(wReadyGuardian),
                                        new EKillsteal(eSpell).Guardian(eReadyGuardian),
                                        new RKillsteal(rSpell).Guardian(rReadyGuardian)
                                    });

            drawingParent.Add(new List<Base>
                                  {
                                    new DamageDrawing(ExampleDmg),
                                    new QDrawing(qSpell),
                                    new WDrawing(wSpell),
                                    new EDrawing(eSpell)
                                  });

            superParent.Add(new List<Base>
                                  {
                                     orbwalkerModule,
                                     comboParent,
                                     harassParent,
                                     laneParent,
                                     jungleParent,
                                     killstealParnet,
                                     drawingParent,
                                     new ExampleAntiGapcloser(qSpell),
                                     new ExampleInterrupter(qSpell)
                                  });

            superParent.Load();

            orbwalkerModule.Menu.Style = FontStyle.Bold;
            orbwalkerModule.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Example!</font></b>");
        }
    }
}
