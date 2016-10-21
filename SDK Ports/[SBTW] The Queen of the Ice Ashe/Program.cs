using EloBuddy;
using LeagueSharp.SDK;
namespace Flowers_Ashe
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Program
    {
        private static Menu Menu;
        private static Spell Q, W, E, R;
        private static AIHeroClient Me;
        private static int SkinID;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        private static List<BuffType> DebuffTypes = new List<BuffType>();
        private static int UseCleanTime, CleanID, Dervish = 3137, Mercurial = 3139, Quicksilver = 3140, Mikaels = 3222;
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
            if (GameObjects.Player.ChampionName.ToLower() != "ashe")
                return;

            Me = GameObjects.Player;

            InitSpells();
            InitMenu();
            InitEvents();
        }

        private static void InitSpells()
        {
            Q = new Spell(SpellSlot.Q, Me.AttackRange);
            W = new Spell(SpellSlot.W, 1255f);
            R = new Spell(SpellSlot.R, 2000f);

            W.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.25f, 130f, 1600f, true, SkillshotType.SkillshotLine);
        }

        private static void InitMenu()
        {
            Menu = new Menu("Flowers_Ashe", "The Queen of the Ice - Ashe", true).Attach();

            var ComboMenu = Menu.Add(new Menu("Combo", "Combo"));
            ComboMenu.Add(new MenuBool("Q", "Use Q", true));
            ComboMenu.Add(new MenuBool("SaveMana", "Save Mana To Cast W&R", true));
            ComboMenu.Add(new MenuBool("W", "Use W", true));
            ComboMenu.Add(new MenuBool("R", "Use R", true));

            var HarassMenu = Menu.Add(new Menu("Harass", "Harass"));
            HarassMenu.Add(new MenuBool("W", "Use W", true));
            HarassMenu.Add(new MenuSlider("WMana", "Min Harass Mana", 60));

            var ClearMenu = Menu.Add(new Menu("Clear", "Clear"));
            ClearMenu.Add(new MenuSeparator("LaneClear Settings", "LaneClear Settings"));
            ClearMenu.Add(new MenuBool("LCW", "Use W", true));
            ClearMenu.Add(new MenuSlider("LCWMana", "If Player ManaPercent >= %", 50, 0, 100));
            ClearMenu.Add(new MenuSlider("LCWCount", "If W CanHit Counts >= ", 3, 1, 10));
            ClearMenu.Add(new MenuSeparator("JungleClear Settings", "JungleClear Settings"));
            ClearMenu.Add(new MenuSliderButton("JCQ", "Use Q | If Player ManaPercent >= %", 30, 0, 100, true));
            ClearMenu.Add(new MenuSliderButton("JCW", "Use W | If Player ManaPercent >= %", 30, 0, 100, true));

            var MiscMenu = Menu.Add(new Menu("Misc", "Misc"));
            MiscMenu.Add(new MenuSeparator("KillSteal", "KillSteal"));
            MiscMenu.Add(new MenuBool("KSW", "KillSteal W", true));
            MiscMenu.Add(new MenuBool("KSR", "KillSteal R", true));
            MiscMenu.Add(new MenuSeparator("KillStealRList", "KillSteal R List:"));
            if (GameObjects.EnemyHeroes.Any())
            {
                GameObjects.EnemyHeroes.ForEach(i => MiscMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
            }
            MiscMenu.Add(new MenuSeparator("R Settings", "R Settings"));
            MiscMenu.Add(new MenuBool("AutoR", "Auto", true));
            MiscMenu.Add(new MenuSliderButton("AntiGapCloser", "Anti GapCloser | If Player HealthPercent <= %", 20, 0, 80, true));
            MiscMenu.Add(new MenuBool("Interrupt", "Interrupt Danger Spells", true));
            MiscMenu.Add(new MenuKeyBind("R", "R Key", System.Windows.Forms.Keys.T, KeyBindType.Press));

            var ItemMenu = Menu.Add(new Menu("Items", "Items"));
            ItemMenu.Add(new MenuBool("Youmuus", "Use Youmuus", true));
            ItemMenu.Add(new MenuBool("Cutlass", "Use Cutlass", true));
            ItemMenu.Add(new MenuBool("Botrk", "Use Botrk", true));
            ItemMenu.Add(new MenuBool("Clean", "Use QSS", true));

            var SkinMenu = Menu.Add(new Menu("Skin", "Skin"));
            SkinMenu.Add(new MenuBool("Enable", "Enabled", false));
            SkinMenu.Add(new MenuList<string>("SkinName", "Select Skin", new[] { "Classic", "Freljord", "Sherwood Forest", "Woad", "Queen", "Amethyst", "Heartseeker", "Marauder" }));

            var DrawMenu = Menu.Add(new Menu("Draw", "Drawings"));
            DrawMenu.Add(new MenuBool("W", "W"));
            DrawMenu.Add(new MenuBool("Damage", "Draw Combo Damage", true));
        }

        private static void InitEvents()
        {
            Events.OnGapCloser += OnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;
        }

        private static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs Args)
        {
            if (Menu["Misc"]["Interrupt"] && R.IsReady() && Args.Sender.IsEnemy && Args.DangerLevel >= LeagueSharp.Data.Enumerations.DangerLevel.High && Args.Sender.IsValidTarget(R.Range))
            {
                var RPred = R.GetPrediction(Args.Sender);

                if (RPred.Hitchance >= HitChance.VeryHigh)
                {
                    R.Cast(RPred.CastPosition);
                    return;
                }
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs Args)
        {
            if (Args.IsDirectedToPlayer)
            {
                if (Menu["Misc"]["AntiGapCloser"].GetValue<MenuSliderButton>().BValue && Me.HealthPercent <= Menu["Misc"]["AntiGapCloser"].GetValue<MenuSliderButton>().SValue && R.IsReady() && Args.End.DistanceToPlayer() <= 300 && Args.Sender.IsValidTarget(R.Range))
                {
                    R.Cast(Args.Sender.Position);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            SeMiRLogic();

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                ComboLogic();
            }

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid)
            {
                HarassLogic();
            }

            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
            {
                LaneLogic();
                JungleLogic();
            }

            AutoRLogic();
            KillStealLogic();
            SkinLogic();
            ItemLogic();
        }

        private static void SeMiRLogic()
        {
            if (Menu["Misc"]["R"].GetValue<MenuKeyBind>().Active && R.IsReady())
            {
                var select = Variables.TargetSelector.GetSelectedTarget();
                var target = Variables.TargetSelector.GetTarget(R);

                if (select != null && !target.HasBuffOfType(BuffType.SpellShield) && target.IsValidTarget(R.Range))
                {
                    var RPred = R.GetPrediction(target);

                    if (RPred.Hitchance >= HitChance.VeryHigh)
                    {
                        R.Cast(RPred.CastPosition);
                        return;
                    }
                }
                else if (select == null && target != null && !target.HasBuffOfType(BuffType.SpellShield) && target.IsValidTarget(R.Range))
                {
                    var RPred = R.GetPrediction(target);

                    if (RPred.Hitchance >= HitChance.VeryHigh)
                    {
                        R.Cast(RPred.CastPosition);
                        return;
                    }
                }
            }
        }

        private static void ItemLogic()
        {
            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                var target = Variables.TargetSelector.GetTarget(600, DamageType.Physical);

                if (Menu["Items"]["Youmuus"] && Items.HasItem(3142) && target.IsValidTarget(ObjectManager.Player.AttackRange + 150))
                {
                    Items.UseItem(3142);
                }

                if (Menu["Items"]["Cutlass"] && Items.HasItem(3144) && target.IsValidTarget(ObjectManager.Player.AttackRange))
                {
                    Items.UseItem(3144, target);
                }

                if (Menu["Items"]["Botrk"] && Items.HasItem(3153) && target.IsValidTarget(ObjectManager.Player.AttackRange))
                {
                    Items.UseItem(3153, target);
                }

                if (Menu["Items"]["Clean"] && CanClean(ObjectManager.Player))
                {
                    if (CanUseDervish() || CanUseMercurial() || CanUseMikaels() || CanUseQuicksilver())
                    {
                        Items.UseItem(CleanID);
                        UseCleanTime = Variables.TickCount + 2500;
                    }
                }
            }
        }

        private static void SkinLogic()
        {
            if (Menu["Skin"]["Enable"])
            {
            }
            else if (!Menu["Skin"]["Enable"])
            {
            }
        }

        private static void ComboLogic()
        {
            if (Menu["Combo"]["W"] && W.IsReady() && !Me.HasBuff("AsheQAttack"))
            {
                var target = Variables.TargetSelector.GetTarget(W);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    var WPred = W.GetPrediction(target);

                    if (WPred.Hitchance >= HitChance.VeryHigh)
                    {
                        W.Cast(WPred.CastPosition);
                    }
                }
            }

            if (Menu["Combo"]["R"] && R.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie && x.IsHPBarRendered))
                {
                    if (target != null)
                    {
                        if (target.IsValidTarget(600) && Me.CountEnemyHeroesInRange(600) >= 3 && target.CountAllyHeroesInRange(200) >= 2)
                        {
                            var RPred = R.GetPrediction(target);

                            if (RPred.Hitchance >= HitChance.VeryHigh)
                            {
                                R.Cast(RPred.CastPosition);
                            }
                        }

                        if (target.DistanceToPlayer() > Me.AttackRange && target.DistanceToPlayer() <= 700 && target.Health > Me.GetAutoAttackDamage(target) && target.Health < R.GetDamage(target) + Me.GetAutoAttackDamage(target) && !target.HasBuffOfType(BuffType.SpellShield))
                        {
                            var RPred = R.GetPrediction(target);

                            if (RPred.Hitchance >= HitChance.VeryHigh)
                            {
                                R.Cast(RPred.CastPosition);
                            }
                        }

                        if (target.DistanceToPlayer() <= 1000 && (!target.CanMove || target.HasBuffOfType(BuffType.Stun) || R.GetPrediction(target).Hitchance == HitChance.Immobile)) // need to rewrite this part (i think i need to write more)
                        {
                            R.Cast(target);
                        }

                        if (Me.CountEnemyHeroesInRange(800) == 1 && target.IsValidTarget(800) && target.Health <= Me.GetAutoAttackDamage(target) * 4 + R.GetDamage(target))
                        {
                            var RPred = R.GetPrediction(target);

                            if (RPred.Hitchance >= HitChance.VeryHigh)
                            {
                                R.Cast(RPred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void HarassLogic()
        {
            if (Menu["Harass"]["W"] && Menu["Harass"]["WMana"].GetValue<MenuSlider>().Value <= Me.ManaPercent && W.IsReady() && !Me.HasBuff("AsheQAttack"))
            {
                var target = Variables.TargetSelector.GetTarget(W);

                if (target != null && !target.IsDead && !target.IsZombie && target.IsHPBarRendered)
                {
                    var WPred = W.GetPrediction(target);

                    if (WPred.Hitchance >= HitChance.VeryHigh)
                    {
                        W.Cast(WPred.CastPosition);
                    }
                }
            }
        }

        private static void LaneLogic()
        {
            var Minions = GameObjects.Minions.Where(x => x.IsValidTarget(W.Range) && x.IsEnemy && x.IsMinion && x.Team != GameObjectTeam.Neutral).ToList();

            if (Minions.Count() > 0)
            {
                if (Menu["Clear"]["LCW"] && W.IsReady() && Menu["Clear"]["LCWMana"].GetValue<MenuSlider>().Value <= Me.ManaPercent)
                {
                    var WFarm = W.GetLineFarmLocation(Minions);

                    if (WFarm.MinionsHit >= Menu["Clear"]["LCWCount"].GetValue<MenuSlider>().Value)
                    {
                        W.Cast(WFarm.Position);
                    }
                }
            }
        }

        private static void JungleLogic()
        {
            var Mobs = ObjectManager.Get<Obj_AI_Minion>().Where(x => !x.IsDead && !x.IsZombie && x.Team == GameObjectTeam.Neutral && x.IsValidTarget(W.Range)).ToList();

            if (Menu["Clear"]["JCW"].GetValue<MenuSliderButton>().BValue && Me.ManaPercent >= Menu["Clear"]["JCW"].GetValue<MenuSliderButton>().SValue && !Me.HasBuff("AsheQAttack") && Mobs.Count() > 0)
            {
                foreach (var mob in Mobs.Where(x => !x.CharData.BaseSkinName.ToLower().Contains("mini") && x.IsValidTarget(300)))
                {
                    if (mob != null)
                    {
                        W.Cast(mob.Position);
                    }
                }
            }
        }

        private static void AutoRLogic()
        {
            if (Menu["Misc"]["AutoR"] && R.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && !x.IsDead && !x.IsZombie && x.IsHPBarRendered))
                {
                    if (target != null)
                    {
                        if (target.DistanceToPlayer() > Me.AttackRange && target.DistanceToPlayer() <= 700 && target.Health > Me.GetAutoAttackDamage(target) && target.Health < R.GetDamage(target) + Me.GetAutoAttackDamage(target) && !target.HasBuffOfType(BuffType.SpellShield))
                        {
                            var RPred = R.GetPrediction(target);

                            if (RPred.Hitchance >= HitChance.VeryHigh)
                            {
                                R.Cast(RPred.CastPosition);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void KillStealLogic()
        {
            if (Menu["Misc"]["KSW"] && W.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range) && !x.IsDead && !x.IsZombie && x.IsHPBarRendered))
                {
                    if (target.IsValid && target.Health < W.GetDamage(target))
                    {
                        if (target.DistanceToPlayer() <= Me.AttackRange && Me.HasBuff("AsheQAttack"))
                        {
                            return;
                        }

                        var WPred = W.GetPrediction(target);

                        if (WPred.Hitchance >= HitChance.VeryHigh)
                        {
                            W.Cast(WPred.CastPosition);
                            return;
                        }
                    }
                }
            }

            if (Menu["Misc"]["KSR"] && R.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && !x.IsDead && !x.IsZombie && x.IsHPBarRendered && Menu["Misc"][x.ChampionName.ToLower()]))
                {
                    if (target.DistanceToPlayer() > 800 && target.IsValid && target.Health < R.GetDamage(target) && !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        var RPred = R.GetPrediction(target);

                        if (RPred.Hitchance >= HitChance.VeryHigh)
                        {
                            R.Cast(RPred.CastPosition);
                            return;
                        }
                    }
                }
            }
        }

        private static void Orbwalker_OnAction(object sender, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack && !Me.HasBuff("AsheQAttack"))
            {
                if (Args.Target is AIHeroClient && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                {
                    var target = (AIHeroClient)Args.Target;

                    if (Menu["Combo"]["Q"] && target.IsValidTarget(Me.AttackRange) && Q.IsReady() && Me.HasBuff("asheqcastready"))
                    {
                        if (Menu["Combo"]["SaveMana"] && Me.Mana < R.Instance.SData.Mana + W.Instance.SData.Mana + Q.Instance.SData.Mana)
                        {
                            return;
                        }

                        Q.Cast();
                        return;
                    }
                }

                if (Args.Target is Obj_AI_Minion && Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
                {
                    var target = (Obj_AI_Base)Args.Target;

                    var Mobs = ObjectManager.Get<Obj_AI_Minion>().Where(x => !x.IsDead && !x.IsZombie && x.Team == GameObjectTeam.Neutral && x.IsValidTarget(Me.AttackRange)).OrderBy(x => x.MaxHealth).ToList();

                    foreach (var mob in Mobs)
                    {
                        if (mob != null)
                        {
                            if (Menu["Clear"]["JCQ"].GetValue<MenuSliderButton>().BValue && mob.IsValidTarget(Me.AttackRange) && Q.IsReady() && Me.HasBuff("asheqcastready") && mob.Health > Me.GetAutoAttackDamage(mob) * 2)
                            {
                                if (Menu["Clear"]["JCQ"].GetValue<MenuSliderButton>().SValue < Me.ManaPercent)
                                {
                                    Q.Cast();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (Args.Type == OrbwalkingType.AfterAttack && !Me.HasBuff("AsheQAttack"))
            {
                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                {
                    var target = Variables.TargetSelector.GetTarget(Q);

                    if (Menu["Combo"]["Q"] && target.IsValidTarget(Me.AttackRange) && Q.IsReady() && Me.HasBuff("asheqcastready"))
                    {
                        if (Menu["Combo"]["SaveMana"] && Me.Mana < R.Instance.SData.Mana + W.Instance.SData.Mana + Q.Instance.SData.Mana)
                        {
                            return;
                        }

                        Q.Cast();
                        Variables.Orbwalker.ResetSwingTimer();
                        return;
                    }

                    if (Menu["Combo"]["W"] && W.IsReady() && target.IsValidTarget(Me.AttackRange))
                    {
                        var WPred = W.GetPrediction(target);

                        if (WPred.Hitchance >= HitChance.VeryHigh)
                        {
                            if (W.Cast(WPred.CastPosition))
                            {
                                Variables.Orbwalker.ResetSwingTimer();
                                return;
                            }
                        }
                    }
                }

                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
                {
                    var Mobs = ObjectManager.Get<Obj_AI_Minion>().Where(x => !x.IsDead && !x.IsZombie && x.Team == GameObjectTeam.Neutral && x.IsValidTarget(Me.AttackRange)).OrderBy(x => x.MaxHealth).ToList();

                    foreach (var mob in Mobs)
                    {
                        if (mob != null)
                        {
                            if (Menu["Clear"]["JCQ"].GetValue<MenuSliderButton>().BValue && mob.IsValidTarget(Me.AttackRange) && Q.IsReady() && Me.HasBuff("asheqcastready") && mob.Health > Me.GetAutoAttackDamage(mob) * 2)
                            {
                                if (Menu["Clear"]["JCQ"].GetValue<MenuSliderButton>().SValue < Me.ManaPercent)
                                {
                                    Q.Cast();
                                    Variables.Orbwalker.ResetSwingTimer();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Draw"]["W"] && W.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.OrangeRed, 2);
            }

            if (Menu["Draw"]["Damage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && !x.IsDead && !x.IsZombie))
                {
                    if (target != null)
                    {
                        HpBarDraw.Unit = target;

                        HpBarDraw.DrawDmg(GetDamage(target), new SharpDX.ColorBGRA(255, 200, 0, 170));
                    }
                }
            }
        }

        private static float GetDamage(AIHeroClient target)
        {
            float Damage = 0f;

            if (Q.IsReady() && Me.HasBuff("AsheQCastReady"))
            {
                Damage += Q.GetDamage(target);
            }

            if (W.IsReady())
            {
                Damage += W.GetDamage(target);
            }

            if (R.IsReady())
            {
                Damage += R.GetDamage(target);
            }

            return Damage;
        }

        private static bool CanClean(AIHeroClient hero)
        {
            bool CanUse = false;

            if (UseCleanTime > Variables.TickCount)
            {
                return CanUse;
            }

            DebuffTypes.Add(BuffType.Blind);
            DebuffTypes.Add(BuffType.Charm);
            DebuffTypes.Add(BuffType.Fear);
            DebuffTypes.Add(BuffType.Flee);
            DebuffTypes.Add(BuffType.Stun);
            DebuffTypes.Add(BuffType.Snare);
            DebuffTypes.Add(BuffType.Taunt);
            DebuffTypes.Add(BuffType.Suppression);
            DebuffTypes.Add(BuffType.Polymorph);
            DebuffTypes.Add(BuffType.Blind);
            DebuffTypes.Add(BuffType.Silence);

            foreach (var buff in hero.Buffs)
            {
                if (DebuffTypes.Contains(buff.Type) && (buff.EndTime - Game.Time) * 1000 >= 800 && buff.IsActive)
                {
                    CanUse = true;
                }
            }

            if (hero.HasBuff("CleanSummonerExhaust"))
            {
                CanUse = true;
            }

            UseCleanTime = Variables.TickCount;

            return CanUse;
        }

        private static bool CanUseQuicksilver()
        {
            if (Items.HasItem(Quicksilver) && Items.CanUseItem(Quicksilver))
            {
                CleanID = Quicksilver;
                return true;
            }
            else
            {
                CleanID = 0;
                return false;
            }
        }

        private static bool CanUseMikaels()
        {
            if (Items.HasItem(Mikaels) && Items.CanUseItem(Mikaels))
            {
                CleanID = Mikaels;
                return true;
            }
            else
            {
                CleanID = 0;
                return false;
            }
        }

        private static bool CanUseMercurial()
        {
            if (Items.HasItem(Mercurial) && Items.CanUseItem(Mercurial))
            {
                CleanID = Mercurial;
                return true;
            }
            else
            {
                CleanID = 0;
                return false;
            }
        }

        private static bool CanUseDervish()
        {
            if (Items.HasItem(Dervish) && Items.CanUseItem(Dervish))
            {
                CleanID = Dervish;
                return true;
            }
            else
            {
                CleanID = 0;
                return false;
            }
        }
    }
}
