using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events
{
    using Games;
    using Drawings;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class EventManager
    {
        internal static void Init()
        {
            Game.OnUpdate += LoopManager.Init;
            Obj_AI_Base.OnProcessSpellCast += SpellCastManager.Init;
            Interrupter2.OnInterruptableTarget += InterruptManager.Init;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}
