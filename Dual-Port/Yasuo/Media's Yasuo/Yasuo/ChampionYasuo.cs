using EloBuddy;
using LeagueSharp.Common;
namespace YasuoMedia.Yasuo
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using global::YasuoMedia.Base.Modules.Assembly;
    using CommonEx.Classes;
    using global::YasuoMedia.Yasuo.Modules.Auto;
    using global::YasuoMedia.Yasuo.Modules.Protector;
    using global::YasuoMedia.Yasuo.Modules.WallDash;
    using global::YasuoMedia.Yasuo.OrbwalkingModes.Combo;
    using global::YasuoMedia.Yasuo.OrbwalkingModes.LaneClear;
    using global::YasuoMedia.Yasuo.OrbwalkingModes.LastHit;
    using global::YasuoMedia.Yasuo.OrbwalkingModes.Mixed;

    using LeagueSharp.Common;

    using Version = global::YasuoMedia.Base.Modules.Assembly.Version;

    #endregion

    internal class ChampionYasuo : IChampion
    {
        #region Public Properties

        public string Name { get; } = "Yasuo";

        #endregion

        #region Public Methods and Operators

        [SuppressMessage("ReSharper", "RedundantNameQualifier")]
        public void Load()
        {
            Console.WriteLine("1a");

            #region parents

            // Core
            var assembly = new Assembly();

            Console.WriteLine("2a");

            // Orbwalking Modes
            var combo = new Combo();
            Console.WriteLine("3a");
            var laneclear = new LaneClear();
            Console.WriteLine("4a");
            var lasthit = new LastHit();
            Console.WriteLine("5a");
            var mixed = new Mixed();
            Console.WriteLine("6a");

            // Additional Features
            var module = new Modules.Modules();
            Console.WriteLine("7a");
            var protector = new Protector();
            Console.WriteLine("8a");

            #endregion

            #region features

            GlobalVariables.Assembly.Features.AddRange(
                new List<IFeatureChild>
                    {

                                // Core
                                new Version(assembly),
                                //new Debug(assembly), new CastManager(assembly), new SpellManager(assembly),

                                /*
                                new OrbwalkingModes.Combo.SteelTempest(combo),
                                new OrbwalkingModes.Combo.SweepingBlade(combo),
                                new OrbwalkingModes.Combo.LastBreath(combo),
                                new OrbwalkingModes.LaneClear.SteelTempest(laneclear),
                                new OrbwalkingModes.LaneClear.SweepingBlade(laneclear),
                                new OrbwalkingModes.LaneClear.Eq(laneclear),
                                new OrbwalkingModes.LastHit.SteelTempest(lasthit),
                                new OrbwalkingModes.LastHit.SweepingBlade(lasthit),
                                new OrbwalkingModes.LastHit.Eq(lasthit),
                                new OrbwalkingModes.Mixed.SteelTempest(mixed),
                                new OrbwalkingModes.Mixed.SweepingBlade(mixed),
                                new Modules.WallDash.WallDash(module),
                                new Modules.Flee.SweepingBlade(module),
                                */
                });

            Console.WriteLine("9a");

            foreach (var feature in GlobalVariables.Assembly.Features.Where(feature => !feature.Handled))
            {
                if (GlobalVariables.Debug)
                {
                    Console.WriteLine(@"Loading Feature: {0}, Enabled: {1}", feature.Name, feature.Enabled);
                }

                feature.HandleEvents();
            }

            #endregion
        }

        #endregion
    }
}