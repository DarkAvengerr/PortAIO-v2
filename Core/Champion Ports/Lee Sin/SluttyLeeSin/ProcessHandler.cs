using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Lee_Sin
{
    class ProcessHandler : LeeSin
    {
        public static void ProcessHandlers()
        {
            if (SelectedAllyAiMinion != null)
            {
                if (SelectedAllyAiMinion.IsDead)
                {
                    SelectedAllyAiMinion = null;
                }
            }
            if (Created)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => Created = false);
            }


            if (Processw && Environment.TickCount - Lastprocessw > 500)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => Processw = false);
            }

            if (Processroncast && Environment.TickCount - Processroncastr > 500)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(2500, () => Processroncast = false);
            }

            if (ProcessW2)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(2500, () => ProcessW2 = false);
            }

            if (Processr && Environment.TickCount - Lastprocessr > 100)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(400, () => Processr = false);
            }

            if (Processr2 && Environment.TickCount - Processr2T > 100)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(400, () => Processr = false);
            }
        }
    }
}
