using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.Helpers;
using VayneHunter_Reborn.Utility.MenuUtility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Skills.Tumble
{
    class TumbleLogic
    {
        private static float LastCondemnTick = 0f;

        private static Spell Q
        {
            get { return Variables.spells[SpellSlot.Q]; }
        }

        public static void OnLoad()
        {
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && (args.Target is Obj_AI_Base))
            {
                if (Environment.TickCount - Variables.LastCondemnFlashTime < 250)
                {
                    return;
                }

                ExecuteAALogic(sender, (Obj_AI_Base) args.Target);
            }

            if (sender.IsMe && args.Slot == SpellSlot.E)
            {
                LastCondemnTick = Environment.TickCount;
            }
        }

        private static void ExecuteAALogic(Obj_AI_Base sender, Obj_AI_Base target)
        {
            var QEnabled = Q.IsEnabledAndReady(Variables.Orbwalker.ActiveMode);
            
            if (QEnabled)
            {
                switch (Variables.Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        TumbleMethods.PreCastTumble(target);
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        if (target is AIHeroClient)
                        {
                            if (MenuExtensions.GetItemValue<bool>("dz191.vhr.mixed.q.2wstacks") &&
                                !(target as AIHeroClient).Has2WStacks())
                            {
                                return;
                            }

                            TumbleMethods.PreCastTumble(target);
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                    case Orbwalking.OrbwalkingMode.LastHit:
                        TumbleMethods.HandleFarmTumble(target);
                        break;
                }
            }

            if (MenuExtensions.GetItemValue<bool>("dz191.vhr.mixed.ethird"))
            {
                if (target is AIHeroClient)
                {
                    var tg = target as AIHeroClient;
                    if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && tg.GetWBuff() != null
                        && tg.GetWBuff().Count == 1 && tg.IsValidTarget(Variables.spells[SpellSlot.E].Range))
                    {
                        Variables.spells[SpellSlot.E].CastOnUnit(tg);
                    }
                }
            }

            foreach (var module in Variables.moduleList.Where(module => module.GetModuleType() == ModuleType.OnAfterAA
                && module.ShouldGetExecuted()))
            {
                module.OnExecute();
            }
        }

        
    }
}
