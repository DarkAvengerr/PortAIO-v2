using System.Collections.Generic;
using RethoughtLib.Utility;

using EloBuddy; namespace ReformedAIO.Champions.Diana
{
    #region Using Directives

    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic.Killsteal;
    using ReformedAIO.Champions.Diana.Menus.Draw;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Combo;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Flee;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Jungleclear;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Laneclear;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Misaya;
    using ReformedAIO.Champions.Diana.OrbwalkingMode.Mixed;

    using RethoughtLib.Bootstraps.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class DianaLoader : LoadableBase
    {
        #region Public Properties

        public override string DisplayName { get; set; } = String.ToTitleCase("Reformed Diana");

        public override string InternalName { get; set; } = "Diana";

        public override IEnumerable<string> Tags { get; set; } = new[] { "Diana" };

        #endregion

        #region Public Methods and Operators

        public override void Load()
        {
            var superParent = new SuperParent(this.DisplayName);

            // Parents
            var combo = new Parent("Combo");
            var misaya = new Parent("Misaya");
            var mixed = new Parent("Mixed");
            var lane = new Parent("LaneClear");
            var jungle = new Parent("JungleClear");
            var ks = new Parent("Killsteal");
            var draw = new Parent("Drawings");
            var flee = new Parent("Flee");

            superParent.AddChildren(new[]
            {
                combo, misaya, mixed, lane, jungle, ks, draw, flee
            });

            combo.AddChild(new CrescentStrike());
            combo.AddChild(new Moonfall());
            combo.AddChild(new LunarRush());
            combo.AddChild(new PaleCascade());
            combo.AddChild(new MisayaCombo());

            mixed.AddChild(new MixedCrescentStrike());

            lane.AddChild(new LaneCrescentStrike());
            lane.AddChild(new LaneLunarRush());

            jungle.AddChild(new JungleCrescentStrike());
            jungle.AddChild(new JungleLunarRush());
            jungle.AddChild(new JungleMoonfall());
            jungle.AddChild(new JunglePaleCascade());

            ks.AddChild(new KsPaleCascade());
            ks.AddChild(new KsCrescentStrike());

            draw.AddChild(new DrawQ());
            draw.AddChild(new DrawE());
            draw.AddChild(new DrawDmg());
            draw.AddChild(new DrawPred());

            flee.AddChild(new FleeMode());

            superParent.OnLoadInvoker();

            var orbWalkingMenu = new Menu("Orbwalker", "Orbwalker");
            Variables.Orbwalker = new Orbwalking.Orbwalker(orbWalkingMenu);

            superParent.Menu.AddSubMenu(orbWalkingMenu);
        }

        #endregion
    }
}