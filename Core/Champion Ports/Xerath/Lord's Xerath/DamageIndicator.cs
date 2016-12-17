using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using EloBuddy; 
using LeagueSharp.Common; 
namespace Lords_Xerath
{
    class DamageIndicator
    {
        private const int BAR_WIDTH = 104;
        private const int LINE_THICKNESS = 9;

        private static LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit;

        private static readonly Vector2 BarOffset = new Vector2(1.25f, 14.25f);
        private static readonly Vector2 PercentOffset = new Vector2(0, -15);

        private static System.Drawing.Color _drawingColor;
        public static System.Drawing.Color DrawingColor
        {
            get { return _drawingColor; }
            set { _drawingColor = Color.FromArgb(170, value); }
        }

        //private static Line OverlayLine { get; set; }
        public static bool Enabled { get; set; }
        public static bool PercentEnabled { get; private set; }

        public static void Initialize(LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit)
        {
            // Apply needed field delegate for damage calculation
            DamageIndicator.damageToUnit = damageToUnit;
            DrawingColor = System.Drawing.Color.Green;
            Enabled = true;



            // Register event handlers
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Enabled)
            {
                foreach (var unit in HeroManager.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    // Get damage to unit
                    var damage = damageToUnit(unit);

                    // Continue on 0 damage
                    if (damage <= 0)
                        continue;

                    // Get remaining HP after damage applied in percent and the current percent of health
                    var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                    var currentHealthPercentage = unit.Health / unit.MaxHealth;

                    // Calculate start and end point of the bar indicator
                    var startPoint = new Vector2(unit.HPBarPosition.X + BarOffset.X + damagePercentage * BAR_WIDTH, unit.HPBarPosition.Y + BarOffset.Y - 5);
                    var endPoint = new Vector2(unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BAR_WIDTH + 1, unit.HPBarPosition.Y + BarOffset.Y - 5);

                    // Draw the line                    
                    Drawing.DrawLine(startPoint, endPoint, LINE_THICKNESS, DrawingColor);


                }
            }
        }
    }
}
   


