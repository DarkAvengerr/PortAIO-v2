using System;
using System.Collections.Generic;
using LeagueSharp.Common;
using _Project_Geass.Functions;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Champions.Core
{

    internal class Base
    {
        #region Public Fields

        public readonly string BaseName=Names.ProjectName+StaticObjects.Player.ChampionName+".";

        /// <summary>
        ///     The RNG :D
        /// </summary>
        public readonly Random Rng;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Base" /> class.
        /// </summary>
        /// <param name="q">
        ///     The q.
        /// </param>
        /// <param name="w">
        ///     The w.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        /// <param name="r">
        ///     The r.
        /// </param>
        /// <param name="rng">
        ///     The RNG.
        /// </param>
        public Base(Spell q, Spell w, Spell e, Spell r, Random rng)
        {
            Rng=rng;
            Q=q;
            W=w;
            E=e;
            R=r;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Base" /> class.
        /// </summary>
        public Base() {Rng=new Random();}

        #endregion Public Constructors

        #region Public Properties

        public Spell E{get;set;}

        /// <summary>
        ///     Orbwalker
        /// </summary>
        /// <value>
        ///     The orbwalker.
        /// </value>
        public Orbwalking.Orbwalker Orbwalker{get;set;}

        public Spell Q{get;set;}
        public Spell R{get;set;}

        /// <summary>
        ///     Champion Spells
        /// </summary>
        /// <value>
        ///     Champion spells.
        /// </value>
        public Dictionary<object, object> Spells{get;} = new Dictionary<object, object>();

        public Spell W{get;set;}

        #endregion Public Properties
    }

}