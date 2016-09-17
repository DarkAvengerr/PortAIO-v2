using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
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
                return SkyLv_Tristana.Player;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Tristana.R;
            }
        }
        #endregion

        static Interrupter()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Misc").AddItem(new MenuItem("Tristana.AutoIR", "Auto R On Interruptable").SetValue(true));

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (SkyLv_Tristana.Menu.Item("Tristana.AutoIR").GetValue<bool>() && R.IsReady() && sender.IsValidTarget(R.Range))
                R.CastOnUnit(sender);
        }
    }
}
