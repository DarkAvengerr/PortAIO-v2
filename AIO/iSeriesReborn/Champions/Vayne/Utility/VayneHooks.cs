using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DZLib.Logging;
using iSeriesReborn.Champions.Vayne.Skills;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.Positioning;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Vayne.Utility
{
    class VayneHooks
    {
        internal static void OnGapCloser(LeagueSharp.Common.ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.LSIsValidTarget() &&
                MenuExtensions.GetItemValue<bool>("iseriesr.vayne.misc.general.antigp") &&
                Variables.spells[SpellSlot.E].LSIsReady())
            {
                Variables.spells[SpellSlot.E].Cast(gapcloser.Sender);
            }
        }

        internal static void OnInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= Interrupter2.DangerLevel.Medium 
                && MenuExtensions.GetItemValue<bool>("iseriesr.vayne.misc.general.antigp") 
                && Variables.spells[SpellSlot.E].LSIsReady() 
                && sender.LSIsValidTarget(Variables.spells[SpellSlot.E].Range))
            {
                Variables.spells[SpellSlot.E].Cast(sender);
            }
        }

        internal static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (MenuExtensions.GetItemValue<KeyBind>("iseriesr.vayne.misc.noaastealthex").Active && VayneUtility.IsStealthed())
            {
                var owTarget = Variables.Orbwalker.GetTarget();
                if (owTarget is AIHeroClient)
                {
                    var owHero = owTarget as AIHeroClient;
                    if (owHero.Health < ObjectManager.Player.LSGetAutoAttackDamage(owHero) * 2)
                    {
                        return;
                    }

                    if (PositioningVariables.EnemiesClose.Count() == 1)
                    {
                        return;
                    }
                    args.Process = false;
                }
            }
        }

        internal static void OnNonKillableMinion(AttackableUnit minion)
        {
            if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear &&
                Variables.spells[SpellSlot.Q].IsEnabledAndReady() && (minion is Obj_AI_Base))
            {
                var mBase = minion as Obj_AI_Base;
                if (HealthPrediction.GetHealthPrediction(mBase, 250 + (int)ObjectManager.Player.AttackCastDelay + (int)(Game.Ping / 2f)) > 0
                    && HealthPrediction.GetHealthPrediction(mBase, 250 + (int)ObjectManager.Player.AttackCastDelay + 75 + (int)(Game.Ping / 2f)) + 5 <
                    Variables.spells[SpellSlot.Q].GetDamage(mBase) + ObjectManager.Player.LSGetAutoAttackDamage(mBase))
                {
                    var qEndPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 325f);

                    if (!VayneQ.IsSafe(qEndPosition))
                    {
                        return;
                    }

                    LeagueSharp.Common.Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);
                    Variables.spells[SpellSlot.Q].Cast(qEndPosition);
                }
            }
        }
    }
}
