using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Harass
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
        #endregion



        static Harass()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.UseQHarass", "Use Q In Harass").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.QMiniManaHarass", "Minimum Mana To Use Q In Harass").SetValue(new Slider(0, 0, 100)));
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.UseEHarass", "Use E In Harass").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.EMiniManaHarass", "Minimum Mana To Use E In Harass").SetValue(new Slider(0, 0, 100)));
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.UsePacketCastHarass", "Use PacketCast In Harass").SetValue(false));
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.HarassActive", "Harass Key!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.HarassActive2", "Harass Key 2!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));


            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!SkyLv_Tristana.Menu.Item("Tristana.AfterAttackModeHarass").GetValue<bool>() && (SkyLv_Tristana.Menu.Item("Tristana.HarassActive").GetValue<KeyBind>().Active || SkyLv_Tristana.Menu.Item("Tristana.HarassActive2").GetValue<KeyBind>().Active || SkyLv_Tristana.Menu.Item("Tristana.HarassActiveT").GetValue<KeyBind>().Active))
            {
                HarassLogic();
            }
        }

        public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastHarass").GetValue<bool>();

            var useQ = SkyLv_Tristana.Menu.Item("Tristana.UseQHarass").GetValue<bool>();
            var MiniManaQ = SkyLv_Tristana.Menu.Item("Tristana.QMiniManaHarass").GetValue<Slider>().Value;

            var useE = SkyLv_Tristana.Menu.Item("Tristana.UseEHarass").GetValue<bool>();
            var MiniManaE = SkyLv_Tristana.Menu.Item("Tristana.EMiniManaHarass").GetValue<Slider>().Value;

            if (useE && E.IsReady() && target.IsValidTarget(E.Range) && Player.ManaPercent > MiniManaE)
                E.CastOnUnit(target, PacketCast);

            if (useQ && Q.IsReady() && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)) && Player.ManaPercent > MiniManaQ)
                Q.Cast(PacketCast);
        }
    }
}
