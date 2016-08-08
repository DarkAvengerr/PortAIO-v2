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
    internal static class ModeLane
    {
        public static Menu MenuLocal { get; private set; }
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell W => Champion.PlayerSpells.W;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;

        public static void Init(Menu mainMenu)
        {
            MenuLocal = new Menu("Lane", "Lane");
            {
                string[] strQ = new string[6];
                {
                    strQ[0] = "Off";
                    strQ[1] = "Just for out of AA range";
                    for (var i = 2; i < 6; i++)
                    {
                        strQ[i] = "Minion Count >= " + i;
                    }
                    MenuLocal.AddItem(new MenuItem("Lane.UseQ", "Q:").SetValue(new StringList(strQ, 1))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
                    MenuLocal.AddItem(new MenuItem("Lane.UseQ.Mode", "Q: Cast Mode:").SetValue(new StringList(new[] {"Cast for Hit", "Cast for Kill"}, 1))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
                }

                string[] strW = new string[6];
                {
                    strW[0] = "Off";
                    for (var i = 1; i < 6; i++)
                    {
                        strW[i] = "If need to AA count >= " + (i + 3);
                    }
                    MenuLocal.AddItem(new MenuItem("Lane.UseW", "W:").SetValue(new StringList(strW, 1))).SetFontStyle(FontStyle.Regular, W.MenuColor());
                }

                MenuLocal.AddItem(new MenuItem("Lane.UseE", "E:").SetValue(new StringList(new[] { "Off", "On: Last hit", "On: Health Prediction", "Both" }, 3))).SetFontStyle(FontStyle.Regular, E.MenuColor());

                MenuLocal.AddItem(new MenuItem("Lane.Item", "Items:").SetValue(new StringList(new[] { "Off", "On" }, 1))).SetFontStyle(FontStyle.Regular, Colors.ColorItems);
            }
            mainMenu.AddSubMenu(MenuLocal);

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {

            
            if (ModeConfig.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Execute(); 
            }
        }

        private static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < CommonManaManager.LaneMinManaPercent || !ModeConfig.MenuFarm.Item("Farm.Enable").GetValue<KeyBind>().Active)
            {
                return;
            }
         
            if (Q.LSIsReady() && MenuLocal.Item("Lane.UseQ").GetValue<StringList>().SelectedIndex != 0)
            {
                var qCount = MenuLocal.Item("Lane.UseQ").GetValue<StringList>().SelectedIndex;

                var objAiHero = from x1 in ObjectManager.Get<Obj_AI_Minion>()
                                where x1.LSIsValidTarget() && x1.IsEnemy
                                select x1
                                     into h
                                orderby h.LSDistance(ObjectManager.Player) descending
                                select h
                                         into x2
                                where x2.LSDistance(ObjectManager.Player) < Q.Range - 20 && !x2.IsDead
                                select x2;

                var aiMinions = objAiHero as Obj_AI_Minion[] ?? objAiHero.ToArray();

                var lastMinion = aiMinions.First();

                var qMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, ObjectManager.Player.LSDistance(lastMinion.Position));

                if (qMinions.Count > 0)
                {
                    var locQ = Q.GetLineFarmLocation(qMinions, Q.Width);

                    if (qMinions.Count == qMinions.Count(m => ObjectManager.Player.LSDistance(m) < Q.Range) && locQ.MinionsHit >= qCount && locQ.Position.LSIsValid())
                    {
                        Q.Cast(lastMinion.Position);
                    }
                }
            }

            if (MenuLocal.Item("Lane.UseW").GetValue<StringList>().SelectedIndex != 0 && W.LSIsReady())
            {
                var wCount = MenuLocal.Item("Lane.UseW").GetValue<StringList>().SelectedIndex;

                var totalAa =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            m =>
                                m.IsEnemy && !m.IsDead &&
                                m.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null)))
                        .Sum(mob => (int)mob.Health);

                totalAa = (int)(totalAa / ObjectManager.Player.TotalAttackDamage);
                if (totalAa >= wCount + 3)
                {
                    W.Cast();
                }
            }

            var useE = MenuLocal.Item("Lane.UseE").GetValue<StringList>().SelectedIndex;
            if (useE != 0 && E.LSIsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                if (useE == 1 || useE == 3)
                {
                    foreach (
                        var eMinion in
                            minions.Where(m => m.Health < E.GetDamage(m) && E.CanCast(m)))
                    {
                        E.CastOnUnit(eMinion);
                    }
                }

                if (useE == 2 || useE == 3)
                {
                    foreach (
                        var eMinion in
                            minions.Where(
                                m =>
                                    HealthPrediction.GetHealthPrediction(m,
                                        (int) (ObjectManager.Player.AttackCastDelay*1000), Game.Ping/2) < 0)
                                .Where(m => m.Health < E.GetDamage(m) && E.CanCast(m)))
                    {
                        E.CastOnUnit(eMinion);
                    }
                }
            }
        }
    }
}
