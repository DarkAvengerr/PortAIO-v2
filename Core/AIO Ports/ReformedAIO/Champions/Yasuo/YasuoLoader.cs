using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Damage;
    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Champions.Yasuo.Drawings.Damage;
    using ReformedAIO.Champions.Yasuo.Drawings.SpellDrawings;
    using ReformedAIO.Champions.Yasuo.Killsteal;
    using ReformedAIO.Champions.Yasuo.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Yasuo.OrbwalkingMode.Harass;
    using ReformedAIO.Champions.Yasuo.OrbwalkingMode.Lane;
    using ReformedAIO.Champions.Yasuo.OrbwalkingMode.Jungle;

    using ReformedAIO.Champions.Yasuo.Utility;

    using ReformedAIO.Library.SpellParent;

    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;
   
    internal sealed class YasuoLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Yasuo";

        public override string InternalName { get; set; } = "Yasuo";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Yasuo" };

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);
            superParent.Initialize();

            var qSpell = new Q1Spell();
            var q3Spell = new Q3Spell();
            var wSpell = new WSpell();
            var eSpell = new ESpell();
            var rSpell = new RSpell();

            var spellParent = new SpellParent();
            spellParent.Add(new List<Base>
                                  {
                                     qSpell,
                                     q3Spell,
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
            var drawingParent = new Parent("Drawings");
            var utilityParent = new Parent("Reformed Utility");

            utilityParent.Add(new YasuoSkinchanger());

            var yasuoDmg = new YasuoDamage(qSpell, eSpell, rSpell);

            var mustNotBeWindingUpGuardian = new PlayerMustNotBeWindingUp();
            var qReadyGuardian = new SpellMustBeReady(SpellSlot.Q);
            var wReadyGuardian = new SpellMustBeReady(SpellSlot.W);
            var eReadyGuardian = new SpellMustBeReady(SpellSlot.E);
            var rReadyGuardian = new SpellMustBeReady(SpellSlot.R);

            comboParent.Add(new List<Base>()
                                {
                                    new QCombo(qSpell, q3Spell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                    //new WCombo(wSpell).Guardian(wReadyGuardian),
                                    new ECombo(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                    new RCombo(rSpell, yasuoDmg).Guardian(mustNotBeWindingUpGuardian).Guardian(rReadyGuardian)
                                });
            harassParent.Add(new List<Base>()
                                 {
                                     new QHarass(qSpell, q3Spell).Guardian(qReadyGuardian),
                                     new EHarass(eSpell).Guardian(eReadyGuardian),
                                 });
            laneParent.Add(new List<Base>()
                               {
                                   new QLane(qSpell, q3Spell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                   new ELane(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(wReadyGuardian),
                               });

            jungleParent.Add(new List<Base>()
                                 {
                                     new QJungle(qSpell, q3Spell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                     new EJungle(eSpell).Guardian(mustNotBeWindingUpGuardian).Guardian(eReadyGuardian),
                                 });

            killstealParnet.Add(new List<Base>
                                    {
                                        new QKillsteal(qSpell, q3Spell).Guardian(mustNotBeWindingUpGuardian).Guardian(qReadyGuardian),
                                        new EKillsteal(eSpell).Guardian(rReadyGuardian)
                                    });

            drawingParent.Add(new List<Base>
                                  {
                                    new YasuoDamageDrawing(yasuoDmg),
                                    new QDrawing(qSpell, q3Spell),
                                    new EDrawing(eSpell)
                                  });

            superParent.Add(new List<Base>
                                  {
                                     utilityParent,
                                     orbwalkerModule,
                                     comboParent,
                                     harassParent,
                                     laneParent,
                                     jungleParent,
                                     new Flee(eSpell),
                                     killstealParnet,
                                     drawingParent,
                                  });

            superParent.Load();

            utilityParent.Menu.Style = FontStyle.Bold;
            utilityParent.Menu.Color = Color.Cyan;

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Reformed AIO</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> - Yasuo!</font></b>");
        }
    }
}
