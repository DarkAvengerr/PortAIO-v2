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

        private static Spell Flash => new Spell(ObjectManager.Player.GetSpellSlot("SummonerFlash"), 425f);

        public void OnLoad()
        {
        }

        public bool ShouldGetExecuted()
        {
            return MenuExtensions.GetItemValue<KeyBind>("dz191.vhr.misc.condemn.flashcondemn").Active
                   && Variables.spells[SpellSlot.E].IsReady() && Flash.Slot != SpellSlot.Unknown && Flash.IsReady();
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

            var flashPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, Flash.Range);

            var prediction = E.GetPrediction(target);

            if (!target.IsValidTarget())
            {
                return;
            }

            if (target.IsDashing() || !E.IsReady()) return;

            if (prediction.Hitchance >= HitChance.VeryHigh)
            {
                var endPosition = prediction.UnitPosition.Extend(flashPosition, -pushDistance);
                if (endPosition.IsWall())
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
                        var endPositionEx = prediction.UnitPosition.Extend(flashPosition, -i);
                        if (endPositionEx.IsWall())
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