using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Olaf.Common;
using Color = SharpDX.Color;
using EloBuddy;

namespace Olaf.Modes
{
    internal static class ModeCombo
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell W => Champion.PlayerSpells.W;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;

        public static SpellSlot IgniteSlot = ObjectManager.Player.LSGetSpellSlot("SummonerDot");
        public static void Init()
        {
            MenuLocal = new Menu("Combo", "Combo").SetFontStyle(FontStyle.Regular, Color.Aqua);
            MenuLocal.AddItem(new MenuItem("Combo.Q", "Q:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.Q.MenuColor())).SetTooltip("Olaf's W / Youmuu", Color.AliceBlue);
            MenuLocal.AddItem(new MenuItem("Combo.W", "W:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.W.MenuColor()));
            MenuLocal.AddItem(new MenuItem("Combo.E", "E:").SetValue(new StringList(new[] { "Off", "On" }, 1)).SetFontStyle(FontStyle.Regular, Champion.PlayerSpells.E.MenuColor()));

            ModeConfig.MenuConfig.AddSubMenu(MenuLocal);
            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
        }

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            {
                return;
            }

            if (!W.LSIsReady() || Modes.ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || MenuLocal.Item("Combo.W").GetValue<StringList>().SelectedIndex == 0)
            {
                return;
            }

            if (Common.CommonHelper.ShouldCastSpell((AIHeroClient) args.Target) && args.Target is AIHeroClient)
            {
                W.Cast();
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            ExecuteCombo();


        }

        private static void ExecuteCombo()
        {
            if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            {
                return;
            }

            var t = CommonTargetSelector.GetTarget(Q.Range);
            if (!t.LSIsValidTarget())
            {
                return;
            }

            if (IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (t.LSIsValidTarget(650) && !t.HaveImmortalBuff() && ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite) + 150 >= t.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, t);
                }
            }

            if (MenuLocal.Item("Combo.Q").GetValue<StringList>().SelectedIndex != 0)
            {
                Champion.PlayerSpells.CastQ(t, Q.Range);
            }

            if (Q.CanCast(t) && MenuLocal.Item("Combo.Q").GetValue<StringList>().SelectedIndex == 1 && t.Health < Q.GetDamage(t))
            {
                Champion.PlayerSpells.CastQ(t, Q.Range);
            }

            if (E.CanCast(t) && MenuLocal.Item("Combo.E").GetValue<StringList>().SelectedIndex == 1)
            {
                Champion.PlayerSpells.CastE(t);
            }
        }
    }
}
