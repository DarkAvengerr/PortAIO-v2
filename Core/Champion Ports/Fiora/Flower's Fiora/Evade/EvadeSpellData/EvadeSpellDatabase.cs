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
                EvadeSpellData spell1 = new InvulnerabilityData("Fiora Q", SpellSlot.Q, 400, 2);
                Spells.Add(spell1);
            }

            if (ObjectManager.Player.ChampionName == "Fiora")
            {
                EvadeSpellData spell = new InvulnerabilityData("Fiora W", SpellSlot.W, 250, 3);
                Spells.Add(spell);
            }
        }

        public static EvadeSpellData GetByName(string Name)
        {
            Name = Name.ToLower();
            foreach (var evadeSpellData in Spells)
            {
                if (evadeSpellData.Name.ToLower() == Name)
                {
                    return evadeSpellData;
                }
            }

            return null;
        }
    }
}
