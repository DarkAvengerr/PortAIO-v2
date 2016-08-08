using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
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
                return SkyLv_AurelionSol.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_AurelionSol.Q;
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


        static Combo()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddItem(new MenuItem("AurelionSol.UseQCombo", "Use Q In Combo").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddItem(new MenuItem("AurelionSol.UseWCombo", "Use W In Combo").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddItem(new MenuItem("AurelionSol.UseRCombo", "Use R In Combo").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddItem(new MenuItem("AurelionSol.MinimumEnemyHitComboR", "Minimum Enemy Hit To Use R In Combo").SetValue(new Slider(2, 1, 5)));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddItem(new MenuItem("AurelionSol.UsePacketCastCombo", "Use PacketCast In Combo").SetValue(false));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            ComboLogic();
        }

        public static void ComboLogic()
        {
            var PacketCast = SkyLv_AurelionSol.Menu.Item("AurelionSol.UsePacketCastCombo").GetValue<bool>();
            var useQ = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseQCombo").GetValue<bool>();
            var useW = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseWCombo").GetValue<bool>();
            var useR = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseRCombo").GetValue<bool>();
            var MinimumEnemyHitComboR = SkyLv_AurelionSol.Menu.Item("AurelionSol.MinimumEnemyHitComboR").GetValue<Slider>().Value;

            if (SkyLv_AurelionSol.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = TargetSelector.GetTarget(W2.Range + 50, TargetSelector.DamageType.Magical);

                if (target.LSIsValidTarget())
                {
                    if (useR && R.LSIsReady() && Player.Mana >= R.ManaCost)
                    {
                        R.CastIfWillHit(target, MinimumEnemyHitComboR, PacketCast);
                    }

                    if (useQ && Q.LSIsReady() && Player.Mana >= Q.ManaCost)
                    {
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                    }

                    if (useW)
                    {
                        if (target.LSDistance(Player) <= W1.Range + 50 && CustomLib.isWInLongRangeMode())
                        {
                            W2.Cast(PacketCast);
                        }

                        if (target.LSDistance(Player) > W1.Range + 50 && target.LSDistance(Player) < W2.Range + 50 && !CustomLib.isWInLongRangeMode())
                        {
                            W1.Cast(PacketCast);
                        }

                        else if (CustomLib.enemyChampionInRange(900) == 0 && CustomLib.isWInLongRangeMode())
                        {
                            W2.Cast(PacketCast);
                        }
                    }
                }
                
            }
        }
    }
}
