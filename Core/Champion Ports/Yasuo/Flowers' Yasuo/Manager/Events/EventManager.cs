using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class EventManager : Logic
    {
        internal static void Init()
        {
            Game.OnUpdate += LoopManager.Init;
            Obj_AI_Base.OnProcessSpellCast += SpellCastManager.Init;
            Obj_AI_Base.OnPlayAnimation += AnimationManager.Init;
            Orbwalking.AfterAttack += AfterAttackManager.Init;
            Interrupter2.OnInterruptableTarget += InterruptManager.Init;
            AntiGapcloser.OnEnemyGapcloser += GapcloserManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}
