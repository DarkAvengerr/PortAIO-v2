// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NidaleeTheBestialHuntress
{
    internal class DamageIndicator
    {
        private const int BarWidth = 104;
        private const int LineThickness = 9;
        private static LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate _damageToUnit;
        private static readonly Vector2 BarOffset = new Vector2(10, 25);
        private static Color _drawingColor;

        public static Color DrawingColor
        {
            get { return _drawingColor; }
            set { _drawingColor = Color.FromArgb(170, value); }
        }

        public static bool Enabled { get; set; }

        public static void Initialize(LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit)
        {
            // Apply needed field delegate for damage calculation
            _damageToUnit = damageToUnit;
            DrawingColor = Color.Green;
            Enabled = true;
            // Register event handlers
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Enabled)
            {
                foreach (
                    var unit in ObjectManager.Get<AIHeroClient>().Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    // Get damage to unit
                    var damage = _damageToUnit(unit);
                    // Continue on 0 damage
                    if (damage <= 0)
                    {
                        continue;
                    }
                    // Get remaining HP after damage applied in percent and the current percent of health
                    var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / unit.MaxHealth;
                    var currentHealthPercentage = unit.Health / unit.MaxHealth;
                    // Calculate start and end point of the bar indicator
                    var startPoint =
                        new Vector2(
                            (int) (unit.HPBarPosition.X + BarOffset.X + damagePercentage * BarWidth),
                            (int) (unit.HPBarPosition.Y + BarOffset.Y) - 5);
                    var endPoint =
                        new Vector2(
                            (int) (unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BarWidth) + 1,
                            (int) (unit.HPBarPosition.Y + BarOffset.Y) - 5);
                    // Draw the line
                    Drawing.DrawLine(startPoint, endPoint, LineThickness, DrawingColor);
                }
            }
        }
    }
}