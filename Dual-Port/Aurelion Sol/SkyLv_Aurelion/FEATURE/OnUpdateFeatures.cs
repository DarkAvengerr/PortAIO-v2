using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class OnUpdateFeatures
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_AurelionSol.Player;
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

        private static Spell R
        {
            get
            {
                return SkyLv_AurelionSol.R;
            }
        }
        #endregion

        static OnUpdateFeatures()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddSubMenu(new Menu("Auto Spell Usage", "Auto Spell Usage"));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("AurelionSol.AutoManageW", "Auto Manage W").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("AurelionSol.AutoR", "Auto R If Minimum Enemy Hit").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddItem(new MenuItem("AurelionSol.MinimumEnemyHitAutoR", "Minimum Enemy Hit To Use R").SetValue(new Slider(3, 1, 5)));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_AurelionSol.Menu.Item("AurelionSol.AutoR").GetValue<bool>())
            {
                AutoR();
            }

            AutoWManager();
        }


        public static void AutoWManager()
        {
            var target = TargetSelector.GetTarget(W2.Range + 50, TargetSelector.DamageType.Magical);
            var PacketCast = SkyLv_AurelionSol.Menu.Item("AurelionSol.UsePacketCastCombo").GetValue<bool>();
            var AutoWManager = SkyLv_AurelionSol.Menu.Item("AurelionSol.AutoManageW").GetValue<bool>();
            if (AutoWManager)
            {
                if (CustomLib.enemyChampionInRange(600 + 300) == 0 && CustomLib.isWInLongRangeMode())
                {
                    W2.Cast(PacketCast);
                }
            }
        }

        public static void AutoR()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var MinimumEnemyHitAutoR = SkyLv_AurelionSol.Menu.Item("AurelionSol.MinimumEnemyHitAutoR").GetValue<Slider>().Value;
            var PacketCast = SkyLv_AurelionSol.Menu.Item("AurelionSol.UsePacketCastCombo").GetValue<bool>();

            if (R.LSIsReady() && Player.Mana >= R.ManaCost)
            {
                R.CastIfWillHit(target, MinimumEnemyHitAutoR, PacketCast);
            }
        }

    }
}
