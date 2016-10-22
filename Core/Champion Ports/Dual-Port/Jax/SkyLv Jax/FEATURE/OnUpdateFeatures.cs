using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class OnUpdateFeatures
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Jax.Player;
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

        static OnUpdateFeatures()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("Combo").AddSubMenu(new Menu("Auto Spell Usage", "Auto Spell Usage"));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Jax.AutoE", "Auto Second E If Minimum Enemy Hit").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Jax.MinimumEnemyHitAutoE", "Minimum Enemy Hit To Auto Second E").SetValue(new Slider(3, 1, 5)));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Jax.UseAutoR", "Use Auto R Safe Mode").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Jax.MinimumHpSafeAutoR", "Minimum Health Percent To Use Auto R Safe Mode").SetValue(new Slider(40, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Jax.UseAutoE", "Use Auto E Safe Mode").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Jax.MinimumHpSafeAutoE", "Minimum Health Percent To Use Auto E Safe Mode").SetValue(new Slider(25, 0, 100)));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Jax.Menu.Item("Jax.UseAutoE").GetValue<bool>())
            {
                AutoSafeE();
            }

            if (SkyLv_Jax.Menu.Item("Jax.UseAutoR").GetValue<bool>())
            {
                AutoR();
            }

            if (SkyLv_Jax.Menu.Item("Jax.AutoE").GetValue<bool>())
            {
                AutoE();
            }
        }

        public static void AutoE()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var MinimumEnemyHitAutoE = SkyLv_Jax.Menu.Item("Jax.MinimumEnemyHitAutoE").GetValue<Slider>().Value;
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();

            if (target.IsValidTarget(E.Range) && CustomLib.iSJaxEActive())
            {
                E.CastIfWillHit(target, MinimumEnemyHitAutoE, PacketCast);
            }
        }

        public static void AutoSafeE()
        {
            var MinimumHpSafeAutoE = SkyLv_Jax.Menu.Item("Jax.MinimumHpSafeAutoE").GetValue<Slider>().Value;
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();

            if (CustomLib.iSJaxEActive() && Player.HealthPercent <= MinimumHpSafeAutoE && CustomLib.enemyChampionInPlayerRange(E.Range) > 0)
            {
                E.Cast();
            }
        }

        public static void AutoR()
        {
            var MinimumHpSafeAutoR = SkyLv_Jax.Menu.Item("Jax.MinimumHpSafeAutoR").GetValue<Slider>().Value;
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();

            if (R.IsReady() && Player.Mana >= R.ManaCost && CustomLib.enemyChampionInPlayerRange(700) > 0 && Player.HealthPercent <= MinimumHpSafeAutoR)
            {
                R.Cast(PacketCast);
            }
        }

    }
}
