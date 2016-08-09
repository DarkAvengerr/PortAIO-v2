using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Olaf.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy;

namespace Olaf.Modes
{
    internal static class ModeHarass
    {
        public static Menu MenuLocal { get; private set; }

        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell E => Champion.PlayerSpells.E;

        public static void Init()
        {
            MenuLocal = new Menu("Harass", "Harass");
            {
                MenuLocal.AddItem(new MenuItem("Harass.Q", "Q:").SetValue(new StringList(new[] {"Off", "On: Small", "On: Large"}, 1)).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
                var qRange = (int) Q.Range;
                MenuLocal.AddItem(new MenuItem("Harass.Q.SmallRange", "Q Small Range:").SetValue(new Slider(qRange / 2, qRange / 2, qRange)).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
                MenuLocal.AddItem(new MenuItem("Harass.E", "E:").SetValue(new StringList(new[] { "Off", "On" }, 0)).SetFontStyle(FontStyle.Regular, Q.MenuColor()));
            }

            ModeConfig.MenuConfig.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent < CommonManaManager.HarassMinManaPercent)
            {
                return;
            }

            if (ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                ExecuteHarass();
            }

            if (ModeConfig.MenuKeys.Item("Key.HarassToggle").GetValue<KeyBind>().Active)
            {
                ExecuteToggle();
            }
        }

        private static void ExecuteToggle()
        {
            ExecuteHarass();
        }

        private static void ExecuteHarass()
        {
            var useQ = MenuLocal.Item("Harass.Q").GetValue<StringList>().SelectedIndex;
            if (useQ == 0)
            {
                return;
            }

            var t = TargetSelector.GetTarget(useQ == 2 ? Q.Range : MenuLocal.Item("Harass.Q.SmallRange").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (t.UnderTurret(true))
            {
                return;
            }

            Champion.PlayerSpells.CastQ(t, useQ == 2 ? Q.Range : MenuLocal.Item("Harass.Q.SmallRange").GetValue<Slider>().Value);

            if (MenuLocal.Item("Harass.E").GetValue<StringList>().SelectedIndex == 1)
            {
                Champion.PlayerSpells.CastE(t);
            }
        }
    }
}
