using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Core.Condemn_Logic
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    interface ICondemnType
    {
        Vector3 Execute(Obj_AI_Base target, float range, Spell spell);
    }
}
