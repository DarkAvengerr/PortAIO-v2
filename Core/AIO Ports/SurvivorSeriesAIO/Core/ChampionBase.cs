// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChampionBase.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using LeagueSharp;
using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public abstract class ChampionBase : IChampion
    {
        protected ChampionBase(IRootMenu menu, Orbwalking.Orbwalker Orbwalker)
        {
            Menu = menu;
            this.Orbwalker = Orbwalker;

            Q = new Spell(SpellSlot.Q, true);
            W = new Spell(SpellSlot.W, true);
            E = new Spell(SpellSlot.E, true);
            R = new Spell(SpellSlot.R, true);
        }

        protected Spell E { get; }

        protected IRootMenu Menu { get; }

        protected Orbwalking.Orbwalker Orbwalker { get; }

        protected AIHeroClient Player { get; } = ObjectManager.Player;

        protected Spell Q { get; }

        protected Spell R { get; }

        protected Spell W { get; }
    }
}