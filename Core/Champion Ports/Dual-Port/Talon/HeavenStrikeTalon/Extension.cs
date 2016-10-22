using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeTalon
{
    using static Program;
    public static class Extension
    {
        public static bool HasItem()
        {
            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady()
                || Items.CanUseItem(3748))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CastItem()
        {

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            if (Items.CanUseItem(3748))
                Items.UseItem(3748);
        }
        public static bool OutOfAA(Obj_AI_Base target)
        {
            return Player.Position.To2D().Distance(target.Position.To2D()) > Player.BoundingRadius + Player.AttackRange + target.BoundingRadius;
        }
        public static bool OutOfAA(Vector2 position, Obj_AI_Base target)
        {
            return Player.Position.To2D().Distance(position) > Player.BoundingRadius + Player.AttackRange + target.BoundingRadius;
        }
        public static bool R1IsReady()
        {
            return R.IsReady() && !Player.HasBuff("talonshadowassaultbuff") ;
        }
        public static bool R2IsReady()
        {
            return R.IsReady() && Player.HasBuff("talonshadowassaultbuff") ;
        }

    }
}
