using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
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
        #endregion



        static Harass()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Harass").AddItem(new MenuItem("AurelionSol.UseQHarass", "Use Q In Harass").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Harass").AddItem(new MenuItem("AurelionSol.QMiniManaHarass", "Minimum Mana To Use Q In Harass").SetValue(new Slider(50, 0, 100)));
            SkyLv_AurelionSol.Menu.SubMenu("Harass").AddItem(new MenuItem("AurelionSol.UseWHarass", "Use W In Harass").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Harass").AddItem(new MenuItem("AurelionSol.WMiniManaHarass", "Minimum Mana To Use W In Harass").SetValue(new Slider(50, 0, 100)));
            SkyLv_AurelionSol.Menu.SubMenu("Harass").AddItem(new MenuItem("AurelionSol.UsePacketCastHarass", "Use PacketCast In Harass").SetValue(false));
            SkyLv_AurelionSol.Menu.SubMenu("Harass").AddItem(new MenuItem("AurelionSol.HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            SkyLv_AurelionSol.Menu.SubMenu("Harass").AddItem(new MenuItem("AurelionSol.HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));


            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_AurelionSol.Menu.Item("AurelionSol.HarassActive").GetValue<KeyBind>().Active || SkyLv_AurelionSol.Menu.Item("AurelionSol.HarassActiveT").GetValue<KeyBind>().Active)
            {
                HarassLogic();
            }
        }

        public static void HarassLogic()
        {

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                var PacketCast = SkyLv_AurelionSol.Menu.Item("AurelionSol.UsePacketCastHarass").GetValue<bool>();

                var useQ = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseQHarass").GetValue<bool>();
                var MiniManaQ = SkyLv_AurelionSol.Menu.Item("AurelionSol.QMiniManaHarass").GetValue<Slider>().Value;

                var useW = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseWHarass").GetValue<bool>();
                var MiniManaW = SkyLv_AurelionSol.Menu.Item("AurelionSol.WMiniManaHarass").GetValue<Slider>().Value;

                if (useQ && Q.IsReady() && Player.Mana >= Q.ManaCost && Player.ManaPercent > MiniManaQ)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                }

                if (useW && W1.IsReady() && Player.ManaPercent > MiniManaW)
                {
                    if (Player.ManaPercent <= MiniManaW && CustomLib.isWInLongRangeMode())
                    {
                        W2.Cast(PacketCast);
                    }

                    if (Player.Distance(target) > W1.Range - 20 && Player.Distance(target) < W1.Range + 20 && CustomLib.isWInLongRangeMode())
                    {
                        W2.Cast(PacketCast);
                    }

                    if (Player.Distance(target) > W2.Range - 20 && Player.Distance(target) < W2.Range + 20 && !CustomLib.isWInLongRangeMode() && Player.ManaPercent > MiniManaW)
                    {
                        W1.Cast(PacketCast);
                    }
                }
            }
        }
    }
}
