using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hikiMarksmanRework.Core.Utilitys
{
    class Catcher
    {
        public static Vector3 WayPointEnemy(AIHeroClient enemy)
        {
            return enemy.Path.ToList()[enemy.Path.ToList().Count - 1];
        }
        public static float Calculate(AIHeroClient enemy)
        {
            var x1 = ObjectManager.Player.Distance(WayPointEnemy(enemy));
            var x2 = x1 / ObjectManager.Player.MoveSpeed;
            return x2;
        }
        public static float GapcloseCalculte(AIHeroClient enemy, Spell spell)
        {
            var x1 = ObjectManager.Player.Distance(WayPointEnemy(enemy)) - spell.Range;
            var x2 = x1 / ObjectManager.Player.MoveSpeed;
            return x2;
        }
       
    }
}
