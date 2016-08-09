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
    class CorkiDrawing
    {
        public static void Init()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (CorkiMenu.Config.Item("corki.q.draw").GetValue<Circle>().Active && CorkiSpells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, CorkiSpells.Q.Range, CorkiMenu.Config.Item("corki.q.draw").GetValue<Circle>().Color);
            }
            if (CorkiMenu.Config.Item("corki.w.draw").GetValue<Circle>().Active && CorkiSpells.W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, CorkiSpells.W.Range, CorkiMenu.Config.Item("corki.w.draw").GetValue<Circle>().Color);
            }
            if (CorkiMenu.Config.Item("corki.e.draw").GetValue<Circle>().Active && CorkiSpells.E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, CorkiSpells.E.Range, CorkiMenu.Config.Item("corki.e.draw").GetValue<Circle>().Color);
            }
            if (CorkiMenu.Config.Item("corki.r.draw").GetValue<Circle>().Active && CorkiSpells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, CorkiSpells.R.Range, CorkiMenu.Config.Item("corki.r.draw").GetValue<Circle>().Color);
            }
            if (!Helper.CEnabled("corki.disable.catch"))
            {
                var selectedtarget = TargetSelector.GetSelectedTarget();
                if (selectedtarget != null)
                {
                    var playerposition = Drawing.WorldToScreen(ObjectManager.Player.Position);
                    var enemyposition = Drawing.WorldToScreen(selectedtarget.Position);
                    if (Helper.CEnabled("corki.catch.line"))
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
                    if (Helper.CEnabled("corki.catch.circle"))
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
                    if (Helper.CEnabled("corki.catch.text"))
                    {
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) < 0 && Catcher.Calculate(selectedtarget) < 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.LawnGreen, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.LawnGreen, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E));
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) > 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Red, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Red, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E));
                        }
                        if (Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E) < 0 && Catcher.Calculate(selectedtarget) > 0)
                        {
                            Drawing.DrawText(playerposition.X, playerposition.Y, Color.Orange, "Catch (Time): " + (int)Catcher.Calculate(selectedtarget));
                            Drawing.DrawText(playerposition.X - 20, playerposition.Y - 20, Color.Orange, "Catch With Gapclose (Time): " + (int)Catcher.GapcloseCalculte(selectedtarget, EzrealSpells.E));
                        }
                    }
                }
            }
        }
    }
}
