using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hikiMarksmanRework.Core.Menus;
using hikiMarksmanRework.Core.Spells;
using hikiMarksmanRework.Core.Utilitys;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Drawings
{
    class EzrealDrawing
    {
        public static void Init()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (EzrealMenu.Config.Item("ezreal.q.draw").GetValue<Circle>().Active && EzrealSpells.Q.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, EzrealSpells.Q.Range, EzrealMenu.Config.Item("ezreal.q.draw").GetValue<Circle>().Color);
            }
            if (EzrealMenu.Config.Item("ezreal.w.draw").GetValue<Circle>().Active && EzrealSpells.W.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, EzrealSpells.W.Range, EzrealMenu.Config.Item("ezreal.w.draw").GetValue<Circle>().Color);
            }
            if (EzrealMenu.Config.Item("ezreal.e.draw").GetValue<Circle>().Active && EzrealSpells.E.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, EzrealSpells.E.Range, EzrealMenu.Config.Item("ezreal.e.draw").GetValue<Circle>().Color);
            }
            if (EzrealMenu.Config.Item("ezreal.r.draw").GetValue<Circle>().Active && EzrealSpells.R.LSIsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, EzrealSpells.R.Range, EzrealMenu.Config.Item("ezreal.r.draw").GetValue<Circle>().Color);
            }
            if (!Helper.EEnabled("ezreal.disable.catch"))
            {
                var selectedtarget = TargetSelector.GetSelectedTarget();
                if (selectedtarget != null)
                {
                    var playerposition = Drawing.WorldToScreen(ObjectManager.Player.Position);
                    var enemyposition = Drawing.WorldToScreen(selectedtarget.Position);
                    if (Helper.EEnabled("ezreal.catch.line"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.LawnGreen);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.Red);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.Orange);
                        }
                    }
                    if (Helper.EEnabled("ezreal.catch.circle"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.LawnGreen);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.Red);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.Orange);
                        }
                    }
                    if (Helper.EEnabled("ezreal.catch.text"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.LawnGreen, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (EzrealSpells.E.LSIsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.LawnGreen, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E));
                            }
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Red, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (EzrealSpells.E.LSIsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Red, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E));
                            }
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Orange, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (EzrealSpells.E.LSIsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Orange, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E));
                            }
                        }
                    }
                }
            }
        }

       
    }
}
