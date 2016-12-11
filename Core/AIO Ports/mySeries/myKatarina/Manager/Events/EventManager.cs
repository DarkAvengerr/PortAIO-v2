using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events
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
            GameObject.OnCreate += CreateManager.Init;
            GameObject.OnDelete += DeleteManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }
    }
}
