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

    public static class Morgana
    {
        private static int CastR;
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static SpellSlot Flash = SpellSlot.Unknown;
        private static HpBarDraw DrawHpBar = new HpBarDraw();

        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Program.Menu;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1150f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 800f);
            R = new Spell(SpellSlot.R, 600f);

            Q.SetSkillshot(0.25f, 70f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.50f, 200f, 2200f, false, SkillshotType.SkillshotCircle);

            Flash = Me.GetSpellSlot("SummonerFlash");

            var ComboMenu = Menu.Add(new Menu("Morgana_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuBool("RSolo", "Use R| If Can Kill Enemy", true));
                ComboMenu.Add(new MenuSlider("RCount", "Use R| Min Hit Enemies >= ", 3, 1, 5));
                ComboMenu.Add(new MenuBool("Zhonya", "Use Zhonya", true));
                ComboMenu.Add(new MenuSlider("ZhonyaHp", "When Player HealthPercent <= ", 80));
                ComboMenu.Add(new MenuKeyBind("FlashR", "Flash R!", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
                ComboMenu.Add(new MenuSlider("FlashRCount", "Flash R Min Hit Enemies >= ", 2, 1, 5));
            }

            var HarassMenu = Menu.Add(new Menu("Morgana_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("W", "Use W", true));
                HarassMenu.Add(new MenuBool("WDebuff", "Use W| Only Target Have Debuff", true));
                HarassMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("Morgana_LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", false));
                LaneClearMenu.Add(new MenuSliderButton("W", "Use W| Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "When Player ManaPercent >= %", 60));
            }

            var JungleMenu = Menu.Add(new Menu("Morgana_JungleClear", "Jungle Clear"));
            {
                JungleMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleMenu.Add(new MenuBool("W", "Use W", true));
                JungleMenu.Add(new MenuSlider("Mana", "Use Q|When Player ManaPercent >= %", 50));
            }

            var KillStealMenu = Menu.Add(new Menu("Morgana_KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("W", "Use W", true));
            }

            var QList = Menu.Add(new Menu("Morgana_QList", "Q Black List"));
            {
                QList.Add(new MenuSeparator("sfsd", "Only Work in Harass"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => QList.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var MiscMenu = Menu.Add(new Menu("Morgana_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("QInt", "Use Q| Interrupt Spells", true));
                MiscMenu.Add(new MenuBool("QGap", "Use Q| Anti Gapcloser", true));
                MiscMenu.Add(new MenuSeparator("QGapList", "Anti Gapcloser List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => MiscMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var Draw = Menu.Add(new Menu("Morgana_Draw", "Draw"));
            {
                Draw.Add(new MenuBool("Q", "Q Range"));
                Draw.Add(new MenuBool("W", "W Range"));
                Draw.Add(new MenuBool("E", "E Range"));
                Draw.Add(new MenuBool("R", "R Range"));
                Draw.Add(new MenuBool("FlashR", "Flash + R Range"));
                Draw.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
                Draw.Add(new MenuBool("DrawBurst", "Draw Flash Combo Status", true));
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Events.OnGapCloser += OnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (Args.SData.Name.ToLower() == "soulshackles")
                {
                    CastR = Environment.TickCount + 3000;
                }

                if (InCombo && Menu["Morgana_Combo"]["Zhonya"] && 
                    Items.HasItem(3157, Me) && Items.CanUseItem(3157) && 
                    Me.HealthPercent <= Menu["Morgana_Combo"]["ZhonyaHp"].GetValue<MenuSlider>().Value &&
                    CastR > Environment.TickCount && Me.CountEnemyHeroesInRange(R.Range) >= 2)
                {
                    Items.UseItem(3157);
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            if (InCombo)
            {
                ComboLogic();
            }

            if (InHarass)
            {
                HarassLogic();
            }

            if (InClear)
            {
                LaneLogic();
                JungleLogic();
            }

            KillStealLogic();
        }

        private static void ComboLogic()
        {
            var target = GetTarget(Q.Range, DamageType.Magical);

            if (CheckTarget(target))
            {
                if (Q.IsReady() && Menu["Morgana_Combo"]["Q"])
                {
                    var QPred = Q.GetPrediction(target);

                    if (!QPred.CollisionObjects.Any() && QPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(target);
                    }
                }

                if (R.IsReady() && Menu["Morgana_Combo"]["RSolo"] && Me.CountEnemyHeroesInRange(R.Range) <= 2 && target.Health < (GetDamage(target) + Me.GetAutoAttackDamage(target) * 2))
                {
                    R.Cast();
                }

                if (R.IsReady() && Menu["Morgana_Combo"]["R"] && Me.CountEnemyHeroesInRange(R.Range) >= Menu["Morgana_Combo"]["RCount"].GetValue<MenuSlider>().Value)
                {
                    R.Cast();
                }

                if (R.IsReady() && Flash != SpellSlot.Unknown && Flash.IsReady() && Menu["Morgana_Combo"]["FlashR"].GetValue<MenuKeyBind>().Active)
                {
                    var ExPos = Me.ServerPosition.Extend(Game.CursorPos, 425f); // 425f => Flash.MaxRange

                    if (ExPos.CountEnemyHeroesInRange(R.Range - 100) >= Menu["Morgana_Combo"]["FlashRCount"].GetValue<MenuSlider>().Value)
                    {
                        if (Me.Spellbook.CastSpell(Flash, ExPos))
                        {
                            R.Cast();
                        }
                    }
                }

                if (W.IsReady() && Menu["Morgana_Combo"]["W"])
                {
                    if (!CanMove(target) || (Q.IsReady() && Q.GetPrediction(target).Hitchance <= HitChance.High) || !Q.IsReady())
                    {
                        var WPred = W.GetPrediction(target, true);

                        if (WPred.Hitchance >= HitChance.VeryHigh)
                        {
                            W.Cast(WPred.CastPosition);
                        }
                    }
                }
            }
        }

        private static void HarassLogic()
        {
            if (Me.ManaPercent >= Menu["Morgana_Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = GetTarget(Q.Range - 100);

                if (CheckTarget(target))
                {
                    if (Q.IsReady() && Menu["Morgana_Harass"]["Q"] && !Menu["Morgana_QList"][target.ChampionName.ToLower()])
                    {
                        var QPred = Q.GetPrediction(target);

                        if (!QPred.CollisionObjects.Any() && QPred.Hitchance >= HitChance.VeryHigh)
                        {
                            Q.Cast(target);
                        }
                    }

                    if (W.IsReady() && Menu["Morgana_Harass"]["W"] && target.IsValidTarget(W.Range))
                    {
                        if (Menu["Morgana_Harass"]["WDebuff"])
                        {
                            if (!CanMove(target))
                            {
                                var WPred = W.GetPrediction(target, true);

                                if (WPred.Hitchance >= HitChance.VeryHigh)
                                {
                                    W.Cast(WPred.CastPosition);
                                }
                            }
                        }
                        else
                        {
                            var WPred = W.GetPrediction(target, true);

                            if (WPred.Hitchance >= HitChance.VeryHigh)
                            {
                                W.Cast(WPred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void LaneLogic()
        {
            if (Me.ManaPercent >= Menu["Morgana_LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var Minions = GetMinions(Me.Position, W.Range);

                if (Minions.Count() > 0)
                {
                    if (Q.IsReady() && Menu["Morgana_LaneClear"]["Q"])
                    {
                        foreach (var min in Minions.Where(x => x.Health < Q.GetDamage(x)))
                        {
                            if (min != null)
                            {
                                if (InAutoAttackRange(min) && min.Health > Me.GetAutoAttackDamage(min))
                                {
                                    Q.Cast(min);
                                }
                                else if (!InAutoAttackRange(min) && min.IsValidTarget(Q.Range))
                                {
                                    Q.Cast(min);
                                }
                            }
                        }
                    }

                    if (W.IsReady() && Menu["Morgana_LaneClear"]["W"].GetValue<MenuSliderButton>().BValue)
                    {
                        var WFarm = W.GetCircularFarmLocation(Minions, W.Width);

                        if (WFarm.MinionsHit >= Menu["Morgana_LaneClear"]["W"].GetValue<MenuSliderButton>().SValue)
                        {
                            W.Cast(WFarm.Position);
                        }
                    }
                }
            }
        }

        private static void JungleLogic()
        {
            if (Me.ManaPercent >= Menu["Morgana_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var Mobs = GetMobs(Me.Position, W.Range);

                if (Mobs.Count() > 0)
                {
                    var mob = Mobs.FirstOrDefault();

                    if (Menu["Morgana_JungleClear"]["Q"] && Q.IsReady())
                    {
                        Q.Cast(mob);
                    }

                    if (Menu["Morgana_JungleClear"]["W"] && W.IsReady())
                    {
                        W.Cast(mob.Position);
                    }
                }
            }
        }

        private static void KillStealLogic()
        {
            if (Menu["Morgana_KillSteal"]["Q"] && Q.IsReady())
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (CheckTarget(qt))
                {
                    Q.Cast(qt);
                    return;
                }
            }

            if (Menu["Morgana_KillSteal"]["W"] && W.IsReady())
            {
                var wt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range) && x.Health < W.GetDamage(x)).FirstOrDefault();

                if (CheckTarget(wt))
                {
                    W.Cast(wt.ServerPosition);
                    return;
                }
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Args.IsDirectedToPlayer && Menu["Morgana_Misc"]["QGap"] &&
                Q.IsReady() && Menu["Morgana_Misc"][Args.Sender.ChampionName.ToLower()])
            {
                Q.Cast(Args.Sender);
            }
        }

        private static void OnInterruptableTarget(object obj, Events.InterruptableTargetEventArgs Args)
        {
            var target = Args.Sender;

            if (Menu["Morgana_Misc"]["QInt"] && target.IsEnemy && target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                if (Args.DangerLevel >= LeagueSharp.Data.Enumerations.DangerLevel.High ||
                    target.IsCastingInterruptableSpell())
                {
                    Q.Cast(target);
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Morgana_Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Morgana_Draw"]["W"] && W.IsReady())
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Morgana_Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Morgana_Draw"]["R"] && R.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Morgana_Draw"]["FlashR"])
                Render.Circle.DrawCircle(Me.Position, R.Range + 425f, System.Drawing.Color.BlueViolet);

            if (Menu["Morgana_Draw"]["DrawBurst"])
            {
                var text = "";

                if (Menu["Morgana_Combo"]["FlashR"].GetValue<MenuKeyBind>().Active)
                    text = "On";
                if (!Menu["Morgana_Combo"]["FlashR"].GetValue<MenuKeyBind>().Active)
                    text = "Off";

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "Flash R (" + Menu["Morgana_Combo"]["FlashR"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 40, System.Drawing.Color.Yellow, text);
            }

            if (Menu["Morgana_Draw"]["DrawDamage"])
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg((float)GetDamage(e), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }
    }
}
