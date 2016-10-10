using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class LaneClear
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



        static LaneClear()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Evelynn.UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Evelynn.QMiniManaLaneClear", "Minimum Mana To Use Q In LaneClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Evelynn.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Evelynn.UseELaneClear", "Use E in LaneClear").SetValue(false));
            SkyLv_Evelynn.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Evelynn.EMiniManaLaneClear", "Minimum Mana To Use E In LaneClear").SetValue(new Slider(70, 0, 100)));
            SkyLv_Evelynn.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Evelynn.UsePacketCastLaneClear", "Use PacketCast In LaneClear").SetValue(false));
            SkyLv_Evelynn.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Evelynn.SafeLaneClear", "Dont Use Spell In Lane Clear If Enemy in Dangerous Range").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            LaneClearLogic();
        }

        public static void LaneClearLogic()
        {
            var PacketCast = SkyLv_Evelynn.Menu.Item("Evelynn.UsePacketCastLaneClear").GetValue<bool>();

            var useQ = SkyLv_Evelynn.Menu.Item("Evelynn.UseQLaneClear").GetValue<bool>();
            var useE = SkyLv_Evelynn.Menu.Item("Evelynn.UseELaneClear").GetValue<bool>();

            var MiniManaQ = SkyLv_Evelynn.Menu.Item("Evelynn.QMiniManaLaneClear").GetValue<Slider>().Value;
            var MiniManaE = SkyLv_Evelynn.Menu.Item("Evelynn.EMiniManaLaneClear").GetValue<Slider>().Value;

            var Minion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy).FirstOrDefault();

            if (Minion.IsValidTarget() && SkyLv_Evelynn.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_Evelynn.Menu.Item("Evelynn.SafeLaneClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (useQ && Player.ManaPercent > MiniManaQ && Q.IsReady())
                {
                    if (Minion.IsValidTarget(Q.Range))
                    Q.Cast(PacketCast);
                }
                
                if (useE && Player.ManaPercent > MiniManaE && E.IsReady())
                {
                    if (Minion.IsValidTarget(E.Range))
                        E.Cast(Minion, PacketCast);
                }
            }
        }
    }
}
