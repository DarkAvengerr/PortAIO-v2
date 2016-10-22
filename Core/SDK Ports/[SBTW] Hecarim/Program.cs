using EloBuddy;
using LeagueSharp.SDK;
namespace Flowers_Hecarim
{
    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Linq;

    internal class Program
    {
        private static AIHeroClient Me;
        private static Menu Menu;
        private static Spell Q, W, E, R;
        private static SpellSlot Ignite = SpellSlot.Unknown;
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
            if (GameObjects.Player.ChampionName.ToLower() != "hecarim")
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
            Q = new Spell(SpellSlot.Q, 350f);
            W = new Spell(SpellSlot.W, 525f);
            E = new Spell(SpellSlot.E, 1000f);//Cast E First Search Range
            R = new Spell(SpellSlot.R, 1230f);

            R.SetSkillshot(0.25f, 300f, 1200f, false, SkillshotType.SkillshotCircle);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        private static void LoadMenu()
        {
            Menu = new Menu("Hecarim - The Shadow Approaches", "Hecarim - The Shadow Approaches", true).Attach();

            var ComboMenu = Menu.Add(new Menu("Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuBool("RSolo", "Use R | 1v1 Mode", true));
                ComboMenu.Add(new MenuSlider("RCount", "Use R | Counts Enemies >=", 2, 1, 5));
                ComboMenu.Add(new MenuBool("Ignite", "Use Ignite", true));
                ComboMenu.Add(new MenuBool("Item", "Use Item", true));
            }

            var HarassMenu = Menu.Add(new Menu("Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= %", 60));
                HarassMenu.Add(new MenuKeyBind("Auto", "Auto Q Harass", System.Windows.Forms.Keys.G, KeyBindType.Toggle));
                HarassMenu.Add(new MenuSlider("AutoMana", "Auto Harass Mode | Min ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuSliderButton("Q", "Use Q | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSliderButton("W", "Use W | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= %", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuBool("Item", "Use Item", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= %", 20));
            }

            var KillStealMenu = Menu.Add(new Menu("KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("W", "Use W", true));
                KillStealMenu.Add(new MenuSliderButton("R", "Use R | If Target Distance >=", 600, 100, (int)R.Range, true));
                KillStealMenu.Add(new MenuSeparator("RList", "R List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => KillStealMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var FleeMenu = Menu.Add(new Menu("Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("W", "Use W", true));
                FleeMenu.Add(new MenuBool("E", "Use E", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var ItemMenu = Menu.Add(new Menu("Items", "Items"));
            {
                ItemMenu.Add(new MenuBool("Youmuus", "Use Youmuus", true));
                ItemMenu.Add(new MenuBool("Cutlass", "Use Cutlass", true));
                ItemMenu.Add(new MenuBool("Botrk", "Use Botrk", true));
                ItemMenu.Add(new MenuBool("Hydra", "Use Hydra", true));
                ItemMenu.Add(new MenuBool("Tiamat", "Use Tiamat", true));
            }

            var SkinMenu = Menu.Add(new Menu("Skin", "Skin"));
            {
                SkinMenu.Add(new MenuBool("Enable", "Enabled", false));
                SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", new[] { "Classic", "Blood Knight", "Reaper", "Headless", "Arcade", "Elderwood" }));
            }

            var MiscMenu = Menu.Add(new Menu("Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("EGap", "Use E Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("EInt", "Use E Interrupt Spell", true));
                MiscMenu.Add(new MenuBool("RInt", "Use R Interrupt Spell", true));
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
                DrawMenu.Add(new MenuBool("Auto", "Draw Auto Q Status", true));
            }
        }

        private static void LoadEvent()
        {
            Variables.Orbwalker.OnAction += OnAction;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Events.OnGapCloser += OnGapCloser;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                {
                    var target = Variables.TargetSelector.GetTarget(R.Range, DamageType.Physical);

                    if (target != null && target.IsHPBarRendered)
                    {
                        if (Menu["Combo"]["W"] && Q.IsReady() && target.IsValidTarget(W.Range) && AutoAttack.IsAutoAttack(Me.ChampionName))
                        {
                            W.Cast();
                        }

                        Item(target);
                    }
                }

                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
                {
                    var Mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(W.Range) && !GameObjects.JungleSmall.Contains(x)).ToList();

                    if (Mobs.Count() > 0)
                    {
                        var mob = Mobs.FirstOrDefault();

                        if (Me.ManaPercent >= Menu["JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                        {
                            if (Menu["JungleClear"]["W"] && W.IsReady() && mob.IsValidTarget(W.Range) && AutoAttack.IsAutoAttack(Me.ChampionName))
                            {
                                W.Cast();
                            }
                        }

                        if (Menu["JungleClear"]["Item"])
                        {
                            if (Menu["Items"]["Hydra"] && Items.HasItem(3074) && Mobs.FirstOrDefault().IsValidTarget(AttackRange()))
                            {
                                Items.UseItem(3074, mob);
                            }

                            if (Menu["Items"]["Tiamat"] && Items.HasItem(3077) && Mobs.FirstOrDefault().IsValidTarget(AttackRange()))
                            {
                                Items.UseItem(3077, mob);
                            }
                        }
                    }
                }
            }
        }

        private static void OnInterruptableTarget(object obj, Events.InterruptableTargetEventArgs Args)
        {
            if (Args.Sender.IsEnemy)
            {
                var sender = Args.Sender as AIHeroClient;

                if (Menu["Misc"]["EInt"] && Args.DangerLevel >= DangerLevel.Medium && sender.IsValidTarget(E.Range) && E.IsReady())
                {
                    E.Cast();
                    Variables.Orbwalker.ForceTarget = sender;
                    return;
                }

                if (Menu["Misc"]["RInt"] && Args.DangerLevel >= DangerLevel.High && sender.IsValidTarget(R.Range) && R.IsReady())
                {
                    R.Cast(sender);
                    return;
                }
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Misc"]["EGap"] && Args.IsDirectedToPlayer)
            {
                var sender = Args.Sender as AIHeroClient;

                if (sender.IsEnemy && (Args.End.DistanceToPlayer() <= 200 || sender.DistanceToPlayer() <= 250) && E.IsReady())
                {
                    E.Cast();
                    Variables.Orbwalker.ForceTarget = sender;
                    return;
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Draw"]["W"] && (W.IsReady() || Me.HasBuff("HecarimW")))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.LightSeaGreen, 2);

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

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
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

            if (Menu["Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                Flee();
            }

            if (Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
            {
                AutoHarass();
            }

            KillSteal();
            Skin();
        }

        private static void Flee()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            if (Menu["Flee"]["E"] && E.IsReady())
            {
                E.Cast();
            }

            if (Menu["Flee"]["W"] && W.IsReady())
            {
                var target = Variables.TargetSelector.GetTarget(W.Range, DamageType.Physical);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        private static void Combo()
        {
            var target = Variables.TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (target != null && target.IsHPBarRendered)
            {
                if (Menu["Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target))
                {
                    Q.Cast(target);
                }

                if (Menu["Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (Menu["Combo"]["E"] && E.IsReady() && E.CanCast(target) && !InAutoAttackRange(target) && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (Menu["Combo"]["R"] && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu["Combo"]["RSolo"] && target.Health <= GetDamage(target) + Me.GetAutoAttackDamage(target) * 3 && Me.CountEnemyHeroesInRange(R.Range) == 1)
                    {
                        R.Cast(target);
                    }

                    if (R.GetPrediction(target).CastPosition.CountEnemyHeroesInRange(250) >= Menu["Combo"]["RCount"].GetValue<MenuSlider>().Value)
                    {
                        R.Cast(R.GetPrediction(target).CastPosition);
                    }
                }

                Item(target);

                if (Menu["Combo"]["Ignite"] && Ignite != SpellSlot.Unknown && Ignite.IsReady() && target.IsValidTarget(600) && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu["Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (Menu["Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void AutoHarass()
        {
            if (!Me.IsRecalling() && !Me.IsUnderEnemyTurret() && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Combo && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Hybrid && Me.ManaPercent >= Menu["Harass"]["AutoMana"].GetValue<MenuSlider>().Value)
            {
                var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void Lane()
        {
            var Minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(W.Range)).ToList();

            if (Minions.Count() > 0)
            {
                if (Me.ManaPercent >= Menu["LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["LaneClear"]["Q"].GetValue<MenuSliderButton>().BValue && Q.IsReady())
                    {
                        if (Minions.Where(x => x.IsValidTarget(Q.Range)).Count() >= Menu["LaneClear"]["Q"].GetValue<MenuSliderButton>().SValue)
                        {
                            Q.Cast();
                        }
                    }

                    if (Menu["LaneClear"]["W"].GetValue<MenuSliderButton>().BValue && W.IsReady())
                    {
                        if (Minions.Where(x => x.IsValidTarget(W.Range)).Count() >= Menu["LaneClear"]["W"].GetValue<MenuSliderButton>().SValue)
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            var Mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(W.Range) && !GameObjects.JungleSmall.Contains(x)).ToList();

            if (Mobs.Count() > 0)
            {
                var mob = Mobs.FirstOrDefault();

                if (Me.ManaPercent >= Menu["JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["JungleClear"]["Q"] && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }

                    if (Menu["JungleClear"]["W"] && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }

                if (Menu["JungleClear"]["Item"])
                {
                    if (Menu["Items"]["Hydra"] && Items.HasItem(3074) && Mobs.FirstOrDefault().IsValidTarget(AttackRange()))
                    {
                        Items.UseItem(3074, mob);
                    }

                    if (Menu["Items"]["Tiamat"] && Items.HasItem(3077) && Mobs.FirstOrDefault().IsValidTarget(AttackRange()))
                    {
                        Items.UseItem(3077, mob);
                    }
                }
            }
        }

        private static void KillSteal()
        {
            if (Menu["KillSteal"]["Q"] && Q.IsReady())
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (qt != null && qt.IsHPBarRendered)
                {
                    Q.Cast();
                    return;
                }
            }

            if (Menu["KillSteal"]["W"] && W.IsReady())
            {
                var wt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range) && x.Health < W.GetDamage(x)).FirstOrDefault();

                if (wt != null && wt.IsHPBarRendered)
                {
                    W.Cast();
                    return;
                }
            }

            if (Menu["KillSteal"]["R"].GetValue<MenuSliderButton>().BValue && R.IsReady())
            {
                var rt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x) && Menu["KillSteal"][x.ChampionName.ToLower()] && x.DistanceToPlayer() >= Menu["KillSteal"]["R"].GetValue<MenuSliderButton>().SValue).FirstOrDefault();

                if (rt != null && rt.IsHPBarRendered)
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

        private static void Item(AIHeroClient target)
        {
            if (target != null && target.IsHPBarRendered && Menu["Combo"]["Item"] && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                if (Menu["Items"]["Youmuus"] && Items.HasItem(3142) && target.IsValidTarget(AttackRange() + 150))
                {
                    Items.UseItem(3142);
                }

                if (Menu["Items"]["Cutlass"] && Items.HasItem(3144) && target.IsValidTarget(AttackRange()))
                {
                    Items.UseItem(3144, target);
                }

                if (Menu["Items"]["Botrk"] && Items.HasItem(3153) && target.IsValidTarget(AttackRange()))
                {
                    Items.UseItem(3153, target);
                }

                if (Menu["Items"]["Hydra"] && Items.HasItem(3074) && target.IsValidTarget(AttackRange()))
                {
                    Items.UseItem(3074, target);
                }

                if (Menu["Items"]["Tiamat"] && Items.HasItem(3077) && target.IsValidTarget(AttackRange()))
                {
                    Items.UseItem(3077, target);
                }
            }
        }

        private static float GetDamage(AIHeroClient target)
        {
            float Damage = 0f;

            if (Q.IsReady())
            {
                Damage += Q.GetDamage(target);
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

        private static float AttackRange()
        {
            return Me.GetRealAutoAttackRange();
        }

        private static bool InAutoAttackRange(AttackableUnit target)
        {
            var baseTarget = (Obj_AI_Base)target;
            var myRange = AttackRange();

            if (baseTarget != null)
            {
                return baseTarget.IsHPBarRendered && Vector2.DistanceSquared(baseTarget.ServerPosition.ToVector2(), Me.ServerPosition.ToVector2()) <= myRange * myRange;
            }

            return target.IsValidTarget() && Vector2.DistanceSquared(target.Position.ToVector2(), Me.ServerPosition.ToVector2()) <= myRange * myRange;
        }
    }
}
