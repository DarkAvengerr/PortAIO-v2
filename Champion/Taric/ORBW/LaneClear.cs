namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class LaneClear
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Taric.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Taric.W;
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



        static LaneClear()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("Q Settings LaneClear").AddItem(new MenuItem("Taric.UseQLaneClear", "Use Q in LaneClear").SetValue(false));
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("Q Settings LaneClear").AddItem(new MenuItem("Taric.QMiniManaLaneClear", "Minimum Mana Percent To Use Q In LaneClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("Q Settings LaneClear").AddItem(new MenuItem("Taric.QMiniMinimionAroundLaneClear", "Minimum Minion Around To Use Q In LaneClear").SetValue(new Slider(6, 1, 10)));
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("W Settings LaneClear").AddItem(new MenuItem("Taric.UseWLaneClear", "Use W in LaneClear").SetValue(false));
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("W Settings LaneClear").AddItem(new MenuItem("Taric.WMiniManaLaneClear", "Minimum Mana Percent To Use W In LaneClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("W Settings LaneClear").AddItem(new MenuItem("Taric.WMiniMinimionAroundLaneClear", "Minimum Minion Around To Use W In LaneClear").SetValue(new Slider(6, 1, 10)));
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("E Settings LaneClear").AddItem(new MenuItem("Taric.UseELaneClear", "Use E in LaneClear").SetValue(false));
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("E Settings LaneClear").AddItem(new MenuItem("Taric.EMiniManaLaneClear", "Minimum Mana Percent To Use E In LaneClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("LaneClear").SubMenu("E Settings LaneClear").AddItem(new MenuItem("Taric.EMiniHitLaneClear", "Minimum Minion Hit To Use E In LaneClear").SetValue(new Slider(3, 1, 6)));
            SkyLv_Taric.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Taric.SafeLaneClear", "Dont Use Spell In Lane Clear If Enemy in Dangerous Range").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Taric.UseTaricAAPassiveLaneClear", "Use All Taric AA Passive In LaneClear").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            LaneClearLogic();
        }

        public static void LaneClearLogic()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();

            var UseQLaneClear = SkyLv_Taric.Menu.Item("Taric.UseQLaneClear").GetValue<bool>();
            var UseWLaneClear = SkyLv_Taric.Menu.Item("Taric.UseWLaneClear").GetValue<bool>();
            var UseELaneClear = SkyLv_Taric.Menu.Item("Taric.UseELaneClear").GetValue<bool>();

            var QMiniManaLaneClear = SkyLv_Taric.Menu.Item("Taric.QMiniManaLaneClear").GetValue<Slider>().Value;
            var WMiniManaLaneClear = SkyLv_Taric.Menu.Item("Taric.WMiniManaLaneClear").GetValue<Slider>().Value;
            var EMiniManaLaneClear = SkyLv_Taric.Menu.Item("Taric.EMiniManaLaneClear").GetValue<Slider>().Value;

            var QMiniMinimionAroundLaneClear = SkyLv_Taric.Menu.Item("Taric.QMiniMinimionAroundLaneClear").GetValue<Slider>().Value;
            var WMiniMinimionAroundLaneClear = SkyLv_Taric.Menu.Item("Taric.WMiniMinimionAroundLaneClear").GetValue<Slider>().Value;
            var EMiniHitLaneClear = SkyLv_Taric.Menu.Item("Taric.EMiniHitLaneClear").GetValue<Slider>().Value;

            var Minion = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy).FirstOrDefault();

            if (Minion.LSIsValidTarget() && SkyLv_Taric.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_Taric.Menu.Item("Taric.SafeLaneClear").GetValue<bool>() && Player.LSCountEnemiesInRange(1500) > 0) return;

                if (UseELaneClear && Player.ManaPercent > EMiniManaLaneClear && E.LSIsReady() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveLaneClear").GetValue<bool>()))
                {
                    var allMinionsE = MinionManager.GetMinions(Player.Position, E.Range, MinionTypes.All, MinionTeam.Enemy);

                    if (allMinionsE.Any())
                    {
                        var farmAll = Q.GetLineFarmLocation(allMinionsE, 150f);
                        if (farmAll.MinionsHit >= EMiniHitLaneClear)
                        {
                            E.Cast(farmAll.Position, PacketCast);
                            return;
                        }
                    }
                }

                if (UseWLaneClear && CustomLib.EnemyMinionInPlayerRange(E.Range) >= WMiniMinimionAroundLaneClear && Player.ManaPercent > WMiniManaLaneClear && W.LSIsReady() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveLaneClear").GetValue<bool>()) && (!E.LSIsReady() || !UseELaneClear))
                {
                    W.Cast(Player, PacketCast);
                    return;
                }

                if (UseQLaneClear && CustomLib.EnemyMinionInPlayerRange(E.Range) >= QMiniMinimionAroundLaneClear && Player.ManaPercent > QMiniManaLaneClear && Q.LSIsReady() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveLaneClear").GetValue<bool>()) && (!E.LSIsReady() || !UseELaneClear) && (Player.HealthPercent < 100 || (!W.LSIsReady() || !UseWLaneClear)))
                {
                    Q.Cast(Player, PacketCast);
                    return;
                }
            }
        }
    }
}
