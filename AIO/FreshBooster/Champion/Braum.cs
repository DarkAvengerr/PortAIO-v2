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
    class Braum
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Braum";   // Edit
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
                _Q = new Spell(SpellSlot.Q, 1000f);
                _Q.SetSkillshot(0.25f, 120f, 1400f, true, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 650);
                _E = new Spell(SpellSlot.E, 0);
                _R = new Spell(SpellSlot.R, 1250f);
                _R.SetSkillshot(0.5f, 120f, 1200f, false, SkillshotType.SkillshotLine);
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
                    Combo.AddItem(new MenuItem("Braum_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Braum_CUse_Q_Hit", "Q HitChance").SetValue(new Slider(3, 1, 6)));
                    Combo.AddItem(new MenuItem("Braum_CUse_R", "Use R").SetValue(true));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {
                    Harass.AddItem(new MenuItem("Braum_HUse_Q", "Use Q").SetValue(true));
                    Harass.AddItem(new MenuItem("Braum_Auto_HEnable", "Auto Harass").SetValue(true));
                    Harass.AddItem(new MenuItem("Braum_HMana", "Min. Mana %").SetValue(new Slider(50, 0, 100)));
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Harass);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Braum_KUse_Q", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Braum_KUse_R", "Use R").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.AddItem(new MenuItem("Braum_Flee", "Flee Key").SetValue(new KeyBind('G', KeyBindType.Press)));
                    Misc.AddItem(new MenuItem("Braum_AutoW", "Auto W").SetValue(true));
                    Misc.AddItem(new MenuItem("Braum_AutoE", "Auto E").SetValue(true));
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Braum_InterR", "Use R").SetValue(true));
                    Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("Braum_GapQ", "Use Q").SetValue(true));
                    Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("Braum_GapR", "Use R").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Braum_Draw_Q", "Draw Q").SetValue(false));
                    Draw.AddItem(new MenuItem("Braum_Draw_W", "Draw W").SetValue(false));
                    Draw.AddItem(new MenuItem("Braum_Draw_R", "Draw R").SetValue(false));
                    Draw.AddItem(new MenuItem("Braum_Indicator", "Draw Damage Indicator").SetValue(true));
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
                if (_MainMenu.Item("Braum_Draw_Q").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Braum_Draw_W").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Braum_Draw_R").GetValue<bool>())
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
                    if (_Q.LSIsReady())
                        damage += _Q.GetDamage(enemy);
                    if (_R.LSIsReady())
                        damage += _R.GetDamage(enemy);
                    if (!Player.Spellbook.IsAutoAttacking)
                        damage += (float)Player.LSGetAutoAttackDamage(enemy, true);
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
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
                {
                    if (_MainMenu.Item("Braum_Indicator").GetValue<bool>())
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
        public Braum()
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
                var RTarget = TargetSelector.GetTarget(_R.Range, TargetSelector.DamageType.Magical);

                // Kill
                if (_MainMenu.Item("Braum_KUse_Q").GetValue<bool>() && QTarget != null && _Q.LSIsReady() && _Q.GetDamage(QTarget) > QTarget.Health)
                {
                    _Q.CastIfHitchanceEquals(QTarget, HitChance.Low, true);
                    return;
                }
                if (_MainMenu.Item("Braum_KUse_R").GetValue<bool>() && QTarget != null && _R.LSIsReady() && _R.GetDamage(RTarget) > RTarget.Health)
                {
                    _R.CastIfHitchanceEquals(QTarget, HitChance.VeryHigh, true);
                    return;
                }

                // Flee
                if (_MainMenu.Item("Braum_Flee").GetValue<KeyBind>().Active)
                {
                    MovingPlayer(Game.CursorPos);
                    if (QTarget != null && _Q.LSIsReady())
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.Low, true);
                }

                // Combo
                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                {
                    if (_MainMenu.Item("Braum_CUse_R").GetValue<bool>() && _R.LSIsReady() && RTarget != null)
                        _R.CastIfHitchanceEquals(RTarget, HitChance.VeryHigh, true);
                    if (_MainMenu.Item("Braum_CUse_Q").GetValue<bool>() && _Q.LSIsReady() && QTarget != null)
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Braum_CUse_Q_Hit"), true);
                }

                // Harass
                if ((_MainMenu.Item("HKey").GetValue<KeyBind>().Active || _MainMenu.Item("Braum_Auto_HEnable").GetValue<bool>())
                    && _MainMenu.Item("Braum_HMana").GetValue<Slider>().Value < Player.ManaPercent)
                {
                    if (_MainMenu.Item("Braum_HUse_Q").GetValue<bool>() && _Q.LSIsReady() && QTarget != null)
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.VeryHigh, true);
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
                if (_MainMenu.Item("Braum_GapQ").GetValue<bool>() && _Q.LSIsReady() && gapcloser.Sender.Position.LSDistance(Player.Position) < _Q.Range)
                {
                    _Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.Low, true);
                    return;
                }
                if (_MainMenu.Item("Braum_GapR").GetValue<bool>() && _R.LSIsReady() && gapcloser.Sender.Position.LSDistance(Player.Position) < _R.Range)
                {
                    _R.CastIfHitchanceEquals(gapcloser.Sender, HitChance.VeryHigh, true);
                    return;
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
                if (!(sender is AIHeroClient) || Player.LSIsRecalling())
                    return;
                // Auto W
                if (_MainMenu.Item("Braum_AutoW").GetValue<bool>() && _W.LSIsReady())
                {
                    if (!(sender is AIHeroClient) || !sender.IsEnemy)
                        return;
                    if (args.Target != null)
                        if (args.SData.Name.ToLower().Contains("attack") && args.Target.Position.LSDistance(Player.Position) < _W.Range)
                            if (args.Target.IsAlly && args.Target is AIHeroClient)
                            {
                                if (args.Target.IsMe && Player.HealthPercent < 20)
                                {
                                    _W.CastOnUnit((Obj_AI_Base)args.Target, true);
                                }
                                else
                                {
                                    _W.CastOnUnit((Obj_AI_Base)args.Target, true);
                                }
                            }
                }
                // Auto E
                if (_MainMenu.Item("Braum_AutoE").GetValue<bool>() && _E.LSIsReady())
                {
                    if (!(sender is AIHeroClient) || !sender.IsEnemy || !Orbwalking.CanAttack())
                        return;
                    var enemyskill = new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.BounceRadius + 20);
                    var myteam = HeroManager.Allies.Where(f => f.LSDistance(Player.Position) < 200);
                    var count = myteam.Count(f => enemyskill.IsInside(f.Position));
                    if (args.Target != null && args.Target.Position.LSDistance(Player.Position) < 200)
                    {
                        if (args.Target.Name == Player.Name && Player.HealthPercent < 20)
                        {
                            _E.Cast(sender.Position, true);
                        }
                        else if (args.Target.Position.LSDistance(Player.Position) < 200 && args.Target is AIHeroClient)
                        {
                            if (_W.LSIsReady() && args.Target.Position.LSDistance(Player.Position) < _W.Range)
                                _W.CastOnUnit((Obj_AI_Base)args.Target, true);
                            _E.Cast(sender.Position, true);
                        }
                    }
                    else if (args.Target == null)
                    {
                        if (Player.HealthPercent < 20 && count == 1)
                        {
                            _E.Cast(sender.Position, true);
                        }
                        else if (count >= 2)
                        {
                            _E.Cast(sender.Position, true);
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
                if (_MainMenu.Item("Braum_InterR").GetValue<bool>() && _R.LSIsReady() && sender.IsEnemy && sender.Position.LSDistance(Player.Position) < _R.Range * 0.9)
                    _R.CastIfHitchanceEquals(sender, HitChance.VeryHigh, true);
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
