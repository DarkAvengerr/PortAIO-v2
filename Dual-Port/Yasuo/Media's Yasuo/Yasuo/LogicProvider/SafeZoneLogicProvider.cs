//using EloBuddy; 
 //using LeagueSharp.Common; 
 //namespace YasuoMedia.Modules.Protector
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    using LeagueSharp;
//    using LeagueSharp.Common;
//    using LeagueSharp.SDK;

//    using SharpDX;

//    using Yasuo.Common;
//    using Yasuo.Common.Extensions;

//    using Geometry = LeagueSharp.Common.Geometry;

//    class SafeZoneLogicProvider
//    {
//        public Dictionary<Skillshot, Obj_AI_Base> possibleCollisions;

//        public SafeZoneLogicProvider(List<Skillshot> skillshots, List<Obj_AI_Base> units)
//        {
//            if (units != null && skillshots != null)
//            {
//                foreach (var unit in units)
//                {
//                    foreach (var skillshot in skillshots)
//                    {
//                        var eta = (int)unit.Distance(skillshot.MisslePosition() / skillshot.SData.MissileSpeed);
//                        if (skillshot.IsAboutToHit(unit, eta + Game.Ping))
//                        {
//                            this.possibleCollisions.Add(skillshot, unit);
//                        }
//                    }
//                }
//            }

//            if (this.possibleCollisions == null)
//            {
//                return;
//            }

//            foreach (var collision in this.possibleCollisions)
//            {
//                var SZ = new SafeZone(
//                    collision.Key.Start,
//                    collision.Key.SpellData.Range,
//                    collision.Key.SpellData.Radius);

//                if (SZ.IsInside(collision.Value))
//                {
//                    Variables.Spells[SpellSlot.W].Cast(SZ.Start);
//                }
//            }
//        }


//        public static Obj_AI_Base GetUnit(List<Obj_AI_Base> units)
//        {

//            return units.Where(x => x is AIHeroClient).MaxOrDefault(x => TargetSelector.GetPriority((AIHeroClient)x));
//        }
//    }
//}

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.LogicProvider
{
}