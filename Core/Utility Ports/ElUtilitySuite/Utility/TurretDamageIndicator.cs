using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Utility
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     Indiciates the tower damage that will be done onto the player.
    /// </summary>
    /// <seealso cref="ElUtilitySuite.IPlugin" />
    internal class TurretDamageIndicator  //: IPlugin
    {
        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        private Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "MiscMenu");
            var menu = rootMenu.Children.Any(predicate)
                           ? rootMenu.Children.First(predicate)
                           : rootMenu.AddSubMenu(new Menu("Misc", "MiscMenu"));

            this.Menu = menu.AddSubMenu(new Menu("Turret Damage Indicator", "TowerDamageIndicicator"));
            this.Menu.AddItem(new MenuItem("Distance", "Tower Distance").SetValue(new Slider(775 + 1000, 775, 5000)));
            this.Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Drawing.OnDraw += this.OnDraw;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnDraw(EventArgs args)
        {
            if (!this.Menu.Item("Enabled").IsActive()
                || !ObjectManager.Get<Obj_AI_Turret>()
                    .Any(
                        x =>
                            x.Distance(Player) < this.Menu.Item("Distance").GetValue<Slider>().Value && x.IsEnemy
                            && !x.IsDead && x.IsValid) || !Player.IsHPBarRendered || !Player.IsValid)
            {
                return;
            }

            var width = Drawing.Width;
            var height = Drawing.Height;

            var barPos = Player.HPBarPosition;

            if (barPos.X < -200 || barPos.X > width + 200)
            {
                return;
            }

            if (barPos.Y < -200 || barPos.X > height + 200)
            {
                return;
            }

            var turret =
                ObjectManager.Get<Obj_AI_Turret>()
                    .OrderBy(x => x.Distance(Player))
                    .First(x => x.IsEnemy && !x.IsDead && x.IsValid);
            var damage = turret.GetAutoAttackDamage(Player, true);

            var percentHealthAfterDamage = Math.Max(0, Player.Health - damage) / Player.MaxHealth;
            var xPos = (float)(barPos.X + 10 + 103 * percentHealthAfterDamage);

            Drawing.DrawLine(xPos, barPos.Y + 20, xPos, barPos.Y + 20 + 8, 2, Color.Lime);
        }

        #endregion
    }
}