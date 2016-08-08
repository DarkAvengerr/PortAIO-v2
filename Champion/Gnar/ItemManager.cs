using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

using Item = LeagueSharp.Common.Items.Item;
using ItemData = LeagueSharp.Common.Data.ItemData;
using EloBuddy;

namespace Gnar
{
    public class ItemManager
    {
        private static AIHeroClient player = ObjectManager.Player;

        // Offensive items
        public static readonly Item TIAMAT = ItemData.Tiamat_Melee_Only.GetItem();
        public static readonly Item HYDRA = ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        public static readonly Item CUTLASS = ItemData.Bilgewater_Cutlass.GetItem();
        public static readonly Item BOTRK = ItemData.Blade_of_the_Ruined_King.GetItem();

        public static readonly Item YOUMUU = ItemData.Youmuus_Ghostblade.GetItem();

        // Defensive items
        public static readonly Item RANDUIN = ItemData.Randuins_Omen.GetItem();
        public static readonly Item FACE_MOUNTAIN = ItemData.Face_of_the_Mountain.GetItem();

        #region Use item methods

        public static bool UseHydra(Obj_AI_Base target)
        {
            if (Config.BoolLinks["itemsHydra"].Value && HYDRA.IsReady() && target.LSIsValidTarget(HYDRA.Range))
            {
                return HYDRA.Cast();
            }
            else if (Config.BoolLinks["itemsTiamat"].Value && TIAMAT.IsReady() && target.LSIsValidTarget(TIAMAT.Range))
            {
                return TIAMAT.Cast();
            }
            return false;
        }

        public static bool UseBotrk(AIHeroClient target)
        {
            if (Config.BoolLinks["itemsBotrk"].Value && BOTRK.IsReady() && target.LSIsValidTarget(BOTRK.Range) &&
                player.Health + player.GetItemDamage(target, Damage.DamageItems.Botrk) < player.MaxHealth)
            {
                return BOTRK.Cast(target);
            }
            else if (Config.BoolLinks["itemsCutlass"].Value && CUTLASS.IsReady() && target.LSIsValidTarget(CUTLASS.Range))
            {
                return CUTLASS.Cast(target);
            }
            return false;
        }

        public static bool UseYoumuu(Obj_AI_Base target)
        {
            if (Config.BoolLinks["itemsYoumuu"].Value && YOUMUU.IsReady() && target.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(player) + 50))
            {
                return YOUMUU.Cast();
            }
            return false;
        }

        public static bool UseRanduin(AIHeroClient target)
        {
            if (Config.BoolLinks["itemsRanduin"].Value && RANDUIN.IsReady() && target.LSIsValidTarget(RANDUIN.Range))
            {
                return RANDUIN.Cast();
            }
            return false;
        }

        #endregion
    }
}
