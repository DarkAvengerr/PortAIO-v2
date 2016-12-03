using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events
{
    using myCommon;
    using LeagueSharp.Common;
    

    internal class BeforeAttackManager : Logic
    {
        internal static void Init(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Menu.GetBool("DisableAA"))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Menu.GetBool("DisableAAALL"))
                    {
                        Args.Process = Me.HasBuff("ViktorPowerTransferReturn") || Me.Mana < Q.Instance.SData.Mana;
                    }
                    else if (Me.Level >= Menu.GetSlider("DisableAALevel"))
                    {
                        Args.Process = Me.HasBuff("ViktorPowerTransferReturn") || Me.Mana < Q.Instance.SData.Mana;
                    }
                    else
                    {
                        Args.Process = true;
                    }
                }
            }
            else
            {
                Args.Process = true;
            }
        }
    }
}