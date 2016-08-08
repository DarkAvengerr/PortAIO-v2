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

    internal class KillSteal
    {
        private readonly Func<AIHeroClient, float> aaDamage;

        private readonly float aaRange;

        private readonly Spell e;

        private readonly Orbwalking.Orbwalker orbwalker;

        private readonly AIHeroClient player;

        private readonly Spell w;

        public KillSteal(CommonChampion champion)
        {
            orbwalker = champion.Orbwalker;
            player = champion.Player;
            e = champion.Spells.E;
            w = champion.Spells.W;
            aaRange = Orbwalking.GetRealAutoAttackRange(champion.Player);
            aaDamage = target => (float)champion.Player.LSGetAutoAttackDamage(target);

            if (KoreanUtils.GetParamBool(champion.MainMenu, "killsteal"))
            {
                Game.OnUpdate += KS;
            }
        }

        public void KS(EventArgs args)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                return;
            }

            List<AIHeroClient> targetList =
                ObjectManager.Get<AIHeroClient>()
                    .Where(target => target.HealthPercent < 20f && target.LSDistance(player) < w.Range && !target.IsDead)
                    .ToList();

            if (targetList.Count == 0)
            {
                return;
            }

            targetList =
                targetList.Where(
                    target =>
                    ((e.LSIsReady() && !target.IsDead && target.LSDistance(player) > aaRange
                      && target.LSDistance(player) < e.Range + aaRange && aaDamage(target) * 1.4f > target.Health)
                     || (w.LSIsReady() && w.CanCast(target) && w.IsKillable(target)))).ToList();

            if (targetList.Count == 0)
            {
                return;
            }

            foreach (AIHeroClient target in targetList)
            {
                if (w.LSIsReady() && w.CanCast(target) && w.IsKillable(target))
                {
                    w.Cast(target);
                }
                else
                {
                    e.Cast(target.Position);
                    orbwalker.ForceTarget(target);
                }
            }
        }
    }
}