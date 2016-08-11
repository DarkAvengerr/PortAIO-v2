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
    class Thresh
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Thresh";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        public static HpBarIndicator Indicator = new HpBarIndicator();
        // Default Setting

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 1100f);
                _Q.SetSkillshot(0.5f, 65f, 1900f, true, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 950f);
                _E = new Spell(SpellSlot.E, 480f);
                _R = new Spell(SpellSlot.R, 450f);
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
                    Combo.AddItem(new MenuItem("Thresh_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Thresh_CUse_E", "Use E").SetValue(true));
                    Combo.AddItem(new MenuItem("Thresh_CUse_R", "Use R").SetValue(true));
                    Combo.AddItem(new MenuItem("Thresh_CUseQ_Hit", "Q HitChance").SetValue(new Slider(5, 1, 6)));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Misc = new Menu("Misc", "Misc");
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                    {
                        if (enemy.Team != Player.Team)
                        {
                            Misc.SubMenu("SetGrab").AddItem(new MenuItem("Thresh_GrabSelect" + enemy.ChampionName, enemy.ChampionName)).SetValue(new StringList(new[] { "Enable", "Dont", "Auto" }));
                        }
                    }
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Thresh_InterQ", "Use Q").SetValue(true));
                    Misc.SubMenu("Interrupt").AddItem(new MenuItem("Thresh_InterE", "Use E").SetValue(true));
                    Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("Thresh_GapQ", "Use Q").SetValue(true));
                    Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("Thresh_GapE", "Use E").SetValue(true));
                    Misc.SubMenu("Auto-Lentern").AddItem(new MenuItem("Thresh_LenternHP", "Min. HP %").SetValue(new Slider(30, 0, 100)));
                    Misc.SubMenu("Auto-Lentern").AddItem(new MenuItem("Thresh_LenternEnable", "Enable").SetValue(true));

                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Thresh_Draw_Q", "Draw Q").SetValue(false));
                    Draw.AddItem(new MenuItem("Thresh_Draw_W", "Draw W").SetValue(false));
                    Draw.AddItem(new MenuItem("Thresh_Draw_E", "Draw E").SetValue(false));
                    Draw.AddItem(new MenuItem("Thresh_Draw_R", "Draw R").SetValue(false));
                    Draw.AddItem(new MenuItem("Thresh_Indicator", "Draw Damage Indicator").SetValue(true));
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
                if (_MainMenu.Item("Thresh_Draw_Q").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Thresh_Draw_W").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Thresh_Draw_E").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (_MainMenu.Item("Thresh_Draw_R").GetValue<bool>())
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
                    if (_E.IsReady())
                        damage += _E.GetDamage(enemy);
                    if (_R.IsReady())
                        damage += _R.GetDamage(enemy);
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
                    if (_MainMenu.Item("Thresh_Indicator").GetValue<bool>())
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
        public Thresh()
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
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                // Set Target
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                var WTarget = ObjectManager.Get<AIHeroClient>().OrderByDescending(x => x.Health).FirstOrDefault(x => x.IsAlly && !x.IsDead && !x.IsMe && x.HealthPercent < _MainMenu.Item("Thresh_LenternHP").GetValue<Slider>().Value && x.Distance(Player) < 1600);
                var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                var RTarget = TargetSelector.GetTarget(_R.Range, TargetSelector.DamageType.Magical);

                if (WTarget != null && _MainMenu.Item("Thresh_LenternEnable").GetValue<bool>() && _W.IsReady())
                {
                    if (WTarget.Distance(Player) > 950)
                    {
                        _W.Cast(Player.ServerPosition.Extend(WTarget.ServerPosition, 950), true);
                    }
                    else
                    {
                        _W.Cast(WTarget.ServerPosition, true);
                    }
                }

                //Auto Q
                if (QTarget != null)
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                    {
                        if (enemy.Team != Player.Team && QTarget != null
                            && _MainMenu.Item("Thresh_GrabSelect" + enemy.ChampionName).GetValue<StringList>().SelectedIndex == 2 && _Q.IsReady()
                            && QTarget.ChampionName == enemy.ChampionName && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "ThreshQ")
                        {
                            if ((HasBuff(QTarget) || QTarget.HasBuffOfType(BuffType.Slow)) && QTarget.Position.Distance(Player.Position) <= _Q.Range * 0.9)
                            {
                                _Q.CastIfHitchanceEquals(QTarget, Hitchance("Thresh_CUseQ_Hit"), true);
                                return;
                            }
                            if (QTarget.Position.Distance(Player.Position) <= _Q.Range * 0.8)
                            {
                                _Q.CastIfHitchanceEquals(QTarget, Hitchance("Thresh_CUseQ_Hit"), true);
                                return;
                            }
                        }
                    }
                }

                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active) // Combo
                {
                    if (_MainMenu.Item("Thresh_CUse_E").GetValue<bool>() && ETarget != null && _E.IsReady())
                    {
                        var EPosition = ETarget.ServerPosition.Extend(Player.ServerPosition, 600);
                        _E.Cast(EPosition, true);
                    }
                    if (_MainMenu.Item("Thresh_CUse_Q").GetValue<bool>() && QTarget != null && _Q.IsReady() && _MainMenu.Item("Thresh_GrabSelect" + QTarget.ChampionName).GetValue<StringList>().SelectedIndex != 1
                        && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "ThreshQ")
                    {
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Thresh_CUseQ_Hit"), true);
                    }
                    if (_MainMenu.Item("Thresh_CUse_R").GetValue<bool>() && RTarget != null && _R.IsReady())
                    {
                        _R.Cast(true);
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
                if (_MainMenu.Item("Thresh_GapE").GetValue<bool>() && _E.IsReady())
                {
                    if (gapcloser.Sender.IsEnemy && gapcloser.Sender.Distance(Player) < 480)
                    {
                        _E.Cast(gapcloser.Sender.ServerPosition, true);
                    }
                }
                if (_MainMenu.Item("Thresh_GapQ").GetValue<bool>() && _Q.IsReady())
                {
                    if (gapcloser.Sender.IsEnemy && gapcloser.Sender.Distance(Player) < 1100 && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "ThreshQ")
                    {
                        _Q.CastIfHitchanceEquals(gapcloser.Sender, Hitchance("Thresh_CUseQ_Hit"), true);
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
                if (_MainMenu.Item("Thresh_InterE").GetValue<bool>() && _E.IsReady())
                {
                    if (sender.IsEnemy && sender.Distance(Player) < 480)
                    {
                        _E.Cast(sender.ServerPosition, true);
                    }
                }
                if (_MainMenu.Item("Thresh_InterQ").GetValue<bool>() && _Q.IsReady())
                {
                    if (sender.IsEnemy && sender.Distance(Player) < 1100 && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "ThreshQ")
                    {
                        _Q.CastIfHitchanceEquals(sender, Hitchance("Thresh_CUseQ_Hit"), true);
                    }
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
    }
}
