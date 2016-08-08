using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
{

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class Interrupter
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_AurelionSol.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_AurelionSol.Q;
            }
        }
        #endregion

        static Interrupter()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Misc").AddItem(new MenuItem("AurelionSol.AutoIQ", "Auto Q On Interruptable").SetValue(true));

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (SkyLv_AurelionSol.Menu.Item("AurelionSol.AutoIQ").GetValue<bool>() && Q.LSIsReady() && sender.LSIsValidTarget(Q.Range))
                Q.Cast(sender);
        }
    }
}
