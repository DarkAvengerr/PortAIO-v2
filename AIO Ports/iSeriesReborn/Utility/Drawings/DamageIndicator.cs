// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DamageIndicator.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//             This program is free software: you can redistribute it and/or modify
//             it under the terms of the GNU General Public License as published by
//             the Free Software Foundation, either version 3 of the License, or
//             (at your option) any later version.
//   
//             This program is distributed in the hope that it will be useful,
//             but WITHOUT ANY WARRANTY; without even the implied warranty of
//             MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//             GNU General Public License for more details.
//   
//             You should have received a copy of the GNU General Public License
//             along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The Damage Inidicator
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Utility.Drawings
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The Damage Inidicator
    /// </summary>
    internal class DamageIndicator
    {
        #region Constants

        /// <summary>
        ///     TODO The height.
        /// </summary>
        private const int Height = 8;

        /// <summary>
        ///     TODO The width.
        /// </summary>
        private const int Width = 103;

        /// <summary>
        ///     TODO The x offset.
        /// </summary>
        private const int XOffset = 10;

        /// <summary>
        ///     TODO The y offset.
        /// </summary>
        private const int YOffset = 20;

        #endregion

        #region Static Fields

        /// <summary>
        ///     TODO The color.
        /// </summary>
        public static Color Color = Color.Cyan;

        /// <summary>
        ///     TODO The enabled.
        /// </summary>
        public static bool Enabled = true;

        /// <summary>
        ///     TODO The fill.
        /// </summary>
        public static bool Fill = true;

        /// <summary>
        ///     TODO The fill color.
        /// </summary>
        public static Color FillColor = Color.Cyan;

        /// <summary>
        ///     TODO The text.
        /// </summary>
        private static readonly Render.Text Text = new Render.Text(
            0,
            0,
            string.Empty,
            14,
            SharpDX.Color.Red,
            "monospace");

        /// <summary>
        ///     TODO The _damage to unit.
        /// </summary>
        private static DamageToUnitDelegate _damageToUnit;

        #endregion

        #region Delegates

        /// <summary>
        ///     TODO The damage to unit delegate.
        /// </summary>
        /// <param name="hero">
        ///     TODO The hero.
        /// </param>
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the damage to unit.
        /// </summary>
        public static DamageToUnitDelegate DamageToUnit
        {
            get
            {
                return _damageToUnit;
            }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }

                _damageToUnit = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     TODO The drawing_ on draw.
        /// </summary>
        /// <param name="args">
        ///     TODO The args.
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = "Killable: " + (unit.Health - damage);
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, Color);

                if (Fill)
                {
                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                    for (var i = 0; i < differenceInHp; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
        }

        #endregion
    }
}