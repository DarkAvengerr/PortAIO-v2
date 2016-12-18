using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Helpers
{
    internal class FpsBalancer
    {
        public static float lastTick, fps, delay, lastTickdelay, autoFpsBalancer, frameRate;
        public static Menu CommonMenu;

        private static void ShowInfo()
        {
            Console.WriteLine("LastTick: " + (System.Environment.TickCount - lastTick));
            Console.WriteLine("fps: " + fps);
        }

        public static bool CheckCounter()
        {
            return false;
        }

        public static Menu AddToMenu(Menu menu)
        {
            Menu menuFps = new Menu("FPS Balancer", "fpssettings");
            menuFps.AddItem(new MenuItem("autoFpsBalancer", "Min FPS"))
                .SetTooltip("Reduces script calculations if your FPS is lower than this.")
                .SetValue(new Slider(45, 1, 350));
            menuFps.AddItem(new MenuItem("requeredRefresh", "Calculations per second"))
                .SetTooltip("Script calculations per second, L# default is your FPS")
                .SetValue(new Slider(35, 1, 350));
            menuFps.AddItem(new MenuItem("EnabledFps", "Enable"))
                .SetTooltip("Enable update limiter in THIS assembly")
                .SetValue(false);
            menu.AddSubMenu(menuFps);
            CommonMenu = menu;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            return menu;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            //Drawing.DrawText(Drawing.Width * 0.9f, Drawing.Height * 0.1f, Color.PaleGreen, ((int) fps).ToString());
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                fps = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            delay += System.Environment.TickCount - lastTickdelay;
            //ShowInfo();
            lastTickdelay = System.Environment.TickCount;
        }
    }
}