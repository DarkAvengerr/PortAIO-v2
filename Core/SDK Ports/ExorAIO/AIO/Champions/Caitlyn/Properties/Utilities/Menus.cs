
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Caitlyn
{
    using System.Windows.Forms;

    using ExorAIO.Utilities;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;

    using Menu = LeagueSharp.SDK.UI.Menu;

    /// <summary>
    ///     The menu class.
    /// </summary>
    internal class Menus
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Initializes the menus.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     Sets the menu for the spells.
            /// </summary>
            Vars.SpellsMenu = new Menu("spells", "Spells");
            {
                /// <summary>
                ///     Sets the menu for the Q.
                /// </summary>
                Vars.QMenu = new Menu("q", "Use Q to:");
                {
                    Vars.QMenu.Add(new MenuBool("combo", "Combo", true));
                    Vars.QMenu.Add(new MenuBool("logical", "Logical (On Trapped Enemies)", true));
                    Vars.QMenu.Add(new MenuBool("killsteal", "KillSteal", true));
                    Vars.QMenu.Add(new MenuSliderButton("harass", "Harass / if Mana >= x%", 50, 0, 99, true));
                    Vars.QMenu.Add(new MenuSliderButton("laneclear", "LaneClear / if Mana >= x%", 50, 0, 99, true));
                    Vars.QMenu.Add(new MenuSliderButton("jungleclear", "JungleClear / if Mana >= x%", 50, 0, 99, true));
                }
                Vars.SpellsMenu.Add(Vars.QMenu);

                /// <summary>
                ///     Sets the menu for the W.
                /// </summary>
                Vars.WMenu = new Menu("w", "Use W to:");
                {
                    Vars.WMenu.Add(new MenuBool("combo", "Combo (Predict Enemy Movement)"));
                    Vars.WMenu.Add(new MenuBool("triplecombo", "Try Double/Triple HeadShot Combo", true));
                    Vars.WMenu.Add(new MenuBool("logical", "Logical (On CC'd Enemies)", true));
                    Vars.WMenu.Add(new MenuBool("gapcloser", "Anti-Gapcloser", true));
                    Vars.WMenu.Add(new MenuBool("interrupter", "Channelling Targets", true));
                }
                Vars.SpellsMenu.Add(Vars.WMenu);

                /// <summary>
                ///     Sets the menu for the E.
                /// </summary>
                Vars.EMenu = new Menu("e", "Use E to:");
                {
                    Vars.EMenu.Add(new MenuBool("combo", "Combo", true));
                    Vars.EMenu.Add(new MenuBool("gapcloser", "Anti-Gapcloser", true));
                    Vars.EMenu.Add(new MenuBool("interrupter", "Channelling Targets", true));
                }
                Vars.SpellsMenu.Add(Vars.EMenu);

                /// <summary>
                ///     Sets the menu for the R.
                /// </summary>
                Vars.RMenu = new Menu("r", "Use R to:");
                {
                    Vars.RMenu.Add(new MenuBool("killsteal", "KillSteal", true));
                    Vars.RMenu.Add(
                        new MenuSeparator(
                            "separator",
                            "The Semi-Automatic R will automatically ult the lowest on health, whitelisted and non-invulnerable enemy in range."));
                    Vars.RMenu.Add(new MenuBool("bool", "Semi-Automatic R", true));
                    Vars.RMenu.Add(new MenuKeyBind("key", "Key:", Keys.T, KeyBindType.Press));
                    {
                        /// <summary>
                        ///     Sets the menu for the R Whitelist.
                        /// </summary>
                        Vars.WhiteListMenu = new Menu("whitelist", "Ultimate: Whitelist Menu");
                        {
                            foreach (var target in GameObjects.EnemyHeroes)
                            {
                                Vars.WhiteListMenu.Add(
                                    new MenuBool(
                                        target.ChampionName.ToLower(),
                                        $"Use against: {target.ChampionName}",
                                        true));
                            }
                        }

                        Vars.RMenu.Add(Vars.WhiteListMenu);
                    }
                }

                Vars.SpellsMenu.Add(Vars.RMenu);
            }

            Vars.Menu.Add(Vars.SpellsMenu);

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = new Menu("miscellaneous", "Miscellaneous");
            {
                Vars.MiscMenu.Add(new MenuBool("reversede", "Reverse E if manually casted (Dash to CursorPos)"));
            }
            Vars.Menu.Add(Vars.MiscMenu);

            /// <summary>
            ///     Sets the menu for the drawings.
            /// </summary>
            Vars.DrawingsMenu = new Menu("drawings", "Drawings");
            {
                Vars.DrawingsMenu.Add(new MenuBool("q", "Q Range"));
                Vars.DrawingsMenu.Add(new MenuBool("w", "W Range"));
                Vars.DrawingsMenu.Add(new MenuBool("e", "E Range"));
                Vars.DrawingsMenu.Add(new MenuBool("r", "R Range"));
            }
            Vars.Menu.Add(Vars.DrawingsMenu);
        }

        #endregion
    }
}