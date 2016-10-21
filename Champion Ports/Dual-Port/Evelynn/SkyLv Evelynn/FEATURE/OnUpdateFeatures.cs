using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
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
                return SkyLv_Evelynn.Player;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Evelynn.W;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Evelynn.R;
            }
        }
        #endregion

        static OnUpdateFeatures()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("Combo").AddSubMenu(new Menu("Auto Spell Usage", "Auto Spell Usage"));
            SkyLv_Evelynn.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Evelynn.AutoR", "Auto R If Minimum Enemy Hit").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Evelynn.MinimumEnemyHitAutoR", "Minimum Enemy Hit To Auto R").SetValue(new Slider(3, 1, 5)));
            SkyLv_Evelynn.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("Evelynn.AutoWSlow", "Auto W On Slow").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Evelynn.Menu.Item("Evelynn.AutoR").GetValue<bool>())
            {
                AutoR();
            }

            if (SkyLv_Evelynn.Menu.Item("Evelynn.AutoWSlow").GetValue<bool>())
            {
                AutoW();
            }
        }

        public static void AutoR()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var MinimumEnemyHitAutoR = SkyLv_Evelynn.Menu.Item("Evelynn.MinimumEnemyHitAutoR").GetValue<Slider>().Value;
            var PacketCast = SkyLv_Evelynn.Menu.Item("Evelynn.UsePacketCastCombo").GetValue<bool>();

            if (R.IsReady())
            {
                R.CastIfWillHit(target, MinimumEnemyHitAutoR, PacketCast);
            }
        }

        public static void AutoW()
        {
            var PacketCast = SkyLv_Evelynn.Menu.Item("Evelynn.UsePacketCastCombo").GetValue<bool>();

            if (W.IsReady())
            {
                if (Player.HasBuffOfType(BuffType.Slow))
                    W.Cast(PacketCast);
            }
        }

    }
}
