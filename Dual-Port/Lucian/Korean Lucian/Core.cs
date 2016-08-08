using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanLucian
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KoreanCommon;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Common.Data;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    using SharpDX;

    internal class Core : CommonCore
    {
        private readonly object lockObject;

        private readonly Lucian lucian;

        private bool hasPassive;

        public Core(Lucian lucian)
            : base(lucian)
        {
            this.lucian = lucian;

            lockObject = new object();

            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Spellbook.OnCastSpell += LockR;
            Spellbook.OnCastSpell += PassiveControl;
        }

        private void PassiveControl(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                ProcessSpell();
            }
        }

        private void ProcessSpell()
        {
            hasPassive = true;
            LeagueSharp.Common.Utility.DelayAction.Add(450, Orbwalking.ResetAutoAttackTimer);
        }

        private void LockR(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot != SpellSlot.R)
            {
                return;
            }

            lock (lockObject)
            {
                Spellbook.OnCastSpell -= LockR;

                if (KoreanUtils.GetParamBool(champion.MainMenu, "useyoumuu")
                    && ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                {
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
                }

                if (KoreanUtils.GetParamBool(champion.MainMenu, "lockr"))
                {
                    AIHeroClient target = null;

                    if (TargetSelector.SelectedTarget != null
                        && TargetSelector.SelectedTarget.LSDistance(champion.Player.Position) < R.Range + 400F)
                    {
                        target = TargetSelector.SelectedTarget;
                    }
                    else
                    {
                        target = TargetSelector.GetTarget(
                            champion.Player,
                            R.Range + 400f,
                            TargetSelector.DamageType.Physical);
                    }

                    if (target != null)
                    {
                        args.Process = false;
                        if (R.Cast(target.Position))
                        {
                            ProcessSpell();
                        }
                    }
                }

                Spellbook.OnCastSpell += LockR;
            }
        }

        private bool CheckPassive()
        {
            return hasPassive;
        }

        private bool CheckHaras(AIHeroClient target)
        {
            if (target == null)
            {
                return false;
            }
            return KoreanUtils.GetParamBool(lucian.MainMenu, target.ChampionName.ToLowerInvariant());
        }

        //Maybe next feature
        //private bool IsJungleMob(Obj_AI_Base target)
        //{
        //    return (target.BaseSkinName.ToLowerInvariant() == "sru_blue"
        //            || target.BaseSkinName.ToLowerInvariant() == "sru_gromp"
        //            || target.BaseSkinName.ToLowerInvariant() == "sru_murkwolf"
        //            || target.BaseSkinName.ToLowerInvariant() == "sru_razorbeak"
        //            || target.BaseSkinName.ToLowerInvariant() == "sru_baron"
        //            || target.BaseSkinName.ToLowerInvariant() == "sru_dragon"
        //            || target.BaseSkinName.ToLowerInvariant() == "sru_red"
        //            || target.BaseSkinName.ToLowerInvariant() == "sru_crab"
        //            || target.BaseSkinName.ToLowerInvariant() == "sru_krug");
        //}

        public bool HaveManaToHaras()
        {
            return champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "manalimittoharas");
        }

        public bool HaveManaToLaneclear()
        {
            return champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "manalimittolaneclear");
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                hasPassive = false;
            }
        }

        public override void LastHitMode()
        {
            //no needed
        }

        public override void HarasMode()
        {
            lock (lockObject)
            {
                AIHeroClient target;

                if (Q.UseOnHaras && Q.LSIsReady() && Q.CanCast() && HaveManaToHaras())
                {
                    if (!KoreanUtils.GetParamKeyBind(champion.MainMenu, "toggleextendedq"))
                    {
                        if (champion.CastExtendedQ(true))
                        {
                            ProcessSpell();
                        }
                    }

                    if (!CheckPassive())
                    {
                        target = TargetSelector.GetTarget(champion.Player, Q.Range, TargetSelector.DamageType.Physical);

                        if (!CheckHaras(target))
                        {
                            return;
                        }

                        if (target != null && Q.IsReadyToCastOn(target) && Q.CanCast(target))
                        {
                            if (Q.CastOnUnit(target))
                            {
                                ProcessSpell();
                            }
                        }
                    }
                }

                if (W.UseOnHaras && !CheckPassive() && HaveManaToHaras() && W.LSIsReady() && W.CanCast())
                {
                    target = TargetSelector.GetTarget(champion.Player, W.Range, TargetSelector.DamageType.Physical);

                    if (!CheckHaras(target))
                    {
                        return;
                    }

                    if (target != null
                        && target.LSDistance(champion.Player) <= Orbwalking.GetRealAutoAttackRange(champion.Player))
                    {
                        if (W.Cast(target.Position))
                        {
                            ProcessSpell();
                        }
                    }
                    else
                    {
                        PredictionOutput wPrediction = W.GetPrediction(target);

                        if (wPrediction != null && wPrediction.Hitchance >= HitChance.High
                            && wPrediction.CastPosition != Vector3.Zero)
                        {
                            if (W.Cast(wPrediction.CastPosition))
                            {
                                ProcessSpell();
                            }
                        }
                    }
                }

                if (E.UseOnHaras && lucian.semiAutomaticE.Holding && E.LSIsReady() && E.CanCast() && !CheckPassive()
                    && ((E.Instance.SData.Mana > 0 && HaveManaToHaras()) || E.Instance.SData.Mana.Equals(0f)))
                {
                    target = TargetSelector.GetTarget(
                        champion.Player,
                        E.Range + Orbwalking.GetRealAutoAttackRange(champion.Player),
                        TargetSelector.DamageType.Physical);

                    if (!CheckHaras(target))
                    {
                        return;
                    }

                    if (target != null && lucian.semiAutomaticE.Cast(target))
                    {
                        ProcessSpell();
                    }
                }
            }
        }

        public override void LaneClearMode()
        {
            if (CheckPassive())
            {
                return;
            }

            lock (lockObject)
            {
                if (Q.UseOnLaneClear && Q.LSIsReady() && Q.CanCast() && !CheckPassive() && HaveManaToLaneclear()
                    && champion.CastExtendedQToLaneClear())
                {
                    ProcessSpell();
                }

                if (W.UseOnLaneClear && W.LSIsReady() && W.CanCast() && !CheckPassive() && HaveManaToHaras())
                {
                    List<Obj_AI_Base> minions = MinionManager.GetMinions(W.Range).ToList();
                    MinionManager.FarmLocation farmLocation = W.GetCircularFarmLocation(minions);

                    int minMinions = KoreanUtils.GetParamSlider(champion.MainMenu, "wcounthit");

                    if (farmLocation.MinionsHit >= minMinions && minions.Count >= minMinions)
                    {
                        if (W.Cast(farmLocation.Position))
                        {
                            ProcessSpell();
                        }
                    }
                }

                if (E.UseOnLaneClear && lucian.semiAutomaticE.Holding && E.LSIsReady() && E.CanCast() && !CheckPassive()
                    && (E.Instance.SData.Mana.Equals(0) || (E.Instance.SData.Mana > 0 && HaveManaToHaras())))
                {
                    Obj_AI_Base target =
                        MinionManager.GetMinions(E.Range + Orbwalking.GetRealAutoAttackRange(champion.Player))
                            .FirstOrDefault(
                                minion =>
                                champion.Player.LSDistance(minion) > Orbwalking.GetRealAutoAttackRange(champion.Player)
                                && Game.CursorPos.LSDistance(minion.Position) < champion.Player.LSDistance(minion));

                    if (target != null && lucian.semiAutomaticE.Cast(target))
                    {
                        ProcessSpell();
                        champion.Orbwalker.ForceTarget(target);
                    }
                }
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "harasonlaneclear"))
            {
                HarasMode();
            }
        }

        public override void ComboMode()
        {
            //IDK why but i need this code since the patch 5.17
            if (champion.Player.IsChannelingImportantSpell())
            {
                Orbwalking.MoveTo(Game.CursorPos);
                return;
            }

            AIHeroClient target;

            if (W.UseOnCombo && !CheckPassive() && W.LSIsReady() && W.CanCast())
            {
                target = TargetSelector.GetTarget(champion.Player, W.Range, TargetSelector.DamageType.Physical);

                PredictionOutput wPrediction = W.GetPrediction(target);

                if (wPrediction != null && wPrediction.Hitchance >= HitChance.High
                    && wPrediction.CastPosition != Vector3.Zero)
                {
                    if (W.Cast(wPrediction.CastPosition))
                    {
                        ProcessSpell();
                    }
                }
                else if (target != null
                         && target.LSDistance(champion.Player) <= Orbwalking.GetRealAutoAttackRange(champion.Player))
                {
                    if (W.Cast(target.ServerPosition))
                    {
                        ProcessSpell();
                    }
                }
            }

            if (Q.UseOnCombo && Q.LSIsReady() && Q.CanCast())
            {
                if (!CheckPassive())
                {
                    target = TargetSelector.GetTarget(champion.Player, Q.Range, TargetSelector.DamageType.Physical);

                    if (target != null && Q.IsReadyToCastOn(target) && Q.CanCast(target))
                    {
                        if (Q.CastOnUnit(target))
                        {
                            ProcessSpell();
                        }
                    }
                }

                if (champion.CastExtendedQ())
                {
                    ProcessSpell();
                }
            }

            if (E.UseOnCombo && !CheckPassive() && E.LSIsReady() && E.CanCast()
                && (!KoreanUtils.GetParamBool(champion.MainMenu, "dashmode")
                    || (KoreanUtils.GetParamBool(champion.MainMenu, "dashmode") && lucian.semiAutomaticE.Holding)))
            {
                target = TargetSelector.GetTarget(
                    E.Range + Orbwalking.GetRealAutoAttackRange(champion.Player) - 25f,
                    TargetSelector.DamageType.Physical);

                if (target != null && target.IsEnemy && target.IsValid && !target.IsDead && lucian.semiAutomaticE.Cast(target))
                {
                    ProcessSpell();
                }
            }
        }

        public override void Ultimate(AIHeroClient target)
        {
            if (target == null)
            {
                R.Cast(Game.CursorPos);
            }
            else if (KoreanUtils.GetParamBool(champion.MainMenu, "lockr"))
            {
                R.Cast(target.Position);
            }
        }
    }
}