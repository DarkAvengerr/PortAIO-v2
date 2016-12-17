using EloBuddy; 
using LeagueSharp.Common; 
namespace ElLeeSin.Components.SpellManagers
{
    using System;
    using System.Collections.Generic;

    using ElLeeSin.Utilities;

    using LeagueSharp;

    internal class PassiveManager
    {
        internal static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            try
            {
                if (!sender.IsMe)
                {
                    return;
                }

                if (args.Buff.DisplayName.Equals("BlindMonkFlurry", StringComparison.InvariantCultureIgnoreCase))
                {
                   LeeSin.PassiveStacks = 2;
                }

                if (args.Buff.DisplayName.Equals("BlindMonkQTwoDash", StringComparison.InvariantCultureIgnoreCase))
                {
                   LeeSin.isInQ2 = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("@PassiveManager.cs: Can not {0} -", e);
                throw;
            }
        }

        internal static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            try
            {
                if (!sender.IsMe)
                {
                    return;
                }

                if (args.Buff.DisplayName.Equals("BlindMonkFlurry", StringComparison.InvariantCultureIgnoreCase))
                {
                   LeeSin.PassiveStacks = 0;
                }

                if (args.Buff.DisplayName.Equals("BlindMonkQTwoDash", StringComparison.InvariantCultureIgnoreCase))
                {
                   LeeSin.isInQ2 = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("@PassiveManager.cs: Can not {0} -", e);
                throw;
            }
        }
    }
}
