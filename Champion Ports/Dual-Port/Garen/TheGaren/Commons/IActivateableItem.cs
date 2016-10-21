using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheKalista.Commons.Items
{
    public interface IActivateableItem
    {
        string GetDisplayName();
        void Initialize(Menu menu, ItemManager itemManager);
        void Update(AIHeroClient target);
        void Use(Obj_AI_Base target);
        int GetRange();
        TargetSelector.DamageType GetDamageType();
    }
}
