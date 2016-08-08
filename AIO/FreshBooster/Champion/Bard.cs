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
    class Bard
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Bard";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        public static HpBarIndicator Indicator = new HpBarIndicator();
        // Default Setting

        public static int cnt = 0;
        public static Obj_AI_Base BardQTarget1, BardQTarget2;
        public static Geometry.Polygon.Rectangle Range1, Range2;
        public static AIHeroClient RRange;
        public static int RCnt;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 950f);
                _Q.SetSkillshot(0.4f, 120f, 1400f, false, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 1000);
                _E = new Spell(SpellSlot.E, 900);
                _R = new Spell(SpellSlot.R, 3400f);
                _R.SetSkillshot(0.5f, 340f, 1400f, false, SkillshotType.SkillshotCircle);
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
                    Combo.AddItem(new MenuItem("Bard_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Bard_CUse_Q_Hit", "Q HitChance").SetValue(new Slider(3, 1, 6)));
                    Combo.AddItem(new MenuItem("Bard_CUse_OnlyQ", "Only use Q sturn").SetValue(true));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {

                    Harass.AddItem(new MenuItem("Bard_HUse_Q", "Use Q").SetValue(true));
                    Harass.AddItem(new MenuItem("Bard_HUse_OnlyQ", "Only use Q sturn").SetValue(true));
                    Harass.AddItem(new MenuItem("Bard_AManarate", "Mana %").SetValue(new Slider(20)));
                    Harass.AddItem(new MenuItem("Bard_Auto_HEnable", "Auto Harass").SetValue(true));
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Harass);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Bard_KUse_Q", "Use Q").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.SubMenu("Heal W").AddItem(new MenuItem("Bard_HealWMin", "Min HP %").SetValue(new Slider(20, 0, 100)));
                    Misc.SubMenu("Heal W").AddItem(new MenuItem("Bard_HealWMinEnable", "Enable").SetValue(true));
                    Misc.AddItem(new MenuItem("Bard_Anti", "Anti-Gabcloser Q").SetValue(true));
                    Misc.AddItem(new MenuItem("Bard_Inter", "Interrupt R").SetValue(true));
                    Misc.AddItem(new MenuItem("BardRKey", "Ult R").SetValue(new KeyBind('G', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Bard_Draw_Q", "Draw Q").SetValue(false));
                    Draw.AddItem(new MenuItem("Bard_Draw_W", "Draw W").SetValue(false));
                    Draw.AddItem(new MenuItem("Bard_Draw_E", "Draw E").SetValue(false));
                    Draw.AddItem(new MenuItem("Bard_Draw_R", "Draw R").SetValue(false));
                    Draw.AddItem(new MenuItem("Bard_Draw_R1", "Draw R in MiniMap").SetValue(false));
                    Draw.AddItem(new MenuItem("Bard_Indicator", "Draw Damage Indicator").SetValue(true));
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
                if (_MainMenu.Item("Bard_Draw_Q").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Bard_Draw_W").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Bard_Draw_E").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Bard_Draw_R").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                if (_MainMenu.Item("Bard_Draw_R1").GetValue<bool>())
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, _R.Range, Color.White, 1, 23, true);
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                if (QTarget != null)
                    Drawing.DrawCircle(QTarget.Position, 150, Color.Green);
                if (_MainMenu.Item("BardRKey").GetValue<KeyBind>().Active)
                {
                    Render.Circle.DrawCircle(Game.CursorPos, 340, Color.Aqua);
                    //Drawing.DrawCircle(Game.CursorPos, _R.Range, Color.AliceBlue);
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
                    if (_MainMenu.Item("Bard_Indicator").GetValue<bool>())
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
        public Bard()
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

                // Set Target
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);

                //Kill
                if (_MainMenu.Item("Bard_KUse_Q").GetValue<bool>())
                    if (QTarget != null)
                        if (_Q.LSIsReady())
                            if (QTarget.Health < _Q.GetDamage(QTarget))
                                _Q.CastIfHitchanceEquals(QTarget, HitChance.High, true);

                // W
                if (_MainMenu.Item("Bard_HealWMinEnable").GetValue<bool>() && !Player.LSIsRecalling())
                {
                    var ally = HeroManager.Allies.OrderBy(f => f.Health).FirstOrDefault(f => f.LSDistance(Player.Position) < _W.Range && !f.IsDead && !f.IsZombie && f.HealthPercent < _MainMenu.Item("Bard_HealWMin").GetValue<Slider>().Value);
                    if (ally != null && _W.LSIsReady() && !ally.LSInFountain(LeagueSharp.Common.Utility.FountainType.OwnFountain))
                        _W.CastOnUnit(ally, true);
                }

                //R
                if (_MainMenu.Item("BardRKey").GetValue<KeyBind>().Active && _R.LSIsReady())
                {
                    RCnt = 0;
                    var range = HeroManager.AllHeroes.OrderBy(f => Game.CursorPos.LSDistance(f.Position) < 340f);
                    if (range == null)
                        return;
                    foreach (var item in range)
                    {
                        RCnt++;
                    }
                    if (RCnt == 0)
                        return;
                    var target = range.FirstOrDefault(f => f.LSDistance(Game.CursorPos) < 340f);
                    _R.SetSkillshot(Player.LSDistance(target.Position) * 3400 / 1.4f, 340f, 1400f, false, SkillshotType.SkillshotCircle);
                    if (target != null)
                        _R.CastIfHitchanceEquals(target, HitChance.Medium, true);
                }

                // Combo
                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                {
                    if (_MainMenu.Item("Bard_CUse_Q").GetValue<bool>() && _Q.LSIsReady() && QTarget != null)
                    {
                        BardQ(QTarget, true);
                        if (_MainMenu.Item("Bard_CUse_OnlyQ").GetValue<bool>())
                        {
                            if (cnt == 2)
                                if (BardQTarget1 is AIHeroClient || BardQTarget2 is AIHeroClient)
                                    _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                            if (cnt == 1 && BardQTarget1 is AIHeroClient && BardQTarget1.Position.LSExtend(Player.Position, -450).LSIsWall())
                                _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                        }
                        else
                        {
                            BardQ(QTarget, false);
                            if (BardQTarget1 == QTarget || BardQTarget2 == QTarget)
                                _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                        }
                    }
                }

                // Harass
                if (_MainMenu.Item("HKey").GetValue<KeyBind>().Active || _MainMenu.Item("Bard_Auto_HEnable").GetValue<bool>())
                {
                    if (_MainMenu.Item("Bard_HUse_Q").GetValue<bool>() && _Q.LSIsReady() && QTarget != null && _MainMenu.Item("Bard_AManarate").GetValue<Slider>().Value < Player.ManaPercent)
                    {
                        BardQ(QTarget, true);
                        // Sturn
                        if (_MainMenu.Item("Bard_HUse_OnlyQ").GetValue<bool>())
                        {
                            if (cnt == 2 && Range2 != null)
                                if ((BardQTarget1 is AIHeroClient && BardQTarget1 != Player) || (BardQTarget2 is AIHeroClient && BardQTarget2 != Player))
                                {
                                    _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                                    Console.WriteLine("1");
                                }

                            if (cnt == 1 && BardQTarget1 is AIHeroClient && BardQTarget1.Position.LSExtend(Player.Position, -400).LSIsWall())
                            {
                                _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                                Console.WriteLine("2");
                            }
                        }
                        else
                        {
                            BardQ(QTarget, false);
                            if (BardQTarget1 == QTarget || BardQTarget2 == QTarget)
                            {
                                _Q.CastIfHitchanceEquals(BardQTarget1, Hitchance("Bard_CUse_Q_Hit"), true);
                                Console.WriteLine("3");
                            }
                        }
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
                if (_MainMenu.Item("Bard_Anti").GetValue<bool>() && _Q.LSIsReady() && gapcloser.Sender.LSDistance(Player.Position) < _Q.Range)
                    _Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.Medium, true);
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
        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {

        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if (_MainMenu.Item("Bard_Inter").GetValue<bool>() && _R.LSIsReady() && sender.LSDistance(Player.Position) < _R.Range)
                {
                    _R.SetSkillshot(Player.LSDistance(sender.Position) * 3400 / 1.5f, 340f, 1400f, false, SkillshotType.SkillshotCircle);
                    _R.CastIfHitchanceEquals(sender, HitChance.Medium, true);
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

        private static void BardQ(AIHeroClient Target, bool Type, bool Draw = false)
        {
            // Type 0: no sturn / 1: only sturn
            // If Draw is true, return draw
            /* return
            target1, target2, type
            */            
            Range1 = new Geometry.Polygon.Rectangle(Player.Position, Player.Position.LSExtend(Target.Position, _Q.Range), _Q.Width);
            Range2 = null;
            if (Draw)
                Range1.Draw(Color.Red);
            cnt = 0;
            BardQTarget1 = Player;
            BardQTarget2 = Player;
            foreach (var item in ObjectManager.Get<Obj_AI_Base>().OrderBy(f => f.LSDistance(f.Position)))
            {
                if (item.LSDistance(Player.Position) < _Q.Range)
                    if (item is AIHeroClient || item is Obj_AI_Minion)
                        if (item.IsEnemy && !item.IsDead)
                        {
                            if (cnt == 2)
                                break;
                            if (cnt == 0 && Range1.IsInside(item.Position))
                            {
                                BardQTarget1 = item;
                                Range2 = new Geometry.Polygon.Rectangle(Player.Position.LSExtend(BardQTarget1.Position, Player.LSDistance(BardQTarget1.Position)),
                                    Player.Position.LSExtend(BardQTarget1.Position, Player.LSDistance(BardQTarget1.Position) + 450), _Q.Width);
                                if (Draw)
                                    Range2.Draw(Color.Yellow);
                                cnt++;
                            }
                            if (cnt == 1 && Range2.IsInside(item.Position))
                            {
                                BardQTarget2 = item;
                                cnt++;
                            }
                        }
            }
        }
    }
}
