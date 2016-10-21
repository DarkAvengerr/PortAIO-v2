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
    using System;
    using System.Linq;
    using static Common.Manager;

    public class Illaoi
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static HpBarDraw DrawHpBar = new HpBarDraw();

        private static AIHeroClient Me => Program.Me;

        private static Menu Menu => Program.Menu;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            W = new Spell(SpellSlot.W, 400f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 450f);

            Q.SetSkillshot(0.75f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.066f, 50f, 1900f, true, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.Add(new Menu("Illaoi_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("QGhost", "Use Q | To Ghost", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("WOutRange", "Use W | Out of Attack Range"));
                ComboMenu.Add(new MenuBool("WUlt", "Use W | Ult Active", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuBool("RSolo", "Use R | 1v1 Mode", true));
                ComboMenu.Add(new MenuSlider("RCount", "Use R | Counts Enemies >=", 2, 1, 5));
            }

            var HarassMenu = Menu.Add(new Menu("Illaoi_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("W", "Use W", true));
                HarassMenu.Add(new MenuBool("WOutRange", "Use W | Only Out of Attack Range", true));
                HarassMenu.Add(new MenuBool("E", "Use E", true));
                HarassMenu.Add(new MenuBool("Ghost", "Attack Ghost", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("Illaoi_LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuSliderButton("Q", "Use Q | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= %", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("Illaoi_JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuBool("Item", "Use Item", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= %", 60));
            }

            var KillStealMenu = Menu.Add(new Menu("Illaoi_KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("E", "Use E", true));
                KillStealMenu.Add(new MenuBool("R", "Use R", true));
                KillStealMenu.Add(new MenuSeparator("RList", "R List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => KillStealMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var EBlacklist = Menu.Add(new Menu("Illaoi_EBlackList", "E BlackList"));
            {
                EBlacklist.Add(new MenuSeparator("Adapt", "Only Adapt to Harass & KillSteal & Anti GapCloser Mode!"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => EBlacklist.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, false)));
                }
            }

            var MiscMenu = Menu.Add(new Menu("Illaoi_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("EGap", "Use E Anti GapCloset", true));
            }

            var DrawMenu = Menu.Add(new Menu("Illaoi_Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Events.OnGapCloser += OnGapCloser;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Variables.Orbwalker.OnAction += OnAction;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Illaoi_Misc"]["EGap"] && Args.IsDirectedToPlayer)
            {
                var sender = Args.Sender as AIHeroClient;

                if (sender.IsEnemy && (Args.End.DistanceToPlayer() <= 200 || sender.DistanceToPlayer() <= 250) && !Menu["Illaoi_EBlackList"][sender.ChampionName.ToLower()])
                {
                    E.Cast(sender);
                }
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (InCombo)
            {
                var target = GetTarget(W.Range, DamageType.Physical);

                if (CheckTarget(target))
                {
                    if (Menu["Illaoi_Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        if (Menu["Illaoi_Combo"]["WOutRange"] && !InAutoAttackRange(target))
                        {
                            W.Cast();
                        }
                        else if (!Menu["Illaoi_Combo"]["WOutRange"])
                        {
                            W.Cast();
                        }

                        if (Menu["Illaoi_Combo"]["WUlt"] && Me.HasBuff("IllaoiR"))
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void OnAction(object sender, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack)
            {
                var target = GetTarget(W.Range, DamageType.Physical);

                if (InHarass && Me.HasBuff("IllaoiW") && CheckTarget(target) && Args.Target is Obj_AI_Minion)
                {
                    Args.Process = false;
                }
                else
                {
                    Args.Process = true;
                }
            }

            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (InCombo)
                {
                    var target = GetTarget(W.Range, DamageType.Physical);

                    if (CheckTarget(target))
                    {
                        if (Menu["Illaoi_Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                        {
                            if (Menu["Illaoi_Combo"]["WOutRange"] && !InAutoAttackRange(target))
                            {
                                W.Cast();
                            }

                            if (Menu["Illaoi_Combo"]["WUlt"] && Me.HasBuff("IllaoiR"))
                            {
                                W.Cast();
                            }
                        }
                    }
                }

                if (InHarass && !Me.IsUnderEnemyTurret())
                {
                    if (Me.ManaPercent >= Menu["Illaoi_Harass"]["Mana"].GetValue<MenuSlider>().Value)
                    {
                        var target = GetTarget(W.Range, DamageType.Physical);

                        if (CheckTarget(target))
                        {
                            if (Menu["Illaoi_Harass"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                            {
                                if (Menu["Illaoi_Harass"]["WOutRange"] && !InAutoAttackRange(target))
                                {
                                    W.Cast();
                                }
                                else if (!Menu["Illaoi_Harass"]["WOutRange"])
                                {
                                    W.Cast();
                                }
                            }
                        }
                    }
                }

                if (InClear)
                {
                    if (Me.ManaPercent >= Menu["Illaoi_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                    {
                        var Mobs = GetMobs(Me.Position, W.Range, true);
                        if (Mobs.Count() > 0)
                        {
                            if (Menu["Illaoi_JungleClear"]["W"] && W.IsReady() && !AutoAttack.IsAutoAttack(Me.ChampionName))
                            {
                                W.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (InCombo)
            {
                Combo();
            }

            if (InHarass)
            {
                Harass();
            }

            if (InClear)
            {
                Lane();
                Jungle();
            }

            if (InLastHit)
            {
                LastHitLogic();
            }

            KillSteal();
        }

        private static void Combo()
        {
            var target = GetTarget(Q.Range, DamageType.Physical);
            var Ghost = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsHPBarRendered && x.IsValidTarget(Q.Range)).FirstOrDefault(x => x.HasBuff("illaoiespirit"));

            if (CheckTarget(target) && target.IsValidTarget(Q.Range))
            {
                if (Menu["Illaoi_Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target) && !((W.IsReady() || Me.HasBuff("IllaoiW")) && target.IsValidTarget(W.Range)))
                {
                    Q.Cast(target);
                }

                if (Menu["Illaoi_Combo"]["E"] && E.IsReady() && !InAutoAttackRange(target) && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (Menu["Illaoi_Combo"]["R"] && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu["Illaoi_Combo"]["RSolo"] && target.Health <= GetDamage(target) + Me.GetAutoAttackDamage(target) * 3 && Me.CountEnemyHeroesInRange(R.Range) == 1)
                    {
                        R.Cast(target);
                    }

                    if (Me.CountEnemyHeroesInRange(R.Range - 50) >= Menu["Illaoi_Combo"]["RCount"].GetValue<MenuSlider>().Value)
                    {
                        R.Cast();
                    }
                }
            }
            else if (target == null && Ghost != null)
            {
                if (Ghost != null && Q.IsReady() && Menu["Illaoi_Combo"]["Q"] && Menu["Illaoi_Combo"]["QGhost"])
                {
                    Q.Cast(Ghost);
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu["Illaoi_Harass"]["Mana"].GetValue<MenuSlider>().Value && !Me.IsUnderEnemyTurret())
            {
                var target = GetTarget(Q.Range, DamageType.Physical);
                var Ghost = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsHPBarRendered && x.IsValidTarget(Q.Range)).FirstOrDefault(x => x.HasBuff("illaoiespirit"));

                if (CheckTarget(target) && target.IsValidTarget(Q.Range))
                {
                    if (Menu["Illaoi_Harass"]["E"] && E.IsReady() && E.CanCast(target) && !Menu["Illaoi_EBlackList"][target.ChampionName.ToLower()] && !InAutoAttackRange(target) && target.IsValidTarget(E.Range))
                    {
                        E.Cast(target);
                    }

                    if (Menu["Illaoi_Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && !(E.IsReady() && Menu["Illaoi_Harass"]["E"] && E.GetPrediction(target).Hitchance >= HitChance.VeryHigh))
                    {
                        Q.Cast(target);
                    }

                    if (Menu["Illaoi_Harass"]["W"] && W.IsReady() && Menu["Illaoi_Harass"]["WOutRange"] && !InAutoAttackRange(target) && target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }
                else if (target == null && Ghost != null)
                {
                    if (Q.IsReady() && Menu["Illaoi_Harass"]["Q"])
                        Q.Cast(Ghost);

                    if (W.IsReady() && Menu["Illaoi_Harass"]["W"])
                    {
                        W.Cast();
                    }

                    if (Menu["Illaoi_Harass"]["Ghost"])
                    {
                        Variables.Orbwalker.ForceTarget = Ghost;
                    }
                }
            }
        }

        private static void Lane()
        {
            if (Me.ManaPercent >= Menu["Illaoi_LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                if (Menu["Illaoi_LaneClear"]["Q"].GetValue<MenuSliderButton>().BValue && Q.IsReady())
                {
                    var Minions = GetMinions(Me.Position, Q.Range);

                    if (Minions.Count() > 0)
                    {
                        var QFarm = Q.GetLineFarmLocation(Minions, Q.Width);

                        if (QFarm.MinionsHit >= Menu["Illaoi_LaneClear"]["Q"].GetValue<MenuSliderButton>().SValue)
                        {
                            Q.Cast(QFarm.Position);
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            var Mobs = GetMobs(Me.Position, Q.Range, true);

            if (Mobs.Count() > 0)
            {
                if (Me.ManaPercent >= Menu["Illaoi_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["Illaoi_JungleClear"]["Q"] && Q.IsReady() && !AutoAttack.IsAutoAttack(Me.ChampionName))
                    {
                        Q.Cast(Mobs.FirstOrDefault());
                    }
                }
            }
        }

        private static void LastHitLogic()
        {
            var Ghost = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsHPBarRendered && x.IsValidTarget(GetAttackRange(Me))).FirstOrDefault(x => x.HasBuff("illaoiespirit"));

            if (Ghost != null)
            {
                Variables.Orbwalker.ForceTarget = Ghost;
            }
        }

        private static void KillSteal()
        {
            if (Menu["Illaoi_KillSteal"]["Q"] && Q.IsReady())
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (CheckTarget(qt))
                {
                    Q.Cast(qt);
                    return;
                }
            }

            if (Menu["Illaoi_KillSteal"]["E"] && E.IsReady())
            {
                var et = GameObjects.EnemyHeroes.Where(x => !Menu["Illaoi_EBlackList"][x.ChampionName.ToLower()] && x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x)).FirstOrDefault();

                if (CheckTarget(et))
                {
                    E.Cast(et);
                    return;
                }
            }

            if (Menu["Illaoi_KillSteal"]["R"] && R.IsReady())
            {
                var rt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range - 50) && x.Health < R.GetDamage(x) && Menu["Illaoi_KillSteal"][x.ChampionName.ToLower()]).FirstOrDefault();

                if (CheckTarget(rt))
                {
                    R.Cast(rt);
                    return;
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Illaoi_Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Illaoi_Draw"]["W"] && (W.IsReady() || Me.HasBuff("IllaoiW")))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (Menu["Illaoi_Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightYellow, 2);

            if (Menu["Illaoi_Draw"]["R"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Illaoi_Draw"]["DrawDamage"])
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg((float)GetDamage(target), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }
    }
}
