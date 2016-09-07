using EloBuddy;
using LeagueSharp.SDK;
namespace Flowers_Vladimir
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
        private static AIHeroClient Me;
        private static Menu Menu;
        private static Spell Q, W, E, R;
        private static SpellSlot Ignite = SpellSlot.Unknown;
        //private static int LastECast = 0;
        private static HpBarDraw DrawHpBar = new HpBarDraw();
        private static string[] AutoEnableList =
        {
             "Annie", "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana", "Evelynn", "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus",
             "Kassadin", "Katarina", "Kayle", "Kennen", "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna",
             "Ryze", "Sion", "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra", "Velkoz", "Azir", "Ekko",
             "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jayce", "Jinx", "KogMaw", "Lucian", "MasterYi", "MissFortune", "Quinn", "Shaco", "Sivir",
             "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Yasuo", "Zed", "Kindred", "AurelionSol"
        };

        public static void Main()
        {
            Bootstrap.Init(null);
            if (GameObjects.Player.ChampionName.ToLower() != "vladimir")
            {
                return;
            }

            Me = GameObjects.Player;

            LoadSpell();
            LoadMenu();
            LoadEvent();
        }

        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 350f);
            E = new Spell(SpellSlot.E, 610f);
            R = new Spell(SpellSlot.R, 625f);

            Q.SetTargetted(0.25f, float.MaxValue);
            R.SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        private static void LoadMenu()
        {
            Menu = new Menu("Vladimir - The Rivers Will Run Red", "Vladimir - The Rivers Will Run Red", true).Attach();

            var ComboMenu = Menu.Add(new Menu("Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", false));
                ComboMenu.Add(new MenuBool("WE", "Use W | Only E Is Charging!", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuSlider("ECharging", "Use E | Charging Max Range", 550, 300, 600));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuSlider("RCount", "Use R | Counts Enemies >=", 2, 1, 5));
                ComboMenu.Add(new MenuBool("Ignite", "Use Ignite", true));
                ComboMenu.Add(new MenuBool("Attack", "Disable AutoAttack In Combo Mode", false));
            }

            var HarassMenu = Menu.Add(new Menu("Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuKeyBind("Auto", "Auto Q Harass", System.Windows.Forms.Keys.G, KeyBindType.Toggle));
            }

            var LaneClearMenu = Menu.Add(new Menu("LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuBool("QFrenzy", "Use Frenzy Q", true));
                LaneClearMenu.Add(new MenuBool("QLh", "Use Q | Only LastHit", true));
                LaneClearMenu.Add(new MenuSliderButton("E", "Use E | Min Hit Count >= ", 3, 1, 5, true));
            }

            var JungleClearMenu = Menu.Add(new Menu("JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
            }

            var LastHitMenu = Menu.Add(new Menu("LastHit", "Last Hit"));
            {
                LastHitMenu.Add(new MenuBool("Q", "Use Q", true));
                LastHitMenu.Add(new MenuBool("QFrenzy", "Use Frenzy Q", true));
                LastHitMenu.Add(new MenuBool("QHarass", "Use Q | Work in Harass Mode", true));
            }

            var KillStealMenu = Menu.Add(new Menu("KillSteal", "Kill Steal"));
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

            var FleeMenu = Menu.Add(new Menu("Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("Q", "Use Q", false));
                FleeMenu.Add(new MenuBool("W", "Use W", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var SkinMenu = Menu.Add(new Menu("Skin", "Skin"));
            {
                SkinMenu.Add(new MenuBool("Enable", "Enabled", false));
                SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", new[] { "Classic", "Count", "Marquis", "Nosferatu", "Vandal", "Blood Lord", "Soulstealer", "Academy" }));
            }

            var MiscMenu = Menu.Add(new Menu("Misc", "Misc"));
            {
                MiscMenu.Add(new MenuSliderButton("WGap", "Use W Anti GapCloset", 40, 0, 100, true));
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
                DrawMenu.Add(new MenuBool("Auto", "Draw Auto Q Status", true));
            }
        }

        private static void LoadEvent()
        {
            //Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Variables.Orbwalker.OnAction += OnAction;
            Events.OnGapCloser += OnGapCloser;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        //private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        //{
        //    if (sender.IsMe && args.Slot == SpellSlot.E)
        //    {
        //        LastECast = Variables.TickCount;
        //    }
        //}

        private static void OnAction(object sender, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo && Menu["Combo"]["Attack"])
            {
                Args.Process = false;
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Misc"]["WGap"].GetValue<MenuSliderButton>().BValue && W.IsReady() && Args.IsDirectedToPlayer)
            {
                if (Args.Sender.DistanceToPlayer() <= 250 || Args.End.DistanceToPlayer() <= 200)
                {
                    if (Me.HealthPercent <= Menu["Misc"]["WGap"].GetValue<MenuSliderButton>().SValue)
                        W.Cast();
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Me.HasBuff("VladimirE") || Me.HasBuff("VladimirSanguinePool"))
            {
                Variables.Orbwalker.Move(Game.CursorPos);
                Variables.Orbwalker.AttackState = false;
            }
            else
            {
                Variables.Orbwalker.AttackState = true;
            }

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
            {
                Harass();
            }

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
            {
                Lane();
                Jungle();
            }

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LastHit || (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid && Menu["LastHit"]["QHarass"]))
            {
                LastHit();
            }

            if (Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
            {
                AutoHarass();
            }

            if (Menu["Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                Flee();
            }

            KillSteal();
            Skin();
        }

        private static void Combo()
        {
            var target = Variables.TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (target != null && target.IsHPBarRendered)
            {
                if (Menu["Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target) && !E.IsCharging)
                {
                    Q.Cast(target);
                }

                if (Menu["Combo"]["E"] && target.IsValidTarget(E.Range) && E.IsReady())
                {
                    if (E.IsCharging && target.DistanceToPlayer() >= Menu["Combo"]["ECharging"].GetValue<MenuSlider>().Value)
                    {
                        Me.Spellbook.CastSpell(E.Slot, false);
                    }
                    else if (!Me.HasBuff("VladimirE"))
                    {
                        E.StartCharging();
                    }
                }

                // W Logic need to make more? o
                if (Menu["Combo"]["W"] && W.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (Menu["Combo"]["WE"] && Me.HasBuff("VladimirE"))
                    {
                        W.Cast();
                    }
                    else if (!Menu["Combo"]["WE"] && target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }

                if (Menu["Combo"]["R"] && R.IsReady() && target.IsValidTarget(R.Range) && !E.IsCharging)
                {
                    if (target.Health <= GetDamage(target) + Me.GetAutoAttackDamage(target))
                    {
                        R.Cast(target);
                    }

                    if (R.GetPrediction(target).CastPosition.CountEnemyHeroesInRange(375f) >= Menu["Combo"]["RCount"].GetValue<MenuSlider>().Value)
                    {
                        R.Cast(R.GetPrediction(target).CastPosition);
                    }
                }

                if (Menu["Combo"]["Ignite"] && Ignite != SpellSlot.Unknown && Ignite.IsReady() && target.IsValidTarget(600) && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }
            }
        }

        private static void Harass()
        {
            var target = Variables.TargetSelector.GetTarget(Q);

            if (target != null && target.IsHPBarRendered)
            {
                if (Menu["Harass"]["Q"] && Q.IsReady() && Q.CanCast(target))
                {
                    Q.Cast(target);
                }
            }
        }

        private static void Lane()
        {
            var Minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)).ToList();

            if (Minions.Count() > 0)
            {
                if (Menu["LaneClear"]["Q"] && Q.IsReady() && !E.IsCharging)
                {
                    if (!Menu["LaneClear"]["QFrenzy"] && Me.HasBuff("vladimirqfrenzy"))
                    {
                        return;
                    }

                    if (!Menu["LaneClear"]["QLh"])
                    {
                        Q.Cast(Minions.FirstOrDefault());
                    }
                    else if (Menu["LaneClear"]["QLh"])
                    {
                        var min = Minions.FirstOrDefault(x => x.Health < Q.GetDamage(x) && x.Health > Me.GetAutoAttackDamage(x));

                        if (min != null)
                        {
                            Q.Cast(min);
                        }
                    }
                }

                if (Menu["LaneClear"]["E"].GetValue<MenuSliderButton>().BValue && E.IsReady())
                {
                    if (Minions.Count() >= Menu["LaneClear"]["E"].GetValue<MenuSliderButton>().SValue)
                    {
                        //if (E.IsCharging)
                        //{
                        //    Me.Spellbook.CastSpell(E.Slot, false);
                        //}
                        //else 
                        if (!Me.HasBuff("VladimirE"))
                        {
                            E.StartCharging();
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            var Mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && !GameObjects.JungleSmall.Contains(x)).ToList();

            if (Mobs.Count() > 0)
            {
                if (Menu["JungleClear"]["Q"] && Q.IsReady() && !E.IsCharging)
                {
                    Q.Cast(Mobs.FirstOrDefault());
                }

                if (Menu["JungleClear"]["E"] && E.IsReady())
                {
                    //if (E.IsCharging && IsChargingMaxE())
                    //{
                    //    Me.Spellbook.UpdateChargeableSpell(E.Slot, Me.Position, true, false);
                    //}
                    //else
                    if (!Me.HasBuff("VladimirE"))
                    {
                        E.StartCharging();
                    }
                }
            }
        }

        private static void LastHit()
        {
            var Minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)).ToList();

            if (Minions.Count() > 0)
            {
                if (Menu["LastHit"]["Q"] && Q.IsReady())
                {
                    var min = Minions.FirstOrDefault(x => x.Health < Q.GetDamage(x) && x.Health > Me.GetAutoAttackDamage(x));
                    var target = Variables.TargetSelector.GetTarget(Q);

                    if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
                    {
                        if (target != null && target.IsHPBarRendered && Me.HasBuff("vladimirqfrenzy") && Menu["Harass"]["Q"])
                        {
                            Q.Cast(target);
                        }
                        else if (target == null && min != null)
                        {
                            if (!Menu["LastHit"]["QFrenzy"] && Me.HasBuff("vladimirqfrenzy"))
                            {
                                return;
                            }

                            Q.Cast(min);
                        }
                    }
                    else
                    {
                        if (min != null)
                        {
                            if (!Menu["LastHit"]["QFrenzy"] && Me.HasBuff("vladimirqfrenzy"))
                            {
                                return;
                            }

                            Q.Cast(min);
                        }
                    }
                }
            }
        }

        private static void AutoHarass()
        {
            var target = Variables.TargetSelector.GetTarget(Q);

            if (target != null && target.IsHPBarRendered)
            {
                if (Q.IsReady() && Q.CanCast(target))
                {
                    Q.Cast(target);
                }
            }
        }

        private static void Flee()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            var Qtarget = Variables.TargetSelector.GetTarget(Q);
            var Wtarget = Variables.TargetSelector.GetTarget(W.Range, DamageType.Magical);

            if (Menu["Flee"]["W"] && Wtarget != null && Wtarget.IsHPBarRendered && W.IsReady())
            {
                W.Cast();
            }

            if (Menu["Flee"]["Q"] && Qtarget != null && Qtarget.IsHPBarRendered && Q.IsReady())
            {
                if (Me.HealthPercent < 60 && Qtarget.DistanceToPlayer() > Qtarget.GetRealAutoAttackRange())
                    Q.Cast(Qtarget);
            }
        }

        private static void KillSteal()
        {
            if (Menu["KillSteal"]["Q"] && Q.IsReady() && !E.IsCharging)
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (qt != null)
                {
                    Q.Cast(qt);
                    return;
                }
            }

            if (Menu["KillSteal"]["E"] && E.IsReady() && !E.IsCharging)
            {
                var et = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x)).FirstOrDefault();

                if (et != null)
                {
                    E.Cast(et);
                    return;
                }
            }

            if (Menu["KillSteal"]["R"] && R.IsReady())
            {
                var rt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x) && Menu["KillSteal"][x.ChampionName.ToLower()]).FirstOrDefault();

                if (rt != null)
                {
                    R.Cast(rt);
                    return;
                }
            }
        }

        private static void Skin()
        {
            if (Menu["Skin"]["Enable"])
            {
            }
            else if (!Menu["Skin"]["Enable"])
            {
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (Menu["Draw"]["R"] && R.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Draw"]["DrawDamage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg(GetDamage(target), new ColorBGRA(255, 204, 0, 170));
                }
            }

            if (Menu["Draw"]["Auto"].GetValue<MenuBool>())
            {
                var text = "";

                if (Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
                {
                    text = "On";
                }
                else
                {
                    text = "Off";
                }

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "Auto Q (" + Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 40, System.Drawing.Color.Yellow, text);
            }
        }

        private static float GetDamage(AIHeroClient target)
        {
            float Damage = 0f;

            if (Q.IsReady())
            {
                if (Me.HasBuff("vladimirqfrenzy"))
                {
                    Damage += Q.GetDamage(target) * 2;
                }
                else
                {
                    Damage += Q.GetDamage(target);
                }
            }

            if (W.IsReady())
            {
                Damage += W.GetDamage(target);
            }

            if (E.IsReady())
            {
                Damage += E.GetDamage(target);
            }

            if (R.IsReady())
            {
                Damage += R.GetDamage(target);
            }

            return Damage;
        }

        //private static bool IsChargingMaxE()
        //{
        //    if (Me.HasBuff("VladimirE") && E.IsCharging)
        //    {
        //        if (Variables.TickCount - LastECast >= 850)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}
    }
}
