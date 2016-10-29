
#pragma warning disable 1587

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbActivator
{
    using System.Windows.Forms;

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
        ///     Sets the menus.
        /// </summary>
        public static void Initialize()
        {
            /// <summary>
            ///     Sets the main menu.
            /// </summary>
            Vars.Menu = new Menu("activator", "NabbActivator", true);
            {
                /// <summary>
                ///     Sets the smite menu.
                /// </summary>
                Vars.SmiteMenu = new Menu("smite", "Smite Menu");
                {
                    /// <summary>
                    ///     Sets the smite options menu.
                    /// </summary>
                    Vars.SmiteMiscMenu = new Menu("misc", "Miscellaneous");
                    {
                        Vars.SmiteMiscMenu.Add(new MenuBool("combo", "Combo", true));
                        Vars.SmiteMiscMenu.Add(new MenuBool("killsteal", "KillSteal", true));
                        Vars.SmiteMiscMenu.Add(new MenuBool("stacks", "Keep 1 Stack for Dragon/Baron/Herald", true));
                        Vars.SmiteMiscMenu.Add(new MenuBool("limit", "Only on Dragon/Baron/Herald"));
                    }
                    Vars.SmiteMenu.Add(Vars.SmiteMiscMenu);

                    /// <summary>
                    ///     Sets the drawings menu.
                    /// </summary>
                    Vars.DrawingsMenu = new Menu("drawings", "Drawings");
                    {
                        Vars.DrawingsMenu.Add(new MenuBool("range", "Smite Range", true));
                        Vars.DrawingsMenu.Add(new MenuBool("damage", "Smite Damage", true));
                    }
                    Vars.SmiteMenu.Add(Vars.DrawingsMenu);
                }
                Vars.Menu.Add(Vars.SmiteMenu);

                /// <summary>
                ///     Sets the consumable sliders menu.
                /// </summary>
                Vars.SliderMenu = new Menu("consumables", "Consumables Menu");
                {
                    Vars.SliderMenu.Add(new MenuSlider("health", "Consumables: Health < x%", 50));
                    Vars.SliderMenu.Add(new MenuSlider("mana", "Consumables: Mana < x%", 50));
                }
                Vars.Menu.Add(Vars.SliderMenu);

                /// <summary>
                ///     Sets the keys menu.
                /// </summary>
                Vars.KeysMenu = new Menu("keys", "Keybinds Menu");
                {
                    Vars.KeysMenu.Add(new MenuSeparator("separator", "The following will only work if Enabled."));
                    Vars.KeysMenu.Add(new MenuKeyBind("combo", "Combo:", Keys.Space, KeyBindType.Press));
                    Vars.KeysMenu.Add(new MenuKeyBind("laneclear", "LaneClear:", Keys.V, KeyBindType.Press));
                    Vars.KeysMenu.Add(new MenuKeyBind("smite", "Smite (Toggle):", Keys.Y, KeyBindType.Toggle));
                }
                Vars.Menu.Add(Vars.KeysMenu);
                Vars.Menu.Add(new MenuBool("offensives", "Offensives", true));
                Vars.Menu.Add(new MenuBool("defensives", "Defensives", true));
                Vars.Menu.Add(new MenuBool("spells", "Spells", true));
                Vars.Menu.Add(new MenuBool("potions", "Potions", true));
                Vars.Menu.Add(new MenuBool("resetters", "Tiamat/Hydra/Titanic", true));
                Vars.Menu.Add(new MenuSliderButton("cleansers", "Cleansers / Delay", 0, 0, 300, true));
            }
            Vars.Menu.Attach();
        }

        #endregion
    }
}