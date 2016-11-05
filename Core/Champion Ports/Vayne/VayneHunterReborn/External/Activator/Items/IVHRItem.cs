using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.External.Activator.Items
{
    interface IVHRItem
    {
        void OnLoad();

        void BuildMenu(Menu RootMenu);

        IVHRItemType GetItemType();

        bool ShouldRun();

        void Run();

        int GetItemId();

        float GetItemRange();

        LeagueSharp.Common.Items.Item GetItemObject();
    }
}
