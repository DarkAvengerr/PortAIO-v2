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
 namespace DZAhri
{
    static class Helpers
    {
        public static bool IsMenuEnabled(String menu)
        {
            return DZAhri.Menu.Item(menu).GetValue<bool>();
        }

        public static bool IsCharmed(this AIHeroClient target)
        {
            return target.HasBuff("AhriSeduce",true);
        }

        public static bool IsSafe(this Vector3 myVector)
        {
            var killableEnemy = myVector.GetEnemiesInRange(600f).Find(h => GetComboDamage(h) >= h.Health);
            var killableEnemyNumber = killableEnemy != null ? 1 : 0;
            var killableEnemyPlayer = ObjectManager.Player.GetEnemiesInRange(600f).Find(h => GetComboDamage(h) >= h.Health);
            var killableEnemyPlayerNumber = killableEnemyPlayer != null ? 1 : 0;

            if ((ObjectManager.Player.UnderTurret(true) && killableEnemyPlayerNumber == 0) || (myVector.UnderTurret(true) && killableEnemyNumber == 0))
            {
                return false;
            }
            if (myVector.CountEnemiesInRange(600f) == 1 || ObjectManager.Player.CountEnemiesInRange(600f) >= 1)
            {
                return true;
            }
            return myVector.CountEnemiesInRange(600f) - killableEnemyNumber - myVector.CountAlliesInRange(600f) + 1 >= 0;
        }

        public static float GetComboDamage(AIHeroClient enemy)
        {
            float totalDamage = 0;
            totalDamage += DZAhri._spells[SpellSlot.Q].IsReady() ? DZAhri._spells[SpellSlot.Q].GetDamage(enemy) : 0;
            totalDamage += DZAhri._spells[SpellSlot.W].IsReady() ? DZAhri._spells[SpellSlot.W].GetDamage(enemy) : 0;
            totalDamage += DZAhri._spells[SpellSlot.E].IsReady() ? DZAhri._spells[SpellSlot.E].GetDamage(enemy) : 0;
            totalDamage += (DZAhri._spells[SpellSlot.R].IsReady() || (RStacks() != 0)) ? DZAhri._spells[SpellSlot.R].GetDamage(enemy) : 0;
            return totalDamage;
        }
        public static bool IsRCasted()
        {
            return ObjectManager.Player.HasBuff("AhriTumble", true);
        }
        public static int RStacks()
        {
            var rBuff = ObjectManager.Player.Buffs.Find(buff => buff.Name == "AhriTumble");
            return rBuff != null ? rBuff.Count : 0;
        }
    }

}
