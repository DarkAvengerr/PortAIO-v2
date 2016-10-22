using System;
using Activator = SurvivorSeriesAIO.Utility.Activator;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public static class ActivatorFactory
    {
        public static IActivator Create(string name, IRootMenu menu)
        {
            switch (name)
            {
                case "Ryze":
                    return new SurvivorSeriesAIO.Utility.Activator(menu);

                case "Malzahar":
                    return new SurvivorSeriesAIO.Utility.Activator(menu);

                case "Ashe":
                    return new SurvivorSeriesAIO.Utility.Activator(menu);

                case "Brand":
                    return new SurvivorSeriesAIO.Utility.Activator(menu);

                case "Irelia":
                    return new SurvivorSeriesAIO.Utility.Activator(menu);

                default:
                    //return new Utility.Activator(menu);
                    throw new NotSupportedException();
            }
        }
    }
}