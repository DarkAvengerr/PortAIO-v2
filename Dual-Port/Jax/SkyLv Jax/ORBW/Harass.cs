using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
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
                return SkyLv_Jax.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Jax.Q;
            }
        }
        private static Spell W
        {
            get
            {
                return SkyLv_Jax.W;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Jax.E;
            }
        }
        #endregion



        static Harass()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("Harass").SubMenu("Q Settings Harass").AddItem(new MenuItem("Jax.UseQHarass", "Use Q In Harass").SetValue(false));
            SkyLv_Jax.Menu.SubMenu("Harass").SubMenu("Q Settings Harass").AddItem(new MenuItem("Jax.QMiniManaHarass", "Minimum Mana To Use Q In Harass").SetValue(new Slider(50, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("Harass").SubMenu("W Settings Harass").AddItem(new MenuItem("Jax.UseWHarass", "Use W In Harass").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Harass").SubMenu("W Settings Harass").AddItem(new MenuItem("Jax.WMiniManaHarass", "Minimum Mana To Use W In Harass").SetValue(new Slider(0, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("Harass").SubMenu("E Settings Harass").AddItem(new MenuItem("Jax.UseEHarass", "Use E In Harass").SetValue(false));
            SkyLv_Jax.Menu.SubMenu("Harass").SubMenu("E Settings Harass").AddItem(new MenuItem("Jax.UseSecondEHarass", "Use Second E In Harass").SetValue(false));
            SkyLv_Jax.Menu.SubMenu("Harass").SubMenu("E Settings Harass").AddItem(new MenuItem("Jax.EMiniManaHarass", "Minimum Mana To Use E In Harass").SetValue(new Slider(0, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("Harass").AddItem(new MenuItem("Jax.HarassActive", "Harass !").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            SkyLv_Jax.Menu.SubMenu("Harass").AddItem(new MenuItem("Jax.HarassActiveT", "Harass (toggle) !").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));


            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Jax.Menu.Item("Jax.HarassActive").GetValue<KeyBind>().Active || SkyLv_Jax.Menu.Item("Jax.HarassActiveT").GetValue<KeyBind>().Active)
            {
                HarassLogic();
            }
        }

        public static void HarassLogic()
        {
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();
            var UseQHarass = SkyLv_Jax.Menu.Item("Jax.UseQHarass").GetValue<bool>();
            var UseWHarass = SkyLv_Jax.Menu.Item("Jax.UseWHarass").GetValue<bool>();
            var UseEHarass = SkyLv_Jax.Menu.Item("Jax.UseEHarass").GetValue<bool>();
            var UseSecondEHarass = SkyLv_Jax.Menu.Item("Jax.UseSecondEHarass").GetValue<bool>();

            var QMiniManaHarass = SkyLv_Jax.Menu.Item("Jax.QMiniManaHarass").GetValue<Slider>().Value;
            var WMiniManaHarass = SkyLv_Jax.Menu.Item("Jax.WMiniManaHarass").GetValue<Slider>().Value;
            var EMiniManaHarass = SkyLv_Jax.Menu.Item("Jax.EMiniManaHarass").GetValue<Slider>().Value;

            if (UseEHarass && E.IsReady() && !CustomLib.iSJaxEActive())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && (Player.Distance(target) <= E.Range || (Player.Distance(target) > E.Range * 1.5 && UseQHarass && Q.IsReady() && Player.Mana > Q.ManaCost + E.ManaCost)))
                {
                    E.Cast(PacketCast);
                }
            }

            if (UseSecondEHarass && CustomLib.iSJaxEActive())
            {
                var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && Player.Distance(target) < E.Range)
                {
                    E.Cast(PacketCast);
                }
            }

            if (UseQHarass && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && target.Health < Q.GetDamage(target) + W.GetDamage(target) * 2 + E.GetDamage(target) && Player.Mana >= Q.ManaCost + W.ManaCost + E.ManaCost)
                    Q.Cast(target, PacketCast);
            }

            if (!SkyLv_Jax.Menu.Item("Jax.AfterAttackModeHarass").GetValue<bool>())
            {
                if (UseWHarass && W.IsReady() && Player.Mana >= W.ManaCost)
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget() && Orbwalking.CanAttack())
                        W.Cast(PacketCast);
                }
            }
        }
    }
}
