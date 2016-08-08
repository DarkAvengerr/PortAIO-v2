using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using EloBuddy;

namespace Irelia.Modes
{
    internal static class ModeFlee
    {
        public static Menu MenuLocal { get; private set; }

        public static void Init(Menu ParentMenu)
        {
            MenuLocal = new Menu("Flee", "Flee");
            {
                MenuLocal.AddItem(new MenuItem("Flee.UseQ", "Q:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()));
                MenuLocal.AddItem(new MenuItem("Flee.Youmuu", "Item Youmuu:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()));
                MenuLocal.AddItem(new MenuItem("Flee.DrawMouse", "Draw Mouse Position:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor()));
            }
            ParentMenu.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += delegate(EventArgs args)
            {
                if (!ModeConfig.MenuKeys.Item("Key.Flee").GetValue<KeyBind>().Active)
                {
                    return;
                }

                if (MenuLocal.Item("Flee.DrawMouse").GetValue<StringList>().SelectedIndex == 1)
                {
                    Render.Circle.DrawCircle(Game.CursorPos, 150f, System.Drawing.Color.Red);
                }
            };
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!ModeConfig.MenuKeys.Item("Key.Flee").GetValue<KeyBind>().Active)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            ModeConfig.Orbwalker.SetAttack(!ModeConfig.MenuKeys.Item("Key.Flee").GetValue<KeyBind>().Active);
            

            var t = TargetSelector.GetTarget(Champion.PlayerSpells.Q.Range, TargetSelector.DamageType.Physical);
            if (t.LSIsValidTarget())
            {
                if (MenuLocal.Item("Flee.UseQ").GetValue<StringList>().SelectedIndex == 1 && Champion.PlayerSpells.Q.LSIsReady())
                {
                    Champion.PlayerSpells.CastQCombo(t);
                }

                if (MenuLocal.Item("Flee.Youmuu").GetValue<StringList>().SelectedIndex == 1 && Common.CommonItems.Youmuu.IsReady())
                {
                    Common.CommonItems.Youmuu.Cast();
                }
            }
        }
    }
}
