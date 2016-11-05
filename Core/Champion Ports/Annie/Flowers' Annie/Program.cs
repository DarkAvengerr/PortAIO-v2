using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers__Annie
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using System.Linq;

    class Program
    {
        internal static AIHeroClient Me;
        internal static Menu Menu;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static Spell Q, W, E, R;
        internal static SpellSlot Flash, Ignite;
        internal static float ClickTime;
        internal static HpBarDraw DrawHpBar = new HpBarDraw();

        public static void Game_OnGameLoad()
        {
            // Judge ChampionName , if not Annie return , not injected.
            if (ObjectManager.Player.ChampionName != "Annie")
                return;

            // this is hero!
            Me = ObjectManager.Player;

            // set Annie Spells
            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 580f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 625f);
            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.50f, 200f, float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.20f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            // set R HitChance
            R.MinHitChance = HitChance.High;

            // set SummonerSpells
            Ignite = Me.GetSpellSlot("SummonerDot");
            Flash = Me.GetSpellSlot("SummonerFlash");

            // Make a Menu
            Menu = new Menu("Flowers' Annie", "NightMoon", true);

            // Make a Orbwalking Menu
            Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            Menu.AddSubMenu(new Menu("Q Setting", "QSetting"));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("ComboQQQQQ", "       Combo Setting", true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("ComboQ", "--- Always Use !!!!!", true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("HarassQQQQQ", "       Harass Setting", true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("HarassQ", "Use Q To Harass", true).SetValue(true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("HarassQOnlyStun", "--- Only Have Stun(Use Q)", true).SetValue(true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("HarassAutoLastHitQ", "--- Auto Q To LastHit", true).SetValue(true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("ClearQQQQQ", "       Clear Setting", true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("LaneClearQ", "Use Q To LaneCLear", true).SetValue(true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("LaneClearQLastHit", "--- Only Use Q in LastHit", true).SetValue(true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("JungleClearQ", "Use Q To JungleClear", true).SetValue(true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("LastHitQQQQQ", "       LastHit Setting", true));
            Menu.SubMenu("QSetting").AddItem(new MenuItem("LastHitQ", "Use Q To LastHit", true).SetValue(true));

            Menu.AddSubMenu(new Menu("W Setting", "WSetting"));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("ComboWWWWWW", "       Combo Setting", true));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("ComboW", "--- Always Use !!!!!", true));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("HarassWWWWW", "       Harass Setting", true));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("HarassW", "Use W To Harass", true).SetValue(true));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("HarassWDebuff", "--- Only Target Has Debuff", true).SetValue(true));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("ClearWWWWW", "       Clear Setting", true));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("LaneClearW", "Use W To LaneCLear", true).SetValue(true));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("LaneClearWCount", "--- Min LaneClear minion Counts >= ", true).SetValue(new Slider(3, 1, 5)));
            Menu.SubMenu("WSetting").AddItem(new MenuItem("JungleClearW", "Use W To JungleClear", true).SetValue(true));

            Menu.AddSubMenu(new Menu("E Setting", "ESetting"));
            Menu.SubMenu("ESetting").AddItem(new MenuItem("ComboEEEEE", "       Combo Setting", true));
            Menu.SubMenu("ESetting").AddItem(new MenuItem("SmartEInCombo", "Smart E in Combo Mode", true).SetValue(true));
            Menu.SubMenu("ESetting").AddItem(new MenuItem("ClearEEEEE", "       Clear Setting", true));
            Menu.SubMenu("ESetting").AddItem(new MenuItem("JungleClearE", "If Jungle Attack Me", true).SetValue(true));

            Menu.AddSubMenu(new Menu("R Setting", "RSetting"));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("ComboRRRRR", "       Combo Setting", true));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("ComboRCountsEnemiesinRange", "--- Min Hit Enemies Counts >= ", true).SetValue(new Slider(2, 1, 5)));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("ComboRCanKill", "--- Or Can Kill Enemy", true).SetValue(true));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("FlashRRRRR", "       FlashR Setting", true));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("EnableFlashR", "FlashR Key!", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("FlashRCountsEnemiesinRange", "--- Min Hit Enemies Counts >= ", true).SetValue(new Slider(3, 1, 5)));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("FlashRCountsAlliesinRange", "--- And Min Allies Counts >= (0 = off)", true).SetValue(new Slider(2, 0, 5)));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("FlashRCanKillEnemy", "--- Or Can Kill (Only In 1v1) (TEST)", true).SetValue(true));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("BearRRRRR", "       Bear Setting", true));
            Menu.SubMenu("RSetting").AddItem(new MenuItem("BearAutoFollow", "--- Auto Follow Enemy Or MySelf", true).SetValue(true));

            Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("KillStealEnable", "Enable", true).SetValue(true));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(false));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("KillStealIgnite", "Use Ignite", true).SetValue(true));

            Menu.AddSubMenu(new Menu("Mana Control", "ManaControl"));
            Menu.SubMenu("ManaControl").AddItem(new MenuItem("HarassMana", "Min Harass Mana >= %", true).SetValue(new Slider(60)));
            Menu.SubMenu("ManaControl").AddItem(new MenuItem("LaneClearMana", "Min LaneClear Mana >= %", true).SetValue(new Slider(40)));
            Menu.SubMenu("ManaControl").AddItem(new MenuItem("JungleClearMana", "Min JungleClear Mana >= %", true).SetValue(new Slider(30)));
            Menu.SubMenu("ManaControl").AddItem(new MenuItem("LastHitMana", "Min LastHit Mana >= %", true).SetValue(new Slider(20)));
            Menu.SubMenu("ManaControl").AddItem(new MenuItem("AutoStackMana", "Min Auto Stack Passive Mana >= %", true).SetValue(new Slider(50)));

            Menu.AddSubMenu(new Menu("Stack Passive", "StackPassive"));
            Menu.SubMenu("StackPassive").AddItem(new MenuItem("AutoStackEnable", "Enable", true).SetValue(true));
            Menu.SubMenu("StackPassive").AddItem(new MenuItem("AutoStackW", "--- Use W", true).SetValue(true));
            Menu.SubMenu("StackPassive").AddItem(new MenuItem("AutoStackE", "--- Use E", true).SetValue(true));

            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("IntAntt", "         GapCloser & Interrupt", true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("IntAnttEnable", "Enable", true).SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("IntAnttQ", "--- Use Q", true).SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("IntAnttW", "--- Use W", true).SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("IntAnttE", "--- Use E", true).SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("StunSet", "         Stun Setting", true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("ClearStun", "--- In ClearMode, Have Passive Disable Use Spells", true).SetValue(true));
            // Menu.SubMenu("Misc").AddItem(new MenuItem("LastHitStun", "--- In LastHitMode, Have Passive Disable Use Spells", true).SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("SupportMode", "         Support Mode", true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("SupportEnable", "Enable!", true).SetValue(false));

            Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawRF", "Draw R + Flash Range", true).SetValue(false));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawDamage", "Draw Damage In HpBar", true).SetValue(false));

            // Finish
            Menu.AddToMainMenu();

            // Chat
            Chat.Print("<font color='#2848c9'>Flowers Annie</font> --> <font color='#b756c5'>Version : 1.0.0.3</font> <font size='30'><font color='#d949d4'>Good Luck!</font></font>");

            // Events
            Orbwalking.BeforeAttack += BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (HarassMode && Menu.GetBool("SupportEnable") && (args.Target.Type == GameObjectType.obj_AI_Minion))
            {
                args.Process = false;
            }
        }

        /// <summary>
        /// use qwe to gapcloser
        /// </summary>
        /// <param name="gapcloser"></param>
        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var sender = gapcloser.Sender;

            if (sender.IsEnemy && Menu.GetBool("IntAnttEnable"))
            {
                if (!HaveStun && BuffCounts == 3 && E.IsReady() && Menu.GetBool("IntAnttE"))
                {
                    E.Cast();
                }

                if (HaveStun)
                {
                    if (Q.IsReady() && Menu.GetBool("IntAnttQ") && sender.IsValidTarget(300))
                    {
                        Q.Cast(sender, true);
                    }
                    else if (W.IsReady() && Menu.GetBool("IntAnttW") && sender.IsValidTarget(250))
                    {
                        W.Cast(sender, true);
                    }
                }
            }
        }

        /// <summary>
        ///  use QWE to interrupt spells
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= Interrupter2.DangerLevel.Medium && Menu.GetBool("IntAnttEnable"))
            {
                if (!HaveStun && BuffCounts == 3 && E.IsReady() && Menu.GetBool("IntAnttE"))
                {
                    E.Cast();
                }

                if (HaveStun)
                {
                    if (Q.IsReady() && Menu.GetBool("IntAnttQ") && sender.IsValidTarget(Q.Range))
                    {
                        Q.Cast(sender, true);
                    }
                    else if (W.IsReady() && Menu.GetBool("IntAnttW") && sender.IsValidTarget(W.Range))
                    {
                        W.Cast(sender, true);
                    }
                }
            }
        }

        /// <summary>
        ///  use e in combo & clear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !args.SData.IsAutoAttack() || !args.Target.IsMe)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Menu.GetBool("SmartEInCombo") && E.IsReady())
            {
                if (sender.IsEnemy && sender is AIHeroClient)
                {
                    E.Cast();
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Menu.GetBool("JungleClearE") && E.IsReady())
            {
                if (sender.IsMinion && sender is Obj_AI_Minion && Me.HealthPercent < 70)
                {
                    E.Cast();
                }
            }
        }

        /// <summary>
        /// you can said this is event loop ....zzz
        /// </summary>
        /// <param name="args"></param>
        private static void OnUpdate(EventArgs args)
        {
            if (Menu.GetBool("BearAutoFollow"))
            {
                AutoBearLogic();
            }

            // if hero dead or reacll not to inject others void
            if (Me.IsDead || Me.IsRecalling())
                return;

            switch (Orbwalker.ActiveMode) // judge you press what key in orbwalker
            {
                case Orbwalking.OrbwalkingMode.Combo:// if you press combo key , load combo logic
                    ComboLogic(); 
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:// if you press harass key , load harass logic
                    HarassLogic(); 
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear: // if you press clear key , load clear logic
                    LaneClearLogic();
                    JungleClearLogic();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit: // if you press lasthit key , load lasthit logic
                    LastHitLogic();
                    break;
            }

            if (Menu.GetKey("EnableFlashR"))
            {
                FlashRLogic();
            }

            if (Menu.GetBool("KillStealEnable"))
            {
                KillStealLogic();
            }

            if (Menu.GetBool("AutoStackEnable"))
            {
                AutoStackPassiveLogic();
            }
        }

        /// <summary>
        /// inject combo logic
        /// </summary>
        private static void ComboLogic()
        {
            var e = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (e != null)
            {
                if (R.IsReady() && !HaveBear && e.IsValidTarget(R.Range))
                {
                    var RPred = R.GetPrediction(e, true);

                    if (RPred.AoeTargetsHitCount >= Menu.GetSlider("ComboRCountsEnemiesinRange") && HaveStun)
                    {
                        R.Cast(RPred.CastPosition, true);
                    }

                    if (Menu.GetBool("ComboRCanKill") && CanR(e))
                    {
                        if (BuffCounts == 3 && E.IsReady())
                        {
                            E.Cast();
                        }

                        R.Cast(e, true, true);
                    }
                }

                if (Q.IsReady() && e.IsValidTarget(Q.Range) && (R.IsReady() && HaveBear) || !R.IsReady())
                {
                    Q.Cast(e, true);
                }

                if (W.IsReady() && e.IsValidTarget(W.Range) && (R.IsReady() && HaveBear) || !R.IsReady())
                {
                    W.Cast(e, true, true);
                }

                if (Ignite.IsReady() && e.IsValidTarget(600) && e.Health < GetDamage(e, false, false, false, true))
                {
                    Me.Spellbook.CastSpell(Ignite, e);
                }
            }
        }

        private static bool CanR(AIHeroClient e)
        {
            if (HaveBear)
            {
                return false;
            }

            if (Q.IsReady() && W.IsReady() && (e.Health < GetDamage(e, true, true, true, true)) && (Me.Mana > Q.ManaCost + W.ManaCost + R.ManaCost) && e.IsValidTarget(W.Range) && Ignite.IsReady() && BuffCounts >= 3)
            {
                return true;
            }

            if (Q.IsReady() && W.IsReady() && (e.Health < GetDamage(e, true, true, true)) && (Me.Mana > Q.ManaCost + W.ManaCost + R.ManaCost) && e.IsValidTarget(W.Range) && BuffCounts >= 3)
            {
                return true;
            }

            if (Q.IsReady() && (e.Health < GetDamage(e, true, false, true)) && (Me.Mana > Q.ManaCost + R.ManaCost) && e.IsValidTarget(Q.Range))
            {
                return true;
            }

            if (W.IsReady() && (e.Health < GetDamage(e, false, true, true)) && (Me.Mana > W.ManaCost + R.ManaCost) && e.IsValidTarget(W.Range))
            {
                return true;
            }

            if (!Q.IsReady() && !W.IsReady() && (e.Health < GetDamage(e, false, false, true)) && (Me.Mana > R.ManaCost) && e.IsValidTarget(R.Range))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  inject q harass or q lasthit or smart w logic
        /// </summary>
        private static void HarassLogic()
        {
            if (Menu.GetSlider("HarassMana") < Me.ManaPercent)
            {
                if (Menu.GetBool("HarassAutoLastHitQ"))
                {
                    LastHitLogic();
                }

                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    var e = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                    if (Menu.GetBool("HarassQOnlyStun") && e != null)
                    {
                        if (HaveStun && e.IsValidTarget(Q.Range))
                        {
                            Q.Cast(e, true);
                        }
                    }
                    else if (!Menu.GetBool("HarassQOnlyStun") && e != null)
                    {
                        if (e.IsValidTarget(Q.Range))
                        {
                            Q.Cast(e, true);
                        }
                    }
                }

                if (Menu.GetBool("HarassW") && W.IsReady())
                {
                    var e = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                    if (Menu.GetBool("HarassWDebuff"))
                    {
                        if (e.IsValidTarget(W.Range))
                        {
                            if (e.HasBuffOfType(BuffType.Charm) || e.HasBuffOfType(BuffType.Fear) || e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Slow) || e.HasBuffOfType(BuffType.Suppression))
                            {
                                W.Cast(e, true, true);
                            }
                        }
                    }
                    else if (!Menu.GetBool("HarassWDebuff"))
                    {
                        if (e.IsValidTarget(W.Range))
                        {
                            W.Cast(e, true, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  qw laneclear
        /// </summary>
        private static void LaneClearLogic()
        {
            if (Menu.GetSlider("LaneClearMana") < Me.ManaPercent && !(Menu.GetBool("ClearStun") && HaveStun))
            {
                var Minions = MinionManager.GetMinions(Q.Range);

                if (Minions.Count() > 0)
                {
                    foreach (var min in Minions)
                    {
                        if (Q.IsReady() && Menu.GetBool("LaneClearQ") && Menu.GetBool("LaneClearQLastHit") && min.IsValidTarget(Q.Range))
                        {
                            if (min.Health < Q.GetDamage(min) && min.Health > Me.GetAutoAttackDamage(min))
                            {
                                Q.Cast(min, true);
                            }
                            else if (Q.IsReady() && Menu.GetBool("LaneClearQ") && !Menu.GetBool("LaneClearQLastHit") && min.IsValidTarget(Q.Range))
                            {
                                Q.Cast(min, true);
                            }
                        }
                    }

                    if (W.IsReady() && Menu.GetBool("LaneClearW"))
                    {
                        var WFarmLocation = W.GetCircularFarmLocation(Minions, W.Width);

                        if (WFarmLocation.MinionsHit >= Menu.GetSlider("LaneClearWCount"))
                        {
                            W.Cast(WFarmLocation.Position, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  qwe jungle
        /// </summary>
        private static void JungleClearLogic()
        {
            if (Menu.GetSlider("JungleClearMana") < Me.ManaPercent)
            {
                var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (mobs.Count() > 0)
                {
                    foreach (var mob in mobs)
                    {
                        if (Q.IsReady() && mob.Health < Q.GetDamage(mob) && W.IsReady() && mob.IsValidTarget(Q.Range))
                        {
                            Q.Cast(mob, true);
                        }
                        else if (Q.IsReady() && Menu.GetBool("JungleClearQ") && mob.IsValidTarget(Q.Range))
                        {
                            Q.Cast(mob, true);
                        }
                        else if (W.IsReady() && Menu.GetBool("JungleClearW") && mob.IsValidTarget(W.Range))
                        {
                            W.Cast(mob, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// use q to last hit
        /// </summary>
        private static void LastHitLogic()
        {
            if ((Menu.GetBool("LastHitQ") && Menu.GetSlider("LastHitMana") < Me.ManaPercent) || (Menu.GetBool("HarassAutoLastHitQ") && Menu.GetSlider("HarassMana") < Me.ManaPercent))
            {
                if (Q.IsReady() && !HaveStun)
                {
                    var Minions = MinionManager.GetMinions(Q.Range);

                    foreach (var min in Minions.Where(m => !m.IsZombie && m.Health < Q.GetDamage(m)))
                    {
                        if (min != null)
                        {
                            if (min.Health > Me.GetAutoAttackDamage(min))
                            {
                                Q.Cast(min, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// auto r to follow enemies or me
        /// </summary>
        private static void AutoBearLogic()
        {
            if (HaveBear)
            {
                var e = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);

                if (e != null && e.IsValidTarget(2000) && !e.IsZombie)
                {
                    if (Game.Time > ClickTime + 1.5)
                    {
                        R.Cast(e);
                        ClickTime = Game.Time;
                    }
                }
                else if (e == null && Game.Time > ClickTime + 1.5)
                {
                    R.Cast(Me.ServerPosition);
                    ClickTime = Game.Time;
                }
            }
        }

        /// <summary>
        ///  inject flashr logic
        /// </summary>
        private static void FlashRLogic()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            foreach (var e in HeroManager.Enemies.Where(em => em.IsValidTarget(R.Range + 425) && !em.IsZombie))
            {
                if (e != null)
                {
                    Chat.Print(e.ChampionName);
                    if (BuffCounts == 3 && E.IsReady() && !HaveStun)
                    {
                        E.Cast();
                    }

                    var RPred = R.GetPrediction(e, true);
                    var RHitCount = R.GetPrediction(e, true).AoeTargetsHitCount;
                    if (RPred.Hitchance >= HitChance.High && HaveStun)
                    {
                        if (Me.CountAlliesInRange(1000) >= Menu.GetSlider("FlashRCountsAlliesinRange") && Me.CountEnemiesInRange(R.Range + 425) >= Menu.GetSlider("FlashRCountsEnemiesinRange") && RHitCount >= Menu.GetSlider("FlashRCountsEnemiesinRange"))
                        {
                            Me.Spellbook.CastSpell(Flash, R.GetPrediction(e, true).CastPosition);
                            if (!Flash.IsReady())
                            {
                                R.Cast(R.GetPrediction(e, true).CastPosition, true);
                            }
                        }
                        else if (Menu.GetBool("FlashRCanKillEnemy") && Me.CountEnemiesInRange(R.Range + 425) == 1 && Q.IsReady() && Ignite.IsReady() && e.Health < GetDamage(e, true, false, true, true))
                        {
                            Me.Spellbook.CastSpell(Flash, R.GetPrediction(e, true).CastPosition);
                            if (!Flash.IsReady())
                            {
                                R.Cast(R.GetPrediction(e, true).CastPosition, true); Me.Spellbook.CastSpell(Ignite, e); Q.Cast(e, true);
                            }
                        }
                        else if (Menu.GetBool("FlashRCanKillEnemy") && Me.CountEnemiesInRange(R.Range + 425) == 1 && Q.IsReady() && e.Health < GetDamage(e, true, false, true))
                        {
                            Me.Spellbook.CastSpell(Flash, R.GetPrediction(e, true).CastPosition);
                            if (!Flash.IsReady())
                            {
                                R.Cast(R.GetPrediction(e, true).CastPosition, true);
                                Q.Cast(e, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  use Q W R Ignite to killsteal
        /// </summary>
        private static void KillStealLogic()
        {
            foreach (var e in HeroManager.Enemies.Where(e => !e.IsZombie && !e.IsDead && e.IsValidTarget()))
            {
                if (e != null)
                {
                    if (Q.IsReady() && Menu.GetBool("KillStealQ") && e.Health + e.MagicShield < Q.GetDamage(e) && e.IsValidTarget(Q.Range))
                    {
                        Q.Cast(e, true);
                        return;
                    }

                    if (W.IsReady() && Menu.GetBool("KillStealW") && e.Health + e.MagicShield < W.GetDamage(e) && e.IsValidTarget(W.Range))
                    {
                        W.Cast(e, true);
                        return;
                    }

                    if (R.IsReady() && Menu.GetBool("KillStealR") && e.Health + e.MagicShield < R.GetDamage(e) && e.IsValidTarget(R.Range))
                    {
                        R.Cast(e, true);
                        return;
                    }

                    if (Ignite.IsReady() && Menu.GetBool("KillStealIgnite") && e.Health < Me.GetSummonerSpellDamage(e, Damage.SummonerSpell.Ignite) && e.IsValidTarget(600))// ignite range is 600f
                    {
                        Me.Spellbook.CastSpell(Ignite, e);
                        return;
                    }
                }
            }
        }

        /// <summary>
        ///  we to static passive
        /// </summary>
        private static void AutoStackPassiveLogic()
        {
            if (Menu.GetBool("AutoStackEnable") && NoneMode && !Menu.GetKey("EnableFlashR"))
            {
                if (Me.InFountain() && !HaveStun)
                {
                    if (E.IsReady() && Menu.GetBool("AutoStackE"))
                        E.Cast();
                    else if (W.IsReady() && Menu.GetBool("AutoStackW"))
                        W.Cast(Game.CursorPos);
                }
                else if (!HaveStun && Menu.GetSlider("AutoStackMana") < Me.ManaPercent && !Me.IsRecalling() && !Me.InFountain())
                {
                    if (E.IsReady() && Menu.GetBool("AutoStackE"))
                    {
                        E.Cast();
                    }
                    else if (W.IsReady() && Menu.GetBool("AutoStackW"))
                    {
                        var countenemies = Me.CountEnemiesInRange(1000);
                        var minions = MinionManager.GetMinions(1000);

                        if (countenemies == 0 && minions == null && W.IsReady())
                        {
                            W.Cast(Game.CursorPos);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Drawing Spell Range Events
        /// </summary>
        /// <param name="args"></param>
        private static void OnDraw(EventArgs args)
        {
            // if heros is dead, not draw circle
            if (Me.IsDead)
                return;

            // Draw Q Range
            if (Q.IsReady() && Menu.GetBool("DrawQ"))
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue);

            // Draw W Range
            if (W.IsReady() && Menu.GetBool("DrawW"))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.DarkMagenta);

            // Draw R Range
            if (R.IsReady() && Menu.GetBool("DrawR"))
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.Indigo);

            // Flash Range is 425!!! 
            if (R.IsReady() && Flash.IsReady() && Menu.GetBool("DrawRF"))
                Render.Circle.DrawCircle(Me.Position, R.Range + 425, System.Drawing.Color.Red);

            if (Menu.GetBool("DrawDamage"))
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg(GetDamage(e, true, true, true, true), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        /// <summary>
        /// if bear life , the hero has this buff
        /// </summary>
        private static bool HaveBear
        {
            get { return Me.HasBuff("InfernalGuardianTimer"); }
        }

        /// <summary>
        /// if hero have stun , the hero has this buff
        /// </summary>
        private static bool HaveStun
        {
            get { return Me.HasBuff("Energized"); }
        }

        /// <summary>
        /// get annie buff count
        /// </summary>
        private static int BuffCounts
        {
            get
            {
                int count = 0;
                if (Me.HasBuff("Pyromania"))
                {
                    count = Me.GetBuffCount("Pyromania");
                }
                else if (!Me.HasBuff("Pyromania") || HaveStun)
                {
                    count = 0;
                }
                return count;
            }
        }

        private static float GetDamage(Obj_AI_Base target, bool SpellQ = false, bool SpellW = false, bool SpellR = false, bool CastIgnite = false)
        {
            float damage = 0;

            if (target == null)
                damage = 0;

            if (SpellQ && Q.IsReady())
            {
                damage += Q.GetDamage(target);
            }

            if (SpellW && W.IsReady())
            {
                damage += W.GetDamage(target);
            }

            if (SpellR && R.IsReady())
            {
                damage += R.GetDamage(target);
            }

            if (CastIgnite && Ignite.IsReady())
            {
                damage += (float)Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            if (target.HasBuff("Undying Rage"))
                damage = 0;

            if (target.HasBuff("Judicator's Intervention"))
                damage = 0;

            if (target.HasBuff("KindredrNoDeathBuff"))
                damage = 0;

            return damage;
        }

        /// <summary>
        /// if you press Combo key , that said is in Combo mode
        /// </summary>
        private static bool ComboMode
        {
            get { return Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo; }
        }

        /// <summary>
        /// // if you press Harass key , that said is in Harass mode
        /// </summary>
        private static bool HarassMode
        {
            get { return Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed; }
        }

        /// <summary>
        /// if you press Clear key , that said is in Clear mode (LaneClear / JungleClear)
        /// </summary>
        private static bool ClearMode
        {
            get { return Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear; }
        }

        /// <summary>
        /// if you press LastHit key , that said is in LastHit mode
        /// </summary>
        private static bool LastHitMode
        {
            get { return Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit; }
        }

        /// <summary>
        /// if you not press any key , that said is None mode
        /// </summary>
        private static bool NoneMode
        {
            get { return Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None; }
        }
    }
}
