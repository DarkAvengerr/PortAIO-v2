using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
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

        private static Spell W1
        {
            get
            {
                return SkyLv_AurelionSol.W1;
            }
        }

        private static Spell W2
        {
            get
            {
                return SkyLv_AurelionSol.W2;
            }
        }
        #endregion



        static JungleClear()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").AddItem(new MenuItem("AurelionSol.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").AddItem(new MenuItem("AurelionSol.QMiniManaJungleClear", "Minimum Mana To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").AddItem(new MenuItem("AurelionSol.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").AddItem(new MenuItem("AurelionSol.WMiniManaJungleClear", "Minimum Mana To Use W In JungleClear").SetValue(new Slider(40, 0, 100)));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").AddItem(new MenuItem("AurelionSol.UsePacketCastJungleClear", "Use PacketCast In JungleClear").SetValue(false));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").AddItem(new MenuItem("AurelionSol.SafeJungleClear", "Dont Use Spell In Jungle Clear If Enemy in Dangerous Range").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleClearLogic();
        }

        public static void JungleClearLogic()
        {
            var PacketCast = SkyLv_AurelionSol.Menu.Item("AurelionSol.UsePacketCastJungleClear").GetValue<bool>();
            var useQ = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseQJungleClear").GetValue<bool>();
            var useW = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseWJungleClear").GetValue<bool>();

            var MiniManaQ = SkyLv_AurelionSol.Menu.Item("AurelionSol.QMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaW = SkyLv_AurelionSol.Menu.Item("AurelionSol.WMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionN = MinionManager.GetMinions(Q.Range + 200, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (MinionN.LSIsValidTarget() && SkyLv_AurelionSol.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_AurelionSol.Menu.Item("AurelionSol.SafeJungleClear").GetValue<bool>() && Player.LSCountEnemiesInRange(1500) > 0) return;

                if (useQ && Player.ManaPercent > MiniManaQ && Q.LSIsReady())
                {
                    Q.CastIfHitchanceEquals(MinionN, HitChance.VeryHigh, PacketCast);
                }

                if (useW && W1.LSIsReady() && Player.ManaPercent > MiniManaW)
                {
                    if (Player.ManaPercent <= MiniManaW && CustomLib.isWInLongRangeMode())
                    {
                        W2.Cast(PacketCast);
                    }

                    if (Player.LSDistance(MinionN) > W1.Range - 20 && Player.LSDistance(MinionN) < W1.Range + 20 && CustomLib.isWInLongRangeMode())
                    {
                        W2.Cast(PacketCast);
                    }

                    if (Player.LSDistance(MinionN) > W2.Range - 20 && Player.LSDistance(MinionN) < W2.Range + 20 && !CustomLib.isWInLongRangeMode() && Player.ManaPercent > MiniManaW)
                    {
                        W1.Cast(PacketCast);
                    }
                }
            }
        }
    }
}
