namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class OnUpdateFeatures
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

        private static Spell R
        {
            get
            {
                return SkyLv_Taric.R;
            }
        }
        #endregion

        static OnUpdateFeatures()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("Combo").AddSubMenu(new Menu("Auto Spell Usage", "Auto Spell Usage"));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddSubMenu(new Menu("Auto Q Settings", "Auto Q Settings"));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto Q Settings").AddItem(new MenuItem("Taric.UseAutoQ", "Use Auto Q Safe Mode").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto Q Settings").AddItem(new MenuItem("Taric.MinimumHpSafeAutoQ", "Minimum Health Percent To Use Auto Q Safe Mode").SetValue(new Slider(25, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto Q Settings").AddItem(new MenuItem("Taric.MinimumEnemySafeAutoQ", "Minimum Enemy In Range To Use Auto Q Safe Mode").SetValue(new Slider(1, 0, 5)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto Q Settings").AddItem(new MenuItem("Taric.MinimumStackSafeAutoQ", "Minimum Q Stack To Use Auto Q Safe Mode").SetValue(new Slider(1, 1, 3)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddSubMenu(new Menu("Auto W Settings", "Auto W Settings"));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto W Settings").AddItem(new MenuItem("Taric.UseAutoW", "Use Auto W Safe Mode").SetValue(false));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto W Settings").AddItem(new MenuItem("Taric.MinimumHpSafeAutoW", "Minimum Health Percent To Use Auto W Safe Mode").SetValue(new Slider(25, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto W Settings").AddItem(new MenuItem("Taric.MinimumEnemySafeAutoW", "Minimum Enemy In Range To Use Auto W Safe Mode").SetValue(new Slider(1, 0, 5)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddSubMenu(new Menu("Auto E Settings", "Auto E Settings"));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto E Settings").AddItem(new MenuItem("Taric.UseAutoEGapCloser", "Use E Anti Gap Closer").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto E Settings").AddItem(new MenuItem("Taric.MinimumHpEGapCloser", "E Anti Gap Closer If Health Percent Under Or Equal").SetValue(new Slider(100, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto E Settings").AddItem(new MenuItem("Taric.MinimumEnemyEGapCloser", "Minimum Enemy In Range To E Anti Gap Closer").SetValue(new Slider(0, 0, 5)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").AddSubMenu(new Menu("Auto R Settings", "Auto R Settings"));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto R Settings").AddItem(new MenuItem("Taric.UseAutoR", "Use Auto R Safe Mode").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto R Settings").AddItem(new MenuItem("Taric.MinimumHpSafeAutoR", "Minimum Health Percent To Use Auto R Safe Mode").SetValue(new Slider(40, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Auto Spell Usage").SubMenu("Auto R Settings").AddItem(new MenuItem("Taric.MinimumEnemySafeAutoR", "Minimum Enemy In Range To Use Auto R Safe Mode").SetValue(new Slider(1, 0, 5)));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Taric.Menu.Item("Taric.UseAutoR").GetValue<bool>())
            {
                AutoR();
            }

            if (SkyLv_Taric.Menu.Item("Taric.UseAutoW").GetValue<bool>())
            {
                AutoW();
            }

            if (SkyLv_Taric.Menu.Item("Taric.UseAutoQ").GetValue<bool>())
            {
                AutoQ();
            }

            if (SkyLv_Taric.Menu.Item("Taric.UseQAlly").GetValue<bool>())
            {
                AutoQAlly();
            }

            if (SkyLv_Taric.Menu.Item("Taric.UseWAlly").GetValue<bool>())
            {
                AutoWAlly();
            }
        }

        public static void AutoR()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();
            var MinimumHpSafeAutoR = SkyLv_Taric.Menu.Item("Taric.MinimumHpSafeAutoR").GetValue<Slider>().Value;
            var MinimumEnemySafeAutoR = SkyLv_Taric.Menu.Item("Taric.MinimumEnemySafeAutoR").GetValue<Slider>().Value;

            if (Player.LSIsRecalling()) return;

            if (R.LSIsReady() && Player.Mana >= R.ManaCost && CustomLib.enemyChampionInPlayerRange(800) >= MinimumEnemySafeAutoR && Player.HealthPercent <= MinimumHpSafeAutoR)
            {
                R.Cast(Player, PacketCast);
            }
        }

        public static void AutoQ()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();
            var MinimumHpSafeAutoQ = SkyLv_Taric.Menu.Item("Taric.MinimumHpSafeAutoQ").GetValue<Slider>().Value;
            var MinimumEnemySafeAutoQ = SkyLv_Taric.Menu.Item("Taric.MinimumEnemySafeAutoQ").GetValue<Slider>().Value;
            var MinimumStackSafeAutoQ = SkyLv_Taric.Menu.Item("Taric.MinimumStackSafeAutoQ").GetValue<Slider>().Value;

            if (Player.LSIsRecalling()) return;

            if (Q.LSIsReady() && Player.Mana >= Q.ManaCost && Q.Instance.Ammo >= MinimumStackSafeAutoQ && CustomLib.enemyChampionInPlayerRange(800) >= MinimumEnemySafeAutoQ && Player.HealthPercent <= MinimumHpSafeAutoQ)
            {
                Q.Cast(Player, PacketCast);
            }
        }

        public static void AutoW()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();
            var MinimumHpSafeAutoW = SkyLv_Taric.Menu.Item("Taric.MinimumHpSafeAutoW").GetValue<Slider>().Value;
            var MinimumEnemySafeAutoW = SkyLv_Taric.Menu.Item("Taric.MinimumEnemySafeAutoW").GetValue<Slider>().Value;

            if (Player.LSIsRecalling()) return;

            if (W.LSIsReady() && Player.Mana >= W.ManaCost && CustomLib.enemyChampionInPlayerRange(800) >= MinimumEnemySafeAutoW && Player.HealthPercent <= MinimumHpSafeAutoW)
            {
                W.Cast(Player, PacketCast);
            }
        }

        public static void AutoQAlly()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();

            if (Player.LSIsRecalling()) return;

            if (SkyLv_Taric.Menu.Item("Taric.UseQAllyMode").GetValue<StringList>().SelectedIndex == 1 && Q.LSIsReady() && Player.Mana >= Q.ManaCost)
            {
                foreach (var AllyHeroQ in HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && Player.LSDistance(x) < Q.Range && 
                Q.Instance.Ammo >= SkyLv_Taric.Menu.Item(x.ChampionName + "MinimumStacksQAlly").GetValue<Slider>().Value &&
                x.HealthPercent <= SkyLv_Taric.Menu.Item(x.ChampionName + "MinimumHpQAlly").GetValue<Slider>().Value))
                {
                    if (AllyHeroQ.LSIsValidTarget())
                    {
                        Q.Cast(PacketCast);
                        return;
                    }
                }
            }
        }

        public static void AutoWAlly()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();

            if (Player.LSIsRecalling()) return;

            if (SkyLv_Taric.Menu.Item("Taric.UseWAllyMode").GetValue<StringList>().SelectedIndex == 1 && W.LSIsReady() && Player.Mana >= W.ManaCost)
            {
                var AllyHeroW = HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && Player.LSDistance(x) <= W.Range &&
                !SkyLv_Taric.Menu.Item(x.ChampionName + "IncomingDamageWAlly", true).GetValue<bool>() &&
                x.HealthPercent <= SkyLv_Taric.Menu.Item(x.ChampionName + "MinimumHpWAlly").GetValue<Slider>().Value).MinOrDefault(t => t.HealthPercent);

                if (AllyHeroW.LSIsValidTarget())
                {
                    W.Cast(AllyHeroW, PacketCast);
                    return;
                }
            }
        }

    }
}
