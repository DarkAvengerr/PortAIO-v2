using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Combo
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

        private static Spell R
        {
            get
            {
                return SkyLv_Jax.R;
            }
        }
        #endregion


        static Combo()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("Combo").AddItem(new MenuItem("Jax.UseQCombo", "Use Q In Combo").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").AddItem(new MenuItem("Jax.UseWCombo", "Use W In Combo").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").AddItem(new MenuItem("Jax.UseECombo", "Use E In Combo").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").AddItem(new MenuItem("Jax.UseRCombo", "Use R Safe Mode In Combo").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").AddItem(new MenuItem("Jax.MinimumEnemySafeR", "Minimum Enemy Around To Use R Safe Mode In Combo").SetValue(new Slider(2, 1, 5)));
            SkyLv_Jax.Menu.SubMenu("Combo").AddItem(new MenuItem("Jax.MinimumEnemyHitSecondE", "Minimum Enemy Hit To Use Second E").SetValue(new Slider(2, 1, 5)));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            ComboLogic();
        }

        public static void ComboLogic()
        {
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();
            var useQ = SkyLv_Jax.Menu.Item("Jax.UseQCombo").GetValue<bool>();
            var useW = SkyLv_Jax.Menu.Item("Jax.UseWCombo").GetValue<bool>();
            var useE = SkyLv_Jax.Menu.Item("Jax.UseECombo").GetValue<bool>();
            var useR = SkyLv_Jax.Menu.Item("Jax.UseRCombo").GetValue<bool>();
            var MinimumEnemyHitSecondE = SkyLv_Jax.Menu.Item("Jax.MinimumEnemyHitSecondE").GetValue<Slider>().Value;
            var MinimumEnemySafeR = SkyLv_Jax.Menu.Item("Jax.MinimumEnemySafeR").GetValue<Slider>().Value;

            if (SkyLv_Jax.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (useR && R.IsReady() && CustomLib.enemyChampionInPlayerRange(500) >= MinimumEnemySafeR)
                {
                    R.Cast(PacketCast);
                }

                if (useE && E.IsReady() && !CustomLib.iSJaxEActive())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget() && (Player.Distance(target) <= E.Range || (Player.Distance(target) < E.Range * 1.5 && useQ && Q.IsReady() && Player.Mana > Q.ManaCost + E.ManaCost)))
                    {
                        E.Cast(PacketCast);
                    }
                }

                if (useQ && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget() && Player.Distance(target) > E.Range * 1.5 && Player.Mana >= Q.ManaCost + W.ManaCost + E.ManaCost)
                        Q.Cast(target, PacketCast);
                }

                if (!SkyLv_Jax.Menu.Item("Jax.AfterAttackModeCombo").GetValue<bool>())
                {
                    if (useW && W.IsReady() && Player.Mana >= W.ManaCost)
                    {
                        var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                        if (target.IsValidTarget() && Orbwalking.CanAttack())
                            W.Cast(PacketCast);
                    }
                }
            }
        }
    }
}
