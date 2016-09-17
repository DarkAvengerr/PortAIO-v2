using System;
using iDZed.Activator.Spells;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iDzed.Activator.Spells
{
    interface ISummonerSpell
    {
        void OnLoad();
        string GetDisplayName();
        void AddToMenu(Menu menu);
        bool RunCondition();
        void Execute();
        SummonerSpell GetSummonerSpell();
        string GetName();
    }
}
