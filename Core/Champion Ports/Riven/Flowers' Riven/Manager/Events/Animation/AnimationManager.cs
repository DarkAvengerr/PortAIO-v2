using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events
{
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class AnimationManager : Logic
    {
        internal static void Init(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee)
            {
                return;
            }

            if (Args.Animation.Contains("Spell1a"))
            {
                lastQTime = Utils.TickCount;
                qStack = 1;
                SpellManager.ResetQA(Menu.Item("Q1Delay", true).GetValue<Slider>().Value);
            }
            else if (Args.Animation.Contains("Spell1b"))
            {
                lastQTime = Utils.TickCount;
                qStack = 2;
                SpellManager.ResetQA(Menu.Item("Q2Delay", true).GetValue<Slider>().Value);
            }
            else if (Args.Animation.Contains("Spell1c"))
            {
                lastQTime = Utils.TickCount;
                qStack = 0;
                SpellManager.ResetQA(Menu.Item("Q3Delay", true).GetValue<Slider>().Value);
            }
        }
    }
}