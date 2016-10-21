using EloBuddy; namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using Font = SharpDX.Direct3D9.Font;

    internal class HealthTracker : IPlugin
    {
        #region Fields

        /// <summary>
        ///     The HP bar height
        /// </summary>
        private readonly int BarHeight = 10;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the font.
        /// </summary>
        /// <value>
        ///     The font.
        /// </value>
        private static Font Font { get; set; }

        #endregion

        /// <summary>
        ///     Gets the spacing between HUD elements
        /// </summary>
        private int HudSpacing => this.Menu.Item("HealthTracker.Spacing").GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the right offset of the HUD elements
        /// </summary>
        private int HudOffsetRight => this.Menu.Item("HealthTracker.OffsetRight").GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the top offset between the HUD elements
        /// </summary>
        private int HudOffsetTop => this.Menu.Item("HealthTracker.OffsetTop").GetValue<Slider>().Value;

        /// <summary>
        ///     Gets the right offset between text and healthbar
        /// </summary>
        private int HudOffsetText => this.Menu.Item("HealthTracker.OffsetText").GetValue<Slider>().Value;

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "Trackers");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Trackers", "Trackers"))
                           : rootMenu.Children.First(predicate);

            var enemySidebarMenu = menu.AddSubMenu(new Menu("Enemy healthbars", "healthenemies"));
            {
                enemySidebarMenu.AddItem(new MenuItem("DrawHealth_", "Activated").SetValue(true));
                enemySidebarMenu.AddItem(new MenuItem("DrawHealth_percent", "Champion health %").SetValue(true));
                enemySidebarMenu.AddItem(new MenuItem("HealthTracker.OffsetText", "Offset Text").SetValue(new Slider(30, 0, 100)));
                enemySidebarMenu.AddItem(new MenuItem("HealthTracker.OffsetTop", "Offset Top").SetValue(new Slider(75, 0, 1500)));
                enemySidebarMenu.AddItem(new MenuItem("HealthTracker.OffsetRight", "Offset Right").SetValue(new Slider(170, 0, 1500)));
                enemySidebarMenu.AddItem( new MenuItem("HealthTracker.Spacing", "Spacing").SetValue(new Slider(10, 0, 30)));
                enemySidebarMenu.AddItem(new MenuItem("FontSize", "Font size").SetValue(new Slider(15, 13, 30)));
            }

            this.Menu = menu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                    {
                        FaceName = "Tahoma", Height = this.Menu.Item("FontSize").GetValue<Slider>().Value,
                        OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased
                    });

            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Drawing.OnPreReset += args => { Font.OnLostDevice(); };
            Drawing.OnPostReset += args => { Font.OnResetDevice(); };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the scene is completely rendered.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!this.Menu.Item("DrawHealth_").GetValue<bool>() || Drawing.Direct3DDevice.IsDisposed || Font.IsDisposed)
            {
                return;
            }

            float i = 0;

            foreach (var hero in HeroManager.Enemies.Where(x => !x.IsDead && x != null))
            {
                var champion = hero.ChampionName;
                if (champion.Length > 12)
                {
                    champion = champion.Remove(7) + "...";
                }


                var championInfo = this.Menu.Item("DrawHealth_percent").IsActive()
                    ? champion + " (" + (int)hero.HealthPercent + "%)"
                    : champion;

                // Draws the championnames
                Font.DrawText(
                    null,
                    championInfo,
                    (int)
                    ((Drawing.Width - this.HudOffsetRight - this.HudOffsetText - Font.MeasureText(null, championInfo, FontDrawFlags.Left).Width)),
                    (int)(this.HudOffsetTop + i + 4 - Font.MeasureText(null, championInfo, FontDrawFlags.Left).Height / 2f),
                    hero.HealthPercent > 0 ? new ColorBGRA(255, 255, 255, 255) : new ColorBGRA(244, 8, 8, 255));

                // Draws the rectangle
                this.DrawRect(
                    Drawing.Width - this.HudOffsetRight,
                    this.HudOffsetTop + i,
                    100,
                    this.BarHeight,
                    1,
                    Color.FromArgb(255, 51, 55, 51));

                // Fils the rectangle
                this.DrawRect(
                    Drawing.Width - this.HudOffsetRight,
                    this.HudOffsetTop + i,
                    hero.HealthPercent <= 0 ? 100 : (int)(hero.HealthPercent),
                    this.BarHeight,
                    1,
                    hero.HealthPercent < 30 && hero.HealthPercent > 0 ? Color.FromArgb(255, 230, 169, 14) : hero.HealthPercent <= 0 ? Color.FromArgb(255, 206, 20, 30) : Color.FromArgb(255, 29, 201, 38));

                i += 20f + this.HudSpacing;
            }
        }

        /// <summary>
        ///     Draws a rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        private void DrawRect(float x, float y, int width, float height, float thickness, Color color)
        {
            for (var i = 0; i < height; i++)
            {
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
            }
        }

        #endregion
    }
}