// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The damage bar.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce.Extensions
{
    // Credits: Nechrito
    #region

    using System;

    using LeagueSharp;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;

    #endregion

    /// <summary>
    ///     The damage bar.
    /// </summary>
    internal class DamageBar
    {
        #region Static Fields

        /// <summary>
        ///     The dx device.
        /// </summary>
        public static Device DxDevice = Drawing.Direct3DDevice;

        /// <summary>
        ///     The dx line.
        /// </summary>
        public static Line DxLine;

        #endregion

        #region Fields

        /// <summary>
        ///     The hight.
        /// </summary>
        public float Hight = 9;

        /// <summary>
        ///     The width.
        /// </summary>
        public float Width = 104;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DamageBar" /> class.
        /// </summary>
        public DamageBar()
        {
            DxLine = new Line(DxDevice) { Width = 9 };

            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The start position.
        /// </summary>
        public Vector2 StartPosition
            => new Vector2(this.Unit.HPBarPosition.X + this.Offset.X, this.Unit.HPBarPosition.Y + this.Offset.Y);

        /// <summary>
        ///     Gets or sets the unit.
        /// </summary>
        public AIHeroClient Unit { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the offset.
        /// </summary>
        private Vector2 Offset
        {
            get
            {
                if (this.Unit != null) return this.Unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);

                return new Vector2();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The draw dmg.
        /// </summary>
        /// <param name="dmg">
        /// The dmg.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public void DrawDmg(float dmg, ColorBGRA color)
        {
            var hpPosNow = this.GetHpPosAfterDmg(0);
            var hpPosAfter = this.GetHpPosAfterDmg(dmg);

            this.FillHpBar(hpPosNow, hpPosAfter, color);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The current domain on domain unload.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            DxLine.Dispose();
        }

        /// <summary>
        /// The drawing on on post reset.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void DrawingOnOnPostReset(EventArgs args)
        {
            DxLine.OnResetDevice();
        }

        /// <summary>
        /// The drawing on on pre reset.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void DrawingOnOnPreReset(EventArgs args)
        {
            DxLine.OnLostDevice();
        }

        /// <summary>
        /// The fill hp bar.
        /// </summary>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        private void FillHpBar(int to, int from, Color color)
        {
            var sPos = this.StartPosition;
            for (var i = from; i < to; i++) Drawing.DrawLine(sPos.X + i, sPos.Y, sPos.X + i, sPos.Y + 9, 1, color);
        }

        /// <summary>
        /// The fill hp bar.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        private void FillHpBar(Vector2 from, Vector2 to, ColorBGRA color)
        {
            DxLine.Begin();

            DxLine.Draw(
                new[] { new Vector2((int)from.X, (int)from.Y + 4f), new Vector2((int)to.X, (int)to.Y + 4f) },
                color);

            DxLine.End();
        }

        /// <summary>
        /// The get hp pos after dmg.
        /// </summary>
        /// <param name="dmg">
        /// The dmg.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/>.
        /// </returns>
        private Vector2 GetHpPosAfterDmg(float dmg)
        {
            var w = this.GetHpProc(dmg) * this.Width;
            return new Vector2(this.StartPosition.X + w, this.StartPosition.Y);
        }

        /// <summary>
        /// The get hp proc.
        /// </summary>
        /// <param name="dmg">
        /// The dmg.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        private float GetHpProc(float dmg)
        {
            var health = this.Unit.Health - dmg > 0 ? this.Unit.Health - dmg : 0;
            return health / this.Unit.MaxHealth;
        }

        #endregion
    }
}