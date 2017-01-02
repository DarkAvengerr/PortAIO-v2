using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ezreal
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using OrbwalkingMode.Combo;

    using Core.Damage;
    using Core.Spells;

    using Ezreal.Drawings;
    using Ezreal.Killsteal;
    using Ezreal.OrbwalkingMode.Harass;
    using Ezreal.OrbwalkingMode.JungleClear;
    using Ezreal.OrbwalkingMode.LaneClear;

    using ReformedAIO.Champions.Ezreal.Misc;
    using ReformedAIO.Champions.Ezreal.OrbwalkingMode.Stack;
    using ReformedAIO.Champions.Ezreal.Utility;
    using ReformedAIO.Library.Dash_Handler;
    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

    internal sealed class EzrealLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Ezreal";

        public override string InternalName { get; set; } = "Ezreal";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Ezreal" };

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
           //var miscParent = new Parent("Misc");
            var killstealParnet = new Parent("Killsteal");
            var drawingParent = new Parent("Drawings");
            var utilityParent = new Parent("Reformed Utility");

            utilityParent.Add(new EzrealSkinchanger());

            var dmg = new EzrealDamage(eSpell, wSpell, qSpell, rSpell);
            var dashSmart = new DashSmart();

            var mustNotBeWindingUpGuardian = new PlayerMustNotBeWindingUp();
            var qReadyGuardian = new SpellMustBeReady(SpellSlot.Q);
            var wReadyGuardian = new SpellMustBeReady(SpellSlot.W);
            var eReadyGuardian = new SpellMustBeReady(SpellSlot.E);
            var rReadyGuardian = new SpellMustBeReady(SpellSlot.R);

            comboParent.Add(new List<Base>()
                                {
                                    new QCombo(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                    new WCombo(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                    new ECombo(eSpell, dashSmart).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                    new RCombo(rSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(rReadyGuardian)
                                });
            harassParent.Add(new List<Base>()
                                 {
                                     new QHarass(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new WHarass(wSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
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
                                 });

            killstealParnet.Add(new List<Base>
                                    {
                                        new QKillsteal(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                        new WKillsteal(wSpell).Guardian(wReadyGuardian).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                                      //  new RKillsteal(rSpell).Guardian(rReadyGuardian),
                                    });

            drawingParent.Add(new List<Base>
                                  {
                                    new DmgDraw(dmg),
                                    new QDraw(qSpell),
                                    new WDraw(wSpell)
                                  });

            superParent.Add(new List<Base>
                                  {
                                     utilityParent,
                                     orbwalkerModule,
                                     comboParent,
                                     harassParent,
                                     laneParent,
                                     jungleParent,
                                     new StackTear(qSpell, wSpell).Guardian(new PlayerMustNotBeWindingUp()),
                                     killstealParnet,
                                     new EzrealAntiGapcloser(eSpell),
                                     drawingParent,
                                  });

            superParent.Load();

            utilityParent.Menu.Style = FontStyle.Bold;
            utilityParent.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Ezreal!</font></b>");
        }
    }
}
