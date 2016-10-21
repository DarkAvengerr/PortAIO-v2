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
 namespace Lee_Sin.InsecPos
{
    class FlashInsecPosition : LeeSin
    {
        public static IEnumerable<AIHeroClient> GetAllyHeroes(AIHeroClient unit, int range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead && hero.Distance(unit) < range).OrderBy(x => x.Distance(Player))
                    .ToList();
        }
        public static Vector3 InsecPos(AIHeroClient target, int extendvalue)
        {

            //  var pos = Player.Position.Extend(target.Position, +target.Position.Distance(Player.Position) + 230);
            if (SelectedAllyAiMinion != null)
            {
                return
                    SelectedAllyAiMinion.Position.Extend(target.Position,
                        +target.Position.Distance(SelectedAllyAiMinion.Position) + extendvalue);

            }
            var objAiHero = GetAllyHeroes(target, 1200).FirstOrDefault();
            if (GetBool("useobjectsallies", typeof(bool)) && objAiHero != null)
            {
                return
                    objAiHero.Position.Extend(target.Position,
                        +target.Position.Distance(objAiHero.Position) + extendvalue);
            }

            if (!GetBool("useobjectsallies", typeof(bool)) || objAiHero == null)
            {
                return Player.Position.Extend(target.Position,
                    +target.Position.Distance(Player.Position) + extendvalue);
            }
            return new Vector3();
        }

    }
}
