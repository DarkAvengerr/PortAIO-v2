using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Hikigaya_Lux.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hikigaya_Lux.Logic
{
    class QLogic
    {
        public static readonly AIHeroClient Lux = ObjectManager.Player;
        public static void HikiQx2Target(AIHeroClient enemy)
        {
            if (Spells.Q.GetPrediction(enemy).CollisionObjects.Count == 2)
            {
                if (Spells.Q.GetPrediction(enemy).CollisionObjects[0].IsChampion() && Spells.Q.GetPrediction(enemy).CollisionObjects[1].IsMinion)
                {
                    Spells.Q.Cast(Spells.Q.GetPrediction(enemy).CastPosition);
                }
                if (Spells.Q.GetPrediction(enemy).CollisionObjects[0].IsMinion && Spells.Q.GetPrediction(enemy).CollisionObjects[1].IsChampion())
                {
                    Spells.Q.Cast(Spells.Q.GetPrediction(enemy).CastPosition);
                }
            }
        }
        public static void NormalQ(AIHeroClient enemy)
        {
            if (Spells.Q.GetPrediction(enemy).Hitchance >= Helper.HikiChance("q.hit.chance"))
            {
                Spells.Q.Cast(enemy);
            }
        }
        public static void QGeneral(AIHeroClient enemy)
        {
            switch (Helper.Slider("min.q.hit"))
            {
                case 1:
                    NormalQ(enemy);
                    break;
                case 2:
                    HikiQx2Target(enemy);
                    break;
            }
        }
        
    }
}
