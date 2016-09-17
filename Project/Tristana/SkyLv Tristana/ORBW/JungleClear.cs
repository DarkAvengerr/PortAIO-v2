using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
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



        static JungleClear()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Tristana.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Tristana.QMiniManaJungleClear", "Minimum Mana To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Tristana.UseEJungleClear", "Use E In JungleClear").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Tristana.EMiniManaJungleClear", "Minimum Mana To Use E In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Tristana.UsePacketCastJungleClear", "Use PacketCast In JungleClear").SetValue(false));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Tristana.SafeJungleClear", "Dont Use Spell In Jungle Clear If Enemy in Dangerous Range").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Tristana.SpellOnlyBigMonster", "Use Spell Only On Big Monster").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleClearLogic();
        }

        public static void JungleClearLogic()
        {
            var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastJungleClear").GetValue<bool>();
            var useQ = SkyLv_Tristana.Menu.Item("Tristana.UseQJungleClear").GetValue<bool>();
            var useE = SkyLv_Tristana.Menu.Item("Tristana.UseEJungleClear").GetValue<bool>();

            var MiniManaQ = SkyLv_Tristana.Menu.Item("Tristana.QMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaE = SkyLv_Tristana.Menu.Item("Tristana.EMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionN = MinionManager.GetMinions(E.Range + 200, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (MinionN.IsValidTarget() && SkyLv_Tristana.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && !SkyLv_Tristana.Menu.Item("Tristana.AfterAttackModeJungleClear").GetValue<bool>())
            {
                if (SkyLv_Tristana.Menu.Item("Tristana.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (useE && Player.ManaPercent > MiniManaE && E.IsReady())
                {
                    if (SkyLv_Tristana.Menu.Item("Tristana.SpellOnlyBigMonster").GetValue<bool>())
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Tristana.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                        {
                            E.CastOnUnit(target, PacketCast);
                        }
                    }
                    else
                        E.CastOnUnit(MinionN, PacketCast);
                }

                if (useQ && Q.IsReady() && Player.ManaPercent > MiniManaQ)
                {
                    if (SkyLv_Tristana.Menu.Item("Tristana.SpellOnlyBigMonster").GetValue<bool>())
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Tristana.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                        {
                            Q.Cast(PacketCast);
                        }
                    }
                    else
                        Q.Cast(PacketCast);
                }
            }
        }
    }
}
