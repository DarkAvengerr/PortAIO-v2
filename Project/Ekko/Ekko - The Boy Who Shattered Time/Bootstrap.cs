// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrap.cs" company="LeagueSharp">
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
//   TODO The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy;
using LeagueSharp.Common;
namespace Ekko_the_Boy_Who_Shattered_Time
{
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     TODO The program.
    /// </summary>
    public class Bootstrap
    {
        #region Methods

        /// <summary>
        ///     Requests the current using EloBuddy; 
        /// </summary>
        /// <returns>
        ///     Current Namespace.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetCurrentNamespace()
        {
            var dType = Assembly.GetCallingAssembly().EntryPoint.DeclaringType;
            return dType == null ? typeof(Bootstrap).Namespace : dType.Namespace;
        }

        /// <summary>
        ///     The main entry point.
        /// </summary>
        /// <param name="args">
        ///     The data transferred to the main entry point.
        /// </param>
        public static void Main()
        {
            Ekko.Player = ObjectManager.Player;
            if (Ekko.Player.ChampionName.Equals(GetCurrentNamespace()))
            {
                Ekko.Menu = new Menu("[L33T] Ekko", "l33t.ekko", true);
                EntryPoint.Invoke(Ekko.Menu);
                Ekko.Menu.AddToMainMenu();

                GameObject.OnCreate += GameObjects.OnCreate;
                GameObject.OnDelete += GameObjects.OnDelete;
                Game.OnUpdate += GameObjects.OnUpdate;
            }
        }

        #endregion
    }
}