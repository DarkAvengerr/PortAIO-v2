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
 namespace HeavenStrikeShyvana
{
    public static class extension
    {
        public static Vector2? GetFirstWallPoint(this Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }

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
            // titanic hydra
            if (Items.CanUseItem(3748))
                Items.UseItem(3748);
        }

        public static int GetSmiteDamage()
        {
            return new int[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 }
                [ObjectManager.Player.Level - 1];
        }

        public static bool HasSmiteRed
            => (new string[] { "s5_summonersmiteduel" }).Contains(ObjectManager.Player.GetSpell(Program.Smite).Name);
        public static bool HasSmiteBlue
            => (new string[] { "s5_summonersmiteplayerganker" }).Contains(ObjectManager.Player.GetSpell(Program.Smite).Name);
        public static int GetSmiteDamage(AIHeroClient target)
        {
            return HasSmiteBlue ? 20 + 8 * ObjectManager.Player.Level :
                   0;
        }
    }
}
