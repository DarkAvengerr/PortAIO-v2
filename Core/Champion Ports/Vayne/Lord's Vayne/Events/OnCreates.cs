using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Events
{
    class OnCreates
    {
        public static void OnCreate(GameObject sender, EventArgs Args)
        {
            var Rengar = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Rengar"));
            var Khazix = HeroManager.Enemies.Find(heros => heros.ChampionName.Equals("Khazix"));

            if (Rengar != null && Program.imenu.Item("AntiRengar", true).GetValue<bool>())
            {
                if (sender.Name == "Rengar_LeapSound.troy" && sender.Position.Distance(ObjectManager.Player.Position) < Program.E.Range)
                {
                    Program.E.CastOnUnit(Rengar);
                }
            }

            if (Khazix != null && Program.imenu.Item("AntiKhazix", true).GetValue<bool>())
            {
                if (sender.Name == "Khazix_Base_E_Tar.troy" && sender.Position.Distance(ObjectManager.Player.Position) <= 300)
                {
                    Program.E.CastOnUnit(Khazix);
                }
            }
        }
    }
}
