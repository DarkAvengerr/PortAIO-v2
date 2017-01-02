// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilityManager.cs" company="LeagueSharp">
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
namespace AlqoholicGalio.Managers
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = System.Drawing.Color;

    #endregion

    internal class DrawingManager
    {
        #region Public Methods and Operators

        public static void Init()
        {
            Console.WriteLine("Utilities Loaded");
        }

        public static void OnDraw(EventArgs args)
        {
            if (!MenuManager.DrawEnabled)
            {
                return;
            }
            if (MenuManager.DrawDamage)
            {
            }
            if (MenuManager.DrawQ)
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    SpellManager.Q.Range - 80,
                    SpellManager.Q.IsReady() ? Color.LightCyan : Color.Tomato);
            }
            if (MenuManager.DrawW)
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    SpellManager.W.Range - 80,
                    SpellManager.W.IsReady() ? Color.LightCyan : Color.Tomato);
            }
            if (MenuManager.DrawE)
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    SpellManager.E.Range - 80,
                    SpellManager.E.IsReady() ? Color.LightCyan : Color.Tomato);
            }
            if (MenuManager.DrawR)
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    SpellManager.R.Range - 80,
                    SpellManager.R.IsReady() ? Color.LightCyan : Color.Tomato);
            }
        }

        #endregion
    }
}