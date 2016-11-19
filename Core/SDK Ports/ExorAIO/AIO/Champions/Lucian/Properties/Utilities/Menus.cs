
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Lucian
{
    using System.Linq;
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
        ///     Sets the menu.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     Sets the spells menu.
            /// </summary>
            Vars.SpellsMenu = new Menu("spells", "Spells");
            {
                /// <summary>
                ///     Sets the menu for the Q.
                /// </summary>
                Vars.QMenu = new Menu("q", "Use Q to:");
                {
                    Vars.QMenu.Add(new MenuBool("combo", "Combo", true));
                    Vars.QMenu.Add(new MenuBool("killsteal", "KillSteal", true));
                    Vars.QMenu.Add(new MenuSliderButton("laneclear", "LaneClear / if Mana >= x%", 50, 0, 99, true));
                    Vars.QMenu.Add(new MenuSliderButton("jungleclear", "JungleClear / if Mana >= x%", 50, 0, 99, true));
                    {
                        /// <summary>
                        ///     Sets the Extended Q menu.
                        /// </summary>
                        Vars.Q2Menu = new Menu("extended", "Use Extended Q in:", true);
                        {
                            Vars.Q2Menu.Add(new MenuBool("excombo", "Combo", true));
                            Vars.Q2Menu.Add(new MenuBool("exkillsteal", "KillSteal", true));
                            Vars.Q2Menu.Add(new MenuSliderButton("mixed", "Mixed / if Mana >= %", 50, 0, 99, true));
                            Vars.Q2Menu.Add(
                                new MenuSliderButton("exlaneclear", "LaneClear / if Mana >= %", 50, 0, 99, true));
                        }
                        Vars.QMenu.Add(Vars.Q2Menu);

                        if (GameObjects.EnemyHeroes.Any())
                        {
                            /// <summary>
                            ///     Sets the Whitelist menu for the Extended Q.
                            /// </summary>
                            Vars.WhiteListMenu = new Menu("whitelist", "Extended Harass: Whitelist", true);
                            {
                                Vars.WhiteListMenu.Add(
                                    new MenuSeparator(
                                        "extendedsep",
                                        "Note: The Whitelist only works for Mixed and LaneClear."));
                                foreach (var target in GameObjects.EnemyHeroes)
                                {
                                    Vars.WhiteListMenu.Add(
                                        new MenuBool(
                                            target.ChampionName.ToLower(),
                                            $"Harass: {target.ChampionName}",
                                            true));
                                }
                            }

                            Vars.QMenu.Add(Vars.WhiteListMenu);
                        }
                        else
                        {
                            Vars.QMenu.Add(
                                new MenuSeparator(
                                    "exseparator",
                                    "No enemy champions found, no need for an Extended Q Menu."));
                        }
                    }
                }

                Vars.SpellsMenu.Add(Vars.QMenu);

                /// <summary>
                ///     Sets the menu for the W.
                /// </summary>
                Vars.WMenu = new Menu("w", "Use W to:");
                {
                    Vars.WMenu.Add(new MenuBool("combo", "Combo", true));
                    Vars.WMenu.Add(new MenuBool("killsteal", "KillSteal", true));
                    Vars.WMenu.Add(new MenuSliderButton("buildings", "Buildings / if Mana >= x%", 50, 0, 99, true));
                    Vars.WMenu.Add(new MenuSliderButton("laneclear", "LaneClear / if Mana >= x%", 50, 0, 99, true));
                    Vars.WMenu.Add(new MenuSliderButton("jungleclear", "JungleClear / if Mana >= x%", 50, 0, 99, true));
                }
                Vars.SpellsMenu.Add(Vars.WMenu);

                /// <summary>
                ///     Sets the menu for the E.
                /// </summary>
                Vars.EMenu = new Menu("e", "Use E to:");
                {
                    Vars.EMenu.Add(new MenuSeparator("esep", "E Modes:"));
                    Vars.EMenu.Add(
                        new MenuSeparator(
                            "esep1",
                            "[KEEP IN MIND THAT, NO MATTER THE MODE, THE DASH WILL BE DIRECTED TOWARDS YOUR MOUSE]"));
                    Vars.EMenu.Add(
                        new MenuSeparator(
                            "esep2",
                            "Exory: The Logic you have always used, with smart Short & Long dash."));
                    Vars.EMenu.Add(
                        new MenuSeparator(
                            "esep3",
                            "Normal: This Logic will make you always dash at the maximum distance."));
                    Vars.EMenu.Add(
                        new MenuSeparator(
                            "esep4",
                            "Normal: This Logic will make you always dash at the minimum distance."));
                    Vars.EMenu.Add(
                        new MenuSeparator(
                            "esep5",
                            "None: This Logic will prevent the assembly from using E automatically in combo."));
                    Vars.EMenu.Add(
                        new MenuList<string>(
                            "mode",
                            "E Mode",
                            new[] { "Exory", "Always Long", "Always Short", "Don't use E in Combo" }));
                    Vars.EMenu.Add(new MenuBool("engager", "Engager", true));
                    Vars.EMenu.Add(new MenuBool("gapcloser", "Anti-Gapcloser", true));
                    Vars.EMenu.Add(new MenuSliderButton("buildings", "Buildings / if Mana >= x%", 50, 0, 99, true));
                    Vars.EMenu.Add(new MenuSliderButton("laneclear", "LaneClear / if Mana >= x%", 50, 0, 99, true));
                    Vars.EMenu.Add(new MenuSliderButton("jungleclear", "JungleClear / if Mana >= x%", 50, 0, 99, true));
                }
                Vars.SpellsMenu.Add(Vars.EMenu);

                /// <summary>
                ///     Sets the menu for the R.
                /// </summary>
                Vars.RMenu = new Menu("r", "Use R to:");
                {
                    Vars.RMenu.Add(new MenuSeparator("separator", "How does it work:"));
                    Vars.RMenu.Add(
                        new MenuSeparator("separator2", "Keep the button pressed until you want to stop the ultimate."));
                    Vars.RMenu.Add(
                        new MenuSeparator(
                            "separator3",
                            "You don't have to press both Spacebar and the Semi-Automatic button while doing this,"));
                    Vars.RMenu.Add(
                        new MenuSeparator(
                            "separator4",
                            "since ExorLucian automatically orbwalks while channelling his R, so just press the button."));
                    Vars.RMenu.Add(new MenuBool("bool", "Semi-Automatic R", true));
                    Vars.RMenu.Add(new MenuKeyBind("key", "Key:", Keys.T, KeyBindType.Press));
                }
                Vars.SpellsMenu.Add(Vars.RMenu);
            }

            Vars.Menu.Add(Vars.SpellsMenu);

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = new Menu("drawings", "Drawings");
            {
                Vars.DrawingsMenu.Add(new MenuBool("q", "Q Range"));
                Vars.DrawingsMenu.Add(new MenuBool("qe", "Q Extended Range"));
                Vars.DrawingsMenu.Add(new MenuBool("w", "W Range"));
                Vars.DrawingsMenu.Add(new MenuBool("e", "E Range"));
            }
            Vars.Menu.Add(Vars.DrawingsMenu);
        }

        #endregion
    }
}