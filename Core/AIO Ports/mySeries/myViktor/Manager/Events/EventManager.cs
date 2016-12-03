using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events
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
            AntiGapcloser.OnEnemyGapcloser += AntiGaplcoserManager.Init;
            Interrupter2.OnInterruptableTarget += InterruptManager.Init;
            Orbwalking.BeforeAttack += BeforeAttackManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}
