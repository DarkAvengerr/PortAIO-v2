using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Menu
{
    #region Using Directives
    
    using System.Drawing;

    using LeagueSharp;
    using LeagueSharp.Common;

    using AlqoholicLissandra.Managers;

    using Color = SharpDX.Color;

    #endregion

    internal class AlqoholicMenu
    {
        #region Static Fields

        /// <summary>
        ///     Gets or sets the Menu
        /// </summary>
        internal static Menu MainMenu;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AlqoholicMenu" /> class
        /// </summary>
        internal AlqoholicMenu()
        {
            MainMenu = new Menu("Alqoholic Lissandra", "AlqoholicLissandra", true).SetFontStyle(
                FontStyle.Bold,
                Color.OrangeRed);

            MainMenu.AddSubMenu(this.GetOrbwalkerNode());
            MainMenu.AddSubMenu(this.GetTargetSelectorNode());

            var nodeHotKeys = new Menu("Hot Keys", "hotkeys");
            {
                nodeHotKeys.AddItem(
                    new MenuItem("farmspells", "Use Farm Spells").SetValue(new KeyBind('N', KeyBindType.Toggle)))
                    .Permashow(true, null, Color.OrangeRed);
            }
            MainMenu.AddSubMenu(nodeHotKeys);

            var nodeDrawing = new Menu("Drawing", "drawing.settings");
            {
                nodeDrawing.AddItem(
                    new MenuItem("draw.q", "Draw Q").SetValue(new Circle(true, System.Drawing.Color.OrangeRed)));
                nodeDrawing.AddItem(
                    new MenuItem("draw.w", "Draw W").SetValue(new Circle(true, System.Drawing.Color.OrangeRed)));
                nodeDrawing.AddItem(
                    new MenuItem("draw.e", "Draw E").SetValue(new Circle(true, System.Drawing.Color.OrangeRed)));
                nodeDrawing.AddItem(
                    new MenuItem("draw.r", "Draw R").SetValue(new Circle(true, System.Drawing.Color.OrangeRed)));

                nodeDrawing.AddItem(new MenuItem("draw.damage", "Draw Damage").SetValue(true));
                {
                    DrawingManager.DamageToUnit = DamageManager.GetComboDamage;
                }

            }
            MainMenu.AddSubMenu(nodeDrawing);

            MainMenu.AddToMainMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get orbwalker node
        /// </summary>
        /// <returns>
        ///     <see cref="Menu" /> class for the orbwalker
        /// </returns>
        internal Menu GetOrbwalkerNode()
        {
            var node = new Menu("Orbwalker", "AlqoholicKarthusOrbwalker");
            Program.Orbwalker = new Orbwalking.Orbwalker(node);

            return node;
        }

        /// <summary>
        ///     The get target selector node
        /// </summary>
        /// <returns>
        ///     <see cref="Menu" /> class for the target selector
        /// </returns>
        internal Menu GetTargetSelectorNode()
        {
            var node = new Menu("Target Selector", "AlqoholicKarthusTargetSelector");
            TargetSelector.AddToMenu(node);

            return node;
        }

        /// <summary>
        ///     Generates the Spells Menu Items
        /// </summary>
        /// <param name="spell">
        ///     Spell Slot
        /// </param>
        internal static void GenerateSpellMenu(SpellSlot spell)
        {
            var spellSlotName = spell.ToString();
            var spellSlotNameLower = spell.ToString().ToLower();

            var node = new Menu(spell + " Settings", "spellmenu" + spellSlotNameLower);

            if (spellSlotNameLower.Equals("q") || spellSlotNameLower.Equals("e"))
            {
                node.AddItem(
                    new MenuItem(spellSlotNameLower + "hitchance", "HitChance").SetValue(
                        new StringList(new[] { "Low", "Medium", "High", "Very High" }, 2)));
            }

            var nodeCombo = new Menu("Combo", spellSlotNameLower + "combomenu");
            {
                nodeCombo.AddItem(
                    new MenuItem("combo" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));

                if (spellSlotNameLower.Equals("e"))
                {
                    nodeCombo.AddItem(
                        new MenuItem("combo" + spellSlotNameLower + "2use", "Use " + spellSlotName + "2").SetValue(
                            true));
                    nodeCombo.AddItem(
                        new MenuItem("combo" + spellSlotNameLower + "smartlogic", "Smart Logic").SetValue(true)).SetTooltip("Will only E if can hit more than 2 with W at end position");
                }

                nodeCombo.AddItem(
                    new MenuItem("combo" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(10)));

                if (spellSlotNameLower.Equals("r"))
                {
                    nodeCombo.AddItem(
                        new MenuItem("r.surround", "Self Ult When x In Range").SetValue(new Slider(2, 0, 5)).SetTooltip("0 to disable self-ulting"));

                    var nodeEnemies = new Menu("Ult Whitelist", "ult.whitelist").SetFontStyle(FontStyle.Bold);
                    {
                        foreach (var enemy in HeroManager.Enemies)
                        {
                            nodeEnemies.AddItem(
                                new MenuItem("ult" + enemy.ChampionName, "Ult " + enemy.ChampionName).SetValue(true));
                        }
                    }
                    nodeCombo.AddSubMenu(nodeEnemies);
                }
            }
            node.AddSubMenu(nodeCombo);

            if (!spellSlotNameLower.Equals("r"))
            {
                var nodeMixed = new Menu("Mixed", spellSlotNameLower + "mixedmenu");
                {
                    nodeMixed.AddItem(
                        new MenuItem("mixed" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                    nodeMixed.AddItem(
                        new MenuItem("mixed" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(60)));
                }
                node.AddSubMenu(nodeMixed);

                var nodeLaneClear = new Menu("LaneClear", spellSlotNameLower + "laneclearmenu");
                {
                    nodeLaneClear.AddItem(
                        new MenuItem("laneclear" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(
                            !spellSlotNameLower.Equals("e") && !spellSlotNameLower.Equals("w")));

                    if (spellSlotNameLower.Equals("w"))
                    {
                        nodeLaneClear.AddItem(
                            new MenuItem("laneclear" + spellSlotNameLower + "minhit", "Min. Hit").SetValue(
                                new Slider(3, 1, 6)));
                    }

                    nodeLaneClear.AddItem(
                        new MenuItem("laneclear" + spellSlotNameLower + "mana", "Min. Mana").SetValue(
                            new Slider(40)));
                }
                node.AddSubMenu(nodeLaneClear);

                if (spellSlotNameLower.Equals("q"))
                {
                    var nodeLastHit = new Menu("LastHit", spellSlotNameLower + "lasthitmenu");
                    {
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(
                                true));
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "mana", "Min. Mana").SetValue(
                                new Slider(40)));
                    }
                    node.AddSubMenu(nodeLastHit);
                }
            }

            if (spellSlotNameLower.Equals("r"))
            {
                var nodeMisc = new Menu("Misc", "r.misc");
                {
                    nodeMisc.AddItem(new MenuItem("notimplemented", "NOT IMPLEMENTED [WIP]"));
                    nodeMisc.AddItem(new MenuItem("r.ultme", "Self R if Low").SetValue(true)).SetTooltip("Selfcast R if low and more than 2 enemies nearby");
                    nodeMisc.AddItem(new MenuItem("r.ultme.health", "Health %").SetValue(new Slider(10)).SetTooltip("Will only cast below this health"));
                }
                node.AddSubMenu(nodeMisc);
            }

            if (spellSlotNameLower.Equals("e"))
            {
                node.AddItem(
                    new MenuItem("etomouse", "E to Mouse - Escape").SetValue(new KeyBind('G', KeyBindType.Press)).SetTooltip("Hold for Escape, Press for E"));
            }

            MainMenu.AddSubMenu(node);
        }

        #endregion
    }
}