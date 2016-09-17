using System;
using System.Drawing;
using System.Linq;
using LeagueSharp.Common;
using LeagueSharp;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen
{
    static class AlexProgram
    {

        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }


        private static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Kennen")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 550);
            Q.SetSkillshot(0.125f, 50, 1700, true, SkillshotType.SkillshotLine);

            Config = new Menu("WR-GFY Kennen", "WR-GFY Kennen", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "Use (W)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Use E for Gapclose").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.combo", "Use (R)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.hit.count", "Min Enemy Count (R)").SetValue(new Slider(4, 1, 5)));
                    Config.AddSubMenu(comboMenu);
                }

                var harassMenu = new Menu("Harass Settings", "Harass Settings");
                {
                    harassMenu.AddItem(new MenuItem("q.harass", "Use (Q)").SetValue(true));
                    Config.AddSubMenu(harassMenu);
                }

                var clearMenu = new Menu("Clear Settings", "Clear Settings");
                {
                    var laneclearMenu = new Menu(":: Wave Clear", ":: Wave Clear");
                    {
                        laneclearMenu.AddItem(new MenuItem("keysinfo1", "                  (Q) Settings").SetTooltip("Q Settings"));
                        laneclearMenu.AddItem(new MenuItem("q.clear", "Use (Q)").SetValue(true));
                        laneclearMenu.AddItem(new MenuItem("keysinfo3", "                  (E) Settings").SetTooltip("E Settings"));
                        laneclearMenu.AddItem(new MenuItem("e.clear", "Use (E)").SetValue(true));
                        laneclearMenu.AddItem(new MenuItem("w.clear", "Use (W)").SetValue(true));
                        laneclearMenu.AddItem(new MenuItem("w.clear.minion.count", "(W) Min. Minion Count").SetValue(new Slider(3, 1, 10)));
                        clearMenu.AddSubMenu(laneclearMenu);
                    }

                    var jungleClear = new Menu(":: Jungle Clear", ":: Jungle Clear");
                    {
                        jungleClear.AddItem(new MenuItem("keysinfo1X", "                  (Q) Settings").SetTooltip("Q Settings"));
                        jungleClear.AddItem(new MenuItem("q.jungle", "Use (Q)").SetValue(true));
                        jungleClear.AddItem(new MenuItem("keysinfo2X", "                  (W) Settings").SetTooltip("W Settings"));
                        jungleClear.AddItem(new MenuItem("w.jungle", "Use (W)").SetValue(true));
                        jungleClear.AddItem(new MenuItem("keysinfo3X", "                  (E) Settings").SetTooltip("E Settings"));
                        jungleClear.AddItem(new MenuItem("e.jungle", "Use (E)").SetValue(true));
                        clearMenu.AddSubMenu(jungleClear);
                    }
                    Config.AddSubMenu(clearMenu);
                }
                var drawMenu = new Menu("Draw Settings", "Draw Settings");
                {
                    var skillDraw = new Menu("Skill Draws", "Skill Draws");
                    {
                        skillDraw.AddItem(new MenuItem("q.draw", "Q Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                        skillDraw.AddItem(new MenuItem("w.draw", "W Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                        skillDraw.AddItem(new MenuItem("e.draw", "E Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                        skillDraw.AddItem(new MenuItem("r.draw", "R Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
                        drawMenu.AddSubMenu(skillDraw);
                    }
                    Config.AddSubMenu(drawMenu);
                }
                Config.AddItem(new MenuItem("flee.active.x", "Flee (E)").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))).SetFontStyle(FontStyle.Bold, SharpDX.Color.Orange);
                Config.AddToMainMenu();
            }
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

        }
        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    Flee();
                    break;
            }

            if (Config.Item("r.combo").GetValue<bool>() && R.IsReady() && ObjectManager.Player.CountEnemiesInRange(R.Range) >=
                Config.Item("r.hit.count").GetValue<Slider>().Value)
            {
                R.Cast();
            }
        }
        private static void JungleClear()
        {

            if (Q.IsReady() && Config.Item("q.jungle").GetValue<bool>())
            {
                var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, ObjectManager.Player.AttackRange + 50, MinionTypes.All, MinionTeam.Neutral);
                if (mob[0].IsValid)
                {
                    Q.Cast(mob[0]);
                }
            }

            if (W.IsReady() && Config.Item("w.jungle").GetValue<bool>())
            {
                var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, ObjectManager.Player.AttackRange + 50, MinionTypes.All, MinionTeam.Neutral);
                if (mob[0].IsValid)
                {
                    W.Cast();
                }
            }

            if (E.IsReady() && Config.Item("e.jungle").GetValue<bool>())
            {
                var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, ObjectManager.Player.AttackRange + 50, MinionTypes.All, MinionTeam.Neutral);
                if (mob[0].IsValid)
                {
                    E.Cast();
                }
            }

        }
        private static void LaneClear()
        {



            if (Q.IsReady() && Config.Item("q.clear").GetValue<bool>())
            {
                var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
                foreach (var minion in min.Where(x => Q.GetDamage(x) > x.Health).Where
                    (minion => minion.CharData.BaseSkinName.ToLower().Contains("siege")))
                {
                    Q.Cast(minion);
                }
           if (Q.IsReady() && Config.Item("q.clear").GetValue<bool>())
            {
                    var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                    foreach (Obj_AI_Base minions in
                        vMinions.Where(minions => minions.Health < ObjectManager.Player.GetSpellDamage(minions, SpellSlot.Q) && (Orbwalker.GetTarget() == null || minions.ToString() != Orbwalker.GetTarget().ToString())))
                        Q.Cast(minions);
                }
            }

            if (E.IsReady() && Config.Item("e.clear").GetValue<bool>())
            {

                var minionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range)
                    .Where(minion => minion.IsValidTarget() && !minion.HasBuff("kennenmarkofstorm") && !minion.UnderTurret(true))
                    .OrderBy(minion => minion.Distance(ObjectManager.Player.ServerPosition));

                if (!ObjectManager.Player.HasBuff("KennenLightningRush"))
                {
                    E.Cast();
                }

                if (ObjectManager.Player.HasBuff("KennenLightningRush"))
                {
                    var target = minionsE.FirstOrDefault();
                    if (target != null)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                    }
                    else
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
            }

            if (W.IsReady() && Config.Item("w.clear").GetValue<bool>())
            {
                var minW = Config.Item("w.clear.minion.count").GetValue<Slider>().Value;
                var wCount = MinionManager
                    .GetMinions(ObjectManager.Player.ServerPosition, W.Range).Count(minion => minion.IsValidTarget() && minion.HasBuff("kennenmarkofstorm"));


                if (!ObjectManager.Player.HasBuff("KennenLightningRush") && wCount >= minW)
                {
                    W.Cast();
                }

            }
        }
        private static void Harass()
        {

            if (Config.Item("q.harass").GetValue<bool>() && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) &&
                    Q.GetPrediction(x).Hitchance >= HitChance.High && Q.GetPrediction(x).CollisionObjects.Count == 0))
                {
                    Q.Cast(enemy);
                }
            }
        }
        private static void Combo()
        {
            if (Config.Item("q.combo").GetValue<bool>() && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) &&
                    Q.GetPrediction(x).Hitchance >= HitChance.High && Q.GetPrediction(x).CollisionObjects.Count == 0))
                {
                    Q.Cast(enemy);
                }
            }

            if (Config.Item("w.combo").GetValue<bool>() && W.IsReady())
            {
                // ReSharper disable once UnusedVariable
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsEnemyStunnable() && x.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }

                if (Config.Item("e.combo").GetValue<bool>())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && !x.IsDead && !x.IsZombie && !ObjectManager.Player.HasBuff("KennenLightningRush")))
                    {
                        E.Cast();
                    }
                }
            }

            if (Config.Item("r.combo").GetValue<bool>() && R.IsReady())
            {
                // ReSharper disable once UnusedVariable
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && R.GetDamage(x) > x.Health))
                {
                    R.Cast();
                }
            }
        }
        private static void Flee()
        {

            if (Config.Item("flee.active.x").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (E.IsReady() && !ObjectManager.Player.HasBuff("KennenLightningRush"))
                {
                    E.Cast();
                }
            }
        }
        private static bool IsEnemyStunnable(this AIHeroClient enemy)
        {
            return enemy.Buffs.Any(buff => buff.Name == "kennenmarkofstorm" && buff.Count >= 2);
        }
        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("q.draw").GetValue<Circle>().Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Config.Item("q.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("w.draw").GetValue<Circle>().Active && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Config.Item("w.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("e.draw").GetValue<Circle>().Active && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Config.Item("e.draw").GetValue<Circle>().Color);
            }
            if (Config.Item("r.draw").GetValue<Circle>().Active && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Config.Item("r.draw").GetValue<Circle>().Color);
            }
        }
//h3h3 is ein blöder homo
        public static float Range { get; set; }
    }
}
