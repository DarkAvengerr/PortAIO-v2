using EloBuddy;
using LeagueSharp.SDK;
namespace Flowers__Illaoi
{
    using LeagueSharp;
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
            Bootstrap.Init();

            if (GameObjects.Player.ChampionName.ToLower() != "illaoi")
            {
                return;
            }

            Me = GameObjects.Player;

            Chat.Print(Me.ChampionName + " : This is Old Version and i dont update it anymore, Please Use Flowers' Series!");

            LoadSpell();
            LoadMenu();
            LoadEvent();
        }

        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            W = new Spell(SpellSlot.W, 400f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 450f);

            Q.SetSkillshot(0.75f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.066f, 50f, 1900f, true, SkillshotType.SkillshotLine);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        private static void LoadMenu()
        {
            Menu = new Menu("Illaoi - The Kraken Priestess", "Illaoi - The Kraken Priestess", true).Attach();
            Menu.Add(new MenuSeparator("OLD", "This Is Old Version and i dont update it"));
            Menu.Add(new MenuSeparator("OLD1", "Please Use Flowers' Series"));
            var ComboMenu = Menu.Add(new Menu("Combo", "Combo"));
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
                ComboMenu.Add(new MenuBool("Ignite", "Use Ignite", true));
                ComboMenu.Add(new MenuBool("Item", "Use Item", true));
            }

            var HarassMenu = Menu.Add(new Menu("Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("W", "Use W", true));
                HarassMenu.Add(new MenuBool("WOutRange", "Use W | Only Out of Attack Range", true));
                HarassMenu.Add(new MenuBool("E", "Use E", true));
                HarassMenu.Add(new MenuBool("Ghost", "Attack Ghost", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuSliderButton("Q", "Use Q | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= %", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuBool("Item", "Use Item", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= %", 60));
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

            var EBlacklist = Menu.Add(new Menu("EBlackList", "E BlackList"));
            {
                EBlacklist.Add(new MenuSeparator("Adapt", "Only Adapt to Harass & KillSteal & Anti GapCloser Mode!"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => EBlacklist.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, false)));
                }
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
                SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", new[] { "Classic", "Void Bringer" }));
            }

            var MiscMenu = Menu.Add(new Menu("Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("EGap", "Use E Anti GapCloset", true));
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            LeagueSharp.SDK.Utils.DelayAction.Add(5000, () => Variables.Orbwalker.Enabled = true);
        }

        private static void LoadEvent()
        {
            Events.OnGapCloser += OnGapCloser;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Variables.Orbwalker.OnAction += OnAction;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Misc"]["EGap"] && Args.IsDirectedToPlayer)
            {
                var sender = Args.Sender as AIHeroClient;

                if (sender.IsEnemy && (Args.End.DistanceToPlayer() <= 200 || sender.DistanceToPlayer() <= 250) && !Menu["EBlackList"][sender.ChampionName.ToLower()])
                {
                    E.Cast(sender);
                }
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                var target = Variables.TargetSelector.GetTarget(W.Range, DamageType.Physical);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (Menu["Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        if (Menu["Combo"]["WOutRange"] && !InAutoAttackRange(target))
                        {
                            W.Cast();
                        }
                        else if (!Menu["Combo"]["WOutRange"])
                        {
                            W.Cast();
                        }

                        if (Menu["Combo"]["WUlt"] && Me.HasBuff("IllaoiR"))
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void OnAction(object sender, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                {
                    var target = Variables.TargetSelector.GetTarget(W.Range, DamageType.Physical);

                    if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                    {
                        if (Menu["Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                        {
                            if (Menu["Combo"]["WOutRange"] && !InAutoAttackRange(target))
                            {
                                W.Cast();
                            }

                            if (Menu["Combo"]["WUlt"] && Me.HasBuff("IllaoiR"))
                            {
                                W.Cast();
                            }
                        }
                    }
                }

                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid && !Me.IsUnderEnemyTurret())
                {
                    if (Me.ManaPercent >= Menu["Harass"]["Mana"].GetValue<MenuSlider>().Value)
                    {
                        var target = Variables.TargetSelector.GetTarget(W.Range, DamageType.Physical);

                        if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                        {
                            if (Menu["Harass"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                            {
                                if (Menu["Harass"]["WOutRange"] && !InAutoAttackRange(target))
                                {
                                    W.Cast();
                                }
                                else if (!Menu["Harass"]["WOutRange"])
                                {
                                    W.Cast();
                                }
                            }
                        }
                    }
                }

                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
                {
                    if (Me.ManaPercent >= Menu["JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                    {
                        var Mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && !GameObjects.JungleSmall.Contains(x)).ToList();

                        if (Mobs.Count() > 0)
                        {
                            if (Menu["JungleClear"]["W"] && W.IsReady() && !AutoAttack.IsAutoAttack(Me.ChampionName))
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

            KillSteal();
            Skin();
            Item();
        }

        private static void Combo()
        {
            var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var Ghost = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsHPBarRendered && x.IsValidTarget(Q.Range)).FirstOrDefault(x => x.HasBuff("illaoiespirit"));

            if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
            {
                if (Menu["Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target) && !((W.IsReady() || Me.HasBuff("IllaoiW")) && target.IsValidTarget(W.Range)))
                {
                    Q.Cast(target);
                }

                if (Menu["Combo"]["E"] && E.IsReady() && !InAutoAttackRange(target) && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (Menu["Combo"]["R"] && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu["Combo"]["RSolo"] && target.Health <= GetDamage(target) + Me.GetAutoAttackDamage(target) * 3 && Me.CountEnemyHeroesInRange(R.Range) == 1)
                    {
                        R.Cast(target);
                    }

                    if (Me.CountEnemyHeroesInRange(R.Range - 50) >= Menu["Combo"]["RCount"].GetValue<MenuSlider>().Value)
                    {
                        R.Cast();
                    }
                }

                if (Menu["Combo"]["Ignite"] && Ignite != SpellSlot.Unknown && Ignite.IsReady() && target.IsValidTarget(600) && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }
            }
            else if (target == null && Ghost != null)
            {
                if (Ghost != null && Q.IsReady() && Menu["Combo"]["Q"] && Menu["Combo"]["QGhost"])
                {
                    Q.Cast(Ghost);
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu["Harass"]["Mana"].GetValue<MenuSlider>().Value && !Me.IsUnderEnemyTurret())
            {
                var target = Variables.TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var Ghost = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsHPBarRendered && x.IsValidTarget(Q.Range)).FirstOrDefault(x => x.HasBuff("illaoiespirit"));

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    if (Menu["Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }

                    if (Menu["Harass"]["W"] && W.IsReady() && Menu["Harass"]["WOutRange"] && !InAutoAttackRange(target) && target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }

                    if (Menu["Harass"]["E"] && E.IsReady() && E.CanCast(target) && !Menu["EBlackList"][target.ChampionName.ToLower()] && !InAutoAttackRange(target) && target.IsValidTarget(E.Range))
                    {
                        E.Cast(target);
                    }
                }
                else if (target == null && Ghost != null)
                {
                    if (Q.IsReady() && Menu["Harass"]["Q"])
                        Q.Cast(Ghost);

                    if (W.IsReady() && Menu["Harass"]["W"])
                    {
                        W.Cast();
                    }

                    if (Menu["Harass"]["Ghost"])
                    {
                        Variables.Orbwalker.ForceTarget = Ghost;
                    }
                }
            }
        }

        private static void Lane()
        {
            if (Me.ManaPercent >= Menu["LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                if (Menu["LaneClear"]["Q"].GetValue<MenuSliderButton>().BValue && Q.IsReady())
                {
                    var Minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)).ToList();

                    if (Minions.Count() > 0)
                    {
                        var QFarm = Q.GetLineFarmLocation(Minions, Q.Width);

                        if (QFarm.MinionsHit >= Menu["LaneClear"]["Q"].GetValue<MenuSliderButton>().SValue)
                        {
                            Q.Cast(QFarm.Position);
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
                if (Me.ManaPercent >= Menu["JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["JungleClear"]["Q"] && Q.IsReady() && !AutoAttack.IsAutoAttack(Me.ChampionName))
                    {
                        Q.Cast(Mobs.FirstOrDefault());
                    }
                }

                if (Menu["JungleClear"]["Item"])
                {
                    if (Menu["Items"]["Hydra"] && Items.HasItem(3074) && Mobs.FirstOrDefault().IsValidTarget(AttackRange()))
                    {
                        Items.UseItem(3074, Mobs.FirstOrDefault());
                    }

                    if (Menu["Items"]["Tiamat"] && Items.HasItem(3077) && Mobs.FirstOrDefault().IsValidTarget(AttackRange()))
                    {
                        Items.UseItem(3077, Mobs.FirstOrDefault());
                    }
                }
            }
        }

        private static void KillSteal()
        {
            if (Menu["KillSteal"]["Q"] && Q.IsReady())
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (qt != null)
                {
                    Q.Cast(qt);
                    return;
                }
            }

            if (Menu["KillSteal"]["E"] && E.IsReady())
            {
                var et = GameObjects.EnemyHeroes.Where(x => !Menu["EBlackList"][x.ChampionName.ToLower()] && x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x)).FirstOrDefault();

                if (et != null)
                {
                    E.Cast(et);
                    return;
                }
            }

            if (Menu["KillSteal"]["R"] && R.IsReady())
            {
                var rt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range - 50) && x.Health < R.GetDamage(x) && Menu["KillSteal"][x.ChampionName.ToLower()]).FirstOrDefault();

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

        private static void Item()
        {
            if (Menu["Combo"]["Item"] && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                var target = Variables.TargetSelector.GetTarget(600, DamageType.Physical);

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

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Draw"]["Q"].GetValue<MenuBool>() && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Draw"]["W"].GetValue<MenuBool>() && (W.IsReady() || Me.HasBuff("IllaoiW")))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (Menu["Draw"]["E"].GetValue<MenuBool>() && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightYellow, 2);

            if (Menu["Draw"]["R"].GetValue<MenuBool>() && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Draw"]["DrawDamage"].GetValue<MenuBool>())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg(GetDamage(target), new SharpDX.ColorBGRA(255, 204, 0, 170));
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

        public static bool InAutoAttackRange(AttackableUnit target)
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
