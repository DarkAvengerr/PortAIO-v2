using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using hikiMarksmanRework.Core.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Drawings
{
    class GravesDrawing
    {
        public static void Init()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (GravesMenu.Config.Item("graves.q.draw").GetValue<Circle>().Active && GravesSpells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GravesSpells.Q.Range, GravesMenu.Config.Item("graves.q.draw").GetValue<Circle>().Color);
            }
            if (GravesMenu.Config.Item("graves.w.draw").GetValue<Circle>().Active && GravesSpells.W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GravesSpells.W.Range, GravesMenu.Config.Item("graves.w.draw").GetValue<Circle>().Color);
            }
            if (GravesMenu.Config.Item("graves.e.draw").GetValue<Circle>().Active && GravesSpells.E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GravesSpells.E.Range, GravesMenu.Config.Item("graves.e.draw").GetValue<Circle>().Color);
            }
            if (GravesMenu.Config.Item("graves.r.draw").GetValue<Circle>().Active && GravesSpells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GravesSpells.R.Range, GravesMenu.Config.Item("graves.r.draw").GetValue<Circle>().Color);
            }
            if (!Helper.Enabled("graves.disable.catch"))
            {
                var selectedtarget = TargetSelector.GetSelectedTarget();
                if (selectedtarget != null)
                {
                    var playerposition = Drawing.WorldToScreen(ObjectManager.Player.Position);
                    var enemyposition = Drawing.WorldToScreen(selectedtarget.Position);
                    if (Helper.Enabled("graves.catch.line"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.LawnGreen);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.Red);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.Orange);
                        }
                    }
                    if (Helper.Enabled("graves.catch.circle"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.LawnGreen);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.Red);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.Orange);
                        }
                    }
                    if (Helper.Enabled("graves.catch.text"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.LawnGreen, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (GravesSpells.E.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.LawnGreen, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E));
                            }
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Red, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (GravesSpells.E.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Red, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E));
                            }
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Orange, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (GravesSpells.E.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Orange, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, GravesSpells.E));
                            }
                        }
                    }
                }
            }
        }
    }
}
