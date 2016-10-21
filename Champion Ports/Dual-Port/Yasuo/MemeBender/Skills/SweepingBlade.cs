using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoTheLastMemebender.Skills
{
    internal static class SweepingBlade
    {
        public static Spell E = new Spell(SpellSlot.E, 475f);

        public static int Stacks
        {
            get
            {
                var buff = ObjectManager.Player.Buffs.First(b => b.Name == "yasuodashscalar");
                return buff?.Count ?? 0;
            }
        }

        public static bool CanCastE(Obj_AI_Base target, bool ignoreMenu = false)
        {
            var endPos = EndPos(target);
            if ((endPos.UnderTurret(true) && YasuoUtils.DangerousTurret(YasuoUtils.GetNearestTurret(endPos))) ||
                target.Distance(ObjectManager.Player, true) >= E.RangeSqr || target.HasBuff("YasuoDashWrapper"))
            {
                return false;
            }
            if (!Config.Param<bool>("ylm.spelle.range") || ignoreMenu) return true;
            var eRangeC = Config.Param<Slider>("ylm.spelle.rangeslider").Value;
            return (target.Distance(endPos, true) < eRangeC*eRangeC);
        }

        public static Vector3 EndPos(Vector3 position)
        {
            return ObjectManager.Player.ServerPosition.Extend(position, E.Range);
        }

        public static Vector3 EndPos(Obj_AI_Base unit)
        {
            return EndPos(unit.ServerPosition);
        }

        public static void JungleClearE()
        {

            if (!E.IsReady() || !Config.Param<bool>("ylm.jungleclear.usee"))
            {
                return;
            }

            var minion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All,
                    MinionTeam.Neutral).FirstOrDefault(m => CanCastE(m) && E.GetHealthPrediction(m) > 0);

            if (minion == null)
            {
                return;
            }
            E.Cast(minion);

        }

        public static void LaneE(bool lastHit = false)
        {
            if (!E.IsReady() || (!Config.Param<bool>("ylm.laneclear.usee") && !Config.Param<bool>("ylm.lasthit.usee")))
            {
                return;
            }
            lastHit = lastHit && Config.Param<bool>("ylm.lasthit.usee");
            var minion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range)
                    .FirstOrDefault(m => CanCastE(m) /*&& E.GetHealthPrediction(m) > 0*/);

            if (minion == null || !lastHit && !Config.Param<bool>("ylm.laneclear.usee"))
            {
                return;
            }

            var mode = Config.Param<StringList>("ylm.laneclear.modee").SelectedIndex;
            if (((lastHit || mode == 0) && E.GetHealthPrediction(minion) <= E.GetDamage(minion))
                || mode == 1)
            {
                E.Cast(minion);
            }
        }

        public static void GapClose(Obj_AI_Base target = null, bool escape = false)
        {
            if (!Config.Param<bool>("ylm.gapclose.on")
                ||
                (Config.Param<bool>("ylm.gapclose.hpcheck") &&
                 Config.Param<Slider>("ylm.gapclose.hpcheck2").Value > ObjectManager.Player.HealthPercent))
            {
                return;
            }

            if (target == null)
            {
                target = TargetSelector.GetTarget(Config.Param<Slider>("ylm.gapclose.limit").Value,
                    TargetSelector.DamageType.Magical);
                if (target == null)
                {
                    return;
                }
            }
            if (ObjectManager.Player.IsDashing() && Config.Param<bool>("ylm.gapclose.stackQ") && !SteelTempest.Empowered 
                && !CanCastE(target))
            {
                SteelTempest.QDash.Cast();
            }
            var distTarget = ObjectManager.Player.Distance(target, true);

            if (!E.IsReady())
            {
                return;
            }
            if (escape)
            {
                E.Cast(target);
            }
            if (distTarget <= ObjectManager.Player.AttackRange*ObjectManager.Player.AttackRange)
            {
                return;
            }
            /*var dashUnit = (from o in ObjectManager.Get<Obj_AI_Base>()
                             where o.IsValidTarget() && o.IsEnemy && (o.IsMinion || o.IsChampion())
                             let distance = o.Distance(ObjectManager.Player, true)
                             let endPos = EndPos(o.ServerPosition)
                             where
                                distance < E.RangeSqr && !endPos.UnderTurret(true) && o.Distance(target, true) < distance
                                && endPos.Distance(target.ServerPosition, true) < distTarget && !o.HasBuff("YasuoDashWrapper")
                             select o).FirstOrDefault();*/ // How to pull a chewymoon :^ )

            foreach (
                var unit in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(o => o.IsEnemy && o.IsValidTarget() && (o.IsMinion || o.IsChampion())))
            {
                var distance = unit.Distance(ObjectManager.Player, true);
                var endPos = EndPos(unit.ServerPosition);

                if (distance < E.RangeSqr &&
                    (!endPos.UnderTurret(true) || !YasuoUtils.DangerousTurret(YasuoUtils.GetNearestTurret(endPos))) &&
                    unit.Distance(target, true) < distance
                    && endPos.Distance(target.ServerPosition, true) < distTarget && !unit.HasBuff("YasuoDashWrapper"))
                {
                    E.Cast(unit);
                    return;
                }
            }
        }

        public static void RunAway(Obj_AI_Base from)
        {
            var minions =
                MinionManager.GetMinions(E.Range)
                    .Where(m => !EndPos(m).UnderTurret(true) && !m.HasBuff("YasuoDashWrapper"));
            minions.OrderBy(m => m.Distance(from, true));
            var furthestMinion = minions.LastOrDefault();
            if (furthestMinion != null)
            {
                E.Cast(from);
            }
        }
    }
}