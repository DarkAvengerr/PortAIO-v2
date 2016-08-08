using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
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

        static AntiGapCLoser()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Misc").AddItem(new MenuItem("AurelionSol.AutoQEGC", "Auto Q On Gapclosers").SetValue(true));

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.LSDistance(Player.ServerPosition) < Q.Range && SkyLv_AurelionSol.Menu.Item("AurelionSol.AutoQEGC").GetValue<bool>())
            {
                var prediction = Q.GetPrediction(gapcloser.Sender);
                if (prediction.Hitchance >= HitChance.High)
                {
                    Q.Cast(prediction.CastPosition);
                }
            }
        }
    }
}
