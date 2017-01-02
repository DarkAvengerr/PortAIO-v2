using System;
using System.Linq;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace OriannaTheruleroftheBall
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
            if (Player.ChampionName != "Orianna")
                return;
            
            LoadSpellData();
            LoadMenu();

            Chat.Print("<font color=\"#66CCFF\" >Kaiser's Orianna -The ruler of the Ball</font><font color=\"#CCFFFF\" >{0}</font> - " +
               "<font color=\"#FFFFFF\" >Version " + Assembly.GetExecutingAssembly().GetName().Version + "</font>", Player.ChampionName);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            BallManager.BallManagerInit();
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        static Orbwalking.Orbwalker Orbwalker;
        static Menu config;
        static Spell Q, W, E, R;
        static readonly AIHeroClient Player = ObjectManager.Player;
        static readonly SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");

        static void LoadSpellData()
        {
            Q = new Spell(SpellSlot.Q, 820);
            W = new Spell(SpellSlot.W, 225);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 375);

            Q.SetSkillshot(0.1f, 125f, 1300f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.15f, 225f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 80f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 375f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        static void LoadMenu()
        {
            config = new Menu("Kaiser's Orianna", "Kaiser's Orianna", true);

            #region Orbwalker
            Orbwalker = new Orbwalking.Orbwalker(config.SubMenu("Orbwalking")); 
            #endregion

            #region TargetSelectorMenu
            var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            {
                TargetSelector.AddToMenu(TargetSelectorMenu);

                config.AddSubMenu(TargetSelectorMenu);
            } 
            #endregion

            #region ComboMenu
            var combomenu = new Menu("Combo", "Combo");
            {
                var Qmenu = new Menu("Q", "Q");
                {
                    Qmenu.AddItem(new MenuItem("C-UseQ", "Use Q", true).SetValue(true));
                    Qmenu.AddItem(new MenuItem("C-UseEQ", "Use EQ Combo", true).SetValue(true));
                    combomenu.AddSubMenu(Qmenu);
                }
                var Wmenu = new Menu("W", "W");
                {
                    Wmenu.AddItem(new MenuItem("C-UseW", "Use W", true).SetValue(true));
                    Wmenu.AddItem(new MenuItem("AutoW", "Use Auto W", true).SetValue(true));
                    Wmenu.AddItem(new MenuItem("WminNoEnemies", "Min No. Of Enemies W", true).SetValue(new Slider(2, 1, 5)));
                    combomenu.AddSubMenu(Wmenu);
                }
                var Emenu = new Menu("E", "E");
                {
                    Emenu.AddItem(new MenuItem("C-UseE", "Use E", true).SetValue(true));
                    Emenu.AddItem(new MenuItem("C-UseER", "Use ER Combo With Ally", true).SetValue(true));
                    Emenu.AddItem(new MenuItem("useEHit", "Use E Hit", true).SetValue(true));
                    Emenu.AddItem(new MenuItem("AutoShield", "Use Auto Shield", true).SetValue(true));
                    combomenu.AddSubMenu(Emenu);
                }
                var Rmenu = new Menu("R", "R");
                {
                    Rmenu.AddItem(new MenuItem("C-UseR", "Use R (Combo Mode)", true).SetValue(true));
                    Rmenu.AddItem(new MenuItem("RminNoEnemies", "Min No. Of Enemies R (Combo Mode)", true).SetValue(new Slider(2, 1, 5)));
                    Rmenu.AddItem(new MenuItem("AutoR", "Use R (Any Time)", true).SetValue(true));
                    Rmenu.AddItem(new MenuItem("AutoRminNoEnemies", "Min No. Of Enemies R (Any Time)", true).SetValue(new Slider(3, 1, 5)));

                    combomenu.AddSubMenu(Rmenu);
                }
                var SummonerSpellsmenu = new Menu("Summoner Spells", "Summoner Spells");
                {
                    SummonerSpellsmenu.AddItem(new MenuItem("C-UseIgnite", "Use Ignite On ComboMode", true).SetValue(true));
                    combomenu.AddSubMenu(SummonerSpellsmenu);
                }
                combomenu.AddItem(new MenuItem("ComboActive", "Combo", true).SetValue(new KeyBind(32, KeyBindType.Press)));
                combomenu.AddItem(new MenuItem("BurstActive", "Burst Combo", true).SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Press)));
                config.AddSubMenu(combomenu);
            }
            #endregion

            #region HarassMenu
            var harassmenu = new Menu("Harass", "Harass");
            {
                var Qmenu = new Menu("Q", "Q");
                {
                    Qmenu.AddItem(new MenuItem("H-UseQ", "Use Q", true).SetValue(true));
                    harassmenu.AddSubMenu(Qmenu);
                }
                var Wmenu = new Menu("W", "W");
                {
                    Wmenu.AddItem(new MenuItem("H-UseW", "Use W", true).SetValue(true));
                    harassmenu.AddSubMenu(Wmenu);
                }

                harassmenu.AddItem(new MenuItem("HarassActive", "Harass", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                harassmenu.AddItem(new MenuItem("HarassToggle", "Harass Toggle", true).SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));
                harassmenu.AddItem(new MenuItem("HMana", "Harass ManaManager", true).SetValue(new Slider(30, 0, 100)));
                config.AddSubMenu(harassmenu);
            } 
            #endregion

            #region FarmMenu
            var FarmMenu = new Menu("Farm", "Farm");
            {
                var LastHitMenu = new Menu("LastHit", "LastHit");
                {
                    var Qmenu = new Menu("Q", "Q");
                    {
                        Qmenu.AddItem(new MenuItem("LH-UseQ", "Use Q", true).SetValue(true));
                        LastHitMenu.AddSubMenu(Qmenu);
                    }
                    FarmMenu.AddSubMenu(LastHitMenu);
                }

                var LaneClearMenu = new Menu("LaneClear", "LaneClear");
                {
                    var Qmenu = new Menu("Q", "Q");
                    {
                        Qmenu.AddItem(new MenuItem("LC-UseQ", "Use Q", true).SetValue(true));
                        LaneClearMenu.AddSubMenu(Qmenu);
                    }
                    var Wmenu = new Menu("W", "W");
                    {
                        Wmenu.AddItem(new MenuItem("LC-UseW", "Use W", true).SetValue(true));
                        LaneClearMenu.AddSubMenu(Wmenu);
                    }
                    FarmMenu.AddSubMenu(LaneClearMenu);
                }

                FarmMenu.AddItem(new MenuItem("LastHitActive", "LastHit", true).SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
                FarmMenu.AddItem(new MenuItem("LaneClearActive", "LaneClear", true).SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                FarmMenu.AddItem(new MenuItem("FMana", "Farm ManaManager", true).SetValue(new Slider(0, 0, 100)));
                config.AddSubMenu(FarmMenu);
            }
            #endregion

            #region KSMenu
            var KSmenu = new Menu("KS", "KS");
            {
                KSmenu.AddItem(new MenuItem("KS-UseR", "Use R Smart KS", true).SetValue(true));

                config.AddSubMenu(KSmenu);
            } 
            #endregion

            #region InitatorMenu
            var initator = new Menu("Initiator", "Initiator");
            {
                initator.AddItem(new MenuItem("useInitiator", "Use Initiator", true)).SetValue(true);
                foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly))
                {
                    foreach (Initiator.Initiatorinfo x in Initiator.InitiatorList)
                    {
                        if (x.Hero == hero.BaseSkinName)
                        {
                            initator.AddItem(new MenuItem(x.Spell, x.Spell, true)).SetValue(true);
                        }
                    }
                }
                config.AddSubMenu(initator);
            } 
            #endregion

            #region MiscMenu
            var Miscmenu = new Menu("Misc", "Misc");
            {
                Miscmenu.AddItem(new MenuItem("BlockR", "Use Block R (no Enemy)", true).SetValue(true));
                Miscmenu.AddItem(new MenuItem("Inter-UseR", "Use Interrupt R", true).SetValue(true));
                Miscmenu.AddItem(new MenuItem("DebugMode", "Debug Mode", true).SetValue(false));

                config.AddSubMenu(Miscmenu);
            } 
            #endregion

            #region DrawingMenu
            var Drawingmenu = new Menu("Drawings", "Drawings");
            {
                Drawingmenu.AddItem(new MenuItem("DrawTarget", "Draw Target", true).SetValue(true));
                Drawingmenu.AddItem(new MenuItem("DrawBall", "Draw Current Ball Position", true).SetValue(true));
                Drawingmenu.AddItem(new MenuItem("Qcircle", "Q Range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                MenuItem drawComboDamageMenu = new MenuItem("DmgDraw", "Draw Combo Damage", true).SetValue(true);
                MenuItem drawFill = new MenuItem("DmgFillDraw", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                Drawingmenu.AddItem(drawComboDamageMenu);
                Drawingmenu.AddItem(drawFill);
                //DamageIndicator.DamageToUnit = GetComboDamage;
                //DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                //DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                //DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        //DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        //DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        //DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };

                config.AddSubMenu(Drawingmenu);
            } 
            #endregion

            #region PermaShow
            config.AddItem(new MenuItem("PermaShow", "PermaShow", true).SetShared().SetValue(true)).ValueChanged += (s, args) =>
            {
                if (args.GetNewValue<bool>())
                {
                    config.Item("ComboActive", true).Permashow(true, "Combo", SharpDX.Color.Aqua);
                    config.Item("BurstActive", true).Permashow(true, "Burst", SharpDX.Color.Aqua);
                    config.Item("HarassActive", true).Permashow(true, "Harass", SharpDX.Color.Aqua);
                    config.Item("LastHitActive", true).Permashow(true, "LastHit", SharpDX.Color.AntiqueWhite);
                    config.Item("LaneClearActive", true).Permashow(true, "LaneClear", SharpDX.Color.AntiqueWhite);
                }
                else
                {
                    config.Item("ComboActive", true).Permashow(false, "Combo");
                    config.Item("BurstActive", true).Permashow(false, "Burst");
                    config.Item("HarassActive", true).Permashow(false, "Harass");
                    config.Item("LastHitActive", true).Permashow(false, "LastHit");
                    config.Item("LaneClearActive", true).Permashow(false, "LaneClear");
                }
            };
            config.Item("ComboActive", true).Permashow(config.IsBool("PermaShow"), "Combo", SharpDX.Color.Aqua);
            config.Item("BurstActive", true).Permashow(config.IsBool("PermaShow"), "Burst", SharpDX.Color.Aqua);
            config.Item("HarassActive", true).Permashow(config.IsBool("PermaShow"), "Harass", SharpDX.Color.AntiqueWhite);
            config.Item("LastHitActive", true).Permashow(config.IsBool("PermaShow"), "LastHit", SharpDX.Color.AntiqueWhite);
            config.Item("LaneClearActive", true).Permashow(config.IsBool("PermaShow"), "LaneClear", SharpDX.Color.Aquamarine);
            #endregion

            config.AddToMainMenu();
        }

        #endregion

        #region Logic

        static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (IgniteSlot != SpellSlot.Unknown && IgniteSlot.IsReady() && config.IsBool("C-UseIgnite"))
                {
                    Ignite();
                }
                if (config.IsBool("C-UseER") && E.IsReady() && R.IsReady())
                {
                    GetER();
                }
                if (config.IsBool("C-UseEQ") && E.IsReady())
                {
                    CastE(target);
                }
                if (config.IsBool("C-UseQ") && Q.IsReady())
                {
                    CastQ(target);
                }
                if (config.IsBool("C-UseW") && W.IsReady())
                {
                    CastW(target);
                }
                if (config.IsBool("C-UseR") && R.IsReady())
                {
                    CastR();
                }
                KSCheck(target);
            }
        }

        static void BurstCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (IgniteSlot != SpellSlot.Unknown && IgniteSlot.IsReady() && config.IsBool("C-UseIgnite"))
                {
                    Ignite();
                }
                if (Q.IsReady())
                {
                    CastQ(target);
                }
                if (W.IsReady())
                {
                    CastW(target);
                }
                if (R.IsReady() && !BallManager.Ball.IsMoving)
                {
                    if (BallManager.Ball.Position.Distance(target.ServerPosition) < R.Width)
                    {
                        R.Cast();
                    }
                }
            }
        }

        static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (config.IsBool("C-UseQ") && Q.IsReady())
                {
                    CastQ(target);
                }
                if (config.IsBool("C-UseW") && W.IsReady())
                {
                    CastW(target);
                }
            }
        }

        static void KSCheck(AIHeroClient target)
        {
            if (!config.IsBool("KS-UseR") || !R.IsReady())
                return;

            if (target.Health < GetComboDamage(target))
            {
                if (GetComboDamage(target) - R.GetDamage(target) < target.Health)
                {
                    Q.UpdateSourcePosition(BallManager.Ball.Position, BallManager.Ball.Position);
                    var predict = R.GetPrediction(target, true);
                    if (predict.Hitchance >= HitChance.High)
                    {
                        R.Cast();
                    }
                }
            }
        }

        static void Ignite()
        {
            var IgniteTarget = TargetSelector.GetTarget(600, TargetSelector.DamageType.True);
            if (IgniteTarget != null && IgniteTarget.Health < GetComboDamage(IgniteTarget))
            {
                Player.Spellbook.CastSpell(IgniteSlot, IgniteTarget);
            }
        }

        static void Farm()
        {
            if (BallManager.Ball.IsMoving)
                return;

            if (config.IsActive("LastHitActive"))
            {
                if (config.IsBool("LH-UseQ") && Q.IsReady())
                {
                    var minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);

                    foreach (var minion in minions)
                    {
                        var helath = HealthPrediction.GetHealthPrediction(minion, (int)(Q.Delay + Player.Distance(minion.Position) / Q.Speed));
                        var minionPredict = Prediction.GetPrediction(minion, Q.Delay, Q.Width, Q.Speed);

                        if (Q.GetDamage(minion) > helath + 10 && minionPredict.Hitchance >= HitChance.High)
                        {
                            Q.Cast(minionPredict.CastPosition);
                        }
                    }
                }
            }
            else if (config.IsActive("LaneClearActive"))
            {
                if (config.IsBool("LC-UseQ") && Q.IsReady())
                {
                    var minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy);
                    var predict = Q.GetCircularFarmLocation(minions);

                    if (predict.MinionsHit > 0)
                    {
                        Q.Cast(predict.Position);
                    }

                    var JGminions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral);
                    var JGpredict = Q.GetCircularFarmLocation(JGminions);

                    if (JGpredict.MinionsHit > 0)
                    {
                        Q.Cast(JGpredict.Position);
                    }
                }

                if (config.IsBool("LC-UseW") && W.IsReady())
                {
                    var minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                    if (minions.Where(x => BallManager.Ball.Position.Distance(x.Position) < W.Width).Count() > 1)
                    {
                        W.Cast();
                    }
                }
            }
        }

        #endregion

        #region Q

        static void CastQ(AIHeroClient target)
        {
            if (BallManager.Ball.IsMoving)
                return;
            
            switch (BallManager.Ball.Status)
            {
                case BallManager.Ballstatus.Land:
                    {
                        Q.UpdateSourcePosition(BallManager.Ball.Position, BallManager.Ball.Position);
                        var pred = Q.GetPrediction(target, true);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    break;
                case BallManager.Ballstatus.Me:
                    {
                        Q.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                        var pred = Q.GetPrediction(target, true);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    break;
                case BallManager.Ballstatus.Ally:
                    {
                        Q.UpdateSourcePosition(BallManager.Ball.Position, BallManager.Ball.Position);
                        var pred = Q.GetPrediction(target, true);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    break;
                default:
                    Debug("Q-Default");
                    break;
            } 
        }

        static void CastQ(Obj_AI_Base target)
        {
            if (BallManager.Ball.IsMoving)
                return;

            switch (BallManager.Ball.Status)
            {
                case BallManager.Ballstatus.Land:
                    {
                        Q.UpdateSourcePosition(BallManager.Ball.Position, BallManager.Ball.Position);
                        var pred = Q.GetPrediction(target, true);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    break;
                case BallManager.Ballstatus.Me:
                    {
                        Q.UpdateSourcePosition(Player.ServerPosition, Player.ServerPosition);
                        var pred = Q.GetPrediction(target, true);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    break;
                case BallManager.Ballstatus.Ally:
                    {
                        Q.UpdateSourcePosition(BallManager.Ball.Position, BallManager.Ball.Position);
                        var pred = Q.GetPrediction(target, true);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    break;
                default:
                    Debug("Q-Default");
                    break;
            }
        }

        #endregion

        #region W

        static void CastW(AIHeroClient target)
        {
            if (BallManager.Ball.IsMoving)
                return;

            if (BallManager.Ball.Position.Distance(target.ServerPosition) < W.Range)
            {
                W.Cast();
            }
        }

        static void AutoW()
        {
            if (BallManager.Ball.IsMoving)
                return;

            var ReqCount = config.GetValue("WminNoEnemies");

            if (HeroManager.Enemies
                .Where(x =>
                    x.IsValidTarget() &&
                    BallManager.Ball.Position.Distance(x.ServerPosition) < W.Width).Count() >= ReqCount)
            {
                W.Cast();
            }
        }

        #endregion

        #region E

        static void CastE(AIHeroClient target)
        {
            if (!target.IsValidTarget() || BallManager.Ball.IsMoving)
                return;

            if (Q.IsReady())
            {
                if (BallManager.Ball.Status == BallManager.Ballstatus.Ally)
                {
                    var Etarget = GetEQ(target, BallManager.Ball.Position);

                    if (Etarget != null && Etarget != BallManager.Ball.Hero)
                    {
                        Debug("EQ");
                        E.CastOnUnit(Etarget);
                    }
                }
                else if (BallManager.Ball.Status == BallManager.Ballstatus.Land)
                {
                    var Etarget = GetEQ(target, BallManager.Ball.Position);

                    if (Etarget != null)
                    {
                        Debug("EQ");
                        E.CastOnUnit(Etarget);
                    }
                }
            }
        }

        static AIHeroClient GetEQ(AIHeroClient target, Vector3 Ball)
        {
            AIHeroClient NewETarget = null;

            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>()
                .Where(x => 
                    x.IsAlly && 
                    !x.IsDead && 
                    Player.Distance(x.Position) <= E.Range))
            {
                if (target.Distance(Ball) - target.Distance(hero.Position) > 150) //1300 * 0.25 
                {
                    if (NewETarget == null)
                    {
                        NewETarget = hero;
                    }
                    else if (Player.Distance(hero.Position) < Player.Distance(NewETarget.Position))
                    {
                        NewETarget = hero;
                    }
                }
            }
            return NewETarget;
        }

        static void GetEW()
        {
            AIHeroClient Etarget = null;
            var WCount = 0;

            foreach (var ally in ObjectManager.Get<AIHeroClient>()
                .Where(x =>
                    x.IsAlly &&
                    !x.IsDead &&
                    Player.Distance(x.ServerPosition) <= E.Range))
            {
                var Temp = GetEnemiesNearHero(ally, W.Width);

                if (Etarget == null && WCount == 0)
                {
                    Etarget = ally;
                    WCount = Temp;
                }
                else if (WCount < Temp)
                {
                    Etarget = ally;
                    WCount = Temp;
                }
            }

            if (Etarget != null && WCount >= config.GetValue("WminNoEnemies"))
            {
                Debug("EW");
                E.CastOnUnit(Etarget);
            }
        }

        static void GetER()
        {
            AIHeroClient Etarget = null;
            var RCount = 0;

            foreach (var ally in ObjectManager.Get<AIHeroClient>()
                .Where(x => 
                    x.IsAlly &&
                    !x.IsDead &&
                    Player.Distance(x.ServerPosition) <= E.Range))
            {
                var Temp = GetEnemiesNearHero(ally, R.Width);

                if (Etarget == null && RCount == 0)
                {
                    Etarget = ally;
                    RCount = Temp;
                }
                else if (RCount < Temp)
                {
                    Etarget = ally;
                    RCount = Temp;
                }
            }

            if (Etarget != null && RCount >= config.GetValue("RminNoEnemies"))
            {
                Debug("ER");
                E.CastOnUnit(Etarget);
            }
        }

        static int GetEnemiesNearHero(AIHeroClient hero, float range)
        {
            return HeroManager.Enemies.Where(x => x.IsEnemy && !x.IsDead && x.IsValidTarget() && hero.Distance(x.ServerPosition) < range).Count();
        }

        static void ELineCheckHit()
        {
            if (!E.IsReady() || !config.IsBool("useEHit") || Player.ManaPercents() < 40)
                return;

            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValidTarget()))
            {
                if (BallManager.Ball.Status == BallManager.Ballstatus.Land || BallManager.Ball.Status == BallManager.Ballstatus.Ally)
                {
                    var prediction = Prediction.GetPrediction(new PredictionInput
                        {
                            Unit = hero,
                            Delay = E.Delay,
                            Radius = E.Width,
                            Speed = E.Speed,
                            From = BallManager.Ball.Position,
                            Range = E.Range,
                            Collision = E.Collision,
                            Type = E.Type,
                            RangeCheckFrom = Player.ServerPosition,
                            Aoe = true,
                        });
                    Object[] obj = VectorPointProjectionOnLineSegment(BallManager.Ball.Position.To2D(), Player.ServerPosition.To2D(), prediction.UnitPosition.To2D());
                    var isOnseg = (bool)obj[2];
                    var pointLine = (Vector2)obj[1];

                    if (E.IsReady() && isOnseg && prediction.UnitPosition.Distance(pointLine.To3D()) < E.Width)
                    {
                        E.CastOnUnit(Player);
                        return;
                    }
                }
            }
        }

        static Object[] VectorPointProjectionOnLineSegment(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float cx = v3.X;
            float cy = v3.Y;
            float ax = v1.X;
            float ay = v1.Y;
            float bx = v2.X;
            float by = v2.Y;
            float rL = ((cx - ax) * (bx - ax) + (cy - ay) * (by - ay)) /
                       ((float)Math.Pow(bx - ax, 2) + (float)Math.Pow(by - ay, 2));
            var pointLine = new Vector2(ax + rL * (bx - ax), ay + rL * (by - ay));
            float rS;
            if (rL < 0)
            {
                rS = 0;
            }
            else if (rL > 1)
            {
                rS = 1;
            }
            else
            {
                rS = rL;
            }
            bool isOnSegment = rS.CompareTo(rL) == 0;
            Vector2 pointSegment = isOnSegment ? pointLine : new Vector2(ax + rS * (bx - ax), ay + rS * (@by - ay));
            return new object[] { pointSegment, pointLine, isOnSegment };
        }

        #endregion

        #region R

        static void CastR()
        {
            if (BallManager.Ball.IsMoving)
                return;

            var ReqCount = config.GetValue("RminNoEnemies");

            if (HeroManager.Enemies
                .Where(x => 
                    x.IsValidTarget() && 
                    BallManager.Ball.Position.Distance(x.ServerPosition) < R.Width).Count() >= ReqCount)
            {
                R.Cast();
            }
        }

        static void AutoR()
        {
            if (BallManager.Ball.IsMoving)
                return;

            var ReqCount = config.GetValue("AutoRminNoEnemies");

            if (HeroManager.Enemies
                .Where(x =>
                    x.IsValidTarget() &&
                    BallManager.Ball.Position.Distance(x.ServerPosition) < R.Width).Count() >= ReqCount)
            {
                R.Cast();
            }
        }

        static void ManualR(AIHeroClient target)
        {
            if (BallManager.Ball.IsMoving)
                return;

            if (BallManager.Ball.Position.Distance(target.ServerPosition) < R.Width)
            {
                R.Cast();
            }
        }

        #endregion

        #region Others

        static float GetComboDamage(AIHeroClient target)
        {
            var result = 0f;
            if (Q.IsReady())
            {
                result += 2 * Q.GetDamage(target);
            }

            if (W.IsReady())
            {
                result += W.GetDamage(target);
            }

            if (R.IsReady())
            {
                result += R.GetDamage(target);
            }

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                result += (float)ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            result += 2 * (float)Player.GetAutoAttackDamage(target);

            return result;
        }

        static bool Debug()
        {
            return config.IsBool("DebugMode");
        }

        static void Debug(string s)
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
            if (Player.IsDead)
                return;

            ELineCheckHit();
            if (config.IsActive("ComboActive"))
            {
                Combo();
            }
            if (config.IsActive("BurstActive"))
            {
                BurstCombo();
            }
            if ((config.IsActive("HarassActive") || config.IsActive("HarassToggle")) && config.GetValue("HMana") < Player.ManaPercents())
            {
                Harass();
            }
            if ((config.IsActive("LaneClearActive") || config.IsActive("LastHitActive")) && config.GetValue("FMana") < Player.ManaPercents())
            {
                Farm();
            }
            if (config.IsBool("AutoW") && E.IsReady() && W.IsReady())
            {
                GetEW();
            }
            if (config.IsBool("AutoW") && W.IsReady())
            {
                AutoW();
            }
            if (config.IsBool("AutoR") && R.IsReady())
            {
                AutoR();
            }
        }

        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.IsDead || !config.IsBool("useInitiator") || !sender.IsAlly)
                return;

            var a = from x in Initiator.InitiatorList
                    where x.Spelldata.ToLower() == args.SData.Name.ToLower() && config.IsBool(x.Spell)
                    select x;

            if (a.Count() > 0)
            {
                if (E.IsReady() && Player.Distance(sender.ServerPosition) < E.Range)
                {
                    E.CastOnUnit(sender);
                }
            }
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!config.IsBool("Inter-UseR") || !R.IsReady())
                return;

            if (sender.IsEnemy && args.DangerLevel == Interrupter2.DangerLevel.High)
            {
                if (BallManager.Ball.Position.Distance(sender.ServerPosition) < R.Range)
                {
                    R.Cast();
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (config.IsBool("DrawBall"))
            {
                if (BallManager.Ball.Status != BallManager.Ballstatus.Me)
                {
                    Render.Circle.DrawCircle(BallManager.Ball.Position, 100, System.Drawing.Color.Yellow);
                }
            }

            var QCircle = config.Item("Qcircle", true).GetValue<Circle>();

            if (QCircle.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, QCircle.Color);
            }
        }

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (Player.IsDead || !R.IsReady() || !config.IsBool("BlockR"))
                return;

            var count = LeagueSharp.Common.Utility.CountEnemiesInRange(BallManager.Ball.Position, R.Width - 15);
            if (args.Slot == SpellSlot.R && count < 1)
            {
                args.Process = false;
            }
        }

        #endregion
    }
}

