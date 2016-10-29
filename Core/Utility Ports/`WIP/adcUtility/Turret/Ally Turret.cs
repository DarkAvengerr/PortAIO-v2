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
    public static class Ally_Turret
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiAllyTurret { get; set; }

        public static Obj_AI_Base AllyTurret
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
        static Ally_Turret()
        {
            Drawing.OnDraw += Drawing_OnDraw;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsAlly);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var allyTurret = Program.Config.Item("ally.turret").GetValue<Circle>();
            var allyTurretColor = Program.Config.Item("ally.turret").GetValue<Circle>().Color;
            var turretRangeDistance = Program.Config.Item("ally.turret.distance").GetValue<Slider>().Value;
            var turretThickness = Program.Config.Item("ally.turret.thickness").GetValue<Slider>().Value;

            if (allyTurret.Active)
            {
                foreach (Obj_AI_Turret turret in ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.IsVisible && !turret.IsDead && turret.IsValid && turret.IsAlly))
                {
                    if (turret.Distance(ObjectManager.Player.Position) < turretRangeDistance)
                    {
                        var turretposition = Drawing.WorldToScreen(turret.Position);
                        
                        Render.Circle.DrawCircle(new Vector3(turret.Position.X, turret.Position.Y, turret.Position.Z), 900, allyTurretColor, turretThickness,false);
                    }
                }
            }
        }

    }
}