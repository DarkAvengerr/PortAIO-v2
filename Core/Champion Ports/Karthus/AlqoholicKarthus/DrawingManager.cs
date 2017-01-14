using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicKarthus
{
    #region Using Directives

    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Menu;

    #endregion

    internal class DrawingManager
    {
        #region Constructors and Destructors
        private const int Height = 8;

        private const int Width = 103;

        private const int XOffset = 10;

        private const int YOffset = 20;

        #endregion

        #region Static Fields

        public static Color Color = Color.Lime;

        public static bool Fill = true;

        public static Color FillColor = Color.Goldenrod;

        private static DamageToUnitDelegate damageToUnit;

        public delegate float DamageToUnitDelegate(AIHeroClient hero);


        public static DamageToUnitDelegate DamageToUnit
        {
            get
            {
                return damageToUnit;
            }

            set
            {
                if (damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }
                damageToUnit = value;
            }
        }

        internal DrawingManager()
        {
            try
            {
                Drawing.OnDraw += Drawing_OnDraw;
            }
            catch (Exception e)
            {
                Console.WriteLine("@DrawingManager.cs: Cannot initiate DrawingManager - {0}", e);
                throw;
            }
        }

        #endregion

        #region Methods

        private static void Drawing_OnDraw(EventArgs args)
        {

            var drawDmg = AlqoholicMenu.MainMenu.Item("draw.damage").GetValue<bool>();

            if (Menu.AlqoholicMenu.MainMenu.Item("drawq").GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    Spells.Spells.Q.SpellObject.Range,
                    Spells.Spells.Q.SpellObject.IsReady() ? Color.LightCyan : Color.Tomato);
            }
            if (Menu.AlqoholicMenu.MainMenu.Item("draww").GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    Spells.Spells.W.SpellObject.Range,
                    Spells.Spells.W.SpellObject.IsReady() ? Color.LightCyan : Color.Tomato);
            }
            if (Menu.AlqoholicMenu.MainMenu.Item("drawe").GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    Spells.Spells.E.SpellObject.Range,
                    Spells.Spells.E.SpellObject.IsReady() ? Color.LightCyan : Color.Tomato);
            }

            if (!drawDmg && damageToUnit == null)
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValid && x.IsHPBarRendered))
            {
                var barPos = enemy.HPBarPosition;
                var damage = damageToUnit(enemy);
                var percentHealthAfterDamage = Math.Max(0, enemy.Health - damage) / enemy.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * enemy.Health / enemy.MaxHealth;

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, Color);

                if (!Fill)
                {
                    continue;
                }
                var differenceInHp = xPosCurrentHp - xPosDamage;
                var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                for (var i = 0; i < differenceInHp; i++)
                {
                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                }
            }
        }

        #endregion
    }
}