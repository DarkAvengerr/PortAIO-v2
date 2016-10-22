// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivatorBase.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public abstract class AutoLevelerBase : IAutoLeveler
    {
        protected AutoLevelerBase(IRootMenu menu)
        {
            Menu = menu;
        }

        protected IRootMenu Menu { get; }

        protected AIHeroClient Player { get; } = ObjectManager.Player;
    }
}