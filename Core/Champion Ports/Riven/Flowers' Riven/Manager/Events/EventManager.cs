using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class EventManager
    {
        internal static void Init()
        {
            Game.OnUpdate += LoopManager.Init;
            Obj_AI_Base.OnSpellCast += DoCastManager.InitCombo;
            Obj_AI_Base.OnSpellCast += DoCastManager.InitBurst;
            Obj_AI_Base.OnSpellCast += DoCastManager.InitMixed;
            Obj_AI_Base.OnSpellCast += DoCastManager.InitClear;
            Obj_AI_Base.OnSpellCast += DoCastManager.InitJungle;
            Obj_AI_Base.OnProcessSpellCast += SpellCastManager.InitSpellCast;
            Obj_AI_Base.OnProcessSpellCast += SpellCastManager.InitSpellShield;
            Obj_AI_Base.OnPlayAnimation += AnimationManager.Init;
            AntiGapcloser.OnEnemyGapcloser += GapcloserManager.Init;
            Interrupter2.OnInterruptableTarget += InterruptManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}
