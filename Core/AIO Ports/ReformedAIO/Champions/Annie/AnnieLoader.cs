using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Annie
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Annie.Core.Damage;
    using ReformedAIO.Champions.Annie.Core.Spells;
    using ReformedAIO.Champions.Annie.Drawings;
    using ReformedAIO.Champions.Annie.Killsteal;
    using ReformedAIO.Champions.Annie.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Annie.OrbwalkingMode.Harass;
    using ReformedAIO.Champions.Annie.OrbwalkingMode.Jungleclear;
    using ReformedAIO.Champions.Annie.OrbwalkingMode.Lasthit;
    using ReformedAIO.Champions.Annie.Stack;
    using ReformedAIO.Champions.Annie.Utility;

    // using ReformedAIO.Champions.Annie.Utility;
    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;
    using QDraw = ReformedAIO.Champions.Annie.Drawings.QDraw;
    using QLane = ReformedAIO.Champions.Annie.OrbwalkingMode.Laneclear.QLane;
    using WLane = ReformedAIO.Champions.Annie.OrbwalkingMode.Laneclear.WLane;

    internal sealed class AnnieLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Annie";

        public override string InternalName { get; set; } = "Annie";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Annie" };

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
            var lasthitParent = new OrbwalkingParent("Lasthit", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LastHit);
            var killstealParnet = new Parent("Killsteal");
            var passiveParent = new Parent("Passive");
            var drawingParent = new Parent("Drawings");
            var utilityParent = new Parent("Reformed Utility");

            utilityParent.Add(new AnnieSkinchanger());

            var annieDmg = new AnnieDamage(qSpell, wSpell, eSpell, rSpell);

            var tibbersAI = new TibbersAI.TibbersAI(orbwalkerModule.OrbwalkerInstance);
            tibbersAI.Load();
            tibbersAI.Switch.InternalEnable(null);

            var mustNotBeWindingUpGuardian = new PlayerMustNotBeWindingUp();
            var qReadyGuardian = new SpellMustBeReady(SpellSlot.Q);
            var wReadyGuardian = new SpellMustBeReady(SpellSlot.W);
            var eReadyGuardian = new SpellMustBeReady(SpellSlot.E);
            var rReadyGuardian = new SpellMustBeReady(SpellSlot.R);

            comboParent.Add(new List<Base>()
                                {
                                    new QCombo(qSpell, orbwalkerModule.OrbwalkerInstance).Guardian(qReadyGuardian),
                                    new WCombo(wSpell).Guardian(wReadyGuardian),
                                    new ECombo(eSpell).Guardian(eReadyGuardian),
                                    new RCombo(rSpell, annieDmg).Guardian(rReadyGuardian)
                                });
            harassParent.Add(new List<Base>()
                                 {
                                     new QHarass(qSpell).Guardian(qReadyGuardian),
                                     new WHarass(wSpell).Guardian(wReadyGuardian),
                                     new EHarass(eSpell).Guardian(eReadyGuardian),
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
                                     new EJungle(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                 });
            lasthitParent.Add(new List<Base>()
                                  {
                                      new QLasthit(qSpell)
                                  });

            killstealParnet.Add(new List<Base>
                                    {
                                        new QKillsteal(qSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                        new WKillsteal(wSpell).Guardian(wReadyGuardian),
                                        new RKillsteal(rSpell).Guardian(rReadyGuardian)
                                    });

            passiveParent.Add(new PassiveStack(eSpell, wSpell));

            drawingParent.Add(new List<Base>
                                  {
                                    new AnnieDamageDraw(annieDmg),
                                    new PassiveDraw(),
                                    new QDraw(qSpell),
                                    new WDraw(wSpell),
                                    new RDraw(rSpell)
                                  });

            superParent.Add(new List<Base>
                                  {
                                     utilityParent,
                                     orbwalkerModule,
                                     comboParent,
                                     harassParent,
                                     laneParent,
                                     jungleParent,
                                     lasthitParent,
                                     killstealParnet,
                                     drawingParent,
                                  });

            superParent.Load();

            utilityParent.Menu.Style = FontStyle.Bold;
            utilityParent.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Annie!</font></b>");
        }
    }
}
