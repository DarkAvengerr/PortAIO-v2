// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawDamage.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Utility
{
    internal class DrawDamage
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int Height = 8;

        private const int Width = 103;

        private const int XOffset = 10;

        private const int YOffset = 20;

        public static Func<Color> Color = () => System.Drawing.Color.Lime;

        public static Func<bool> Enabled = () => true;

        public static Func<bool> Fill = () => true;

        public static Func<Color> FillColor = () => System.Drawing.Color.Goldenrod;

        private static readonly Render.Text Text = new Render.Text(
            0,
            0,
            string.Empty,
            14,
            SharpDX.Color.Red,
            "monospace");

        private static DamageToUnitDelegate _damageToUnit;

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                    Drawing.OnDraw += Drawing_OnDraw;

                _damageToUnit = value;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled() || (_damageToUnit == null))
                return;

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValidTarget() && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage)/unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width*percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width*unit.Health/unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int) barPos.X + XOffset;
                    Text.Y = (int) barPos.Y + YOffset - 13;
                    Text.text = "Killable: " + (unit.Health - damage);
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, Color());

                if (Fill())
                {
                    var differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + 107*percentHealthAfterDamage;

                    for (var i = 0; i < differenceInHP; i++)
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor());
                }
            }
        }
    }
}