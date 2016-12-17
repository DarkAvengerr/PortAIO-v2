using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
using LeagueSharp.Common; 
namespace BadaoKingdom.BadaoChampion.BadaoGraves
{
    using static BadaoGravesVariables;
    public static class BadaoGravesHelper
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
        }
        public static bool CanAttack()

        {
            if (ObjectManager.Player.Spellbook.IsAutoAttacking)
                return false;
            var attackDelay = (1.0740296828d * 1000 * Player.AttackDelay - 725) * (1.10 + ExtraAADelay.GetValue<Slider>().Value/1000);
            if (Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= Orbwalking.LastAATick + attackDelay
                && Player.HasBuff("GravesBasicAttackAmmo1"))
            {
                return true;
            }
            return false;
        }
    }
}
