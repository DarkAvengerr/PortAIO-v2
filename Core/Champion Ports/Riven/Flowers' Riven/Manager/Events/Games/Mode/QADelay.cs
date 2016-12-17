using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class QADelay :  Logic
    {
        internal static void Init()
        {
            if (Menu.GetBool("AutoSetDelay"))
            {
                var delay = 0;

                if (Game.Ping <= 20)
                {
                    delay = Game.Ping;
                }
                else if (Game.Ping <= 50)
                {
                    delay = Game.Ping / 2 + 5;
                }
                else
                {
                    delay = Game.Ping / 2 - 5;
                }

                if (delay >= 70)
                {
                    delay = 70;
                }

                Menu.Item("Q1Delay", true).SetValue(new Slider(280 + delay, 200, 350));
                Menu.Item("Q2Delay", true).SetValue(new Slider(280 + delay, 200, 350));
                Menu.Item("Q3Delay", true).SetValue(new Slider(380 + delay, 300, 450));
            }
        }
    }
}
