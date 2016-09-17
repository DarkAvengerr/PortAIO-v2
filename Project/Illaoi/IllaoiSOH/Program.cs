using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Linq;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace IllaoiSOH
{
    class Program
    {

        const string champName = "Illaoi";
        static AIHeroClient player { get { return ObjectManager.Player; } }
        static Spell q, w, e, r;
        static Orbwalking.Orbwalker orbwalker;
        static Menu menu;
        static readonly Render.Text Text = new Render.Text(0, 0, "", 11, new ColorBGRA(255, 0, 0, 255), "monospace");

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (player.ChampionName != champName)
                return;

            q = new Spell(SpellSlot.Q, 800);
            w = new Spell(SpellSlot.W, 390);
            e = new Spell(SpellSlot.E, 900);
            r = new Spell(SpellSlot.R, 350);
            q.SetSkillshot(.484f, 0, 500, false, SkillshotType.SkillshotCircle);
            e.SetSkillshot(.066f, 50, 1900, true, SkillshotType.SkillshotLine);
            //r.SetSkillshot(.310f, 0, 0, false, SkillshotType.SkillshotCircle);

            maimMenu();
            menu.AddToMainMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Draw_OnDraw;
            Chat.Print("<font color ='#2F9D27'>Illaoi - SOH</font> Korean Developer");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (player.IsDead)
                return;
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                        DoCombo(menu.Item("UseQ").GetValue<bool>(), menu.Item("UseQG").GetValue<bool>(), menu.Item("UseW").GetValue<bool>(), menu.Item("UseE").GetValue<bool>(), menu.Item("UseR").GetValue<bool>());
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                        DoHarass(menu.Item("harassUseQ").GetValue<bool>(), menu.Item("harassUseW").GetValue<bool>());
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                        DoLaneClear(menu.Item("lcUseQ").GetValue<bool>());
                    break;
            }
        }

        static void DoCombo(bool useQ, bool useQG, bool useW, bool useE, bool useR)
        {
            AIHeroClient target = TargetSelector.GetTarget(e.Range, TargetSelector.DamageType.Magical);
            var Ghost = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Name == target.Name);
            if (target != null && target.IsValidTarget())
            {
                if (player.Distance(target.Position) < w.Range)//W>>EQ(R)>>W
                {
                    if (w.IsReady() && useW)
                        w.Cast(target, false);
                    if (e.IsReady() && useE)
                    {
                        PredictionOutput prediction;
                        prediction = e.GetPrediction(target, true);
                        if (prediction.Hitchance >= HitChance.High)
                            e.Cast(target, true);
                    }
                    if (q.IsReady() && useQ)
                        q.Cast(target, false);
                    if (r.IsReady() && useR && player.CountEnemiesInRange(r.Range) >= menu.Item("r.enemy").GetValue<Slider>().Value)
                        r.Cast(target, true);
                    if (w.IsReady() && useW && target != null)
                        w.Cast(target, false);
                }
                else if (e.IsReady() == false && player.Distance(target.Position) < e.Range)//E>>Q
                {
                    if (e.IsReady() && useE)
                    {
                        PredictionOutput prediction;
                        prediction = e.GetPrediction(target, true);
                        if (prediction.Hitchance >= HitChance.High)
                            e.Cast(target, true);
                    }
                    if (q.IsReady() && useQ)
                        q.Cast(target, true);
                }
            }
            else if (target == null && Ghost != null && Ghost.IsValidTarget())
            {
                if (player.Distance(Ghost.Position) < q.Range)//Ghost attack
                {
                    if (q.IsReady() && useQG)
                        q.Cast(Ghost, true);
                }
            }
        }

        static void DoHarass(bool useQ, bool useW)
        {
            if (player.ManaPercentage() < menu.Item("harassMana").GetValue<Slider>().Value)
            {
                return;
            }
            var enemy = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(850));
            AIHeroClient target = TargetSelector.GetTarget(q.Range, TargetSelector.DamageType.Magical);
            if (target != null && target.IsValidTarget())
            {
                if (player.Distance(target.Position) < q.Range)
                {
                    if (q.IsReady() && useQ)
                        q.CastIfHitchanceEquals(target, HitChance.High, true);
                }
                if (player.Distance(target.Position) < w.Range && enemy != null)
                {
                    if (w.IsReady() && useW)
                        w.Cast(target, true);
                }
            }
        }

        static void DoLaneClear(bool useq)
        {
            if (player.ManaPercent < menu.Item("laneclearMana").GetValue<Slider>().Value)
            {
                return;
            }
            var minionCount = MinionManager.GetMinions(player.Position, q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (q.IsReady())
            {
                var mfarm = q.GetLineFarmLocation(minionCount);
                if (minionCount.Count >= menu.Item("q.minion").GetValue<Slider>().Value)
                {
                    q.Cast(mfarm.Position);
                }
            }
        }

        static void Draw_OnDraw(EventArgs args)
        {
            if (menu.Item("drawQ").GetValue<bool>())
            {
                if (q.IsReady())
                {
                    Render.Circle.DrawCircle(player.Position, q.Range+50, Color.LightGreen);
                }
                else
                {
                    Render.Circle.DrawCircle(player.Position, q.Range+50, Color.Red);
                }
            }
            if (menu.Item("drawW").GetValue<bool>())
            {
                if (w.IsReady())
                {
                    Render.Circle.DrawCircle(player.Position, w.Range, Color.LightGreen);
                }
                else
                {
                    Render.Circle.DrawCircle(player.Position, w.Range, Color.Red);
                }
            }
            if (menu.Item("drawE").GetValue<bool>())
            {
                if (e.IsReady())
                {
                    Render.Circle.DrawCircle(player.Position, e.Range, Color.LightGreen);
                }
                else
                {
                    Render.Circle.DrawCircle(player.Position, e.Range, Color.Red);
                }
            }
            if (menu.Item("drawR").GetValue<bool>())
            {
                if (r.IsReady())
                {
                    Render.Circle.DrawCircle(player.Position, player.BoundingRadius + r.Range + 100, Color.LightGreen);
                }
                else
                {
                    Render.Circle.DrawCircle(player.Position, player.BoundingRadius + r.Range + 100, Color.Red);
                }
            }
            if (menu.Item("Passive").GetValue<bool>())
            {
                var enemy = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(2000));
                foreach (var passive in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name == "God"))
                {
                    Render.Circle.DrawCircle(new Vector3(passive.Position.X, passive.Position.Y, passive.Position.Z), 50, Color.YellowGreen, 50);
                    if (enemy != null)
                    {
                        var xx = Drawing.WorldToScreen(passive.Position.Extend(enemy.Position, 850));
                        var xy = Drawing.WorldToScreen(passive.Position);
                        Drawing.DrawLine(xy.X, xy.Y, xx.X, xx.Y, 5, Color.YellowGreen);
                    }

                }
            }
            if (menu.Item("drawDmg").GetValue<bool>())
                DrawHPBarDamage();
        }

        static void maimMenu()
        {
            menu = new Menu("Illaoi - SOH", "Illaoi - SOH", true);

            Menu tsMenu = menu.AddSubMenu(new Menu("Target Selector", "TS"));
            TargetSelector.AddToMenu(tsMenu);

            Menu orbwalkerMenu = menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            
            Menu comboMenu = menu.AddSubMenu(new Menu("Combo", "combo"));
            comboMenu.AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseQG", "Use Q Ghost").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("r.enemy", "R Enemy").SetValue(new Slider(1, 1, 5)));

            Menu harassMenu = menu.AddSubMenu(new Menu("Harass", "harass"));
            harassMenu.AddItem(new MenuItem("harassUseQ", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassUseW", "Use W").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassMana", "Mana Manager (%)").SetValue(new Slider(70, 1, 100)));

            Menu lcMenu = menu.AddSubMenu(new Menu("Lane Clear", "laneClear"));
            lcMenu.AddItem(new MenuItem("lcUseQ", "Use Q").SetValue(true));
            lcMenu.AddItem(new MenuItem("laneclearMana", "Mana Manager (%)").SetValue(new Slider(30, 1, 100)));
            lcMenu.AddItem(new MenuItem("q.minion", "Minion").SetValue(new Slider(3, 1, 10)));

            Menu drawMenu = menu.AddSubMenu(new Menu("Drawing", "drawing"));
            drawMenu.AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawW", "Draw W Range").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawE", "Draw E Range").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawR", "Draw R Range").SetValue(true));
            drawMenu.AddItem(new MenuItem("Passive", "Draw Passive").SetValue(true));
            drawMenu.AddItem(new MenuItem("drawDmg", "Draw Combo Damage").SetValue(true));

        }


        static double getComboDamage(Obj_AI_Base target)
        {
            double damage = player.GetAutoAttackDamage(target);
            if (q.IsReady() && menu.Item("UseQ").GetValue<bool>())
                damage += player.GetSpellDamage(target, SpellSlot.Q);
            if (w.IsReady() && menu.Item("UseW").GetValue<bool>())
                damage += player.GetSpellDamage(target, SpellSlot.W);
            if (e.IsReady() && menu.Item("UseE").GetValue<bool>())
                damage += player.GetSpellDamage(target, SpellSlot.E);
            if (r.IsReady() && menu.Item("UseR").GetValue<bool>())
                damage += player.GetSpellDamage(target, SpellSlot.R);
            return damage;
        }

        static void DrawHPBarDamage()
        {
            const int XOffset = 10;
            const int YOffset = 20;
            const int Width = 103;
            const int Height = 8;
            foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValid && h.IsHPBarRendered && h.IsEnemy))
            {
                var barPos = unit.HPBarPosition;
                var damage = getComboDamage(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = ((int)(unit.Health - damage)).ToString();
                    Text.OnEndScene();
                }
                Drawing.DrawLine((float)xPosDamage, yPos, (float)xPosDamage, yPos + Height, 2, Color.Red);
            }
        }
    }
}