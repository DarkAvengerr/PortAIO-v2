using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch.Commons.Items
{
    public interface IActivateableItem
    {
        void Initialize(Menu menu, ItemManager itemManager);
        string GetDisplayName();
        void Update(AIHeroClient target);
        void Use(Obj_AI_Base target);
        int GetRange();
        TargetSelector.DamageType GetDamageType();
    }
}
