using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using ReformedAIO.Champions.Lux.Core.Damage;
    using ReformedAIO.Champions.Lux.Core.Spells;
    using ReformedAIO.Champions.Lux.Drawings.Damage;
    using ReformedAIO.Champions.Lux.Drawings.Spells;
    using ReformedAIO.Champions.Lux.Killsteal;
    using ReformedAIO.Champions.Lux.Misc;
    using ReformedAIO.Champions.Lux.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Lux.OrbwalkingMode.Harass;
    using ReformedAIO.Champions.Lux.OrbwalkingMode.Jungle;
    using ReformedAIO.Champions.Lux.OrbwalkingMode.Lane;

    using Color = SharpDX.Color;

    internal sealed class LuxLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Lux";

        public override string InternalName { get; set; } = "Lux";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Lux" };

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

            var LuxDmg = new LuxDamage(qSpell, eSpell, rSpell);

            comboParent.Add(new List<Base>()
                                {
                                    new QCombo(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                    new WCombo(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                    new ECombo(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                    new RCombo(rSpell, eSpell, LuxDmg).Guardian(mustNotBeWindingUpGuardian).Guardian(rReadyGuardian)
                                });

            harassParent.Add(new List<Base>()
                                 {
                                     new QHarass(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new WHarass(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                     new EHarass(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                                 });
            laneParent.Add(new List<Base>()
                               {
                                   new ELane(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                               });

            jungleParent.Add(new List<Base>()
                                 {
                                     new QJungle(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new EJungle(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                                 });

            killstealParnet.Add(new List<Base>()
                                    {
                                        new QKillsteal(qSpell).Guardian(qReadyGuardian),
                                        new EKillsteal(eSpell).Guardian(eReadyGuardian),
                                        new RKillsteal(rSpell).Guardian(rReadyGuardian)
                                    });

            drawingParent.Add(new List<Base>
                                  {
                                    new DamageDrawing(LuxDmg),
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
                                     new LuxAntiGapcloser(qSpell)
                                  });

            superParent.Load();

            orbwalkerModule.Menu.Style = FontStyle.Bold;
            orbwalkerModule.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Lux!</font></b>");
        }
    }
}
