using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using KoreanCommon;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAkali
{
    class Zhonyas
    {
        //private CommonChampion Akali;

        public Zhonyas(CommonChampion akali)
        {
            //Akali = akali;
            //Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //if (!KoreanUtils.GetParamBool(Akali.MainMenu, "usezhonyas") ||
            //        !ItemData.Zhonyas_Hourglass.GetItem().IsReady())
            //{
            //    return;
            //}

            //if (!args.Target.IsMe)
            //{
            //    return;
            //}

            //if ((args.SData.IsAutoAttack() && sender.GetAutoAttackDamage(Akali.Player) + 100 >= Akali.Player.Health) ||
            //    (sender.GetSpellDamage(Akali.Player, args.SData.Name) + 100 >= Akali.Player.Health))
            //{
            //    ItemData.Zhonyas_Hourglass.GetItem().Cast();
            //}
        }
    }
}
