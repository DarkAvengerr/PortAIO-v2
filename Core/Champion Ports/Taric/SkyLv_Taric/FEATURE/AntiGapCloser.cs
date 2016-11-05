namespace SkyLv_Taric
{
    using EloBuddy;
    using LeagueSharp;
    using LeagueSharp.Common;


    internal class AntiGapCLoser
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
        }
        #endregion

        static AntiGapCLoser()
        {
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var MinimumHpEGapCloser = SkyLv_Taric.Menu.Item("Taric.MinimumHpEGapCloser").GetValue<Slider>().Value;
            var MinimumEnemyEGapCloser = SkyLv_Taric.Menu.Item("Taric.MinimumEnemyEGapCloser").GetValue<Slider>().Value;
            var UseAutoEGapCloser = SkyLv_Taric.Menu.Item("Taric.UseAutoEGapCloser").GetValue<bool>();
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();

            if (Player.IsRecalling()) return;

            if (UseAutoEGapCloser && gapcloser.End.Distance(Player.ServerPosition) < E.Range && Player.HealthPercent <= MinimumHpEGapCloser && CustomLib.enemyChampionInPlayerRange(800) >= MinimumEnemyEGapCloser)
            {
                E.Cast(gapcloser.End, PacketCast);
            }
        }
    }
}
