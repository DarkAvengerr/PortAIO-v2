using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hGnar
{
    public class ActiveModes
    {
        private static AIHeroClient player = ObjectManager.Player;

        private static Spell Q
        {
            get { return SpellManager.Q; }
        }
        private static Spell W
        {
            get { return SpellManager.W; }
        }
        private static Spell E
        {
            get { return SpellManager.E; }
        }
        private static Spell R
        {
            get { return SpellManager.R; }
        }

        private class Mode
        {
            public const string COMBO = "combo";
            public const string HARASS = "harass";
            public const string WAVE = "wave";
            public const string JUNGLE = "jungle";
            public const string FLEE = "flee";
        }

        public static void OnPermaActive()
        {
            // Face of the Mountain Kappa
            if (Config.BoolLinks["itemsFace"].Value && ItemManager.FACE_MOUNTAIN.IsReady())
            {
                foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.Team == player.Team && h.IsValidTarget(700, false)))
                {
                    if (ally.HealthPercent < 15 && ally.CountEnemiesInRange(700) > 0)
                    {
                        ItemManager.FACE_MOUNTAIN.Cast(ally);
                        break;
                    }
                }
            }
        }
        public static HitChance HikiChance(string menuName)
        {
            return Config.HitchanceArray[Config.Menu.MainMenu.MenuHandle.Item(menuName).GetValue<StringList>().SelectedIndex];
        }

        public static void OnCombo(bool afterAttack = false, Obj_AI_Base afterAttackTarget = null)
        {
            // Item usage (General)
            if (Config.BoolLinks["comboUseItems"].Value)
            {
                if (afterAttack && afterAttackTarget is AIHeroClient)
                {
                    var target = (AIHeroClient)afterAttackTarget;

                    // All in Kappa
                    if (player.IsMegaGnar())
                        ItemManager.UseBotrk(target);
                    ItemManager.UseRanduin(target);
                    ItemManager.UseYoumuu(target);
                }
            }

            // Ignite
            if (Program.HasIgnite && Config.BoolLinks["comboUseIgnite"].Value)
            {
                var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.True);
                if (target != null && player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health)
                {
                    player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
                }
            }

            // Mini
            if (player.IsMiniGnar())
            {
                // Q usage
                if (Q.IsEnabledAndReady(Mode.COMBO) && !player.IsAboutToTransform())
                {
                    var target = Q.GetTarget();
                    if (target != null)
                    {
                        var prediction = Q.GetPrediction(target);
                        if (prediction.Hitchance >= HikiChance("hikiChance"))
                        {
                            Q.Cast(prediction.CastPosition);
                        }

                        if (prediction.Hitchance > HitChance.Collision)
                        {
                            var colliding = prediction.CollisionObjects.OrderBy(o => o.Distance(player, true)).ToList();
                            if (colliding.Count > 0)
                            {
                                // First colliding target is < 100 units away from our main target
                                if (colliding[0].Distance(target, true) < 10000)
                                    Q.Cast(prediction.CastPosition);
                            }
                        }
                    }
                }

                // E usage (only when transforming into Mega Gnar)
                if (E.IsEnabledAndReady(Mode.COMBO) && player.IsAboutToTransform())
                {
                    var target = E.GetTarget(E.Width / 2);
                    if (target != null)
                    {
                        var prediction = E.GetPrediction(target);

                        if (prediction.Hitchance >= HikiChance("hikiChance"))
                        {
                            // Get the landing point of our E
                            var arrivalPoint = player.ServerPosition.Extend(prediction.CastPosition, player.ServerPosition.Distance(prediction.CastPosition) + E.Range);

                            // If we will land in the tower attack range of 775, don't continue
                            if (!ObjectManager.Get<Obj_AI_Turret>().Any(t => t.Team != player.Team && !t.IsDead && t.Distance(arrivalPoint, true) < 775 * 775))
                            {
                                // Arrival point won't be in the turret range, cast it
                                E.Cast(prediction.CastPosition);
                            }
                        }
                    }
                }
            }
            // Mega
            else
            {
                // Item usage (Hydra/Tiamat)
                if (afterAttack && Config.BoolLinks["comboUseItems"].Value && ItemManager.UseHydra(afterAttackTarget))
                    return;

                // R usage
                #region Ult calculations

                if (R.IsEnabledAndReady(Mode.COMBO) && !SpellManager.HasCastedStun)
                {
                    var target = R.GetTarget();
                    if (target != null && target.GetStunDuration() < R.Delay)
                    {
                        var prediction = Prediction.GetPrediction(target, R.Delay);
                        if (prediction.Hitchance >= HitChance.High && R.IsInRange(prediction.UnitPosition))
                        {
                            // 12 angle checks for casting, prefer to player direction
                            var direction = (player.ServerPosition - prediction.UnitPosition).Normalized();
                            var maxAngle = 180f;
                            var step = maxAngle / 6f;
                            var currentAngle = 0f;
                            var currentStep = 0f;
                            while (true)
                            {
                                // Validate the counter, break if no valid spot was found in previous loops
                                if (currentStep > maxAngle && currentAngle < 0)
                                    break;

                                // Check next angle
                                if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                                {
                                    currentAngle = (currentStep) * (float)Math.PI / 180;
                                    currentStep += step;
                                }
                                else if (currentAngle > 0)
                                    currentAngle = -currentAngle;

                                Vector3 checkPoint;

                                // One time only check for direct line of sight without rotating
                                if (currentStep == 0)
                                {
                                    currentStep = step;
                                    checkPoint = prediction.UnitPosition + 500 * direction;
                                }
                                // Rotated check
                                else
                                    checkPoint = prediction.UnitPosition + 500 * direction.Rotated(currentAngle);

                                // Check for a wall between the checkPoint and the target position
                                if (prediction.UnitPosition.GetFirstWallPoint(checkPoint).HasValue)
                                {
                                    // Cast ult into the direction where the wall is located
                                    R.Cast(player.Position + 500 * (checkPoint - prediction.UnitPosition).Normalized());
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion

                // W usage
                if (W.IsEnabledAndReady(Mode.COMBO) && !SpellManager.HasCastedStun)
                {
                    var target = W.GetTarget();
                    if (target != null && target.GetStunDuration() < W.Delay)
                    {
                        // Only cast if target is not already stunned to make the longest chain possible
                        W.Cast(target);
                    }
                }

                // E usasge
                if (E.IsEnabledAndReady(Mode.COMBO))
                {
                    var target = E.GetTarget(E.Width / 2);
                    if (target != null)
                    {
                        var pred = E.GetPrediction(target);
                        if (pred.Hitchance >= HikiChance("hikiChance"))
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                }

                // Q usage
                if (Q.IsEnabledAndReady(Mode.COMBO))
                {
                    var target = Q.GetTarget();
                    if (target != null)
                    {
                        var pred = Q.GetPrediction(target);
                        if (pred.Hitchance >= HikiChance("hikiChance"))
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        public static void OnHarass()
        {
            // Q usage
            if (Q.IsEnabledAndReady(Mode.HARASS) && (player.IsMiniGnar() && (player.IsAboutToTransform() ? Config.BoolLinks["harassUseQMega"].Value : true)))
            {
                var target = Q.GetTarget();
                if (target != null)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.Hitchance >= HikiChance("hikiChance") || prediction.Hitchance > HitChance.Immobile)
                    {
                        Q.Cast(prediction.CastPosition);
                    }

                    if (prediction.Hitchance > HitChance.Collision)
                    {
                        var colliding = prediction.CollisionObjects.OrderBy(o => o.Distance(player, true)).ToList();
                        if (colliding.Count > 0)
                        {
                            // First colliding target is < 100 units away from our main target
                            if (colliding[0].Distance(target, true) < 10000)
                                Q.Cast(prediction.CastPosition);
                        }
                    }
                }
            }

            // Mega
            if (player.IsMegaGnar())
            {
                if (W.IsEnabledAndReady(Mode.HARASS))
                {
                    var target = W.GetTarget();
                    if (target != null)
                        W.Cast(target);
                }
            }
        }

        public static void OnWaveClear(bool afterAttack = false, Obj_AI_Base afterAttackTarget = null)
        {
            // Mini
            if (player.IsMiniGnar())
            {
                // Q usage
                if (Q.IsEnabledAndReady(Mode.WAVE) && (player.IsAboutToTransform() ? Config.BoolLinks["waveUseQMega"].Value : true))
                {
                    var wminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range).ToList();
                    var xx = Q.GetLineFarmLocation(wminion);
                    if (xx.MinionsHit >= Config.Menu.MainMenu.MenuHandle.Item("mini.q.minion.count").GetValue<Slider>().Value)
                    {
                        Q.Cast(xx.Position);
                    }
                }
            }
            // Mega (this is just wasting spells, I disable every spell in here for myself :P)
            else
            {
                // Item usage
                if (afterAttack && Config.BoolLinks["waveUseItems"].Value && ItemManager.UseHydra(afterAttackTarget))
                    return;

                // Q usage
                if (Q.IsEnabledAndReady(Mode.WAVE))
                {
                    var wminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range).ToList();
                    var xx = Q.GetLineFarmLocation(wminion);
                    if (xx.MinionsHit >= Config.Menu.MainMenu.MenuHandle.Item("mega.q.minion.count").GetValue<Slider>().Value)
                    {
                        Q.Cast(xx.Position);
                    }
                }

                // W usage
                if (W.IsEnabledAndReady(Mode.WAVE))
                {
                    // Get farm location
                    var wminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range).ToList();
                    var xx = W.GetLineFarmLocation(wminion);
                    if (xx.MinionsHit >= Config.Menu.MainMenu.MenuHandle.Item("mega.w.minion.count").GetValue<Slider>().Value)
                    {
                        W.Cast(xx.Position);
                    }
                }

                // E usage
                if (E.IsEnabledAndReady(Mode.WAVE))
                {
                    // Get farm location
                    var wminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range).ToList();
                    var xx = E.GetLineFarmLocation(wminion);
                    if (xx.MinionsHit >= Config.Menu.MainMenu.MenuHandle.Item("mega.e.minion.count").GetValue<Slider>().Value)
                    {
                        E.Cast(xx.Position);
                    }
                }
            }
        }

        public static void OnJungleClear(bool afterAttack = false, Obj_AI_Base afterAttackTarget = null)
        {
            // Mini
            if (player.IsMiniGnar())
            {
                // Q usage
                if (Q.IsEnabledAndReady(Mode.JUNGLE) && (player.IsAboutToTransform() ? Config.BoolLinks["jungleUseQMega"].Value : true))
                {
                    // Get farm location
                    var position = Q.GetFarmLocation(MinionTeam.Neutral);
                    if (position.HasValue)
                        Q.Cast(position.Value.Position);
                }
            }
            // Mega
            else
            {
                // Item usage
                if (afterAttack && Config.BoolLinks["jungleUseItems"].Value && ItemManager.UseHydra(afterAttackTarget))
                    return;

                // Q usage
                if (Q.IsEnabledAndReady(Mode.JUNGLE))
                {
                    // Get farm location
                    var position = Q.GetFarmLocation(MinionTeam.Neutral);
                    if (position.HasValue)
                        Q.Cast(position.Value.Position);
                }

                // W usage
                if (W.IsEnabledAndReady(Mode.JUNGLE))
                {
                    // Get farm location
                    var position = W.GetFarmLocation(MinionTeam.Neutral);
                    if (position.HasValue)
                        W.Cast(position.Value.Position);
                }

                // E usage
                if (E.IsEnabledAndReady(Mode.JUNGLE))
                {
                    // Get farm location
                    var position = E.GetFarmLocation(MinionTeam.Neutral);
                    if (position.HasValue)
                        E.Cast(position.Value.Position);
                }
            }
        }

        public static void OnFlee()
        {
            // Nothing yet Kappa
            Orbwalking.Orbwalk(null, Game.CursorPos);
        }

        public static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                if (Config.KeyLinks["comboActive"].Value.Active)
                    ActiveModes.OnCombo(true, target as Obj_AI_Base);
                if (Config.KeyLinks["waveActive"].Value.Active)
                    ActiveModes.OnWaveClear(true, target as Obj_AI_Base);
                if (Config.KeyLinks["jungleActive"].Value.Active)
                    ActiveModes.OnJungleClear(true, target as Obj_AI_Base);
            }
        }
    }
}
