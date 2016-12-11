using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events
{
    using LeagueSharp;

    internal class SpellCastManager : Logic
    {
        public static void Init(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (Args.SData.Name == "YasuoDashWrapper" && Args.Target != null && !Args.Target.IsAlly)
                {
                    var target = (Obj_AI_Base)Args.Target;

                    lastEPos = PosAfterE(target);
                }
            }
        }
    }
}