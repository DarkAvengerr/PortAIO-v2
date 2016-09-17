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
    class RLogic
    {
        public static readonly AIHeroClient Lux = ObjectManager.Player;

        public static void NormalR(AIHeroClient enemy)
        {
            if (Spells.R.GetPrediction(enemy).Hitchance >= Helper.HikiChance("r.hit.chance.x") && Calculators.R(enemy) > enemy.Health)
            {
                Spells.R.Cast(enemy);
            }
        }
        public static void HikiRxTarget(AIHeroClient enemy)
        {
            if (Spells.R.GetPrediction(enemy).Hitchance >= Helper.HikiChance("r.hit.chance.x"))
            {
                Spells.R.CastIfWillHit(enemy, Helper.Slider("min.r.hit.x"));
            }
        }
        public static void FaceCheckUlt(AIHeroClient enemy)
        {
            if (Spells.R.GetPrediction(enemy).Hitchance >= Helper.HikiChance("r.hit.chance.x") && Calculators.R(enemy) > enemy.Health
                && ObjectManager.Player.IsFacing(enemy))
            {
                Spells.R.Cast(enemy);
            }
        }
        public static void RGeneral(AIHeroClient enemy)
        {
            switch (LuxMenu.Config.Item("r.style.x").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    NormalR(enemy);
                    break;
                case 1:
                    HikiRxTarget(enemy);
                    break;
                case 2:
                    FaceCheckUlt(enemy);
                    break;
            }
        }
    }
}
