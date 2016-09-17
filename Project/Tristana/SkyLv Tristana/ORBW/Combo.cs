using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Combo
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


        static Combo()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Combo").AddItem(new MenuItem("Tristana.UseQCombo", "Use Q In Combo").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Combo").AddItem(new MenuItem("Tristana.UseECombo", "Use E In Combo").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Combo").AddItem(new MenuItem("Tristana.UseRCombo", "Use R Safe Mode In Combo").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Combo").AddItem(new MenuItem("Tristana.MinimumHpSafeR", "Minimum Health Percent To Use R Safe Mode In Combo").SetValue(new Slider(35, 0, 100)));
            SkyLv_Tristana.Menu.SubMenu("Combo").AddItem(new MenuItem("Tristana.UsePacketCastCombo", "Use PacketCast In Combo").SetValue(false));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            ComboLogic();
        }

        public static void ComboLogic()
        {
            var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastCombo").GetValue<bool>();
            var useQ = SkyLv_Tristana.Menu.Item("Tristana.UseQCombo").GetValue<bool>();
            var useE = SkyLv_Tristana.Menu.Item("Tristana.UseECombo").GetValue<bool>();



            if (SkyLv_Tristana.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                foreach (var closeTarget in ObjectManager.Get<AIHeroClient>().Where(t => !t.IsMe && t.Team != ObjectManager.Player.Team && t.Distance(Player) < 300))
                {
                    if (Player.HealthPercent < SkyLv_Tristana.Menu.Item("Tristana.MinimumHpSafeR").GetValue<Slider>().Value)
                    {
                        R.CastOnUnit(closeTarget, PacketCast);
                    }
                }

                if (!SkyLv_Tristana.Menu.Item("Tristana.AfterAttackModeCombo").GetValue<bool>())
                {
                    if (useQ && Q.IsReady() && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                        Q.Cast(PacketCast);
                    if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                        E.CastOnUnit(target, PacketCast);
                }

            }
        }
    }
}
