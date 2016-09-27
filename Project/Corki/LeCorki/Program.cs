using System;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LeCorki
{
    using System.Linq;

    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker orbwalker;

        private static Spell q, w, e, r;

        private static Menu menu;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Corki")
            {
                return;
            }

            //Spells

            q = new Spell(SpellSlot.Q, 825);
            w = new Spell(SpellSlot.W, 800);
            e = new Spell(SpellSlot.E, 600);
            r = new Spell(SpellSlot.R, 1225);

            //SkillShots
            //Thanks jQuery for spell data!

            q.SetSkillshot(0.35f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            e.SetSkillshot(0f, (float)(45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);
            r.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            //Menu

            menu = new Menu("LeCorki", "LeCorki", true);

            //Orbwalking

            var orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));

            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            //Target Selector

            var ts = menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));

            TargetSelector.AddToMenu(ts);

            //LeCombo Menu

            var comboMenu = menu.AddSubMenu(new Menu("LeCombo", "LeCombo"));

            comboMenu.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboR", "Use R").SetValue(true));

            comboMenu.AddItem(new MenuItem("LeCombo", "LeCombo").SetValue(new KeyBind(32, KeyBindType.Press)));

            //LeHarass Menu

            var harassMenu = menu.AddSubMenu(new Menu("LeHarass", "LeHarass"));

            harassMenu.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassE", "Use E").SetValue(false));
            harassMenu.AddItem(new MenuItem("harassR", "Use R").SetValue(true));

            harassMenu.AddItem(
                new MenuItem("LeHarass", "LeHarass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            //LeLaneClear Menu

            var laneclearMenu = menu.AddSubMenu(new Menu("LeLaneClear", "LeLaneClear"));

            laneclearMenu.AddItem(new MenuItem("laneclearQ", "Use Q").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("laneclearE", "Use E").SetValue(false));
            laneclearMenu.AddItem(new MenuItem("laneclearR", "Use R").SetValue(true));

            laneclearMenu.AddItem(
                new MenuItem("LeLaneClear", "LeLaneClear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //LeDraw Menu

            var drawMenu = menu.AddSubMenu(new Menu("LeDrawings", "LeDrawings"));

            drawMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));

            //Stuff

            menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Notifications.AddNotification("LeCorki Chargé!", 10000);

        }

        //Le Spell Stuffs

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboQ();
                    ComboE();
                    ComboR();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneclearQ();
                    LaneclearE();
                    LaneclearR();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassQ();
                    HarassR();
                    break;
            }
        }

        //Le Combo Stuffs

        private static void ComboQ()
        {
            var target = TargetSelector.GetTarget(r.Range, TargetSelector.DamageType.Magical);

            if (q.IsReady() && target.IsValidTarget(q.Range) && menu.Item("comboQ").GetValue<bool>())
                q.CastIfHitchanceEquals(target, HitChance.High);
        }

        private static void ComboE()
        {
            var target = TargetSelector.GetTarget(r.Range, TargetSelector.DamageType.Magical);

            if (e.IsReady() && target.IsValidTarget(e.Range) && menu.Item("comboE").GetValue<bool>())
                e.CastIfHitchanceEquals(target, HitChance.High);
        }

        private static void ComboR()
        {
            var target = TargetSelector.GetTarget(r.Range, TargetSelector.DamageType.Magical);

            if (r.IsReady() && target.IsValidTarget(r.Range) && menu.Item("comboR").GetValue<bool>())
                r.CastIfHitchanceEquals(target, HitChance.High);
        }

        //Le Harass Stuffs

        private static void HarassQ()
        {
            var target = TargetSelector.GetTarget(r.Range, TargetSelector.DamageType.Magical);

            if (q.IsReady() && target.IsValidTarget(q.Range) && menu.Item("harassQ").GetValue<bool>())
                q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
        }

        private static void HarassR()
        {
            var target = TargetSelector.GetTarget(r.Range, TargetSelector.DamageType.Magical);

            if (r.IsReady() && target.IsValidTarget(r.Range) && menu.Item("harassR").GetValue<bool>())
                r.CastIfHitchanceEquals(target, HitChance.High);
        }

        //Le LaneClear Stuffs
        //Thanks blacky for fixing my Lane Clear!

        private static void LaneclearQ()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, q.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestCircularFarmLocation(
                    MinionManager.GetMinions(q.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Select(m => m.ServerPosition.To2D())
                        .ToList(),
                    q.Width,
                    q.Range);

            if (menu.Item("laneclearQ").GetValue<bool>() && minion.IsValidTarget() && q.IsReady())
            {
                q.Cast(farmLocation.Position);
            }
        }

        private static void LaneclearE()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, e.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestLineFarmLocation(
                    MinionManager.GetMinions(r.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Select(m => m.ServerPosition.To2D())
                        .ToList(),
                    e.Width,
                    e.Range);

            if (menu.Item("laneclearE").GetValue<bool>() && e.IsReady())
            {
                e.Cast(farmLocation.Position);
            }
        }

        private static void LaneclearR()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, r.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            var farmLocation =
                MinionManager.GetBestLineFarmLocation(
                    MinionManager.GetMinions(r.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Select(m => m.ServerPosition.To2D())
                        .ToList(),
                    r.Width,
                    r.Range);

            if (menu.Item("laneclearR").GetValue<bool>() && r.IsReady())
            {
                r.Cast(farmLocation.Position);
            }
        }

        //Le Draw Stuffs

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            if (menu.Item("drawQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, q.Range, Color.LawnGreen);
            }

            if (menu.Item("drawW").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, w.Range, Color.LawnGreen);
            }

            if (menu.Item("drawE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, e.Range, Color.LawnGreen);
            }

            if (menu.Item("drawR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, r.Range, Color.LawnGreen);
            }
        }
    }
}