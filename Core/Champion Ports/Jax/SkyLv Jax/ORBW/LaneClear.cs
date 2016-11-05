using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
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
                return SkyLv_Jax.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Jax.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Jax.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Jax.E;
            }
        }
        #endregion



        static LaneClear()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("LaneClear").SubMenu("Q Settings LaneClear").AddItem(new MenuItem("Jax.UseQLaneClear", "Use Q in LaneClear").SetValue(false));
            SkyLv_Jax.Menu.SubMenu("LaneClear").SubMenu("Q Settings LaneClear").AddItem(new MenuItem("Jax.QMiniManaLaneClear", "Minimum Mana Percent To Use Q In LaneClear").SetValue(new Slider(70, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("LaneClear").SubMenu("W Settings LaneClear").AddItem(new MenuItem("Jax.UseWLaneClear", "Use W in LaneClear").SetValue(false));
            SkyLv_Jax.Menu.SubMenu("LaneClear").SubMenu("W Settings LaneClear").AddItem(new MenuItem("Jax.WMiniManaLaneClear", "Minimum Mana Percent To Use W In LaneClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("LaneClear").SubMenu("E Settings LaneClear").AddItem(new MenuItem("Jax.UseELaneClear", "Use E in LaneClear").SetValue(false));
            SkyLv_Jax.Menu.SubMenu("LaneClear").SubMenu("E Settings LaneClear").AddItem(new MenuItem("Jax.EMiniManaLaneClear", "Minimum Mana Percent To Use E In LaneClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("LaneClear").SubMenu("E Settings LaneClear").AddItem(new MenuItem("Jax.EMiniHitLaneClear", "Minimum Minion Hit To Use E In LaneClear").SetValue(new Slider(3, 1, 6)));
            SkyLv_Jax.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Jax.SafeLaneClear", "Dont Use Spell In Lane Clear If Enemy in Dangerous Range").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            LaneClearLogic();
        }

        public static void LaneClearLogic()
        {
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();

            var useQ = SkyLv_Jax.Menu.Item("Jax.UseQLaneClear").GetValue<bool>();
            var useW = SkyLv_Jax.Menu.Item("Jax.UseWLaneClear").GetValue<bool>();
            var useE = SkyLv_Jax.Menu.Item("Jax.UseELaneClear").GetValue<bool>();

            var MiniManaQ = SkyLv_Jax.Menu.Item("Jax.QMiniManaLaneClear").GetValue<Slider>().Value;
            var MiniManaW = SkyLv_Jax.Menu.Item("Jax.WMiniManaLaneClear").GetValue<Slider>().Value;
            var MiniManaE = SkyLv_Jax.Menu.Item("Jax.EMiniManaLaneClear").GetValue<Slider>().Value;

            var EMiniHitLaneClear = SkyLv_Jax.Menu.Item("Jax.EMiniHitLaneClear").GetValue<Slider>().Value;

            var Minion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy).FirstOrDefault();

            if (Minion.IsValidTarget() && SkyLv_Jax.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_Jax.Menu.Item("Jax.SafeLaneClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (useQ && Player.ManaPercent > MiniManaQ && Q.IsReady())
                {
                    if (Minion.IsValidTarget(Q.Range) && Q.GetDamage(Minion) > Minion.Health)
                    Q.Cast(Minion, PacketCast);
                }
                
                if (useE && Player.ManaPercent > MiniManaE && E.IsReady() && CustomLib.EnemyMinionInPlayerRange(E.Range) >= EMiniHitLaneClear && !CustomLib.iSJaxEActive())
                {
                    if (Minion.IsValidTarget(E.Range))
                        E.Cast(PacketCast);
                }

                if (useE && CustomLib.EnemyMinionInPlayerRange(E.Range) >= EMiniHitLaneClear && CustomLib.iSJaxEActive())
                {
                    if (Minion.IsValidTarget(E.Range))
                        E.Cast(PacketCast);
                }

                if (useW && W.IsReady() && Player.Mana >= W.ManaCost)
                {
                    if (Minion.IsValidTarget() && Orbwalking.CanAttack())
                        W.Cast(PacketCast);
                }
            }
        }
    }
}
