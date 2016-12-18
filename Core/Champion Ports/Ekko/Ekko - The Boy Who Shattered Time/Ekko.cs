// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ekko.cs" company="LeagueSharp">
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
//   <c>Ekko</c> information class.
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
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     <c>Ekko</c> information class.
    /// </summary>
    public class Ekko
    {
        #region Constants

        /// <summary>
        ///     The Delay.
        /// </summary>
        private const int Delay = 50;

        /// <summary>
        ///     The minimum distance.
        /// </summary>
        private const float MinDistance = 400;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The Old Health Percent.
        /// </summary>
        public static readonly IDictionary<int, float> OldHealth = new Dictionary<int, float>();

        /// <summary>
        ///     <c>Ekko</c>'s spells.
        /// </summary>
        public static readonly IDictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>();

        /// <summary>
        ///     The Random.
        /// </summary>
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        ///     The Last Move Command Tick.
        /// </summary>
        private static int lastMoveCommandT;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <c>Ekko</c> field.
        /// </summary>
        public static Obj_GeneralParticleEmitter EkkoField { get; set; }

        /// <summary>
        /// Gets the <c>Ekko</c> field name.
        /// </summary>
        public static string EkkoFieldName
        {
            get
            {
                return "Ekko_Base_W_Detonate_Slow.troy";
            }
        }

        /// <summary>
        ///     Gets or sets the <c>Ekko</c> ghost.
        /// </summary>
        public static Obj_GeneralParticleEmitter EkkoGhost { get; set; }

        /// <summary>
        ///     Gets the <c>Ekko</c> ghost particle name.
        /// </summary>
        public static string EkkoGhostName
        {
            get
            {
                return "Ekko_Base_R_TrailEnd.troy";
            }
        }

        /// <summary>
        ///     Gets the game time.
        /// </summary>
        public static int GameTime
        {
            get
            {
                return (int)(Game.Time * 0x3E8);
            }
        }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        public static Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the <c>orbwalker</c>.
        /// </summary>
        public static Orbwalking.Orbwalker Orbwalker { get; set; }

        /// <summary>
        ///     Gets or sets the player.
        /// </summary>
        public static AIHeroClient Player { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     MoveTo Command.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        /// <param name="holdAreaRadius">
        ///     The hold area radius
        /// </param>
        /// <param name="overrideTimer">
        ///     The override timer
        /// </param>
        /// <param name="useFixedDistance">
        ///     The use fixed distance
        /// </param>
        /// <param name="randomizeMinDistance">
        ///     The randomize min distance
        /// </param>
        public static void MoveTo(
            Vector3 position, 
            float holdAreaRadius = 0, 
            bool overrideTimer = false, 
            bool useFixedDistance = true, 
            bool randomizeMinDistance = true)
        {
            if (GameTime - lastMoveCommandT < Delay && !overrideTimer)
            {
                return;
            }

            lastMoveCommandT = GameTime;

            if (Player.ServerPosition.Distance(position, true) < holdAreaRadius * holdAreaRadius)
            {
                if (Player.Path.Count() > 1)
                {
                    EloBuddy.Player.IssueOrder((GameObjectOrder)10, Player.ServerPosition);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, Player.ServerPosition);
                }

                return;
            }

            var point = position;
            if (useFixedDistance)
            {
                point = Player.ServerPosition
                        + (randomizeMinDistance ? (Random.NextFloat(0.6f, 1) + 0.2f) * MinDistance : MinDistance)
                        * (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D();
            }
            else
            {
                if (randomizeMinDistance)
                {
                    point = Player.ServerPosition
                            + (Random.NextFloat(0.6f, 1) + 0.2f) * MinDistance
                            * (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D();
                }
                else if (Player.ServerPosition.Distance(position) > MinDistance)
                {
                    point = Player.ServerPosition
                            + MinDistance * (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D();
                }
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, point);
        }

        #endregion
    }
}