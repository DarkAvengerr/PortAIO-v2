using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Herrari_488_GTB
{
    class DamageIndicator
    {
        internal delegate float DamageToUnitDelegate(AIHeroClient hero);

        const int XOffset = 10;
        const int YOffset = 20;
        const int Width = 103;
        const int Height = 8;

        internal static Color Color = Color.Lime;
        internal static Color FillColor = Color.Goldenrod;

        internal static bool Fill = true;
        internal static bool Enabled = true;

        static readonly Render.Text Text = new Render.Text(0, 0, string.Empty, 14, SharpDX.Color.Red, "monospace");

        static DamageToUnitDelegate _damageToUnit;

        internal static DamageToUnitDelegate DamageToUnit
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

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
                return;

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
                    float differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                    for (int i = 0; i < differenceInHP; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
        }
    }
}
