using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengarDecentralized.Components
{
    using System;
    using System.Linq;

    using ElRengarDecentralized.Enumerations;
    using ElRengarDecentralized.Utils;

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
            RootMenu = new Menu("ElRengarDecentralized", "ElRengarDecentralized", true);
            RootMenu.AddSubMenu(GetOrbwalkerNode());
            RootMenu.AddSubMenu(GetItemsNode());
            RootMenu.AddSubMenu(GetPrioNode());
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

                    if (spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                    {
                        nodeCombo.AddItem(new MenuItem("comborootstunned", "Auto root").SetValue(false))
                            .SetTooltip("automatically cast empowered E on stunned targets");
                    }
                }

                node.AddSubMenu(nodeCombo);

                if (spellSlotNameLower.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeCleanse = new Menu("Cleanse", spellSlotNameLower + "cleansemenu");
                    {
                        nodeCleanse.AddItem(new MenuItem("MinDuration", "Minimum Duration (MS)").SetValue(new Slider(500, 0, 25000))).SetTooltip("The minimum duration of the spell to get cleansed");
                        foreach (var buffType in BuffManager.Buffs.Select(x => x.ToString()))
                        {
                            nodeCleanse.SubMenu("Buff Types").AddItem(new MenuItem($"Cleanse{buffType}", buffType).SetValue(true));
                        }
                    }

                    node.AddSubMenu(nodeCleanse);
                }

                var nodeMixed = new Menu("Mixed", spellSlotNameLower + "mixedmenu");
                {
                    nodeMixed.AddItem(new MenuItem("mixed" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                }

                node.AddSubMenu(nodeMixed);

                if (!spellSlotNameLower.Equals("r", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeLastHit = new Menu("LastHit", spellSlotNameLower + "lasthitmenu");
                    {
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                    }
                    node.AddSubMenu(nodeLastHit);
                }


                if (!spellSlotNameLower.Equals("r", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeLaneClear = new Menu("Clear", spellSlotNameLower + "laneclearmenu");
                    {
                        nodeLaneClear.AddItem(new MenuItem("laneclear" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                    }

                    if (spellSlotNameLower.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                    {
                        nodeLaneClear.AddItem(new MenuItem("laneclear.w.hit", "Minimum minions hit").SetValue(new Slider(2, 1, 6)));
                    }

                    node.AddSubMenu(nodeLaneClear);
                }

                RootMenu.AddSubMenu(node);
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@MyMenu.cs: Can not generate menu for spell - {0}", e);
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
            var node = new Menu("Orbwalker", "ElRengarDecentralizedorbwalker");
            Program.Orbwalker = new Orbwalking.Orbwalker(node);

            return node;
        }

        /// <summary>
        ///     The get items node.
        /// </summary>
        private static Menu GetItemsNode()
        {
            var node = new Menu("Items", "items");
            {
                node.AddItem(new MenuItem("Itemscombo", "Use in Combo").SetValue(true));
                node.AddItem(new MenuItem("Itemsharass", "Use in Harass").SetValue(true));
                node.AddItem(new MenuItem("FarmEnabled", "Activate").SetValue(true)).SetTooltip("Enabled in LastHit and LaneClear mode.");
                node.AddItem(new MenuItem("Itemslaneclear", "Use in LaneClear").SetValue(true));
            }

            return node;
        }


        /// <summary>
        ///     The get items node.
        /// </summary>
        private static Menu GetPrioNode()
        {
            var node = new Menu("Priority", "Priority");
            {
                node.AddItem(
                               new MenuItem("combo.prio", "Prioritize").SetValue(
                                   new StringList(new[] { "E", "W", "Q" }, 2)))
                           .SetTooltip("Prioritze an empowered spell to get casted first");

                node.AddItem(
                    new MenuItem("combo.switch", "Priority switcher").SetValue(
                        new KeyBind("L".ToCharArray()[0], KeyBindType.Press))).SetTooltip("Change the spell priority");
            }

            return node;
        }


        #endregion
    }
}