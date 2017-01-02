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
    class Veigar
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Veigar";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        
        // Default Setting

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 900);
                _Q.SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 900);
                _W.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                _E = new Spell(SpellSlot.E, 1150); // true range 750, circle 400
                _E.SetSkillshot(1.2f, 80f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                _R = new Spell(SpellSlot.R, 615);
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
                    Combo.AddItem(new MenuItem("Veigar_CUseQ", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Veigar_CUseW", "Use W").SetValue(true));
                    Combo.AddItem(new MenuItem("Veigar_CUseE", "Use E").SetValue(true));
                    Combo.AddItem(new MenuItem("Veigar_CUseR", "Use R").SetValue(true));
                    Combo.AddItem(new MenuItem("Veigar_CUseR_Select", "When can be Kill, Only use R").SetValue(true));
                    Combo.AddItem(new MenuItem("Veigar_CUseQ_Hit", "Q HitChance").SetValue(new Slider(3, 1, 6)));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {
                    Harass.AddItem(new MenuItem("Veigar_HUseQ", "Use Q").SetValue(true));
                    Harass.AddItem(new MenuItem("Veigar_HUseW", "Use W - When target can't only move").SetValue(true));
                    Harass.AddItem(new MenuItem("Veigar_HUseE", "Use E - When target can't only move").SetValue(true));
                    Harass.AddItem(new MenuItem("Veigar_HUseR", "Use R").SetValue(true));
                    Harass.AddItem(new MenuItem("Veigar_HManarate", "Mana %").SetValue(new Slider(20)));
                    Harass.AddItem(new MenuItem("Veigar_AutoHUseQ", "Auto Harass").SetValue(new KeyBind('T', KeyBindType.Toggle)));
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Harass);

                var LaneClear = new Menu("LaneClear", "LaneClear");
                {
                    LaneClear.AddItem(new MenuItem("Veigar_LUseQ", "Use Q").SetValue(true));
                    LaneClear.AddItem(new MenuItem("Veigar_LUseQSet", "Use Q Only use lasthit to minion").SetValue(true));
                    LaneClear.AddItem(new MenuItem("Veigar_LManarate", "Mana %").SetValue(new Slider(20)));
                    LaneClear.AddItem(new MenuItem("LKey", "LaneClear Key").SetValue(new KeyBind('V', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(LaneClear);

                var JungleClear = new Menu("JungleClear", "JungleClear");
                {
                    JungleClear.AddItem(new MenuItem("Veigar_JUseQ", "Use Q").SetValue(true));
                    JungleClear.AddItem(new MenuItem("Veigar_JUseQSet", "Use Q Only use lasthit to minion").SetValue(true));
                    JungleClear.AddItem(new MenuItem("Veigar_JManarate", "Mana %").SetValue(new Slider(20)));
                    JungleClear.AddItem(new MenuItem("JKey", "JungleClear Key").SetValue(new KeyBind('V', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(JungleClear);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Veigar_KseQ", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Veigar_KseW", "Use W").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Veigar_KseR", "Use R").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.AddItem(new MenuItem("Veigar_Anti-GapCloser", "Anti GapCloser").SetValue(true));
                    Misc.AddItem(new MenuItem("Veigar_Interrupt", "E with Interrupt").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Veigar_Draw_Q", "Draw Q").SetValue(false));
                    Draw.AddItem(new MenuItem("Veigar_Draw_W", "Draw W").SetValue(false));
                    Draw.AddItem(new MenuItem("Veigar_Draw_E", "Draw E").SetValue(false));
                    Draw.AddItem(new MenuItem("Veigar_Draw_R", "Draw R").SetValue(false));
                    Draw.AddItem(new MenuItem("Veigar_Indicator", "Draw Damage Indicator").SetValue(true));
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
                if (Player.IsDead)
                    return;
                if (_MainMenu.Item("Veigar_Draw_Q").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Veigar_Draw_W").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Veigar_Draw_E").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (_MainMenu.Item("Veigar_Draw_R").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                var QTarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
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
        public Veigar()
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
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                var WTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);
                var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                var RTarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);

                var KTarget = ObjectManager.Get<AIHeroClient>().OrderBy(x => x.Health).FirstOrDefault(x => x.IsEnemy && Player.Distance(x) < 900);
                //KillSteal
                if (KTarget != null && !KTarget.IsDead && KTarget.IsEnemy && KTarget.Health > 0)
                {
                    if (_MainMenu.Item("Veigar_KseQ").GetValue<bool>() && _Q.IsReady() && KTarget.Health < _Q.GetDamage(KTarget) && KTarget.Distance(Player) <= _Q.Range)
                    {
                        _Q.CastIfHitchanceEquals(KTarget, Hitchance("Veigar_CUseQ_Hit"), true);
                        return;
                    }
                    if (_MainMenu.Item("Veigar_KseR").GetValue<bool>() && _R.IsReady() && KTarget.Health < _R.GetDamage(KTarget) && KTarget.Distance(Player) <= _R.Range)
                    {
                        _R.Cast(KTarget, true);
                        return;
                    }

                    if (_MainMenu.Item("Veigar_KseW").GetValue<bool>() && _W.IsReady() && !KTarget.CanMove && KTarget.Health < _W.GetDamage(KTarget) && KTarget.Distance(Player) <= _W.Range)
                    {
                        _W.CastIfHitchanceEquals(KTarget, HitChance.VeryHigh, true);
                        return;
                    }
                }

                //Combo
                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                {
                    if (_MainMenu.Item("Veigar_CUseE").GetValue<bool>() && ETarget != null && _E.IsReady())
                    {
                        SpellUseE(ETarget);
                    }
                    if (_MainMenu.Item("Veigar_CUseW").GetValue<bool>() && WTarget != null && _W.IsReady())
                    {
                        _W.CastIfHitchanceEquals(WTarget, HitChance.VeryHigh, true);
                        return;
                    }
                    if (_MainMenu.Item("Veigar_CUseQ").GetValue<bool>() && QTarget != null && _Q.IsReady())
                    {
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Veigar_CUseQ_Hit"), true);
                        return;
                    }
                    if (_MainMenu.Item("Veigar_CUseR").GetValue<bool>() && RTarget != null && _R.IsReady() && !_MainMenu.Item("Veigar_CUseR_Select").GetValue<bool>())
                    {
                        _R.Cast(RTarget, true);
                        return;
                    }
                }

                //Harass
                if ((_MainMenu.Item("HKey").GetValue<KeyBind>().Active || _MainMenu.Item("Veigar_AutoHUseQ").GetValue<KeyBind>().Active)
                    && _MainMenu.Item("Veigar_HManarate").GetValue<Slider>().Value < Player.ManaPercent)
                {
                    if (_MainMenu.Item("Veigar_HUseE").GetValue<bool>() && ETarget != null && _E.IsReady() && !ETarget.CanMove)
                    {
                        SpellUseE(ETarget);
                    }
                    if (_MainMenu.Item("Veigar_HUseW").GetValue<bool>() && WTarget != null && _W.IsReady() && !WTarget.CanMove)
                    {
                        _W.CastIfHitchanceEquals(WTarget, HitChance.VeryHigh, true);
                        return;
                    }
                    if (_MainMenu.Item("Veigar_HUseQ").GetValue<bool>() && QTarget != null && _Q.IsReady())
                    {
                        _Q.CastIfHitchanceEquals(QTarget, Hitchance("Veigar_CUseQ_Hit"), true);
                        return;
                    }
                    if (_MainMenu.Item("Veigar_HUseR").GetValue<bool>() && RTarget != null && _R.IsReady())
                    {
                        _R.Cast(RTarget, true);
                        return;
                    }
                }

                //LaneClear
                if (_MainMenu.Item("LKey").GetValue<KeyBind>().Active && _MainMenu.Item("Veigar_LManarate").GetValue<Slider>().Value < Player.ManaPercent)
                {
                    var MinionTarget = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                    foreach (var item in MinionTarget)
                    {
                        if (_MainMenu.Item("Veigar_LUseQ").GetValue<bool>())
                        {
                            if (_MainMenu.Item("Veigar_LUseQSet").GetValue<bool>() && item.Health < _Q.GetDamage(item))
                            {
                                _Q.CastIfHitchanceEquals(item, HitChance.Low, true);
                                return;
                            }
                            if (!_MainMenu.Item("Veigar_LUseQSet").GetValue<bool>())
                            {
                                _Q.CastIfHitchanceEquals(item, HitChance.Low, true);
                                return;
                            }
                        }
                    }
                }

                //JungleClear
                if (_MainMenu.Item("JKey").GetValue<KeyBind>().Active && _MainMenu.Item("Veigar_JManarate").GetValue<Slider>().Value < Player.ManaPercent)
                {
                    var MinionTarget = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health);
                    foreach (var item in MinionTarget)
                    {
                        if (_MainMenu.Item("Veigar_JUseQ").GetValue<bool>())
                        {
                            if (_MainMenu.Item("Veigar_JUseQSet").GetValue<bool>() && item.Health < _Q.GetDamage(item))
                            {
                                _Q.CastIfHitchanceEquals(item, HitChance.Low, true);
                                return;
                            }
                            if (!_MainMenu.Item("Veigar_JUseQSet").GetValue<bool>())
                            {
                                _Q.CastIfHitchanceEquals(item, HitChance.Low, true);
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
                    //Chat.Print(ChampName + " in FreshBooster isn't Load. Error Code 06");
                    ErrorTime = TickCount(10000);
                }
            }
        }
        public static void SpellUseE(AIHeroClient target)
        {
            if (!target.CanMove && !target.IsMoving)
            {
                var EPosition = target.ServerPosition.Extend(Player.Position, 400);
                _E.Cast(EPosition, true);
                return;
            }
            if (target.Distance(Player) > 750)
            {
                var EPosition = target.ServerPosition.Extend(Player.Position, 200);
                _E.Cast(EPosition, true);
                return;
            }
            else
            {
                var EPosition = target.ServerPosition;
                _E.Cast(EPosition, true);
                return;
            }
        }
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            try
            {
                if (_MainMenu.Item("Veigar_Anti-GapCloser").GetValue<bool>() && _E.IsReady())
                {
                    var EPosition = gapcloser.End.Extend(Player.ServerPosition, 400);
                    _E.Cast(EPosition, true);
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
                if (_MainMenu.Item("Veigar_Interrupt").GetValue<bool>() && sender.IsEnemy && _E.IsReady() && sender.Distance(Player) < 1150)
                {
                    var EPosition = sender.ServerPosition.Extend(Player.Position, 400);
                    _E.Cast(EPosition, true);
                    return;
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
