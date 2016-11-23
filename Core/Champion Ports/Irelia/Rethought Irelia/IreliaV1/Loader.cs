//     Copyright (C) 2016 Rethought
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
//     Created: 04.10.2016 1:05 PM
//     Last Edited: 04.10.2016 1:44 PM

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Rethought_Irelia.IreliaV1
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.DamageCalculator;
    using RethoughtLib.Drawings;
    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Guardians;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Orbwalker.Implementations;

    using Rethought_Irelia.IreliaV1.Combo;
    using Rethought_Irelia.IreliaV1.GraphGenerator;
    using Rethought_Irelia.IreliaV1.Interrupter;
    using Rethought_Irelia.IreliaV1.Pathfinder;
    using Rethought_Irelia.IreliaV1.PathfindToMouse;
    using Rethought_Irelia.IreliaV1.Spells;

    using Color = SharpDX.Color;

    #endregion

    /// <summary>
    ///     Class which represents the loader for <c>Irelia</c>
    /// </summary>
    /// <seealso cref="RethoughtLib.Bootstraps.Abstract_Classes.LoadableBase" />
    public class Loader : LoadableBase
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name that will get displayed.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string DisplayName { get; set; } = "Rethought Irelia";

        /// <summary>
        ///     Gets or sets the internal name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string InternalName { get; set; } = "Rethought_Irelia_Version1";

        /// <summary>
        ///     Gets or sets the tags.
        /// </summary>
        /// <value>
        ///     The tags.
        /// </value>
        public override IEnumerable<string> Tags { get; set; } = new[] { "Irelia" };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public override void Load()
        {
            var superParent = new SuperParent(this.DisplayName);

            var orbwalkerModule = new OrbwalkerModule();
            orbwalkerModule.Load();

            var graphGeneratorModule = new GraphGeneratorModule(null);

            var pathfinderModule = new PathfinderModule();

            var damageCalculator = new DamageCalculatorParent();

            //var ireliaP = new IreliaP();
            var ireliaQ = new IreliaQ(graphGeneratorModule, pathfinderModule);
            graphGeneratorModule.IreliaQ = ireliaQ;
            var ireliaW = new IreliaW();
            var ireliaE = new IreliaE();
            var ireliaR = new IreliaR();

            var aa = new AutoAttacks();

            damageCalculator.Add(aa);
            damageCalculator.Add(ireliaQ);
            damageCalculator.Add(ireliaW);
            damageCalculator.Add(ireliaE);
            damageCalculator.Add(ireliaR);

            graphGeneratorModule.IreliaQ = ireliaQ;

            var spellInterrupterModule =
                new SpellInterrupterModule(new SpellInterrupter(ireliaE.Spell, sender => ireliaE.CanStun(sender)));

            var dashToMouseModule = new PathfindToMouseModule(ireliaQ);

            var spellParent = new SpellParent.SpellParent();

            spellParent.Add(new List<Base> { ireliaQ, ireliaW, ireliaE, ireliaR, });
            spellParent.Load();

            var comboParent = new OrbwalkingParent(
                                  "Combo",
                                  orbwalkerModule.OrbwalkerInstance,
                                  Orbwalking.OrbwalkingMode.Combo);

            var laneClearParent = new OrbwalkingParent(
                                      "LaneClear",
                                      orbwalkerModule.OrbwalkerInstance,
                                      Orbwalking.OrbwalkingMode.LaneClear);

            var lastHitParent = new OrbwalkingParent(
                                    "LastHit",
                                    orbwalkerModule.OrbwalkerInstance,
                                    Orbwalking.OrbwalkingMode.LastHit,
                                    Orbwalking.OrbwalkingMode.Mixed,
                                    Orbwalking.OrbwalkingMode.LaneClear);

            var mixedParent = new OrbwalkingParent(
                                  "Mixed",
                                  orbwalkerModule.OrbwalkerInstance,
                                  Orbwalking.OrbwalkingMode.Mixed);

            var drawingParent = new Parent("Drawings");

            comboParent.Add(
                new List<Base>
                    {
                        new Q(ireliaQ, damageCalculator).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                        new W(ireliaW, ireliaQ).Guardian(new SpellMustBeReady(SpellSlot.W)),
                        new E(ireliaE).Guardian(new SpellMustBeReady(SpellSlot.E)).Guardian(new PlayerMustNotBeWindingUp()),
                        new R(ireliaR, ireliaQ, damageCalculator).Guardian(new SpellMustBeReady(SpellSlot.R))
                            .Guardian(new PlayerMustNotBeWindingUp())
                    });

            laneClearParent.Add(
                new List<Base>
                    {
                        new LaneClear.W(ireliaW).Guardian(new SpellMustBeReady(SpellSlot.W)),
                        new LaneClear.E(ireliaE).Guardian(new SpellMustBeReady(SpellSlot.E))
                            .Guardian(new PlayerMustNotBeWindingUp())
                    });

            lastHitParent.Add(
                new List<Base>
                    {
                        new LastHit.Q(ireliaQ).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                        new LastHit.E(ireliaE).Guardian(new SpellMustBeReady(SpellSlot.E))
                            .Guardian(new PlayerMustNotBeWindingUp())
                    });

            mixedParent.Add(
                new List<Base>
                    {
                        new Q(ireliaQ, damageCalculator).Guardian(new SpellMustBeReady(SpellSlot.Q)),
                        new W(ireliaW, ireliaQ).Guardian(new SpellMustBeReady(SpellSlot.W)),
                        new Mixed.E(ireliaE).Guardian(new SpellMustBeReady(SpellSlot.E))
                            .Guardian(new PlayerMustNotBeWindingUp())
                    });

            var module = new DamageDrawingParent("Damage Drawings");

            const byte defaultAlpha = 50 * 255 / 100;

            module.Add(
                new List<IDamageDrawing>
                    {
                        new DamageDrawingChild("Auto-Attacks", aa.GetDamage)
                            {
                                Color = new Color(40, 175, 175) { A = defaultAlpha },
                            },
                        new DamageDrawingChild("Q", ireliaQ.GetDamage)
                            {
                                Color = new Color(25, 100, 125) { A = defaultAlpha }
                            },
                        new DamageDrawingChild("W", ireliaW.GetDamage)
                            {
                                Color = new Color(40, 175, 175) { A = defaultAlpha }
                            },
                        new DamageDrawingChild("E", ireliaE.GetDamage)
                            {
                                Color = new Color(255, 210, 95) { A = defaultAlpha }
                            },
                        new DamageDrawingChild("R", ireliaR.GetDamage)
                            {
                                Color = new Color(240, 150, 75) { A = defaultAlpha }
                            }
                    });

            var spellRangeParent = new Parent("Ranges");

            spellRangeParent.Add(
                new List<Base>
                    {
                        new RangeDrawingChild(ireliaQ.Spell, "Q"),
                        new RangeDrawingChild(ireliaE.Spell, "E"),
                        new RangeDrawingChild(ireliaR.Spell, "R", true),
                    });

            drawingParent.Add(new List<Base> { spellRangeParent, module });

            superParent.Add(
                new List<Base>
                    {
                        orbwalkerModule,
                        spellParent,
                        damageCalculator,
                        pathfinderModule,
                        graphGeneratorModule,
                        comboParent,
                        laneClearParent,
                        lastHitParent,
                        mixedParent,
                        dashToMouseModule,
                        spellInterrupterModule,
                        drawingParent
                    });

            superParent.Load();

            superParent.Menu.Style = FontStyle.Bold;
            superParent.Menu.Color = Color.Firebrick;
        }

        #endregion
    }
}