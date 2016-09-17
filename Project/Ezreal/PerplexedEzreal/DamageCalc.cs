using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PerplexedEzreal
{
    class DamageCalc
    {
        public static AIHeroClient Player = ObjectManager.Player;

        public static double GetQDamage(Obj_AI_Base target)
        {
            return Player.GetSpellDamage(target, SpellSlot.Q);
        }

        public static double GetWDamage(Obj_AI_Base target)
        {
            return Player.GetSpellDamage(target, SpellSlot.W);
        }

        public static double GetEDamage(Obj_AI_Base target)
        {
            return Player.GetSpellDamage(target, SpellSlot.E);
        }

        public static double GetRDamage(Obj_AI_Base target)
        {
            double dmg = Player.GetSpellDamage(target, SpellSlot.R);
            int collisions = SpellManager.R.GetCollision(Player.ServerPosition.To2D(),
                new List<Vector2>() { target.ServerPosition.To2D() }).Count;
            collisions = collisions > 7 ? 7 : collisions;
            float reduction = 1 - (collisions / 10);
            dmg = dmg * reduction;
            return dmg;
        }
    }
}
