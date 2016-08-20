using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Rammus
{
    class Program
    {
        public const string ChampionName = "Rammus";
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Orbwalking.Orbwalker Orbwalker;
        //Menu
        public static Menu Menu;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public static void Main()
        {
            Game_OnGameLoad();
        }
        private static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Rammus") return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R, 300);

            Menu = new Menu("Rammus by Busky", "Rammus by Busky", true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu TS = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(TS);

            var Combo = new Menu("Combo", "Combo");
            Menu.AddSubMenu(Combo);
            Combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            Combo.AddItem(new MenuItem("Gapclose", "Anti-Gapclose with Q").SetValue(true));
            Combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            Combo.AddItem(new MenuItem("ESelected", "Only E Selected Target").SetValue(true));
            Combo.AddItem(new MenuItem("Interrupt", "Interrupt with E").SetValue(true));
            Combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            Combo.AddItem(new MenuItem("NumberOfEnemys", "Minimum Enemies to Use Ult").SetValue(new Slider(1, 5, 0)));

            var JungleClear = new Menu("JungleClear", "JungleClear");
            Menu.AddSubMenu(JungleClear);
            JungleClear.AddItem(new MenuItem("JungleClearQ", "Use Q").SetValue(true));
            JungleClear.AddItem(new MenuItem("JungleClearW", "Use W").SetValue(true));
            JungleClear.AddItem(new MenuItem("JungleClearE", "Use E").SetValue(true));
            JungleClear.AddItem(new MenuItem("JungleMana", "Minimum Jungle Mana").SetValue(new Slider(0)));

            var Misc = new Menu("Misc", "Misc");
            Menu.AddSubMenu(Misc);
            Misc.AddItem(new MenuItem("Flee", "Flee Key").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
            Misc.AddItem(new MenuItem("FleeQ", "Use Q to Flee").SetValue(true));
            Misc.AddItem(new MenuItem("EscapeW", "Automatically use W at X HP").SetValue(new Slider(10)));


            Menu.AddToMainMenu();
            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Chat.Print("Rammus by Busky");
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear();
            }
            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                Flee();
            }
            float healthPercent = Player.Health / Player.MaxHealth * 100;

            int sliderValue = Menu.Item("EscapeW").GetValue<Slider>().Value;

            if (healthPercent < sliderValue && ObjectManager.Player.ManaPercent > 20)
            {
                W.Cast();
            }
        }
        private static void Combo()
        {
            if (Menu.Item("useQ").GetValue<bool>() && Q.IsReady() && !Player.HasBuff("PowerBall") && !Player.HasBuff("DefensiveBallCurl"))
            {
                var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

                if (Player.Distance(target.Position) > 150)
                {
                    Q.Cast(target);
                }
            }
            if (Menu.Item("useW").GetValue<bool>() && W.IsReady())
            {
                var target = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);    
                W.Cast(target);
            }
            if (Menu.Item("useE").GetValue<bool>() && !Player.HasBuff("PowerBall") && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);    
                E.CastOnUnit(target);
            }

            if (Menu.Item("ESelected").GetValue<bool>() && Menu.Item("useE").GetValue<bool>())
            {
                var target = TargetSelector.GetSelectedTarget();
                {
                    if (E.IsReady() && !Player.HasBuff("DefensiveBallCurl") && target.IsValidTarget() && !target.IsZombie && (Player.Distance(target.Position) > 500))
                    {
                        E.CastOnUnit(target);
                    }
                }
            }

            if (R.IsReady())
            {
                var valR = Menu.Item("NumberOfEnemys").GetValue<Slider>().Value;
                if (valR > 0 && Player.CountEnemiesInRange(R.Range) >= valR)
                {
                    R.Cast();
                }
            }
        }
        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;
            if (Menu.Item("Gapclose").GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("Interrupt").GetValue<bool>() && E.IsReady() && E.IsReady() && sender.IsValidTarget(E.Range))
            {
                E.CastOnUnit(sender);
            }
        }

        private static void JungleClear()
        {
            if (ObjectManager.Player.ManaPercent < Menu.Item("JungleMana").GetValue<Slider>().Value)
            {
                return;
            }
            var mobs = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (!mobs.Any())
                return;
            var mob = mobs.First();

            if (Menu.Item("JungleClearQ").GetValue<bool>() && Q.IsReady() && mob.IsValidTarget(Q.Range) && !Player.HasBuff("PowerBall") && !Player.HasBuff("DefensiveBallCurl"))
            {
                Q.Cast();
            }
            if (Menu.Item("JungleClearE").GetValue<bool>() && E.IsReady() && mob.IsValidTarget(E.Range))
            {
                E.Cast(mob);
            }
            if (Menu.Item("JungleClearW").GetValue<bool>() && W.IsReady() && mob.IsValidTarget(W.Range))
            {
                W.Cast();
            }
        }
        private static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (Menu.Item("FleeQ").GetValue<bool>() && Q.IsReady())
            {
                if (!Player.HasBuff("PowerBall"))
                {
                    Q.Cast();
                }
            }
        }
    }
}