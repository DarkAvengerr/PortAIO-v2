using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.External.Activator.ActivatorSpells
{
    interface ISRSpell
    {
        void OnLoad();

        void BuildMenu(Menu RootMenu);
        
        bool ShouldRun();

        void Run();
    }
}
