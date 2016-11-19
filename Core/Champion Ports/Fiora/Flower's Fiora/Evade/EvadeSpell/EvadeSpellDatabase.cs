using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Fiora.Evade
{
    using System.Collections.Generic;
    using LeagueSharp;

    internal class EvadeSpellDatabase
    {
        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        static EvadeSpellDatabase()
        {
            if (ObjectManager.Player.ChampionName == "Fiora")
            {
                Spells.Add(new EvadeSpellData
                {
                    Name = "FioraQ",
                    Slot = SpellSlot.Q,
                    Range = 340,
                    Delay = 50,
                    Speed = 1100,
                    _dangerLevel = 3
                });

                Spells.Add(new EvadeSpellData
                {
                    Name = "FioraW",
                    Slot = SpellSlot.W,
                    Range = 750,
                    Delay = 100,
                    Speed = int.MaxValue,
                    _dangerLevel = 3
                });
            }
        }
    }
}
