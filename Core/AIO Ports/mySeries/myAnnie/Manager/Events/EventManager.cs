using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events
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
            Orbwalking.BeforeAttack += BeforeAttackManager.Init;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserManager.Init;
            Interrupter2.OnInterruptableTarget += InterruptManager.Init;
            Obj_AI_Base.OnProcessSpellCast += SpellCastManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}
