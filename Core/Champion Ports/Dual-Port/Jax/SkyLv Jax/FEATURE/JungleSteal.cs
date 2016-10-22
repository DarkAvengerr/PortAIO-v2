using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class JungleSteal
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

        static JungleSteal()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("JungleClear").AddSubMenu(new Menu("Jungle KS Mode", "Jungle KS Mode"));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddItem(new MenuItem("Jax.JungleKS", "Jungle KS").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Advanced Settings").AddItem(new MenuItem("Jax.UseQJungleKS", "KS With Q").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Advanced Settings").AddItem(new MenuItem("Jax.UseWQJungleKS", "KS With W + Q").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Advanced Settings").AddItem(new MenuItem("Jax.UseSecondEJungleKS", "KS With Second E").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleKS();
        }

        public static void JungleKS()
        {
            if (SkyLv_Jax.Menu.Item("Jax.JungleKS").GetValue<bool>())
            {
                var UseQJungleKS = SkyLv_Jax.Menu.Item("Jax.UseQJungleKS").GetValue<bool>();
                var UseWQJungleKS = SkyLv_Jax.Menu.Item("Jax.UseWQJungleKS").GetValue<bool>();
                var UseSecondEJungleKS = SkyLv_Jax.Menu.Item("Jax.UseSecondEJungleKS").GetValue<bool>();
                var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();

                foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Jax.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                {
                    if (UseQJungleKS && Q.GetDamage(target) > target.Health && Player.Distance(target) <= Q.Range && Player.Mana >= Q.ManaCost)
                    {
                        Q.Cast(target, PacketCast);
                    }

                    if (UseSecondEJungleKS && E.GetDamage(target) > target.Health && Player.Distance(target) <= E.Range && CustomLib.iSJaxEActive())
                    {
                        E.Cast(PacketCast);
                    }

                    if (UseWQJungleKS && Player.Distance(target) <= Q.Range && Q.IsReady() && W.IsReady() && Player.Mana > Q.ManaCost + W.ManaCost && target.Health < Q.GetDamage(target) + W.GetDamage(target))
                    {
                        W.Cast();
                        Q.Cast(target, PacketCast);
                    }
                }
            }
        }
    }
}
