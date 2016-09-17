using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class KillSteal
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

        private static Spell R
        {
            get
            {
                return SkyLv_Jax.R;
            }
        }
        #endregion




        static KillSteal()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Jax.UseIgniteKS", "KS With Ignite").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Jax.UseQKS", "KS With Q").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Jax.UseWQKS", "KS With W + Q").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Jax.UseWQIgniteKS", "KS With W + Q + Ignite").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Jax.UseSecondEKS", "KS With Second E").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            KS();
        }

        public static void KS()
        {
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();
            var UseIgniteKS = SkyLv_Jax.Menu.Item("Jax.UseIgniteKS").GetValue<bool>();
            var UseQKS = SkyLv_Jax.Menu.Item("Jax.UseQKS").GetValue<bool>();
            var UseWQKS = SkyLv_Jax.Menu.Item("Jax.UseWQKS").GetValue<bool>();
            var UseWQIgniteKS = SkyLv_Jax.Menu.Item("Jax.UseWQIgniteKS").GetValue<bool>();
            var UseSecondEKS = SkyLv_Jax.Menu.Item("Jax.UseSecondEKS").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && !target.IsDead && target.Team != ObjectManager.Player.Team && !target.IsZombie && (SkyLv_Jax.Ignite.Slot != SpellSlot.Unknown || !target.HasBuff("summonerdot"))))
            {
                if (UseQKS && Q.GetDamage(target) > target.Health && Player.Distance(target) <= Q.Range && Player.Mana >= Q.ManaCost)
                {
                    Q.Cast(target, PacketCast);
                }

                if (UseSecondEKS && E.GetDamage(target) > target.Health && Player.Distance(target) <= E.Range && CustomLib.iSJaxEActive())
                {
                    E.Cast(PacketCast);
                }

                if (UseWQKS && Player.Distance(target) <= Q.Range && Q.IsReady() && W.IsReady() && Player.Mana > Q.ManaCost + W.ManaCost && target.Health < Q.GetDamage(target) + W.GetDamage(target))
                {
                    W.Cast();
                    Q.Cast(target, PacketCast);
                }

                if (UseIgniteKS && SkyLv_Jax.Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= SkyLv_Jax.Ignite.Range)
                {
                    SkyLv_Jax.Ignite.Cast(target, PacketCast);
                }

                if (UseWQIgniteKS && Player.Distance(target) <= Q.Range && Q.IsReady() && W.IsReady() && SkyLv_Jax.Ignite.Slot != SpellSlot.Unknown && Player.Mana > Q.ManaCost + W.ManaCost && target.Health < Q.GetDamage(target) + W.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                {
                    W.Cast();
                    Q.Cast(target, PacketCast);
                    SkyLv_Jax.Ignite.Cast(target, PacketCast);
                }
            }
            
        }

    }
}
