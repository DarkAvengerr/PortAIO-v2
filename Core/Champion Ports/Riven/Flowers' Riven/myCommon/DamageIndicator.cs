using EloBuddy; 
using LeagueSharp.Common; 
namespace FlowersRivenCommon
{
    using System;
    using System.Linq;
    using System.Globalization;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;

    public static class DamageIndicator // Credit By Detuks
    {
        public static bool Fill = true;
        public static bool Enabled = true;
        public static Color FillColor = Color.Goldenrod;
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Color Color = Color.Lime;
        private static DamageToUnitDelegate _damageToUnit;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 11, new ColorBGRA(255, 0, 0, 255), "monospace");

        public static void AddToMenu(Menu mainMenu, DamageToUnitDelegate Damage = null)
        {
            mainMenu.AddItem(new MenuItem("DrawComboDamage", "Draw Combo Damage", true).SetValue(true));
            mainMenu.AddItem(
                new MenuItem("DrawFillDamage", "Draw Fill Combo Damage", true).SetValue(new Circle(true,
                    Color.FromArgb(255, 255, 169, 4))));

            DamageToUnit = Damage ?? DamageCalculate.GetComboDamage;
            Enabled = mainMenu.Item("DrawComboDamage", true).GetValue<bool>();
            Fill = mainMenu.Item("DrawFillDamage", true).GetValue<Circle>().Active;
            FillColor = mainMenu.Item("DrawFillDamage", true).GetValue<Circle>().Color;

            mainMenu.Item("DrawComboDamage", true).ValueChanged += delegate(object obj, OnValueChangeEventArgs Args)
            {
                Enabled = Args.GetNewValue<bool>();
            };

            mainMenu.Item("DrawFillDamage", true).ValueChanged += delegate (object obj, OnValueChangeEventArgs Args)
            {
                Fill = Args.GetNewValue<Circle>().Active;
                FillColor = Args.GetNewValue<Circle>().Color;
            };
        }

        public static DamageToUnitDelegate DamageToUnit
        {
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

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage)/unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width*percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width*unit.Health/unit.MaxHealth;

                if (unit.Health > 0)
                {
                    Text.X = (int) barPos.X + XOffset;
                    Text.Y = (int) barPos.Y + YOffset - 13;
                    Text.text = ((int) (unit.Health - damage)).ToString(CultureInfo.InvariantCulture);
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 2, Color);

                if (Fill)
                {
                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + 107*percentHealthAfterDamage;

                    for (var i = 0; i < differenceInHp; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
        }
    }
}
