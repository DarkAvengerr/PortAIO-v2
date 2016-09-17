using System;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using globals = KAPPALISTAXD.Core.KappalistaGlobals;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KAPPALISTAXD.Core
{
    class KappalistaDrawing
    {
        private static Color manaStatus;

        public KappalistaDrawing()
        {
            Drawing.OnDraw += OnDraw;
        }

        private void OnDraw(EventArgs args)
        {

            if ((!globals.inGameChampion.IsDead) | (!globals.inGameChampion.IsRecalling()))
            {
                if(globals.inGameChampion.ManaPercent > 60)
                {
                    manaStatus = Color.DarkBlue;
                }
                else if ((globals.inGameChampion.ManaPercent < 60) & (globals.inGameChampion.ManaPercent > 30))
                {
                    manaStatus = Color.DeepSkyBlue;
                }
                else if (globals.inGameChampion.ManaPercent < 30)
                {
                    manaStatus = Color.Gray;
                }

                if (globals.mainMenu.Item("drawing-q").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(globals.inGameChampion.Position, globals.Q.Range, manaStatus);
                }
                if (globals.mainMenu.Item("drawing-w").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(globals.inGameChampion.Position, globals.W.Range, manaStatus);
                }
                if (globals.mainMenu.Item("drawing-e").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(globals.inGameChampion.Position, globals.E.Range, manaStatus);
                }
                if (globals.mainMenu.Item("drawing-r").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(globals.inGameChampion.Position, globals.R.Range, manaStatus);
                }
            }
        }
    }
}
