using System;
using System.Linq;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshTherulerofthesoul
{
    class Program
    {
        #region Init

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Thresh")
                return;
            
            LoadSpellData();
            LoadMenu();

            Chat.Print("<font color=\"#66CCFF\" >Kaiser's Thresh -The ruler of the soul</font><font color=\"#CCFFFF\" >{0}</font> - " +
               "<font color=\"#FFFFFF\" >Version " + Assembly.GetExecutingAssembly().GetName().Version + "</font>", Player.ChampionName);

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            EscapeBlocker.OnDetectEscape += EscapeBlocker_OnDetectEscape;
        }
        
        static Orbwalking.Orbwalker Orbwalker;
        public static Menu config;
        static Spell Q, W, E, R;
        static AIHeroClient catchedUnit = null;
        static int qTimer;
        static readonly AIHeroClient Player = ObjectManager.Player;

        //Mana
        static int QMana { get { return 80; } }
        static int WMana { get { return 50 * W.Level; } }
        static int EMana { get { return 60 * E.Level; } }
        static int RMana { get { return R.Level > 0 ? 100 : 0; } }

        static void LoadSpellData()
        {
            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 380);

            Q.SetSkillshot(0.5f, 60f, 1900f, true, SkillshotType.SkillshotLine);
        }

        static void LoadMenu()
        {
            config = new Menu("Kaiser's Thresh", "Kaiser's Thresh", true);

            //OrbWalk
            Orbwalker = new Orbwalking.Orbwalker(config.SubMenu("Orbwalking"));

            //Target selector
            var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            {
                TargetSelector.AddToMenu(TargetSelectorMenu);

                config.AddSubMenu(TargetSelectorMenu);
            }

            var combomenu = new Menu("Combo", "Combo");
            {
                var Qmenu = new Menu("Q", "Q");
                {
                    Qmenu.AddItem(new MenuItem("Predict", "Set Predict", true).SetValue(new StringList(new[] { "L#Predict", "L#Predict2" }, 1)));
                    Qmenu.AddItem(new MenuItem("C-UseQ", "Use Q", true).SetValue(true));
                    Qmenu.AddItem(new MenuItem("C-UseQ2", "Use Q2 AutoMatical", true).SetValue(true));
                    combomenu.AddSubMenu(Qmenu);
                }
                var Wmenu = new Menu("W", "W");
                {
                    Wmenu.AddItem(new MenuItem("C-UseHW", "Use Hooeked W", true).SetValue(true));
                    Wmenu.AddItem(new MenuItem("Use-SafeLantern", "Use SafeLantern for our team", true).SetValue(true));
                    Wmenu.AddItem(new MenuItem("C-UseSW", "Use Shield W Min 3", true).SetValue(true));

                    combomenu.AddSubMenu(Wmenu);
                }
                var Emenu = new Menu("E", "E");
                {
                    Emenu.AddItem(new MenuItem("C-UseE", "Use E", true).SetValue(true));
                    combomenu.AddSubMenu(Emenu);
                }
                var Rmenu = new Menu("R", "R");
                {
                    Rmenu.AddItem(new MenuItem("C-UseR", "Use Auto R", true).SetValue(true));
                    Rmenu.AddItem(new MenuItem("minNoEnemies", "Min No. Of Enemies R", true).SetValue(new Slider(2, 1, 5)));
                    combomenu.AddSubMenu(Rmenu);
                }
                combomenu.AddItem(new MenuItem("ComboActive", "Combo", true).SetValue(new KeyBind(32, KeyBindType.Press)));
                combomenu.AddItem(new MenuItem("FlayPush", "Flay Push Key", true).SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Press)));
                combomenu.AddItem(new MenuItem("FlayPull", "Flay Pull Key", true).SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Press)));
                combomenu.AddItem(new MenuItem("SafeLanternKey", "Safe Lantern Key", true).SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Press)));

                config.AddSubMenu(combomenu);
            }

            var harassmenu = new Menu("Harass", "Harass");
            {
                var Qmenu = new Menu("Q", "Q");
                {
                    Qmenu.AddItem(new MenuItem("H-UseQ", "Use Q", true).SetValue(true));
                    harassmenu.AddSubMenu(Qmenu);
                }
                var Emenu = new Menu("E", "E");
                {
                    Emenu.AddItem(new MenuItem("H-UseE", "Use E", true).SetValue(true));
                    harassmenu.AddSubMenu(Emenu);
                }
                harassmenu.AddItem(new MenuItem("HarassActive", "Harass", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                harassmenu.AddItem(new MenuItem("Mana", "ManaManager", true).SetValue(new Slider(30, 0, 100)));
                config.AddSubMenu(harassmenu);
            }

            var KSmenu = new Menu("KS", "KS");
            {
                KSmenu.AddItem(new MenuItem("KS-UseQ", "Use Q KS", true).SetValue(true));
                KSmenu.AddItem(new MenuItem("KS-UseE", "Use E KS", true).SetValue(true));
                KSmenu.AddItem(new MenuItem("KS-UseR", "Use R KS", true).SetValue(false));

                config.AddSubMenu(KSmenu);
            }

            var Miscmenu = new Menu("Misc", "Misc");
            {
                Miscmenu.AddItem(new MenuItem("UseEGapCloser", "Use E On Gap Closer", true).SetValue(true));
                Miscmenu.AddItem(new MenuItem("UseQInterrupt", "Use Q On Interrupt", true).SetValue(true));
                Miscmenu.AddItem(new MenuItem("UseEInterrupt", "Use E On Interrupt", true).SetValue(true));
                Miscmenu.AddItem(new MenuItem("AntiRengar", "Use E AntiGapCloser (Rengar Passive)(Beta)", true).SetValue(false));
                Miscmenu.AddItem(new MenuItem("DebugMode", "Debug Mode", true).SetValue(false));

                var EscapeMenu = new Menu("Block Enemy Escape Skills", "Block Enemy Escape Skills");
                {
                    EscapeMenu.AddItem(new MenuItem("BlockEscapeE", "Use E When Enemy have to Use Escape Skills", true).SetValue(true));
                    //EscapeMenu.AddItem(new MenuItem("BlockEscapeQ", "Use Q When Enemy have to Use Escape Skills", true).SetValue(true));
                    //EscapeMenu.AddItem(new MenuItem("BlockEscapeFlash", "Use Q When Enemy have to Use Flash", true).SetValue(true));

                    Miscmenu.AddSubMenu(EscapeMenu);
                }

                config.AddSubMenu(Miscmenu);
            }

            Items.LoadItems();

            var Drawingmenu = new Menu("Drawings", "Drawings");
            {
                Drawingmenu.AddItem(new MenuItem("DrawTarget", "Draw Target", true).SetValue(true));
                Drawingmenu.AddItem(new MenuItem("Qcircle", "Q Range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                Drawingmenu.AddItem(new MenuItem("Wcircle", "W Range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
                Drawingmenu.AddItem(new MenuItem("Ecircle", "E Range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                Drawingmenu.AddItem(new MenuItem("Rcircle", "R Range", true).SetValue(new Circle(false, Color.FromArgb(255, 255, 255, 255))));

                config.AddSubMenu(Drawingmenu);
            }
            config.AddItem(new MenuItem("PermaShow", "PermaShow", true).SetShared().SetValue(true)).ValueChanged += (s, args) => {
                if (args.GetNewValue<bool>())
                {
                    config.Item("ComboActive", true).Permashow(true, "Combo", SharpDX.Color.Aqua);
                    config.Item("HarassActive", true).Permashow(true, "Harass", SharpDX.Color.Aqua);
                    config.Item("FlayPush", true).Permashow(true, "E Push", SharpDX.Color.AntiqueWhite);
                    config.Item("FlayPull", true).Permashow(true, "E Pull", SharpDX.Color.AntiqueWhite);
                    config.Item("SafeLanternKey", true).Permashow(true, "Safe Lantern", SharpDX.Color.Aquamarine);
                }
                else
                {
                    config.Item("ComboActive", true).Permashow(false, "Combo");
                    config.Item("HarassActive", true).Permashow(false, "Harass");
                    config.Item("FlayPush", true).Permashow(false, "E Push");
                    config.Item("FlayPull", true).Permashow(false, "E Pull");
                    config.Item("SafeLanternKey", true).Permashow(false, "Safe Lantern");
                }
            };
            config.Item("ComboActive", true).Permashow(config.IsBool("PermaShow"), "Combo", SharpDX.Color.Aqua);
            config.Item("HarassActive", true).Permashow(config.IsBool("PermaShow"), "Harass", SharpDX.Color.Aqua);
            config.Item("FlayPush", true).Permashow(config.IsBool("PermaShow"), "E Push", SharpDX.Color.AntiqueWhite);
            config.Item("FlayPull", true).Permashow(config.IsBool("PermaShow"), "E Pull", SharpDX.Color.AntiqueWhite);
            config.Item("SafeLanternKey", true).Permashow(config.IsBool("PermaShow"), "Safe Lantern", SharpDX.Color.Aquamarine);

            config.AddToMainMenu();
        }

        #endregion

        #region Logic

        static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var Etarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (config.IsActive("ComboActive"))
            {
                if (target != null)
                {
                    if (CastQ2())
                    {
                        CastCatchedLatern();
                    }
                    if (config.IsBool("C-UseQ") && Q.IsReady())
                    {
                        CastQ(target);
                    }
                    if (config.IsBool("C-UseSW") && W.IsReady())
                    {
                        ShieldLantern();
                    }
                    KSCheck(target);
                }

                if (Etarget != null)
                {
                    if (config.IsBool("C-UseE") && E.IsReady())
                    {
                        CastE(Etarget);
                    }
                }
            }

            if (config.IsActive("FlayPush") && Etarget != null && 
                E.IsReady())
            {
                Push(Etarget);
            }

            if (config.IsActive("FlayPull") && Etarget != null &&
                E.IsReady())
            {
                Pull(Etarget);
            }

            if (config.IsActive("SafeLanternKey"))
            {
                SafeLanternKeybind();
            }

            if (config.IsBool("Use-SafeLantern"))
            {
                SafeLantern();
            }
        }

        static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var Etarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var mana = config.Item("Mana", true).GetValue<Slider>().Value;

            if (Player.ManaPercents() < mana)
                return;

            if (config.IsActive("HarassActive"))
            {
                if (config.IsBool("H-UseE") && E.IsReady() && Etarget != null)
                {
                    CastE(Etarget);
                }
                if (config.IsBool("H-UseQ") && Q.IsReady() && target != null)
                {
                    CastQ(target);
                }
            }
        }

        static void KSCheck(AIHeroClient target)
        {
            if (target != null)
            {
                if (config.Item("KS-UseQ", true).GetValue<bool>())
                {
                    var myDmg = Player.GetSpellDamage(target, SpellSlot.Q);
                    if (myDmg >= target.Health)
                    {
                        CastQ(target);
                    }
                }
                if (config.Item("KS-UseE", true).GetValue<bool>())
                {
                    var myDmg = Player.GetSpellDamage(target, SpellSlot.E);
                    if (myDmg >= target.Health)
                    {
                        CastE(target);
                    }
                }
                if (config.Item("KS-UseR", true).GetValue<bool>())
                {
                    var myDmg = Player.GetSpellDamage(target, SpellSlot.R);
                    if (myDmg >= target.Health)
                    {
                        if (Player.Distance(target.Position) <= R.Range)
                        {
                            R.Cast();
                        }
                    }
                }
            }
        }

        #endregion

        #region Q

        static void CastQ(AIHeroClient target)
        {
            if (!Q.IsReady() || target == null || Helper.EnemyHasShield(target) || !target.IsValidTarget())
                return;

            var Catched = IsPulling().Item1;
            var CatchedQtarget = IsPulling().Item2;

            if (!Catched && qTimer == 0)
            {
                if (!E.IsReady() || (E.IsReady() && 
                    E.Range < Player.Distance(target.Position)))
                {
                    var Mode = config.Item("Predict", true).GetValue<StringList>().SelectedIndex;
                    
                    switch(Mode)
                    {
                        #region L# Predict
                        case 0:
                            {
                                var b = Q.GetPrediction(target);

                                if (b.Hitchance >= HitChance.High &&
                                    Player.Distance(target.ServerPosition) < Q.Range)
                                {
                                    Q.Cast(target);
                                }
                            }
                            break;
                        #endregion

                        #region L# Predict2
                        case 1:
                            {
                                if (Player.Distance(target.ServerPosition) < Q.Range)
                                {
                                    Q.CastIfHitchanceEquals(target, HitChance.High);
                                }
                            }
                            break;
                        #endregion
                    }
                }
            }
            else if (Catched && Environment.TickCount > qTimer - 200 && CastQ2() && CatchedQtarget.Type == GameObjectType.AIHeroClient && config.IsBool("C-UseQ2"))
            {
                Q.Cast();
            }
        }

        static bool CastQ2()
        {
            var status = false;
            var Catched = IsPulling().Item1;
            var CatchedQtarget = IsPulling().Item2;

            if (Catched && CatchedQtarget != null && 
                CatchedQtarget.Type == GameObjectType.AIHeroClient && 
                !Turret.IsUnderEnemyTurret(CatchedQtarget))
            {
                var EnemiesCount = Helper.GetEnemiesNearTarget(CatchedQtarget).Count();
                var AlliesCount = GetAlliesNearTarget(CatchedQtarget).Item1;
                var CanKill = GetAlliesNearTarget(CatchedQtarget).Item2;

                if (CanKill)
                {
                    EnemiesCount = EnemiesCount - 1;
                }

                if (EnemiesCount == 0)
                {
                    status = true;
                }
                else if (AlliesCount >= EnemiesCount)
                {
                    status = true;
                }
                else if (E.IsReady() && Turret.IsUnderAllyTurret(CatchedQtarget))
                {
                    status = true;
                }
            }
            return status;
        }

        static Tuple<int, bool> GetAlliesNearTarget(AIHeroClient target)
        {
            var Count = 0;
            var status = false;
            double dmg = 0;
            double allyDmg = 0;

            foreach (AIHeroClient allyhero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsDead))
            {
                if (allyhero.Distance(target.Position) <= 900)
                {
                    Count += 1;

                    dmg = dmg + Player.GetAutoAttackDamage(target, true);
                    dmg = dmg + E.GetDamage(target);
                    dmg = R.IsReady() ? dmg + R.GetDamage(target) : dmg;

                    if (allyhero.ChampionName != Player.ChampionName)
                    {
                        allyDmg = allyDmg + Helper.GetAlliesComboDmg(target, allyhero);
                    }

                }
            }

            if (E.IsReady())
            {
                dmg = dmg * 2;
                allyDmg = allyDmg * 1.5;
            }

            var totalDmg = dmg + allyDmg;

            if (totalDmg > target.Health)
            {
                status = true;
            }

            return new Tuple<int, bool>(Count, status);
        }

        #endregion

        #region W

        static void CastW(Vector3 Position)
        {
            if (!W.IsReady() || Player.Distance(Position) > W.Range)
                return;

            W.Cast(Position);
        }

        static void CastCatchedLatern()
        {
            if (!W.IsReady() || !config.Item("C-UseHW", true).GetValue<bool>())
                return;

            bool Catched = IsPulling().Item1;
            AIHeroClient CatchedQtarget = IsPulling().Item2;

            if (Catched && CatchedQtarget != null && CatchedQtarget.Type == GameObjectType.AIHeroClient)
            {
                var Wtarget = GetFurthestAlly(CatchedQtarget);
                if (Wtarget != null)
                {
                    if (Player.Distance(Wtarget.Position) <= W.Range)
                    {
                        CastW(Wtarget.Position);
                    }
                    else if (Player.Distance(Wtarget.Position) > W.Range)
                    {
                        var Pos = Player.Position + (Wtarget.Position - Player.Position).Normalized() * W.Range;

                        CastW(Pos);
                    }
                }
            }
        }

        static AIHeroClient GetFurthestAlly(AIHeroClient target)
        {
            AIHeroClient Wtarget = null;
            float distance = 0;

            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && !x.IsDead))
            {
                if (Player.Distance(hero.Position) <= 1500 &&
                    hero.Distance(target.Position) > Player.Distance(target.Position))
                {
                    var temp = Player.Distance(hero.Position);

                    if (distance == 0 && Wtarget == null)
                    {
                        Wtarget = hero;
                        distance = Player.Distance(hero.Position);
                    }
                    else if (temp > distance)
                    {
                        Wtarget = hero;
                        distance = Player.Distance(hero.Position);
                    }
                }
            }
            return Wtarget;
        }

        static void ShieldLantern()
        {
            int count = 0;
            AIHeroClient target = null;
            foreach (var allyhero in ObjectManager.Get<AIHeroClient>().Where
                (x => x.IsAlly &&
                    Player.Distance(x.Position) < W.Range &&
                    !x.IsDead && !x.HasBuff("Recall")))
            {
                var tmp = LeagueSharp.Common.Utility.CountAlliesInRange(allyhero, 200);

                if (count == 0)
                {
                    count = tmp;
                }
                else if (tmp > count)
                {
                    count = tmp;
                }
            }

            if (count > 2 && target != null)
            {
                CastW(target.Position);
            }
        }

        static void SafeLantern()
        {
            if (!ManaManager())
                return;

            foreach (var hero in ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsAlly && !x.IsDead && !x.IsMe &&
                Player.Distance(x.Position) < 1500 && 
                !x.HasBuff("Recall")))
            {
                if (hero.HpPercents() < 25)
                {
                    if (Player.Distance(hero.Position) <= W.Range)
                    {
                        var Pos = W.GetPrediction(hero).CastPosition;

                        CastW(Pos);
                    }
                }
                else if (hero.HasBuffOfType(BuffType.Suppression) ||
                    hero.HasBuffOfType(BuffType.Taunt) ||
                    hero.HasBuffOfType(BuffType.Knockup) ||
                    hero.HasBuffOfType(BuffType.Flee))
                {
                    if (Player.Distance(hero.Position) <= W.Range)
                    {
                        CastW(hero.Position);
                    }
                }
            }
        }

        static void SafeLanternKeybind()
        {
            AIHeroClient Wtarget = null;
            float Hp = 0;

            foreach (var hero in ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsAlly && !x.IsDead && !x.IsMe &&
                Player.Distance(x.Position) < 1500 &&
                !x.HasBuff("Recall")))
            {
                var temp = hero.HpPercents();

                if (hero.HasBuffOfType(BuffType.Suppression) ||
                    hero.HasBuffOfType(BuffType.Taunt) ||
                    hero.HasBuffOfType(BuffType.Knockup) ||
                    hero.HasBuffOfType(BuffType.Flee))
                {
                    if (Player.Distance(hero.Position) <= W.Range)
                    {
                        CastW(hero.Position);
                    }
                }

                if (Wtarget == null && Hp == 0)
                {
                    Wtarget = hero;
                    Hp = temp;
                }
                else if (temp < Hp)
                {
                    Wtarget = hero;
                    Hp = temp;
                }
            }

            if (Wtarget != null)
            {
                CastW(Wtarget.Position);
            }
        }

        #endregion

        #region E

        static void CastE(AIHeroClient target)
        {
            if (!E.IsReady() || target == null || !target.IsValidTarget())
                return;

            bool Catched = IsPulling().Item1;
            AIHeroClient CatchedQtarget = IsPulling().Item2;

            if (!Catched && qTimer == 0)
            {
                if (Player.Distance(target.Position) <= E.Range)
                {
                    if (Player.HpPercents() < 20 && 
                        target.HpPercents() > 20)
                    {
                        Push(target);
                    }
                    else
                    {
                        Pull(target);
                    }
                }
            }
            else if (Catched && CatchedQtarget != null)
            {
                if (Environment.TickCount > qTimer - 200 && Player.Distance(CatchedQtarget.Position) <= E.Range)
                {
                    Pull(CatchedQtarget);
                }
            }
        }

        static void Pull(Obj_AI_Base target)
        {
            var pos = target.Position.Extend(Player.Position, Player.Distance(target.Position) + 200);
            E.Cast(pos);
        }

        static void Push(Obj_AI_Base target)
        {
            var pos = target.Position.Extend(Player.Position, Player.Distance(target.Position) - 200);
            E.Cast(pos);
        }

        #endregion

        #region R

        static void AutoR()
        {
            if (!R.IsReady() && config.Item("C-UseR", true).GetValue<bool>())
                return;

            // Menu Count
            int RequireCount = config.Item("minNoEnemies", true).GetValue<Slider>().Value;

            // Enemeis count in R range
            var hit = HeroManager.Enemies.Where(i => i.IsValidTarget(R.Range)).ToList();

            if (RequireCount <= hit.Count && R.IsReady())
            {
                R.Cast();
            }
        }

        #endregion

        #region Others

        static Tuple<bool, AIHeroClient> IsPulling()
        {
            bool Catched;
            AIHeroClient CatchedQtarget;

            if (catchedUnit != null)
            {
                Catched = true;
                CatchedQtarget = catchedUnit;
            }
            else
            {
                Catched = false;
                CatchedQtarget = null;
            }

            return new Tuple<bool, AIHeroClient>(Catched, CatchedQtarget);
        }

        static void CheckBuff()
        {
            if (Player.IsDead)
                return;

            foreach (AIHeroClient enemyhero in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy && !enemy.IsDead && enemy.IsValid))
            {
                if (enemyhero.HasBuff("ThreshQ") || enemyhero.HasBuff("threshqfakeknockup"))
                {
                    catchedUnit = enemyhero;
                    return;
                }
            }

            if (catchedUnit != null)
            {
                if (!catchedUnit.HasBuff("ThreshQ"))
                {
                    catchedUnit = null;
                }
            }
        }

        static bool ManaManager()
        {
            var status = false;
            var ReqMana = R.IsReady() ? QMana + EMana + RMana : QMana + EMana; 

            if (ReqMana < Player.Mana)
            {
                status = true;
            }
            else if (Player.MaxHealth * 0.3 > Player.Health)
            {
                status = true;
            }

            return status;
        }

        static bool Debug()
        {
            return config.IsBool("DebugMode");
        }

        public static void Debug(string s)
        {
            if (Debug())
            {
                Console.WriteLine("" + s);
            }
        }

        static void Debug(Vector3 pos)
        {
            if (!Debug())
                return;

            Drawing.OnDraw += delegate(EventArgs args)
            {
                Render.Circle.DrawCircle(pos, 150, System.Drawing.Color.Yellow);
            };
        }

        #endregion 

        #region Events

        static void Game_OnUpdate(EventArgs args)
        {
            CheckBuff();

            Combo();
            Harass();

            AutoR();
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!config.Item("UseEInterrupt", true).GetValue<bool>())
                return;

            if (Player.Distance(sender.Position) < E.Range && sender.IsEnemy)
            {
                if (E.IsReady())
                {
                    Chat.Print("Debug : EInterrupt");
                    Pull(sender);
                }
            }

            if (Player.Distance(sender.ServerPosition) < Q.Range && 
                (!E.IsReady() || (E.IsReady() && E.Range < Player.Distance(sender.Position))) && 
                sender.IsEnemy && args.DangerLevel == Interrupter2.DangerLevel.High && 
                args.EndTime > Utils.TickCount + Q.Delay + (Player.Distance(sender.Position) / Q.Speed))
            {
                if (Q.IsReady())
                {
                    CastQ(sender);
                }
            }
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!config.Item("UseEGapCloser", true).GetValue<bool>())
                return;

            if (Player.Distance(gapcloser.Sender.Position) < E.Range && gapcloser.Sender.IsEnemy)
            {
                if (E.IsReady())
                {
                    Push(gapcloser.Sender);
                    Debug("AntiGapclose");
                }
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe && Player.Distance(x.Position) < E.Range))
            {
                if (gapcloser.End.Distance(hero.Position) < 100 &&
                    E.IsReady())
                {
                    Push(gapcloser.Sender);
                    Debug("AntiGapclose");
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            var QCircle = config.Item("Qcircle", true).GetValue<Circle>();
            var WCircle = config.Item("Wcircle", true).GetValue<Circle>();
            var ECircle = config.Item("Ecircle", true).GetValue<Circle>();
            var RCircle = config.Item("Rcircle", true).GetValue<Circle>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (QCircle.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, QCircle.Color);
            }

            if (WCircle.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, WCircle.Color);
            }

            if (ECircle.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, ECircle.Color);
            }

            if (RCircle.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, RCircle.Color);
            }

            if (config.Item("DrawTarget", true).GetValue<bool>() && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 150, System.Drawing.Color.Red);
            }
        }

        static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation.ToLower() == "spell1_in")
                {
                    qTimer = Environment.TickCount + 1200;
                }
                else if (args.Animation.ToLower() == "spell1_out")
                {
                    qTimer = 0;
                }
                else if (args.Animation.ToLower() == "spell1_pull1")
                {
                    qTimer = Environment.TickCount + 900;
                }
                else if (args.Animation.ToLower() == "spell1_pull2")
                {
                    qTimer = Environment.TickCount + 900;
                }
                else if (qTimer > 0 && Environment.TickCount > qTimer)
                {
                    qTimer = 0;
                }
            }

            if (config.IsBool("AntiRengar"))
                return;

            if (!(sender is AIHeroClient))
                return;

            var _sender = sender as AIHeroClient;
            var dis = _sender.GetBuffCount("rengartrophyicon1") > 5 ? 600 : 750;
            
            if (_sender.ChampionName == "Rengar" && args.Animation == "Spell5" &&
                Player.Distance(_sender.Position) < dis && E.IsReady())
            {
                Push(_sender);
            }
        }
        
        static void EscapeBlocker_OnDetectEscape(AIHeroClient sender, GameObjectEscapeDetectorEventArgs args)
        {
            if (!sender.IsEnemy)
                return;

            #region BLockFlashEscape
            /*
            if (Menubool("BlockEscapeFlash") && sender.IsEnemy &&
                args.SpellData == "summonerflash")
            {
                if (Player.Distance(args.End) < Q.Range && Q.IsReady() &&
                    Player.Distance(args.End) > E.Range)
                {
                    Debug(args.End);
                    Debug("flash");

                    var predict = Q.GetPrediction(sender);

                    if (predict.Hitchance != HitChance.Collision)
                    {
                        Debug("EscapeFlash");
                        Q.Cast(args.End);
                    }
                }
            }
            */
            #endregion

            #region BLockSpellsEscape

            if (args.SpellData == "summonerflash")
                return;

            if (Player.Distance(args.Start) < E.Range && E.IsReady() &&
                    Player.Distance(args.End) > E.Range &&
                    config.IsBool("BlockEscapeE"))
            {
                Debug(args.End);
                Debug("EscapeE");
                Pull(sender);
            }
                /*
            else if ((!E.IsReady() || Player.Distance(args.Start) > E.Range) &&
                Player.Distance(args.End) < Q.Range && Q.IsReady() &&
                Player.Distance(args.End) > E.Range &&
                Menubool("BlockEscapeQ"))
            {
                var predict = Q.GetPrediction(sender);

                if (predict.Hitchance != HitChance.Collision)
                {
                    Debug(args.End);
                    Debug("EscapeQ");
                    Q.Cast(args.End);
                }
            }
            */
            #endregion
        }
        
        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Debug() && sender.IsEnemy && sender is AIHeroClient)
            {
                var _sender = sender as AIHeroClient;
                Console.WriteLine(": " + args.SData.Name + " - " + _sender.ChampionName + _sender.GetSpellSlot(args.SData.Name));
            }
        }

        #endregion
    }
}
