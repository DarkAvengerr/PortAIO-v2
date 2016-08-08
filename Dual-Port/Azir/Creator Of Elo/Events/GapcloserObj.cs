using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Azir_Free_elo_Machine
{
    class GapcloserObj
    {
        private string Name;
        private SpellSlot spellSlot;

        public string getName
        {
            get
            {
                return Name;
            }
        }
        public SpellSlot getSpellSlot
        {
            get
            {
                return spellSlot;
            }
        }
        public string getChampName
        {
            get
            {
                return ChampName;
            }
        }
        private string ChampName;

        public GapcloserObj(string Name, SpellSlot spellSlot, string champName)
        {
            this.Name = Name;
            this.spellSlot = spellSlot;
            this.ChampName = champName;
        }
    }
}
