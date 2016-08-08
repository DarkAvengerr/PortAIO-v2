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
    class Leblanc
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Leblanc";   // Edit
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        public static HpBarIndicator Indicator = new HpBarIndicator();
        // Default Setting

        public static List<Spell> SpellList = new List<Spell>();
        public static List<int> SpellUseTree = new List<int>();
        public static string[] SpellName = new string[]
        {
            "LeblancChaosOrb","LeblancChaosOrbM",   // 0,1
            "LeblancSlide", "LeblancSlideM", "leblancslidereturn","leblancslidereturnm",    // 2,3,4,5
            "LeblancSoulShackle","LeblancSoulShackleM"     // 6,7
        };
        public static int SpellUseCnt = 0, SpellUseCnt1 = 0, ERCC = 0;
        public static int SpellUseTime = 0, ERTIME = 0, WTime = 0, RTime = 0, ReturnTime = 0, PetTime = 0;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 720);
                _Q.SetTargetted(0.5f, 1500f);
                _W = new Spell(SpellSlot.W, 700);
                _W.SetSkillshot(0.6f, 220f, 1450f, false, SkillshotType.SkillshotCircle);
                _E = new Spell(SpellSlot.E, 900);
                _E.SetSkillshot(0.3f, 55f, 1650f, true, SkillshotType.SkillshotLine);
                _R = new Spell(SpellSlot.R, 720);
                {
                    SpellList.Add(_Q);
                    SpellList.Add(_W);
                    SpellList.Add(_E);
                    SpellList.Add(_R);
                }
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
                    Combo.AddItem(new MenuItem("Leblanc_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Leblanc_CUse_W", "Use W").SetValue(true));
                    Combo.AddItem(new MenuItem("Leblanc_CUse_WReturn", "Use W Return").SetValue(true));
                    Combo.AddItem(new MenuItem("Leblanc_CUse_E", "Use E").SetValue(true));
                    Combo.SubMenu("Use R").AddItem(new MenuItem("Leblanc_CUse_Q2", "Use Q").SetValue(true));
                    Combo.SubMenu("Use R").AddItem(new MenuItem("Leblanc_CUse_W2", "Use W").SetValue(true));
                    Combo.SubMenu("Use R").AddItem(new MenuItem("Leblanc_CUse_W2Return", "Use W Return").SetValue(true));
                    Combo.SubMenu("Use R").AddItem(new MenuItem("Leblanc_CUse_E2", "Use E").SetValue(true));
                    Combo.AddItem(new MenuItem("Leblanc_CUseE_Hit", "E HitChance").SetValue(new Slider(3, 1, 6)));
                    Combo.AddItem(new MenuItem("Leblanc_ComboMode", "Combo Mode is Teamfight").SetValue(new KeyBind('N', KeyBindType.Toggle)));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                    Harass.SubMenu("Auto Harass").AddItem(new MenuItem("Leblanc_AUse_Q", "Use Q").SetValue(true));
                    Harass.SubMenu("Auto Harass").AddItem(new MenuItem("Leblanc_AUse_W", "Use W").SetValue(true));
                    Harass.SubMenu("Auto Harass").AddItem(new MenuItem("Leblanc_AUse_E", "Use E").SetValue(true));
                    Harass.AddItem(new MenuItem("Leblanc_AManarate", "Mana %").SetValue(new Slider(20)));
                    Harass.SubMenu("Auto Harass").AddItem(new MenuItem("Leblanc_AHToggle", "Auto Enable").SetValue(false));
                }
                _MainMenu.AddSubMenu(Harass);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Leblanc_KUse_Q", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Leblanc_KUse_W", "Use W").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Leblanc_KUse_E", "Use E").SetValue(true));
                    KillSteal.SubMenu("Use R").AddItem(new MenuItem("Leblanc_KUse_Q2", "Use Q").SetValue(true));
                    KillSteal.SubMenu("Use R").AddItem(new MenuItem("Leblanc_KUse_W2", "Use W").SetValue(true));
                    KillSteal.SubMenu("Use R").AddItem(new MenuItem("Leblanc_KUse_E2", "Use E").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.AddItem(new MenuItem("Leblanc_ERCC", "Use _E + _R CC").SetValue(new KeyBind('G', KeyBindType.Press)));
                    Misc.AddItem(new MenuItem("Leblanc_Flee", "Flee & W + R").SetValue(new KeyBind('T', KeyBindType.Press)));
                    Misc.AddItem(new MenuItem("Leblanc_Pet", "Passive will be locating between Me & Enemy").SetValue(true));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Leblanc_Draw_Q", "Draw Q").SetValue(false));
                    Draw.AddItem(new MenuItem("Leblanc_Draw_W", "Draw W").SetValue(false));
                    Draw.AddItem(new MenuItem("Leblanc_Draw_E", "Draw E").SetValue(false));
                    Draw.AddItem(new MenuItem("Leblanc_Draw_WR", "Draw W + R").SetValue(false));
                    Draw.AddItem(new MenuItem("Leblanc_Indicator", "Draw Damage Indicator").SetValue(true));
                    Draw.AddItem(new MenuItem("Leblanc_WTimer", "Indicate _W Timer").SetValue(true));
                    Draw.AddItem(new MenuItem("Leblanc_Draw_ComboMode", "Draw Combo Mode").SetValue(true));
                    Draw.AddItem(new MenuItem("Leblanc_REnable", "Show _R Status").SetValue(true));
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
                if (_MainMenu.Item("Leblanc_Draw_Q").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Leblanc_Draw_W").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Leblanc_Draw_E").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (_MainMenu.Item("Leblanc_Draw_WR").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, 1400, Color.White, 1);
                if (WTime > Environment.TickCount && _MainMenu.Item("Leblanc_WTimer").GetValue<bool>())
                    Drawing.DrawText(Drawing.WorldToScreen(Player.Position)[0] - 20, Drawing.WorldToScreen(Player.Position)[1] + 20, Color.YellowGreen, "W Time is : " + ((WTime - Environment.TickCount) / 10));
                if (RTime > Environment.TickCount && _MainMenu.Item("Leblanc_WTimer").GetValue<bool>())
                    Drawing.DrawText(Drawing.WorldToScreen(Player.Position)[0] - 20, Drawing.WorldToScreen(Player.Position)[1] + 30, Color.YellowGreen, "R Time is : " + ((RTime - Environment.TickCount) / 10));
                if (_MainMenu.Item("Leblanc_REnable").GetValue<bool>())
                {
                    string Status_R = string.Empty;
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
                        Status_R = "_Q";
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
                        Status_R = "_W";
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm")
                        Status_R = "_W (Return)";
                    if (Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
                        Status_R = "_E";
                    Drawing.DrawText(Player.HPBarPosition.X + 30, Player.HPBarPosition.Y - 40, Color.IndianRed, "R: " + Status_R);
                }
                if (_MainMenu.Item("Leblanc_Draw_ComboMode").GetValue<bool>())
                {
                    var drawtext = _MainMenu.Item("Leblanc_ComboMode").GetValue<KeyBind>().Active ? "TeamFight" : "Private";
                    Drawing.DrawText(Player.HPBarPosition.X + 40, Player.HPBarPosition.Y + 30, Color.White, drawtext);
                }
                if (TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical) != null)
                    Drawing.DrawCircle(TargetSelector.GetTarget(1400, TargetSelector.DamageType.Magical).Position, 150, Color.Green);
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
                        if (_R.LSIsReady() || _W.LSIsReady() || _E.LSIsReady())
                        {
                            damage += (_Q.GetDamage(enemy) * 2);
                        }
                        else
                        {
                            damage += _Q.GetDamage(enemy);
                        }
                    if (_W.LSIsReady())
                        damage += _W.GetDamage(enemy);
                    if (_E.LSIsReady())
                        damage += (_E.GetDamage(enemy));
                    if (_R.LSIsReady())
                        if (_Q.LSIsReady())
                        {
                            damage += (_Q.GetDamage(enemy) * 1.5f * 2f);
                        }
                        else
                        {
                            damage += _R.GetDamage(enemy);
                        }
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
                    if (_MainMenu.Item("Leblanc_Indicator").GetValue<bool>())
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
        public Leblanc()
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
        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Player.IsDead) return;
                if (_MainMenu.Item("Leblanc_Flee").GetValue<KeyBind>().Active)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);  // 커서방향 이동
                    if (_W.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
                        _W.Cast(Game.CursorPos, true);
                    if (_R.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
                        _R.Cast(Game.CursorPos, true);
                }

                if (_MainMenu.Item("Leblanc_ERCC").GetValue<KeyBind>().Active) // ERCC
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);  // 커서방향 이동
                    var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                    if (_E.LSIsReady() && ETarget != null && Environment.TickCount > ERTIME)
                    {
                        _E.CastIfHitchanceEquals(ETarget, HitChance.Low, true);
                    }
                    else if (ETarget != null && _R.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem" && Environment.TickCount > ERTIME)
                    {
                        _R.CastIfHitchanceEquals(ETarget, HitChance.Low, true);
                    }
                }

                // Pet
                if (_MainMenu.Item("Leblanc_Pet").GetValue<bool>() && Player.Pet != null && Player.Pet.IsValid && !Player.Pet.IsDead)
                {
                    var Enemy = ObjectManager.Get<AIHeroClient>().OrderBy(x => x.LSDistance(Player)).FirstOrDefault(x => x.IsEnemy && !x.IsDead);
                    Random Ran = new Random();
                    var PetLocate = Player.ServerPosition.LSExtend(Enemy.ServerPosition, Ran.Next(150, 300));
                    PetLocate.X = PetLocate.X + Ran.Next(0, 20);
                    PetLocate.Y = PetLocate.Y + Ran.Next(0, 20);
                    PetLocate.Z = PetLocate.Z + Ran.Next(0, 20);
                    if (PetTime < Environment.TickCount)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, PetLocate);
                        PetTime = TickCount(750);
                    }

                }

                if (_MainMenu.Item("Leblanc_KUse_Q").GetValue<bool>() && _Q.LSIsReady())
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                    if (QTarget == null) return;
                    if (QTarget.Health <= _Q.GetDamage(QTarget))
                    {
                        _Q.Cast(QTarget, true);
                        return;
                    }
                }
                if (_MainMenu.Item("Leblanc_KUse_W").GetValue<bool>() && _W.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide")
                {
                    var WTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);
                    if (WTarget == null) return;
                    if (WTarget.Health <= _W.GetDamage(WTarget))
                    {
                        _W.Cast(WTarget.ServerPosition, true);
                        return;
                    }
                }
                if (_MainMenu.Item("Leblanc_KUse_E").GetValue<bool>() && _E.LSIsReady())
                {
                    var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                    if (ETarget == null) return;
                    if (ETarget.Health <= _E.GetDamage(ETarget))
                    {
                        _E.CastIfHitchanceEquals(ETarget, HitChance.Low, true);
                        return;
                    }
                }
                if (_MainMenu.Item("Leblanc_KUse_Q2").GetValue<bool>() && _R.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm")
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                    if (QTarget == null) return;
                    if (QTarget.Health <= _Q.GetDamage(QTarget))
                    {
                        _R.Cast(QTarget, true);
                        return;
                    }
                }
                if (_MainMenu.Item("Leblanc_KUse_W2").GetValue<bool>() && _R.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
                {
                    var QTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);
                    if (QTarget == null) return;
                    if (QTarget.Health <= _W.GetDamage(QTarget))
                    {
                        _R.Cast(QTarget.ServerPosition, true);
                        return;
                    }
                }
                if (_MainMenu.Item("Leblanc_KUse_E2").GetValue<bool>() && _R.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancsoulshacklem")
                {
                    var QTarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                    if (QTarget == null) return;
                    if (QTarget.Health <= _E.GetDamage(QTarget))
                    {
                        _R.CastIfHitchanceEquals(QTarget, HitChance.Low, true);
                        return;
                    }
                }

                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active) // Combo
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                    var WTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);
                    var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                    var TargetSel = TargetSelector.GetSelectedTarget();
                    if (QTarget == null && WTarget == null && ETarget == null) return;

                    // Q
                    if (QTarget != null
                            && _MainMenu.Item("Leblanc_CUse_Q").GetValue<bool>() && _Q.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "leblancchaosorb"
                            )
                    {
                        ReturnTime = TickCount(1000);
                        _Q.Cast(QTarget, true);
                        return;
                    }

                    if (Player.Level > 5 && TargetSel == null && _MainMenu.Item("Leblanc_ComboMode").GetValue<KeyBind>().Active)   // teamcombo
                    {
                        // W
                        if (WTarget != null
                                && _MainMenu.Item("Leblanc_CUse_W").GetValue<bool>() && _W.LSIsReady()
                                && Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide"
                                )
                        {
                            _W.Cast(WTarget.ServerPosition, true);
                            return;
                        }

                        // W2
                        if (WTarget != null
                                && _MainMenu.Item("Leblanc_CUse_W2").GetValue<bool>() && _R.LSIsReady()
                                && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
                        {
                            _R.Cast(WTarget.ServerPosition, true);
                            return;
                        }
                    }

                    // Q2
                    if (QTarget != null
                            && _MainMenu.Item("Leblanc_CUse_Q2").GetValue<bool>() && _R.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancchaosorbm"
                            )
                    {
                        _R.Cast(QTarget, true);
                        return;
                    }
                    // W
                    if (WTarget != null
                            && _MainMenu.Item("Leblanc_CUse_W").GetValue<bool>() && _W.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide"
                            )
                    {
                        _W.Cast(WTarget.ServerPosition, true);
                        return;
                    }

                    // W2
                    if (WTarget != null
                            && _MainMenu.Item("Leblanc_CUse_W2").GetValue<bool>() && _R.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidem")
                    {
                        _R.Cast(WTarget.ServerPosition, true);
                        return;
                    }

                    // E
                    if (ETarget != null
                            && _MainMenu.Item("Leblanc_CUse_E").GetValue<bool>() && _E.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.E).Name.ToLower() == "leblancsoulshackle" && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() != "leblancslidem"
                            )
                    {
                        _E.CastIfHitchanceEquals(ETarget, Hitchance("Leblanc_CUseE_Hit"), true);
                        return;
                    }

                    // E2
                    if (ETarget != null
                            && _MainMenu.Item("CUse_E2").GetValue<bool>() && _E.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.E).Name.ToLower() == "leblancsoulshacklem"
                            )
                    {
                        _R.CastIfHitchanceEquals(ETarget, Hitchance("Leblanc_CUseE_Hit"), true);
                        return;
                    }

                    // WReturn
                    if (ETarget.Buffs.Find(buff => (buff.Name.ToLower() == "leblancsoulshackle" || buff.Name.ToLower() == "leblancsoulshacklem") && buff.LSIsValidBuff()) == null
                        && _MainMenu.Item("Leblanc_CUse_WReturn").GetValue<bool>()
                        && _W.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn" && !_Q.LSIsReady() && !_R.LSIsReady() && ReturnTime < Environment.TickCount)
                    {
                        _W.Cast(true);
                        return;
                    }

                    // WR Return
                    if (ETarget.Buffs.Find(buff => (buff.Name.ToLower() == "leblancsoulshackle" || buff.Name.ToLower() == "leblancsoulshacklem") && buff.LSIsValidBuff()) == null
                        && _MainMenu.Item("Leblanc_CUse_W2Return").GetValue<bool>()
                        && _W.LSIsReady() && Player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "leblancslidereturnm" && !_Q.LSIsReady() && !_W.LSIsReady() && ReturnTime < Environment.TickCount)
                    {
                        _R.Cast(true);
                        return;
                    }

                }
                if ((_MainMenu.Item("HKey").GetValue<KeyBind>().Active || _MainMenu.Item("Leblanc_AHToggle").GetValue<bool>())
                    && _MainMenu.Item("Leblanc_AManarate").GetValue<Slider>().Value < Player.ManaPercent) // Harass
                {
                    var QTarget = TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical);
                    var WTarget = TargetSelector.GetTarget(_W.Range, TargetSelector.DamageType.Magical);
                    var ETarget = TargetSelector.GetTarget(_E.Range, TargetSelector.DamageType.Magical);
                    // Q
                    if (QTarget != null
                            && _MainMenu.Item("Leblanc_AUse_Q").GetValue<bool>() && _Q.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "leblancchaosorb"
                            )
                    {
                        _Q.Cast(QTarget, true);
                        return;
                    }

                    // W
                    if (WTarget != null
                            && _MainMenu.Item("Leblanc_AUse_W").GetValue<bool>() && _W.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslide"
                            )
                    {
                        _W.Cast(WTarget.ServerPosition, true);
                        return;
                    }

                    // E
                    if (ETarget != null
                            && _MainMenu.Item("Leblanc_AUse_E").GetValue<bool>() && _E.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.E).Name.ToLower() == "leblancsoulshackle"
                            )
                    {
                        _E.CastIfHitchanceEquals(ETarget, Hitchance("Leblanc_CUseE_Hit"), true);
                        return;
                    }

                    // WW
                    if (WTarget != null
                            && _MainMenu.Item("Leblanc_AUse_W").GetValue<bool>() && _W.LSIsReady()
                            && Player.Spellbook.GetSpell(SpellSlot.W).Name.ToLower() == "leblancslidereturn"
                            )
                    {
                        _W.Cast(true);
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
                    if (args.SData.Name.ToLower() == "leblancslide")
                        WTime = TickCount(4000);
                    if (args.SData.Name.ToLower() == "leblancslideM")
                        RTime = TickCount(4000);
                    if (_MainMenu.Item("Leblanc_ERCC").GetValue<KeyBind>().Active)
                    {
                        if (ERCC == 0 && args.SData.Name.ToLower() == "leblancsoulshackle")
                        {
                            ERTIME = TickCount(2000);
                        }
                        if (ERCC == 1 && args.SData.Name.ToLower() == "leblancsoulshacklem")
                        {
                            ERTIME = TickCount(2000);
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
    }
}
