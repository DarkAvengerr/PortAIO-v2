using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace JayceSharpV2
{
    class SmoothMouse
    {

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        public static bool doMouse = true;

        /// <summary>
        /// Not sure if we're just supposed to create our own point class.
        /// </summary>
        struct Point
        {
            public int x;
            public int y;
        }

        public struct MouseAction
        {
            public bool click;
            public Vector3 pos;

            public MouseAction(Vector3 p, bool c)
            {
                click = c;
                pos = p;
            }
        }

        public static Queue<MouseAction> queuePos = new Queue<MouseAction>();

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public static Vector2 difOfMouse;

        private static int lastAdd = Environment.TickCount;


        public static void doMouseClick()
        {
            Point pos = new Point();
            GetCursorPos(ref pos);
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, pos.x, pos.y, 0, 0);
        }

        public static void doMouseClick(long x, long y)
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
        }

        public static Vector2 getMousePos()
        {
            Point posScreen = new Point();
            if (ScreenToClient(Process.GetCurrentProcess().MainWindowHandle, ref posScreen))
            {
                Point pos = new Point();
                if (GetCursorPos(ref pos))
                {
                    var vec = new Vector2(pos.x + posScreen.x, pos.y + posScreen.y);
                    difOfMouse = vec - Drawing.WorldToScreen(Game.CursorPos);
                    return vec;
                }
            }
            return new Vector2();
        }

        public static void updateDif()
        {
            Point pos = new Point();
            if (GetCursorPos(ref pos))
            {
                var vec = new Vector2(pos.x, pos.y);
                difOfMouse = vec - Drawing.WorldToScreen(Game.CursorPos);
            }
        }

        public static void MoveMouse(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void MoveMouse(Vector2 pos)
        {
            Point posScreen = new Point();
            if (ScreenToClient(Process.GetCurrentProcess().MainWindowHandle, ref posScreen))
            {
                SetCursorPos((int)(pos.X - posScreen.x), (int)(pos.Y - posScreen.y));
            }
        }

        public static void addMouseEvent(Vector3 pos, bool click = false)
        {
            if (lastAdd + 250 < Environment.TickCount)
            {
                lastAdd = Environment.TickCount;
                queuePos.Enqueue(new MouseAction(pos, click));
            }
        }


        public static void start()
        {
            new Thread(() =>
            {
                while (doMouse)
                {
                    if (queuePos.Count > 0)
                    {
                        var posNow = getMousePos();
                        var first = queuePos.First();
                        if (!first.pos.IsOnScreen())
                        {
                            queuePos.Dequeue();
                        }
                        else
                        {
                            var firstOnScreen = first.pos.toScreen();
                            if (firstOnScreen.Distance(posNow, true) <= 55 * 55)
                            {
                                if (first.click)
                                    doMouseClick();
                                queuePos.Dequeue();
                            }
                            else
                            {
                                updateDif();
                                var moveTo = posNow.Extend(firstOnScreen, 55);
                                MoveMouse(moveTo);
                            }
                        }
                    }
                    Thread.Sleep(1000 / 60);
                }
            }).Start();
        }
    }


    public static class easierShit
    {
        public static Vector2 toScreen(this Vector3 pos)
        {
            return Drawing.WorldToScreen(pos);
        }
    }
}
