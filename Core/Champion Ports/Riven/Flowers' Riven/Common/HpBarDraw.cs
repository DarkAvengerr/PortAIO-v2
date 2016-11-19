using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Riven
{
    using LeagueSharp;
    using SharpDX;
    using SharpDX.Direct3D9;
    using System;
    using Color = System.Drawing.Color;

    public class HpBarDraw
    {
        public static Device DxDevice = Drawing.Direct3DDevice;
        public static Line DxLine;
        public float Hight = 9;
        public float Width = 104;

        public AIHeroClient Unit { get; set; }

        private Vector2 Offset
        {
            get
            {
                if (Unit != null)
                {
                    return Unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
                }

                return new Vector2();
            }
        }

        public Vector2 StartPosition => new Vector2(Unit.HPBarPosition.X + Offset.X, Unit.HPBarPosition.Y + Offset.Y);

        public HpBarDraw()
        {
            DxLine = new Line(DxDevice) { Width = 9 };

            Drawing.OnPreReset += OnPreReset;
            Drawing.OnPostReset += OnPostReset;
            AppDomain.CurrentDomain.DomainUnload += Unload;
            AppDomain.CurrentDomain.ProcessExit += Unload;
        }

        private static void Unload(object sender, EventArgs eventArgs)
        {
            DxLine.Dispose();
        }

        private static void OnPostReset(EventArgs args)
        {
            DxLine.OnResetDevice();
        }

        private static void OnPreReset(EventArgs args)
        {
            DxLine.OnLostDevice();
        }


        private float GetHpProc(float dmg = 0)
        {
            var Health = Unit.Health - dmg > 0 ? Unit.Health - dmg : 0;
            return Health / Unit.MaxHealth;
        }

        private Vector2 GetHpPosAfterDmg(float dmg)
        {
            var w = GetHpProc(dmg) * Width;
            return new Vector2(StartPosition.X + w, StartPosition.Y);
        }

        public void DrawDmg(float dmg, ColorBGRA color)
        {
            var hpPosNow = GetHpPosAfterDmg(0);
            var hpPosAfter = GetHpPosAfterDmg(dmg);

            FullHPBar(hpPosNow, hpPosAfter, color);
        }

        private void FullHPBar(int to, int from, Color color)
        {
            var sPos = StartPosition;

            for (var i = from; i < to; i++)
            {
                Drawing.DrawLine(sPos.X + i, sPos.Y, sPos.X + i, sPos.Y + 9, 1, color);
            }
        }

        private void FullHPBar(Vector2 from, Vector2 to, ColorBGRA color)
        {
            DxLine.Begin();

            DxLine.Draw(new[]
            {
                new Vector2((int) from.X, (int) from.Y + 4f),
                new Vector2((int) to.X, (int) to.Y + 4f)
            }, color);

            DxLine.End();
        }
    }
}
