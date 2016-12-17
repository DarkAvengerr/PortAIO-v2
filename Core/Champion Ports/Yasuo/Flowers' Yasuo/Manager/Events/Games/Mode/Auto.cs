using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using Common;
    using System.Linq;
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;

    internal class Auto : Logic
    {
        internal static void Init()
        {
            if (Menu.Item("AutoR", true).GetValue<bool>() && R.IsReady())
            {
                var Enemies =
                    HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range))
                        .Count(x => x.HasBuffOfType(BuffType.Knockup) || x.HasBuffOfType(BuffType.Knockback));

                var Allies = HeroManager.Allies.Count(x => x.DistanceToPlayer() <= R.Range);

                if (Enemies >= Menu.Item("AutoRCount", true).GetValue<Slider>().Value &&
                    Me.HealthPercent >= Menu.Item("AutoRMyHp", true).GetValue<Slider>().Value &&
                    Allies >= Menu.Item("AutoRRangeCount", true).GetValue<Slider>().Value)
                {
                    R.Cast();
                }
            }

            if (Menu.Item("AutoQ", true).GetValue<KeyBind>().Active && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None &&
                !Me.UnderTurret(true))
            {
                if (Menu.Item("AutoQ3", true).GetValue<bool>() && Q3.IsReady() && SpellManager.HaveQ3)
                {
                    SpellManager.CastQ3();
                }
                else if (!SpellManager.HaveQ3 && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                    if (target.IsValidTarget(Q.Range))
                    {
                        var qPred = Q.GetPrediction(target, true);

                        if (qPred.Hitchance >= HitChance.VeryHigh)
                        {
                            Q.Cast(qPred.CastPosition, true);
                        }
                    }
                }
            }

            if (Menu.Item("StackQ", true).GetValue<KeyBind>().Active && Q.IsReady() && !SpellManager.HaveQ3 && 
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && !Me.UnderTurret(true))
            {
                var minions = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly);

                if (minions.Any())
                {
                    var qFarm =
                        MinionManager.GetBestLineFarmLocation(
                            minions.Select(x => x.Position.To2D()).ToList(), Q.Width, Q.Range);

                    if (qFarm.MinionsHit >= 1)
                    {
                        Q.Cast(qFarm.Position, true);
                    }
                }
            }
        }
    }
}
