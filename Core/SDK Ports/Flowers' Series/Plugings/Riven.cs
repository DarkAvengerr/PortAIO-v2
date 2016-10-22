using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Plugings
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Linq;
    using static Common.Manager;
    using Keys = System.Windows.Forms.Keys;

    public class Riven
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static SpellSlot Ignite;
        private static SpellSlot Flash;
        private static int QCount;
        private static bool CanQ, CanR, CanR2;
        private static AttackableUnit Target;
        private static HpBarDraw HpBarDraw = new HpBarDraw();

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W, 265f);
            E = new Spell(SpellSlot.E, 312f);
            R = new Spell(SpellSlot.R, 900f) { MinHitChance = HitChance.High };
            R.SetSkillshot(0.25f, 45f, 1600f, false, SkillshotType.SkillshotCone);

            Ignite = Me.GetSpellSlot("SummonerDot");
            Flash = Me.GetSpellSlot("SummonerFlash");

            var ComboMenu = Menu.Add(new Menu("Riven_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuKeyBind("R", "Use R1", Keys.L, KeyBindType.Toggle));
                ComboMenu.Add(new MenuBool("R2", "Max Damage R2", true));
                ComboMenu.Add(new MenuSeparator("Misc Settings", "Misc Settings"));
                ComboMenu.Add(new MenuBool("Ignite", "Use Ignite", true));
                ComboMenu.Add(new MenuBool("Item", "Use Item", true));
                ComboMenu.Add(new MenuKeyBind("Burst", "Burst Mode", Keys.T, KeyBindType.Toggle));
                ComboMenu.Add(new MenuBool("Flash", "Burst Mode| Use Flash", true));
                ComboMenu.Add(new MenuSeparator("AboutBurst", "In Burst Mode You Should Click A Target"));
            }

            var HarassMenu = Menu.Add(new Menu("Riven_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("W", "Use W", true));
                HarassMenu.Add(new MenuBool("E", "Use E", true));
                HarassMenu.Add(new MenuBool("Item", "Use Item", true));
            }

            var LaneClearMenu = Menu.Add(new Menu("Riven_LaneClear", "LaneClear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuSliderButton("W", "Use W| Min Hit Counts >=", 3, 1, 5, true));
            }

            var JungleClearMenu = Menu.Add(new Menu("Riven_JungleClear", "JungleClear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
            }

            var MiscMenu = Menu.Add(new Menu("Riven_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("KeepQ", "Keep Q Alive", true));
                MiscMenu.Add(new MenuBool("GapCloser", "Use W|Anti GapCloser", true));
                MiscMenu.Add(new MenuBool("Interrupt", "Use W|Interrupt Danger Spell", true));
            }

            var FleeMenu = Menu.Add(new Menu("Riven_Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("Q", "Use Q", true));
                FleeMenu.Add(new MenuBool("W", "Use W", true));
                FleeMenu.Add(new MenuBool("E", "Use E", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var DrawMenu = Menu.Add(new Menu("Riven_Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("W", "Draw W"));
                DrawMenu.Add(new MenuBool("DrawBurstMin", "Draw Burst Engage(Not Flash)", true));
                DrawMenu.Add(new MenuBool("DrawBurstEngage", "Draw Burst Engage(Flash)", true));
                DrawMenu.Add(new MenuBool("DrawR1", "Draw R1 Statis", true));
                DrawMenu.Add(new MenuBool("DrawBurst", "Draw Burst Statis", true));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Obj_AI_Base.OnProcessSpellCast += SpellBool;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Events.OnGapCloser += OnGapCloser;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
            Variables.Orbwalker.OnAction += OnAction;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += CastSpellLogic;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
        }

        private static void SpellBool(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (Args.SData.Name.Contains("RivenTriCleave"))
                {
                    CanQ = false;
                }

                if (Args.SData.Name.Contains("RivenFengShuiEngine"))
                {
                    CanR = false;
                }

                if (Args.SData.Name.Contains("RivenIzunaBlade"))
                {
                    CanR2 = false;
                }
            }
        }

        private static void CastSpellLogic(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (InCombo)
            {
                AIHeroClient target = null;

                if (Variables.TargetSelector.GetSelectedTarget() != null)
                {
                    target = GetTarget(E.Range + W.Range);
                }
                else
                {
                    target = Variables.TargetSelector.GetSelectedTarget();
                }

                if(CheckTarget(target))
                {
                    if (R.Instance.Name.Contains("RivenIzunaBlade"))
                    {
                        if(Args.SData.Name.Contains(Q.Instance.Name) && target.IsValidTarget(Q.Range))
                        {
                            var hp = target.Health;
                            var QDamage = GetQDamage(target);
                            var WDamage = GetWDamage(target);
                            var RDamage = GetRDamage(target);
                            var QRDamage = QDamage + RDamage;
                            var QWRDamage = QDamage + WDamage + RDamage;

                            if (IsBurstActive)
                            {
                                DelayAction.Add(10, () => CastR2());
                            }
                            else if (hp < QRDamage || (hp < QWRDamage && W.IsReady() && target.IsValidTarget(W.Range)) && hp < GetComboDamage(target) && hp > QDamage)
                            {
                                DelayAction.Add(10, () => CastR2());
                            }
                        }
                    }

                    if (Args.SData.Name.Contains(W.Instance.Name) || Args.SData.Name.Contains(E.Instance.Name))
                    {
                        CastItem(target);
                    }
                }
            }
        }

        private static void OnInterruptableTarget(object obj, Events.InterruptableTargetEventArgs Args)
        {
            if (Menu["Riven_Misc"]["Interrupt"] && Args.Sender.IsEnemy && Args.Sender.IsValidTarget(W.Range) && W.IsReady())
            {
                if (Args.DangerLevel >= LeagueSharp.Data.Enumerations.DangerLevel.High ||
                    Args.Sender.IsCastingInterruptableSpell())
                {
                    W.Cast();
                }
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Riven_Misc"]["GapCloser"] && W.IsReady())
            {
                if (Args.IsDirectedToPlayer && (Args.End.DistanceToPlayer() < W.Range || Args.Sender.IsValidTarget(W.Range)))
                {
                    W.Cast();
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Riven_Draw"]["W"] && W.IsReady())
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.YellowGreen);

            if (Menu["Riven_Draw"]["DrawBurstMin"])
                Render.Circle.DrawCircle(Me.Position, E.Range + GetAttackRange(Me), System.Drawing.Color.Blue);

            if (Menu["Riven_Draw"]["DrawBurstEngage"])
            {
                Render.Circle.DrawCircle(Me.Position, E.Range + 425f, System.Drawing.Color.Red);
            }

            if (Menu["Riven_Draw"]["DrawDamage"])
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg((float)GetComboDamage(e), new ColorBGRA(255, 204, 0, 170));
                }
            }

            if (Menu["Riven_Draw"]["DrawR1"])
            {
                var text = "";
                if (Menu["Riven_Combo"]["R"].GetValue<MenuKeyBind>().Active)
                    text = "Enable";
                if (!Menu["Riven_Combo"]["R"].GetValue<MenuKeyBind>().Active)
                    text = "Off";
                Drawing.DrawText(Me.HPBarPosition.X + 0, Me.HPBarPosition.Y - 60, System.Drawing.Color.Red, "Use R1(" + Menu["Riven_Combo"]["R"].GetValue<MenuKeyBind>().Key.ToString() + "): " );
                Drawing.DrawText(Me.HPBarPosition.X + 90, Me.HPBarPosition.Y - 60, System.Drawing.Color.GreenYellow, text);
            }

            if (Menu["Riven_Draw"]["DrawBurst"])
            {
                var text = "";
                if (IsBurstActive)
                    text = "Enable";
                if (!IsBurstActive)
                    text = "Off";
                Drawing.DrawText(Me.HPBarPosition.X + 0, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "Burst(" + Menu["Riven_Combo"]["Burst"].GetValue<MenuKeyBind>().Key.ToString() + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 90, Me.HPBarPosition.Y - 40, System.Drawing.Color.GreenYellow, text);
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (sender.IsMe)
            {
                switch(Args.Animation)
                {
                    case "Spell1a":
                        QCount = 1;
                        break;
                    case "Spell1b":
                        QCount = 2;
                        break;
                    case "Spell1c":
                        QCount = 3;
                        break;
                    default:
                        QCount = 0;
                        break;
                }
            }
        }

        private static void CastQ(AttackableUnit target)
        {
            CanQ = true;
            Target = target;
        }

        private static void CastR()
        {
            CanR = R.IsReady() && R.Instance.Name.Contains("RivenFengShuiEngine");

            DelayAction.Add(500, () => CanR = false);
        }

        private static void CastR2()
        {
            CanR2 = R.IsReady() && R.Instance.Name.Contains("RivenIzunaBlade");

            DelayAction.Add(500, () => CanR2 = false);
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (Args.Target != null)
                {
                    var target = (Obj_AI_Base)Args.Target;

                    if (InCombo || InHarass)
                    {
                        if (Variables.TargetSelector.GetSelectedTarget() != null)
                        {
                            target = Variables.TargetSelector.GetSelectedTarget();
                        }
                        else
                        {
                            target = (Obj_AI_Base)Args.Target;
                        }

                        if (Q.IsReady() && target.IsValidTarget(Q.Range))
                        {
                            CastQ(target);
                        }
                    }

                    if (InClear)
                    {
                        LaneDoCast(Args);
                        JungleDoCast(Args);
                    }
                }
            }
        }

        private static void Logics()
        {
            var target = Target as Obj_AI_Base;

            if (CanQ && target != null)
            {
                Q.Cast(target.ServerPosition - 10);
            }

            if (CanR && R.IsReady() && R.Instance.Name.Contains("RivenFengShuiEngine"))
            {
                R.Cast();
            }

            if (CanR2 && R.Instance.Name.Contains("RivenIzunaBlade") && target != null && target.IsHPBarRendered)
            {
                if (R.GetPrediction(target, true).Hitchance >= HitChance.High)
                {
                    R.Cast(R.GetPrediction(target, true).CastPosition);
                }
            }
        }

        private static void LaneDoCast(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu["Riven_LaneClear"]["Q"] && Q.IsReady())
            {
                if (Args.Target is Obj_AI_Turret || Args.Target is Obj_Barracks || Args.Target is Obj_BarracksDampener || Args.Target is Obj_Building)
                {
                    var target = (Obj_AI_Base)Args.Target;
                    CastQ(target);
                }
                else
                {
                    var minions = GetMinions(Me.Position, Q.Range);

                    if (minions.Count() >= 2)
                    {
                        var target = (Obj_AI_Base)Args.Target;
                        CastQ(target);
                    }
                }
            }
        }

        private static void JungleDoCast(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu["Riven_JungleClear"]["Q"] && Q.IsReady())
            {
                var mob = GetMobs(Me.Position, Q.Range, true).FirstOrDefault();

                if (mob != null)
                {
                    var target = (Obj_AI_Base)Args.Target;
                    CastQ(target);
                }
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear && Menu["Riven_JungleClear"]["E"])
                {
                    foreach (var mob in GetMobs(Me.Position, E.Range))
                    {
                        if (mob != null)
                        {
                            if (mob.HasBuffOfType(BuffType.Stun) && !W.IsReady())
                            {
                                E.Cast(Game.CursorPos);
                            }
                            else if (!mob.HasBuffOfType(BuffType.Stun))
                            {
                                E.Cast(Game.CursorPos);
                            }
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                CanQ = false;
                CanR = false;
                CanR2 = false;
                return;
            }

            W.Range = Me.HasBuff("RivenFengShuiEngine") ? 330 : 265;

            Logics();

            if (InCombo)
            {
                BurstLogic();
                ComboLogic();
            }

            if (InHarass)
            {
                HarassLogic();
            }

            if (InClear)
            {
                LaneClearLogic();
                JungleClearLogic();
            }

            if (Menu["Riven_Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                FleeLogic();
            }

            if (Menu["Riven_Misc"]["KeepQ"] && !Me.IsUnderEnemyTurret() && !Me.IsRecalling() && Me.HasBuff("RivenTriCleave") && InNone)
            {
                if (Me.GetBuff("RivenTriCleave").EndTime - Game.Time < 0.3)
                    Q.Cast(Game.CursorPos);
            }

            if (R.Instance.Name.Contains("RivenIzunaBlade"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && !x.IsZombie && x.IsValidTarget(900) && x.Health < GetRDamage(x)))
                {
                    if (CheckTarget(target))
                    {
                        CastR2();
                    }
                }
            }
        }

        private static void FastHarassLogic()
        {
            var target = Variables.TargetSelector.GetSelectedTarget();

            if (CheckTarget(target))
            {
                if (QCount == 2)
                {
                    if (E.IsReady())
                    {
                        E.Cast(Me.ServerPosition + (Me.ServerPosition - target.ServerPosition).Normalized() * E.Range);
                    }

                    if (!E.IsReady())
                    {
                        Q.Cast(Me.ServerPosition + (Me.ServerPosition - target.ServerPosition).Normalized() * E.Range);
                    }
                }

                if (W.IsReady())
                {
                    if (target.IsValidTarget(W.Range) && QCount == 1)
                    {
                        W.Cast();
                    }
                }

                if (Q.IsReady())
                {
                    if (QCount == 0)
                    {
                        if (target.IsValidTarget(Me.AttackRange + Me.BoundingRadius + 150))
                        {
                            Q.Cast(target.Position);
                        }
                    }
                }

                if (E.IsReady() && target.DistanceToPlayer() > GetAttackRange(Me) + E.Range + W.Range)
                {
                    E.Cast(target.Position);
                }
            }
        }

        private static void HarassLogic()
        {
            var target = GetTarget(600, DamageType.Physical);

            if (CheckTarget(target))
            {
                if (Menu["Riven_Harass"]["E"] && !target.IsValidTarget(W.Range) && target.IsValidTarget(600) && E.IsReady())
                {
                    E.Cast(target.Position);
                }

                if (Menu["Riven_Harass"]["W"] && target.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast();
                }

                CastItem(target);
            }
        }

        private static void LaneClearLogic()
        {
            if (GetMinions(Me.Position, W.Range).Count() > 0)
            {
                if (Menu["Riven_LaneClear"]["W"].GetValue<MenuSliderButton>().BValue && W.IsReady() 
                    && GetMinions(Me.Position, W.Range).Count() >= Menu["Riven_LaneClear"]["W"].GetValue<MenuSliderButton>().SValue)
                {
                    W.Cast();
                }

                if (Items.HasItem(3074))
                {
                    Items.UseItem(3074);
                }

                if (Items.HasItem(3077))
                {
                    Items.UseItem(3077);
                }

                if (Items.HasItem(3053))
                {
                    Items.UseItem(3053);
                }
            }
        }

        private static void JungleClearLogic()
        {
            if (GetMobs(Me.Position, W.Range).Count() > 0)
            {
                if (Menu["Riven_JungleClear"]["Q"] && W.IsReady())
                {
                    W.Cast();
                }

                if (Items.HasItem(3074))
                {
                    Items.UseItem(3074);
                }

                if (Items.HasItem(3077))
                {
                    Items.UseItem(3077);
                }

                if (Items.HasItem(3053))
                {
                    Items.UseItem(3053);
                }
            }
        }

        private static void BurstLogic()
        {
            if (IsBurstActive)
            {
                var target = Variables.TargetSelector.GetSelectedTarget();

                if (IsBurstActive && target != null && target.IsHPBarRendered)
                {
                    if (target.DistanceToPlayer() < E.Range + 425f + GetAttackRange(Me))
                    {
                        if (R.IsReady() && R.Instance.Name.Contains("RivenFengShuiEngine") && E.IsReady() && W.IsReady() && target.DistanceToPlayer() < E.Range + GetAttackRange(Me) - 50)
                        {
                            CastYM();
                            E.Cast(target.ServerPosition);
                            CastR();
                            DelayAction.Add(50, () => W.Cast());
                        }
                        else if (R.IsReady() && R.Instance.Name.Contains("RivenFengShuiEngine") && E.IsReady() && W.IsReady() && Flash.IsReady() && Menu["Riven_Combo"]["Flash"] && target.DistanceToPlayer() < E.Range + 425f)
                        {
                            CastYM();
                            E.Cast(target.ServerPosition);
                            CastR();

                            DelayAction.Add(50, () =>
                            {
                                W.Cast();
                                DelayAction.Add(10, () => Me.Spellbook.CastSpell(Flash, target.ServerPosition));
                            });
                        }
                    }
                }
            }
        }

        private static void ComboLogic()
        {
            var target = GetTarget(900, DamageType.Physical);

            if (CheckTarget(target))
            {
                RLogic();

                if (Menu["Riven_Combo"]["E"] && Menu["Riven_Combo"]["W"] && E.IsReady() && W.IsReady())
                {
                    if (target.DistanceToPlayer() < E.Range + W.Range && !target.IsValidTarget(W.Range))
                    {
                        E.Cast(target.Position);
                    }
                }
                else if (Menu["Riven_Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
                else if (Menu["Riven_Combo"]["E"] && E.IsReady())
                {
                    E.Cast(target.Position);
                }

                if (Menu["Riven_Combo"]["Item"])
                {
                    CastItem(target);
                }

                if (Menu["Riven_Combo"]["Ignite"] && Ignite.IsReady())
                {
                    if (target != null && target.HealthPercent < 25)
                    {
                        Me.Spellbook.CastSpell(Ignite, target);
                    }
                }
            }
        }

        private static void RLogic()
        {
            var target = GetTarget(900);

            if (CheckTarget(target))
            {
                if (Menu["Riven_Combo"]["R"].GetValue<MenuKeyBind>().Active && R.IsReady() &&
                    (!IsBurstActive || (IsBurstActive && Variables.TargetSelector.GetSelectedTarget() == null)))
                {
                    if (R.Instance.Name.Contains("RivenFengShuiEngine") && Menu["Riven_Combo"]["R"].GetValue<MenuKeyBind>().Active)
                    {
                        if (target.DistanceToPlayer() < E.Range + GetAttackRange(Me) && Me.CountEnemyHeroesInRange(500) > 0)
                        {
                            CastR();
                        }
                    }

                    if (R.Instance.Name.Contains("RivenIzunaBlade") && Menu["Riven_Combo"]["R2"])
                    {
                        if (target.HealthPercent < 25 && target.Health > R.GetDamage(target) + Damage.GetAutoAttackDamage(Me, target) * 2)
                        {
                            CastR2();
                        }
                        else if (Me.HealthPercent < 5 && target.DistanceToPlayer() < 600)
                        {
                            CastR2();
                        }
                    }
                }
            }
        }

        private static void FleeLogic()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            if (!Me.IsDashing())
            {
                if (Q.IsReady() && Menu["Riven_Flee"]["Q"])
                {
                    Q.Cast(Me.Position.Extend(Game.CursorPos, 200));
                }

                if (E.IsReady() && Menu["Riven_Flee"]["E"])
                {
                    if (Me.Position.Extend(Game.CursorPos, 300).IsWall())
                    {
                        return;
                    }

                    E.Cast(Me.Position.Extend(Game.CursorPos, 300));
                }
            }

            if (W.IsReady() && Menu["Riven_Flee"]["W"])
            {
                var enemy = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range) && !x.IsDead).Any();

                if (enemy)
                {
                    W.Cast();
                }
            }
        }

        private static void CastYM()
        {
            if (Items.HasItem(3142) && Items.CanUseItem(3142))
            {
                Items.UseItem(3142);
            }
        }

        private static void CastItem(Obj_AI_Base target)
        {
            if (InCombo && Items.HasItem(3153) && Items.CanUseItem(3153) && target.IsValidTarget(550))
            {
                Items.UseItem(3153, target);
            }

            if (Items.HasItem(3074) && target.IsValidTarget(330))
            {
                Items.UseItem(3074);
            }

            if (Items.HasItem(3077) && target.IsValidTarget(330))
            {
                Items.UseItem(3077);
            }

            if (Items.HasItem(3053) && target.IsValidTarget(330))
            {
                Items.UseItem(3053);
            }

            if (Items.HasItem(3142) && Items.CanUseItem(3142) && target.IsValidTarget(Me.AttackRange + E.Range))
            {
                Items.UseItem(3142);
            }
        }

        private static bool IsBurstActive => Menu["Riven_Combo"]["Burst"].GetValue<MenuKeyBind>().Active;

        private static double GetComboDamage(AIHeroClient target)
        {
            return GetQDamage(target) + GetWDamage(target) + GetRDamage(target) + Me.GetAutoAttackDamage(target);
        }

        private static double GetQDamage(AIHeroClient target)
        {
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
                var qhan = 3 - QCount;
                damage += Q.GetDamage(target) * qhan + Me.GetAutoAttackDamage(target) * qhan * (1 + passive);
            }

            return damage;
        }

        private static double GetWDamage(AIHeroClient target)
        {
            double damage = 0;

            if (W.IsReady())
                damage += W.GetDamage(target);

            return damage;
        }

        private static double GetRDamage(AIHeroClient target)
        {
            double damage = 0;

            if (R.IsReady())
                if (Me.HasBuff("RivenFengShuiEngine"))
                    damage += Me.CalculateDamage
                        (target, DamageType.Physical, 
                        (new double[] { 80, 120, 160 }[R.Level - 1] +
                        0.6 * Me.FlatPhysicalDamageMod) * 
                        (1 + (target.MaxHealth - target.Health) / target.MaxHealth > 0.75 ?
                        0.75 : (target.MaxHealth - target.Health) / target.MaxHealth) * 8 / 3);

            return damage;
        }
    }
}
