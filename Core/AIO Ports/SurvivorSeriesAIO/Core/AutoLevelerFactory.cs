using System;
using SurvivorSeriesAIO.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public static class AutoLevelerFactory
    {
        public static IAutoLeveler Create(string name, IRootMenu menu)
        {
            switch (name)
            {
                case "Ryze":
                    return new AutoLeveler(menu);

                case "Malzahar":
                    return new AutoLeveler(menu);

                case "Ashe":
                    return new AutoLeveler(menu);

                case "Brand":
                    return new AutoLeveler(menu);

                /*case "Irelia":
                    return new AutoLeveler(menu);*/

                default:
                    //return new Utility.AutoLeveler(menu);
                    throw new NotSupportedException();
            }
        }
    }
}