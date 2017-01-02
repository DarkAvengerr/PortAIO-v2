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
    class Soraka
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Soraka";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        
        // Default Setting

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 900);
                _Q.SetSkillshot(0.5f, 210, 1100, false, SkillshotType.SkillshotCircle);
                _W = new Spell(SpellSlot.W, 550);
                _E = new Spell(SpellSlot.E, 1025);
                _E.SetSkillshot(0.5f, 70f, 1750, false, SkillshotType.SkillshotCircle);
                _R = new Spell(SpellSlot.R);
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
                    Combo.AddItem(new MenuItem("Soraka_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Soraka_CUse_E", "Use E").SetValue(true));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {
                    Harass.AddItem(new MenuItem("Soraka_HUse_Q", "Use Q").SetValue(true));
                    Harass.AddItem(new MenuItem("Soraka_HUse_E", "Use E").SetValue(true));
                    Harass.AddItem(new MenuItem("Soraka_Auto_HEnable", "Auto Harass").SetValue(true));
                    Harass.AddItem(new MenuItem("Soraka_HMana", "Min. Mana %").SetValue(new Slider(50, 0, 100)));
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Harass);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Soraka_KUse_Q", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Soraka_KUse_E", "Use E").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.SubMenu("Heal W").AddItem(new MenuItem("Soraka_WMin", "Min Ally HP %").SetValue(new Slider(70, 0, 100)));
                    Misc.SubMenu("Heal W").AddItem(new MenuItem("Soraka_WMMin", "Min My HP %").SetValue(new Slider(30, 0, 100)));
                    Misc.SubMenu("Heal W").AddItem(new MenuItem("Soraka_WMinEnable", "Enable").SetValue(true));
                    Misc.SubMenu("Heal R").AddItem(new MenuItem("Soraka_RMin", "Min Ally HP %").SetValue(new Slider(15, 0, 100)));
                    Misc.SubMenu("Heal R").AddItem(new MenuItem("Soraka_RMinEnable", "Enable").SetValue(true));
                    Misc.AddItem(new MenuItem("Soraka_AntiE", "Anti-Gabcloser E").SetValue(true));
                    Misc.AddItem(new MenuItem("Soraka_InterE", "Interrupt E").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Soraka_QRange", "Q Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Soraka_WRange", "W Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Soraka_ERange", "E Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Soraka_WAlly", "Draw line Me to dangerous Ally").SetValue(false));
                    Draw.AddItem(new MenuItem("Soraka_Indicator", "Draw Damage Indicator").SetValue(false));
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
                var Rint = (_R.Level * 1000) + 3000;
                if (_MainMenu.Item("Soraka_QRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Soraka_WRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Soraka_ERange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                var QTarget = TargetSelector.GetTarget(_E.Range + 200, TargetSelector.DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 100, Color.Green);
                if (_MainMenu.Item("Soraka_WAlly").GetValue<bool>())
                    foreach (var item in HeroManager.Allies.OrderBy(f => f.Health))
                    {
                        if (item.Distance(Player.Position) <= 2500 && !item.IsDead)
                            if (item.HealthPercent <= _MainMenu.Item("Soraka_WMin").GetValue<Slider>().Value)
                                Drawing.DrawLine(ToScreen(Player.Position), ToScreen(item.Position), 1, LineColor(item));
                    }
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
                    if (_E.IsReady())
                        damage += _E.GetDamage(enemy);
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
        public Soraka()
        {
            Player = ObjectManager.Player;
            SkillSet();
            Menu();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnSpellCast += OnProcessSpell;
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
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                var WTarget = HeroManager.Allies.OrderBy(f => f.Health)
                    .FirstOrDefault(f => f.Position.Distance(Player.Position) <= _W.Range && f.HealthPercent <= _MainMenu.Item("Soraka_WMin").GetValue<Slider>().Value
                    && !f.InFountain(LeagueSharp.Common.Utility.FountainType.OwnFountain) && !f.IsDead && !f.IsZombie);
                var RTarget = HeroManager.Allies.OrderBy(f => f.Health)
                    .FirstOrDefault(f => f.HealthPercent <= _MainMenu.Item("Soraka_RMin").GetValue<Slider>().Value
                    && !f.InFountain(LeagueSharp.Common.Utility.FountainType.OwnFountain) && !f.IsDead && !f.IsZombie);

                //Kill
                if (_MainMenu.Item("Soraka_KUse_E").GetValue<bool>() && ETarget != null && _E.GetDamage(ETarget) > ETarget.Health)
                {
                    if (_E.GetPrediction(ETarget).Hitchance >= HitChance.Medium)
                        _E.CastIfHitchanceEquals(ETarget, _E.GetPrediction(ETarget).Hitchance, true);
                }
                if (_MainMenu.Item("Soraka_KUse_Q").GetValue<bool>() && QTarget != null && _Q.GetDamage(QTarget) > QTarget.Health)
                {
                    _Q.SetSkillshot(0.5f, 210, QTarget.Distance(Player.Position) / 2f, false, SkillshotType.SkillshotCircle);
                    if (_Q.GetPrediction(QTarget).Hitchance >= HitChance.Medium)
                        _Q.CastIfHitchanceEquals(QTarget, _Q.GetPrediction(QTarget).Hitchance, true);
                }

                //Auto W, R
                if (_MainMenu.Item("Soraka_WMinEnable").GetValue<bool>())
                    if (_W.IsReady())
                        if (WTarget != null)
                            _W.CastOnUnit(WTarget, true);
                if (_MainMenu.Item("Soraka_RMinEnable").GetValue<bool>())
                    if (_R.IsReady())
                        if (RTarget != null)
                        {
                            var enemy = HeroManager.Enemies.FirstOrDefault(f => RTarget.Distance(f.Position) < 1000);
                            if (enemy != null)
                                _R.Cast(true);
                        }

                //Combo
                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                {
                    if (_MainMenu.Item("Soraka_CUse_E").GetValue<bool>() && ETarget != null)
                        if (_E.IsReady())
                            if (_E.GetPrediction(ETarget).Hitchance >= HitChance.Medium)
                                _E.CastIfHitchanceEquals(ETarget, _E.GetPrediction(ETarget).Hitchance, true);
                    if (_MainMenu.Item("Soraka_CUse_Q").GetValue<bool>() && QTarget != null)
                        if (_Q.IsReady())
                        {
                            _Q.SetSkillshot(0.5f, 210, QTarget.Distance(Player.Position) / 2f, false, SkillshotType.SkillshotCircle);
                            if (_Q.GetPrediction(QTarget).Hitchance >= HitChance.Medium)
                                _Q.CastIfHitchanceEquals(QTarget, _Q.GetPrediction(QTarget).Hitchance, true);
                        }
                }

                //Harass
                if ((_MainMenu.Item("HKey").GetValue<KeyBind>().Active || _MainMenu.Item("Soraka_Auto_HEnable").GetValue<bool>())
                    && _MainMenu.Item("Soraka_HMana").GetValue<Slider>().Value < Player.ManaPercent)
                {
                    if (_MainMenu.Item("Soraka_HUse_E").GetValue<bool>() && ETarget != null)
                        if (_E.IsReady())
                            if (_E.GetPrediction(ETarget).Hitchance >= HitChance.Medium)
                                _E.CastIfHitchanceEquals(ETarget, _E.GetPrediction(ETarget).Hitchance, true);
                    if (_MainMenu.Item("Soraka_HUse_Q").GetValue<bool>() && QTarget != null)
                        if (_Q.IsReady())
                        {
                            _Q.SetSkillshot(0.5f, 210, QTarget.Distance(Player.Position) / 1.8f, false, SkillshotType.SkillshotCircle);
                            if (_Q.GetPrediction(QTarget).Hitchance >= HitChance.Medium)
                                _Q.CastIfHitchanceEquals(QTarget, _Q.GetPrediction(QTarget).Hitchance, true);
                        }
                }
            }
            catch (Exception e)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = TickCount(10000);
                    Console.WriteLine(e);
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {
                if (_MainMenu.Item("Soraka_AntiE").GetValue<bool>())
                    if (_E.IsReady())
                        if (gapcloser.Sender.IsEnemy)
                            if (gapcloser.Sender.Distance(Player.Position) <= _E.Range)
                                _E.CastIfHitchanceEquals(gapcloser.Sender, HitChance.Low, true);
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
                if (_MainMenu.Item("Soraka_InterE").GetValue<bool>())
                    if (_E.IsReady())
                        if (sender.IsEnemy)
                            if (sender.Distance(Player.Position) <= _E.Range)
                                _E.CastIfHitchanceEquals(sender, HitChance.Low, true);
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
