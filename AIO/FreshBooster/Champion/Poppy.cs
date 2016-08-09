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
    class Poppy
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Poppy";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        public static HpBarIndicator Indicator = new HpBarIndicator();
        // Default Setting

        public static AIHeroClient Enemy_Obj;
        public static Shield shield = new Shield();
        public static Use_R_Target Rsender = new Use_R_Target();
        public static BuffInstance PoppyPassive;
        public static bool anti_R = false;
        public class Shield
        {
            public GameObject ShieldMe;
        }

        public class Use_R_Target
        {
            public AIHeroClient Sender;
            public bool Enable = false;
            public string Type;
        }

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 430f);
                _Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W);
                _E = new Spell(SpellSlot.E, 590f);
                _R = new Spell(SpellSlot.R, 1230f);
                _R.SetSkillshot(0.6f, 120f, float.MaxValue, false, SkillshotType.SkillshotLine);
                _R.SetCharged("PoppyR", "PoppyR", 470, 1230, 1.5f);
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
                    Combo.AddItem(new MenuItem("Poppy_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Poppy_CUse_W", "Use W").SetValue(true));
                    Combo.AddItem(new MenuItem("Poppy_CUse_E", "Use E").SetValue(true));
                    Combo.AddItem(new MenuItem("Poppy_CUse_R", "Use R").SetValue(true));
                    Combo.SubMenu("Use _R to Enemy").AddItem(new MenuItem("Poppy_CUse_R_Enable", "Enable").SetValue(true));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                    {
                        if (enemy.Team != Player.Team)
                        {
                            Combo.SubMenu("Use _R to Enemy").AddItem(new MenuItem("Poppy_CUse" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
                        }
                    }
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);


                var LaneClear = new Menu("LaneClear", "LaneClear");
                {
                    LaneClear.AddItem(new MenuItem("Poppy_LUse_Q", "Use Q").SetValue(true));
                    LaneClear.AddItem(new MenuItem("Poppy_LUse_E", "Use E").SetValue(true));
                    LaneClear.AddItem(new MenuItem("LKey", "LaneClear Key").SetValue(new KeyBind('V', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(LaneClear);

                var JungleClear = new Menu("JungleClear", "JungleClear");
                {
                    JungleClear.AddItem(new MenuItem("Poppy_JUse_Q", "Use Q").SetValue(true));
                    JungleClear.AddItem(new MenuItem("Poppy_JUse_E", "Use E").SetValue(true));
                    JungleClear.AddItem(new MenuItem("JKey", "JungleClear Key").SetValue(new KeyBind('V', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(JungleClear);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Poppy_KUse_Q", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Poppy_KUse_E", "Use E").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Poppy_KUse_R", "Use R").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("Poppy_Gap_W", "Use W").SetValue(true));
                    Misc.SubMenu("Anti-GapCloser").AddItem(new MenuItem("Poppy_Gap_E", "Use E - WallBreak").SetValue(true));
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                    {
                        if (enemy.Team != Player.Team)
                        {
                            Misc.SubMenu("Anti-GapCloser").SubMenu("Use R").AddItem(new MenuItem(enemy.ChampionName, enemy.ChampionName).SetValue(true));
                        }
                    }
                    Misc.AddItem(new MenuItem("Poppy_inter_E", "Interrupt Use E").SetValue(true));
                    Misc.AddItem(new MenuItem("Poppy_inter_R", "Interrupt Use R").SetValue(true));
                    Misc.AddItem(new MenuItem("Poppy_CC", "CC use _R Super Airborne").SetValue(new KeyBind('G', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Poppy_Draw_Q", "Draw Q").SetValue(false));
                    Draw.AddItem(new MenuItem("Poppy_Draw_E", "Draw E").SetValue(false));
                    Draw.AddItem(new MenuItem("Poppy_Draw_E2", "Draw After use E").SetValue(false));
                    Draw.AddItem(new MenuItem("Poppy_Draw_R", "Draw R").SetValue(false));
                    Draw.AddItem(new MenuItem("Poppy_Indicator", "Draw Damage Indicator").SetValue(true));
                    Draw.AddItem(new MenuItem("Poppy_Shield", "Show location of My Shield").SetValue(true));
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
                if (_MainMenu.Item("Poppy_Draw_Q").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Poppy_Draw_E").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (_MainMenu.Item("Poppy_Draw_R").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                if (_MainMenu.Item("Poppy_Draw_E2").GetValue<bool>())
                {
                    var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                    if (ETarget != null)
                    {
                        var FinalPosition = ETarget.Position.Extend(ObjectManager.Player.Position, -360);
                        Drawing.DrawCircle(FinalPosition, 75, Color.Gold);
                    }
                }
                if (_MainMenu.Item("Poppy_Draw_R").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                if (shield.ShieldMe != null)
                {
                    Render.Circle.DrawCircle(shield.ShieldMe.Position, 100, Color.Red, 10);
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
                    {
                        var FinalPosition = enemy.BoundingRadius + enemy.Position.Extend(ObjectManager.Player.Position, -360);
                        if (FinalPosition.IsWall())
                        {
                            damage += (_E.GetDamage(enemy) * 2);
                        }
                        else
                        {
                            damage += _E.GetDamage(enemy);
                        }
                    }
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
                    if (_MainMenu.Item("Poppy_Indicator").GetValue<bool>())
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
        public Poppy()
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
                var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.True);
                var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Physical);
                //var RTarget = TargetSelector.GetTarget(_R.Range, TargetSelector.DamageType.Physical);
                //var RTarget = ObjectManager.Get<AIHeroClient>().OrderByDescending(x => x.Health).FirstOrDefault(x => x.IsEnemy && x.Distance(Player) < 1200);                
                PoppyPassive = ObjectManager.Player.Buffs.Find(DrawFX => DrawFX.Name == "poppypassiveshield" && DrawFX.IsValidBuff());
                if (PoppyPassive != null)
                    shield.ShieldMe = null;

                if (Rsender.Enable == true && _R.IsReady() && _R.IsCharging && Rsender.Type == "gap")
                    _R.Cast(Rsender.Sender, true);
                if (Rsender.Enable == true && _R.IsReady() && _R.IsCharging && Rsender.Type == "KillSteal" && Player.Distance(Rsender.Sender) < 1200)
                    _R.Cast(Rsender.Sender, true);
                if (Rsender.Enable == true && _R.IsReady() && _R.IsCharging && Rsender.Type == "Combo" && Player.Distance(Rsender.Sender) < 1200)
                    _R.Cast(Rsender.Sender, true);

                // KillSteal
                if (_MainMenu.Item("Poppy_KUse_Q").GetValue<bool>() && QTarget != null && QTarget.Health < _Q.GetDamage(QTarget) && _Q.IsReady())
                    _Q.Cast(QTarget, true);
                if (_MainMenu.Item("Poppy_KUse_E").GetValue<bool>() && ETarget != null && _E.IsReady())
                {
                    var FinalPosition = ETarget.BoundingRadius + ETarget.Position.Extend(ObjectManager.Player.Position, -360);
                    if (FinalPosition.IsWall() && ETarget.Health < (_E.GetDamage(ETarget) * 2))
                        _E.Cast(ETarget, true);
                    if (ETarget.Health < _E.GetDamage(ETarget))
                        _E.Cast(ETarget, true);
                }
                if (_MainMenu.Item("Poppy_KUse_R").GetValue<bool>() && _R.IsReady() && !Rsender.Enable)
                {
                    foreach (var item in ObjectManager.Get<AIHeroClient>().OrderByDescending(x => x.Health))
                    {
                        if (item.IsEnemy && !item.IsDead && item.Distance(Player) < 1200 && !_R.IsCharging && item.Health < _R.GetDamage(item))
                        {
                            _R.StartCharging();
                            Rsender.Sender = item;
                            Rsender.Enable = true;
                            Rsender.Type = "KillSteal";
                        }
                    }
                }
                // KillSteal

                //CC
                if (_MainMenu.Item("Poppy_CC").GetValue<KeyBind>().Active)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    if (_R.IsReady())
                    {
                        foreach (var item in ObjectManager.Get<AIHeroClient>().OrderByDescending(x => x.IsEnemy))
                        {
                            if (item.Distance(Player) < 600 && item.Distance(Game.CursorPos) < 200)
                            {
                                if (_R.IsCharging)
                                {
                                    _R.Cast(item, true);
                                }
                                else
                                {
                                    _R.StartCharging();
                                }
                            }
                        }
                    }
                }

                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active) // Combo
                {
                    if (_MainMenu.Item("Poppy_CUse_R").GetValue<bool>() && _R.IsReady() && !Rsender.Enable)
                    {
                        if (_MainMenu.Item("Poppy_CUse_R_Enable").GetValue<bool>())
                        {
                            foreach (var item in ObjectManager.Get<AIHeroClient>().OrderBy(x => x.MaxHealth))
                            {
                                if (item.IsEnemy && !item.IsDead && _MainMenu.Item("Poppy_CUse" + item.ChampionName).GetValue<bool>() && item.Distance(Player) < 1200)
                                {
                                    _R.StartCharging();
                                    Rsender.Sender = item;
                                    Rsender.Enable = true;
                                    Rsender.Type = "Combo";
                                }
                            }
                        }
                        else
                        {
                            var RTarget = ObjectManager.Get<AIHeroClient>().OrderBy(x => x.MaxHealth).FirstOrDefault(x => x.IsEnemy && x.Distance(Player) < 1200);
                            _R.StartCharging();
                            Rsender.Sender = RTarget;
                            Rsender.Enable = true;
                            Rsender.Type = "Combo";
                        }
                    }
                    if (_MainMenu.Item("Poppy_CUse_Q").GetValue<bool>() && QTarget != null && _Q.IsReady())
                        _Q.Cast(QTarget, true);
                    //벽꿍
                    if (_MainMenu.Item("Poppy_CUse_E").GetValue<bool>() && ETarget != null && _E.IsReady())
                    {
                        var FinalPosition = ETarget.BoundingRadius + ETarget.Position.Extend(ObjectManager.Player.Position, -360);
                        if (FinalPosition.IsWall())
                        {
                            _E.Cast(ETarget, true);
                        }
                    }
                }

                if (_MainMenu.Item("LKey").GetValue<KeyBind>().Active) // LaneClear
                {
                    var MinionTarget = MinionManager.GetMinions(1100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                    foreach (var minion in MinionTarget)
                    {
                        if (_Q.IsReady() && _MainMenu.Item("Poppy_LUse_Q").GetValue<bool>() && minion != null && !Orbwalking.CanAttack() && Orbwalking.CanMove(5))
                        {
                            _Q.Cast(minion, true);

                        }
                        if (_E.IsReady() && _MainMenu.Item("Poppy_LUse_E").GetValue<bool>() && minion != null && !Orbwalking.CanAttack() && Orbwalking.CanMove(5))
                        {
                            _E.Cast(minion, true);
                        }
                    }
                }
                if (_MainMenu.Item("JKey").GetValue<KeyBind>().Active) // JungleClear
                {
                    var JungleTarget = MinionManager.GetMinions(1100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    foreach (var minion in JungleTarget)
                    {
                        if (_Q.IsReady() && _MainMenu.Item("Poppy_JUse_Q").GetValue<bool>() && minion != null && !Orbwalking.CanAttack() && Orbwalking.CanMove(5))
                        {
                            _Q.Cast(minion, true);
                        }
                        if (_E.IsReady() && _MainMenu.Item("Poppy_JUse_E").GetValue<bool>() && minion != null && !Orbwalking.CanAttack() && Orbwalking.CanMove(5))
                        {
                            _E.Cast(minion, true);
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
                if (_MainMenu.Item("Poppy_Gap_W").GetValue<bool>() && _W.IsReady())
                {
                    _W.Cast();
                    return;
                }

                if (_MainMenu.Item("Poppy_Gap_E").GetValue<bool>() && _E.IsReady())
                {
                    var FinalPosition = gapcloser.Sender.BoundingRadius + gapcloser.Sender.Position.Extend(ObjectManager.Player.Position, -360);
                    if (FinalPosition.IsWall())
                        _E.Cast(gapcloser.Sender, true);
                    return;
                }
                if (_MainMenu.Item(gapcloser.Sender.ChampionName).GetValue<bool>() && _R.IsReady())
                {
                    if (!_R.IsCharging)
                    {
                        _R.StartCharging();
                        Rsender.Sender = gapcloser.Sender;
                        Rsender.Enable = true;
                        Rsender.Type = "gap";
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
                if (sender.IsMe)
                {
                    if (args.SData.Name == "PoppyRSpell")
                        Rsender.Enable = false;
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
                if (sender.IsEnemy && sender.IsValid<AIHeroClient>())
                {
                    if (_MainMenu.Item("Poppy_inter_E").GetValue<bool>() && _E.IsReady() && Player.Distance(sender) < 525)
                    {
                        _E.Cast(sender, true);
                    }
                    if (_MainMenu.Item("Poppy_inter_R").GetValue<bool>() && _R.IsReady() && Player.Distance(sender) < 1200)
                    {
                        if (_R.IsCharging)
                        {
                            _R.Cast(sender, true);
                        }
                        else
                        {
                            _R.StartCharging();
                        }
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
                if (_R.IsChanneling)
                    args.Process = false;
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
                if (obj.IsAlly && obj.Name == "Shield")
                {
                    shield.ShieldMe = obj;
                }
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
                if (obj.IsAlly && obj.Name == "Shield")
                {
                    shield.ShieldMe = null;
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                Chat.Print("FreshPoppy is not working. plz send message by KorFresh (Code 14)");
            }
        }
    }
}
