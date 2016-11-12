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
 namespace adcUtility.Turret
{
    public static class Enemy_Turret
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiEnemyTurret { get; set; }

        public static Obj_AI_Base EnemyTurret
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
        static Enemy_Turret()
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
            var enemyTurret = Program.Config.Item("enemy.turret.set").GetValue<Circle>();
            var enemyTurretColor = Program.Config.Item("enemy.turret.set").GetValue<Circle>().Color;
            var enemyturretRangeDistance = Program.Config.Item("enemy.turret.distance").GetValue<Slider>().Value;
            var enemyturretThickness = Program.Config.Item("enemy.turret.thickness").GetValue<Slider>().Value;

            if (enemyTurret.Active)
            {
                foreach (Obj_AI_Turret turret in ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.IsVisible && !turret.IsDead && turret.IsValid && turret.IsEnemy))
                {
                    if (turret.Distance(ObjectManager.Player.Position) < enemyturretRangeDistance)
                    {
                        var turretposition = Drawing.WorldToScreen(turret.Position);
                        Render.Circle.DrawCircle(new Vector3(turret.Position.X, turret.Position.Y, turret.Position.Z), 900, enemyTurretColor, enemyturretThickness, false);
                    }
                }
            }
        }

    }
}