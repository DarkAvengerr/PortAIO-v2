using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Spells
{
    using System.Linq;
    using System.Collections.Generic;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellManager : Logic
    {
        internal const int PassiveRange = 340;

        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 300f);
            E = new Spell(SpellSlot.E, 725f);
            R = new Spell(SpellSlot.R, 550f);

            R.SetCharged(550, 550, 1.0f);

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        internal static void QEWLogic(bool useQ, bool useW, bool useE)
        {
            var target = TargetSelector.GetTarget(E.Range + 300f, TargetSelector.DamageType.Magical);

            if (target.Check(E.Range + 300f))
            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(target, true);
                }

                if (useE && E.IsReady() && target.IsValidTarget(E.Range + 300f) && !Q.IsReady())
                {
                    if (Daggers.Any(
                        x =>
                            HeroManager.Enemies.Any(a => a.Distance(x.Position) <= PassiveRange) &&
                            x.Position.DistanceToPlayer() <= E.Range))
                    {
                        foreach (
                            var obj in
                            Daggers.Where(x => x.Position.Distance(target.Position) <= PassiveRange)
                                .OrderByDescending(x => x.Position.Distance(target.Position)))
                        {
                            if (obj.Dagger != null && obj.Dagger.IsValid && obj.Position.DistanceToPlayer() <= E.Range)
                            {
                                E.Cast(obj.Position, true);
                            }
                        }
                    }
                    else if (
                        Daggers.Any(
                            x =>
                                HeroManager.Enemies.Any(a => a.Distance(x.Position) <= E.Range) &&
                                x.Position.DistanceToPlayer() <= E.Range))
                    {
                        foreach (
                            var obj in
                            Daggers.Where(x => x.Position.Distance(target.Position) <= E.Range)
                                .OrderBy(x => x.Position.Distance(target.Position)))
                        {
                            if (obj.Dagger != null && obj.Dagger.IsValid && obj.Position.DistanceToPlayer() <= E.Range)
                            {
                                E.Cast(obj.Position, true);
                            }
                        }
                    }
                    else if (target.DistanceToPlayer() <= E.Range + 130)
                    {
                        var pos = Me.Position.Extend(target.Position, target.DistanceToPlayer() + 130);

                        E.Cast(pos, true);
                    }
                    else if (target.IsValidTarget(E.Range))
                    {
                        E.Cast(target, true);
                    }
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        internal static void EQWLogic(bool useQ, bool useW, bool useE)
        {
            var target = TargetSelector.GetTarget(E.Range + 300f, TargetSelector.DamageType.Magical);

            if (target.Check(E.Range + 300f))
            {
                if (useE && E.IsReady() && target.IsValidTarget(E.Range + 300f))
                {
                    if (Daggers.Any(
                        x =>
                            HeroManager.Enemies.Any(a => a.Distance(x.Position) <= PassiveRange) &&
                            x.Position.DistanceToPlayer() <= E.Range))
                    {
                        foreach (
                            var obj in
                            Daggers.Where(x => x.Position.Distance(target.Position) <= PassiveRange)
                                .OrderByDescending(x => x.Position.Distance(target.Position)))
                        {
                            if (obj.Dagger != null && obj.Dagger.IsValid && obj.Position.DistanceToPlayer() <= E.Range)
                            {
                                E.Cast(obj.Position, true);
                            }
                        }
                    }
                    else if (
                        Daggers.Any(
                            x =>
                                HeroManager.Enemies.Any(a => a.Distance(x.Position) <= E.Range) &&
                                x.Position.DistanceToPlayer() <= E.Range))
                    {
                        foreach (
                            var obj in
                            Daggers.Where(x => x.Position.Distance(target.Position) <= E.Range)
                                .OrderBy(x => x.Position.Distance(target.Position)))
                        {
                            if (obj.Dagger != null && obj.Dagger.IsValid && obj.Position.DistanceToPlayer() <= E.Range)
                            {
                                E.Cast(obj.Position, true);
                            }
                        }
                    }
                    else if (target.DistanceToPlayer() <= E.Range + 130)
                    {
                        var pos = Me.Position.Extend(target.Position, target.DistanceToPlayer() + 130);

                        E.Cast(pos, true);
                    }
                    else if(target.IsValidTarget(E.Range))
                    {
                        E.Cast(target, true);
                    }
                }

                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !E.IsReady())
                {
                    Q.CastOnUnit(target, true);
                }

                if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        internal static void CancelUlt(bool ignoreCheck = false)
        {
            if (!ignoreCheck && HeroManager.Enemies.Any(x => !x.IsDead && x.DistanceToPlayer() <= R.Range))
            {
                return;
            }

            if (Utils.TickCount - lastCancelTime > 5000)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Me.Position.Extend(Game.CursorPos, 100));
                lastCancelTime = Utils.TickCount;
            }
        }

        internal static bool isCastingUlt
            => Me.Buffs.Any(x => x.Name.ToLower().Contains("katarinar")) || Me.IsChannelingImportantSpell();

        internal static IEnumerable<GameObject> badaoFleeLogic
        {
            get
            {
                var Vinasun = new List<GameObject>();
                Vinasun.AddRange(
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(x => !(x is Obj_AI_Minion && MinionManager.IsWard((Obj_AI_Minion) x))));
                Vinasun.AddRange(
                    HeroManager.AllHeroes.Where(
                        unit =>
                            unit != null && unit.IsValid && !unit.IsDead && unit.IsTargetable &&
                            Me.Distance(unit) <= E.Range));
                Vinasun.AddRange(Daggers.Select(x => x.Dagger).ToList());
                return Vinasun;
            }
        }
    }
}
