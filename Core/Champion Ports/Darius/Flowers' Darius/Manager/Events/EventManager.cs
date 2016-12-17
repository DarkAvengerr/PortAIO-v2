using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Events
{
    using LeagueSharp;

    internal class EventManager
    {
        internal static void Init()
        {
            Game.OnUpdate += LoopManager.Init;
            Obj_AI_Base.OnSpellCast += DoCastManager.Init;
            Obj_AI_Base.OnProcessSpellCast += SpellCastManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}
