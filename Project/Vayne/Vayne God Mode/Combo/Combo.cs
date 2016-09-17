using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GodModeOn_Vayne.Combo
{
    static class Combo
    {
        public static void Do()
            {
            // uso de la q
         //       Chat.Print("no llego lol llego?");
                var Qcombo = Program.menu.Item("QC").GetValue<bool>();
                var Ecombo = Program.menu.Item("EC").GetValue<bool>();
                var Etarcombo = Program.menu.Item("ECT").GetValue<bool>();
                var Rcombo = Program.menu.Item("RC").GetValue<bool>();
               var Rmincombo = Program.menu.Item("CNr").GetValue<Slider>().Value;
           //     var Etarcombo = Program.menu.Item("RC").GetValue<bool>();
       //     TargetSelector.
                var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
            if(Qcombo)
            {
                if (target != null)
                {
                    Program.Q.Cast(Game.CursorPos, false);
                }
            }
            if (Ecombo)
            {
                if (target != null)
                {
                    if (!Etarcombo)
                    {
                        if (SergixCondemn())
                            Program.E.Cast(Program.targetE());
                    }
                    else
                    {
                        if (target == TargetSelector.GetSelectedTarget())
                        {
                            if (SergixCondemn())
                                Program.E.Cast(Program.targetE());
                        }
                    }
                }
            }
         if(Rcombo)
           {
              if (WillHitEnemys(Program.Player,800,Rmincombo))
              {
                  Program.R.Cast();
              }
           }
            //end uso de la q
            }
        public static float CondemnRange = 550f;
        public static float CondemnKnockback = 490f;
        private static bool TreesCondemn(Vector3 position,Obj_AI_Base Hero)
        {
            var pointList = new List<Vector3>();

            for (var j = CondemnKnockback; j >= 50; j -= 100)
            {
                var offset = (int)(2 * Math.PI * j / 100);

                for (var i = 0; i <= offset; i++)
                {
                    var angle = i * Math.PI * 2 / offset;
                    var point =
                        new Vector2(
                            (float)(position.X + j * Math.Cos(angle)),
                            (float)(position.Y - j * Math.Sin(angle))).To3D();

                    if (point.IsWall())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool SergixCondemn()
        {
            var pointwaswall = false;
            var d = Program.targetE().Position.Distance(Program.Efinishpos(Program.targetE()));
            for (var i = 0; i < d; i += 10)
            {
                var dist = i > d ? d : i;
                var point = Program.targetE().Position.Extend(Program.Efinishpos(Program.targetE()), dist);
                if (pointwaswall)
                {
                    if (point.IsWall())
                    {
                        Render.Circle.DrawCircle(point, 3, System.Drawing.Color.Red);
                        return true;
                    }
                    else
                    {

                        Render.Circle.DrawCircle(point, 1, System.Drawing.Color.Yellow);
                    }
                }
                if (point.IsWall())
                {
                    pointwaswall = true;
                }
            }
            return false;
        }
        public static bool WillHitEnemys(Obj_AI_Base zone, int Range, int min)
        {
            int i = 0;
            int mine = 0;
            foreach (AIHeroClient b in ObjectManager.Get<AIHeroClient>())
            {
                if (b.IsEnemy && !b.IsDead && b.Distance(zone) < Range)
                {
                    i++;
                }
            }
            mine = i;
            if (i >= min)
                return true;
            else
                return false;
        }

    }
}
