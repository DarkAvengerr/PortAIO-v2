namespace ElTahmKench.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElTahmKench.Enumerations;
    using ElTahmKench.Utils;
    using EloBuddy;
    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The my menu class.
    /// </summary>
    internal class MyMenu
    {

        /// <summary>
        ///     List with default activated spells.
        /// </summary>
        public static readonly List<string> TrueStandard = new List<string> { "Stun", "Charm", "Flee", "Fear", "Taunt", "Polymorph", "Suppression" };
                   

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MyMenu" /> class.
        /// </summary>
        internal MyMenu()
        {
            RootMenu = new Menu("ElTahmKench", "ElTahmKench", true);

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

                if (!spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeCombo = new Menu("Combo", spellSlotNameLower + "combomenu");
                    {
                        nodeCombo.AddItem(
                            new MenuItem("combo" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                        nodeCombo.AddItem(
                            new MenuItem("combo" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(5)));

                        if (spellSlotNameLower.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                        {
                            nodeCombo.AddItem(new MenuItem("combominionuse", "Eat minion").SetValue(false));
                        }
                    }

                    node.AddSubMenu(nodeCombo);
                }

                if (spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeShield = new Menu("Shield", spellSlotNameLower + "shieldmenu");
                    {
                        nodeShield.AddItem(new MenuItem("shield" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                        nodeShield.AddItem(new MenuItem("ehealthpercentage", "Use " + spellSlotName + " on health percentage").SetValue(new Slider(20)));
                        nodeShield.AddItem(new MenuItem("shield" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(5)));
                    }

                    node.AddSubMenu(nodeShield);
                }

                if (!spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                {

                    var nodeMixed = new Menu("Mixed", spellSlotNameLower + "mixedmenu");
                    {
                        nodeMixed.AddItem(
                            new MenuItem("mixed" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                        nodeMixed.AddItem(
                            new MenuItem("mixed" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(50)));
                    }

                    node.AddSubMenu(nodeMixed);
                }

                if (spellSlotNameLower.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeAlly = new Menu("Ally settings", spellSlotNameLower + "allymenu");
                    {
                        nodeAlly.AddItem(new MenuItem("allydangerousults", "Use " + spellSlotName + " on dangerous ults").SetValue(true));
                        nodeAlly.AddItem(new MenuItem("walktotarget", "Orbwalk to target").SetValue(true));
                        nodeAlly.AddItem(new MenuItem("sep3-0-2", String.Empty));
                        nodeAlly.AddItem(new MenuItem("allylowhpults", "Use " + spellSlotName + " on low HP allies").SetValue(true));
                        nodeAlly.AddItem(new MenuItem("allylowhpultsslider", "Use " + spellSlotName + " Ally Health percentage").SetValue(new Slider(20)));
                        nodeAlly.AddItem(new MenuItem("sep3-2", String.Empty));
                        nodeAlly.AddItem(new MenuItem("allycc", "Use " + spellSlotName + " when ally is cc'd").SetValue(true));

                        foreach (var buffType in Misc.DevourerBuffTypes.Select(x => x.ToString()))
                        {
                            nodeAlly.SubMenu("Buff Types").AddItem(new MenuItem($"buffscc{buffType}", buffType).SetValue(TrueStandard.Contains($"{buffType}")));
                        }

                        nodeAlly.AddItem(new MenuItem("sep3-0", String.Empty));

                        foreach (var allies in HeroManager.Allies.Where(a => !a.IsMe))
                        {
                            nodeAlly.AddItem(new MenuItem($"won{allies.ChampionName}", "Use on " + allies.ChampionName))
                                .SetValue(true);
                        }
                    }

                    node.AddSubMenu(nodeAlly);
                }

                if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeLastHit = new Menu("LastHit", spellSlotNameLower + "lasthitmenu");
                    {
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(50)));
                    }

                    node.AddSubMenu(nodeLastHit);
                }


                if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodedraw = new Menu("Draw", spellSlotNameLower + "drawmenu");
                    {
                        nodedraw.AddItem(
                            new MenuItem("draw" + spellSlotNameLower + "use", "Draw " + spellSlotName).SetValue(true));
                    }

                    node.AddSubMenu(nodedraw);
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
            var node = new Menu("Orbwalker", "ElTahmKenchorbwalker");
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
            var node = new Menu("Target Selector", "ElTahmKenchtargetselector");
            TargetSelector.AddToMenu(node);

            return node;
        }

        #endregion
    }
}