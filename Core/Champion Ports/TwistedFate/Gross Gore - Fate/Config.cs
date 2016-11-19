using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{
    #region Use

    using LeagueSharp.Common;
    using System.Drawing;

    #endregion

    internal static class Config
    {
        #region Properties

        internal static Menu QMenu { get; private set; }

        internal static Menu WMenu { get; private set; }

        internal static Menu ExtraMenu { get; private set; }

        internal static Menu DrawMenu { get; private set; }

        internal static Menu OrbwalkMenu { get; private set; }

        internal static Menu TargetSelectorMenu { get; private set; }

        internal static Menu TwistedFateMenu { get; private set; }

        #endregion

        #region Methods

        internal static void BuildConfig()
        {
            TwistedFateMenu = new Menu("Gross Gore - Fate", "twistedfate", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.OrangeRed);

            TargetSelectorMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            TwistedFateMenu.AddSubMenu(TargetSelectorMenu);

            OrbwalkMenu = new Menu("Orbwalker", "orbwalker");
            Mainframe.Orbwalker = new Orbwalking.Orbwalker(OrbwalkMenu);
            TwistedFateMenu.AddSubMenu(OrbwalkMenu);

            QMenu = new Menu("Q Spell", "qSpellMenu");
            QMenu.AddItem(
                new MenuItem("qClear", "Q Wave Clear (hold)").SetValue(
                    new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            QMenu.AddItem(
                new MenuItem("qEnemy", "Q Enemy (hold)").SetValue(
                    new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            QMenu.AddItem(new MenuItem("qDashing", "Q Dashing").SetValue(false));
            QMenu.AddItem(new MenuItem("qDashingTip", "Tip: Disable Q Dashing against Yasuo.")).SetFontStyle(FontStyle.Bold, SharpDX.Color.OrangeRed);
            QMenu.AddItem(new MenuItem("qRed", "Burst Red Card -> Q").SetValue(false));
            QMenu.AddItem(new MenuItem("qSlowed", "Q Slowed").SetValue(false));
            QMenu.AddItem(new MenuItem("qDashingDesc", "Always casts Q spell on immobile - KS - Burst Gold->Q.")).SetFontStyle(FontStyle.Bold, SharpDX.Color.OrangeRed);
            TwistedFateMenu.AddSubMenu(QMenu);

            WMenu = new Menu("W Spell", "wSpellMenu");
            WMenu.AddItem(new MenuItem("wHarass", "Fast Harass").SetValue(true));
            WMenu.AddItem(new MenuItem("wHarassDesc", "Fast Harass pick the fist card if a target is near your AA range in Mixed Mode.")).SetFontStyle(FontStyle.Bold, SharpDX.Color.OrangeRed);
            WMenu.AddItem(new MenuItem("wHarassTip", "Tip: Recommended.")).SetFontStyle(FontStyle.Bold, SharpDX.Color.OrangeRed);
            WMenu.AddItem(
                new MenuItem("csBlue", "Manual Blue Card").SetValue(
                    new KeyBind("U".ToCharArray()[0], KeyBindType.Press)));
            WMenu.AddItem(
                new MenuItem("csRed", "Manual Red Card").SetValue(new KeyBind("I".ToCharArray()[0], KeyBindType.Press)));
            WMenu.AddItem(new MenuItem("wSelect", "Always Gold in Combo.")).SetFontStyle(FontStyle.Bold, SharpDX.Color.OrangeRed);
            TwistedFateMenu.AddSubMenu(WMenu);

            ExtraMenu = new Menu("More+", "extraMenu");
            ExtraMenu.AddItem(new MenuItem("goldInter", "Interrupter").SetValue(true));
            ExtraMenu.AddItem(new MenuItem("goldGap", "Anti-GapCloser").SetValue(true));
            ExtraMenu.AddItem(new MenuItem("moreDesc", "Requires a READY Gold Card!")).SetFontStyle(FontStyle.Bold, SharpDX.Color.OrangeRed);
            TwistedFateMenu.AddSubMenu(ExtraMenu);

            DrawMenu = new Menu("Drawings", "drawings");
            DrawMenu.AddItem(new MenuItem("drawQrange", "Q Range").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawRrange", "R Range").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawRmap", "R Range Minimap").SetValue(true));
            DrawMenu.AddItem(new MenuItem("drawOnlyReady", "Ready Only").SetValue(true));
            TwistedFateMenu.AddSubMenu(DrawMenu);

            TwistedFateMenu.AddToMainMenu();
        }

        #region Getters

        internal static int GetSliderValue(string itemName)
        {
            return TwistedFateMenu.Item(itemName).GetValue<Slider>().Value;
        }

        internal static int GetStringListValue(string itemName)
        {
            return TwistedFateMenu.Item(itemName).GetValue<StringList>().SelectedIndex;
        }

        internal static bool IsChecked(string itemName)
        {
            return TwistedFateMenu.Item(itemName).GetValue<bool>();
        }

        internal static bool IsKeyPressed(string itemName)
        {
            return TwistedFateMenu.Item(itemName).GetValue<KeyBind>().Active;
        }

        #endregion

        #endregion
    }
}