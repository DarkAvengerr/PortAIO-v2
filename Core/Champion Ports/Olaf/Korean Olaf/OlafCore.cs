using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using KoreanOlaf.QueueActions;

    using SharpDX;

    class OlafCore
    {
        private Orbwalking.Orbwalker olafOrbwalker { get; set; }

        private readonly OlafMenu olafMenu;

        private readonly OlafSpell q;

        private readonly OlafSpell w;

        private readonly OlafSpell e;

        private readonly OlafSpell r;

        private readonly AIHeroClient player;

        private readonly OlafOffensiveItems olafItems;

        private readonly ActionQueueList harasQueue;

        private readonly ActionQueueList comboQueue;

        private readonly ActionQueueList laneClearQueue;

        private readonly ActionQueue actionQueue;

        private readonly ActionQueueCheckAutoAttack autoAttackCheck;

        public OlafCore(OlafSpells olafSpells, Orbwalking.Orbwalker olafOrbwalker, OlafMenu olafMenu)
        {
            q = olafSpells.Q;
            w = olafSpells.W;
            e = olafSpells.E;
            r = olafSpells.R;

            player = ObjectManager.Player;
            this.olafOrbwalker = olafOrbwalker;
            this.olafMenu = olafMenu;

            actionQueue = new ActionQueue();
            harasQueue = new ActionQueueList();
            comboQueue = new ActionQueueList();
            laneClearQueue = new ActionQueueList();
            autoAttackCheck = new ActionQueueCheckAutoAttack();
            olafItems = new OlafOffensiveItems(olafMenu);

            Game.OnUpdate += Game_OnUpdate;
        }  

        private void Game_OnUpdate(EventArgs args)
        {
            switch (olafOrbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                default:
                    return;
            }
        }

        private bool CastQ(bool combo, IEnumerable<AIHeroClient> blackList = null)
        {
            if ((q.IsReady()) && ((combo && q.UseOnCombo) || (!combo && q.UseOnHarass)))
            {
                AIHeroClient target = TargetSelector.GetTarget(q.Range, q.DamageType, true, blackList);

                if (target != null)
                {
                    PredictionOutput predictionOutput = q.GetPrediction(target);

                    if (predictionOutput.Hitchance >= HitChance.Medium)
                    {

                        float distance1 = predictionOutput.CastPosition.Distance(player.Position);
                        float distance2 = target.Distance(player.Position);

                        float extension = -75F;
                        if (distance2 < player.AttackRange)
                        {
                            extension = 50F;
                        }
                        if (distance1 > 250F)
                        {
                            extension = -100;
                        }
                        if (distance1 > 500F)
                        {
                            extension = -130F;
                        }
                        if (distance1 > 700)
                        {
                            extension = -150F;
                        }
                        else if (distance1 > 850F)
                        {
                            extension = -175F;
                        }

                        q.Cast(predictionOutput.CastPosition.Extend(player.Position, extension));
                        return true;
                    }
                }
            }
            return false;
        }

        private void CastE(Obj_AI_Base target)
        {
            e.Cast(target);
            Orbwalking.ResetAutoAttackTimer();
        }

        private void Combo()
        {
            if (actionQueue.ExecuteNextAction(comboQueue))
            {
                return;
            }

            if (q.UseOnCombo && q.IsReady())
            {
                if (CastQ(true))
                {
                    return;
                }
            }

            if (w.UseOnCombo && w.IsReady()
                && TargetSelector.GetTarget(player.AttackRange + 30F, TargetSelector.DamageType.Physical) != null)
            {
                AIHeroClient target = TargetSelector.GetTarget(player.AttackRange + 30F, TargetSelector.DamageType.Physical);

                if (target != null)
                {
                    actionQueue.EnqueueAction(comboQueue, () => true, () => w.Cast(), () => !w.IsReady());
                    actionQueue.EnqueueAction(comboQueue, () => true, () => olafItems.UseItems(target), () => true);
                    actionQueue.EnqueueAction(
                        comboQueue,
                        () => true,
                        () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target),
                        () =>
                        autoAttackCheck.Status || player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target));
                    return;
                }
            }

            if (e.UseOnCombo && e.IsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(e.Range, e.DamageType);

                if (target != null)
                {
                    actionQueue.EnqueueAction(
                        comboQueue,
                        () => true,
                        () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target),
                        () => autoAttackCheck.Status);
                    actionQueue.EnqueueAction(comboQueue, () => true, () => olafItems.UseItems(target), () => true);
                    actionQueue.EnqueueAction(comboQueue, () => true, () => CastE(target), () => !e.IsReady());
                    actionQueue.EnqueueAction(
                        comboQueue,
                        () => true,
                        () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target),
                        () =>
                        autoAttackCheck.Status || player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target));
                }
            }
        }

        private void Harass()
        {
            if (actionQueue.ExecuteNextAction(harasQueue))
            {
                return;
            }

            LastHit();

            List<AIHeroClient> blackList = olafMenu.GetHarasBlockList();

            if (q.UseOnHarass && q.IsReady() && CheckManaToHaras())
            {
                if (CastQ(false, blackList))
                {
                    return;
                }
            }

            if (w.UseOnHarass && w.IsReady() && CheckManaToHaras() 
                && TargetSelector.GetTarget(player.AttackRange + 30F, TargetSelector.DamageType.Physical, true, blackList) != null)
            {
                AIHeroClient target = TargetSelector.GetTarget(player.AttackRange + 30F, TargetSelector.DamageType.Physical, true, blackList);

                if (target != null)
                {
                    actionQueue.EnqueueAction(harasQueue, () => true, () => w.Cast(), () => !w.IsReady());
                    actionQueue.EnqueueAction(harasQueue, () => true, () => olafItems.UseHarasItems(), () => true);
                    actionQueue.EnqueueAction(harasQueue, () => true, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target), () => autoAttackCheck.Status || player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target));
                }
            }

            if (e.UseOnHarass && e.IsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(e.Range, e.DamageType, true, blackList);

                if (target != null)
                {
                    actionQueue.EnqueueAction(harasQueue, () => true, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target), () => autoAttackCheck.Status);
                    actionQueue.EnqueueAction(harasQueue, () => true, () => olafItems.UseHarasItems(), () => true);
                    actionQueue.EnqueueAction(harasQueue, () => true, () => EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position), () => true);
                    actionQueue.EnqueueAction(harasQueue, () => true, () => CastE(target), () => !e.IsReady());
                    actionQueue.EnqueueAction(harasQueue, () => true, () => Orbwalking.ResetAutoAttackTimer(), () => true);
                    actionQueue.EnqueueAction(harasQueue, () => true, () => EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position), () => true);
                    actionQueue.EnqueueAction(harasQueue, () => true, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target), () => autoAttackCheck.Status);
                }
            }
        }

        private bool CheckManaToHaras()
        {
            return player.ManaPercent > olafMenu.GetParamSlider("koreanolaf.harasmenu.manalimit");
        }

        private void LaneClear()
        {
            LastHit();

            if (actionQueue.ExecuteNextAction(laneClearQueue))
            {
                return;
            }

            if (q.UseOnLaneClear && q.IsReady() && CheckManaToLaneClear())
            {
                MinionManager.FarmLocation farmLocation = q.GetLineFarmLocation(MinionManager.GetMinions(q.Range));
                if (farmLocation.MinionsHit >= olafMenu.GetParamSlider("koreanolaf.laneclearmenu.useqif"))
                {
                    var target = q.GetCollision(player.Position.To2D(), new List<Vector2>() { farmLocation.Position })
                        .OrderByDescending(minion => player.Distance(minion))
                        .FirstOrDefault();

                    if (target != null)
                    {
                        q.Cast(target.Position);
                        return;
                    }
                }
                else
                {
                    Obj_AI_Base jungleMob =
                        MinionManager.GetMinions(q.Range / 1.5F, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                            .FirstOrDefault();

                    if (jungleMob != null)
                    {
                        q.Cast(q.GetPrediction(jungleMob).CastPosition);
                    }
                }
            }

            if (w.UseOnLaneClear && w.IsReady() && CheckManaToLaneClear())
            {
                if (MinionManager.GetMinions(300F).Count() >= 3)
                {
                    actionQueue.EnqueueAction(laneClearQueue, () => true, () => w.Cast(), () => !w.IsReady());
                    return;
                }
                else if (
                    MinionManager.GetMinions(
                        300F,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).Any())
                {
                    w.Cast();
                }
            }

            if (e.UseOnLaneClear && e.IsReady() && CheckHealthToLaneClear())
            {
                Obj_AI_Base target =
                    MinionManager.GetMinions(e.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth)
                        .FirstOrDefault();

                if (target != null)
                {
                    actionQueue.EnqueueAction(laneClearQueue, () => true, () => CastE(target), () => !e.IsReady());
                    return;
                }
            }

            if ((olafMenu.GetParamBool("koreanolaf.laneclearmenu.items.hydra")
                || olafMenu.GetParamBool("koreanolaf.laneclearmenu.items.tiamat")) &&
                (MinionManager.GetMinions(300F).Count() >= olafMenu.GetParamSlider("koreanolaf.laneclearmenu.items.when")
                || MinionManager.GetMinions(300F, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Any()))
            {
                actionQueue.EnqueueAction(laneClearQueue, () => true, () => olafItems.UseItemsLaneClear(), () => true);
                
            }
        }

        private bool CheckManaToLaneClear()
        {
            return player.ManaPercent > olafMenu.GetParamSlider("koreanolaf.laneclearmenu.manalimit");
        }
         
        private bool CheckHealthToLaneClear()
        {
            return player.HealthPercent > olafMenu.GetParamSlider("koreanolaf.laneclearmenu.healthlimit");
        }

        private void LastHit()
        {
            if (q.UseOnLastHit && q.IsReady() && CheckManaToLastHit())
            {
                Obj_AI_Base target =
                    MinionManager.GetMinions(q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth)
                        .FirstOrDefault(
                            minion =>
                            minion.Distance(player) > Orbwalking.GetRealAutoAttackRange(minion) + 10F
                            && !minion.IsDead
                            && q.GetDamage(minion) > minion.Health);

                if (target != null)
                {
                    PredictionOutput predictionOutput = q.GetPrediction(target);
                    q.Cast(predictionOutput.CastPosition);
                }
            }

            if (e.UseOnLastHit && e.IsReady())
            {
                Obj_AI_Base target =
                    MinionManager.GetMinions(e.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth)
                        .FirstOrDefault(minion => e.GetDamage(minion) > minion.Health);

                if (target != null 
                    && olafMenu.GetParamBool("koreanolaf.miscmenu.savee")
                    && MinionManager.GetMinions(e.Range * 2)
                           .Any(minion => minion.Name.ToLowerInvariant().Contains("cannon") 
                                && !minion.IsDead)
                    && !target.Name.ToLowerInvariant().Contains("cannon"))
                {
                    return;
                }

                if (target != null)
                {
                    e.Cast(target);
                }
                else if (
                    MinionManager.GetMinions(
                        e.Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth).Any())
                {
                    Obj_AI_Base jungleTarget =
                        MinionManager.GetMinions(
                            e.Range,
                            MinionTypes.All,
                            MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth).FirstOrDefault(minion => e.GetDamage(minion) > minion.Health);
                    if (jungleTarget != null)
                    {
                        e.Cast(jungleTarget);
                    }
                }
            }
        }

        private bool CheckManaToLastHit()
        {
            return player.ManaPercent > olafMenu.GetParamSlider("koreanolaf.lasthitmenu.manalimit");
        }

        public float ComboDamage(AIHeroClient target)
        {
            float result = q.UseOnCombo && q.IsReady() ? q.GetDamage(target): 0F;

            result += e.UseOnCombo && e.IsReady() ? e.GetDamage(target) : 0F;

            return result;
        }

        public void ForceUltimate(AIHeroClient target = null)
        {
            if (target != null && r.CanCast(target))
            {
                r.Cast(target);
            }
            else
            {
                r.Cast(TargetSelector.GetTarget(r.Range, r.DamageType));
            }
        }
    }
}