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
    class VayneDrawing
    {
        public static void Init()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (VayneMenu.Config.Item("vayne.q.draw").GetValue<Circle>().Active && VayneSpells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, VayneSpells.Q.Range, VayneMenu.Config.Item("vayne.q.draw").GetValue<Circle>().Color);
            }
            if (VayneMenu.Config.Item("vayne.e.draw").GetValue<Circle>().Active && VayneSpells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, VayneSpells.E.Range, VayneMenu.Config.Item("vayne.e.draw").GetValue<Circle>().Color);
            }
            if (!Helper.VEnabled("vayne.disable.catch"))
            {
                var selectedtarget = TargetSelector.GetSelectedTarget();
                if (selectedtarget != null)
                {
                    var playerposition = Drawing.WorldToScreen(ObjectManager.Player.Position);
                    var enemyposition = Drawing.WorldToScreen(selectedtarget.Position);
                    if (Helper.VEnabled("vayne.catch.line"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.LawnGreen);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.Red);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.Orange);
                        }
                    }
                    if (Helper.VEnabled("vayne.catch.circle"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.LawnGreen);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.Red);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.Orange);
                        }
                    }
                    if (Helper.VEnabled("vayne.catch.text"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.LawnGreen, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (VayneSpells.Q.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.LawnGreen, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q));
                            }
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Red, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (VayneSpells.Q.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Red, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q));
                            }
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Orange, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (VayneSpells.Q.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Orange, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, VayneSpells.Q));
                            }
                        }
                    }
                }
            }

        }
    }
}
