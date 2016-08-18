using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using static FreshBooster.FreshCommon;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FreshBooster.Champion
{
    class Ryze
    {
        // Default Setting
        public static int ErrorTime;
        public const string ChampName = "Ryze";
        public static AIHeroClient Player;
        public static Spell _Q, _W, _E, _R;
        public static HpBarIndicator Indicator = new HpBarIndicator(); 
        // Default Setting

        public static BuffInstance RyzePassive, Ryzepassivecharged;
        public static bool Buff_Recall;
        public static int RyzeStack = 0;
        public static float Recall_Time = 0, PassiveEndTime = 0, SpeedTime = 0;

        private void SkillSet()
        {
            try
            {
                _Q = new Spell(SpellSlot.Q, 1000);
                _Q.SetSkillshot(0.25f, 50f, 1700, true, SkillshotType.SkillshotLine);
                _W = new Spell(SpellSlot.W, 620);
                _E = new Spell(SpellSlot.E, 620);
                _R = new Spell(SpellSlot.R, 1550);
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
                    Combo.AddItem(new MenuItem("Ryze_CUse_Q", "Use Q").SetValue(true));
                    Combo.AddItem(new MenuItem("Ryze_CUse_W", "Use W").SetValue(true));
                    Combo.AddItem(new MenuItem("Ryze_CUse_E", "Use E").SetValue(true));
                    //Combo.AddItem(new MenuItem("Ryze_CUse_R", "Use R").SetValue(true));
                    Combo.AddItem(new MenuItem("CKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Combo);

                var Harass = new Menu("Harass", "Harass");
                {
                    Harass.AddItem(new MenuItem("Ryze_HUse_Q", "Use Q").SetValue(true));
                    Harass.AddItem(new MenuItem("Ryze_HUse_W", "Use W").SetValue(true));
                    Harass.AddItem(new MenuItem("Ryze_HUse_E", "Use E").SetValue(true));
                    Harass.AddItem(new MenuItem("Ryze_HManarate", "Mana %").SetValue(new Slider(50)));
                    Harass.AddItem(new MenuItem("Ryze_HToggle", "Auto Enable").SetValue(false));
                    Harass.AddItem(new MenuItem("HKey", "Harass Key").SetValue(new KeyBind('C', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(Harass);

                var LaneClear = new Menu("LaneClear", "LaneClear");
                {
                    LaneClear.AddItem(new MenuItem("Ryze_LUseQ", "Use Q").SetValue(true));
                    LaneClear.AddItem(new MenuItem("Ryze_LUseW", "Use W").SetValue(true));
                    LaneClear.AddItem(new MenuItem("Ryze_LUseE", "Use E").SetValue(true));
                    LaneClear.AddItem(new MenuItem("Ryze_LManaRate", "Mana %").SetValue(new Slider(20)));
                    LaneClear.AddItem(new MenuItem("LKey", "LaneClear Key").SetValue(new KeyBind('V', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(LaneClear);

                var JungleClear = new Menu("JungleClear", "JungleClear");
                {
                    JungleClear.AddItem(new MenuItem("Ryze_JUseQ", "Use Q").SetValue(true));
                    JungleClear.AddItem(new MenuItem("Ryze_JUseW", "Use W").SetValue(true));
                    JungleClear.AddItem(new MenuItem("Ryze_JUseE", "Use E").SetValue(true));
                    JungleClear.AddItem(new MenuItem("Ryze_JManaRate", "Mana %").SetValue(new Slider(20)));
                    JungleClear.AddItem(new MenuItem("JKey", "JungleClear Key").SetValue(new KeyBind('V', KeyBindType.Press)));
                }
                _MainMenu.AddSubMenu(JungleClear);

                var KillSteal = new Menu("KillSteal", "KillSteal");
                {
                    KillSteal.AddItem(new MenuItem("Ryze_KUse_Q", "Use Q").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Ryze_KUse_W", "Use W").SetValue(true));
                    KillSteal.AddItem(new MenuItem("Ryze_KUse_E", "Use E").SetValue(true));
                }
                _MainMenu.AddSubMenu(KillSteal);

                var Misc = new Menu("Misc", "Misc");
                {
                    Misc.SubMenu("Auto Stack").AddItem(new MenuItem("Ryze_AutoStack", "Auto Stack").SetValue(new Slider(0, 0, 4)));
                    Misc.SubMenu("Auto Stack").AddItem(new MenuItem("Ryze_AutoKey", "Toggle Key").SetValue(new KeyBind('H', KeyBindType.Toggle)));
                    Misc.SubMenu("Auto Lasthit").AddItem(new MenuItem("Ryze_AutoLasthitQ", "Use Q").SetValue(false));
                    Misc.SubMenu("Auto Lasthit").AddItem(new MenuItem("Ryze_AutoLasthitW", "Use W").SetValue(false));
                    Misc.SubMenu("Auto Lasthit").AddItem(new MenuItem("Ryze_AutoLasthitE", "Use E").SetValue(false));
                    Misc.SubMenu("Auto Lasthit").AddItem(new MenuItem("Ryze_AutoLasthit", "Enable").SetValue(false));
                    Misc.AddItem(new MenuItem("Ryze_WGap", "Auto W, When Anti-GapCloser").SetValue(true));
                    Misc.AddItem(new MenuItem("Ryze_Speed", "Skill Speed mode").SetValue(new StringList(new[] { "Crazy", "Fast", "Nomal", "Random" })));
                }
                _MainMenu.AddSubMenu(Misc);

                var Draw = new Menu("Draw", "Draw");
                {
                    Draw.AddItem(new MenuItem("Ryze_QRange", "Q Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Ryze_WRange", "W Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Ryze_ERange", "E Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Ryze_RRange", "R Range").SetValue(false));
                    Draw.AddItem(new MenuItem("Ryze_R1Range", "R Range in minimap").SetValue(false));
                    Draw.AddItem(new MenuItem("Ryze_DisplayStack", "Display Stack").SetValue(true));
                    Draw.AddItem(new MenuItem("Ryze_DisplayTime", "Display Time").SetValue(true));
                    Draw.AddItem(new MenuItem("Ryze_AutoStackText", "Auto Stack Enable").SetValue(true));
                    Draw.AddItem(new MenuItem("Ryze_indicator", "Draw Damage Indicator").SetValue(true));
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
                if (_MainMenu.Item("Ryze_QRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _Q.Range, Color.White, 1);
                if (_MainMenu.Item("Ryze_WRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _W.Range, Color.White, 1);
                if (_MainMenu.Item("Ryze_ERange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _E.Range, Color.White, 1);
                if (_MainMenu.Item("Ryze_RRange").GetValue<bool>())
                    Render.Circle.DrawCircle(Player.Position, _R.Range, Color.White, 1);
                if (_MainMenu.Item("Ryze_R1Range").GetValue<bool>())
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, _R.Range, Color.White, 1, 23, true);

                var PlayerPosition = ObjectManager.Player.Position;
                var PlayerPosition2 = Drawing.WorldToScreen(PlayerPosition);
                if (_MainMenu.Item("Ryze_DisplayStack").GetValue<bool>() && RyzePassive != null)
                    Drawing.DrawText(PlayerPosition2.X - 20, PlayerPosition2.Y - 120, Color.Gold, "Stack: " + RyzeStack);
                if (_MainMenu.Item("Ryze_DisplayTime").GetValue<bool>() && RyzePassive != null)
                {
                    if (RyzePassive.EndTime - Game.Time > 2f)
                        Drawing.DrawText(PlayerPosition2.X - 20, PlayerPosition2.Y + 30, Color.Gold, "Stack Time: " + "{0:#.##}", (int)(RyzePassive.EndTime - Game.Time));
                    else
                        Drawing.DrawText(PlayerPosition2.X - 20, PlayerPosition2.Y + 30, Color.Red, "Stack Time: " + "{0:#.##}", (int)(RyzePassive.EndTime - Game.Time));
                }
                if (_MainMenu.Item("Ryze_AutoStackText").GetValue<bool>())
                    Drawing.DrawText(PlayerPosition2.X - 20, PlayerPosition2.Y + 45, Color.Gold, "Auto Stack: " + (_MainMenu.Item("Ryze_AutoKey").GetValue<KeyBind>().Active ? "On" : "Off"));
                if (TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical) != null)
                    Drawing.DrawCircle(TargetSelector.GetTarget(_Q.Range, TargetSelector.DamageType.Magical).Position, 150, Color.Green);
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
                    if (_E.IsReady())
                        damage += (_E.GetDamage(enemy));
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
                    if (_MainMenu.Item("Ryze_indicator").GetValue<bool>())
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
        public Ryze()
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
                
                RyzePassive = ObjectManager.Player.Buffs.Find(DrawFX => DrawFX.Name.ToLower() == "ryzepassivestack" && DrawFX.IsValidBuff()); // 라이즈 스택
                Ryzepassivecharged = ObjectManager.Player.Buffs.Find(DrawFX => DrawFX.Name.ToLower() == "ryzepassivecharged" && DrawFX.IsValidBuff());
                Buff_Recall = ObjectManager.Player.IsRecalling();
                if (Buff_Recall)
                    Recall_Time = TickCount(1000);
                if (RyzePassive != null)
                {
                    RyzeStack = RyzePassive.Count;
                    PassiveEndTime = RyzePassive.EndTime;
                }
                else
                    RyzeStack = 0;

                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                    _Q.Collision = false;
                else
                    _Q.Collision = true;

                if (_MainMenu.Item("Ryze_KUse_Q").GetValue<bool>() && QTarget != null && _Q.IsReady() && _Q.GetDamage(QTarget) > QTarget.Health)
                    _Q.CastIfHitchanceEquals(QTarget, HitChance.High);
                if (_MainMenu.Item("Ryze_KUse_E").GetValue<bool>() && ETarget != null && _E.IsReady() && _E.GetDamage(ETarget) > ETarget.Health)
                    _E.CastOnUnit(ETarget, true);
                if (_MainMenu.Item("Ryze_KUse_W").GetValue<bool>() && WTarget != null && _W.IsReady() && _W.GetDamage(WTarget) > WTarget.Health)
                    _W.CastOnUnit(WTarget, true);

                if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active) // Combo
                {
                    if (SpeedTime > Environment.TickCount)
                        return;
                    //if (_MainMenu.Item("Ryze_CUse_R").GetValue<bool>() && _R.IsReady() && WTarget != null
                    //    && ((_Q.IsReady() || _W.IsReady() || _E.IsReady()) && RyzeStack > 2)
                    //    || RyzeStack == 4 || Ryzepassivecharged != null)
                    //    _R.Cast(true);
                    if (_MainMenu.Item("Ryze_CUse_E").GetValue<bool>() && _E.IsReady() && ETarget != null)
                        _E.CastOnUnit(ETarget, true);
                    if (_MainMenu.Item("Ryze_CUse_W").GetValue<bool>() && _W.IsReady() && WTarget != null)
                        _W.CastOnUnit(WTarget, true);
                    if (_MainMenu.Item("Ryze_CUse_Q").GetValue<bool>() && _Q.IsReady() && WTarget != null && _W.IsReady())
                        _Q.Cast(WTarget, true);
                    if (_MainMenu.Item("Ryze_CUse_Q").GetValue<bool>() && _Q.IsReady() && QTarget != null && !_W.IsReady())
                        _Q.Cast(QTarget, true);                    
                }

                if ((_MainMenu.Item("HKey").GetValue<KeyBind>().Active || _MainMenu.Item("Ryze_HToggle").GetValue<bool>())
                    && _MainMenu.Item("Ryze_HManarate").GetValue<Slider>().Value < Player.ManaPercent) // Harass
                {
                    if (_MainMenu.Item("Ryze_HToggle").GetValue<bool>() && Recall_Time > Environment.TickCount)
                        return;
                    if (_MainMenu.Item("Ryze_HUse_W").GetValue<bool>() && _W.IsReady() && WTarget != null)
                        _W.CastOnUnit(WTarget, true);
                    if (_MainMenu.Item("Ryze_HUse_Q").GetValue<bool>() && _Q.IsReady() && QTarget != null)
                        _Q.CastIfHitchanceEquals(QTarget, HitChance.VeryHigh, true);
                    if (_MainMenu.Item("Ryze_HUse_E").GetValue<bool>() && _E.IsReady() && ETarget != null)
                        _E.CastOnUnit(ETarget, true);
                }

                if (_MainMenu.Item("LKey").GetValue<KeyBind>().Active && _MainMenu.Item("Ryze_LManaRate").GetValue<Slider>().Value < Player.ManaPercent) // LaneClear
                {
                    var MinionTarget = MinionManager.GetMinions(_W.Range, MinionTypes.All, MinionTeam.Enemy).OrderBy(x => x.ServerPosition.Distance(Player.ServerPosition));
                    foreach (var item in MinionTarget)
                    {
                        if (_MainMenu.Item("Ryze_LUseW").GetValue<bool>() && _W.IsReady())
                            _W.CastOnUnit(item, true);
                        if (_MainMenu.Item("Ryze_LUseQ").GetValue<bool>() && _Q.IsReady())
                            _Q.CastIfHitchanceEquals(item, HitChance.High, true);
                        if (_MainMenu.Item("Ryze_LUseE").GetValue<bool>() && _E.IsReady())
                            _E.CastOnUnit(item, true);
                    }
                }

                if (_MainMenu.Item("JKey").GetValue<KeyBind>().Active && _MainMenu.Item("Ryze_JManaRate").GetValue<Slider>().Value < Player.ManaPercent) // JungleClear
                {
                    var MinionTarget = MinionManager.GetMinions(_W.Range, MinionTypes.All, MinionTeam.Neutral).OrderBy(x => x.ServerPosition.Distance(Player.ServerPosition));
                    foreach (var item in MinionTarget)
                    {
                        if (_MainMenu.Item("Ryze_JUseW").GetValue<bool>() && _W.IsReady())
                            _W.CastOnUnit(item, true);
                        if (_MainMenu.Item("Ryze_JUseQ").GetValue<bool>() && _Q.IsReady())
                            _Q.CastIfHitchanceEquals(item, HitChance.High, true);
                        if (_MainMenu.Item("Ryze_JUseE").GetValue<bool>() && _E.IsReady())
                            _E.CastOnUnit(item, true);
                    }
                }

                if (_MainMenu.Item("Ryze_AutoLasthit").GetValue<bool>() && !_MainMenu.Item("CKey").GetValue<KeyBind>().Active && !_MainMenu.Item("HKey").GetValue<KeyBind>().Active
                    && Recall_Time < Environment.TickCount && RyzeStack < 4)
                {
                    var MinionTarget = MinionManager.GetMinions(_W.Range, MinionTypes.All, MinionTeam.Enemy).OrderBy(x => x.Health).FirstOrDefault(x => !x.IsDead);
                    if (_MainMenu.Item("Ryze_AutoLasthitQ").GetValue<bool>() && _Q.IsReady() && MinionTarget != null && _Q.GetDamage(MinionTarget) > MinionTarget.Health)
                        _Q.CastIfHitchanceEquals(MinionTarget, HitChance.High, true);
                    if (_MainMenu.Item("Ryze_AutoLasthitE").GetValue<bool>() && _E.IsReady() && MinionTarget != null && _E.GetDamage(MinionTarget) > MinionTarget.Health)
                        _E.CastOnUnit(MinionTarget, true);
                    if (_MainMenu.Item("Ryze_AutoLasthitW").GetValue<bool>() && _W.IsReady() && MinionTarget != null && _W.GetDamage(MinionTarget) > MinionTarget.Health)
                        _W.CastOnUnit(MinionTarget, true);
                }

                if (_MainMenu.Item("Ryze_AutoKey").GetValue<KeyBind>().Active
                    && !_MainMenu.Item("CKey").GetValue<KeyBind>().Active && !_MainMenu.Item("HKey").GetValue<KeyBind>().Active
                    && Recall_Time < Environment.TickCount && _MainMenu.Item("Ryze_AutoStack").GetValue<Slider>().Value > RyzeStack
                    && _MainMenu.Item("Ryze_AutoStack").GetValue<Slider>().Value != RyzeStack
                    && _MainMenu.Item("Ryze_AutoStack").GetValue<Slider>().Value != 0
                    && _Q.IsReady() && PassiveEndTime <= Game.Time + 0.5f)
                    _Q.Cast(Game.CursorPos, true);
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
                if (_MainMenu.Item("Ryze_WGap").GetValue<bool>() && _W.IsReady() && gapcloser.Sender.IsEnemy && gapcloser.Sender.ServerPosition.Distance(Player.ServerPosition) < _W.Range)
                    _W.CastOnUnit(gapcloser.Sender, true);
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
                    switch (_MainMenu.Item("Ryze_Speed").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            break;
                        case 1:
                            SpeedTime = TickCount(200);
                            break;
                        case 2:
                            SpeedTime = TickCount(400);
                            break;
                        case 3:
                            var ran = new Random().Next(0, 2);
                            if (ran == 1)
                                SpeedTime = TickCount(150);
                            else if (ran == 2)
                                SpeedTime = TickCount(400);
                            break;
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
                if (args.Unit.IsMe)
                {
                    if (_MainMenu.Item("CKey").GetValue<KeyBind>().Active)
                    {
                        if (_Q.IsReady() || _W.IsReady() || _E.IsReady())
                            args.Process = false;
                        else
                            args.Process = true;
                    }
                }
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
