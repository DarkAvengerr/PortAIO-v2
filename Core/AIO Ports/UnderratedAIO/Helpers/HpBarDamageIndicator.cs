using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace UnderratedAIO.Helpers
{
    public static class HpBarDamageIndicator
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        public static Color Color = Color.FromArgb(190, 255, 245, 142);
        public static bool Enabled = true;
        private static DamageToUnitDelegate _damageToUnit;

        private static readonly Render.Text Text = new Render.Text(
            0, 0, string.Empty, 11, new ColorBGRA(255, 0, 0, 255), "monospace");

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }
                _damageToUnit = value;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
            {
                return;
            }

            var width = Drawing.Width;
            var height = Drawing.Height;

            foreach (var unit in
                HeroManager.Enemies.FindAll(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentDamage = damage / unit.MaxHealth;
                var healtpercentage = unit.Health / unit.MaxHealth;
                var barWidth = Width * percentDamage;
                var barOffset = Width * healtpercentage;
                var YOffset2 = YOffset;
                var XOffset2 = XOffset;
                if (unit.ChampionName == "Annie")
                {
                    YOffset2 -= 17;
                    XOffset2 -= 9;
                }

                if (barOffset - barWidth < 0)
                {
                    barWidth = barWidth + barOffset - barWidth;
                }
                var xPos = barPos.X + XOffset2 + barOffset;

                //if (damage > unit.Health)
                {
                    Text.X = (int) barPos.X + XOffset2;
                    Text.Y = (int) barPos.Y + YOffset2 - 13;
                    Text.text = ((int) (unit.Health - damage)).ToString();
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPos, barPos.Y + YOffset2, xPos, barPos.Y + YOffset2 + Height, barWidth, Color);
            }
        }
    }
}