using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Vi.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Vi.Modes
{
    internal static class ModeLane
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;

        public static void Init(Menu mainMenu)
        {
            MenuLocal = new Menu("Lane", "Lane");
            {
                MenuLocal.AddItem(new MenuItem("Lane.LaneQuick", "Fast Lane Clear Mode:").SetValue(new KeyBind('T', KeyBindType.Toggle))).SetFontStyle(FontStyle.Regular, SharpDX.Color.DarkKhaki).SetTooltip("Using all spell for fast clear lane. Tip: Use for under ally turret farm").Permashow(true, ObjectManager.Player.ChampionName + " | Quick Lane Clear");

                MenuLocal.AddItem(new MenuItem("Lane.UseQ", "Q Last Hit:").SetValue(new StringList(new[] {"Off", "On"}, 1))).SetFontStyle(FontStyle.Regular, Q.MenuColor());

                MenuLocal.AddItem(new MenuItem("Lane.UseE", "E:").SetValue(new StringList(new[] {"Off", "On: Last hit"}, 1))).SetFontStyle(FontStyle.Regular, E.MenuColor());
                MenuLocal.AddItem(new MenuItem("Lane.Item", "Items:").SetValue(new StringList(new[] {"Off", "On"}, 1))).SetFontStyle(FontStyle.Regular, Colors.ColorItems);
                MenuLocal.AddItem(new MenuItem("Lane.MinMana.Alone", "Min. Mana: I'm Alone %").SetValue(new Slider(30, 100, 0))).SetFontStyle(FontStyle.Regular, SharpDX.Color.LightSkyBlue).SetTag(2);
                MenuLocal.AddItem(new MenuItem("Lane.MinMana.Enemy", "Min. Mana: I'm NOT Alone (Enemy Close) %").SetValue(new Slider(60, 100, 0))).SetFontStyle(FontStyle.Regular, SharpDX.Color.IndianRed).SetTag(2);
                MenuLocal.AddItem(
                    new MenuItem("MinMana.Jungle.Default", "Load Recommended Settings").SetValue(true).SetTag(9))
                    .SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow)
                    .ValueChanged += (sender, args) =>
                    {
                        if (args.GetNewValue<bool>() == true)
                        {
                            LoadDefaultSettings();
                        }
                    };
            }
            mainMenu.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
        }

        public static void LoadDefaultSettings()
        {
            MenuLocal.Item("Lane.LaneQuick").SetValue(new KeyBind('T', KeyBindType.Toggle));
            MenuLocal.Item("Lane.UseQ").SetValue(new StringList(new[] { "Off", "On" }, 1));

            string[] strW = new string[6];
            {
                strW[0] = "Off";
                for (var i = 1; i < 6; i++)
                {
                    strW[i] = "If need to AA count >= " + (i + 3);
                }
                MenuLocal.Item("Lane.UseW").SetValue(new StringList(strW, 4));
            }

            MenuLocal.Item("Lane.UseE").SetValue(new StringList(new[] {"Off", "On: Last hit"}, 1));
            MenuLocal.Item("Lane.Item").SetValue(new StringList(new[] {"Off", "On"}, 1));

            MenuLocal.Item("Lane.MinMana.Alone").SetValue(new Slider(30, 100, 0));
            MenuLocal.Item("Lane.MinMana.Enemy").SetValue(new Slider(60, 100, 0));
        }

        public static float LaneMinManaPercent
        {
            get
            {
                if (ModeConfig.MenuFarm.Item("Farm.MinMana.Enable").GetValue<KeyBind>().Active)
                {
                    return HeroManager.Enemies.Find(e => e.IsValidTarget(2000) && !e.IsZombie) == null
                        ? MenuLocal.Item("Lane.MinMana.Alone").GetValue<Slider>().Value
                        : MenuLocal.Item("Lane.MinMana.Enemy").GetValue<Slider>().Value;
                }

                return 0f;
            }
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

            if (ObjectManager.Player.ManaPercent < LaneMinManaPercent)
            {
                return;
            }

            if (Q.IsReady() && MenuLocal.Item("Lane.UseQ").GetValue<StringList>().SelectedIndex != 0)
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range).Where(m => !m.UnderTurret(true));

                if (ModeConfig.MenuConfig.Item("Lane.LaneQuick").GetValue<KeyBind>().Active)
                {
                    foreach (
                        var minion in
                            minions.Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                    {
                        Champion.PlayerSpells.CastQObjects(minion);
                    }
                }
                else
                {

                    foreach (
                        var minion in
                            minions.Where(
                                m =>
                                    HealthPrediction.GetHealthPrediction(m,
                                        (int) (ObjectManager.Player.AttackCastDelay*1000), Game.Ping/2 - 100) < 0)
                                .Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                    {
                        Champion.PlayerSpells.CastQObjects(minion);
                    }
                }
            }

            if ((MenuLocal.Item("Lane.UseQ").GetValue<StringList>().SelectedIndex == 0 || !Q.IsReady()) && E.IsReady() &&
                MenuLocal.Item("Lane.UseE").GetValue<StringList>().SelectedIndex != 0)
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                foreach (
                    var minion in
                        minions.Where(
                            m =>
                                HealthPrediction.GetHealthPrediction(m,
                                    (int) (ObjectManager.Player.AttackCastDelay*1000), Game.Ping/2 - 100) < 0)
                            .Where(m => m.Health < E.GetDamage(m) - 10 && E.CanCast(m)))
                {
                    Champion.PlayerSpells.CastQObjects(minion);
                }
            }
        }

        private static void ExecuteQuickLaneClear()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (!MenuLocal.Item("Lane.Enable").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (Q.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);

                foreach (
                    var minion in
                        MinionManager.GetMinions(Q.Range)
                            .Where(m => m.CanKillableWith(Q) && Q.CanCast(m)))
                {
                    Champion.PlayerSpells.CastQObjects(minion);
                }
            }

            if (!Q.IsReady() && E.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                foreach (
                    var minion in
                        minions.Where(
                            m =>
                                HealthPrediction.GetHealthPrediction(m,
                                    (int) (ObjectManager.Player.AttackCastDelay*1000), Game.Ping/2 - 100) < 0)
                            .Where(m => m.CanKillableWith(E) && E.CanCast(m)))
                {
                    Champion.PlayerSpells.CastQObjects(minion);
                }
            }
        }
    }
}