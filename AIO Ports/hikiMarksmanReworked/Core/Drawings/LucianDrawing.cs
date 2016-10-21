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
    class LucianDrawing
    {
        public static void Init()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (LucianMenu.Config.Item("lucian.q.draw").GetValue<Circle>().Active && LucianSpells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.Q.Range, LucianMenu.Config.Item("lucian.q.draw").GetValue<Circle>().Color);
            }
            if (LucianMenu.Config.Item("lucian.q2.draw").GetValue<Circle>().Active && LucianSpells.Q2.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.Q2.Range, LucianMenu.Config.Item("lucian.q2.draw").GetValue<Circle>().Color);
            }
            if (LucianMenu.Config.Item("lucian.w.draw").GetValue<Circle>().Active && LucianSpells.W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.W.Range, LucianMenu.Config.Item("lucian.w.draw").GetValue<Circle>().Color);
            }
            if (LucianMenu.Config.Item("lucian.e.draw").GetValue<Circle>().Active && LucianSpells.E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.E.Range, LucianMenu.Config.Item("lucian.e.draw").GetValue<Circle>().Color);
            }
            if (LucianMenu.Config.Item("lucian.r.draw").GetValue<Circle>().Active && LucianSpells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.R.Range, LucianMenu.Config.Item("lucian.r.draw").GetValue<Circle>().Color);
            }
            if (!Helper.LEnabled("lucian.disable.catch"))
            {
                var selectedtarget = TargetSelector.GetSelectedTarget();
                if (selectedtarget != null)
                {
                    var playerposition = Drawing.WorldToScreen(ObjectManager.Player.Position);
                    var enemyposition = Drawing.WorldToScreen(selectedtarget.Position);
                    if (Helper.LEnabled("lucian.catch.line"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.LawnGreen);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.Red);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawLine(playerposition, enemyposition, 2, Color.Orange);
                        }
                    }
                    if (Helper.LEnabled("lucian.catch.circle"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.LawnGreen);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.Red);
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Render.Circle.DrawCircle(selectedtarget.Position, 100, Color.Orange);
                        }
                    }
                    if (Helper.LEnabled("lucian.catch.text"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.LawnGreen, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (LucianSpells.E.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.LawnGreen, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E));
                            }
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Red, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (LucianSpells.E.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Red, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E));
                            }
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Orange, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            if (LucianSpells.E.IsReady())
                            {
                                Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Orange, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, LucianSpells.E));
                            }
                        }
                    }
                }
            }
        }
    }
}
