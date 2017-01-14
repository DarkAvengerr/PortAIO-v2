using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Misc
{
    class CondemnFlash
    {
        
	    private static readonly SpellSlot FlashSlot = ObjectManager.Player.GetSpellSlot("SummonerFlash");
        public static void CondemnFlashs()
        {
            if (CondemnCheck(ObjectManager.Player.ServerPosition) != null)
            {
                return;
            }

            var positions = GetRotatedFlashPositions();

            foreach (var p in positions)
            {
                var condemnUnit = CondemnCheck(p);
                if (condemnUnit != null)
                {
                    Program.E.CastOnUnit(condemnUnit);
                    {
                        ObjectManager.Player.Spellbook.CastSpell(FlashSlot, p);
                    };
                }
            }
        }
        private static AIHeroClient CondemnCheck(Vector3 fromPosition)
        {
            var HeroList = HeroManager.Enemies.Where(
                                    h =>
                                        h.IsValidTarget(Program.E.Range) &&
                                        !h.HasBuffOfType(BuffType.SpellShield) &&
                                        !h.HasBuffOfType(BuffType.SpellImmunity));
            foreach (var Hero in HeroList)
            {
                var ePred = Program.E.GetPrediction(Hero);
                int pushDist = Program.emenu.Item("PushDistance").GetValue<Slider>().Value;
                for (int i = 0; i < pushDist; i += (int)Hero.BoundingRadius)
                {
                    Vector3 loc3 = ePred.UnitPosition.To2D().Extend(fromPosition.To2D(), -i).To3D();
                    if (loc3.IsWall())
                    {
                        return Hero;
                    }
                }
            }
            return null;
        }
        public static List<Vector3> GetRotatedFlashPositions()
        {
            const int currentStep = 30;
            var direction = ObjectManager.Player.Direction.To2D().Perpendicular();

            var list = new List<Vector3>();
            for (var i = -90; i <= 90; i += currentStep)
            {
                var angleRad = Geometry.DegreeToRadian(i);
                var rotatedPosition = ObjectManager.Player.Position.To2D() + (400f * direction.Rotated(angleRad));
                list.Add(rotatedPosition.To3D());
            }
            return list;
        }
        private static void LoadFlash()
        {
            var testSlot = ObjectManager.Player.GetSpellSlot("summonerflash");
            if (testSlot != SpellSlot.Unknown)
            {
                Console.WriteLine("Flash Slot: {0}", testSlot);
                var FlashSlot = testSlot;
            }
            else
            {
                Console.WriteLine("Error loading Flash! Not found!");
            }
        }
    }
}
