using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FastTrundle
{
    internal enum Spells
    {
        Q,
        W,
        E,
        R
    }

    internal static class Trundle
    {
        #region Data

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 550f) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 900f) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 1000f) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 700f) }
                                                             };

        private static SpellSlot ignite;

        private static Vector3 pillarPosition;
        
        private static bool allowQAfterAA, allowItemsAfterAA;

        public static string ScriptVersion => typeof(Trundle).Assembly.GetName().Version.ToString();
        
        private static AIHeroClient Player => ObjectManager.Player;

        #endregion

        #region Methods

        #region Event handlers

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Trundle") return;

            spells[Spells.E].SetSkillshot(0.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            Notifications.AddNotification(string.Format("FastTrundle v{0}", ScriptVersion), 8000);
            FastTrundleMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnSpellCast += ObjAIBase_OnSpellCast;
        }

        private static void ObjAIBase_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (args.Target == null || !args.Target.IsValid) return;
            
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (Orbwalker.GetTarget() == null) return;

                if (allowQAfterAA && !(args.Target is Obj_AI_Turret || args.Target is Obj_Barracks || args.Target is Obj_BarracksDampener || args.Target is Obj_Building) && spells[Spells.Q].IsReady())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(50, () => 
                    {
                        spells[Spells.Q].Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    });
                    return;
                }
                if (allowItemsAfterAA && IsActive("FastTrundle.Items.Titanic") && Items.HasItem(3748) && Items.CanUseItem(3748)) // Titanic
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(50, () => 
                    {
                        Items.UseItem(3748);
                        Orbwalking.ResetAutoAttackTimer();
                    });
                    return;
                }
                if (allowItemsAfterAA && IsActive("FastTrundle.Items.Hydra") && Items.HasItem(3077) && Items.CanUseItem(3077))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(50, () => 
                    {
                        Items.UseItem(3077);
                        Orbwalking.ResetAutoAttackTimer();
                    });
                    return;
                }
                if (allowItemsAfterAA && IsActive("FastTrundle.Items.Hydra") && Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(50, () => 
                    {
                        Items.UseItem(3074);
                        Orbwalking.ResetAutoAttackTimer();
                    });
                    return;
                }
            }
        }
        
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.Distance(Player) > spells[Spells.E].Range || !IsActive("FastTrundle.Antigapcloser"))
            {
                return;
            }

            if (gapcloser.Sender.IsValidTarget(spells[Spells.E].Range))
            {
                if (spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(gapcloser.Sender);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!IsActive("FastTrundle.Interrupter"))
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.Distance(Player) > spells[Spells.E].Range)
            {
                return;
            }

            if (spells[Spells.E].CanCast(sender) && args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                spells[Spells.E].Cast(sender.Position);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var newTarget = TargetSelector.GetTarget(spells[Spells.E].Range + 200, TargetSelector.DamageType.Physical);
            var drawOff = FastTrundleMenu.Menu.Item("FastTrundle.Draw.off").GetValue<bool>();
            var drawQ = FastTrundleMenu.Menu.Item("FastTrundle.Draw.Q").GetValue<Circle>();
            var drawW = FastTrundleMenu.Menu.Item("FastTrundle.Draw.W").GetValue<Circle>();
            var drawE = FastTrundleMenu.Menu.Item("FastTrundle.Draw.E").GetValue<Circle>();
            var drawR = FastTrundleMenu.Menu.Item("FastTrundle.Draw.R").GetValue<Circle>();
            var drawPillar = FastTrundleMenu.Menu.Item("FastTrundle.Draw.Pillar").GetValue<Circle>();

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, drawQ.Color);
                }
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, drawW.Color);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, drawE.Color);
                }
            }

            if (drawR.Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, drawR.Color);
                }
            }

            if (drawPillar.Active)
            {
                if (spells[Spells.E].Level > 0
                    && newTarget != null
                    && newTarget.IsVisible
                    && newTarget.IsValidTarget()
                    && !newTarget.IsDead
                    && Player.Distance(newTarget) < 3000)
                {
                    Drawing.DrawCircle(GetPillarPosition(newTarget), 188, drawPillar.Color);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            allowQAfterAA = allowItemsAfterAA = false;
            if (Player.IsDead || Player.IsRecalling() || MenuGUI.IsChatOpen || Shop.IsOpen) return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
        }

        #endregion

        #region Orbwalking Modes

        private static void JungleClear()
        {
            allowItemsAfterAA = true;

            var minion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minion == null) return;

            if (Player.ManaPercent < FastTrundleMenu.Menu.Item("FastTrundle.JungleClear.Mana").GetValue<Slider>().Value)
                return;

            if (IsActive("FastTrundle.JungleClear.Q") && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (IsActive("FastTrundle.JungleClear.W") && spells[Spells.W].IsReady()
                && minion.IsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void LaneClear()
        {
            allowItemsAfterAA = true;

            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null) return;

            if (Player.ManaPercent < FastTrundleMenu.Menu.Item("FastTrundle.LaneClear.Mana").GetValue<Slider>().Value)
                return;

            if (IsActive("FastTrundle.LaneClear.Q")
                && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                if (IsActive("FastTrundle.LaneClear.Q.Lasthit"))
                {
                    if (minion.Health <= QDamage(minion)
                        && (minion.Health > Player.GetAutoAttackDamage(minion) ||
                            (!Player.Spellbook.IsAutoAttacking && !Orbwalking.CanAttack()))) // don't overkill with Q unless we need AA reset to get it
                    {
                        spells[Spells.Q].Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                    }
                }
                else
                    allowQAfterAA = true;
            }

            if (IsActive("FastTrundle.LaneClear.W") && spells[Spells.W].IsReady()
                && minion.IsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void LastHit()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null) return;

            if (Player.ManaPercent < FastTrundleMenu.Menu.Item("FastTrundle.LastHit.Mana").GetValue<Slider>().Value) return;

            if (IsActive("FastTrundle.LastHit.Q")
                && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range)
                && minion.Health <= QDamage(minion)
                && (minion.Health > Player.GetAutoAttackDamage(minion) ||
                    (!Player.Spellbook.IsAutoAttacking && !Orbwalking.CanAttack()))) // don't overkill with Q unless we need AA reset to get it
            {
                spells[Spells.Q].Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
            }
        }

        private static void Combo()
        {
            allowItemsAfterAA = true;

            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget()) return;

            if (IsActive("FastTrundle.Combo.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            UseItems(target);

            if (IsActive("FastTrundle.Combo.W") && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }

            if (IsActive("FastTrundle.Combo.Q") && target.IsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (spells[Spells.R].IsReady() && IsActive("FastTrundle.Combo.R"))
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>())
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = FastTrundleMenu.Menu.Item("FastTrundle.R.On" + hero.ChampionName);
                        if (getEnemies != null && getEnemies.GetValue<bool>())
                        {
                            spells[Spells.R].Cast(hero);
                        }

                        if (getEnemies != null && !getEnemies.GetValue<bool>() && Player.CountEnemiesInRange(1500) == 1)
                        {
                            spells[Spells.R].Cast(hero);
                        }
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health
                && IsActive("FastTrundle.Combo.Ignite"))
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Harass()
        {
            allowItemsAfterAA = true;

            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget()) return;

            if (Player.ManaPercent < FastTrundleMenu.Menu.Item("FastTrundle.Harass.Mana").GetValue<Slider>().Value) return;

            if (IsActive("FastTrundle.Harass.Q") && target.IsValidTarget(spells[Spells.Q].Range))
            {
                allowQAfterAA = true;
            }

            if (IsActive("FastTrundle.Harass.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            if (IsActive("FastTrundle.Harass.W") && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }
        }

        #endregion

        #region Helpers

        private static void UseItems(Obj_AI_Base target)
        {
            if (IsActive("FastTrundle.Items.Blade")
                && Player.HealthPercent <= FastTrundleMenu.Menu.Item("FastTrundle.Items.Blade.MyHP").GetValue<Slider>().Value)
            {
                if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady()
                    && ItemData.Blade_of_the_Ruined_King.Range < Player.Distance(target))
                {
                    ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
                }

                if (ItemData.Bilgewater_Cutlass.GetItem().IsReady()
                    && ItemData.Bilgewater_Cutlass.Range < Player.Distance(target))
                {
                    ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
                }
            }

            if (IsActive("FastTrundle.Items.Youmuu"))
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()
                    && Orbwalking.GetRealAutoAttackRange(Player) < Player.Distance(target))
                {
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
                }
            }
        }

        private static bool IsActive(string menuItem)
        {
            return FastTrundleMenu.Menu.Item(menuItem).GetValue<bool>();
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static double QDamage(Obj_AI_Base target)
        {
            return Player.GetAutoAttackDamage(target) + spells[Spells.Q].GetDamage(target);
        }

        private static Vector3 GetPillarPosition(AIHeroClient target)
        {
            pillarPosition = Player.Position;

            return V2E(pillarPosition, target.Position, target.Distance(pillarPosition) + 230).To3D();
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance*Vector3.Normalize(direction - from).To2D();
        }

        #endregion

        #endregion
    }
}
