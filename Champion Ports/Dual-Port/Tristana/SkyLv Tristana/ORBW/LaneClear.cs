using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
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
                return SkyLv_Tristana.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Tristana.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Tristana.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Tristana.E;
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



        static LaneClear()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.QMiniManaLaneClear", "Minimum Mana To Use Q In LaneClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.QLaneClearCount", "Minimum Minion To Use Q In LaneClear").SetValue(new Slider(8, 1, 10)));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.UseELaneClear", "Use E in LaneClear").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.EMiniManaLaneClear", "Minimum Mana To Use E In LaneClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.ELaneClearCount", "Minimum Minion To Use E In LaneClear").SetValue(new Slider(3, 1, 6)));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.UseELaneClearOnlyLastHitable", "Use E in LaneClear Only On LastHitable Minion").SetValue(false));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.UsePacketCastLaneClear", "Use PacketCast In LaneClear").SetValue(false));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.SafeLaneClear", "Dont Use Spell In Lane Clear If Enemy in Dangerous Range").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            LaneClearLogic();
        }

        public static void LaneClearLogic()
        {
            var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastLaneClear").GetValue<bool>();

            var useQ = SkyLv_Tristana.Menu.Item("Tristana.UseQLaneClear").GetValue<bool>();
            var useE = SkyLv_Tristana.Menu.Item("Tristana.UseELaneClear").GetValue<bool>();

            var MiniManaQ = SkyLv_Tristana.Menu.Item("Tristana.QMiniManaLaneClear").GetValue<Slider>().Value;
            var MiniManaE = SkyLv_Tristana.Menu.Item("Tristana.EMiniManaLaneClear").GetValue<Slider>().Value;

            var MiniCountQ = SkyLv_Tristana.Menu.Item("Tristana.QLaneClearCount").GetValue<Slider>().Value;
            var MiniCountE = SkyLv_Tristana.Menu.Item("Tristana.ELaneClearCount").GetValue<Slider>().Value;

            var Minion = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy).FirstOrDefault();

            if (Minion.IsValidTarget() && SkyLv_Tristana.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && !SkyLv_Tristana.Menu.Item("Tristana.AfterAttackModeLaneClear").GetValue<bool>())
            {
                if (SkyLv_Tristana.Menu.Item("Tristana.SafeLaneClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (useE && Player.ManaPercent > MiniManaE && E.IsReady())
                {
                    foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.Team != ObjectManager.Player.Team && !m.IsDead && Player.Distance(m) <= E.Range))
                    {
                        if (CustomLib.EnemyMinionInMinionRange(minion, 300) >= MiniCountE)
                        {
                            if (Player.GetAutoAttackDamage(minion) * 2 > minion.Health && SkyLv_Tristana.Menu.Item("Tristana.UseELaneClearOnlyLastHitable").GetValue<bool>())
                                E.CastOnUnit(minion, PacketCast);
                            if (!SkyLv_Tristana.Menu.Item("Tristana.UseELaneClearOnlyLastHitable").GetValue<bool>())
                                E.CastOnUnit(minion, PacketCast);
                        }
                    }
                }

                if (useQ && Player.ManaPercent > MiniManaQ && Q.IsReady())
                {
                    if (CustomLib.EnemyMinionInPlayerRange(Orbwalking.GetRealAutoAttackRange(Player)) >= MiniCountQ)
                    {
                        Q.Cast(PacketCast);
                    }
                }
            }
        }
    }
}
