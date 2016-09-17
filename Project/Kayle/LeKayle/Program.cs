using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LeKayle
{
    class Program
    {

        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, R;

        private static Menu Menu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Kayle")
                return;

            Notifications.AddNotification("Kayle Loaded!", 10000);

            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 525);
            R = new Spell(SpellSlot.R, 900);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));

            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));

            TargetSelector.AddToMenu(ts);

            Menu comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));

            comboMenu.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboE", "Use E").SetValue(true));

            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Menu harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));

            harassMenu.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassW", "Use W").SetValue(false));
            harassMenu.AddItem(new MenuItem("harassE", "Use E").SetValue(true));

            harassMenu.AddItem(
                new MenuItem("Harass", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            Menu laneclearMenu = Menu.AddSubMenu(new Menu("Lane Clear", "Lane Clear"));

            laneclearMenu.AddItem(new MenuItem("laneclearQ", "Use Q").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("laneclearE", "Use E").SetValue(true));

            laneclearMenu.AddItem(
                new MenuItem("Lane Clear", "Lane Clear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Menu healMenu = Menu.AddSubMenu(new Menu("Auto Heal", "Auto Heal"));

            healMenu.AddItem(new MenuItem("healMe", "Use W On Self").SetValue(true));
            healMenu.AddItem(new MenuItem("healMeHP", "Use W On Self %")).SetValue(new Slider(70, 1, 100));
            healMenu.AddItem(new MenuItem("healAlly", "Use W On Ally").SetValue(true));
            healMenu.AddItem(new MenuItem("healAllyHP", "Use W On Ally %")).SetValue(new Slider(65, 1, 100));

            Menu ultMenu = Menu.AddSubMenu(new Menu("Ultimate", "Ultimate"));

            ultMenu.AddItem(new MenuItem("useRme", "Use R On Self").SetValue(true));
            ultMenu.AddItem(new MenuItem("useRmeHP", "Use R On Self %")).SetValue(new Slider(25, 1, 100));
            ultMenu.AddItem(new MenuItem("useRally", "Use R On Ally").SetValue(true));
            ultMenu.AddItem(new MenuItem("useRallyHP", "Use R On Ally %")).SetValue(new Slider(20, 1, 100));

            Menu ksMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));

            ksMenu.AddItem(new MenuItem("ksQ", "Use Q").SetValue(true));
            ksMenu.AddItem(new MenuItem("ksE", "Use E").SetValue(true));

            Menu fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));

            fleeMenu.AddItem(new MenuItem("fleeW", "Use W").SetValue(true));

            fleeMenu.AddItem(
                new MenuItem("Flee", "Flee").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Menu drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));

            drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));

            Menu.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                comboW();
                comboQ();
                comboE();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harassW();
                harassQ();
                harassE();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                clearQ();
                clearE();
            }

            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                flee();
            }

            if (Menu.Item("useRme").GetValue<bool>())
            {
                useRme();
            }

            if (Menu.Item("useRally").GetValue<bool>())
            {
                useRally();
            }

            if (Menu.Item("healMe").GetValue<bool>())
            {
                healMe();
            }

            if (Menu.Item("healAlly").GetValue<bool>())
            {
                healAlly();
            }

            if (Menu.Item("ksQ").GetValue<bool>())
            {
                ksQ();
            }

            if (Menu.Item("ksE").GetValue<bool>())
            {
                ksE();
            }
        }

        //Combo

        private static void comboQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Menu.Item("comboQ").GetValue<bool>())
                Q.Cast(target);
        }

        private static void comboW()
        {
            var target = TargetSelector.GetTarget(750, TargetSelector.DamageType.Magical);

            if (W.IsReady() && target.IsValidTarget(750) && Menu.Item("comboW").GetValue<bool>())
                W.Cast(Player);
        }

        private static void comboE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && Menu.Item("comboE").GetValue<bool>())
                E.Cast();
        }

        //Harass

        private static void harassQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Menu.Item("harassQ").GetValue<bool>())
                Q.Cast(target);
        }

        private static void harassW()
        {
            var target = TargetSelector.GetTarget(750, TargetSelector.DamageType.Magical);

            if (W.IsReady() && target.IsValidTarget(750) && Menu.Item("harassW").GetValue<bool>())
                W.Cast(Player);
        }

        private static void harassE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && Menu.Item("harassE").GetValue<bool>())
                E.Cast();
        }

        //Lane Clear

        private static void clearQ()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var farmLocation =
                MinionManager.ReferenceEquals(
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Select(m => m.ServerPosition.To2D())
                        .ToList(),
                    Q.Range);

            if (Menu.Item("laneclearQ").GetValue<bool>() && minion.IsValidTarget() && Q.IsReady())
            {
                Q.Cast(minion);
            }
        }

        private static void clearE()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, E.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            if (Menu.Item("laneclearE").GetValue<bool>() && minion.IsValidTarget() && E.IsReady())
            {
                E.Cast();
            }
        }

        //KillSteal

        private static void ksQ()
        {

            var ksQ = Menu.Item("ksQ").GetValue<bool>();

            if (ksQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (target != null && Q.IsKillable(target) && Q.CastOnUnit(target))
                {
                    return;
                }
            }
        }

        private static void ksE()
        {
            var ksE = Menu.Item("ksE").GetValue<bool>();

            if (ksE && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (target != null && E.IsKillable(target) && E.Cast())
                {
                    return;
                }
            }
        }

        //Flee

        private static void flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (W.IsReady())
            {
                W.Cast(Player);
            }
        }

        //Ultimate

        private static void useRme()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;

            var autoRmeHP = Menu.Item("useRmeHP").GetValue<Slider>().Value;
            var autoRme = Menu.Item("useRme").GetValue<bool>();

            if (autoRme && (Player.Health / Player.MaxHealth) * 100 <= autoRmeHP && R.IsReady()
                && Player.CountEnemiesInRange(750) > 0)
            {
                R.Cast(Player);
            }
        }

        private static void useRally()
        {
            var autoRally = Menu.Item("useRally").GetValue<bool>();
            var RallyHP = Menu.Item("useRallyHP").GetValue<Slider>().Value;

            foreach (var Ally in ObjectManager.Get<AIHeroClient>().Where(Ally => Ally.IsAlly && !Ally.IsMe))
            {
                var allys = Menu.Item("useRally" + Ally.CharData);

                if (Player.InFountain() || Player.IsRecalling())
                    return;

                if (autoRally && ((Ally.Health / Ally.MaxHealth) * 100 <= RallyHP) && R.IsReady() &&
                    Player.CountEnemiesInRange(900) > 0 && (Ally.Distance(Player.Position) <= R.Range))
                {
                    if (allys != null && allys.GetValue<bool>())
                    {
                        R.Cast(Ally);
                    }
                }
            }
        }

        //Auto Heal

        private static void healMe()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;

            var healMe = Menu.Item("healMe").GetValue<bool>();
            var healMeHP = Menu.Item("healMeHP").GetValue<Slider>().Value;

            if (healMe && (Player.Health / Player.MaxHealth) * 100 <= healMeHP && W.IsReady())
            {
                W.Cast(Player);
            }
        }

        private static void healAlly()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;

            var healAlly = Menu.Item("healAlly").GetValue<bool>();
            var healAllyHP = Menu.Item("healAllyHP").GetValue<Slider>().Value;

            foreach (var Ally in ObjectManager.Get<AIHeroClient>().Where(Ally => Ally.IsAlly && !Ally.IsMe))
            {
                var allys = Menu.Item("useRally" + Ally.CharData);

                if (Player.InFountain() || Player.IsRecalling())
                    return;

                if (healAlly && ((Ally.Health / Ally.MaxHealth) * 100 <= healAllyHP) && W.IsReady() &&
                    Ally.Distance(Player.Position) <= W.Range)
                {
                    if (allys != null && allys.GetValue<bool>())
                    {
                        W.Cast(Ally);
                    }
                }
            }

        }

        //Drawings

        private static void Drawing_OnDraw(EventArgs args)
        {
            {
                if (Player.IsDead)
                    return;

                if (Menu.Item("drawQ").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
                }

                if (Menu.Item("drawW").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color.Aqua);
                }

                if (Menu.Item("drawE").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.Aqua);
                }

                if (Menu.Item("drawR").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, Color.Aqua);
                }

            }
        }
    }
}
