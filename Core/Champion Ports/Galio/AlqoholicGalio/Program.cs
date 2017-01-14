// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LeagueSharp">
//   Copyright (C) 2016 LeagueSharp
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
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicGalio
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    /// <summary>
    ///     Program Class
    /// </summary>
    internal class Program
    {
        #region Methods

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Galio")
            {
                return;
            }

            Galio.Init();
        }

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        #endregion
    }
}