using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Managers
{
    #region Using Directives

    using System;
    using System.Drawing;
    using System.Linq;

    using AlqoholicLissandra.Menu;
    using AlqoholicLissandra.Spells;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class DrawingManager
    {
        #region Constants

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

        #endregion

        #region Constructors and Destructors

        internal DrawingManager()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        #endregion

        #region Delegates

        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        #endregion

        #region Public Properties

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

        #endregion

        #region Methods

        private static void Drawing_OnDraw(EventArgs args)
        {
            var q = AlqoholicMenu.MainMenu.Item("draw.q").GetValue<Circle>();
            var w = AlqoholicMenu.MainMenu.Item("draw.w").GetValue<Circle>();
            var e = AlqoholicMenu.MainMenu.Item("draw.e").GetValue<Circle>();
            var r = AlqoholicMenu.MainMenu.Item("draw.r").GetValue<Circle>();
            var drawDmg = AlqoholicMenu.MainMenu.Item("draw.damage").GetValue<bool>();

            if (q.Active && Spells.Q.SpellObject.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.Q.SpellObject.Range, q.Color);
            }

            if (w.Active && Spells.W.SpellObject.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.W.SpellObject.Range, w.Color);
            }

            if (e.Active && Spells.E.SpellObject.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.SpellObject.Range, e.Color);
            }

            if (r.Active && Spells.R.SpellObject.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.R.SpellObject.Range, r.Color);
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