using EloBuddy; 
using LeagueSharp.Common; 
namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Linq;

    using ElUtilitySuite.Logging;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    internal class BuildingTracker : IPlugin
    {
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

            var buildingMenu = menu.AddSubMenu(new Menu("Tower and Inhib tracker", "healthbuilding"));
            {
                buildingMenu.AddItem(new MenuItem("DrawHealth", "Activated").SetValue(true));
                buildingMenu.AddItem(new MenuItem("DrawTurrets", "Turrets").SetValue(true));
                buildingMenu.AddItem(new MenuItem("DrawInhibs", "Inhibitors").SetValue(true));
                buildingMenu.AddItem(new MenuItem("Turret.FontSize", "Tower Font size").SetValue(new Slider(13, 13, 30)));
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
                        FaceName = "Tahoma", Height = this.Menu.Item("Turret.FontSize").GetValue<Slider>().Value,
                        OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default
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
            try
            {
                if (!this.Menu.Item("DrawHealth").GetValue<bool>() || Drawing.Direct3DDevice.IsDisposed || Font.IsDisposed)
                {
                    return;
                }

                if (this.Menu.Item("DrawTurrets").GetValue<bool>())
                {
                    foreach (var turret in
                        ObjectManager.Get<Obj_AI_Turret>()
                            .Where(x => x != null && x.IsValid && !x.IsDead & x.HealthPercent <= 75))
                    {
                        var turretPosition = Drawing.WorldToMinimap(turret.Position);
                        var healthPercent = $"{(int)turret.HealthPercent}%";

                        Font.DrawText(
                            null,
                            healthPercent,
                            (int)
                            (turretPosition.X - Font.MeasureText(null, healthPercent, FontDrawFlags.Center).Width / 2f),
                            (int)
                            (turretPosition.Y - Font.MeasureText(null, healthPercent, FontDrawFlags.Center).Height / 2f),
                            new ColorBGRA(255, 255, 255, 255));
                    }
                }

                if (this.Menu.Item("DrawInhibs").GetValue<bool>())
                {
                    foreach (var inhibitor in
                        ObjectManager.Get<Obj_BarracksDampener>()
                            .Where(x => x.IsValid && !x.IsDead && x.Health > 1 && x.HealthPercent <= 75))
                    {
                        var turretPosition = Drawing.WorldToMinimap(inhibitor.Position);
                        var healthPercent = $"{(int)inhibitor.HealthPercent}%";

                        Font.DrawText(
                            null,
                            healthPercent,
                            (int)
                            (turretPosition.X - Font.MeasureText(null, healthPercent, FontDrawFlags.Center).Width / 2f),
                            (int)
                            (turretPosition.Y - Font.MeasureText(null, healthPercent, FontDrawFlags.Center).Height / 2f),
                            new ColorBGRA(255, 255, 255, 255));
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "BuildingTracker.cs: An error occurred: {0}", e);
            }
        }

        #endregion
    }
}