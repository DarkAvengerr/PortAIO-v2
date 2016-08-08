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
    class Morgana
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Morgana";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        public static HpBarIndicator Indicator = new HpBarIndicator();
        // Default Setting

        public static BuffInstance EnemyBuff;
        public static AIHeroClient PassiveEnemy;
        private static Vector3 QFPos, BestPos;
        private static int QTime, AntiTime, InterTime;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 1175f);
                _Q.SetSkillshot(0.25f, 72f, 1200f, true, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 900f);
                _W.SetSkillshot(0.5f, 225f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                _E = new Spell(SpellSlot.E, 750f);
                _R = new Spell(SpellSlot.R, 600f);
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
                    Combo.AddItem(new MenuItem("Morgana_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Morgana_CUse_Q_Hit", "Q Hitchance").SetValue(new Slider(4, 1, 6)));
                    Combo.AddItem(new MenuItem("Morgana_CUse_W", "Use W").SetValue(true));
                    Combo.AddItem(new MenuItem("Morgana_CUse_R", "Use R").SetValue(true));
                    Combo.AddItem(new MenuItem("Morgana_MinR", "R Min Enemy").SetValue(new Slider(2, 1, 5)));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {
                    Harass.AddItem(new MenuItem("Morgana_HUse_Q", "Use Q").SetValue(true));
                    Harass.AddItem(new MenuItem("Morgana_HUse_Q_Hit", "Q Hitchance").SetValue(new Slider(4, 1, 6)));
                    Harass.AddItem(new MenuItem("Morgana_HUse_W", "Use W").SetValue(true));
                    Harass.AddItem(new MenuItem("Morgana_Auto_HEnable", "Auto Harass").SetValue(true));
                    Harass.AddItem(new MenuItem("Morgana_HMana", "Min. Mana %").SetValue(new Slider(50, 0, 100)));
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Harass);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Morgana_KUse_Q", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Morgana_KUse_W", "Use W").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Morgana_KUse_R", "Use R").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.AddItem(new MenuItem("Morgana_AutoQ", "Auto Q to has CC").SetValue(true));
                    Misc.AddItem(new MenuItem("Morgana_AutoE", "Auto E").SetValue(true));
                    Misc.AddItem(new MenuItem("Morgana_Anti-Q", "Use Q").SetValue(true));
                    Misc.AddItem(new MenuItem("Morgana_Inter-Q", "Use Q").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Morgana_QRange", "Q Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Morgana_WRange", "W Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Morgana_ERange", "E Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Morgana_RRange", "R Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Morgana_Indicator", "Draw Damage Indicator").SetValue(false));
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
                if (_MainMenu.Item("Morgana_QRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Morgana_WRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Morgana_ERange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (_MainMenu.Item("Morgana_RRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 100, Color.Green);
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
                    if (_W.LSIsReady())
                        damage += _W.GetDamage(enemy);
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
                    if (_MainMenu.Item("Morgana_Indicator").GetValue<bool>())
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
        public Morgana()
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
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                var WTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);

                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                {
                    if (_MainMenu.Item("Morgana_CUse_Q").GetValue<bool>() && !_R.IsChanneling)
                        if (_Q.LSIsReady())
                            if (QTarget != null)
                                if (_Q.GetPrediction(QTarget, false).Hitchance >= Hitchance("Morgana_CUse_Q_Hit"))
                                    _Q.CastIfHitchanceEquals(QTarget, _Q.GetPrediction(QTarget, false).Hitchance, true);
                    if (_MainMenu.Item("Morgana_CUse_W").GetValue<bool>())
                        if (_W.LSIsReady())
                            if (WTarget != null)
                                if (!WTarget.CanMove)
                                    _W.CastIfHitchanceEquals(WTarget, HitChance.Low, true);
                    if (_MainMenu.Item("Morgana_CUse_R").GetValue<bool>())
                        if (_R.LSIsReady())
                        {
                            int Count = HeroManager.Enemies.Where(f => f.LSDistance(Player.Position) < _R.Range + 500).Count(f => !f.IsDead && !f.IsZombie && f.LSDistance(Player.Position) < _R.Range);                            
                            if (_MainMenu.Item("Morgana_MinR").GetValue<Slider>().Value <= Count)
                            {
                                if (_E.LSIsReady())
                                    _E.CastOnUnit(Player, true);
                                _R.Cast(true);
                            }   
                        }
                }
            }
            catch (Exception)
            {
                if (NowTime() > ErrorTime)
                {
                    Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {

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
                if(sender.IsEnemy && sender is AIHeroClient)
                {
                    
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

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.Write(e);
                Chat.Print("FreshPoppy is not working. plz send message by KorFresh (Code 15)");
            }
        }
    }
}
