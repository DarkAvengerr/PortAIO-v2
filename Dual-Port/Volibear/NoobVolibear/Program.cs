using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NoobVolibear
{
    class Program
    {
        public const string ChampionName = "Volibear";

        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        static Orbwalking.Orbwalker orbwalker;
        static Menu menu;
        static AIHeroClient Player { get { return ObjectManager.Player; } }
        static AIHeroClient Target = null;

        private static Items.Item tiamat;
        private static Items.Item hydra;
        private static Items.Item cutlass;
        private static Items.Item botrk;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Volibear")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 405);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 125);

            menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            //Combo Menu
            var combo = new Menu("Combo", "Combo");
            menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("useR", "Use R").SetValue(true));

            //Lane Clear
            var lc = new Menu("Laneclear", "Laneclear");
            menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearW", "Use W to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));

            //Jungle Clear
            var jungle = new Menu("JungleClear", "JungleClear");
            menu.AddSubMenu(jungle);
            jungle.AddItem(new MenuItem("jungleclearQ", "Use Q to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclearW", "Use W to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclearE", "Use E to JungleClear").SetValue(true));

            hydra = new Items.Item(3074, 185);
            tiamat = new Items.Item(3077, 185);
            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);

            menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            Chat.Print("NoobVolibear by 1Shinigamix3");
        }
        private static void OnUpdate(EventArgs args)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo(TargetSelector.GetTarget(1100, TargetSelector.DamageType.Physical));
            }
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
                Jungle();
            }
            var ksW = HeroManager.Enemies.FindAll(champ => champ.IsValidTarget() && (champ.Health <= ObjectManager.Player.GetSpellDamage(champ, SpellSlot.W)));
            if (ksW.Any())
            {
                W.CastOnUnit(ksW.FirstOrDefault());
            }
        }
        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (menu.Item("useR").GetValue<bool>() && R.IsReady() && args.Target.IsEnemy && args.Target.IsValid<AIHeroClient>())
            {
                R.Cast();
            }
        }
        private static void Combo(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            if (Player.Distance(target) <= Q.Range && Q.IsReady() && (menu.Item("useQ").GetValue<bool>()))
            {
                Q.Cast();
            }
            if (Player.Distance(target) <= E.Range && E.IsReady() && (menu.Item("useE").GetValue<bool>()))
            {
                E.Cast();
            }          
                if (Player.Distance(target) <= hydra.Range)
                {
                    hydra.Cast(target);
                }
                if (Player.Distance(target) <= tiamat.Range)
                {
                    tiamat.Cast(target);
                }
                if (Player.Distance(target) <= botrk.Range)
                {
                    botrk.Cast(target);
                }
                if (Player.Distance(target) <= cutlass.Range)
                {
                    cutlass.Cast(target);
                }
        }
        private static void Lane()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
                if (menu.Item("laneclearW").GetValue<bool>() && W.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            W.CastOnUnit(minion);
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
            var allMinions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
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

                if (menu.Item("jungleclearW").GetValue<bool>() && W.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget())
                        {
                            W.CastOnUnit(minion);
                        }
                    }
                }
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
            
        }
    }
}
