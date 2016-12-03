
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Karma
{
    using ExorAIO.Utilities;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

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
                    Vars.QMenu.Add(new MenuSliderButton("harass", "Harass / if Mana >= x%", 50, 0, 99, true));
                    Vars.QMenu.Add(new MenuSliderButton("clear", "Clear / if Mana >= x%", 50, 0, 99, true));
                }
                Vars.SpellsMenu.Add(Vars.QMenu);

                /// <summary>
                ///     Sets the menu for the W.
                /// </summary>
                Vars.WMenu = new Menu("w", "Use W to:");
                {
                    Vars.WMenu.Add(new MenuBool("combo", "Combo", true));
                    Vars.WMenu.Add(new MenuSliderButton("jungleclear", "JungleClear / if Mana >= x%", 50, 0, 99, true));
                    Vars.WMenu.Add(
                        new MenuSliderButton("lifesaver", "Logical Lifesaver / If Health <= x%", 20, 10, 100, true));
                }
                Vars.SpellsMenu.Add(Vars.WMenu);

                /// <summary>
                ///     Sets the menu for the E.
                /// </summary>
                Vars.EMenu = new Menu("e", "Use E to:");
                {
                    Vars.EMenu.Add(new MenuBool("engager", "Engager"));
                    Vars.EMenu.Add(new MenuBool("logical", "Logical", true));
                    Vars.EMenu.Add(new MenuBool("gapcloser", "Anti-Gapcloser", true));
                    Vars.EMenu.Add(new MenuSliderButton("aoe", "Logical AoE / If Allies >= x", 3, 2, 6, true));
                    {
                        /// <summary>
                        ///     Sets the whitelist menu for the E.
                        /// </summary>
                        Vars.WhiteListMenu = new Menu("whitelist", "Shield: Whitelist Menu", true);
                        {
                            foreach (var ally in GameObjects.AllyHeroes)
                            {
                                Vars.WhiteListMenu.Add(
                                    new MenuBool(ally.ChampionName.ToLower(), $"Use for: {ally.ChampionName}", true));
                            }
                        }

                        Vars.EMenu.Add(Vars.WhiteListMenu);
                    }
                }

                Vars.SpellsMenu.Add(Vars.EMenu);

                /// <summary>
                ///     Sets the menu for the R.
                /// </summary>
                Vars.RMenu = new Menu("r", "Use R to:");
                {
                    Vars.RMenu.Add(new MenuBool("empq", "Logical Q Empower", true));
                    Vars.RMenu.Add(new MenuBool("empe", "Logical E Empower", true));
                }
                Vars.SpellsMenu.Add(Vars.RMenu);
            }

            Vars.Menu.Add(Vars.SpellsMenu);

            /// <summary>
            ///     Sets the miscellaneous menu.
            /// </summary>
            Vars.MiscMenu = new Menu("miscellaneous", "Miscellaneous");
            {
                Vars.MiscMenu.Add(
                    new MenuSeparator(
                        "separator",
                        "The Support mode doesn't attack or throw spells to minions if there are allies nearby."));
                Vars.MiscMenu.Add(new MenuBool("support", "Support Mode"));
            }
            Vars.Menu.Add(Vars.MiscMenu);

            /// <summary>
            ///     Sets the drawings menu.
            /// </summary>
            Vars.DrawingsMenu = new Menu("drawings", "Drawings");
            {
                Vars.DrawingsMenu.Add(new MenuBool("q", "Q Range"));
                Vars.DrawingsMenu.Add(new MenuBool("w", "W Range"));
                Vars.DrawingsMenu.Add(new MenuBool("e", "E Range"));
            }
            Vars.Menu.Add(Vars.DrawingsMenu);
        }

        #endregion
    }
}