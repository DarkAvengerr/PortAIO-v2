// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameObjects.cs" company="LeagueSharp">
//   Copyright (C) 2015 L33T
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Game Objects Tracker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_the_Boy_Who_Shattered_Time
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    /// <summary>
    ///     Game Objects Tracker.
    /// </summary>
    public class GameObjects
    {
        #region Static Fields

        /// <summary>
        ///     The heroes collection.
        /// </summary>
        private static readonly List<AIHeroClient> Heroes = new List<AIHeroClient>();

        /// <summary>
        ///     The minions collection.
        /// </summary>
        private static readonly List<Obj_AI_Minion> Minions = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The last tick collection update.
        /// </summary>
        private static int lastTickUpdate = Ekko.GameTime;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="GameObjects" /> class.
        /// </summary>
        static GameObjects()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                Heroes.Add(hero);
            }

            Ekko.EkkoGhost =
                ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(p => p.Name.Equals(Ekko.EkkoGhostName));
            Ekko.EkkoField =
                ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(p => p.Name.Equals(Ekko.EkkoFieldName));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the ally heroes.
        /// </summary>
        public static IEnumerable<AIHeroClient> AllyHeroes
        {
            get
            {
                return Heroes.Where(h => h != null && h.IsValid && h.IsAlly);
            }
        }

        /// <summary>
        ///     Gets the ally minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> AllyMinions
        {
            get
            {
                return Minions.Where(h => h != null && h.IsValid && h.IsAlly);
            }
        }

        /// <summary>
        ///     Gets the enemy heroes.
        /// </summary>
        public static IEnumerable<AIHeroClient> EnemyHeroes
        {
            get
            {
                return Heroes.Where(h => h != null && h.IsValid && h.IsEnemy);
            }
        }

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> EnemyMinions
        {
            get
            {
                return Minions.Where(h => h != null && h.IsValid && h.IsEnemy);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     OnCreate event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        public static void OnCreate(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                Minions.Add(minion);
            }

            var particle = sender as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals(Ekko.EkkoGhostName))
                {
                    Ekko.EkkoGhost = particle;
                }
                else if (particle.Name.Equals(Ekko.EkkoFieldName))
                {
                    Ekko.EkkoField = particle;
                }
            }
        }

        /// <summary>
        ///     OnDelete event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        public static void OnDelete(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && Minions.Contains(minion))
            {
                Minions.Remove(minion);
            }

            var particle = sender as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals(Ekko.EkkoGhostName))
                {
                    Ekko.EkkoGhost = null;
                }
                else if (particle.Name.Equals(Ekko.EkkoFieldName))
                {
                    Ekko.EkkoField = particle;
                }
            }
        }

        /// <summary>
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        public static void OnUpdate(EventArgs args)
        {
            if (Ekko.GameTime - lastTickUpdate > 1000)
            {
                Minions.Clear();
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    Minions.Add(minion);
                }

                lastTickUpdate = Ekko.GameTime;
            }
        }

        #endregion
    }
}