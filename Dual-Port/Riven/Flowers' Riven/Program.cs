using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Linq;
using Color = System.Drawing.Color;

/*
Update log:
    First Push (2016-02-17 03.38)
    Fix LaneClear & JungleClear WE Use Question (2016-02-17 04.37)
    Add Keep Q alive Logic (2016-02-17 07.03)
    Delect about Q Range Draw bool and fix burst status show (2016-02-17 07.10)
    Fix burst logic (2016-02-17 08.40)
    Add KillSteal Logic(QKillSteal WKillSteal R2KillSteal E+R2KillSteal) (2016-02-17 09.52)
    Fix Error (2016-02-17 10.02)
    Not Use Dance in QA (2016-02-17 20.18)
    Add Quick Harass Logic (Not Done) (2016-02-17 22.36)
    Change Name (Flowers Riven (Text Version) => Flowers Riven) (2016-02-17 22.38)
    Fix Orbwalker Not Move Problem (2016-02-17 23.01)
    Fix Burst Status Show Problem (now can sure to show Can Use Flash to burst) (2016-02-28 02.02)
    Add ComboE UseMode (To Target or To Mouse  . default Is To Target) (2016-02-28 12.28)
    Fix Burst R1 question and Not Orbwalker If you Enable Dance not enable.. (2016-02-28 15.39)
    Fix QuickHarass Logic (Q1A -> WA -> Q2A -> E + Q3 Back), and Fix LaneClear % JungleClear Q Logic (2016-02-29 00.49)
    Fix JungleClear E Logic (Now Will control your passive) (2016-02-29 01.35)
    Add R2 Mode(First Cast R2) and Change "NightMoon Logic" -> "Max Damage" (2016-02-29 15.01)
    Fix QuickHarass Mode EQ3 back Logic (2016-02-29 15.01)
    Fix JungleClear Q Cast Problem (2016-02-29 15.01)
    Add Burst Range Draw and Quickharass Range( == Burst Min Range)  (2016-02-29 15.01)
    Fix Damage Including (2016-03-01 21.43)
    Clean Menu, Update QALogic(Faster), Add Quickharass Range Show, Add R1 Status, Add WallJump Logic (2016-03-03 03.41)
    Add R1 Color Draw (2016-03-04 20.53)
    Fix R2 Cast Logic (2016-03-04 23.14)
Next Work:
    Now Perfect ! If you find more bugs plz give me feedback! (if all is working i will make a SDK Riven assembly)
*/

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Riven
{
    class Program
    {
        public static bool DisableAttack;
        public static Spell Q, W, E, R;
        public static SpellSlot Ignite, Flash;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Menu;
        public static AIHeroClient Me;
        public static HpBarDraw DrawHpBar = new HpBarDraw();
        public static int QStack;
        public static bool CastR2, CanFlash;
        private static Vector3 FleePosition = Vector3.Zero;
        private static Vector3 TargetPosition = Vector3.Zero;
        private static int InitTime { get; set; }

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Riven")
                return;

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 270f);
            E = new Spell(SpellSlot.E, 312f);
            R = new Spell(SpellSlot.R, 900f) { MinHitChance = HitChance.High };
            R.SetSkillshot(0.25f, 45f, 1600f, false, SkillshotType.SkillshotCone);
            Ignite = Me.GetSpellSlot("SummonerDot");
            Flash = Me.GetSpellSlot("SummonerFlash");

            Menu = new Menu("Flowers-Riven", "NightMoon", true);

            Menu.AddSubMenu(new Menu("[FL] Orbwalking", "nightmoon.Orbwalker.Menu"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("nightmoon.Orbwalker.Menu"));

            Menu.AddSubMenu(new Menu("[FL] Q Setting", "nightmoon.Q.Menu"));
            Menu.SubMenu("nightmoon.Q.Menu").AddSubMenu(new Menu("QA Setting", "nightmoon.QA.Menu"));
            Menu.SubMenu("nightmoon.Q.Menu").SubMenu("nightmoon.QA.Menu").AddItem(new MenuItem("Dance", "Use Dance in QA", true).SetValue(false));
            Menu.SubMenu("nightmoon.Q.Menu").SubMenu("nightmoon.QA.Menu").AddItem(new MenuItem("DC", "Dance Delay", true).SetValue(new Slider(100, 0, 200)));
            Menu.SubMenu("nightmoon.Q.Menu").AddItem(new MenuItem("ComboQ", "Combo: Always Use", true));
            Menu.SubMenu("nightmoon.Q.Menu").AddItem(new MenuItem("HarassQ", "Harass: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.Q.Menu").AddItem(new MenuItem("LaneClearQ", "LaneClear: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.Q.Menu").AddItem(new MenuItem("JungleClearQ", "JungleClear: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.Q.Menu").AddItem(new MenuItem("KillStealQ", "KillSteal: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.Q.Menu").AddItem(new MenuItem("KeepQALive", "Keep Q alive", true).SetValue(true));

            Menu.AddSubMenu(new Menu("[FL] W Setting", "nightmoon.W.Menu"));
            Menu.SubMenu("nightmoon.W.Menu").AddItem(new MenuItem("ComboW", "Combo: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.W.Menu").AddItem(new MenuItem("HarassW", "Harass: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.W.Menu").AddItem(new MenuItem("LaneClearW", "LaneClear: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.W.Menu").AddItem(new MenuItem("JungleClearW", "JungleClear: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.W.Menu").AddItem(new MenuItem("AntiGapCloserW", "AntiGapCloser: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.W.Menu").AddItem(new MenuItem("InterruptTargetW", "Interrupt: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.W.Menu").AddItem(new MenuItem("KillStealW", "KillSteal: Use Spells", true).SetValue(true));

            Menu.AddSubMenu(new Menu("[FL] E Setting", "nightmoon.E.Menu"));
            Menu.SubMenu("nightmoon.E.Menu").AddItem(new MenuItem("ComboE", "Combo: Use Mode", true).SetValue(new StringList(new string[] { "To Target", "To Mouse", "Off"})));
            Menu.SubMenu("nightmoon.E.Menu").AddItem(new MenuItem("JungleClearE", "Harass: Use Spells", true).SetValue(true));
            Menu.SubMenu("nightmoon.E.Menu").AddItem(new MenuItem("KillStealE", "KillSteal: Use Spells", true).SetValue(true));

            Menu.AddSubMenu(new Menu("[FL] Ult Setting", "nightmoon.R.Menu"));
            Menu.SubMenu("nightmoon.R.Menu").AddItem(new MenuItem("ComboR", "Combo: Use R", true).SetValue(true));
            Menu.SubMenu("nightmoon.R.Menu").AddItem(new MenuItem("R1Combo", "Combo: Use R1", true).SetValue(new KeyBind('L', KeyBindType.Toggle, true)));
            Menu.SubMenu("nightmoon.R.Menu").AddItem(new MenuItem("R2Mode", "Combo: R2 Mode", true).SetValue(new StringList(new string[] { "Killable", "Max Damage", "First Cast" , "Off" }, 1)));
            Menu.SubMenu("nightmoon.R.Menu").AddItem(new MenuItem("KillStealR", "KillSteal: Use Spells", true).SetValue(true));

            Menu.AddSubMenu(new Menu("[FL] Burst Mode", "nightmoon.Burst.Menu"));
            Menu.SubMenu("nightmoon.Burst.Menu").AddItem(new MenuItem("BurstFlash", "Burst: Use Flash", true).SetValue(true));
            Menu.SubMenu("nightmoon.Burst.Menu").AddItem(new MenuItem("BurstIgnite", "Burst: Use Ignite", true).SetValue(true));
            Menu.SubMenu("nightmoon.Burst.Menu").AddItem(new MenuItem("BurstItem", "Burst: Use Items", true).SetValue(true));

            Menu.AddSubMenu(new Menu("[FL] Support Using", "nightmoon.Item.Menu"));
            Menu.SubMenu("nightmoon.Item.Menu").AddItem(new MenuItem("ComboItem", "Combo: Use Items", true).SetValue(true));
            Menu.SubMenu("nightmoon.Item.Menu").AddItem(new MenuItem("HarassItem", "Harass: Use Items", true).SetValue(true));
            Menu.SubMenu("nightmoon.Item.Menu").AddItem(new MenuItem("LaneClearItem", "LaneClear: Use Items", true).SetValue(true));
            Menu.SubMenu("nightmoon.Item.Menu").AddItem(new MenuItem("JungleClearItem", "JungleClear: Use Items", true).SetValue(true));
            Menu.SubMenu("nightmoon.Item.Menu").AddItem(new MenuItem("Say4", " Tiyamat, Ghostblade, Hydra", true));

            Menu.AddSubMenu(new Menu("[FL] Draw Setting", "nightmoon.Draw.Menu"));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("drawingW", "W Range", true).SetValue(new Circle(false, Color.FromArgb(202, 170, 255))));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("BrustMinRange", "Burst Min Range", true).SetValue(new Circle(true, Color.FromArgb(255, 255, 0))));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("BrustMaxRange", "Burst Max Range", true).SetValue(new Circle(true, Color.FromArgb(255, 0, 0))));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("QuickHarassRange", "Quick Harass Range", true).SetValue(new Circle(true, Color.FromArgb(255, 255, 0))));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("DrawDamage", "Draw Combo Damage", true).SetValue(true));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("ShowR1", "Show R1 Status", true).SetValue(true));
            Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("ShowBurst", "Show Burst Status", true).SetValue(true));

            Menu.AddItem(new MenuItem("Credit", "Credit : NightMoon"));
            Menu.AddToMainMenu();

            Chat.Print("<font color='#2848c9'>Flowers Riven</font> --> <font color='#b756c5'>Version : 1.0.0.2</font> <font size='30'><font color='#d949d4'>Good Luck!</font></font>");

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Orbwalking.BeforeAttack += ProcessBool;
            Orbwalking.AfterAttack += JungleClearELogic;
            Obj_AI_Base.OnPlayAnimation += LightQALogic;
            Obj_AI_Base.OnSpellCast += ItemCastLogic;
            Obj_AI_Base.OnSpellCast += AfterAAQLogic;
        }

        #region
        public static bool KeepQALive => Menu.Item("KeepQALive", true).GetValue<bool>();
        public static bool ShowBurst => Menu.Item("ShowBurst", true).GetValue<bool>();
        public static bool BurstFlash => Menu.Item("BurstFlash", true).GetValue<bool>();
        public static bool BurstIgnite => Menu.Item("BurstIgnite", true).GetValue<bool>();
        public static bool BurstItem => Menu.Item("BurstItem", true).GetValue<bool>();
        public static bool Dance => Menu.Item("Dance", true).GetValue<bool>();
        public static int DanceDelay => Menu.Item("DC", true).GetValue<Slider>().Value;
        public static bool HarassQ => Menu.Item("HarassQ", true).GetValue<bool>();
        public static bool LaneClearQ => Menu.Item("LaneClearQ", true).GetValue<bool>();
        public static bool JungleClearQ => Menu.Item("JungleClearQ", true).GetValue<bool>();
        public static bool ComboW => Menu.Item("ComboW", true).GetValue<bool>();
        public static bool KillStealQ => Menu.Item("KillStealQ", true).GetValue<bool>();
        public static bool KillStealW => Menu.Item("KillStealW", true).GetValue<bool>();
        public static bool KillStealE => Menu.Item("KillStealE", true).GetValue<bool>();
        public static bool KillStealR => Menu.Item("KillStealR", true).GetValue<bool>();
        public static bool HarassW => Menu.Item("HarassW", true).GetValue<bool>();
        public static bool LaneClearW => Menu.Item("LaneClearW", true).GetValue<bool>();
        public static bool JungleClearW => Menu.Item("JungleClearW", true).GetValue<bool>();
        public static bool AntiGapCloserW => Menu.Item("AntiGapCloserW", true).GetValue<bool>();
        public static bool InterruptTargetW => Menu.Item("InterruptTargetW", true).GetValue<bool>();
        public static int ComboE => Menu.Item("ComboE", true).GetValue<StringList>().SelectedIndex;
        public static bool JungleClearE => Menu.Item("JungleClearE", true).GetValue<bool>();
        public static bool ComboR => Menu.Item("ComboR", true).GetValue<bool>();
        public static bool R1Combo => Menu.Item("R1Combo", true).GetValue<KeyBind>().Active;
        public static int R2Mode => Menu.Item("R2Mode", true).GetValue<StringList>().SelectedIndex;
        public static bool ComboItem => Menu.Item("ComboItem", true).GetValue<bool>();
        public static bool HarassItem => Menu.Item("HarassItem", true).GetValue<bool>();
        public static bool LaneClearItem => Menu.Item("LaneClearItem", true).GetValue<bool>();
        public static bool JungleClearItem => Menu.Item("JungleClearItem", true).GetValue<bool>();
        public static bool DrawDamage => Menu.Item("DrawDamage", true).GetValue<bool>();
        public static bool ShowR1 => Menu.Item("ShowR1", true).GetValue<bool>();
        public static Circle drawingW => Menu.Item("drawingW", true).GetValue<Circle>();
        public static Circle BrustMaxRange => Menu.Item("BrustMaxRange", true).GetValue<Circle>();
        public static Circle BrustMinRange => Menu.Item("BrustMinRange", true).GetValue<Circle>();
        public static Circle QuickHarassRange => Menu.Item("QuickHarassRange", true).GetValue<Circle>();
        #endregion

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
                return;

            if (drawingW.Active)
            {
                if (W.IsReady())
                    Render.Circle.DrawCircle(Me.Position, W.Range, drawingW.Color);
            }

            if (BrustMaxRange.Active && Me.Level >= 6 && R.IsReady())
            {
                if (E.IsReady() && Flash.IsReady())
                    Render.Circle.DrawCircle(Me.Position, 465 + E.Range, BrustMaxRange.Color);
            }

            if(BrustMinRange.Active && Me.Level >= 6 && R.IsReady())
            {
                if (E.IsReady() && Flash.IsReady())
                    Render.Circle.DrawCircle(Me.Position, E.Range + Me.BoundingRadius, BrustMinRange.Color);
            }

            if (DrawDamage)
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsZombie))
                {
                    DrawHpBar.Unit = e;
                    DrawHpBar.DrawDmg((float)GetComboDamage(e), new ColorBGRA(255, 204, 0, 170));
                }
            }

            if(QuickHarassRange.Active)
            {
                Render.Circle.DrawCircle(Me.Position, E.Range + Me.BoundingRadius, QuickHarassRange.Color);
            }

            if(ShowR1)
            {
                var text = "";
                if (R1Combo)
                    text = "Enable";
                if (!R1Combo)
                    text = "Off";
                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, Color.Red, "Use R1: ");
                Drawing.DrawText(Me.HPBarPosition.X + 90, Me.HPBarPosition.Y - 40, Color.Blue, text);
                Menu.Item("R1Combo", true).Permashow();
            }

            if (ShowBurst)
            {
                foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget()))
                {
                    var text = "";
                    var text2 = "";
                    var Mepos = Drawing.WorldToScreen(Me.Position);
                    var hero = TargetSelector.GetSelectedTarget();

                    if (hero == null)
                    {
                        text = "Lock Target Is Null!";
                    }

                    if (hero != null)
                    {
                        text = "Lock Target is : " + hero.ChampionName;
                        text2 = "Can Flash : " + CanFlash.ToString();
                    }

                    if (BurstFlash && Flash.IsReady() && e.Distance(Me.ServerPosition) <= 800 && e.Distance(Me.ServerPosition) >= E.Range + Me.AttackRange + 85)
                    {
                        CanFlash = true;
                    }
                    else
                    {
                        CanFlash = false;
                    }

                    Drawing.DrawText(Mepos[0] - 20, Mepos[1], Color.Red, text);
                    Drawing.DrawText(Mepos[0] - 20, Mepos[1] + 14, Color.GreenYellow, text2);
                }
            }
        }

        private static double GetComboDamage(AIHeroClient e) 
        {
            //Thanks Asuvril
            double passive = 0;

            if (Me.Level == 18)
                passive = 0.5;
            else if (Me.Level >= 15)
                passive = 0.45;
            else if (Me.Level >= 12)
                passive = 0.4;
            else if (Me.Level >= 9)
                passive = 0.35;
            else if (Me.Level >= 6)
                passive = 0.3;
            else if (Me.Level >= 3)
                passive = 0.25;
            else
                passive = 0.2;
            double damage = 0;

            if (Q.IsReady())
            {
                var qhan = 3 - QStack;
                damage += Q.GetDamage(e) * qhan + Me.GetAutoAttackDamage(e) * qhan * (1 + passive);
            }

            if (W.IsReady())
                damage += W.GetDamage(e);

            if (R.IsReady())
                if (Me.HasBuff("RivenFengShuiEngine"))
                    damage += Me.CalcDamage(e, Damage.DamageType.Physical, (new double[] { 80, 120, 160 }[R.Level - 1] + 0.6 * Me.FlatPhysicalDamageMod) * (1 + (e.MaxHealth - e.Health) / e.MaxHealth > 0.75 ? 0.75 : (e.MaxHealth - e.Health) / e.MaxHealth) * 8 / 3);

            return damage;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboLogic();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassLogic();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClearLogic();
                    JungleClearLogic();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    FleeLogic();
                    break;
                case Orbwalking.OrbwalkingMode.Brust:
                    BurstLogic();
                    break;
                case Orbwalking.OrbwalkingMode.QuickHarass:
                    QuickHarassLogic();
                    break;
                case Orbwalking.OrbwalkingMode.WallJump:
                    WallJumpLogic();
                    break;
            }

            KeelQLogic();
            KillStealLogic();
        }

        private static void KeelQLogic()
        {
            if (KeepQALive && !Me.UnderTurret(true) && !Me.IsRecalling() && Me.HasBuff("RivenTriCleave"))
            {
                if (Me.GetBuff("RivenTriCleave").EndTime - Game.Time < 0.3)
                    Q.Cast(Game.CursorPos);
            }
        }

        private static void WallJumpLogic()
        {
            if (Q.IsReady() && QStack != 2)
            {
                Q.Cast(Game.CursorPos);
            }

            //Thanks Asuvril

            var wallCheck = VectorHelper.GetFirstWallPoint(Me.Position, Game.CursorPos);
            if (wallCheck != null)
            {
                wallCheck = VectorHelper.GetFirstWallPoint((Vector3)wallCheck, Game.CursorPos, 5);
            }

            var movePosition = wallCheck != null ? (Vector3)wallCheck : Game.CursorPos;
            var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);
            FleePosition = NavMesh.GridToWorld((short)tempGrid.X, (short)tempGrid.Y);

            if (wallCheck != null)
            {
                var wallPosition = movePosition;
                var direction = (Game.CursorPos.To2D() - wallPosition.To2D()).Normalized();
                const float maxAngle = 80f;
                const float step = maxAngle / 20;
                var currentAngle = 0f;
                var currentStep = 0f;
                while (true)
                {
                    if (currentStep > maxAngle && currentAngle < 0)
                    {
                        break;
                    }
                    if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                    {
                        currentAngle = (currentStep) * (float)Math.PI / 180;
                        currentStep += step;
                    }
                    else if (currentAngle > 0)
                    {
                        currentAngle = -currentAngle;
                    }

                    Vector3 checkPoint;
                    if (currentStep == 0)
                    {
                        currentStep = step;
                        checkPoint = wallPosition + 300 * direction.To3D();
                    }
                    else
                    {
                        checkPoint = wallPosition + 300 * direction.Rotated(currentAngle).To3D();
                    }
                    if (!checkPoint.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) &&
                        !checkPoint.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building))
                    {
                        wallCheck = VectorHelper.GetFirstWallPoint(checkPoint, wallPosition);
                        if (wallCheck != null)
                        {
                            var wallPositionOpposite = (Vector3)VectorHelper.GetFirstWallPoint((Vector3)wallCheck, wallPosition);

                            if (Math.Sqrt(Me.GetPath(wallPositionOpposite).Sum(o => o.To2D().LengthSquared())) - Me.Distance(wallPositionOpposite) > 200)
                            {
                                if (Me.Distance(wallPositionOpposite, true) < Math.Pow(300 - Me.BoundingRadius / 2, 5) && QStack == 2)
                                {
                                    InitTime = Environment.TickCount;
                                    TargetPosition = wallPositionOpposite;

                                    if(E.IsReady())
                                    {
                                        E.Cast(Game.CursorPos);
                                    }
                                    else if (!E.IsReady())
                                    {
                                        Q.Cast(Game.CursorPos);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void QuickHarassLogic()
        {
            var t = TargetSelector.GetSelectedTarget();

            if (t != null && t.IsValidTarget())
            {
                if (QStack == 2)
                {
                    if (E.IsReady())
                    {
                        E.Cast(Me.ServerPosition + (Me.ServerPosition - t.ServerPosition).Normalized() * E.Range);
                    }

                    if (!E.IsReady())
                    {
                        Q.Cast(Me.ServerPosition + (Me.ServerPosition - t.ServerPosition).Normalized() * E.Range);
                    }
                }

                if(W.IsReady())
                {
                    if (t.IsValidTarget(W.Range) && QStack == 1)
                    {
                        W.Cast();
                    }
                }

                if (Q.IsReady())
                {
                    if(QStack == 0)
                    {
                        if(t.IsValidTarget(Me.AttackRange + Me.BoundingRadius + 150))
                        {
                            Q.Cast(t.Position);
                        }
                    }
                }
            }
        }

        private static void KillStealLogic()
        {
            foreach (var e in HeroManager.Enemies.Where(e => !e.IsZombie && !e.HasBuff("KindredrNoDeathBuff") && !e.HasBuff("Undying Rage") && !e.HasBuff("JudicatorIntervention") && e.IsValidTarget()))
            {
                if (Q.IsReady() && KillStealQ)
                {
                    if (Me.HasBuff("RivenFengShuiEngine"))
                    {
                        if (e.Distance(Me.ServerPosition) < Me.AttackRange + Me.BoundingRadius + 50 && Me.GetSpellDamage(e, SpellSlot.Q) > e.Health + e.HPRegenRate)
                            Q.Cast(e.Position);
                    }
                    else if (!Me.HasBuff("RivenFengShuiEngine"))
                    {
                        if (e.Distance(Me.ServerPosition) < Me.AttackRange + Me.BoundingRadius && Me.GetSpellDamage(e, SpellSlot.Q) > e.Health + e.HPRegenRate)
                            Q.Cast(e.Position);
                    }
                }

                if (W.IsReady() && KillStealW)
                {
                    if (e.IsValidTarget(W.Range) && Me.GetSpellDamage(e, SpellSlot.W) > e.Health + e.HPRegenRate)
                    {
                        W.Cast();
                    }
                }

                if (R.IsReady() && KillStealR)
                {
                    if (Me.HasBuff("RivenWindScarReady"))
                    {
                        if (E.IsReady() && KillStealE)
                        {
                            if (Me.ServerPosition.CountEnemiesInRange(R.Range + E.Range) < 3 && Me.HealthPercent > 50)
                            {
                                if (Me.GetSpellDamage(e, SpellSlot.R) > e.Health + e.HPRegenRate && e.IsValidTarget(R.Range + E.Range - 100))
                                {
                                    if (E.IsReady())
                                    {
                                        E.Cast(e.Position);
                                    }
                                    else if (!E.IsReady())
                                    {
                                        R.CastIfHitchanceEquals(e, HitChance.High, true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Me.GetSpellDamage(e, SpellSlot.R) > e.Health + e.HPRegenRate && e.IsValidTarget(R.Range - 50))
                            {
                                R.CastIfHitchanceEquals(e, HitChance.High, true);
                            }
                        }
                    }
                }
            }
        }

        private static void BurstLogic()
        {
            var e = TargetSelector.GetSelectedTarget();

            if (e != null && !e.IsDead && e.IsValidTarget() && !e.IsZombie)
            {
                if (R.IsReady())
                {
                    if (Me.HasBuff("RivenFengShuiEngine"))
                    {
                        if (Q.IsReady())
                        {
                            if (E.IsReady() && W.IsReady())
                            {
                                if (e.Distance(Me.ServerPosition) < E.Range + Me.AttackRange + 100)
                                {
                                    E.Cast(e.Position);
                                }
                            }
                        }
                    }

                    if (E.IsReady())
                    {
                        if (e.Distance(Me.ServerPosition) < Me.AttackRange + E.Range + 100)
                        {
                            R.Cast();
                            E.Cast(e.Position);
                        }
                    }
                }

                if (W.IsReady())
                {
                    if (HeroManager.Enemies.Any(x => x.IsValidTarget(W.Range)))
                        W.Cast();
                }

                if (QStack == 1 || QStack == 2 || e.HealthPercent < 50)
                {
                    if (Me.HasBuff("RivenWindScarReady"))
                    {
                        R.Cast(e);
                    }
                }

                if (BurstItem)
                {
                    if (e.Distance(Me.ServerPosition) <= E.Range + 500)
                    {
                        if (LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                            LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem().Cast();
                    }

                    if (Orbwalker.InAutoAttackRange(e))
                    {
                        if (LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                            LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().Cast();

                        if (LeagueSharp.Common.Data.ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady())
                            LeagueSharp.Common.Data.ItemData.Titanic_Hydra_Melee_Only.GetItem().Cast();

                        if (LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                            LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
                    }
                }

                if (BurstIgnite)
                {
                    if (e.HealthPercent < 50)
                    {
                        if (Ignite.IsReady())
                        {
                            Me.Spellbook.CastSpell(Ignite, e);
                        }
                    }
                }

                if (BurstFlash)
                {
                    if (Flash.IsReady())
                    {
                        if (R.IsReady() && R.Instance.Name == "RivenFengShuiEngine")
                        {
                            if (E.IsReady() && W.IsReady())
                            {
                                if (e.Distance(Me.ServerPosition) <= 780 && e.Distance(Me.ServerPosition) >= E.Range + Me.AttackRange + 85)
                                {
                                    R.Cast();
                                    E.Cast(e.Position);
                                    LeagueSharp.Common.Utility.DelayAction.Add(150, () => { Me.Spellbook.CastSpell(Flash, e.Position); });
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void FleeLogic()
        {
            var e = HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !enemy.HasBuffOfType(BuffType.SpellShield));

            if (W.IsReady())
                if (e != null)
                    if (e.FirstOrDefault().IsValidTarget(W.Range))
                        W.Cast();

            if (E.IsReady() && !Me.IsDashing())
            {
                E.Cast(Me.Position.Extend(Game.CursorPos, 300));
            }
            else if(Q.IsReady() && !Me.IsDashing())
            {
                Q.Cast(Game.CursorPos);
            }
        }

        private static void LaneClearLogic()
        {
            if (LaneClearW)
            {
                if(W.IsReady())
                {
                    var WMinions = MinionManager.GetMinions(Me.ServerPosition, W.Range);

                    if (WMinions != null)
                        if (WMinions.FirstOrDefault().IsValidTarget(W.Range))
                            if (WMinions.Count >= 3)
                                W.Cast();
                }
            }
        }

        private static void JungleClearLogic()
        {
            var Mob = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (JungleClearW)
            {
                if (Mob != null)
                    if (Mob.FirstOrDefault().IsValidTarget(W.Range))
                        W.Cast();
            }
        }

        private static void JungleClearELogic(AttackableUnit unit, AttackableUnit target)
        {
            if(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if(target is Obj_AI_Minion)
                {
                    if (JungleClearE)
                    {
                        var Mob = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                        if (Mob.FirstOrDefault().IsValidTarget(E.Range))
                        {
                            if (Mob.FirstOrDefault().HasBuffOfType(BuffType.Stun) && !W.IsReady())
                            {
                                E.Cast(Game.CursorPos);
                            }
                            else if (!Mob.FirstOrDefault().HasBuffOfType(BuffType.Stun))
                            {
                                E.Cast(Game.CursorPos);
                            }
                        }
                    }
                }
            }
        }

        private static void HarassLogic()
        {
            if (HarassW)
            {
                var t = HeroManager.Enemies.Find(x => x.IsValidTarget(W.Range) && !x.HasBuffOfType(BuffType.SpellShield));

                if (t != null)
                    if (W.IsReady())
                        W.Cast();
            }
        }

        private static void ComboLogic()
        {
            if (ComboW)
            {
                var t = HeroManager.Enemies.Find(x => x.IsValidTarget(W.Range) && !x.HasBuffOfType(BuffType.SpellShield));

                if (t != null)
                    if (W.IsReady())
                        W.Cast();
            }

            if (E.IsReady())
            {
                var t = HeroManager.Enemies.Where(e => e.IsValidTarget(E.Range + Orbwalking.GetAttackRange(Me)));

                if (ComboE == 0)
                {
                    var t1 = t.OrderByDescending(e => TargetSelector.GetPriority(e)).FirstOrDefault();

                    if (t1 != null)
                        E.Cast(t1.ServerPosition);
                }
                else if(ComboE == 1)
                {
                    if (t != null)
                        E.Cast(Game.CursorPos);
                }
            }

            if (ComboR)
            {
                if (R.IsReady())
                {
                    if (R1Combo && !Me.HasBuff("RivenFengShuiEngine"))
                    {
                        var t = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

                        if (t.Distance(Me.ServerPosition) < E.Range + Me.AttackRange && Me.CountEnemiesInRange(500) >= 1 && !t.IsDead)
                            R.Cast();
                    }

                    if (Me.HasBuff("RivenFengShuiEngine"))
                    {
                        var t = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

                        if (t.ServerPosition.Distance(Me.ServerPosition) < 850 && !t.IsDead)
                        {
                            switch (R2Mode)
                            {
                                case 0:
                                    if (R.GetDamage(t) > t.Health && t.IsValidTarget(R.Range) && t.Distance(Me.ServerPosition) < 600)
                                    {
                                        CastR2 = true;
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                    break;
                                case 1:
                                    if (t.HealthPercent < 25 && t.Health > R.GetDamage(t) + Damage.GetAutoAttackDamage(Me, t) * 2)
                                    {
                                        CastR2 = true;
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                    break;
                                case 2:
                                    if (t.IsValidTarget(R.Range) && t.Distance(Me.ServerPosition) < 600)
                                    {
                                        CastR2 = true;
                                    }
                                    else
                                    {
                                        CastR2 = false;
                                    }
                                    break;
                                case 3:
                                    CastR2 = false;
                                    break;
                            }
                        }

                        if (CastR2 && !t.IsDead)
                        {
                            R.Cast(t);
                        }
                    }
                }
            }
        }

        private static void ItemCastLogic(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            var t = args.Target;

            if (t is AIHeroClient)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ComboItem)
                {
                    if (LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                        LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem().Cast();

                    if (LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                        LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().Cast();

                    if (LeagueSharp.Common.Data.ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady())
                        LeagueSharp.Common.Data.ItemData.Titanic_Hydra_Melee_Only.GetItem().Cast();

                    if (LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                        LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
                }

                if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass) && HarassItem)
                {
                    if (LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                        LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().Cast();

                    if (LeagueSharp.Common.Data.ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady())
                        LeagueSharp.Common.Data.ItemData.Titanic_Hydra_Melee_Only.GetItem().Cast();

                    if (LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                        LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
                }
            }

            if (t is Obj_AI_Minion)// && Orbwalking.InAutoAttackRange(t as Obj_AI_Minion))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (LaneClearItem || JungleClearItem)
                    {
                        if (LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                            LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().Cast();

                        if (LeagueSharp.Common.Data.ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady())
                            LeagueSharp.Common.Data.ItemData.Titanic_Hydra_Melee_Only.GetItem().Cast();

                        if (LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                            LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
                    }
                }
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (AntiGapCloserW)
            {
                if (W.IsReady())
                    if (gapcloser.Sender.IsValidTarget(W.Range))
                        W.Cast();
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (InterruptTargetW)
            {
                if (args.DangerLevel >= Interrupter2.DangerLevel.High)
                    if (sender.IsValidTarget(W.Range))
                        if (W.IsReady())
                            W.Cast();
            }
        }

        private static void LightQALogic(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
                return;

            switch (args.Animation)
            {
                case "Spell1a":
                    DisableAttack = true;
                    QStack = 1;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
                    LeagueSharp.Common.Utility.DelayAction.Add(250 - Game.Ping, () => { Chat.Say("/d"); });
                    Orbwalking.LastAATick = 0;
                    if (Dance) { LeagueSharp.Common.Utility.DelayAction.Add(DanceDelay, () => { Chat.Say("/d"); }); }
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
                    break;
                case "Spell1b":
                    DisableAttack = true;
                    QStack = 2;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
                    LeagueSharp.Common.Utility.DelayAction.Add(250 - Game.Ping, () => { Chat.Say("/d"); });
                    Orbwalking.LastAATick = 0;
                    if (Dance) { LeagueSharp.Common.Utility.DelayAction.Add(DanceDelay, () => { Chat.Say("/d"); }); }
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 15);
                    break;
                case "Spell1c":
                    DisableAttack = true;
                    QStack = 0;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
                    LeagueSharp.Common.Utility.DelayAction.Add(250 - Game.Ping, () => { Chat.Say("/d"); });
                    Orbwalking.LastAATick = 0;
                    if (Dance) { LeagueSharp.Common.Utility.DelayAction.Add(DanceDelay, () => { Chat.Say("/d"); }); }
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
                    break;
                case "Spell2":
                    DisableAttack = true;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
                    LeagueSharp.Common.Utility.DelayAction.Add(250 - Game.Ping, () => { Chat.Say("/d"); });
                    Orbwalking.LastAATick = 0;
                    if (Dance) { LeagueSharp.Common.Utility.DelayAction.Add(DanceDelay, () => { Chat.Say("/d"); }); }
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos + 10);
                    break;
                case "Spell3":
                    DisableAttack = true;
                    break;
                case "Spell4":
                    DisableAttack = true;
                    break;
                default:
                    DisableAttack = false;
                    break;
            }
        }

        private static void ProcessBool(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe)
                args.Process = !DisableAttack;
        }

        private static void AfterAAQLogic(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            var t = args.Target;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Brust)
            {
                if (Q.IsReady())
                {
                    if (t is AIHeroClient && !t.IsDead)
                        Q.Cast(t.Position);
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (Q.IsReady())
                {
                    if (HarassQ)
                        if (t is AIHeroClient && !t.IsDead)
                            Q.Cast(t.Position);
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass)
            {
                if(Q.IsReady() && QStack != 2)
                {
                    if(HarassQ)
                    {
                        if (t is AIHeroClient && !t.IsDead)
                            Q.Cast(t.Position);
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (t is Obj_AI_Minion)
                {
                    if (Q.IsReady())
                    {
                        if (LaneClearQ)
                        {
                            var min = MinionManager.GetMinions(E.Range + Me.AttackRange);

                            if (min != null)
                                if(min.Count > 2)
                                    Q.Cast(min.FirstOrDefault().Position);
                        }

                        if (JungleClearQ)
                        {
                            var mob = MinionManager.GetMinions(E.Range + Me.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                            if (mob != null)
                                Q.Cast(mob.FirstOrDefault());
                        }
                    }
                }

                if (t.Type == GameObjectType.obj_AI_Turret || t.Type == GameObjectType.obj_Turret)
                {
                    if (Q.IsReady() && !t.IsDead)
                    {
                        Q.Cast(t.Position);
                    }
                }
            }
        }
    }
}