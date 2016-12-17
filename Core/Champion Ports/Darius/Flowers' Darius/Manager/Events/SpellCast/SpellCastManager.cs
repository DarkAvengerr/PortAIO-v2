using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Events
{
    using System.Linq;
    using FlowersDariusCommon;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class SpellCastManager : Logic
    {
        internal static void Init(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsMe || Args.SData == null)
            {
                return;
            }

            if (Args.SData.Name.Contains("DariusAxeGrabCone"))
            {
                lastETime = Utils.TickCount;
            }

            if (Args.SData.Name.Contains("ItemTiamatCleave"))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (!HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= Orbwalking.GetRealAutoAttackRange(Me)))
                    {
                        return;
                    }

                    if (Menu.GetBool("ComboW") && W.IsReady())
                    {
                        W.Cast();
                    }
                }
            }
        }
    }
}