using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Evade
{
    using System.Collections.Generic;
    using LeagueSharp;

    internal class EvadeSpellDatabase
    {
        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        static EvadeSpellDatabase()
        {
            if (ObjectManager.Player.ChampionName == "Yasuo")
            {
                Spells.Add(new EvadeSpellData
                {
                    Name = "YasuoWMovingWall",
                    Slot = SpellSlot.W,
                    Range = 400,
                    Delay = 250,
                    Speed = int.MaxValue,
                    _dangerLevel = 3
                });

                Spells.Add(new EvadeSpellData
                {
                    Name = "YasuoDashWrapper",
                    Slot = SpellSlot.E,
                    Range = 475,
                    Delay = 100,
                    Speed = 1200,
                    _dangerLevel = 2
                });
            }
        }
    }
}
