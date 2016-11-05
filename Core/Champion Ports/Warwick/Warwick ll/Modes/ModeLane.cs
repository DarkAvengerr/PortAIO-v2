using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Modes
{
    internal static class ModeLane
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell W => Champion.PlayerSpells.W;

        public static void Initialize(Menu mainMenu)
        {
            MenuLocal = new Menu("Lane", "Lane");
            {
                MenuLocal.AddItem(new MenuItem("Lane.Use.QL", "Q: Last Hit:").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
                MenuLocal.AddItem(new MenuItem("Lane.Use.QH", "Q: Health Prediction:").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());

                string[] strW = new string[6];
                {
                    strW[0] = "Off";
                    for (var i = 1; i < 6; i++)
                    {
                        strW[i] = "If need to AA count >= " + (i + 3);
                    }
                    MenuLocal.AddItem(new MenuItem("Lane.Use.W", "W:").SetValue(new StringList(strW, 4))).SetFontStyle(FontStyle.Regular, W.MenuColor());
                }

                MenuLocal.AddItem(new MenuItem("Lane.Item.Use", "Items:").SetValue(true)).SetFontStyle(FontStyle.Regular, Colors.ColorItems);
                MenuLocal.AddItem(new MenuItem("Lane.MinMana", "Min. Mana: %").SetValue(new Slider(30, 100, 0))).SetFontStyle(FontStyle.Regular, SharpDX.Color.LightSkyBlue).SetTag(2);
            }
            mainMenu.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                ExecuteLaneClear();
            }
        }

        private static void ExecuteLaneClear()
        {
            if (!ModeConfig.MenuFarm.Item("Farm.Enable").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (ObjectManager.Player.ManaPercent < CommonManaManager.MinManaPercent(CommonManaManager.FarmMode.Lane))
            {
                return;
            }
            return;
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);

            if (minions.Count < 1)
            {
                return;
            }

            if (MenuLocal.Item("Lane.Use.QL").GetValue<bool>() && Q.IsReady())
            {
                foreach (var minion in minions.Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                {
                    Champion.PlayerSpells.Cast.Q(minion);
                }
            }

            if (MenuLocal.Item("Lane.Use.QH").GetValue<bool>() && Q.IsReady())
            {
                foreach (
                    var minion in
                        minions.Where(
                            m =>
                                HealthPrediction.GetHealthPrediction(m,
                                    (int)(ObjectManager.Player.AttackCastDelay * 1000), Game.Ping / 2 - 100) < 0)
                            .Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                {
                    Champion.PlayerSpells.Cast.Q(minion);
                }
            }

            if (MenuLocal.Item("Lane.Use.W").GetValue<StringList>().SelectedIndex != 0 && W.IsReady())
            {
                var totalAa =
                    (int)
                        (minions.Where(m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                            .Sum(m => (int) m.Health)/ObjectManager.Player.TotalAttackDamage);

                if (totalAa >= MenuLocal.Item("Jungle.W.Use").GetValue<StringList>().SelectedIndex * 3)
                {
                    Champion.PlayerSpells.Cast.W();
                }
            }
        }
    }
}