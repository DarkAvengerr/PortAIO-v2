
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir_Free_elo_Machine.Math;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Azir_Creator_of_Elo
{
    class Spells
    {
       public Spell _q, _w, _e, _r;

        public Spell Q
        {
            get { return _q; }
        }

        public Spell W
        {
            get { return _w; }
        }

        public Spell E
        {
            get { return _e; }
        }

        public Spell R
        {
            get { return _r; }
        }

        public Spells()
        {
            _q = new Spell(SpellSlot.Q, 825);


            _w = new Spell(SpellSlot.W, 450);
            _e = new Spell(SpellSlot.E, 1250);
            _r = new Spell(SpellSlot.R, 450);

            _q.SetSkillshot(0, 70, 1600, false, SkillshotType.SkillshotCircle);
            _e.SetSkillshot(0, 100, 1700, false, SkillshotType.SkillshotLine);
            _r.SetSkillshot(0.5f, 0, 1400, false, SkillshotType.SkillshotLine);
            //  ignite = ObjectManager.Player.GetSpellSlot("SummonerDot");
        }



      

    }


     internal class StaticSpells
     {
         private static Points _pointer;
        public static void CastQ(AzirMain azir, AIHeroClient target, bool useQ)
        {
           var pointsAttack=new Points[120];
            var points = Azir_Free_elo_Machine.Math.Geometry.PointsAroundTheTarget(target.ServerPosition, 640, 80);
            var i = 0;
           
            foreach (var point in points)
            {
               
                    if (point.Distance(azir.Hero.ServerPosition) <= azir.Spells.Q.Range)
                    {
                        _pointer.hits = Azir_Free_elo_Machine.Math.Geometry.Nattacks(azir, point, target);
                        _pointer.point = point;
                        pointsAttack[i] = _pointer;


                    }
                    i++;
        

            }
            if (pointsAttack.MaxOrDefault(x => x.hits).hits > 0)
            {
                azir.Spells.Q.Cast(pointsAttack.MaxOrDefault(x => x.hits).point);
            }
        }
  
     
    }

}
  