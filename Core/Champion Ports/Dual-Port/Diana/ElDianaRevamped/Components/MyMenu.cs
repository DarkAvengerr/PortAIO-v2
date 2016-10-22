namespace ElDianaRevamped.Components
{
    using System;

    using ElDianaRevamped.Enumerations;
    using ElDianaRevamped.Utils;
    using EloBuddy;
    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The my menu class.
    /// </summary>
    internal class MyMenu
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MyMenu" /> class.
        /// </summary>
        internal MyMenu()
        {
            RootMenu = new Menu("ElDianaRevamped", "eldianarevamped", true);

            RootMenu.AddSubMenu(GetTargetSelectorNode());
            RootMenu.AddSubMenu(GetOrbwalkerNode());

            RootMenu.AddToMainMenu();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the root menu.
        /// </summary>
        internal static Menu RootMenu { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The generate spell menu method.
        /// </summary>
        /// <param name="spellSlot">
        ///     The spell slot.
        /// </param>
        internal static void GenerateSpellMenu(SpellSlot spellSlot)
        {
            try
            {
                var spellSlotName = spellSlot.ToString();
                var spellSlotNameLower = spellSlotName.ToLower();

                var node = new Menu(spellSlot + " Settings", "spellmenu" + spellSlotNameLower);

                var nodeCombo = new Menu("Combo", spellSlotNameLower + "combomenu");
                {
                    nodeCombo.AddItem(new MenuItem("combo" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                    nodeCombo.AddItem(new MenuItem("combo" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(5)));

 
                    if (spellSlotNameLower.Equals("r", StringComparison.InvariantCultureIgnoreCase))
                    {
                        nodeCombo.AddItem(new MenuItem("comboRSecure", "Use R to secure kill").SetValue(true));
                        nodeCombo.AddItem(new MenuItem("comboRSecureRange", "Max enemies in range").SetValue(new Slider(5, 1, 5)));
                        nodeCombo.AddItem(new MenuItem("comboRSecureHealth", "Don't use ult if HP% <").SetValue(new Slider(20)));
                    }
                }

                node.AddSubMenu(nodeCombo);

                var nodeMixed = new Menu("Mixed", spellSlotNameLower + "mixedmenu");
                {
                    nodeMixed.AddItem(new MenuItem("mixed" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                    nodeMixed.AddItem(new MenuItem("mixed" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(50)));
                }

                node.AddSubMenu(nodeMixed);

                if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeLastHit = new Menu("LastHit", spellSlotNameLower + "lasthitmenu");
                    {
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(50)));

                        nodeCombo.AddItem(new MenuItem("lasthit.mode", "Q mode").SetValue(new StringList(new[] { "Always", "Out of range" }, 0)));
                    }
                    node.AddSubMenu(nodeLastHit);
                }


                if (!spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeLaneClear = new Menu("Clear", spellSlotNameLower + "laneclearmenu");
                    {
                        nodeLaneClear.AddItem(new MenuItem("laneclear" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                        nodeLaneClear.AddItem(new MenuItem("laneclear" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(50)));

                        if (spellSlotNameLower.Equals("r", StringComparison.InvariantCultureIgnoreCase))
                        {
                            nodeLaneClear.AddItem(new MenuItem("laneclear.r.siege", "Use " + spellSlotName + " to execute siege").SetValue(false));
                        }

                        if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                        {
                            nodeLaneClear.AddItem(new MenuItem("lasthit.count", "Minions hit Q").SetValue(new Slider(2, 1, 5)));
                        }
                    }

                    node.AddSubMenu(nodeLaneClear);
                }

                if (spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeInterrupt = new Menu("Interrupt", spellSlotNameLower + "interruptmenu");
                    {
                        nodeInterrupt.AddItem(new MenuItem("gapcloser.e", "Use E on gapcloser").SetValue(false));
                        nodeInterrupt.AddItem(new MenuItem("interrupt.e", "Use E to interrupt").SetValue(false));
                        nodeInterrupt.AddItem(new MenuItem("interrupt.e.dash", "Use E to interrupt dashes").SetValue(false));
                        nodeInterrupt.AddItem(new MenuItem("interrupt.e.dash.mana", "Min. Mana").SetValue(new Slider(50)));
                    }

                    node.AddSubMenu(nodeInterrupt);
                }

                RootMenu.AddSubMenu(node);
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@MyMenu.cs: Can not generate menu for spell - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The get orbwalker node.
        /// </summary>
        /// <returns>
        ///     <see cref="Menu" /> class for the orbwalker.
        /// </returns>
        private static Menu GetOrbwalkerNode()
        {
            var node = new Menu("Orbwalker", "eldianarevampedorbwalker");
            Program.Orbwalker = new Orbwalking.Orbwalker(node);

            return node;
        }

        /// <summary>
        ///     The get target selector node.
        /// </summary>
        /// <returns>
        ///     <see cref="Menu" /> class for the target selector.
        /// </returns>
        private static Menu GetTargetSelectorNode()
        {
            var node = new Menu("Target Selector", "eldianarevampedtargetselector");
            TargetSelector.AddToMenu(node);

            return node;
        }

        #endregion
    }
}