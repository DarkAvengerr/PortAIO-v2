using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Damage;
    using ReformedAIO.Champions.Ziggs.Core.Spells;
    using ReformedAIO.Champions.Ziggs.Drawings.Damage;
    using ReformedAIO.Champions.Ziggs.Drawings.Spells;
    using ReformedAIO.Champions.Ziggs.Killsteal;
    using ReformedAIO.Champions.Ziggs.Misc;
    using ReformedAIO.Library.SpellParent;

    using ReformedAIO.Champions.Ziggs.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Ziggs.OrbwalkingMode.Flee;
    using ReformedAIO.Champions.Ziggs.OrbwalkingMode.Harass;
    using ReformedAIO.Champions.Ziggs.OrbwalkingMode.Jungleclear;
    using ReformedAIO.Champions.Ziggs.OrbwalkingMode.Laneclear;

    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

    internal sealed class ZiggsLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Ziggs";

        public override string InternalName { get; set; } = "Ziggs";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Ziggs" };

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

            var ziggsDmg = new ZiggsDamage(qSpell, wSpell, eSpell, rSpell);

            comboParent.Add(new List<Base>()
                                {
                                    new QCombo(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                    new WCombo(wSpell, eSpell, qSpell, ziggsDmg).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                    new ECombo(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                    new RCombo(rSpell, ziggsDmg).Guardian(rReadyGuardian)
                                });

            harassParent.Add(new List<Base>()
                                 {
                                     new QHarass(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new WHarass(wSpell, eSpell, qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
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
                                    new ZiggsDamageDrawing(ziggsDmg),
                                    new QDrawing(qSpell),
                                    new EDrawing(eSpell)
                                  });

            superParent.Add(new List<Base>
                                  {
                                     orbwalkerModule,
                                     comboParent,
                                     harassParent,
                                     laneParent,
                                     jungleParent,
                                     new Flee(wSpell),
                                     killstealParnet,
                                     drawingParent,
                                     new ZiggsAntiGapcloser(wSpell),
                                     new ZiggsInterrupter(wSpell)
                                  });

            superParent.Load();

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Ziggs!</font></b>");
        }
    }
}
