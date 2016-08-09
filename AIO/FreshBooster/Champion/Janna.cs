using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using static FreshBooster.FreshCommon;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FreshBooster.Champion
{
    class Janna
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Janna";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        public static HpBarIndicator Indicator = new HpBarIndicator();
        // Default Setting

        public static bool QSpell = false;
        public static int SpellTime = 0, RTime;
        public static AIHeroClient FleeQ;
        public static Vector3 QPosition;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 850f);
                _Q.SetSkillshot(0.25f, 120f, 900f, false, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 600);
                _E = new Spell(SpellSlot.E, 800f);
                _R = new Spell(SpellSlot.R, 875f);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 01");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private void Menu()
        {
            try
            {
                var Combo = new Menu("Combo", "Combo");
                {
                    Combo.AddItem(new MenuItem("Janna_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Janna_CUse_W", "Use W").SetValue(true));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.AddItem(new MenuItem("Janna_Flee", "Flee Key").SetValue(new KeyBind('G', KeyBindType.Press)));
                    Misc.AddItem(new MenuItem("Janna_AutoE", "Auto E").SetValue(true));
                    Misc.AddItem(new MenuItem("Janna_AutoE1", "Auto E(Not use me)").SetValue(true));
                    Misc.SubMenu("Auto R").AddItem(new MenuItem("Janna_AutoRHP", "Min HP %").SetValue(new Slider(10, 0, 100)));
                    Misc.SubMenu("Auto R").AddItem(new MenuItem("Janna_AutoREnable", "Enable").SetValue(true));
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Janna_InterQ", "Use Q").SetValue(true));
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Janna_InterR", "Use R").SetValue(true));
                    Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("Janna_GapQ", "Use Q").SetValue(true));
                    Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("Janna_GapR", "Use R").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Janna_Draw_Q", "Draw Q").SetValue(false));
                    Draw.AddItem(new MenuItem("Janna_Draw_W", "Draw W").SetValue(false));
                    Draw.AddItem(new MenuItem("Janna_Draw_E", "Draw E").SetValue(false));
                    Draw.AddItem(new MenuItem("Janna_Draw_R", "Draw R").SetValue(false));
                    Draw.AddItem(new MenuItem("Janna_Indicator", "Draw Damage Indicator").SetValue(true));
                }
                _MainMenu.AddSubMenu(Draw);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 02");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        public static void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                if (_MainMenu.Item("Janna_Draw_Q").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Janna_Draw_W").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Janna_Draw_E").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (_MainMenu.Item("Janna_Draw_R").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 150, Color.Green);
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 03");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        static float getComboDamage(Obj_AI_Base enemy)
        {
            try
            {
                if (enemy != null)
                {
                    float damage = 0;
                    if (_Q.IsReady())
                        damage += _Q.GetDamage(enemy);
                    if (_W.IsReady())
                        damage += _W.GetDamage(enemy);
                    if (!Player.Spellbook.IsAutoAttacking)
                        damage += (float)Player.GetAutoAttackDamage(enemy, true);
                    return damage;
                }
                return 0;
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 04");
                    ErrorTime = TickCount(10000);
                }
                return 0;
            }
        }
        public static void Drawing_OnEndScene(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget() && !ene.IsZombie))
                {
                    if (_MainMenu.Item("Janna_Indicator").GetValue<bool>())
                    {
                        Indicator.unit = enemy;
                        Indicator.drawDmg(getComboDamage(enemy), new ColorBGRA(255, 204, 0, 160));
                    }
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 05");
                    ErrorTime = TickCount(10000);
                }
            }
        }

        // OnLoad
        public Janna()
        {
            Player = ObjectManager.Player;
            SkillSet();
            Menu();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                // Set Target
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                var WTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);

                if (QSpell && _Q.IsCharging)
                {
                    _Q.Cast(true);
                    QSpell = false;
                }
                if (_MainMenu.Item("Janna_AutoREnable").GetValue<bool>())
                {
                    var AutoR = ObjectManager.Get<AIHeroClient>().OrderByDescending(x => x.Health).FirstOrDefault(x => x.HealthPercent < _MainMenu.Item("Janna_AutoRHP").GetValue<Slider>().Value && x.Distance(Player.ServerPosition) < _R.Range && !x.IsDead && x.IsAlly);
                    if (AutoR != null && _R.IsReady() && !Player.IsRecalling())
                    {
                        _R.Cast(true);
                        RTime = TickCount(2000);
                    }
                }
                if (_MainMenu.Item("Janna_Flee").GetValue<KeyBind>().Active) // Flee
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    if (WTarget != null && _W.IsReady() && WTarget.Distance(Player.ServerPosition) < 400)
                        _W.CastOnUnit(WTarget, true);
                }

                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active) // Combo
                {
                    if (QTarget != null && _MainMenu.Item("Janna_CUse_Q").GetValue<bool>() && _Q.IsReady())
                    {
                        var Prediction = _Q.GetPrediction(QTarget);
                        if (!_Q.IsCharging && Prediction.Hitchance >= HitChance.Medium)
                        {
                            _Q.Cast(Prediction.CastPosition, true);
                        }
                        if (_Q.IsCharging)
                        {
                            _Q.Cast(true);
                        }
                    }
                    if (WTarget != null && _W.IsReady())
                        _W.Cast(WTarget, true);
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    //Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {
                if (_MainMenu.Item("Janna_GapQ").GetValue<bool>() && _Q.IsReady() && gapcloser.Sender.ServerPosition.Distance(Player.ServerPosition) < 850 && !_Q.IsCharging)
                {
                    _Q.Cast(gapcloser.Sender.ServerPosition, true);
                    QSpell = true;
                    SpellTime = TickCount(1000);
                }

                if (_MainMenu.Item("Janna_GapR").GetValue<bool>() && _R.IsReady() && gapcloser.Sender.ServerPosition.Distance(Player.ServerPosition) < 875 && SpellTime < Environment.TickCount && !Player.IsRecalling())
                {
                    _R.Cast(true);
                    RTime = TickCount(2000);
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 07");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {

                if (args.Target is Obj_AI_Minion || !(sender is AIHeroClient))
                    return;
                if (_MainMenu.Item("Janna_AutoE").GetValue<bool>() && _E.IsReady())
                {
                    if (sender.IsEnemy)
                    {                        
                        var StartPos = args.Start;
                        var EndPos = args.End;
                        var NonTRange = new Geometry.Polygon.Rectangle(StartPos, EndPos, sender.BoundingRadius + 30);
                        var Target = HeroManager.Allies.FirstOrDefault(f => f.Position.Distance(Player.Position) <= _E.Range && NonTRange.IsInside(f.Position));
                        if (Target == Player && _MainMenu.Item("Janna_AutoE1").GetValue<bool>()) return;
                        if (Target != null)
                        {
                            _E.CastOnUnit(Target, true);
                            return;
                        }
                        if (args.Target != null && args.Target.Position.Distance(Player.Position) <= _E.Range && args.Target is AIHeroClient)
                        {
                            var ShieldTarget = HeroManager.Allies.FirstOrDefault(f => f.Position.Distance(args.Target.Position) <= 10);
                            _E.CastOnUnit(ShieldTarget, true);
                            return;
                        }
                    }
                    if (sender.IsAlly && args.Target is AIHeroClient)
                    {
                        if (sender.Position.Distance(Player.Position) <= _E.Range && args.Target != null && args.SData.Name.ToLower().Contains("attack"))
                        {
                            _E.CastOnUnit(sender, true);
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 08");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (!sender.IsEnemy || !sender.IsValid<AIHeroClient>())
                    return;
                if (_MainMenu.Item("Janna_InterQ").GetValue<bool>() && _Q.IsReady() && sender.ServerPosition.Distance(Player.ServerPosition) < 850 && !_Q.IsCharging)
                {
                    _Q.Cast(sender.ServerPosition, true);
                    QSpell = true;
                    SpellTime = TickCount(1000);
                }

                if (_MainMenu.Item("Janna_InterR").GetValue<bool>() && _R.IsReady() && sender.ServerPosition.Distance(Player.ServerPosition) < 875 && SpellTime < Environment.TickCount && !Player.IsRecalling())
                {
                    _R.Cast(true);
                    RTime = TickCount(2000);
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 09");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        public static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            try
            {

            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 10");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            try
            {
                if (sender.IsMe && RTime > Environment.TickCount)
                    args.Process = false;
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 11");
                    ErrorTime = TickCount(10000);
                }
            }

        }
        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.Write(e);
                Chat.Print("FreshPoppy is not working. plz send message by KorFresh (Code 13)");
            }
        }

        private static void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.Write(e);
                Chat.Print("FreshPoppy is not working. plz send message by KorFresh (Code 14)");
            }
        }
    }
}
