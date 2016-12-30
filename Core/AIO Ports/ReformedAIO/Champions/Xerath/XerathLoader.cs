using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Xerath
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Xerath.Core.Damage;
    using ReformedAIO.Champions.Xerath.Core.Spells;
    using ReformedAIO.Champions.Xerath.Drawings.Damage;
    using ReformedAIO.Champions.Xerath.Drawings.Spells;
    using ReformedAIO.Champions.Xerath.Killsteal;
    using ReformedAIO.Champions.Xerath.Misc;
    using ReformedAIO.Champions.Xerath.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Xerath.OrbwalkingMode.Harass;
    using ReformedAIO.Champions.Xerath.OrbwalkingMode.Jungle;
    using ReformedAIO.Champions.Xerath.OrbwalkingMode.Lane;
    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

    internal sealed class XerathLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Xerath";

        public override string InternalName { get; set; } = "Xerath";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Xerath" };

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

            var XerathDmg = new XerathDamage(qSpell, wSpell, eSpell, rSpell);

            comboParent.Add(new List<Base>()
                                {
                                    new QCombo(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                    new WCombo(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                    new ECombo(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
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
                                    new DamageDrawing(XerathDmg),
                                    new QDrawing(qSpell),
                                    new WDrawing(wSpell),
                                    new EDrawing(eSpell),
                                    new RDrawing(rSpell)
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
                                     new XerathAntiGapcloser(qSpell),
                                     new XerathInterrupter(qSpell)
                                  });

           

            superParent.Load();

            orbwalkerModule.Menu.Style = FontStyle.Bold;
            orbwalkerModule.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Xerath!</font></b>");
        }
    }
}
