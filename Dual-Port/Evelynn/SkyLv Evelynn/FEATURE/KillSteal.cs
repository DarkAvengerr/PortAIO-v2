using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
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

        private static Spell R
        {
            get
            {
                return SkyLv_Evelynn.R;
            }
        }
        #endregion




        static KillSteal()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            SkyLv_Evelynn.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Evelynn.UseIgniteKS", "KS With Ignite").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Evelynn.UseQKS", "KS With Q").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Evelynn.UseEKS", "KS With E").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Evelynn.UseRKS", "KS With R").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Evelynn.PacketCastKS", "PacketCast KS").SetValue(false));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            KS();
        }

        public static void KS()
        {
            var PacketCast = SkyLv_Evelynn.Menu.Item("Evelynn.PacketCastKS").GetValue<bool>();
            var useIgniteKS = SkyLv_Evelynn.Menu.Item("Evelynn.UseIgniteKS").GetValue<bool>();
            var useQKS = SkyLv_Evelynn.Menu.Item("Evelynn.UseQKS").GetValue<bool>();
            var useEKS = SkyLv_Evelynn.Menu.Item("Evelynn.UseEKS").GetValue<bool>();
            var useRKS = SkyLv_Evelynn.Menu.Item("Evelynn.UseRKS").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && !target.IsDead && target.Team != ObjectManager.Player.Team && !target.IsZombie && (SkyLv_Evelynn.Ignite.Slot != SpellSlot.Unknown || !target.HasBuff("summonerdot"))))
            {
                if (useQKS && Q.GetDamage(target) > target.Health && Player.Distance(target) <= Q.Range)
                {
                    Q.Cast(target, PacketCast);
                }

                if (useEKS && E.GetDamage(target) > target.Health && Player.Distance(target) <= E.Range)
                {
                    E.Cast(target, PacketCast);
                }

                if (useRKS && R.GetDamage(target) > target.Health && Player.Distance(target) <= R.Range)
                {
                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                }

                if (useIgniteKS && SkyLv_Evelynn.Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= SkyLv_Evelynn.Ignite.Range)
                {
                    SkyLv_Evelynn.Ignite.Cast(target, true);
                }
            }
            
        }

    }
}
