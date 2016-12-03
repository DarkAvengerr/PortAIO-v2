using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events
{
    using myCommon;
    using LeagueSharp.Common;
    

    internal class BeforeAttackManager : Logic
    {
        internal static void Init(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                switch (Menu.GetList("ComboDisableAA"))
                {
                    case 0:
                        if (W.IsReady() || E.IsReady())
                        {
                            Args.Process = false;
                        }
                        break;
                    case 1:
                        Args.Process = false;
                        break;
                    default:
                        Args.Process = true;
                        break;
                }
            }
        }
    }
}