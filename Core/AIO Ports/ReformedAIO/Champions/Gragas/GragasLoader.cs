using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Core.Damage;
    using ReformedAIO.Champions.Gragas.Core.Spells;
    using ReformedAIO.Champions.Gragas.Drawings.Damage;
    using ReformedAIO.Champions.Gragas.Drawings.Insec;
    using ReformedAIO.Champions.Gragas.Drawings.Spells;
    using ReformedAIO.Champions.Gragas.Killsteal;
    using ReformedAIO.Champions.Gragas.Misc;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Harass;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Jungle;
    using ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane;
    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

    internal sealed class GragasLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Gragas";

        public override string InternalName { get; set; } = "Gragas";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Gragas" };

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

            var GragasDmg = new GragasDamage(qSpell, wSpell, eSpell, rSpell);

            comboParent.Add(new List<Base>()
                                {
                                    new WCombo(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                    new ECombo(eSpell, GragasDmg).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                    new RQCombo(qSpell, rSpell)
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
                                        new EKillsteal(eSpell).Guardian(eReadyGuardian),
                                    });

            drawingParent.Add(new List<Base>
                                  {
                                    new DamageDrawing(GragasDmg),
                                    new InsecDrawing(qSpell, rSpell),
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
                                     new GragasAntiGapcloser(eSpell),
                                     new GragasInterrupter(rSpell)
                                  });

            superParent.Load();

            orbwalkerModule.Menu.Style = FontStyle.Bold;
            orbwalkerModule.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Gragas!</font></b>");
        }
    }
}
