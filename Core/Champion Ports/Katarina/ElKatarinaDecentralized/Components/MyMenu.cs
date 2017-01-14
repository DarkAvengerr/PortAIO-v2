using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElKatarinaDecentralized.Components
{
    using System;
    using System.Drawing;

    using ElKatarinaDecentralized.Damages;
    using ElKatarinaDecentralized.Enumerations;
    using ElKatarinaDecentralized.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = SharpDX.Color;

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
            RootMenu = new Menu("ElKatarinaDecentralized", "ElKatarinaDecentralized", true);
            RootMenu.AddSubMenu(GetTargetSelectorNode());
            RootMenu.AddSubMenu(GetOrbwalkerNode());
            RootMenu.AddSubMenu(GetFleeNode());
            RootMenu.AddSubMenu(GetKillstealNode());

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

                    if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                    {
                        nodeCombo.AddItem(new MenuItem("combo.q.units", "Cast Q on minions").SetValue(true));
                    }

                    if (spellSlotNameLower.Equals("e", StringComparison.InvariantCultureIgnoreCase))
                    {
                        nodeCombo.AddItem(new MenuItem("combo.e.tower", "Block E under tower").SetValue(false));
                        nodeCombo.AddItem(new MenuItem("combo.e.daggers", "Cast E on daggers only").SetValue(true))
                            .SetTooltip("When disabled E will cast on daggers and on champions. Daggers prioritized. ");
                    }

                    if (spellSlotNameLower.Equals("r", StringComparison.InvariantCultureIgnoreCase))
                    {
                        nodeCombo.AddItem(
                            new MenuItem("combo.r.s", "R options:").SetValue(
                                new StringList(new[] { "Only R when killable", "Always R" }, 1)));

                        nodeCombo.AddItem(new MenuItem("combo.r.range", "R range").SetValue(new Slider(350, 300, 550)));

                        nodeCombo.AddItem(new MenuItem("combo.r.ticks", "R ticks").SetValue(new Slider(7, 1, 15)));

                        nodeCombo.AddItem(new MenuItem("combo.disable.movement", "Disable Movement while casting R").SetValue(true));
                        nodeCombo.AddItem(new MenuItem("combo.disable.evade", "Disable Evade while casting R").SetValue(true));
                        nodeCombo.AddItem(new MenuItem("combo.r.no.enemies", "Cancel R if no enemies").SetValue(false));
                    }
                }

                node.AddSubMenu(nodeCombo);

                if (!spellSlotNameLower.Equals("r", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeMixed = new Menu("Mixed", spellSlotNameLower + "mixedmenu");
                    {
                        nodeMixed.AddItem(
                            new MenuItem("mixed" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(false));
                    }

                    node.AddSubMenu(nodeMixed);
                }

                if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeLastHit = new Menu("LastHit", spellSlotNameLower + "lasthitmenu");
                    {
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                    }
                    node.AddSubMenu(nodeLastHit);
                }


                if (spellSlotNameLower.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    var nodeLaneClear = new Menu("Clear", spellSlotNameLower + "laneclearmenu");
                    {
                        nodeLaneClear.AddItem(new MenuItem("laneclear" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                    }

                    node.AddSubMenu(nodeLaneClear);
                }

                var nodeDrawings = new Menu("Drawings", spellSlotNameLower + "drawingsmenu");
                {
                    nodeDrawings.AddItem(
                        new MenuItem("draw" + spellSlotNameLower, "Draw " + spellSlotName).SetValue(
                            new Circle(true, System.Drawing.Color.DeepSkyBlue)));
                }

                node.AddSubMenu(nodeDrawings);

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
            var node = new Menu("Orbwalker", "ElKatarinaDecentralizedorbwalker");
            Program.Orbwalker = new Orbwalking.Orbwalker(node);

            return node;
        }

        /// <summary>
        ///     The get items node.
        /// </summary>
        private static Menu GetFleeNode()
        {
            var node = new Menu("Flee", "Flee").SetFontStyle(FontStyle.Bold, Color.BlueViolet);
            {
                node.AddItem(
                        new MenuItem("wardjump.key", "Flee key").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)))
                    .SetTooltip("Jump to minions and allies");
            }

            return node;
        }

        /// <summary>
        ///     The get items node.
        /// </summary>
        private static Menu GetKillstealNode()
        {
            var node = new Menu("Misc / Killsteal", "Killsteal").SetFontStyle(FontStyle.Bold, Color.Pink);
            {
                node.AddItem(new MenuItem("ks.activated", "Enable killsteal").SetValue(true));
                node.AddItem(new MenuItem("ks.tower", "Killsteal under tower").SetValue(false));
                node.AddItem(new MenuItem("ks.q", "Use Q").SetValue(true));
                node.AddItem(new MenuItem("ks.e", "Use E").SetValue(true));
                node.AddItem(new MenuItem("ks.r", "Use R").SetValue(false));
                node.AddItem(new MenuItem("ks.r.ticks", "R ticks").SetValue(new Slider(7, 1, 15)));
                node.AddItem(new MenuItem("ks.r.cancel.r", "Cancel R to KS").SetValue(false));
                node.AddItem(new MenuItem("ks.rhp", "Min Health").SetValue(new Slider(10)));

                var dmgAfterE = new MenuItem("misc.drawcombodamage", "Draw combo damage").SetValue(true);
                var drawFill =
                    new MenuItem("misc.drawcolour", "Fill colour", true).SetValue(
                        new Circle(true, System.Drawing.Color.FromArgb(204, 255, 0, 1)));

                node.SubMenu("Combo drawings").AddItem(drawFill);
                node.SubMenu("Combo drawings").AddItem(dmgAfterE);

                //DrawDamage.DamageToUnit = RealDamages.GetTotalDamage;
                //DrawDamage.Enabled = dmgAfterE.GetValue<bool>();
                //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

                dmgAfterE.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                    };

                drawFill.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                    //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
            }

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
            var node = new Menu("Target Selector", "ElKatarinaDecentralizedtargetselector");
            TargetSelector.AddToMenu(node);

            return node;
        }

        #endregion
    }
}