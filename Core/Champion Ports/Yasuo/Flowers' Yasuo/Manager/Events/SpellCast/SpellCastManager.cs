using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellCastManager : Logic
    {
        public static void Init(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender != null && sender.IsMe && Args.SData != null && Args.SData.Name == "YasuoDashWrapper" && 
                Args.Target != null && !Args.Target.IsAlly)
            {
                var target = (Obj_AI_Base)Args.Target;

                if (target.IsValidTarget())
                {
                    lastEPos = PosAfterE(target);
                }
            }
        }
    }
}