using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class JungleClear
    {

        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Evelynn.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Evelynn.Q;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Evelynn.E;
            }
        }
        #endregion



        static JungleClear()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Evelynn.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Evelynn.QMiniManaJungleClear", "Minimum Mana To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Evelynn.UseEJungleClear", "Use E In JungleClear").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Evelynn.EMiniManaJungleClear", "Minimum Mana To Use E In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Evelynn.UsePacketCastJungleClear", "Use PacketCast In JungleClear").SetValue(false));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Evelynn.SafeJungleClear", "Dont Use Spell In Jungle Clear If Enemy in Dangerous Range").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleClearLogic();
        }

        public static void JungleClearLogic()
        {
            var PacketCast = SkyLv_Evelynn.Menu.Item("Evelynn.UsePacketCastJungleClear").GetValue<bool>();
            var useQ = SkyLv_Evelynn.Menu.Item("Evelynn.UseQJungleClear").GetValue<bool>();
            var useE = SkyLv_Evelynn.Menu.Item("Evelynn.UseEJungleClear").GetValue<bool>();

            var MiniManaQ = SkyLv_Evelynn.Menu.Item("Evelynn.QMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaE = SkyLv_Evelynn.Menu.Item("Evelynn.EMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionN = MinionManager.GetMinions(Q.Range + 300, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (MinionN.IsValidTarget() && SkyLv_Evelynn.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_Evelynn.Menu.Item("Evelynn.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (useQ && Q.IsReady() && Player.ManaPercent > MiniManaQ)
                {
                    Q.Cast(PacketCast);
                }

                if (useE && E.IsReady() && Player.ManaPercent > MiniManaE)
                {
                    E.Cast(MinionN, PacketCast);
                }
            }
        }
    }
}
