using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy;
using LeagueSharp.Common;
namespace NoobAatrox
{
    class Program
    {
        public const string ChampionName = "Aatrox";

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;

        public static Spell W;

        public static Spell E;

        public static Spell R;

        static Orbwalking.Orbwalker orbwalker;

        static Menu menu;

        static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        static AIHeroClient Target = null;

        private static Items.Item tiamat;
        private static Items.Item hydra;
        private static Items.Item titanic;
        private static Items.Item cutlass;
        private static Items.Item botrk;

        /*private static bool IsW1Used
          {
              get
              {
                  return Player.HasBuff("aatroxwlife");
              }
          }

          private static bool IsW2Used
          {
              get
              {
                  return Player.HasBuff("aatroxwpower");
              }
          }*/
        /* private static bool RisUsed
         {
             get
             {
                 return Player.HasBuff("aatroxR");
             }
         }*/

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Aatrox")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
            E = new Spell(SpellSlot.E, 1075);
            R = new Spell(SpellSlot.R, 550);

            menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            var combo = new Menu("Combo", "Combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("Combo", "Combo"));
            combo.AddItem(new MenuItem("space", ""));
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(false));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("useR", "Use R").SetValue(true));

            var lc = new Menu("Laneclear", "Laneclear");
            menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));

            var jc = new Menu("Laneclear", "Laneclear");
            menu.AddSubMenu(jc);
            jc.AddItem(new MenuItem("jungleclearQ", "Use Q to JungleClear").SetValue(true));
            jc.AddItem(new MenuItem("jungleclearE", "Use E to JungleClear").SetValue(true));

            var ks = new Menu("KillSteal", "KillSteal");
            menu.AddSubMenu(ks);
            ks.AddItem(new MenuItem("useQKS", "Q to KillSteal").SetValue(true));
            ks.AddItem(new MenuItem("useEKS", "E to KillSteal").SetValue(true));

            hydra = new Items.Item(3074, 185);
            tiamat = new Items.Item(3077, 185);
            titanic = new Items.Item(3748, 450);
            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += AfterAa;
            Chat.Print("NoobAatrox by 1Shinigamix3");
        }
        private static void OnUpdate(EventArgs args)
        {
            AIHeroClient m = TargetSelector.GetTarget(650, TargetSelector.DamageType.Physical);
            if (menu.Item("useQKS").GetValue<bool>())
                if (m.Health < Q.GetDamage(m))
                {
                    Q.CastIfHitchanceEquals(m, HitChance.High); ;
                }
            if (menu.Item("useEKS").GetValue<bool>())
                if (m.Health < E.GetDamage(m))
                {
                    E.Cast(m);
                }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                //itemusage
                if (Player.Distance(m) <= botrk.Range)
                {
                    botrk.Cast(m);
                }
                if (Player.Distance(m) <= cutlass.Range)
                {
                    cutlass.Cast(m);
                }
                //Combo
                /*if (menu.Item("useE").GetValue<bool>() && E.IsReady())
                {
                    E.Cast(m);
                }*/
                if (menu.Item("useQ").GetValue<bool>() && Q.IsReady() && !E.IsReady())
                {
                    Q.Cast(m);
                }
                if (menu.Item("useW").GetValue<bool>() && W.IsReady())
                {
                    W.Cast();
                }
                if (menu.Item("useR").GetValue<bool>() && R.IsReady() && !Q.IsReady())
                {
                    R.Cast();
                }
                if (menu.Item("useE").GetValue<bool>() && E.IsReady() && !Q.IsReady() && !hydra.IsReady() && !tiamat.IsReady() && !titanic.IsReady())
                {
                    E.Cast(m);
                }
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
                Jungle();
            }
        }
        private static void AfterAa(AttackableUnit unit, AttackableUnit target)
        {
            AIHeroClient m = TargetSelector.GetTarget(1075, TargetSelector.DamageType.Physical);
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (hydra.IsOwned() && Player.Distance(m) < hydra.Range && hydra.IsReady() && !Q.IsReady()) hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(m) < tiamat.Range && tiamat.IsReady() && !Q.IsReady()) tiamat.Cast();
                if (titanic.IsOwned() && Player.Distance(m) < titanic.Range && titanic.IsReady() && !Q.IsReady()) titanic.Cast();
            }
        }
        //Lane&JungleClear
        private static void Lane()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 600);
            if (menu.Item("laneclearQ").GetValue<bool>() && Q.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.Cast();
                    }
                }
            }
            if (menu.Item("laneclearE").GetValue<bool>() && E.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        E.Cast();
                    }
                }
            }
        }

        private static void Jungle()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (menu.Item("jungleclearE").GetValue<bool>() && E.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        E.Cast();
                    }
                }
            }
            if (menu.Item("jungleclearQ").GetValue<bool>() && Q.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget())
                    {
                        Q.Cast();
                    }
                }
            }
        }
    }
}
