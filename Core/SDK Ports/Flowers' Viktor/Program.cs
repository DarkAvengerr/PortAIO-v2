using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Viktor
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Linq;

    class Program
    {
        private static AIHeroClient Me, Target = null;
        private static Spell Q, W, E, E2, R;
        private static SpellSlot Ignite = SpellSlot.Unknown;
        private static Menu Menu;
        private static int tick = 0;
        private static HpBarDraw DrawHpBar = new HpBarDraw();

        public static void Main()
        {
            Bootstrap.Init();
            Load();
        }

        private static void Load()
        {
            if (GameObjects.Player.ChampionName != "Viktor")
                return;

            Me = GameObjects.Player;

            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 525f);
            E2 = new Spell(SpellSlot.E, 525f + 700f);
            R = new Spell(SpellSlot.R, 700f);

            W.SetSkillshot(0.5f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E2.SetSkillshot(0.25f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 450f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Q.DamageType = W.DamageType = E.DamageType = E2.DamageType = R.DamageType = DamageType.Magical;
            W.MinHitChance = E.MinHitChance = E2.MinHitChance = R.MinHitChance = HitChance.High;

            Ignite = Me.GetSpellSlot("SummonerDot");

            Chat.Print(Me.ChampionName + " : This is Old Version and i dont update it anymore, Please Use Flowers' Series!");

            Menu = new Menu("NightMoon", "Flowers' Viktor", true).Attach();

            Menu.Add(new MenuSeparator("OLD", "This Is Old Version and i dont update it"));
            Menu.Add(new MenuSeparator("OLD1", "Please Use Flowers' Series"));

            ComboMenu = Menu.Add(new Menu("Combo", "Combo"));
            ComboQ = ComboMenu.Add(new MenuBool("ComboQ", "Use Q", true));
            ComboW = ComboMenu.Add(new MenuBool("ComboW", "Use W", true));
            ComboWSmart = ComboMenu.Add(new MenuBool("ComboWSmart", "Solo W Cast", false));
            ComboWMin = ComboMenu.Add(new MenuSlider("ComboWMin", "Min Enemies to Cast W", 2, 1, 5));
            ComboE = ComboMenu.Add(new MenuBool("ComboE", "Use E", true));
            ComboR = ComboMenu.Add(new MenuBool("ComboR", "Use R", true));
            ComboROnlyKill = ComboMenu.Add(new MenuBool("ComboROnlyKill", "Only Use R In Can Kill Enemy(1v1)", true));
            ComboRCounts = ComboMenu.Add(new MenuSlider("ComboRCounts", "Or Counts Enemies >= (6 is off)", 3, 2, 6));
            ComboIgnite = ComboMenu.Add(new MenuBool("ComboIgnite", "Use Ignite", true));

            HarassMenu = Menu.Add(new Menu("Harass", "Harass"));
            HarassQ = HarassMenu.Add(new MenuBool("HarassQ", "Use Q", true));
            HarassE = HarassMenu.Add(new MenuBool("HarassE", "Use E", true));
            HarassMana = HarassMenu.Add(new MenuSlider("HarassMana", "Min Harass ManaPercent", 40));

            LaneClearMenu = Menu.Add(new Menu("LaneClear", "Lane Clear"));
            LaneClearQ = LaneClearMenu.Add(new MenuBool("LaneClearQ", "Use Q", true));
            LaneClearE = LaneClearMenu.Add(new MenuBool("LaneClearE", "Use E", true));
            LaneClearEMin = LaneClearMenu.Add(new MenuSlider("LaneClearEMin", "Use E Clear Min", 3, 1, 7));
            LaneClearMana = LaneClearMenu.Add(new MenuSlider("LaneClearMana", "Min LaneClear ManaPercent", 40));

            JungleClearMenu = Menu.Add(new Menu("JungleClear", "Jungle Clear"));
            JungleClearQ = JungleClearMenu.Add(new MenuBool("JungleClearQ", "Use Q", true));
            JungleClearE = JungleClearMenu.Add(new MenuBool("JungleClearE", "Use E", true));
            JungleClearMana = JungleClearMenu.Add(new MenuSlider("JungleClearMana", "Min JungleClear ManaPercent", 40));

            KillStealMenu = Menu.Add(new Menu("Kill Steal", "Kill Steal"));
            KSQ = KillStealMenu.Add(new MenuBool("KSQ", "Use Q", true));
            KSE = KillStealMenu.Add(new MenuBool("KSE", "Use E", true));


            var FleeMenu = Menu.Add(new Menu("Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("Q", "Use Q", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }


            MiscMenu = Menu.Add(new Menu("Misc", "Misc"));
            GapcloserW = MiscMenu.Add(new MenuBool("AutoWInMePos", "Use W | Anti Gapcloser", true));
            InterruptW = MiscMenu.Add(new MenuBool("SmartW", "Use W | Interrupt", true));
            AutoFollowR = MiscMenu.Add(new MenuBool("AutoFollowR", "Auto R Follow", true));

            DrawMenu = Menu.Add(new Menu("Drawings", "Drawings"));
            DrawQ = DrawMenu.Add(new MenuBool("DrawQ", "Draw Q"));
            DrawW = DrawMenu.Add(new MenuBool("DrawW", "Draw W"));
            DrawE = DrawMenu.Add(new MenuBool("DrawE", "Draw E"));
            DrawEMax = DrawMenu.Add(new MenuBool("DrawEMax", "Draw E Max Range"));
            DrawR = DrawMenu.Add(new MenuBool("DrawR", "Draw R"));
            DrawDamage = DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            DrawTarget = DrawMenu.Add(new MenuBool("DrawTarget", "Draw Target", true));

            Game.OnUpdate += OnUpdate;
            Events.OnGapCloser += OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        #region 
        private static Menu ComboMenu;
        private static Menu HarassMenu;
        private static Menu LaneClearMenu;
        private static Menu JungleClearMenu;
        private static Menu KillStealMenu;
        private static Menu MiscMenu;
        private static Menu DrawMenu;
        private static MenuBool ComboQ;
        private static MenuBool ComboW;
        private static MenuBool ComboWSmart;
        private static MenuBool ComboE;
        private static MenuBool ComboR;
        private static MenuBool ComboROnlyKill;
        private static MenuBool ComboIgnite;
        private static MenuBool HarassQ;
        private static MenuBool HarassE;
        private static MenuBool LaneClearQ;
        private static MenuBool LaneClearE;
        private static MenuBool JungleClearQ;
        private static MenuBool JungleClearE;
        private static MenuBool AutoFollowR;
        private static MenuBool GapcloserW;
        private static MenuBool InterruptW;
        private static MenuBool KSQ;
        private static MenuBool KSE;
        private static MenuBool DrawQ;
        private static MenuBool DrawW;
        private static MenuBool DrawE;
        private static MenuBool DrawEMax;
        private static MenuBool DrawR;
        private static MenuBool DrawDamage;
        private static MenuBool DrawTarget;
        private static MenuSlider ComboRCounts;
        private static MenuSlider LaneClearEMin;
        private static MenuSlider ComboWMin;
        private static MenuSlider HarassMana;
        private static MenuSlider LaneClearMana;
        private static MenuSlider JungleClearMana;
        #endregion

        private static void OnDraw(EventArgs args)
        {
            if (DrawQ.Value && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.SkyBlue);

            if (DrawW.Value && W.IsReady())
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.YellowGreen);

            if (DrawE.Value)
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.PaleGoldenrod);

            if (DrawEMax.Value)
                Render.Circle.DrawCircle(Me.Position, E2.Range, System.Drawing.Color.Red);

            if (DrawR.Value && R.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.LightSkyBlue);

            if (DrawTarget.Value && (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo || Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid))
            {
                if (Target != null)
                {
                    Render.Circle.DrawCircle(Target.Position, Target.BoundingRadius, System.Drawing.Color.Red, 2);
                }
            }
        }

        private static float GetComboDamage(AIHeroClient e)
        {
            float Damage = 0f;

            if (Q.IsReady())
                Damage += Q.GetDamage(e);

            if (E.IsReady())
                Damage += E.GetDamage(e);

            if (R.IsReady())
                Damage += R.GetDamage(e);

            return Damage;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy &&  W.IsReady() && sender.IsValidTarget(W.Range))
            {
                if (CastWTargetPos(args.SData.Name))
                {
                    W.Cast(sender.ServerPosition);
                }
                else if (CastWMePos(args.SData.Name))
                {
                    if (args.End.Distance(Me.Position) <= 100)
                        W.Cast(Me);
                }
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs e)
        {
            if (GapcloserW.Value && W.IsReady())
            {
                if (Me.Distance(e.End) <= 170)
                    W.Cast(Me);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
                return;

            if (AutoFollowR.Value)
                FollowLogic();

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                Combo();

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
                Harass();

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
            {
                LaneClear();
                JungleClear();
            }

            KillSteal();

            if (Menu["Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                Variables.Orbwalker.Move(Game.CursorPos);

                if (Q.IsReady() && Menu["Flee"]["Q"])
                {
                    var target = Variables.TargetSelector.GetTarget(Q);
                    var minion = GameObjects.Minions.Where(x => !x.IsAlly && x.IsValidTarget(Q.Range)).FirstOrDefault();
                    var mob = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range)).FirstOrDefault();

                    if (target != null && target.IsHPBarRendered)
                    {
                        Q.CastOnUnit(target);
                        return;
                    }
                    else if (minion != null)
                    {
                        Q.CastOnUnit(minion);
                        return;
                    }
                    else if (mob != null)
                    {
                        Q.CastOnUnit(mob);
                        return;
                    }
                }
            }

            if (Variables.Orbwalker.ActiveMode != OrbwalkingMode.Combo && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Hybrid)
            {
                Target = null;
            }
        }

        private static void KillSteal()
        {
            foreach (var e in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(1200) && !e.IsDead && !e.IsZombie))
            {
                if(e != null)
                {
                    if(E.IsReady() && KSE.Value && e.Health < E.GetDamage(e))
                    {
                        CastE(e);
                    }
                    else if(Q.IsReady() && e.IsValidTarget(Q.Range) && e.Health < Q.GetDamage(e))
                    {
                        CastQ(e);
                    }
                }
            }
        }

        private static void FollowLogic()
        {
            if(Me.HasBuff("ViktorChaosStormTimer"))
            {
                if(Variables.TickCount >= tick + 500)
                {
                    var e = Variables.TargetSelector.GetTarget(2000, DamageType.Magical);
                    
                    if(e != null && e.IsValid && e.IsVisible && e.IsHPBarRendered)
                    {
                        R.Cast(e.ServerPosition);
                        tick = Variables.TickCount;
                    }
                    else if(e == null)
                    {
                        R.Cast(Me.ServerPosition);
                        tick = Variables.TickCount;
                    }
                }
            }
        }

        private static void JungleClear()
        {
            if (Me.ManaPercent > JungleClearMana.Value)
            {
                var MOB = GameObjects.Jungle.Where(m => m.IsValidTarget(E.Range) && !GameObjects.JungleSmall.Contains(m)).ToList();

                if (MOB != null)
                {
                    foreach (var min in MOB)
                    {
                        if (E.IsReady() && JungleClearE.Value)
                        {
                            var EPos = E.GetLineFarmLocation(MOB, E.Width).Position;

                            if (E.IsReady())
                            {
                                E.Cast(min.Position.ToVector2(), EPos);
                                return;
                            }
                        }

                        if (Q.IsReady() && JungleClearQ.Value)
                        {
                            Q.Cast(min);
                            return;
                        }
                    }

                }
            }
        }

        private static void LaneClear()
        {
            if(Me.ManaPercent > LaneClearMana.Value)
            {
                var mins = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(E2.Range)).ToList();

                if (mins != null)
                {
                    foreach (var min in mins)
                    {
                        if (E.IsReady() && LaneClearE.Value)
                        {
                            var EFarm = E.GetLineFarmLocation(mins, E.Width);

                            if (EFarm.MinionsHit >= LaneClearEMin)
                            {
                                E.Cast(min.ServerPosition.ToVector2(), EFarm.Position);
                                return;
                            }
                        }

                        if (LaneClearQ.Value && Q.IsReady())
                        {
                            Q.Cast(min);
                            return;
                        }
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent > HarassMana)
            {
                var target = Variables.TargetSelector.GetTarget(E2.Range, DamageType.Magical);

                if (target != null && target.IsHPBarRendered)
                {
                    Target = target;

                    if (E.IsReady() && HarassE.Value)
                        CastE(target);

                    if (Q.IsReady() && HarassQ.Value)
                        DelayAction.Add(200, () => {
                            CastQ(target);
                        });
                }
            }
        }

        private static void Combo()
        {
            var target = Variables.TargetSelector.GetTarget(1200, DamageType.Magical);

            if (target != null && target.IsHPBarRendered)
            {
                Target = target;

                if (E.IsReady() && ComboE.Value)
                    CastE(target);

                if (Q.IsReady() && ComboQ.Value)
                    CastQ(target);

                if (W.IsReady() && ComboW.Value)
                    CastW(target);

                if (R.IsReady() && ComboR.Value)
                    CastR(target);

                if (Ignite.IsReady() && ComboIgnite.Value)
                    CastIgnite(target);
            }
        }

        private static void CastIgnite(AIHeroClient e)
        {
            if (e != null && !e.IsZombie)
            {
                if ((e.Health < GetIgniteDamage(e)) || (e.Health > GetReadyComboDamage(e) && e.Health <= (GetReadyComboDamage(e) + GetIgniteDamage(e))))
                {
                    Me.Spellbook.CastSpell(Ignite, e);
                }
            }
        }

        private static bool CastQ(AIHeroClient target)
        {
            if (target != null && target.IsHPBarRendered && ComboQ.Value && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
                return true;
            }

            return false;
        }

        private static bool CastW(AIHeroClient target)
        {
            if (target != null && target.IsValidTarget(W.Range) && W.IsReady())
            {
                if (ComboWSmart.Value)
                {
                    if (Me.IsFacing(target))
                    {
                        var WPos = W.GetPrediction(target).CastPosition - 150 * (W.GetPrediction(target).UnitPosition - target.ServerPosition).Normalized();
                        W.Cast(WPos);
                        return true;
                    }
                    else if (!Me.IsFacing(target))
                    {
                        var WPos = W.GetPrediction(target).CastPosition + 150 * (W.GetPrediction(target).UnitPosition - target.ServerPosition).Normalized();
                        W.Cast(WPos);
                        return true;
                    }
                }

                if (target.CountEnemyHeroesInRange(W.Range) >= ComboWMin.Value)
                {
                    W.Cast(target);
                    return true;
                }

                return false;
            }
            return false;
        }

        private static bool CastE(AIHeroClient target)
        {
            if (target != null && target.IsHPBarRendered)
            {
                var EInput = new PredictionInput
                {
                    Unit = target,
                    Delay = E.Delay,
                    Radius = E.Width,
                    Range = E.Range,
                    Speed = E.Speed,
                    Type = E.Type,
                    UseBoundingRadius = true
                };

                var E2Input = new PredictionInput
                {
                    Unit = target,
                    Delay = E.Delay,
                    Radius = E.Width,
                    Range = E2.Range,
                    Speed = E.Speed,
                    Type = E.Type,
                    UseBoundingRadius = true
                };

                var FromPos = Vector3.Zero;
                var ToPos = Vector3.Zero;

                if (target.IsValidTarget(E.Range))
                {
                    var MovePred = Movement.GetPrediction(EInput);

                    FromPos = target.ServerPosition;
                    ToPos = MovePred.CastPosition;

                    if (MovePred.Hitchance >= HitChance.VeryHigh && FromPos != Vector3.Zero && ToPos != Vector3.Zero)
                    {
                        E.Cast(FromPos, ToPos);
                        return true;
                    }
                }
                else if (target.IsValidTarget(E2.Range) && !target.IsValidTarget(E.Range))
                {
                    var MovePred = Movement.GetPrediction(E2Input);
                    FromPos = Me.ServerPosition + Geometry.Normalized(target.ServerPosition - Me.ServerPosition) * E.Range;
                    ToPos = MovePred.CastPosition;

                    if (MovePred.Hitchance >= HitChance.VeryHigh && FromPos != Vector3.Zero && ToPos != Vector3.Zero)
                    {
                        E.Cast(FromPos, ToPos);
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        private static void CastR(AIHeroClient e)
        {
            if (e != null && !e.IsZombie && R.IsReady() && !Me.HasBuff("ViktorChaosStormTimer"))
            {
                var GetDamage = GetReadyComboDamage(e);

                if (e.HealthPercent > 5)
                {
                    if (ComboROnlyKill)
                    {
                        if (e.Health < GetDamage)
                        {
                            R.Cast(e.ServerPosition);
                        }
                    }
                    else if (!ComboROnlyKill && e.IsValidTarget(600))
                    {
                        R.Cast(e.ServerPosition);
                    }
                }

                if (Me.CountEnemyHeroesInRange(1200) >= ComboRCounts.Value)
                {
                    var couts = GameObjects.EnemyHeroes.OrderBy(x => x.Health - GetReadyComboDamage(x)).Where(x => x.IsValidTarget(1200) && !x.IsDead && !x.IsZombie);

                    foreach (var cast in couts)
                    {
                        R.Cast(cast);
                    }
                }
            }
        }

        private static bool CastWMePos(string SpellName)
        {
            if (SpellName == "AatroxQ")
                return true;

            if (SpellName == "AkaliShadowDance")
                return true;

            if (SpellName == "Headbutt")
                return true;

            if (SpellName == "DianaTeleport")
                return true;

            if (SpellName == "AlZaharNetherGrasp")
                return true;

            if (SpellName == "JaxLeapStrike")
                return true;

            if (SpellName == "KatarinaE")
                return true;

            if (SpellName == "KhazixE")
                return true;

            if (SpellName == "LeonaZenithBlade")
                return true;

            if (SpellName == "MaokaiTrunkLine")
                return true;

            if (SpellName == "MonkeyKingNimbus")
                return true;

            if (SpellName == "PantheonW")
                return true;

            if (SpellName == "PoppyHeroicCharge")
                return true;

            if (SpellName == "ShenShadowDash")
                return true;

            if (SpellName == "SejuaniArcticAssault")
                return true;

            if (SpellName == "RenektonSliceAndDice")
                return true;

            if (SpellName == "Slash")
                return true;

            if (SpellName == "XenZhaoSweep")
                return true;

            if (SpellName == "RocketJump")
                return true;

            return false;
        }

        private static bool CastWTargetPos(string SpellName)
        {
            if (SpellName == "KatarinaR")
                return true;

            if (SpellName == "GalioIdolOfDurand")
                return true;

            if (SpellName == "GragasE")
                return true;

            if (SpellName == "Crowstorm")
                return true;

            if (SpellName == "BandageToss")
                return true;

            if (SpellName == "LissandraE")
                return true;

            if (SpellName == "AbsoluteZero")
                return true;

            if (SpellName == "AlZaharNetherGrasp")
                return true;

            if (SpellName == "FallenOne")
                return true;

            if (SpellName == "PantheonRJump")
                return true;

            if (SpellName == "CaitlynAceintheHole")
                return true;

            if (SpellName == "MissFortuneBulletTime")
                return true;

            if (SpellName == "InfiniteDuress")
                return true;

            if (SpellName == "ThreshQ")
                return true;

            if (SpellName == "RocketGrab")
                return true;

            return false;
        }

        private static float GetReadyComboDamage(AIHeroClient e)
        {
            float damage = 0f;

            if (!e.IsEnemy || e.IsZombie || e.IsDead)
            {
                return 0f;
            }

            if (Q.IsReady())
            {
                damage += Q.GetDamage(e);

                if (e.InAutoAttackRange())
                {
                    damage += (float)GetQAttackDamage(e);
                }
            }

            if (E.IsReady() && Me.ServerPosition.Distance(e.ServerPosition) <= 1225)
            {
                damage += E.GetDamage(e);
            }

            if (R.IsReady() && R.IsInRange(e))
            {
                damage += R.GetDamage(e);
            }

            return damage;
        }

        private static double GetQAttackDamage(AIHeroClient e)
        {
            double[] AttackDamage = new double[] { 20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210 };

            return AttackDamage[Me.Level - 1] + Me.TotalMagicalDamage * 0.5 + Me.TotalAttackDamage;
        }

        private static double GetIgniteDamage(AIHeroClient e)
        {
            return 50 + 20 + Me.Level - (e.HPRegenRate / 5 * 3);
        }
    }
}
