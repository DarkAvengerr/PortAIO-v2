namespace SkyLv_Taric
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Harass
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

        private static Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
        }
        #endregion



        static Harass()
        {
            //Menu

            SkyLv_Taric.Menu.SubMenu("Harass").SubMenu("Q Settings Harass").AddItem(new MenuItem("Taric.UseQHarass", "Use Q In Harass").SetValue(false));
            SkyLv_Taric.Menu.SubMenu("Harass").SubMenu("Q Settings Harass").AddItem(new MenuItem("Taric.QMiniManaHarass", "Minimum Mana To Use Q In Harass").SetValue(new Slider(50, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("Harass").SubMenu("W Settings Harass").AddItem(new MenuItem("Taric.UseWHarass", "Use W In Harass").SetValue(false));
            SkyLv_Taric.Menu.SubMenu("Harass").SubMenu("W Settings Harass").AddItem(new MenuItem("Taric.WMiniManaHarass", "Minimum Mana To Use W In Harass").SetValue(new Slider(50, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("Harass").SubMenu("E Settings Harass").AddItem(new MenuItem("Taric.UseEHarass", "Use E In Harass").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Harass").SubMenu("E Settings Harass").AddItem(new MenuItem("Taric.EMiniManaHarass", "Minimum Mana To Use E In Harass").SetValue(new Slider(0, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("Harass").AddItem(new MenuItem("Taric.UseTaricAAPassiveHarass", "Use All Taric AA Passive In Harass").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Harass").AddItem(new MenuItem("Taric.HarassActive", "Harass !").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            SkyLv_Taric.Menu.SubMenu("Harass").AddItem(new MenuItem("Taric.HarassActiveT", "Harass (toggle) !").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));


            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Taric.Menu.Item("Taric.HarassActive").GetValue<KeyBind>().Active || SkyLv_Taric.Menu.Item("Taric.HarassActiveT").GetValue<KeyBind>().Active)
            {
                HarassLogic();
            }
        }

        public static void HarassLogic()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();
            var UseQHarass = SkyLv_Taric.Menu.Item("Taric.UseQHarass").GetValue<bool>();
            var UseWHarass = SkyLv_Taric.Menu.Item("Taric.UseWHarass").GetValue<bool>();
            var UseEHarass = SkyLv_Taric.Menu.Item("Taric.UseEHarass").GetValue<bool>();

            var QMiniManaHarass = SkyLv_Taric.Menu.Item("Taric.QMiniManaHarass").GetValue<Slider>().Value;
            var WMiniManaHarass = SkyLv_Taric.Menu.Item("Taric.WMiniManaHarass").GetValue<Slider>().Value;
            var EMiniManaHarass = SkyLv_Taric.Menu.Item("Taric.EMiniManaHarass").GetValue<Slider>().Value;

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target.LSIsValidTarget())
            {
                if (UseEHarass && Player.ManaPercent > EMiniManaHarass && E.LSIsReady() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveHarass").GetValue<bool>()))
                {
                    if (Player.LSDistance(target) < E.Range)
                    {
                        E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                        return;
                    }
                }

                if (UseWHarass && Player.ManaPercent > WMiniManaHarass && W.LSIsReady() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveHarass").GetValue<bool>()) && (!E.LSIsReady() || !UseEHarass))
                {
                    W.Cast(Player, PacketCast);
                    return;
                }

                if (UseQHarass && Player.ManaPercent > QMiniManaHarass && Q.LSIsReady() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveHarass").GetValue<bool>()) && (!E.LSIsReady() || !UseEHarass) && (Player.HealthPercent < 100 || (!W.LSIsReady() || !UseWHarass)))
                {
                    Q.Cast(Player, PacketCast);
                    return;
                }
            }
        }
    }
}
