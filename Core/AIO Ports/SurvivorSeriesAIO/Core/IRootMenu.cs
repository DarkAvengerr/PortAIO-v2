// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRootMenu.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public interface IRootMenu
    {
        Menu Orbwalking { get; }

        Menu Root { get; }

        MenuItem SelectedHitChance { get; }

        MenuItem SelectedPrediction { get; }

        MenuItem PlugAutoLeveler { get; set; }

        MenuItem PlugActivator { get; set; }

        Menu TargetSelector { get; }

        Menu Activator { get; }

        Menu AutoLeveler { get; }

        Menu Champion { get; }
    }
}