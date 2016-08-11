using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir_Creator_of_Elo;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Azir_Free_elo_Machine
{
    class Events
    {
        private AzirMain azir;
        public Events(AzirMain azir)
        {
            this.azir = azir;
            Obj_AI_Base.OnSpellCast += Game_ProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnGapClose;

        }

        private void OnGapClose(ActiveGapcloser gapcloser)
        {
            if (azir.Menu.GetMenu.Item("UseRGapcloser").GetValue<bool>())
            {
                if (gapcloser.End.Distance(azir.Hero.ServerPosition) < azir.Spells.R.Range)
                {
                    for (int i = 0; i < azir.GapcloserNum; i++)
                    {
                        if (azir.Menu.GetMenu.Item("G" + i).GetValue<bool>())
                        {
                            if (
                                azir.gapcloserList.FindGapCloserBy(gapcloser.Sender.ChampionName, gapcloser.Slot)
                                    .getName == azir.Gapcloser[i])
                            {
                                azir.Spells.R.Cast(gapcloser.Sender);
                            }


                        }

                    }
                }
            }
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (azir.Menu.GetMenu.Item("UseRInterrupt").GetValue<bool>())
                for (int i = 0; i < azir.InterruptNum; i++)
                {
                    if (azir.Menu.GetMenu.Item("S" + i).GetValue<bool>())
                    {

                        if (args.SData.Name == azir.InterruptSpell[i])
                        {
                            if (azir.Hero.Distance(sender) <= 550)
                            {
                                azir.Spells.R.Cast(sender);
                            }
                        }
                    }
                }

          
        }
    }
}
