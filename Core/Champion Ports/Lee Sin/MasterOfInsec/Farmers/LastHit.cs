using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using EloBuddy; 
using LeagueSharp.Common; 
namespace MasterOfInsec.Farmers
{
    static class LastHit
    {
        public static void Do()
        {
            Obj_AI_Base minion = ObjectManager.Get<Obj_AI_Base>().Where(x => (x.IsEnemy && x.IsMinion) && Program.Q.GetDamage(x) >= x.Health && Program.Q.IsInRange(x) && Program.Q.CanCast(x)).MinOrDefault(x=> x==x);
            var useQ = Program.menu.Item("QLas").GetValue<bool>();
            if (minion == null) return;
            if(useQ)
            {
                if (minion.Distance(Program.Player) > Program.Player.AttackRange)
              {
               //     if(Program.Q.IsKillable(minion))
                        Program.Q.CastOnUnit(minion);

              }
            }
        }
    }
}
