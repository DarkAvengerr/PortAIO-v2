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
    class TahmKench
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "TahmKench";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _W2, _E, _R;
        public static HpBarIndicator Indicator = new HpBarIndicator();
        // Default Setting

        public static BuffInstance EnemyBuff;
        public static AIHeroClient PassiveEnemy;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 850);
                _Q.SetSkillshot(0.75f, 75f, 2000, true, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 300);
                _W2 = new Spell(SpellSlot.W, 700);
                _W2.SetSkillshot(0.5f, 75f, 950, true, SkillshotType.SkillshotLine);
                _E = new Spell(SpellSlot.E);
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
                    Combo.AddItem(new MenuItem("TahmKench_CUse_Q", "Use Q").SetValue(true));
                    Combo.SubMenu("Use W").AddItem(new MenuItem("TahmKench_CUse_W1", "Devour Enemy").SetValue(true));
                    Combo.SubMenu("Use W").AddItem(new MenuItem("TahmKench_CUse_W2", "Regurgitate Minion").SetValue(true));
                    Combo.AddItem(new MenuItem("TahmKench_CUse_E", "Use E").SetValue(true));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {
                    Harass.AddItem(new MenuItem("TahmKench_HUse_Q", "Use Q").SetValue(true));
                    Harass.AddItem(new MenuItem("TahmKench_HUse_W2", "Regurgitate Minion").SetValue(true));
                    Harass.AddItem(new MenuItem("TahmKench_HManarate", "Mana %").SetValue(new Slider(50)));
                    Harass.AddItem(new MenuItem("TahmKench_HToggle", "Auto Enable").SetValue(false));
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Harass);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("TahmKench_KUse_Q", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("TahmKench_KUse_W", "Use W").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.SubMenu("Auto W").AddItem(new MenuItem("TahmKench_MyTeamCC", "When Ally has CC").SetValue(true));
                    Misc.SubMenu("Auto W").AddItem(new MenuItem("TahmKench_MyTeamMin", "Attck to Ally. Min HP %").SetValue(new Slider(15, 0, 100)));
                    Misc.SubMenu("Auto W").AddItem(new MenuItem("TahmKench_MyTeamMinEnable", "Attck to Ally Enable").SetValue(true));
                    Misc.SubMenu("Auto E").AddItem(new MenuItem("TahmKench_MinE", "Min. HP %").SetValue(new Slider(20, 0, 100)));
                    Misc.SubMenu("Auto E").AddItem(new MenuItem("TahmKench_AutoEUse", "Enable").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("TahmKench_QRange", "Q Range").SetValue(false));
                    Draw.AddItem(new MenuItem("TahmKench_WRange", "W Range").SetValue(false));
                    Draw.AddItem(new MenuItem("TahmKench_WRange2", "W2 Range").SetValue(false));
                    Draw.AddItem(new MenuItem("TahmKench_RRange", "R Range to Screen").SetValue(false));
                    Draw.AddItem(new MenuItem("TahmKench_RRange1", "R Range to MiniMap").SetValue(false));
                    Draw.AddItem(new MenuItem("TahmKench_Indicator", "Draw Damage Indicator").SetValue(false));
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
                if (_MainMenu.Item("TahmKench_QRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("TahmKench_WRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("TahmKench_WRange2").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W2.Range, Color.White, 1);
                if (_MainMenu.Item("TahmKench_RRange").GetValue<bool>() && _R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, Rint, Color.White, 1);
                if (_MainMenu.Item("TahmKench_RRange1").GetValue<bool>() && _R.Level > 0)
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, Rint, Color.White, 1, 23, true);
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
                    if (_MainMenu.Item("TahmKench_Indicator").GetValue<bool>())
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
        public TahmKench()
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

                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                var WTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);
                var WTarget2 = TargetSelector.GetTarget(_W2.Range, TargetSelector.DamageType.Magical);

                if (_MainMenu.Item("TahmKench_MyTeamCC").GetValue<bool>() && _W.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name != "tahmkenchwspitready")
                {
                    var Ally = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsAlly && !x.IsMe && !x.IsDead && !x.IsZombie && x.ServerPosition.LSDistance(Player.ServerPosition) < _W.Range);
                    if (Ally != null && HasBuff(Ally))
                        _W.CastOnUnit(Ally, true);
                }

                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active) // Combo
                {
                    if (WTarget2 != null && _W.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name == "tahmkenchwspitready")
                    {
                        _W2.CastIfHitchanceEquals(WTarget2, HitChance.Low, true);
                    }
                    if (WTarget2 != null && _E.LSIsReady() && Player.HealthPercent <= 7 && _MainMenu.Item("TahmKench_CUse_E").GetValue<bool>())
                        _E.Cast(true);
                    if (QTarget != null && _Q.LSIsReady() && _MainMenu.Item("TahmKench_CUse_Q").GetValue<bool>())
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.Low, true);
                    if (WTarget != null && _W.LSIsReady() && WTarget.Buffs.Find(x => x.Name == "tahmkenchpdebuffcounter" && x.LSIsValidBuff()).Count == 3 && _MainMenu.Item("TahmKench_CUse_W1").GetValue<bool>())
                        _W.CastOnUnit(WTarget, true);
                    if (WTarget2 != null && _W.LSIsReady() && _MainMenu.Item("TahmKench_CUse_W2").GetValue<bool>())
                    {
                        if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "tahmkenchw")
                        {
                            var getminion = MinionManager.GetMinions(_W.Range, MinionTypes.All, MinionTeam.NotAlly).OrderBy(x => x.LSDistance(Player.ServerPosition)).FirstOrDefault();
                            if (getminion != null)
                                _W.CastOnUnit(getminion, true);
                        }
                    }
                }

                if ((_MainMenu.Item("HKey").GetValue<KeyBind>().Active || _MainMenu.Item("TahmKench_HToggle").GetValue<bool>())
                    && _MainMenu.Item("TahmKench_HManarate").GetValue<Slider>().Value < Player.ManaPercent) // Harass
                {
                    if (QTarget != null && _Q.LSIsReady() && _MainMenu.Item("TahmKench_HUse_Q").GetValue<bool>())
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.Low, true);
                    if (WTarget2 != null && _W.LSIsReady() && _MainMenu.Item("TahmKench_HUse_W2").GetValue<bool>())
                    {
                        if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "tahmkenchwspitready")
                        {
                            _W2.CastIfHitchanceEquals(WTarget2, HitChance.Low, true);
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

                if (!sender.IsMe && sender.IsEnemy)
                {
                    if (_MainMenu.Item("TahmKench_MyTeamMinEnable").GetValue<bool>() && sender.IsEnemy && _W.LSIsReady())
                    {
                        var SkillRange = new Geometry.Polygon.Rectangle(args.Start, args.End, sender.BoundingRadius + 30);
                        var Target = HeroManager.Allies.FirstOrDefault(f => f.LSDistance(Player.Position) <= _W.Range
                        && !f.IsZombie && !f.IsDead && SkillRange.IsInside(f.Position)
                        && _MainMenu.Item("TahmKench_MyTeamMin").GetValue<Slider>().Value >= f.HealthPercent);
                        if (Target != null)
                        {
                            _W.CastOnUnit(Target, true);
                            return;
                        }
                        if (args.Target != null)
                        {
                            var target = HeroManager.Allies.FirstOrDefault(f => f.Position.LSDistance(Player.Position) <= _W.Range
                            && !f.IsZombie && !f.IsDead && f.HealthPercent <= _MainMenu.Item("TahmKench_MyTeamMin").GetValue<Slider>().Value);
                            if (target != null)
                            {
                                _W.CastOnUnit(target, true);
                                return;
                            }
                        }
                    }
                    if (_MainMenu.Item("TahmKench_AutoEUse").GetValue<bool>() && sender.IsEnemy && _E.LSIsReady() && Player.HealthPercent <= _MainMenu.Item("TahmKench_MinE").GetValue<Slider>().Value)
                    {
                        var target = new Geometry.Polygon.Rectangle(args.Start, args.End, sender.BoundingRadius + 30);
                        if (target != null)
                        {
                            _E.Cast(true);
                            return;
                        }
                        if (args.Target != null)
                        {
                            var TargetMe = HeroManager.Allies.FirstOrDefault(f => f.Position.LSDistance(Player.Position) <= 10);
                            if (TargetMe == Player)
                            {
                                _E.Cast(true);
                                return;
                            }
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
