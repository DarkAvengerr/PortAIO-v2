using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
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

        private static Spell E
        {
            get
            {
                return SkyLv_Evelynn.E;
            }
        }
        #endregion



        static Harass()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("Harass").AddItem(new MenuItem("Evelynn.UseQHarass", "Use Q In Harass").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Harass").AddItem(new MenuItem("Evelynn.QMiniManaHarass", "Minimum Mana To Use Q In Harass").SetValue(new Slider(50, 0, 100)));
            SkyLv_Evelynn.Menu.SubMenu("Harass").AddItem(new MenuItem("Evelynn.UseEHarass", "Use E In Harass").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Harass").AddItem(new MenuItem("Evelynn.EMiniManaHarass", "Minimum Mana To Use E In Harass").SetValue(new Slider(50, 0, 100)));
            SkyLv_Evelynn.Menu.SubMenu("Harass").AddItem(new MenuItem("Evelynn.UsePacketCastHarass", "Use PacketCast In Harass").SetValue(false));
            SkyLv_Evelynn.Menu.SubMenu("Harass").AddItem(new MenuItem("Evelynn.HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            SkyLv_Evelynn.Menu.SubMenu("Harass").AddItem(new MenuItem("Evelynn.HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));


            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Evelynn.Menu.Item("Evelynn.HarassActive").GetValue<KeyBind>().Active || SkyLv_Evelynn.Menu.Item("Evelynn.HarassActiveT").GetValue<KeyBind>().Active)
            {
                HarassLogic();
            }
        }

        public static void HarassLogic()
        {
            var PacketCast = SkyLv_Evelynn.Menu.Item("Evelynn.UsePacketCastHarass").GetValue<bool>();

            var useQ = SkyLv_Evelynn.Menu.Item("Evelynn.UseQHarass").GetValue<bool>();
            var MiniManaQ = SkyLv_Evelynn.Menu.Item("Evelynn.QMiniManaHarass").GetValue<Slider>().Value;

            var useE = SkyLv_Evelynn.Menu.Item("Evelynn.UseEHarass").GetValue<bool>();
            var MiniManaE = SkyLv_Evelynn.Menu.Item("Evelynn.EMiniManaHarass").GetValue<Slider>().Value;


            if (useQ && Q.IsReady() && Player.Mana >= Q.ManaCost && Player.ManaPercent > MiniManaQ)
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (CustomLib.enemyChampionInPlayerRange(Q.Range) > 0)
                    Q.Cast(PacketCast);
            }

            if (useE && E.IsReady() && Player.ManaPercent > MiniManaE)
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                E.CastOnUnit(target, PacketCast);
            }
        }
    }
}
