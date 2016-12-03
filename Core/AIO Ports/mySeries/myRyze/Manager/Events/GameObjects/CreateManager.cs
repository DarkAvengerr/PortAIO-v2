using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using myCommon;

    internal class CreateManager : Logic
    {
        internal static void Init(GameObject sender, EventArgs Args)
        {
            var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
            var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

            if (Rengar != null && Menu.GetBool("AntiRengar"))
            {
                if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(Me.Position) < W.Range)
                {
                    W.CastOnUnit(Rengar);
                }
            }

            if (Khazix != null && Menu.GetBool("AntiKhazix"))
            {
                if (sender.Name == "Khazix_Base_E_Tar.troy" && sender.Position.Distance(Me.Position) <= 300)
                {
                    W.CastOnUnit(Khazix);
                }
            }
        }
    }
}