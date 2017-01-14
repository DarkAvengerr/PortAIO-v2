using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicKarthus.Menu
{
    #region Using Directives

    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = SharpDX.Color;

    #endregion

    internal class AlqoholicMenu
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AlqoholicMenu" /> class
        /// </summary>
        internal AlqoholicMenu()
        {
            MainMenu = new Menu("Alqoholic Karthus", "AlqoholicKarthus", true).SetFontStyle(
                FontStyle.Bold,
                Color.OrangeRed);

            MainMenu.AddSubMenu(GetOrbwalkerNode());
            MainMenu.AddSubMenu(GetTargetSelectorNode());

            MainMenu.AddItem(
                new MenuItem("prediction.mode", "Prediction Mode").SetValue(
                new StringList(new[] { "Common", "OKTW" }))).SetFontStyle(FontStyle.Bold, Color.OrangeRed);

            var nodeHotKeys = new Menu("HotKeys", "HotKeys");
            {
                nodeHotKeys.AddItem(
                    new MenuItem("q.auto", "Mixed - Auto Q").SetValue(new KeyBind('T', KeyBindType.Toggle))).Permashow(true, null, Color.OrangeRed);
                nodeHotKeys.AddItem(
                    new MenuItem("farmspells", "Farm Spells").SetValue(new KeyBind('N', KeyBindType.Toggle, true)))
                    .Permashow(true, null, Color.OrangeRed);
            }

            MainMenu.AddSubMenu(nodeHotKeys);

            var nodeDrawing = new Menu("Draw", "draw.settings");
            {
                nodeDrawing.AddItem(new MenuItem("draw" + "q", "Draw " + "Q").SetValue(true));
                nodeDrawing.AddItem(new MenuItem("draw" + "w", "Draw " + "W").SetValue(true));
                nodeDrawing.AddItem(new MenuItem("draw" + "e", "Draw " + "E").SetValue(true));
            }

            MainMenu.AddSubMenu(nodeDrawing);

            MainMenu.AddToMainMenu();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the Menu
        /// </summary>
        internal static Menu MainMenu { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Generates the Spells Menu Items
        /// </summary>
        /// <param name="spell">
        ///     Spell Slot
        /// </param>
        internal static void GenerateSpellMenu(SpellSlot spell)
        {
            try
            {
                var spellSlotName = spell.ToString();
                var spellSlotNameLower = spell.ToString().ToLower();

                var node = new Menu(spell + " Settings", "spellmenu" + spellSlotNameLower);

                var nodeCombo = new Menu("Combo", spellSlotNameLower + "combomenu");
                {
                    nodeCombo.AddItem(
                        new MenuItem("combo" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                    if (spellSlotNameLower.Equals("q"))
                    {
                        nodeCombo.AddItem(
                            new MenuItem("combo.mode", "Mode").SetValue(
                                new StringList(new[] { "Always", "Only Single Hit" })));
                        nodeCombo.AddItem(
                            new MenuItem("combo.hitchance", "HitChance").SetValue(
                                new StringList(new[] { "Low", "Medium", "High", "Very High", "Only Immobile" }, 2)));
                    }
                    nodeCombo.AddItem(
                        new MenuItem("combo" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(10)));

                    if (spellSlotNameLower.Equals("r"))
                    {
                        nodeCombo.AddItem(new MenuItem("autor", "Auto R when Killable - WIP").SetValue(true));
                        nodeCombo.AddItem(new MenuItem("rsafespace", "Safe Space").SetValue(new Slider(2000, 0, 4000)));
                        nodeCombo.AddItem(
                            new MenuItem("rdmgreduction", "Dmg. Reduction %").SetValue(new Slider(0, 0, 10)));
                        var nodeEnemies = new Menu("Enemy Black List", "enemyblacklist");

                        foreach (var enemy in HeroManager.Enemies.ToList())
                        {
                            nodeEnemies.AddItem(
                                new MenuItem("dontr" + enemy.ChampionName, "Dont Use on " + enemy.ChampionName).SetValue
                                    (false));
                        }
                        nodeCombo.AddSubMenu(nodeEnemies);
                    }
                }
                node.AddSubMenu(nodeCombo);

                if (!spellSlotNameLower.Equals("w") && !spellSlotNameLower.Equals("r"))
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
                                true));
                        if (spellSlotNameLower.Equals("q"))
                        {
                            nodeLaneClear.AddItem(
                                new MenuItem("laneclear.mode", "Mode").SetValue(
                                    new StringList(new[] { "Always", "Out of range" })));
                        }
                        nodeLaneClear.AddItem(
                            new MenuItem("laneclear" + spellSlotNameLower + "mana", "Min. Mana").SetValue(
                                new Slider(60)));
                    }

                    node.AddSubMenu(nodeLaneClear);
                }

                if (spellSlotNameLower.Equals("q"))
                {
                    var nodeLastHit = new Menu("LastHit", spellSlotNameLower + "lasthitmenu");
                    {
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "use", "Use " + spellSlotName).SetValue(true));
                        nodeLastHit.AddItem(
                            new MenuItem("lasthit" + spellSlotNameLower + "mana", "Min. Mana").SetValue(new Slider(30)));
                    }
                    node.AddSubMenu(nodeLastHit);
                }

                MainMenu.AddSubMenu(node);
            }
            catch (Exception e)
            {
                Console.WriteLine("@AlqoholicMenu.cs: Cannot GenerateSpellMenu - {0}", e);
                throw;
            }
        }

        /// <summary>
        ///     The get orbwalker node
        /// </summary>
        /// <returns>
        ///     <see cref="Menu" /> class for the orbwalker
        /// </returns>
        private static Menu GetOrbwalkerNode()
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
        private static Menu GetTargetSelectorNode()
        {
            var node = new Menu("Target Selector", "AlqoholicKarthusTargetSelector");
            TargetSelector.AddToMenu(node);

            return node;
        }

        #endregion
    }
}