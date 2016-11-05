namespace ElNamiDecentralized.Components
{
    using System;
    using System.Linq;

    using ElNamiDecentralized.Enumerations;
    using ElNamiDecentralized.Utils;

    using EloBuddy;
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
            RootMenu = new Menu("ElNamiDecentralized", "ElNamiDecentralized", true);

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
                        nodeCombo.AddItem(new MenuItem("combo" + spellSlotNameLower + "hit", "Minimum enemies hit R").SetValue(new Slider(3, 1, 5)));
                    }
                }

                node.AddSubMenu(nodeCombo);

                if (!spellSlotNameLower.Equals("r", StringComparison.InvariantCultureIgnoreCase))
                {

                    var nodeMixed = new Menu("Mixed", spellSlotNameLower + "mixedmenu");
                    {
                        nodeMixed.AddItem(
                            new MenuItem("mixed" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                        nodeMixed.AddItem(
                            new MenuItem("mixed" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(50)));

                        if (spellSlotNameLower.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                        {
                            nodeMixed.AddItem(
                                new MenuItem("harass.mode", "W mode").SetValue(
                                    new StringList(new[] { "Smart", "YOLO!" }, 0)));
                            nodeMixed.AddItem(
                                new MenuItem("smart.harass.health", "[SMART] Minimum enemy healthpercentage").SetValue(
                                    new Slider(75)));

                        }
                    }

                    node.AddSubMenu(nodeMixed);
                }

                if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeSupport = new Menu("Support", spellSlotNameLower + "supportmenu");
                    {
                        nodeSupport.AddItem(new MenuItem("support.mode", "Use support mode").SetValue(true));
                    }

                    node.AddSubMenu(nodeSupport);
                }


                if (spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeTideCaller = new Menu("Tidecaller's Blessing (E)", spellSlotNameLower + "tidecallermenu");
                    {
                        nodeTideCaller.AddItem(new MenuItem("tide.activated", "Use E").SetValue(true));
                        nodeTideCaller.AddItem(new MenuItem("tide.mode", "E mode").SetValue(new StringList(new[] { "Always", "Combo" }, 0)));
                        nodeTideCaller.AddItem(new MenuItem("tide.mana", "Min. Mana").SetValue(new Slider(20)));
                        foreach (var allies in HeroManager.Allies)
                        {
                            nodeTideCaller.AddItem(new MenuItem($"eon{allies.ChampionName}", "Use on " + allies.ChampionName))
                                .SetValue(true);
                        }
                    }

                    node.AddSubMenu(nodeTideCaller);
                }


                if (spellSlotNameLower.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeAllyHeal = new Menu("Heal", spellSlotNameLower + "healmenu");
                    {
                        nodeAllyHeal.AddItem(new MenuItem("heal.allies", "Use W").SetValue(true));
                        nodeAllyHeal.AddItem(new MenuItem("allies.healthpercantage", "Health Percentage").SetValue(new Slider(20)));
                    }

                    node.AddSubMenu(nodeAllyHeal);
                }


                if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeInterrupt = new Menu("Interrupt", spellSlotNameLower + "interruptmenu");
                    {
                        nodeInterrupt.AddItem(new MenuItem("gapcloser.q", "Use Q on gapcloser").SetValue(true));
                        nodeInterrupt.AddItem(new MenuItem("interrupt.q", "Use Q to interrupt").SetValue(true));
                        nodeInterrupt.AddItem(new MenuItem("interrupt.q.mana", "Min. Mana").SetValue(new Slider(5)));
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
            var node = new Menu("Orbwalker", "ElNamiDecentralizedorbwalker");
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
            var node = new Menu("Target Selector", "ElNamiDecentralizedtargetselector");
            TargetSelector.AddToMenu(node);

            return node;
        }

        #endregion
    }
}