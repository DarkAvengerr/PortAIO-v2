using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NoobXin_Zhao
{
    class Program
    {
        public const string ChampionName = "XinZhao";

        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        static Orbwalking.Orbwalker orbwalker;
        static Menu menu;
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static AIHeroClient Target = null;

        private static Items.Item tiamat = new Items.Item(3077, 185);
        private static Items.Item hydra = new Items.Item(3074, 185);
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "XinZhao")
            {
                return;
            }
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 480);

            menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            var combo = new Menu("Combo", "Combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("useR", "Use R to kill").SetValue(true));

            var lc = new Menu("Laneclear", "Laneclear");
            menu.AddSubMenu(lc); 
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearW", "Use W to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));

            hydra = new Items.Item((int)ItemId.Ravenous_Hydra_Melee_Only, 250);
            tiamat = new Items.Item((int)ItemId.Tiamat_Melee_Only, 250);
            menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.OnAttack += OnAa;
            Orbwalking.AfterAttack += AfterAa;
            Chat.Print("NoobXin Zhao by 1Shinigamix3");
        }

        private static void OnUpdate(EventArgs args)
        {
            AIHeroClient m = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
            if (menu.Item("useR").GetValue<bool>())
                if (m.Health < R.GetDamage(m))
                {
                    R.Cast(m);
                }      
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                AIHeroClient o = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
                if (menu.Item("useE").GetValue<bool>()) if (Player.Distance(o.Position) > 175) E.Cast(o);
            }           
        }

        private static
            void OnAa(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                AIHeroClient o = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
                if (menu.Item("useW").GetValue<bool>())
                        W.Cast(o);              
            }
        }
        private static void AfterAa(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (menu.Item("useQ").GetValue<bool>())
                {
                    Q.Cast();
                }
                if (hydra.IsOwned() && Player.Distance(target) < hydra.Range && hydra.IsReady() && !W.IsReady())
                    hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(target) < tiamat.Range && tiamat.IsReady() && !W.IsReady())
                    tiamat.Cast();
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Obj_AI_Base minion = MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();
                //Obj_AI_Base minion = MinionManager.GetMinions(Player.Position, 600).FirstOrDefault();
                if (menu.Item("laneclearE").GetValue<bool>())
                    E.Cast(minion);
                if (menu.Item("laneclearQ").GetValue<bool>())
                    Q.Cast(minion);
                if (menu.Item("laneclearW").GetValue<bool>())
                    W.Cast(minion);
                if (hydra.IsOwned() && Player.Distance(minion) < hydra.Range && hydra.IsReady() && !W.IsReady())
                    hydra.Cast(minion);
                if (tiamat.IsOwned() && Player.Distance(minion) < tiamat.Range && tiamat.IsReady() && !W.IsReady())
                    tiamat.Cast(minion);
            }
        }

    }
}
