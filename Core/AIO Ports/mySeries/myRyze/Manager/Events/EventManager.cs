using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events
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
            Spellbook.OnCastSpell += CastSpellManager.Init;
            Orbwalking.BeforeAttack += BeforeAttackManager.Init;
            Interrupter2.OnInterruptableTarget += InterruptManager.Init;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserManager.Init;
            GameObject.OnCreate += CreateManager.Init;
            Drawing.OnDraw += DrawManager.Init;
            Drawing.OnEndScene += DrawManager.InitMinMap;
        }
    }
}
