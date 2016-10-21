using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Condemn;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;

namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    class AutoE : IModule
    {
        private static Spell E => Variables.spells[SpellSlot.E];

        public void OnLoad()
        {
            
        }

        public bool ShouldGetExecuted()
        {
            return MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.condemn.autoe") &&
                   Variables.spells[SpellSlot.E].IsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var pushDistance = MenuExtensions.GetItemValue<Slider>("dz191.vhr.misc.condemn.pushdistance").Value - 25;

            foreach (var target in HeroManager.Enemies.Where(en => en.IsValidTarget(E.Range)))
            {
                var Prediction = Variables.spells[SpellSlot.E].GetPrediction(target);

                if (Prediction.Hitchance >= HitChance.VeryHigh)
                {
                    var endPosition = Prediction.UnitPosition.Extend(ObjectManager.Player.ServerPosition, -pushDistance);
                    if (endPosition.IsWall())
                    {
                        E.CastOnUnit(target);
                    }
                    else
                    {
                        //It's not a wall.
                        var step = pushDistance / 5f;
                        for (float i = 0; i < pushDistance; i += step)
                        {
                            var endPositionEx = Prediction.UnitPosition.Extend(ObjectManager.Player.ServerPosition, -i);
                            if (endPositionEx.IsWall())
                            {
                                E.CastOnUnit(target);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
