using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NoobPantheon
{
    class Program
    {
        public const string ChampionName = "Pantheon";

        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;

        static Orbwalking.Orbwalker orbwalker;
        static Menu menu;
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static AIHeroClient Target = null;

        private static Items.Item tiamat = new Items.Item(3077, 185);
        private static Items.Item hydra = new Items.Item(3074, 185);

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Pantheon")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 700);

            menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            var combo = new Menu("Combo", "Combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));

            var lc = new Menu("Laneclear", "Laneclear");
            menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));

            var harass = new Menu("Harass", "Harass");
            menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("HarassQ", "Use Q to Harass").SetValue(true));
            harass.AddItem(new MenuItem("HarassW", "Use W to Harass").SetValue(true));
            harass.AddItem(new MenuItem("HarassE", "Use E to Harass").SetValue(true));
            harass.AddItem(new MenuItem("AutoHarassQ", "Auto Q if enemy is in Range").SetValue(false));

            var ks = new Menu("KillSteal", "KillSteal");
            menu.AddSubMenu(ks);
            ks.AddItem(new MenuItem("useQKS", "Auto Q to KillSteal").SetValue(true));

            hydra = new Items.Item((int)ItemId.Ravenous_Hydra_Melee_Only, 250);
            tiamat = new Items.Item((int)ItemId.Tiamat_Melee_Only, 250);
            menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.OnAttack += OnAa;
            Orbwalking.AfterAttack += AfterAa;
            Chat.Print("NoobPantheon by 1Shinigamix3");
        }
        private static void OnUpdate(EventArgs args)
        {
            AIHeroClient m = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
            if (menu.Item("useQKS").GetValue<bool>())
                if (m.Health < Q.GetDamage(m))
                {
                    Q.Cast(m);
                }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (menu.Item("useQ").GetValue<bool>())
                    if (Player.Distance(m.Position) > 150)
                        Q.Cast(m);
                if (menu.Item("useW").GetValue<bool>())
                    if (Player.Distance(m.Position) > 150)
                        W.Cast(m);
            }
            if (menu.Item("AutoHarassQ").GetValue<bool>())
            {
                    Q.CastOnBestTarget();
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (menu.Item("HarassQ").GetValue<bool>())
                    if (Player.Distance(m.Position) > 150)
                        Q.CastOnBestTarget();
            }
        }
        private static void OnAa(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (hydra.IsOwned() && Player.Distance(target) < hydra.Range && hydra.IsReady() && !W.IsReady())
                    hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(target) < tiamat.Range && tiamat.IsReady() && !W.IsReady())
                    tiamat.Cast();
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (hydra.IsOwned() && Player.Distance(target) < hydra.Range && hydra.IsReady() && !W.IsReady())
                    hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(target) < tiamat.Range && tiamat.IsReady() && !W.IsReady())
                    tiamat.Cast();
            }
        }
        private static void AfterAa(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (menu.Item("useE").GetValue<bool>() && !hydra.IsReady() || !tiamat.IsReady())
                    E.CastOnBestTarget();
                if (menu.Item("useQ").GetValue<bool>())
                    Q.CastOnBestTarget();
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                AIHeroClient o = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
                if (menu.Item("HarassW").GetValue<bool>())
                    if (Player.Distance(o.Position) > 150)
                        W.Cast(o);
                if (menu.Item("HarassE").GetValue<bool>() && !W.IsReady() && !hydra.IsReady() || !tiamat.IsReady())
                    E.CastOnBestTarget();
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Obj_AI_Base minion = MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();
                if (menu.Item("laneclearE").GetValue<bool>())
                    E.Cast(minion);
                if (menu.Item("laneclearQ").GetValue<bool>() && !E.IsReady())
                    Q.Cast(minion);
                if (hydra.IsOwned() && Player.Distance(minion) < hydra.Range && hydra.IsReady())
                    hydra.Cast(minion);
                if (tiamat.IsOwned() && Player.Distance(minion) < tiamat.Range && tiamat.IsReady())
                    tiamat.Cast(minion);
            }
        }
    }
}
