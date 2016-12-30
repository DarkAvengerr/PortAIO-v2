using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using OrbwalkingMode.Combo;

    using Core.Damage;
    using Core.Spells;

    using Antigapcloser;
    using Core.Condemn_Logic;
    using Drawings.Damage;
    using Drawings.Spells;
    using Interrupter;
    using OrbwalkingMode.Harass;
    using OrbwalkingMode.Jungle;
    using OrbwalkingMode.Laneclear;

    using Vayne.Killsteal;
   
    using ReformedAIO.Library.Dash_Handler;
    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

    internal sealed class VayneLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Vayne";

        public override string InternalName { get; set; } = "Vayne";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Vayne" };

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
           
            var killstealParnet = new Parent("Killsteal");
            var drawingParent = new Parent("Drawings");

            var condemnTypes = new CondemnTypes();
            var dmg = new Damages(qSpell, wSpell, eSpell, rSpell);
            var dashSmart = new DashSmart();

            var mustNotBeWindingUpGuardian = new PlayerMustNotBeWindingUp();
            var qReadyGuardian = new SpellMustBeReady(SpellSlot.Q);
            var wReadyGuardian = new SpellMustBeReady(SpellSlot.W);
            var eReadyGuardian = new SpellMustBeReady(SpellSlot.E);
            var rReadyGuardian = new SpellMustBeReady(SpellSlot.R);

            comboParent.Add(new List<Base>()
                                {
                                    new QCombo(qSpell, dashSmart).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                    new ECombo(eSpell, condemnTypes).Guardian(eReadyGuardian),
                                    new RCombo(rSpell, qSpell, dmg).Guardian(mustNotBeWindingUpGuardian).Guardian(rReadyGuardian)
                                });

            harassParent.Add(new List<Base>
            {
                new QHarass(qSpell, dashSmart).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                new EHarass(eSpell, condemnTypes).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
            });

            laneParent.Add(new List<Base>()
                               {
                                  new QLane(qSpell, dashSmart, dmg)
                               });

            jungleParent.Add(new List<Base>()
                                 {
                                   new QJungle(qSpell, dashSmart).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                   new EJungle(eSpell, condemnTypes).Guardian(eReadyGuardian),
                                 });

            killstealParnet.Add(new List<Base>
                                    {
                                        new QKillsteal(qSpell, dashSmart).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                        new EKillsteal(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian)
                                    });

            drawingParent.Add(new List<Base>
                                  {
                                    new DamageDrawing(dmg),
                                    new EDraw(eSpell),
                                    new RDraw(rSpell)
                                  });

            superParent.Add(new List<Base>
                                  {
                                     orbwalkerModule,
                                     comboParent,
                                     harassParent,
                                     laneParent,
                                     jungleParent,
                                     killstealParnet,
                                     new VayneAntiGapcloser(qSpell, eSpell),
                                     new VayneInterrupter(eSpell).Guardian(eReadyGuardian),
                                     drawingParent,
                                  });

            superParent.Load();

            orbwalkerModule.Menu.Style = FontStyle.Bold;
            orbwalkerModule.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Vayne!</font></b>");
        }
    }
}
