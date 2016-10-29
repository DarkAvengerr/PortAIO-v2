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
 namespace adcUtility.Range
{
    public static class Enemy_AA
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiEnemyAA { get; set; }

        public static Obj_AI_Base KalistaAllyAA
        {
            get
            {
                if (adCarry != null && adCarry.IsValid)
                {
                    return adCarry;
                }
                return null;
            }
        }
        static Enemy_AA()
        {
            Drawing.OnDraw += Drawing_OnDraw;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsEnemy);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var enemyRange = Program.Config.Item("enemy.range").GetValue<Circle>();
            var enemyRangeColor = Program.Config.Item("enemy.range").GetValue<Circle>().Color;
            var rangeDistance = Program.Config.Item("enemy.distance").GetValue<Slider>().Value;
            var thickness = Program.Config.Item("enemy.thickness").GetValue<Slider>().Value;

            if (enemyRange.Active)
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsEnemy && !o.IsDead && !o.IsZombie))
                {
                    if (enemy.Distance(ObjectManager.Player.Position) < rangeDistance)
                    {
                        var allyposition = Drawing.WorldToScreen(enemy.Position);
                        Render.Circle.DrawCircle(new Vector3(enemy.Position.X, enemy.Position.Y, enemy.Position.Z), enemy.AttackRange, enemyRangeColor, thickness, true);
                    }
                }
            }
        }

    }
}