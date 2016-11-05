using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SoloVayne.Modules;
using SoloVayne.Modules.Condemn;
using SoloVayne.Modules.General;
using SoloVayne.Skills;
using SoloVayne.Skills.Tumble;
using SoloVayne.Skills.Tumble.CCTracker;
using SoloVayne.Skills.Tumble.CCTracker.Tracker;
using SoloVayne.Utility.Enums;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Utility
{
    class Variables
    {
        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public static Menu Menu { get; set; }

        /// <summary>
        /// Gets or sets the instance of the assembly.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static SOLOBootstrap Instance { get; set; }

        /// <summary>
        /// Gets or sets the orbwalker.
        /// </summary>
        /// <value>
        /// The orbwalker.
        /// </value>
        public static Orbwalking.Orbwalker Orbwalker { get; set; }

        /// <summary>
        /// The spells dictionary
        /// </summary>
        public static Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q) },
            { SpellSlot.W, new Spell(SpellSlot.W) },
            { SpellSlot.E, new Spell(SpellSlot.E, 590f) },
            { SpellSlot.R, new Spell(SpellSlot.R) }
        };

        /// <summary>
        /// The tracker
        /// </summary>
        public static TrackerBootstrap tracker = new TrackerBootstrap();

        /// <summary>
        /// The cc list
        /// </summary>
        public static CCList CCList = new CCList();

        /// <summary>
        /// The skills dictionary
        /// </summary>
        public static List<Skill> skills = new List<Skill>()
        {
            new Tumble(),
            new Condemn()
        };

        /// <summary>
        /// The module list
        /// </summary>
        public static List<ISOLOModule> ModuleList = new List<ISOLOModule>()
        {
            //Condemn Modules
            new AutoE(), 
            new JungleE(),
            new SaveE(),

            //Tumble Modules
            new KSQ(),

            //General Modules
            new AutoR(),
            new ActivatorModule(),
            new NoAAStealth()
        };
    }
}
