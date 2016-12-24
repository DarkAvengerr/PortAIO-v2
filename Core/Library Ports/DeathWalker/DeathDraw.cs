using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;
using EloBuddy;

namespace DetuksSharp
{
    class DeathDraw
    {


        public static SharpDX.Direct3D9.Device dxDevice = Drawing.Direct3DDevice;
        public static SharpDX.Direct3D9.Line dxLine;
        public static SharpDX.Direct3D9.Sprite sprite;


        public float width = 104;

        public float hight = 9;

        public int windowsH = 0;
        public int windowsW = 0;

        public DeathDraw()
        {
            dxLine = new Line(dxDevice) { Width = 9 };
            sprite = new Sprite(dxDevice);

            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
            windowsH = dxDevice.Viewport.Height;
            windowsW = dxDevice.Viewport.Width;
            //Console.WriteLine("Xtest: " + dxDevice.Viewport.Width + " : " + dxDevice.Viewport.Height);

        }


        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            sprite.Dispose();
            dxLine.Dispose();
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            dxLine.OnResetDevice();
            sprite.OnResetDevice();
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            sprite.OnLostDevice();
            dxLine.OnLostDevice();
        }


        public Vector2 sPos
        {

            get { return new Vector2(windowsW / 2 - windowsW / 10, windowsH - windowsH/5); }
        }

        public Vector2 mPos
        {

            get { return new Vector2(windowsW / 2 -  10, windowsH - 5); }
        }





        public void draw(int percent)
        {
            //if ((DeathWalker.menu.Item("nobar").GetValue<bool>()))
            //    return;
            //dxLine.Begin();
            //var tPos = sPos + new Vector2(((windowsW * percent) / (500)), 0);
            //var tPosFull = sPos + new Vector2(windowsW/5-5, 0);
            //dxLine.Draw(new[]
            //                        {
            //                            new Vector2((int)sPos.X, (int)sPos.Y + 6f),
            //                            new Vector2( (int)tPosFull.X, (int)tPosFull.Y + 6f)
            //                        }, new ColorBGRA(200, 0, 0, 220));
            //dxLine.Draw(new[]
            //                        {
            //                            new Vector2((int)sPos.X, (int)sPos.Y + 6f),
            //                            new Vector2( (int)tPos.X, (int)tPos.Y + 6f)
            //                        }, new ColorBGRA(0, 200, 0, 220));
            
            //// Vector2 sPos = startPosition;
            ////Drawing.DrawLine((int)from.X, (int)from.Y + 9f, (int)to.X, (int)to.Y + 9f, 9f, color);

            //dxLine.End();
        }

    }
}
