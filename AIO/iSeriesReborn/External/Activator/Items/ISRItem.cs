using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.External.Activator.Items
{
    interface ISRItem
    {
        void OnLoad();

        void BuildMenu(Menu RootMenu);

        ISRItemType GetItemType();

        bool ShouldRun();

        void Run();
    }
}
