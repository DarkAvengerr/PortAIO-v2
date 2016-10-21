using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Lissandra_the_Ice_Goddess.Utility
{
    internal class DamageIndicator
    {
        private const int BarWidth = 104;
        private const int LineThickness = 9;
        private static LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit;
        private static readonly Vector2 BarOffset = new Vector2(10, 25);
        private static Color drawingColor;

        public static Color DrawingColor
        {
            get { return drawingColor; }
            set { drawingColor = Color.FromArgb(170, value); }
        }

        public static bool Enabled { get; set; }

        public static void Initialize(LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnitDelegate)
        {
            // Apply needed field delegate for damage calculation
            damageToUnit = damageToUnitDelegate;
            DrawingColor = Color.GreenYellow;
            Enabled = true;
            // Register event handlers
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Enabled)
            {
                foreach (
                    var unit in ObjectManager.Get<AIHeroClient>().Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    // Get damage to unit
                    var damage = damageToUnit(unit);
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