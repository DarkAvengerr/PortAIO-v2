using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Handlers;
using Color = System.Drawing.Color;

namespace S_Plus_Class_Kalista.Drawing
{
    internal class DrawingOnChamps : Core
    {
        private const string _MenuNameBase = ".Champions Menu";
        private const string _MenuItemBase = ".Champions.";

        public static Menu DrawingOnChampionsMenu()
        {
            var menu = new Menu(_MenuNameBase, "enemyMenu");

            var enemyMenu = new Menu(".Enemys", "enemyMenu");
            enemyMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnEnemy", "Draw On Enemys").SetValue(true));
            enemyMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnEnemy.FillColor", "Damage Fill").SetValue(new Circle(true,
                    Color.DarkGray)));
            enemyMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnEnemy.KillableColor", "Killable Text").SetValue(
                    new Circle(true, Color.DarkGray)));

            var selfMenu = new Menu(".Self", "selfMenu");
            selfMenu.AddItem(new MenuItem(_MenuItemBase + "Boolean.DrawOnSelf", "Draw On Self").SetValue(true));
            selfMenu.AddItem(
                new MenuItem(_MenuItemBase + "Boolean.DrawOnSelf.RendColor", "Rend Range").SetValue(new Circle(true,
                    Color.DarkSlateBlue)));

            selfMenu.AddItem(
    new MenuItem(_MenuItemBase + "Boolean.DrawOnSelf.SentinelMode", "Display Sentinel Mode").SetValue(true));
            menu.AddSubMenu(enemyMenu);
            menu.AddSubMenu(selfMenu);

            return menu;
        }

        private static Utility.HpBarDamageIndicator.DamageToUnitDelegate _damageToEnemy;

        public static Utility.HpBarDamageIndicator.DamageToUnitDelegate DamageToEnemy
        {
            get { return _damageToEnemy; }

            set
            {
                if (_damageToEnemy == null)
                {
                    EloBuddy.Drawing.OnDraw += OnDrawEnemy;
                }
                _damageToEnemy = value;
            }
        }

        public static void OnDrawEnemy(EventArgs args)
        {
            if (!SMenu.Item(_MenuItemBase + "Boolean.DrawOnEnemy").GetValue<bool>() || DamageToEnemy == null)
                return;

            foreach (
                var unit in
                    HeroManager.Enemies.Where(
                        unit => unit.IsValid && unit.IsHPBarRendered && Champion.E.IsInRange(unit)))
            {
                const int xOffset = 10;
                const int yOffset = 20;
                const int width = 103;
                const int height = 8;

                var barPos = unit.HPBarPosition;
                var damage = Libaries.Damage.DamageCalc.CalculateRendDamage(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage)/unit.MaxHealth;
                var yPos = barPos.Y + yOffset;
                var xPosDamage = barPos.X + xOffset + width*percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + xOffset + width*unit.Health/unit.MaxHealth;

                if (SMenu.Item(_MenuItemBase + "Boolean.DrawOnEnemy.KillableColor").GetValue<Circle>().Active &&
                    damage > unit.Health)
                    EloBuddy.Drawing.DrawText(barPos.X + xOffset, barPos.Y + yOffset - 13,
                        SMenu.Item(_MenuItemBase + "Boolean.DrawOnEnemy.KillableColor").GetValue<Circle>().Color,
                        "Killable");

                EloBuddy.Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + height, 1, Color.LightGray);

                if (!SMenu.Item(_MenuItemBase + "Boolean.DrawOnEnemy.FillColor").GetValue<Circle>().Active) return;

                var differenceInHp = xPosCurrentHp - xPosDamage;
                var pos1 = barPos.X + 9 + (107*percentHealthAfterDamage);

                for (var i = 0; i < differenceInHp; i++)
                {
                    EloBuddy.Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1,
                        SMenu.Item(_MenuItemBase + "Boolean.DrawOnEnemy.FillColor").GetValue<Circle>().Color);
                }
            }
        }

        private static Color GetColor(bool b)
        {
            return b ? Color.White : Color.SlateGray;
        }

        public static void OnDrawSelf(EventArgs args)
        {
            if (!SMenu.Item(_MenuItemBase + "Boolean.DrawOnSelf").GetValue<bool>())
                return;

            if (!Player.Position.IsOnScreen())
                return;
  
                if (SMenu.Item(_MenuItemBase + "Boolean.DrawOnSelf.RendColor").GetValue<Circle>().Active &&
                    Champion.E.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, Champion.E.Range,
                        SMenu.Item(_MenuItemBase + "Boolean.DrawOnSelf.RendColor").GetValue<Circle>().Color, 2);

            if (SMenu.Item(_MenuItemBase + "Boolean.DrawOnSelf.SentinelMode").GetValue<bool>() && Champion.W.Level > 0)
            {
                if (SMenu.Item(SentinelHandler._MenuItemBase + "Boolean.UseSentinel").GetValue<bool>()) //enabled
                {
                    var playerPos = EloBuddy.Drawing.WorldToScreen(Player.Position);
                    var vColor = GetColor(Champion.W.LSIsReady());
                    var text = "Sentinel Mode:" + SentinelHandler.GetSentinelSelected();
                    EloBuddy.Drawing.DrawText(playerPos.X - 30 + 50,
                        playerPos.Y - 80 + 60, vColor, text);
                }
            }

        }
    }
}
