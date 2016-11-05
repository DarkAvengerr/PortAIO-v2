using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy;
using LeagueSharp.Common;
namespace NoobFizz
{
    class Program
    {
        public const string ChampionName = "Fizz";
        public static AIHeroClient Player => ObjectManager.Player;
        public static List<Obj_AI_Base> MinionList;
        public static Orbwalking.Orbwalker Orbwalker;
        //Menu
        public static Menu Menu;
        //Spells
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        private static Items.Item tiamat;
        private static Items.Item hydra;
        private static Items.Item cutlass;
        private static Items.Item botrk;
        private static Items.Item hextech;

        private static Obj_AI_Base Target;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Fizz") return;

            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 1300);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //Combo Menu
            var combo = new Menu("Combo", "Combo");
            Menu.AddSubMenu(combo);
            combo.AddItem(new MenuItem("ComboMode", "ComboMode").SetValue(new StringList(new[] { "R after Dash", "R on Dash", "R to gapclose" })));
            combo.AddItem(new MenuItem("Combo", "Combo"));
            combo.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            //Harass Menu
            var harass = new Menu("Harass", "Harass");
            Menu.AddSubMenu(harass);
            harass.AddItem(new MenuItem("useharassQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("useharassW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("useharassE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("harassmana", "Min Harass Mana").SetValue(new Slider(30)));
            //LaneClear Menu
            var lc = new Menu("Laneclear", "Laneclear");
            Menu.AddSubMenu(lc);
            lc.AddItem(new MenuItem("laneclearQ", "Use Q to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearW", "Use W to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("laneclearE", "Use E to LaneClear").SetValue(true));
            lc.AddItem(new MenuItem("lanemana", "Min Farm Mana").SetValue(new Slider(30)));
            //JungleClear Menu
            var jungle = new Menu("JungleClear", "JungleClear");
            Menu.AddSubMenu(jungle);
            jungle.AddItem(new MenuItem("jungleclearQ", "Use Q to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclearW", "Use W to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclearE", "Use E to JungleClear").SetValue(true));
            jungle.AddItem(new MenuItem("junglemana", "Min Jungle Mana").SetValue(new Slider(30)));
            //Misc Menu
            var miscMenu = new Menu("Misc", "Misc");
            Menu.AddSubMenu(miscMenu);
            miscMenu.AddItem(new MenuItem("drawQ", "Draw Q range").SetValue(false));
            miscMenu.AddItem(new MenuItem("drawAa", "Draw Autoattack range").SetValue(false));
            miscMenu.AddItem(new MenuItem("Killsteal", "Killsteal with Q").SetValue(true));
            miscMenu.AddItem(new MenuItem("Flee", "Flee Key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            miscMenu.AddItem(new MenuItem("useFleeE", "Use E to Flee").SetValue(true));

            hydra = new Items.Item(3074, 185);
            tiamat = new Items.Item(3077, 185);
            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            hextech = new Items.Item(3146, 700);
            Menu.AddToMainMenu();

            OnSpellCast();
            Game.OnUpdate += OnUpdate;
            //Orbwalking.AfterAttack += AfterAa;
            Drawing.OnDraw += OnDraw;
            Chat.Print("<font color='#00CC83'>Noob</font> <font color='#B6250B'>Fizz</font><font color='#00B4D2'>Loaded</font>");
            Chat.Print("<font color='#00B4D2'>Don't forget to upvote if you like NoobFizz! </font>");
        }
        private static void OnSpellCast()
        {
            Obj_AI_Base.OnSpellCast += (sender, args) =>
            {
                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    var useE = (Menu.Item("useE").GetValue<bool>() && E.IsReady());
                    var useR = (Menu.Item("useR").GetValue<bool>() && R.IsReady());
                    var ondash = (Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 1);
                    var afterdash = (Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 0);
                    var target = (AIHeroClient)args.Target;
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        if (ondash)
                        {
                            if (useE && !R.IsReady() && E.Instance.Name == "FizzJump" && Player.Distance(target.Position) < E.Range) E.Cast(Game.CursorPos);
                        }
                        if (afterdash)
                        {
                            if (useR) R.CastOnUnit(target);
                        }
                    }
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    {
                        if ((Menu.Item("useharassE").GetValue<bool>() && E.IsReady()) && !W.IsReady() && !Q.IsReady()) E.Cast(Game.CursorPos);
                    }
                }
            };
        }
        private static void OnDraw(EventArgs args)
        {
            if (Menu.Item("drawQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.DarkRed, 3);
            }
            if (Menu.Item("drawAa").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(Player), System.Drawing.Color.Blue);
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            if (Menu.Item("Killsteal").GetValue<bool>())
            {
                Killsteal();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Lane();
                Jungle();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                Flee();
            }
        }
        private static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (Menu.Item("useFleeE").GetValue<bool>() && E.IsReady())
            {
                E.Cast(Game.CursorPos);
            }
        }
        //R usage
        public static void UseTr(AIHeroClient target)
        {
            var castPosition = R.GetPrediction(target).CastPosition;
            castPosition = Player.ServerPosition.Extend(castPosition, R.Range);

            R.Cast(castPosition);
        }
        //Lane&JungleClear
        private static void Lane()
        {
            if (ObjectManager.Player.ManaPercent < Menu.Item("lanemana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Menu.Item("laneclearQ").GetValue<bool>() && Q.IsReady())
            {
                MinionList = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                foreach (var minion in MinionList)
                {
                    Q.CastOnUnit(minion);
                }
            }
            if (Menu.Item("laneclearW").GetValue<bool>() && W.IsReady())
            {
                var allMinionsW = MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).ToList();
                foreach (var minion in allMinionsW)
                {
                    W.Cast(minion);
                }
            }
            if (Menu.Item("laneclearE").GetValue<bool>() && E.Instance.Name == "FizzJump" && E.IsReady())
            {
                var allMinionsE = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).ToList();
                foreach (var minion in allMinionsE)
                {
                    E.Cast(minion);
                }
            }
        }
        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < Menu.Item("junglemana").GetValue<Slider>().Value)
            {
                return;
            }
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (!mobs.Any())
                return;
            var mob = mobs.First();

            if (Menu.Item("jungleclearQ").GetValue<bool>() && Q.IsReady() && mob.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(mob);
            }
            if (Menu.Item("jungleclearW").GetValue<bool>() && W.IsReady() && mob.IsValidTarget(W.Range))
            {
                W.Cast(mob);
            }
            if (Menu.Item("jungleclearE").GetValue<bool>() && E.IsReady() && mob.IsValidTarget(E.Range))
            {
                E.Cast(mob.ServerPosition);
            }
        }
        private static void Harass()
        {
            var useQ = (Menu.Item("useharassQ").GetValue<bool>() && Q.IsReady());
            var useW = (Menu.Item("useharassW").GetValue<bool>() && W.IsReady());
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (ObjectManager.Player.ManaPercent < Menu.Item("harassmana").GetValue<Slider>().Value)
            {
                return;
            }
            if (useW && (Player.Distance(target.Position) < Q.Range)) W.Cast();
            if (useQ && Player.Distance(target.Position) > 175) Q.CastOnUnit(target);

        }
        /* private static void AfterAa(AttackableUnit unit, AttackableUnit target)
         {
             var useE = (Menu.Item("useE").GetValue<bool>() && E.IsReady());
             var useR = (Menu.Item("useR").GetValue<bool>() && R.IsReady());
             var ondash = (Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 1);
             var afterdash = (Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 0);
             var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

             if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
             {
                 if (ondash)
                 {
                     if (useE && !R.IsReady() && E.Instance.Name == "FizzJump" && Player.Distance(m.Position) < E.Range) E.Cast(Game.CursorPos);
                 }
                 if (afterdash)
                 {
                     if (useR) R.CastOnUnit(m);
                 }
             }
             if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
             {
                 if ((Menu.Item("useharassE").GetValue<bool>() && E.IsReady()) && !W.IsReady() && !Q.IsReady()) E.Cast(Game.CursorPos);
             }
         }*/
        private static void Combo()
        {
            var useQ = (Menu.Item("useQ").GetValue<bool>() && Q.IsReady());
            var useW = (Menu.Item("useW").GetValue<bool>() && W.IsReady());
            var useE = (Menu.Item("useE").GetValue<bool>() && E.IsReady());
            var useR = (Menu.Item("useR").GetValue<bool>() && R.IsReady());
            var gapclose = (Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 2);
            var ondash = (Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 1);
            var afterdash = (Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex == 0);
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var target = TargetSelector.GetTarget(700, TargetSelector.DamageType.Magical);
            if (m != null && Player.Distance(m) <= botrk.Range)
            {
                botrk.Cast(m);
            }
            if (m != null && Player.Distance(m) <= cutlass.Range)
            {
                cutlass.Cast(m);
            }
            if (m != null && Player.Distance(m) <= hextech.Range)
            {
                hextech.Cast(m);
            }
            if (ondash)
            {
                if (useQ && Player.Distance(m.Position) > 175) Q.CastOnUnit(m);
                if (useW && (Player.Distance(m.Position) < 551)) W.Cast();
                if (useR && m.HealthPercent > 30) R.CastIfWillHit(m);
                if (hydra.IsOwned() && Player.Distance(m) < hydra.Range && hydra.IsReady() && !E.IsReady()) hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(m) < tiamat.Range && tiamat.IsReady() && !E.IsReady()) tiamat.Cast();
            }
            if (afterdash)
            {
                if (useW && Player.Distance(m.Position) < Q.Range) W.Cast();
                if (useQ && Player.Distance(m.Position) > 175) Q.CastOnBestTarget();
                if (useE && !R.IsReady() && E.Instance.Name == "FizzJump" && Player.Distance(m.Position) < E.Range) E.Cast(Game.CursorPos);
                if (hydra.IsOwned() && Player.Distance(m) < hydra.Range && hydra.IsReady() && !E.IsReady()) hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(m) < tiamat.Range && tiamat.IsReady() && !E.IsReady()) tiamat.Cast();
            }
            if (gapclose)
            {
                if (useR) R.CastIfWillHit(target);
                if (useQ && Player.Distance(m.Position) > 175) Q.CastOnUnit(m);
                if (useW && Player.Distance(m.Position) < Q.Range) W.Cast();
                if (useE && !R.IsReady() && E.Instance.Name == "FizzJump" && Player.Distance(m.Position) < E.Range) E.Cast(Game.CursorPos);
                if (hydra.IsOwned() && Player.Distance(m) < hydra.Range && hydra.IsReady() && !E.IsReady()) hydra.Cast();
                if (tiamat.IsOwned() && Player.Distance(m) < tiamat.Range && tiamat.IsReady() && !E.IsReady()) tiamat.Cast();
            }
        }
        private static void Killsteal()
        {
            var targetQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (targetQ != null && targetQ.Health < Q.GetDamage(targetQ) && Q.IsReady())
            {
                Q.Cast(targetQ);
            }
            if (targetR != null && targetR.Health < R.GetDamage(targetR) && R.IsReady())
            {
                UseTr(targetR);
            }
        }
    }
}
