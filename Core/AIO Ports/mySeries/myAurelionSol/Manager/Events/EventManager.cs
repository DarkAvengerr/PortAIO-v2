using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events
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
            Interrupter2.OnInterruptableTarget += InterruptManager.Init;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserManager.Init;
            GameObject.OnCreate += CreateManager.Init;
            GameObject.OnDelete += DeleteManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}
