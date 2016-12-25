using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Spells
{
    using System.Linq;
    using System.Collections.Generic;
    using myCommon;
    using SharpDX;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellManager : Logic
    {
        internal const int PassiveRange = 340;

        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 625f, TargetSelector.DamageType.Magical);
            W = new Spell(SpellSlot.W, 300f, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 725f, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 550f, TargetSelector.DamageType.Magical);

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
                    var ePos = GetEPosition(target);

                    if (ePos != Vector3.Zero && ePos.DistanceToPlayer() <= E.Range && CanCastE(ePos, target))
                    {
                        if (Menu.GetBool("Humanizer"))
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(Menu.GetSlider("HumanizerD"), () => E.Cast(ePos, true));
                        }
                        else
                        {
                            E.Cast(ePos, true);
                        }
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
                    var ePos = GetEPosition(target);

                    if (ePos != Vector3.Zero && ePos.DistanceToPlayer() <= E.Range && CanCastE(ePos, target))
                    {
                        if (Menu.GetBool("Humanizer"))
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(Menu.GetSlider("HumanizerD"), () => E.Cast(ePos, true));
                        }
                        else
                        {
                            E.Cast(ePos, true);
                        }
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

        internal static Vector3 GetEPosition(AIHeroClient target)
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
                        return obj.Position;
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
                        return obj.Position;
                    }
                }
            }
            else if (target.DistanceToPlayer() <= E.Range - 130)
            {
                return Me.Position.Extend(target.Position, target.DistanceToPlayer() + 130);
            }
            else if (target.IsValidTarget(E.Range))
            {
                return target.Position;
            }
            else
            {
                return Vector3.Zero;
            }

            return Vector3.Zero;
        }

        internal static bool CanCastE(Vector3 pos, AIHeroClient target)
        {
            if (pos == Vector3.Zero || target == null || target.IsDead)
            {
                return false;
            }

            if (Menu.GetList("Eturret") == 0 && pos.UnderTurret(true))
            {
                return false;
            }

            if (Menu.GetList("Eturret") == 1 && pos.UnderTurret(true))
            {
                if (Me.HealthPercent <= Menu.GetSlider("EturretHP") &&
                    target.Health > DamageCalculate.GetComboDamage(target)*0.85)
                {
                    return false;
                }
            }

            if (Menu.GetBool("LogicE"))
            {
                if (HeroManager.Enemies.Count(x => x.Distance(pos) <= R.Range) >= 3)
                {
                    if (HeroManager.Enemies.Count(x => x.Distance(pos) <= R.Range) == 3)
                    {
                        if (HeroManager.Enemies.Count(x => x.Health < DamageCalculate.GetComboDamage(target)*1.45) <= 2)
                        {
                            return false;
                        }
                    }
                    else if (HeroManager.Enemies.Count(x => x.Distance(pos) <= R.Range) == 4)
                    {
                        if (HeroManager.Enemies.Count(x => x.Health < DamageCalculate.GetComboDamage(target)*1.45) < 2)
                        {
                            return false;
                        }
                    }
                    else if (HeroManager.Enemies.Count(x => x.Distance(pos) <= R.Range) == 5)
                    {
                        if (HeroManager.Enemies.Count(x => x.Health < DamageCalculate.GetComboDamage(target)*1.45) < 3)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        if (target.Health >
                            (HeroManager.Allies.Any(x => x.DistanceToPlayer() <= E.Range)
                                ? DamageCalculate.GetComboDamage(target) + Me.Level*45
                                : DamageCalculate.GetComboDamage(target)))
                        {
                            return false;
                        }
                    }
                    else if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    {
                        if (pos.UnderTurret(true))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
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
