using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TSM_Teemo
{
    class Program
    {

        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, R;

        private static Menu Menu;

        public static float RRange
        {
            get { return 300 * R.Level; }
        }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Teemo")
                return;

            Q = new Spell(SpellSlot.Q, 680);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 300);

            R.SetSkillshot(0.5f, 120f, 1000f, false, SkillshotType.SkillshotCircle);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));

            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));

            TargetSelector.AddToMenu(ts);

            Menu comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));

            comboMenu.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboR", "Use R").SetValue(true));

            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Menu harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));

            harassMenu.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassW", "Use W").SetValue(false));
            harassMenu.AddItem(new MenuItem("harassR", "Use R").SetValue(false));

            harassMenu.AddItem(
                new MenuItem("Harass", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            Menu clearMenu = Menu.AddSubMenu(new Menu("Lane Clear", "Lane Clear"));

            clearMenu.AddItem(new MenuItem("clearQ", "Use Q").SetValue(false));
            clearMenu.AddItem(new MenuItem("clearR", "Use R").SetValue(false));

            clearMenu.AddItem(
                new MenuItem("Lane Clear", "Lane Clear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Menu fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));

            fleeMenu.AddItem(new MenuItem("fleeQ", "Use Q").SetValue(false));
            fleeMenu.AddItem(new MenuItem("fleeW", "Use W").SetValue(true));
            fleeMenu.AddItem(new MenuItem("fleeR", "Use R").SetValue(true));

            fleeMenu.AddItem(
                new MenuItem("Flee", "Flee").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Menu ksMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));

            ksMenu.AddItem(new MenuItem("ksQ", "Use Q").SetValue(true));

            Menu drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));

            drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawShrooms", "Draw Shroom Spots").SetValue(true));

            Menu.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnUpdate;

            Notifications.AddNotification("Teemo Loaded!", 10000);

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            R.Range = RRange;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                comboW();
                comboQ();
                comboR();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harassW();
                harassQ();
                harassR();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                clearR();
                clearQ();
            }

            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                flee();
            }

            if (Menu.Item("ksQ").GetValue<bool>())
            {
                ksQ();
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
            var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Magical);

            if (W.IsReady() && target.IsValidTarget(800) && Menu.Item("comboW").GetValue<bool>())
                W.Cast(Player);
        }

        private static void comboR()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (R.IsReady() && target.IsValidTarget(R.Range) && Menu.Item("comboR").GetValue<bool>())
                R.CastIfHitchanceEquals(target, HitChance.VeryHigh);
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
            var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Magical);

            if (W.IsReady() && target.IsValidTarget(800) && Menu.Item("harassW").GetValue<bool>())
                W.Cast(Player);
        }

        private static void harassR()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (R.IsReady() && target.IsValidTarget(R.Range) && Menu.Item("harassR").GetValue<bool>())
                R.CastIfHitchanceEquals(target, HitChance.VeryHigh);
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

            if (Menu.Item("clearQ").GetValue<bool>() && minion.IsValidTarget() && Q.IsReady())
            {
                Q.Cast(minion);
            }
        }

        private static void clearR()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, R.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestCircularFarmLocation(
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Select(m => m.ServerPosition.To2D())
                        .ToList(),
                    R.Width,
                    R.Range);

            if (Menu.Item("clearR").GetValue<bool>() && minion.IsValidTarget() && R.IsReady())
            {
                R.Cast(farmLocation.Position);
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

            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (R.IsReady())
            {
                R.Cast(target);
            }
        }

        //KillSteal

        private static void ksQ()
        {
            var ksQ = Menu.Item("ksQ").GetValue<bool>();

            if (ksQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target != null && Q.IsKillable(target) && Q.CastOnUnit(target))
                {
                    return;
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

                if (Menu.Item("drawR").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Player.Position, R.Range, Color.Aqua);
                }

            }
        }

    }
}
