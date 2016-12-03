using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events
{
    using LeagueSharp.Common;
    using myCommon;

    internal class AntiGapcloserManager : Logic
    {
        internal static void Init(ActiveGapcloser Args)
        {
            if (W.IsReady())
            {
                if (Menu.GetBool("AntiAlistar") && Args.Sender.ChampionName == "Alistar" && 
                    Args.SkillType == GapcloserType.Targeted)
                {
                    W.CastOnUnit(Args.Sender, true);
                }

                if (Menu.GetBool("Gapcloser") && Menu.GetBool("AntiGapcloser" + Args.Sender.ChampionName.ToLower()))
                {
                    if (Args.Sender.DistanceToPlayer() <= 200 && Args.Sender.IsValid)
                    {
                        W.CastOnUnit(Args.Sender, true);
                    }
                }
            }
        }
    }
}