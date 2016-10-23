using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Rendering;

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

        //private static readonly Render.Text Text = new Render.Text(0, 0, string.Empty, 11, new ColorBGRA(255, 0, 0, 255), "monospace");
        private static Text Text = new Text(string.Empty, new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 8, System.Drawing.FontStyle.Regular));

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnPresent += Drawing_OnDrawLine;
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

                if (barPos.X < -200 || barPos.X > width + 200)
                    continue;

                if (barPos.Y < -200 || barPos.X > height + 200)
                    continue;

                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var xPos = barPos.X + XOffset + Width * percentHealthAfterDamage;

                //if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.TextValue = ((int)(unit.Health - damage)).ToString();
                    Text.Position = new Vector2(Text.X, Text.Y);
                    Text.Draw();
                }
            }
        }
        private static void Drawing_OnDrawLine(EventArgs args)
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

                if (barPos.X < -200 || barPos.X > width + 200)
                    continue;

                if (barPos.Y < -200 || barPos.X > height + 200)
                    continue;

                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var xPos = barPos.X + XOffset + Width * percentHealthAfterDamage;

                Drawing.DrawLine(xPos, barPos.Y + YOffset, xPos, barPos.Y + YOffset + Height, 2, Color);
            }
        }
    }
}