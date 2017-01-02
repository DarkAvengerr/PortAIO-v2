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
    class Alistar
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Alistar";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _W2, _E, _R;
        
        // Default Setting

        public static BuffInstance EnemyBuff;
        public static AIHeroClient PassiveEnemy;
        private static Vector3 QFPos, BestPos;
        private static int QTime, AntiTime, InterTime;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 365f);
                _W = new Spell(SpellSlot.W, 650f);
                _E = new Spell(SpellSlot.E, 575f);
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
                    Combo.AddItem(new MenuItem("Alistar_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Alistar_CUse_W", "Use W").SetValue(true));
                    Combo.AddItem(new MenuItem("Alistar_CUse_R", "Use R").SetValue(true));
                    Combo.AddItem(new MenuItem("Alistar_CUse_WQ", "Use Combo with W + Q").SetValue(new KeyBind('T', KeyBindType.Press)));
                    Combo.AddItem(new MenuItem("Alistar_FQKey", "Q + Flash").SetValue(new KeyBind('G', KeyBindType.Press)));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Alistar_KUse_Q", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Alistar_KUse_W", "Use W").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.SubMenu("Heal E").AddItem(new MenuItem("Alistar_EMMin", "Min. HP %").SetValue(new Slider(70, 0, 100)));
                    Misc.SubMenu("Heal E").AddItem(new MenuItem("Alistar_EMinEnable", "Enable").SetValue(true));
                    Misc.SubMenu("Anti-Gabcloser").AddItem(new MenuItem("Alistar_Anti-Q", "Use Q").SetValue(true));
                    Misc.SubMenu("Anti-Gabcloser").AddItem(new MenuItem("Alistar_Anti-W", "Use W").SetValue(true));
                    Misc.SubMenu("Anti-Gabcloser").AddItem(new MenuItem("Alistar_Anti-Order", "Anti-Gabcloser Order").SetValue(new StringList(new[] { "Q-W", "W-Q" })));
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Alistar_Inter-Q", "Use Q").SetValue(true));
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Alistar_Inter-W", "Use W").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Alistar_QRange", "Q Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Alistar_WRange", "W Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Alistar_ERange", "E Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Alistar_QFange", "Q+Flash Range").SetValue(false));                    
                    Draw.AddItem(new MenuItem("Alistar_Indicator", "Draw Damage Indicator").SetValue(false));
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
                if (_MainMenu.Item("Alistar_QRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Alistar_WRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Alistar_ERange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (_MainMenu.Item("Alistar_QFange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range + 425, Color.Aqua, 1);
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 100, Color.Green);
                if (_MainMenu.Item("Alistar_CUse_WQ").GetValue<KeyBind>().Active
                    || _MainMenu.Item("Alistar_FQKey").GetValue<KeyBind>().Active)
                {
                    if (TargetSelector.GetSelectedTarget() == null)
                        Drawing.DrawText(Drawing.Width / 2, Drawing.Height / 3, Color.Red, "Not Select Target");
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
        public Alistar()
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
                var WTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);
                var GetTarget = TargetSelector.GetSelectedTarget();
                // Pulverize, Headbutt ,TriumphantRoar

                // E
                if (_MainMenu.Item("Alistar_EMinEnable").GetValue<bool>())
                {
                    var EAlly = HeroManager.Allies.FirstOrDefault(f => f.Distance(Player.Position) < _E.Range && !f.IsRecalling() && !f.InFountain(LeagueSharp.Common.Utility.FountainType.OwnFountain)
                        && _MainMenu.Item("Alistar_EMMin").GetValue<Slider>().Value > f.HealthPercent);
                    if (EAlly.IsRecalling() || Player.IsRecalling())
                        return;
                    if (EAlly != null)
                        _E.Cast(true);
                }

                // Combo
                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                {
                    if (_MainMenu.Item("Alistar_CUse_R").GetValue<bool>())
                        if (_R.IsReady())
                        {
                            var Enemy = HeroManager.Enemies.FirstOrDefault(f => f.Distance(Player.Position) < _Q.Range);
                            if (Enemy != null && HasBuff(Player))
                                _R.Cast(true);
                        }
                    if (QTarget != null && _Q.IsReady())
                        if (_MainMenu.Item("Alistar_CUse_Q").GetValue<bool>())
                            _Q.Cast(true);
                    if (WTarget != null && _W.IsReady())
                        if (_MainMenu.Item("Alistar_CUse_W").GetValue<bool>())
                            _W.Cast(WTarget, true);
                }
                // WQ
                if (_MainMenu.Item("Alistar_CUse_WQ").GetValue<KeyBind>().Active)
                {
                    MovingPlayer(Game.CursorPos);
                    if (GetTarget == null)
                        return;
                    if (GetTarget.Position.Distance(Player.Position) < _W.Range)
                    {
                        if (_W.IsReady() && _Q.IsReady())
                        {
                            _W.Cast(GetTarget, true);
                        }
                    }
                }
                // FQ
                if (_MainMenu.Item("Alistar_FQKey").GetValue<KeyBind>().Active)
                {
                    MovingPlayer(Game.CursorPos);
                    if (GetTarget == null)
                        return;
                    QFPos = Player.Position.Extend(GetTarget.Position, 425);
                    if (GetTarget.Position.Distance(Player.Position) < 425 + _Q.Range)
                        if (_Q.IsReady() && Player.GetSpellSlot("SummonerFlash").IsReady())
                        {
                            _Q.Cast(true);
                            return;
                        }
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
                if (_MainMenu.Item("Alistar_Anti-Order").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (_Q.IsReady() && gapcloser.Sender.Position.Distance(Player.Position) < _Q.Range && AntiTime < Environment.TickCount && _MainMenu.Item("Alistar_Anti-Q").GetValue<bool>())
                    {
                        _Q.Cast(true);
                        AntiTime = TickCount(1000);
                    }
                    if (_W.IsReady() && gapcloser.Sender.Position.Distance(Player.Position) < _W.Range && AntiTime < Environment.TickCount && _MainMenu.Item("Alistar_Anti-W").GetValue<bool>())
                    {
                        _W.Cast(gapcloser.Sender, true);
                        AntiTime = TickCount(1000);
                    }
                }
                if (_MainMenu.Item("Alistar_Anti-Order").GetValue<StringList>().SelectedIndex == 1)
                {
                    if (_W.IsReady() && gapcloser.Sender.Position.Distance(Player.Position) < _W.Range && AntiTime < Environment.TickCount && _MainMenu.Item("Alistar_Anti-W").GetValue<bool>())
                    {
                        _W.Cast(gapcloser.Sender, true);
                        AntiTime = TickCount(1000);
                    }
                    if (_Q.IsReady() && gapcloser.Sender.Position.Distance(Player.Position) < _Q.Range && AntiTime < Environment.TickCount && _MainMenu.Item("Alistar_Anti-Q").GetValue<bool>())
                    {
                        _Q.Cast(true);
                        AntiTime = TickCount(1000);
                    }
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
                if (_MainMenu.Item("Alistar_FQKey").GetValue<KeyBind>().Active)
                    if (sender.IsMe && TargetSelector.GetSelectedTarget().Position.Distance(Player.Position) > _Q.Range)
                        if (args.SData.Name == "Pulverize")
                        {
                            var Flash = Player.GetSpellSlot("SummonerFlash");
                            Player.Spellbook.CastSpell(Flash, QFPos, true);
                        }
                if (_MainMenu.Item("Alistar_CUse_WQ").GetValue<KeyBind>().Active)
                {
                    if (sender.IsMe && args.SData.Name == "Headbutt")
                        if (_Q.IsReady())
                            _Q.Cast(true);
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
                if (sender.IsAlly)
                    return;
                if (_MainMenu.Item("Alistar_Inter-Q").GetValue<bool>() && _Q.IsReady() && sender.Position.Distance(Player.Position) < _Q.Range && InterTime < Environment.TickCount)
                {
                    _Q.Cast(true);
                    InterTime = TickCount(1000);
                }
                if (_MainMenu.Item("Alistar_Inter-W").GetValue<bool>() && _W.IsReady() && sender.Position.Distance(Player.Position) < _W.Range && InterTime < Environment.TickCount)
                {
                    _W.Cast(true);
                    InterTime = TickCount(1000);
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
