using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
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



        static LaneClear()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("LaneClear").AddItem(new MenuItem("AurelionSol.UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("LaneClear").AddItem(new MenuItem("AurelionSol.QMiniManaLaneClear", "Minimum Mana To Use Q In LaneClear").SetValue(new Slider(70, 0, 100)));
            SkyLv_AurelionSol.Menu.SubMenu("LaneClear").AddItem(new MenuItem("AurelionSol.QLaneClearCount", "Minimum Minion To Use Q In LaneClear").SetValue(new Slider(3, 1, 6)));
            SkyLv_AurelionSol.Menu.SubMenu("LaneClear").AddItem(new MenuItem("AurelionSol.UseWLaneClear", "Use W in LaneClear").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("LaneClear").AddItem(new MenuItem("AurelionSol.WMiniManaLaneClear", "Minimum Mana To Use W In LaneClear").SetValue(new Slider(70, 0, 100)));
            SkyLv_AurelionSol.Menu.SubMenu("LaneClear").AddItem(new MenuItem("AurelionSol.WLaneClearCount", "Minimum Minion To Use W In LaneClear").SetValue(new Slider(4, 1, 6)));
            SkyLv_AurelionSol.Menu.SubMenu("LaneClear").AddItem(new MenuItem("AurelionSol.UsePacketCastLaneClear", "Use PacketCast In LaneClear").SetValue(false));
            SkyLv_AurelionSol.Menu.SubMenu("LaneClear").AddItem(new MenuItem("AurelionSol.SafeLaneClear", "Dont Use Spell In Lane Clear If Enemy in Dangerous Range").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            LaneClearLogic();
        }

        public static void LaneClearLogic()
        {
            var PacketCast = SkyLv_AurelionSol.Menu.Item("AurelionSol.UsePacketCastLaneClear").GetValue<bool>();

            var useQ = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseQLaneClear").GetValue<bool>();
            var useW = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseWLaneClear").GetValue<bool>();

            var MiniManaQ = SkyLv_AurelionSol.Menu.Item("AurelionSol.QMiniManaLaneClear").GetValue<Slider>().Value;
            var MiniManaW = SkyLv_AurelionSol.Menu.Item("AurelionSol.WMiniManaLaneClear").GetValue<Slider>().Value;

            var MiniCountQ = SkyLv_AurelionSol.Menu.Item("AurelionSol.QLaneClearCount").GetValue<Slider>().Value;
            var MiniCountW = SkyLv_AurelionSol.Menu.Item("AurelionSol.WLaneClearCount").GetValue<Slider>().Value;

            var Minion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy).FirstOrDefault();

            if (Minion.IsValidTarget() && SkyLv_AurelionSol.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_AurelionSol.Menu.Item("AurelionSol.SafeLaneClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (useQ && Player.ManaPercent > MiniManaQ && Q.IsReady())
                {
                    var allMinionsQ = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy);

                    if (allMinionsQ.Any())
                    {
                        var farmAll = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);
                        if (farmAll.MinionsHit >= MiniCountQ)
                        {
                            Q.Cast(farmAll.Position, true);
                        }
                    }
                }

                if (useW && W1.IsReady())
                {
                    var allMinionsW1 = MinionManager.GetMinions(Player.Position + W1.Range - 20, W1.Range + 20, MinionTypes.All, MinionTeam.Enemy);
                    var allMinionsW2 = MinionManager.GetMinions(Player.Position + W2.Range - 20, W2.Range + 20, MinionTypes.All, MinionTeam.Enemy);

                    if (Player.ManaPercent <= MiniManaW && CustomLib.isWInLongRangeMode())
                    {
                        W2.Cast(PacketCast);
                    }

                    if (allMinionsW1.Any() && CustomLib.isWInLongRangeMode())
                    {
                        var farmAll = W1.GetCircularFarmLocation(allMinionsW1);
                        if (farmAll.MinionsHit >= MiniCountW)
                        {
                            W2.Cast(PacketCast);
                        }
                    }

                    if (allMinionsW2.Any() && !CustomLib.isWInLongRangeMode() && Player.ManaPercent > MiniManaW)
                    {
                        var farmAll = W2.GetCircularFarmLocation(allMinionsW2);
                        if (farmAll.MinionsHit >= MiniCountW)
                        {
                            W1.Cast(PacketCast);
                        }
                    }
                }
            }
        }
    }
}
