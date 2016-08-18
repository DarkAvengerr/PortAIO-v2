using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoTheLastMemebender.Skills
{
    static class LastBreath
    {
        public static Spell R = new Spell(SpellSlot.R, 1300);

        public static bool ShouldUlt(AIHeroClient target)
        {

            var mode =
                Config.Param<StringList>(string.Format("ylm.spellr.rtarget.{0}", target.ChampionName.ToLower())).SelectedIndex;

            if (mode == 0 || !R.IsReady()|| (!Config.Param<bool>("ylm.spellr.towercheck") && target.UnderTurret(true)) 
                || !target.HasBuffOfType(BuffType.Knockup) || !target.HasBuffOfType(BuffType.Knockback))
                return false;

            switch (mode)
            {
                case 1:
                    return R.GetDamage(target) + SteelTempest.Q.GetDamage(target) > target.Health;
                case 2:
                    return true;
                case 3:
                    if (Config.Param<bool>("ylm.spellr.targethealth") &&
                        (target.HealthPercent > Config.Param<Slider>("ylm.spellr.targethealthslider").Value))
                        return false;

                    if (Config.Param<bool>("ylm.spellr.playerhealth") &&
                        (ObjectManager.Player.HealthPercent < Config.Param<Slider>("ylm.spellr.playerhealthslider").Value))
                        return false;

                    return true;
            }
            return false;
        }

        public static void CastR(AIHeroClient target)
        {
            if (Config.Param<bool>("ylm.spellr.delay"))
            {

                var buff = target.Buffs.FirstOrDefault(b=>b.Type == BuffType.Knockup || b.Type == BuffType.Knockback);
                if (buff == null)
                {
                    return;
                }
                var delayTime = buff.EndTime - Game.Time - Game.Ping/2;
                delayTime = delayTime < 0 ? 0 : delayTime;
                LeagueSharp.Common.Utility.DelayAction.Add((int)delayTime, () =>
                {
                    R.Cast(target);
                });
            }
            else
            {

                R.Cast(target);
            }
        }

        public static void AutoUltimate()
        {
            if (!R.IsReady())
            {
                return;
            }
            var enemies = ObjectManager.Player.GetEnemiesInRange(R.Range)
                .Where(h=>h.HasBuffOfType(BuffType.Knockup) || h.HasBuffOfType(BuffType.Knockback));
            if (!enemies.Any())
            {
                return;
            }
            var avgPoint = enemies.FirstOrDefault().ServerPosition;
            var bestAvg = avgPoint;
            int inRange = avgPoint.CountEnemiesInRange(ObjectManager.Player.AttackRange);
            int bestInRange = inRange;
            foreach (var enemy in enemies)
            {
                avgPoint += enemy.ServerPosition;
                avgPoint /= 2;
                inRange = avgPoint.CountEnemiesInRange(ObjectManager.Player.AttackRange);
                if (inRange > bestInRange)
                {
                    bestInRange = inRange;
                    bestAvg = avgPoint;
                }

            }
            if (Config.Param<Slider>("ylm.spellr.targetnumber").Value <= bestInRange)
            {
                R.Cast(bestAvg);
            }
        }
    }
}
