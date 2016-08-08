using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;

using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Tumble.VHRQ;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Modules.ModuleList.Condemn
{
    using EloBuddy;
    using Utility = LeagueSharp.Common.Utility;

    class FlashCondemn : IModule
    {
        private static Spell E => Variables.spells[SpellSlot.E];

        private static Spell Flash => new Spell(ObjectManager.Player.LSGetSpellSlot("SummonerFlash"), 425f);

        public void OnLoad()
        {
        }

        public bool ShouldGetExecuted()
        {
            return MenuExtensions.GetItemValue<KeyBind>("dz191.vhr.misc.condemn.flashcondemn").Active
                   && Variables.spells[SpellSlot.E].LSIsReady() && Flash.Slot != SpellSlot.Unknown && Flash.LSIsReady();
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate; // idk why thiis wwas on after attack m8 pls
        }

        public void OnExecute()
        {
            var pushDistance = 450;

            var target = TargetSelector.SelectedTarget != null
                             ? TargetSelector.GetSelectedTarget()
                             : TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            var flashPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, Flash.Range);

            var prediction = E.GetPrediction(target);

            if (!target.LSIsValidTarget())
            {
                return;
            }

            if (target.LSIsDashing() || !E.LSIsReady()) return;

            if (prediction.Hitchance >= HitChance.VeryHigh)
            {
                var endPosition = prediction.UnitPosition.LSExtend(flashPosition, -pushDistance);
                if (endPosition.LSIsWall())
                {
                    Variables.LastCondemnFlashTime = Environment.TickCount;
                    E.CastOnUnit(target);
                    Utility.DelayAction.Add((int)(E.Delay + Game.Ping / 2f), () => Flash.Cast(flashPosition));
                }
                else
                {
                    // It's not a wall.
                    var step = pushDistance / 5f;
                    for (float i = 0; i < pushDistance; i += step)
                    {
                        var endPositionEx = prediction.UnitPosition.LSExtend(flashPosition, -i);
                        if (endPositionEx.LSIsWall())
                        {
                            Variables.LastCondemnFlashTime = Environment.TickCount;
                            E.CastOnUnit(target);
                            Utility.DelayAction.Add((int)(E.Delay + Game.Ping / 2f), () => Flash.Cast(flashPosition));

                            // Flash.Cast(flashPosition);
                            return;
                        }
                    }
                }
            }
        }
    }
}