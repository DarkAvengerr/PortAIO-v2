using EloBuddy; 
using LeagueSharp.Common; 
namespace NechritoRiven.Draw
{
    #region

    using System;

    using LeagueSharp;

    using SharpDX;
    using SharpDX.Direct3D9;

    #endregion

    internal class HpBarIndicator
    {
        #region Static Fields

        public static Device DxDevice = Drawing.Direct3DDevice;

        public static Line DxLine;

        #endregion

        #region Fields

        public float Hight = 9;

        public float Width = 104;

        #endregion

        #region Constructors and Destructors

        public HpBarIndicator()
        {
            DxLine = new Line(DxDevice) { Width = 9 };

            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
        }

        #endregion

        #region Public Properties

        public Vector2 StartPosition => new Vector2(this.Unit.HPBarPosition.X + this.Offset.X, this.Unit.HPBarPosition.Y + this.Offset.Y);

        public AIHeroClient Unit { get; set; }

        #endregion

        #region Properties

        private Vector2 Offset
        {
            get
            {
                if (this.Unit != null)
                {
                    return this.Unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
                }

                return new Vector2();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void DrawDmg(float dmg, ColorBGRA color)
        {
            var hpPosNow = this.GetHpPosAfterDmg(0);
            var hpPosAfter = this.GetHpPosAfterDmg(dmg);

            FillHpBar(hpPosNow, hpPosAfter, color);

            // fillHPBar((int)(hpPosNow.X - startPosition.X), (int)(hpPosAfter.X- startPosition.X), color);
        }

        #endregion

        #region Methods

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            DxLine.Dispose();
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            DxLine.OnResetDevice();
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            DxLine.OnLostDevice();
        }

        private static void FillHpBar(Vector2 from, Vector2 to, ColorBGRA color)
        {
            DxLine.Begin();

            DxLine.Draw(
                new[] { new Vector2((int)from.X, (int)from.Y + 4f), new Vector2((int)to.X, (int)to.Y + 4f) },
                color);

            DxLine.End();
        }

        private Vector2 GetHpPosAfterDmg(float dmg)
        {
            var w = this.GetHpProc(dmg) * this.Width;
            return new Vector2(this.StartPosition.X + w, this.StartPosition.Y);
        }

        private float GetHpProc(float dmg)
        {
            var health = this.Unit.Health - dmg > 0 ? this.Unit.Health - dmg : 0;
            return health / this.Unit.MaxHealth;
        }

        #endregion
    }
}