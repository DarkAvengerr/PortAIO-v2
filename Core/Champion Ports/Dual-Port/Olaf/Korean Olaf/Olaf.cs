using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
{
    using System;

    using KoreanOlaf.Common;

    using LeagueSharp.Common;

    class Olaf
    {
        private readonly OlafSpells olafSpells;

        private readonly OlafMenu olafMenu;

        private readonly Orbwalking.Orbwalker olafOrbwalker;

        private readonly OlafPotions olafPotions;

        private readonly OlafCore olafCore;

        private readonly OlafLegacysAxeFinder olafLegacysAxeFinder;

        private readonly OlafDrawingSpells olafDrawingSpells;

        private readonly CommonForceUltimate commonForceUltimate;

        public Olaf()
        {
            olafSpells = new OlafSpells();
            olafMenu = new OlafMenu(olafSpells, out olafOrbwalker);
            olafPotions = new OlafPotions(olafMenu);
            olafCore = new OlafCore(olafSpells, olafOrbwalker, olafMenu);
            olafLegacysAxeFinder = new OlafLegacysAxeFinder(olafMenu);
            olafDrawingSpells = new OlafDrawingSpells(olafMenu, olafSpells);
            commonForceUltimate = new CommonForceUltimate(olafMenu, olafSpells, olafOrbwalker);
            commonForceUltimate.ForceUltimate = olafCore.ForceUltimate;
        }
    }
}
