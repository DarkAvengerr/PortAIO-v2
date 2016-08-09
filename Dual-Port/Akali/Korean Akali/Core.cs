using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KoreanCommon;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAkali
{
    internal class Core : CommonCore
    {
        private List<QueueItem> comboQueue;

        public Core(CommonChampion akali)
            : base(akali)
        {
            comboQueue = new List<QueueItem>();
        }

        private void UseQ(Obj_AI_Base target)
        {
            if (target != null && Q.IsReady() && Q.IsInRange(target))
            {
                Q.Cast(target);
            }
        }

        public override void LastHitMode()
        {
            if (!KoreanUtils.GetParamBool(champion.MainMenu, "useqtofarm"))
            {
                return;
            }

            Obj_AI_Base target =
                MinionManager.GetMinions(champion.Player.Position, Q.Range)
                    .Where(
                        minion =>
                            !champion.Player.CanAttack ||
                            (champion.Player.CanAttack &&
                             champion.Player.Distance(minion) > Orbwalking.GetRealAutoAttackRange(champion.Player)))
                    .FirstOrDefault(
                        minion =>
                            Q.GetDamage(minion) >
                            HealthPrediction.GetHealthPrediction(minion,
                                (int) (champion.Player.Distance(minion)/Q.Speed)*1000, (int) Q.Delay*1000));

            if (target != null)
            {
                UseQ(target);
            }
        }

        public override void HarasMode()
        {
            AIHeroClient target;

            if (Q.UseOnHaras && Q.IsReady() &&
                champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "minenergytoharas"))
            {
                target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }

            if (E.UseOnHaras && E.IsReady() &&
                champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "minenergytoharas"))
            {
                target = TargetSelector.GetTarget(E.Range, E.DamageType);
                if (target != null)
                {
                    E.Cast();
                }
            }

            LastHitMode();
        }

        public override void LaneClearMode()
        {
            if (Q.UseOnLaneClear && Q.IsReady() &&
                champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "minenergytolaneclear"))
            {
                if (KoreanUtils.GetParamBool(champion.MainMenu, "saveqtofarm"))
                {
                    LastHitMode();
                }
                else
                {
                    Q.Cast(MinionManager.GetMinions(Q.Range).FirstOrDefault());
                }
            }

            if (E.UseOnLaneClear && E.IsReady() &&
                champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "minenergytolaneclear") &&
                MinionManager.GetMinions(E.Range).Count >=
                KoreanUtils.GetParamSlider(champion.MainMenu, "minminionstoe"))
            {
                E.Cast();
            }
        }

        private bool IsOffensiveItemsReady()
        {
            if (KoreanUtils.GetParamBool(champion.MainMenu, "usebilgewatercutlass") &&
                ItemData.Bilgewater_Cutlass.GetItem().IsReady())
            {
                return true;
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "usehextechgunblade") &&
                ItemData.Hextech_Gunblade.GetItem().IsReady())
            {
                return true;
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "usebotrk") &&
                ItemData.Blade_of_the_Ruined_King.GetItem().IsReady())
            {
                return true;
            }

            return false;
        }

        private void UseOffensiveItems(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "usebilgewatercutlass") &&
                ItemData.Bilgewater_Cutlass.GetItem().IsReady())
            {
                ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "usehextechgunblade") &&
                ItemData.Hextech_Gunblade.GetItem().IsReady())
            {
                ItemData.Hextech_Gunblade.GetItem().Cast(target);
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "usebotrk") &&
                ItemData.Blade_of_the_Ruined_King.GetItem().IsReady())
            {
                ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
            }
        }

        private bool FullComboIsReady()
        {
            if (Q.UseOnCombo && !Q.IsReady())
                return false;
            if (W.UseOnCombo && !W.IsReady())
                return false;
            if (E.UseOnCombo && !E.IsReady())
                return false;
            if (R.UseOnCombo && (!E.IsReady() || R.Instance.Ammo == 1))
                return false;

            return true;
        }

        struct QueueItem
        {
            public Func<bool> PreConditionFunc;
            public Func<bool> ConditionToRemoveFunc;
            public Action ComboAction;
        }

        private void EnqueueAction(Func<bool> preCondition, Action comboAction, Func<bool> conditionToRemove)
        {
            comboQueue.Add(new QueueItem()
            {
                PreConditionFunc = preCondition,
                ComboAction = comboAction,
                ConditionToRemoveFunc = conditionToRemove
            });
        }

        private void EnqueueSpell(CommonSpell spell, AIHeroClient target)
        {
            if (!spell.UseOnCombo)
                return;

            AIHeroClient selectedTarget = target ?? TargetSelector.GetTarget(spell.Range, spell.DamageType);

            if (selectedTarget == null)
                return;

            if (spell.IsInRange(selectedTarget))
            {
                EnqueueAction(
                    () => spell.IsReadyToCastOn(selectedTarget),
                    () => spell.Cast(selectedTarget),
                    () => !spell.IsReadyToCastOn(selectedTarget) || selectedTarget.IsDead);
            }
        }

        private void EnqueueItems(AIHeroClient target)
        {
            if (target == null)
                return;

            EnqueueAction(
                () => true,
                () => UseOffensiveItems(target),
                () => true
                );
        }

        private bool ExecuteNextAction()
        {
            if (comboQueue.Count > 0)
            {
                if (comboQueue[0].PreConditionFunc.Invoke())
                {
                    comboQueue[0].ComboAction.Invoke();
                }

                if (comboQueue[0].ConditionToRemoveFunc.Invoke())
                {
                    comboQueue.Remove(comboQueue[0]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void ComboMode()
        {
            AIHeroClient target;

            if (R.UseOnCombo && R.IsReady())
            {
                target =
                    HeroManager.Enemies.FirstOrDefault(
                        champ =>
                            R.IsInRange(champ) &&
                            R.GetDamage(champ) + champion.Player.GetAutoAttackDamage(champ) > champ.Health);

                if (target != null)
                {
                    R.Cast(target);
                }
            }

            if (ExecuteNextAction())
            {
                return;
            }

            if (R.UseOnCombo && R.IsReady() && R.Instance.Ammo >= 2)
            {
                target = TargetSelector.GetTarget(R.Range, R.DamageType);

                if (target != null)
                {
                    EnqueueItems(target);

                    EnqueueSpell(R, target);
                    if (W.UseOnCombo && W.IsReady())
                    {
                        EnqueueSpell(W, target);
                    }
                    return;
                }
            }

            if (W.UseOnCombo && Q.UseOnCombo && W.IsReady() && Q.IsReady())
            {
                target = TargetSelector.GetTarget(Q.Range, Q.DamageType);

                if (target != null)
                {
                    EnqueueSpell(Q, target);
                    EnqueueSpell(W, target);
                    return;
                }
            }

            if (Q.UseOnCombo && Q.IsReady())
            {
                EnqueueSpell(Q, null);
                return;
            }

            if (E.UseOnCombo && E.IsReady())
            {
                EnqueueSpell(E, null);
                return;
            }

            if (IsOffensiveItemsReady())
            {
                target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(champion.Player) * 2, TargetSelector.DamageType.True);

                if (target != null)
                {
                    EnqueueItems(target);
                }
            }
        }

        public override void Ultimate(AIHeroClient target)
        {
            if (target == null)
            {
                AIHeroClient newTarget = TargetSelector.GetTarget(R.Range, R.DamageType, false);

                if (newTarget != null)
                {
                    R.Cast(newTarget);
                    EnqueueItems(newTarget);
                }
            }
            else if (R.IsReadyToCastOn(target))
            {
                R.Cast(target);
                EnqueueItems(target);
            }
        }

    }
}
