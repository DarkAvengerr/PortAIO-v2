using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class AntiGapCLoser
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

        static AntiGapCLoser()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Misc").AddItem(new MenuItem("Tristana.AutoREGC", "Auto R On Gapclosers").SetValue(false));

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.Distance(Player.ServerPosition) < 300 && SkyLv_Tristana.Menu.Item("Tristana.AutoREGC").GetValue<bool>())
            {
                R.CastOnUnit(gapcloser.Sender);
            }
        }
    }
}
