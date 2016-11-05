/**
 * 
 * Credits: Sebby for the Lib and Brand R Logic + few other stuff (Since I'm "newbie" to his Lib :kappa:)
 * 
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; namespace SurvivorBrand
{
    class Program
    {

        #region Declaration
        private static Spell Q, W, E, R;
        private static float RManaC = 0;
        private static SpellSlot IgniteSlot;
        private static SebbyLib.Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public const string ChampionName = "Brand";
        private static int lvl1, lvl2, lvl3, lvl4;
        #endregion

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != ChampionName)
                return;

            #region Spells
            Q = new Spell(SpellSlot.Q, 1050f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 625f);
            R = new Spell(SpellSlot.R, 750f);
            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.15f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0.25f, 2000f);
            #endregion

            #region SummonerSpells
            IgniteSlot = Player.GetSpellSlot("summonerdot");
            #endregion

            #region Menu
            Menu = new Menu("SurvivorBrand", "SurvivorBrand", true);

            Menu OrbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new SebbyLib.Orbwalking.Orbwalker(OrbwalkerMenu);

            Menu TargetSelectorMenu = Menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
            TargetSelector.AddToMenu(TargetSelectorMenu);

            Menu ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            ComboMenu.AddItem(new MenuItem("ComboUseQ", "Use Q").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseW", "Use W").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseE", "Use E").SetValue(true));
            ComboMenu.AddItem(new MenuItem("ComboUseR", "Use R").SetValue(true));
            // ComboMenu.AddItem(new MenuItem("ComboUseIgnite", "Use Ignite").SetValue(true));

            Menu HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            HarassMenu.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("harassW", "Use W").SetValue(true));
            HarassMenu.AddItem(new MenuItem("harassE", "Use E").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassBlockAA", "Block AA's in Harass Mode").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassManaManager", "Mana Manager (%)").SetValue(new Slider(30, 1, 100)));

            Menu LaneClearMenu = Menu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            LaneClearMenu.AddItem(new MenuItem("laneclearW", "Use W").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("laneclearE", "Use E").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("LaneClearWMinions", "Minimum Enemies inside W").SetValue(new Slider(2, 1, 10)));
            LaneClearMenu.AddItem(new MenuItem("LaneClearManaManager", "Mana Manager (%)").SetValue(new Slider(30, 1, 100)));

            Menu KillStealMenu = Menu.AddSubMenu(new Menu("Kill Steal", "KillSteal"));
            KillStealMenu.AddItem(new MenuItem("KillStealWithAvSpells", "KS with available spells (Q/W/E)").SetValue(true));

            Menu MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            MiscMenu.AddItem(new MenuItem("PrioritizeStunned", "Prioritize Stunned Targets?").SetValue(true));
            MiscMenu.AddItem(new MenuItem("StunTargetKey", "Stun the target if possible while holding Key -> 'G'").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press)));
            MiscMenu.AddItem(new MenuItem("QAblazedEnemy", "Auto Q if Target's [ABlazed]").SetValue(true));
            MiscMenu.AddItem(new MenuItem("QGapC", "Auto Stun GapClosers").SetValue(true));
            MiscMenu.AddItem(new MenuItem("InterruptEQ", "Auto E-Q to Interrupt").SetValue(false));
            MiscMenu.AddItem(new MenuItem("QOnlyAblazed", "Use Q Only if Enemy is Ablazed").SetValue(false));
            MiscMenu.AddItem(new MenuItem("NearbyREnemies", "Use R in Combo if X Enemies are nearby 'X' ->").SetValue(new Slider(1, 0, 5)));

            Menu AutoLevelerMenu = Menu.AddSubMenu(new Menu("AutoLeveler Menu", "AutoLevelerMenu"));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp", "AutoLevel Up Spells?").SetValue(true));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp1", "First: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 3)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp2", "Second: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 0)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp3", "Third: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp4", "Fourth: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 2)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLvlStartFrom", "AutoLeveler Start from Level: ").SetValue(new Slider(2, 6, 1)));

            Menu DrawingMenu = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            DrawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawPassiveBombOnEnemy", "Draw Passive Bomb on Enemy (Range) (Soon)").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(true));

            Menu.AddToMainMenu();
            #endregion

            Menu SkinMenu = Menu.AddSubMenu(new Menu("Skins Menu", "SkinMenu"));
            SkinMenu.AddItem(new MenuItem("SkinID", "Skin ID")).SetValue(new Slider(5, 0, 5));
            var UseSkin = SkinMenu.AddItem(new MenuItem("UseSkin", "Enabled")).SetValue(true);
            UseSkin.ValueChanged += (sender, eventArgs) =>
            {
                if (!eventArgs.GetNewValue<bool>())
                {
                }
            };

            #region DrawHPDamage
            var dmgAfterShave = new MenuItem("SurvivorBrand.DrawComboDamage", "Draw Combo Damage").SetValue(true);
            var drawFill =
                new MenuItem("SurvivorBrand.DrawColour", "Fill Color", true).SetValue(
                    new Circle(true, Color.FromArgb(204, 255, 0, 1)));
            DrawingMenu.AddItem(drawFill);
            DrawingMenu.AddItem(dmgAfterShave);
            DrawDamage.DamageToUnit = CalculateDamage;
            DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
            DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
            dmgAfterShave.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            #endregion

            #region Subscriptions
            Game.OnUpdate += OnUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += OnDraw;
            Chat.Print("<font color='#800040'>[SurvivorSeries] Brand</font> <font color='#ff6600'>Loaded.</font>");
            // Add AntiGapCloser + Interrupter + Killsteal //
            #endregion
        }

        private static void OnDraw(EventArgs args)
        {
            //var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Menu.Item("DrawQ").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
            //if (Menu.Item("DrawPassiveBombOnEnemy").GetValue<bool>())
            //    Render.Circle.DrawCircle(m.Position, 420f, Color.Orange);
            if (Menu.Item("DrawW").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.MediumPurple);
            if (Menu.Item("DrawE").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.LightPink);
            if (Menu.Item("DrawR").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.MediumVioletRed);
        }

        private static void AABlock()
        {
            Orbwalker.SetAttack(!Menu.Item("HarassBlockAA").GetValue<bool>());
            //SebbyLib.OktwCommon.blockAttack = Menu.Item("CBlockAA").GetValue<bool>();
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            if (Menu.Item("UseSkin").GetValue<bool>())
            {
            }
            // Checks
            RManaCost();
            // Combo
            if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            //Lane
            if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.LaneClear) // LaneClear mode broken? kappa
            {
                LaneClear();
            }
            if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Mixed)
            {
                AABlock();
                Harass();
            }
            if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.None)
            {
                Orbwalker.SetMovement(true);
                Orbwalker.SetAttack(true);
            }
            if (Menu.Item("StunTargetKey").GetValue<KeyBind>().Active)
            {
                SebbyLib.Orbwalking.MoveTo(Game.CursorPos);
                if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.None)
                {
                    var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                    if (!t.IsValidTarget() || t == null)
                        return;
                    // Wait till the enemy has BrandABlaze from the W or E casting and then cast Q.
                    if (t.HasBuff("brandablaze"))
                    {
                        if (Q.IsReady())
                            SebbySpell(Q, t);
                    }
                    else
                    {
                        if (Q.IsReady() && E.IsReady())
                        {
                            E.CastOnUnit(t);
                        }
                        else if (Q.IsReady() && W.IsReady())
                        {
                            SebbySpell(W, t);
                        }
                    }
                }
            }

            //AutoLeveler
            if (Menu.Item("AutoLevelUp").GetValue<bool>())
            {
                lvl1 = Menu.Item("AutoLevelUp1").GetValue<StringList>().SelectedIndex;
                lvl2 = Menu.Item("AutoLevelUp2").GetValue<StringList>().SelectedIndex;
                lvl3 = Menu.Item("AutoLevelUp3").GetValue<StringList>().SelectedIndex;
                lvl4 = Menu.Item("AutoLevelUp4").GetValue<StringList>().SelectedIndex;
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("QGapC", true).GetValue<bool>() || Player.Mana < Q.Instance.SData.Mana + E.Instance.SData.Mana)
                return;

            var t = gapcloser.Sender;

            if (t.IsValidTarget(E.Range) && (t.HasBuff("brandablaze") || E.IsReady()))
            {
                
                E.CastOnUnit(t);
                if (Q.IsReady())
                    Q.Cast(t);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Menu.Item("InterruptEQ", true).GetValue<bool>() || Player.Mana < Q.Instance.SData.Mana + E.Instance.SData.Mana)
                return;

            if (t.IsValidTarget(E.Range) && (t.HasBuff("brandablaze") || E.IsReady()))
            {
                E.CastOnUnit(t);
                if (Q.IsReady())
                    Q.Cast(t);
            }
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !Menu.Item("AutoLevelUp").GetValue<bool>() || ObjectManager.Player.Level < Menu.Item("AutoLvlStartFrom").GetValue<Slider>().Value)
                return;
            if (lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4)
                return;
            int delay = 700;
            LeagueSharp.Common.Utility.DelayAction.Add(delay, () => LevelUp(lvl1));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 50, () => LevelUp(lvl2));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 100, () => LevelUp(lvl3));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 150, () => LevelUp(lvl4));
        }

        private static void LevelUp(int indx)
        {
            if (ObjectManager.Player.Level < 4)
            {
                if (indx == 0 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (indx == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (indx == 3)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        private static float BonusDmg(AIHeroClient target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Magical, (target.MaxHealth * 0.08) - (target.HPRegenRate * 5));
        }

        private static bool LogQUse(Obj_AI_Base m)
        {
            if (m.HasBuff("brandablaze"))
                return true;
            else if (E.Instance.CooldownExpires - Game.Time + 2 >= Q.Instance.Cooldown && W.Instance.CooldownExpires - Game.Time + 2 >= Q.Instance.Cooldown)
                return true;
            else
                return false;
        }

        private static void QUsage()
        {
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!m.IsValidTarget())
            {
                return;
            }
            // Q Improvement + KS Below
            if (OktwCommon.GetKsDamage(m, Q) + BonusDmg(m) + OktwCommon.GetEchoLudenDamage(m) > m.Health)
            {
                SebbySpell(Q, m);
            }
            if (Menu.Item("QOnlyAblazed").GetValue<bool>() && m.HasBuff("brandablaze"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && enemy.HasBuff("brandablaze")))
                {
                    SebbySpell(Q, enemy);
                }
            }
            else
            {
                if (m.HasBuff("brandablaze") && Menu.Item("QAblazedEnemy").GetValue<bool>())
                {
                    var spreadTarget = m;

                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && enemy.HasBuff("brandablaze")))
                        m = enemy;

                    if (spreadTarget == m && !LogQUse(m))
                        return;
                }

                if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Combo && !W.IsReady() && Player.ManaPercentage() > RManaC + Q.ManaCost)
                {
                    var enemystunned = HeroManager.Enemies.Find(enemy => enemy.IsValidTarget(Q.Range) && enemy.HasBuff("stun") || enemy.IsStunned && Menu.Item("PrioritizeStunned").GetValue<bool>());
                    if (enemystunned != null)
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        SebbySpell(Q, enemystunned);
                    }
                    else
                    {
                        Orbwalker.ForceTarget(m);
                        SebbySpell(Q, m);
                    }
                }
                if (Player.Mana > RManaC + Q.ManaCost)
                {
                    var enemystunned = HeroManager.Enemies.Find(enemy => enemy.IsValidTarget(Q.Range) && enemy.HasBuff("stun") || enemy.IsStunned && Menu.Item("PrioritizeStunned").GetValue<bool>());
                    if (enemystunned != null)
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        SebbySpell(Q, enemystunned);
                    }
                    else
                    {
                        Orbwalker.ForceTarget(m);
                        SebbySpell(Q, m);
                    }
                }
            }
        }

        private static void SebbySpell(Spell QWER, Obj_AI_Base target)
        {
            SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            bool aoe2 = false;

            if (QWER.Type == SkillshotType.SkillshotCircle)
            {
                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if (QWER.Width > 80 && !QWER.Collision)
                aoe2 = true;

            var predInput2 = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = aoe2,
                Collision = QWER.Collision,
                Speed = QWER.Speed,
                Delay = QWER.Delay,
                Range = QWER.Range,
                From = Player.ServerPosition,
                Radius = QWER.Width,
                Unit = target,
                Type = CoreType2
            };
            var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

            //var poutput2 = QWER.GetPrediction(target);

            if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                QWER.Cast(poutput2.CastPosition);
        }

        private static void RManaCost()
        {
            if (!R.IsReady())
            {
                RManaC = W.Instance.SData.Mana;
            }
            else
            {
                RManaC = R.Instance.SData.Mana;
            }
        }

        private static void WUsage()
        {
            // W Usage
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                var Qdamage = Q.GetDamage(t);
                var Wdamage = OktwCommon.GetKsDamage(t, W) + BonusDmg(t) + OktwCommon.GetEchoLudenDamage(t);
                if (Wdamage > t.Health)
                {
                    SebbySpell(W, t);
                }
                else if (Wdamage + Qdamage > t.Health && Player.ManaPercentage() > Q.ManaCost + E.ManaCost)
                {
                    SebbySpell(W, t);
                }

                if (Player.Mana > RManaC + W.ManaCost)
                {
                    var enemystunned = HeroManager.Enemies.Find(enemy => enemy.IsValidTarget(W.Range) && enemy.HasBuff("stun") || enemy.IsStunned && Menu.Item("PrioritizeStunned").GetValue<bool>());
                    if (enemystunned != null)
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        SebbySpell(W, enemystunned);
                    }
                    else
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        SebbySpell(W, t);
                    }
                }
            }
        }

        private static void EUsage()
        {
            // E Usage
            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                // If in Combo Mode
                if (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.Combo && Player.Mana > RManaC + E.ManaCost)
                {
                    var enemystunned = HeroManager.Enemies.Find(enemy => enemy.IsValidTarget(E.Range) && enemy.HasBuff("stun") || enemy.IsStunned && Menu.Item("PrioritizeStunned").GetValue<bool>());
                    if (enemystunned != null)
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        E.CastOnUnit(enemystunned);
                    }
                    else
                    {
                        Orbwalker.ForceTarget(t);
                        E.CastOnUnit(t);
                    }
                }
                else
                {
                    // If there's a chance to KS/Get :kappa: a kill.
                    var eDmg = OktwCommon.GetKsDamage(t, E) + BonusDmg(t) + OktwCommon.GetEchoLudenDamage(t);
                    var wDmg = W.GetDamage(t);
                    if (eDmg > t.Health)
                    {
                        E.CastOnUnit(t);
                    }
                    else if (wDmg + eDmg > t.Health && Player.Mana > W.ManaCost + E.ManaCost)
                    {
                        E.CastOnUnit(t);
                        SebbySpell(W, t);
                    }
                }
            }
        }

        private static void KSHPPrediction(Obj_AI_Base enemy)
        {
            // Thanks Sebby for making such a wonderful Lib :feelsniceman:
            //var TargetHP = SebbyLib.HealthPrediction.GetHealthPrediction(enemy, 500) + SebbyLib.OktwCommon.GetIncomingDamage(enemy);
        }

        private static void RUsage()
        {
            // Massive Thanks to Sebby for the time saved instead of writing it from scratch, instead I 'stole' it from him :feelsbadman:
            var bounceRange = 430;
            var t2 = TargetSelector.GetTarget(R.Range + bounceRange, TargetSelector.DamageType.Magical);

            if (t2.IsValidTarget(R.Range) && t2.CountEnemiesInRange(bounceRange) >= Menu.Item("NearbyREnemies").GetValue<Slider>().Value && Menu.Item("NearbyREnemies").GetValue<Slider>().Value > 0)
                R.Cast(t2);

            if (t2.IsValidTarget() && OktwCommon.ValidUlt(t2))
            {
                if (t2.CountAlliesInRange(550) == 0 || Player.HealthPercent < 50 || t2.CountEnemiesInRange(bounceRange) > 1)
                {
                    var prepos = R.GetPrediction(t2).CastPosition;
                    var dmgR = R.GetDamage(t2);

                    if (t2.Health < dmgR * 3)
                    {
                        var totalDmg = dmgR;
                        var minionCount = CountMinionsInRange(bounceRange, prepos);

                        if (t2.IsValidTarget(R.Range))
                        {
                            if (prepos.CountEnemiesInRange(bounceRange) > 1)
                            {
                                if (minionCount > 2)
                                    totalDmg = dmgR * 2;
                                else
                                    totalDmg = dmgR * 3;
                            }
                            else if (minionCount > 0)
                            {
                                totalDmg = dmgR * 2;
                            }

                            if (W.IsReady())
                            {
                                totalDmg += W.GetDamage(t2);
                            }

                            if (E.IsReady())
                            {
                                totalDmg += E.GetDamage(t2);
                            }

                            if (Q.IsReady())
                            {
                                totalDmg += Q.GetDamage(t2);
                            }

                            totalDmg += BonusDmg(t2);
                            totalDmg += OktwCommon.GetEchoLudenDamage(t2);

                            // Hex
                            if (Items.HasItem(3155, t2))
                            {
                                // Maximum is 280, but lets use 250 as an average value.
                                totalDmg = totalDmg - 250;
                            }

                            // MoM
                            if (Items.HasItem(3156, t2))
                            {
                                // Nerfed? Kappa
                                totalDmg = totalDmg - 300;
                            }

                            if (totalDmg > t2.Health - OktwCommon.GetIncomingDamage(t2) && Player.GetAutoAttackDamage(t2) * 2 < t2.Health)
                            {
                                // Kill the Target :feelsbadman:
                                R.CastOnUnit(t2);
                            }

                        }
                        else if (t2.Health - OktwCommon.GetIncomingDamage(t2) < dmgR * 2 + BonusDmg(t2))
                        {
                            if (Player.CountEnemiesInRange(R.Range) > 0)
                            {
                                foreach (var t in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && enemy.Distance(prepos) < bounceRange))
                                {
                                    R.CastOnUnit(t);
                                }
                            }
                            else
                            {
                                var minions = Cache.GetMinions(Player.Position, R.Range);
                                foreach (var minion in minions.Where(minion => minion.IsValidTarget(R.Range) && minion.Distance(prepos) < bounceRange))
                                {
                                    R.CastOnUnit(minion);
                                }
                            }
                        }
                    }
                }
            }
        }

        private double Wdmg(Obj_AI_Base target)
        {
            return target.MaxHealth * (4.5 + W.Level * 1.5) * 0.01;
        }

        private static int CountMinionsInRange(float range, Vector3 pos)
        {
            var minions = Cache.GetMinions(pos, range);
            int count = 0;
            foreach (var minion in minions)
            {
                count++;
            }
            return count;
        }

        private static void Combo()
        {
            // If Mana is < ManaManager : return;
            // Combo
            WUsage();
            QUsage();
            EUsage();
            RUsage();
        }

        private static void Harass()
        {
            // Harass
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!t.IsValidTarget())
            {
                return;
            }
            if (Player.ManaPercentage() > Menu.Item("HarassManaManager").GetValue<Slider>().Value)
            {
                if (Menu.Item("harrasW").GetValue<bool>() && W.IsInRange(t))
                {
                    SebbySpell(W, t);
                }
            }
            if (Player.ManaPercentage() > Menu.Item("HarassManaManager").GetValue<Slider>().Value)
            {
                if (Menu.Item("harrasQ").GetValue<bool>() && Q.IsInRange(t))
                {
                    SebbySpell(Q, t);
                }
            }
            if (Player.ManaPercentage() > Menu.Item("HarassManaManager").GetValue<Slider>().Value)
            {
                if (Menu.Item("harrasE").GetValue<bool>() && E.IsInRange(t))
                {
                    E.CastOnUnit(t);
                }
            }
        }

        private static void LaneClear()
        {
            // LaneClear W
            if (Player.ManaPercentage() > Menu.Item("LaneClearManaManager").GetValue<Slider>().Value)
                {
                    if (Menu.Item("laneclearW").GetValue<bool>() && W.IsReady())
                    {
                        var allMinionsW = Cache.GetMinions(Player.ServerPosition, W.Range, MinionTeam.Enemy);
                        var farmPos = Q.GetCircularFarmLocation(allMinionsW, 250);
                        if (farmPos.MinionsHit > Menu.Item("LaneClearWMinions").GetValue<Slider>().Value)
                            W.Cast(farmPos.Position);
                    }
                }
            // LaneClear E
            if (Player.ManaPercentage() > Menu.Item("LaneClearManaManager").GetValue<Slider>().Value)
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Enemy);
                if (Menu.Item("laneclearE").GetValue<bool>() && E.IsReady() && !W.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (minion.IsValidTarget() && minion.HasBuff("brandablaze"))
                        {
                            E.CastOnUnit(minion);
                        }
                        else
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            // Calculate Damage + Bleed ticking
            float damage = 0;
            float BrandPassive = (enemy.MaxHealth * 14) / 100;
            float PassiveDamage = (enemy.MaxHealth * 2) / 100;
            // bool HasABlazeBomb = enemy.HasBuff("brandablazebomb");
            damage += BrandPassive;
            if (enemy.HasBuff("brandablazebomb"))
            {
                damage += PassiveDamage;
            }
            double ultdamage = 0;

            if (Q.IsReady())
            {
                damage += Q.GetDamage(enemy);
            }

            if (W.IsReady())
            {
                if (enemy.HasBuff("brandablaze") || enemy.HasBuff("ablaze"))
                {
                    float BonusWDmg = W.GetDamage(enemy) + ((W.GetDamage(enemy) * 25) / 100);
                    damage += BonusWDmg;
                }
                else
                {
                    damage += W.GetDamage(enemy);
                }
            }

            if (E.IsReady())
            {
                damage += E.GetDamage(enemy);
            }

            if (R.IsReady())
            {
                ultdamage += Player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return damage;
        }

    }
}