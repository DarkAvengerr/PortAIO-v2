using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;

    using Core.Damage;
    using Core.Spells;
    using Drawings.Damage;
    using Drawings.Spells;
    using Killsteal;
    using Misc;
    using OrbwalkingMode.Combo;
    using OrbwalkingMode.Harass;
    using OrbwalkingMode.Insec;
    using OrbwalkingMode.Jungle;
    using OrbwalkingMode.Lane;

    using ReformedAIO.Champions.Lee_Sin.Drawings.Insec;
    using ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Flee;
    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;

    using Color = SharpDX.Color;
    using RethoughtLib.Orbwalker.Implementations;

    internal sealed class LeeSinLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Lee Sin";

        public override string InternalName { get; set; } = "LeeSin";

        public override IEnumerable<string> Tags { get; set; } = new[] { "LeeSin" };

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

            var insecParent = new OrbwalkingParent(
                "INSEC",
                orbwalkerModule.OrbwalkerInstance,
                Orbwalking.OrbwalkingMode.Burst);

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

            var fleeParent = new OrbwalkingParent(
                 "Flee",
                 orbwalkerModule.OrbwalkerInstance,
                 Orbwalking.OrbwalkingMode.Flee);

            var killstealParnet = new Parent("Killsteal");
            var drawingParent = new Parent("Drawings");

            var mustNotBeWindingUpGuardian = new PlayerMustNotBeWindingUp();
            var qReadyGuardian = new SpellMustBeReady(SpellSlot.Q);
            var wReadyGuardian = new SpellMustBeReady(SpellSlot.W);
            var eReadyGuardian = new SpellMustBeReady(SpellSlot.E);
            var rReadyGuardian = new SpellMustBeReady(SpellSlot.R);

            var leeSinStatistisks = new LeeSinStatistisks(qSpell, wSpell, eSpell, rSpell);

            insecParent.Add(new List<Base> {
                                    new RwInsec(wSpell, rSpell, qSpell).Guardian(mustNotBeWindingUpGuardian)
                                });

            comboParent.Add(new List<Base> {
                                    new QCombo(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                    new WCombo(wSpell, rSpell, leeSinStatistisks).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                    new ECombo(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                    new RCombo(rSpell, leeSinStatistisks).Guardian(rReadyGuardian)
                                });

            harassParent.Add(new List<Base> {
                                     new QHarass(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new WHarass(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                     new EHarass(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                                 });
            laneParent.Add(new List<Base> {
                                   new QLane(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                   new WLane(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                   new ELane(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                               });

            jungleParent.Add(new List<Base> {
                                     new QJungle(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new WJungle(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                     new EJungle(eSpell, wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                                 });

            fleeParent.Add(new List<Base>
                               {
                                   new Flee(qSpell, wSpell).Guardian(qReadyGuardian),
                               });

            killstealParnet.Add(new List<Base> {
                                        new QKillsteal(qSpell).Guardian(qReadyGuardian),
                                        new EKillsteal(eSpell).Guardian(eReadyGuardian),
                                        new RKillsteal(rSpell).Guardian(rReadyGuardian),
                                        new RQQKillsteal(qSpell, rSpell).Guardian(qReadyGuardian).Guardian(rReadyGuardian)
                                    });

            drawingParent.Add(new List<Base>
                                  {
                                    new DamageDrawing(leeSinStatistisks, qSpell),
                                    new InsecDrawing(wSpell, rSpell).Guardian(rReadyGuardian),
                                    new QDrawing(qSpell),
                                    new WDrawing(wSpell),
                                    new EDrawing(eSpell)
                                  });

            superParent.Add(new List<Base>
                                  {
                                     orbwalkerModule,
                                     insecParent,
                                     comboParent,
                                     harassParent,
                                     laneParent,
                                     jungleParent,
                                     new SmiteHandler(qSpell, wSpell),
                                     fleeParent,
                                     new Wardjump(wSpell).Guardian(wReadyGuardian),
                                     killstealParnet,
                                     drawingParent,
                                     new LeeSinAntiGapcloser(rSpell, wSpell),
                                     new LeeSinInterrupter(rSpell)
                                  });

            superParent.Load();

            insecParent.Menu.Style = FontStyle.Bold;
            insecParent.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Lee Sin!</font></b>");
        }
    }
}
