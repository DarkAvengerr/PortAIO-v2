using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NoobTeemo
{
    class Program
    {

        public const string ChampionName = "Teemo";

        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;

        static Orbwalking.Orbwalker orbwalker;
        static Menu menu;
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static AIHeroClient Target = null;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Teemo")
            {
                return;
            }
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 600);

            menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            var combo = new Menu("Combo", "Combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));

            var harass = new Menu("Harass", "Harass");
            menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("AutoHarassQ", "Auto Q if enemy is in Range").SetValue(false));

            var ks = new Menu("KillSteal", "KillSteal");
            menu.AddSubMenu(ks);
            ks.AddItem(new MenuItem("useQKS", "Auto Q to KillSteal").SetValue(true));
            menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.OnAttack += OnAa;
            Orbwalking.AfterAttack += AfterAa;
            Chat.Print("NoobTeemo by 1Shinigamix3");
        }
        private static void OnUpdate(EventArgs args)
        {
            AIHeroClient m = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
            if (menu.Item("useQKS").GetValue<bool>())
                if (m.Health < Q.GetDamage(m))
                {
                    Q.Cast(m);
                }
            if (menu.Item("AutoHarassQ").GetValue<bool>())
            {
                Q.Cast(m);
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                W.Cast(m);
            }
        }
        private static void OnAa(AttackableUnit unit, AttackableUnit target)
        {
        }
        private static void AfterAa(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Q.CastOnBestTarget();
            }
        }
    }
}
