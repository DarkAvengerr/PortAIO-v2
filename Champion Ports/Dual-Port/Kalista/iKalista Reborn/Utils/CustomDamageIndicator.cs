using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iKalistaReborn.Utils
{
    using System;
    using System.Linq;

    using iKalistaReborn;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    class CustomDamageIndicator
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int XOffset = 10;

        private const int YOffset = 20;

        private const int Width = 103;

        private const int Height = 8;

        private static DamageToUnitDelegate damageToUnit;

        private static readonly Render.Rectangle DamageBar = new Render.Rectangle(0, 0, 1, 8, Color.White);

        public static bool Enabled { get; set; }

        public static System.Drawing.Color DrawingColor
            => Kalista.Menu.Item("com.ikalista.drawing.eDamage").GetValue<Circle>().Color;

        public static void Initialize(DamageToUnitDelegate dmg)
        {
            damageToUnit = dmg;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled)
            {
                return;
            }

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;

                var champOffset = unit.ChampionName == "Jhin" ? new Vector2(-8, -14) : Vector2.Zero;
                var xPos = barPos.X + XOffset + champOffset.X;
                var xPosDamage = xPos + Width * percentHealthAfterDamage;
                var xPosCurrentHp = xPos + Width * unit.Health / unit.MaxHealth;
                var yPos = barPos.Y + YOffset + champOffset.Y;

                var differenceInHp = xPosCurrentHp - xPosDamage;
                DamageBar.Color = DrawingColor.ToSharpDxColor();
                DamageBar.X = (int)(barPos.X + 9 + 107 * percentHealthAfterDamage);
                DamageBar.Y = (int)yPos - 1;
                DamageBar.Width = (int)Math.Round(differenceInHp);
                DamageBar.Height = Height + 3;
                DamageBar.OnEndScene();
            }
        }
    }
}