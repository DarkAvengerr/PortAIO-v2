using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian
{
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;
    using Core.Spells.SpellParent;

    using Drawings;
    using Core.Damage;

    using OrbwalkingMode.Combo;
    using OrbwalkingMode.JungleClear;
    using OrbwalkingMode.LaneClear;
    using OrbwalkingMode.Harass;

    using Perma_Active.Killsteal;

    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Color = SharpDX.Color;

    internal sealed class LucianLoader : LoadableBase
    {
        public override string DisplayName { get; set; } = "Reformed Lucian";

        public override string InternalName { get; set; } = "Lucian";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Lucian" };

        public override void Load()
        {
            var superParent = new SuperParent(DisplayName);
            superParent.Initialize();

                 var qSpell = new QSpell();
                var q2Spell = new Q2Spell();
                 var wSpell = new WSpell();
                 var eSpell = new ESpell();
                 var rSpell = new RSpell();

            var spellParent = new SpellParent();
              spellParent.Add(new List<Base>
                                  {
                                     qSpell,
                                     q2Spell,
                                     wSpell,
                                     eSpell,
                                     rSpell
                                  });
            spellParent.Load();

            var dmg = new LucDamage(eSpell, wSpell, qSpell, rSpell);

            var orbwalkerModule = new OrbwalkerModule();
            orbwalkerModule.Load();

            var comboParent  = new OrbwalkingParent("Combo", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Combo);
            var harassParent = new OrbwalkingParent("Harass", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.Mixed);
            var laneParent   = new OrbwalkingParent("Lane", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);
            var jungleParent = new OrbwalkingParent("Jungle", orbwalkerModule.OrbwalkerInstance, Orbwalking.OrbwalkingMode.LaneClear);

         var killstealParnet = new Parent("Killsteal");
           var drawingParent = new Parent("Drawings");
        

            comboParent.Add(new List<Base>
                                {
                                    new QCombo(qSpell, q2Spell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E) { Negated = true}).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                                    new WCombo(wSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q) {Negated = true}).Guardian(new SpellMustBeReady(SpellSlot.W)),
                                    new ECombo(eSpell, dmg).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E)),
                                    new RCombo(rSpell, dmg).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.R)),
                                 });

            harassParent.Add(new List<Base>
                                 {
                                    new QHarass(qSpell, q2Spell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                                    new WHarass(wSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q) {Negated = true}).Guardian(new SpellMustBeReady(SpellSlot.W)),
                                    new EHarass(eSpell, dmg).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E)),
                                 });

            laneParent.Add(new List<Base>
                               {
                                    new QLaneClear(qSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                                    new WLaneClear(wSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.W)),
                                    new ELaneClear(eSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E)),
                               });

            jungleParent.Add(new List<Base>
                                 {
                                     new QJungleClear(qSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.E) {Negated = true}),
                                     new WJungleClear(wSpell).Guardian(new PlayerMustNotBeWindingUp()),
                                     new EJungleClear(eSpell).Guardian(new PlayerMustNotBeWindingUp())
                                 });

            killstealParnet.Add(new List<Base>
                                    {
                                        new Q(qSpell, q2Spell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                                        new W(wSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.W)),
                                        new R(rSpell).Guardian(new PlayerMustNotBeWindingUp()).Guardian(new SpellMustBeReady(SpellSlot.R))
                                    });

            drawingParent.Add(new List<Base>
                                  {
                                    new DmgDraw(dmg),
                                    new RDraw(rSpell),
                                    new WDraw(wSpell)
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
                                  });

            superParent.Load();

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Cyan;

            if (superParent.Loaded)
            {
                Chat.Print(DisplayName + " - Loaded");
            }
        }
    }
}
