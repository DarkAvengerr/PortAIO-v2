using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using YasuoTheLastMemebender.Skills;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoTheLastMemebender
{
    static class YasuoUtils
    {

        public static bool DecideKnockup(Obj_AI_Base target)
        {
            if (Config.Param<StringList>("ylm.combo.mode").SelectedIndex == 0 || !target.IsValid)
            {
                return false;
            }
            return target.Distance(ObjectManager.Player, true) <= (475+200)*(475+200) &&  SteelTempest.Q.IsReady() && SteelTempest.Empowered
                   && SweepingBlade.E.IsReady() && !target.HasBuff("YasuoDashWrapper");
        }

        public static bool DangerousTurret(Obj_AI_Turret turret)
        {
            if (!Config.Param<bool>("ylm.towerdive.enabled"))
                return true;
            var units = ObjectManager.Get<Obj_AI_Base>().Where(o => (o.IsAlly && (o.IsChampion() || o.IsMinion)) && turret.Distance(o, true) <= 900*900);
            return units.Count() >= Config.Param<Slider>("ylm.towerdive.minAllies").Value;
        }

        public static Obj_AI_Turret GetNearestTurret(Vector3 pos)
        {
            var turrets = ObjectManager.Get<Obj_AI_Turret>().OrderBy(t => t.Distance(pos, true));
            return turrets.FirstOrDefault();
        }

        public static Obj_AI_Base BestQDashKnockupUnit()
        {
            //var heroes = ObjectManager.Get<AIHeroClient>();
            var eRangeheroes = ObjectManager.Player.GetEnemiesInRange(SweepingBlade.E.Range)
                .Where(h=> SweepingBlade.CanCastE(h, true));
            int max = 0;
            AIHeroClient bestUnit = null;
            foreach (var eHero in eRangeheroes)
            {
                var enemyCount = SweepingBlade.EndPos(eHero).CountEnemiesInRange(SteelTempest.QDash.Width);
                if (enemyCount > max)
                {
                    max = enemyCount;
                    bestUnit = eHero;
                }
            }
            return bestUnit;
        }

        public static Obj_AI_Base ClosestMinion(Vector3 position)
        {
            var minion = MinionManager.GetMinions(Config.Param<Slider>("ylm.gapclose.limit").Value, MinionTypes.All,
                MinionTeam.NotAlly, MinionOrderTypes.None).OrderBy(m => m.Distance(position, true))
                .FirstOrDefault(m => position.Distance(ObjectManager.Player.ServerPosition, true) > m.Distance(position, true))
               ;
            //Console.WriteLine(minions.FirstOrDefault(m => position.Distance(ObjectManager.Player.ServerPosition, true) > m.Distance(position, true)).Name);
                /*
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(m => m.IsEnemy && m.IsValidTarget())
                    .OrderBy(m=>m.Distance(position, true)).FirstOrDefault();*/
            return minion;
        }
    }
}
