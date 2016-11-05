using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshTherulerofthesoul
{
    class Turret
    {
        public static bool IsUnderEnemyTurret(AIHeroClient hero)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.Distance(hero.Position) < 950 && turret.IsEnemy);
        }

        public static bool IsUnderAllyTurret(AIHeroClient hero)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.Distance(hero.Position) < 950 && turret.IsAlly);
        }
    }
}
