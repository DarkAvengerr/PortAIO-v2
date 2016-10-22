using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Helpers.Modules;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Interface
{
    interface IChampion
    {
        void OnLoad(Menu menu);

        void RegisterEvents();

        Dictionary<SpellSlot, Spell> GetSpells();

        List<IModule> GetModules();

        void OnTick();

        void OnCombo();

        void OnMixed();

        void OnLastHit();

        void OnLaneclear();
    }
}
