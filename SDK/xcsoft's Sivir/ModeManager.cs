using System;
using System.Collections.Generic;

using xcSivir.Modes;

using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using NLog;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcSivir
{
    internal static class ModeManager
    {
        internal static readonly List<ModeBase> Modes;

        static ModeManager()
        {
            Modes = new List<ModeBase>
            {
                new PermaActive(),
                new Combo(),
                new Harass(),
                new LaneClear(),
                new JungleClear(),
                new Flee()
            };

            new TickOperation(0x42, () =>
            {
                if (GameObjects.Player.IsDead)
                {
                    return;
                }

                Modes.ForEach(mode =>
                {
                    try
                    {
                        if (mode.ShouldBeExecuted())
                        {
                            mode.Execute();
                        }
                    }
                    catch (Exception e)
                    {
                        LogManager.GetCurrentClassLogger().Error($"Error executing mode '{mode.GetType().Name}'\n{e}");
                    }
                });
            }).Start();
        }

        internal static void Initialize() { }
    }
}
