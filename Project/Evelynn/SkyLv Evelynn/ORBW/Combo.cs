using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
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
                return SkyLv_Evelynn.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Evelynn.Q;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Evelynn.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Evelynn.E;
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


        static Combo()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("Combo").AddItem(new MenuItem("Evelynn.UseQCombo", "Use Q In Combo").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").AddItem(new MenuItem("Evelynn.UseWCombo", "Use W In Combo").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").AddItem(new MenuItem("Evelynn.UseECombo", "Use E In Combo").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").AddItem(new MenuItem("Evelynn.UseRCombo", "Use R In Combo").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").AddItem(new MenuItem("Evelynn.MinimumEnemyHitR", "Minimum Enemy Hit To Use R").SetValue(new Slider(2, 1, 5)));
            SkyLv_Evelynn.Menu.SubMenu("Combo").AddItem(new MenuItem("Evelynn.UsePacketCastCombo", "Use PacketCast In Combo").SetValue(false));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            ComboLogic();
        }

        public static void ComboLogic()
        {
            var PacketCast = SkyLv_Evelynn.Menu.Item("Evelynn.UsePacketCastCombo").GetValue<bool>();
            var useQ = SkyLv_Evelynn.Menu.Item("Evelynn.UseQCombo").GetValue<bool>();
            var useW = SkyLv_Evelynn.Menu.Item("Evelynn.UseWCombo").GetValue<bool>();
            var useE = SkyLv_Evelynn.Menu.Item("Evelynn.UseWCombo").GetValue<bool>();
            var useR = SkyLv_Evelynn.Menu.Item("Evelynn.UseRCombo").GetValue<bool>();
            var MinimumEnemyHitR = SkyLv_Evelynn.Menu.Item("Evelynn.MinimumEnemyHitR").GetValue<Slider>().Value;

            if (SkyLv_Evelynn.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (useR && R.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                    if (target.IsValidTarget())
                    R.CastIfWillHit(target, MinimumEnemyHitR, PacketCast);
                }

                if (useW && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                    if (target.IsValidTarget())
                        if (Player.Mana > R.ManaCost + E.ManaCost + Q.ManaCost && (CustomLib.enemyChampionInPlayerRange(W.Range) > 0 || Player.HasBuffOfType(BuffType.Slow)))
                        W.Cast(PacketCast);
                }

                if (useE && E.IsReady())
                {
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                    if (target.IsValidTarget())
                        E.CastOnUnit(target, PacketCast);
                }

                if (useQ && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                    if (target.IsValidTarget())
                        Q.Cast(PacketCast);
                }
            }
        }
    }
}
