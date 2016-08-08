using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
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




        static KillSteal()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("AurelionSol.UseIgniteKS", "KS With Ignite").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("AurelionSol.UseQKS", "KS With Q").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("AurelionSol.UseWKS", "KS With W").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("AurelionSol.UseRKS", "KS With R").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("AurelionSol.PacketCastKS", "PacketCast KS").SetValue(false));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            KS();
        }

        public static void KS()
        {
            var PacketCast = SkyLv_AurelionSol.Menu.Item("AurelionSol.PacketCastKS").GetValue<bool>();
            var UseIgniteKS = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseIgniteKS").GetValue<bool>();
            var UseQKS = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseQKS").GetValue<bool>();
            var UseWKS = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseWKS").GetValue<bool>();
            var UseRKS = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseRKS").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && !target.IsDead && target.Team != ObjectManager.Player.Team && !target.IsZombie && (SkyLv_AurelionSol.Ignite.Slot != SpellSlot.Unknown || !target.LSHasBuff("summonerdot"))))
            {
                if (UseQKS && Player.LSDistance(target) <= Q.Range && Q.LSIsReady() && target.Health < CustomLib.QDamage(target) && Player.Mana >= Q.ManaCost)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                }

                if (UseWKS && W1.LSIsReady() && target.Health < W1.GetDamage(target))
                {
                    if (Player.LSDistance(target) > W1.Range - 20 && Player.LSDistance(target) < W1.Range + 20 && CustomLib.isWInLongRangeMode())
                    {
                        W2.Cast(PacketCast);
                    }

                    if (Player.LSDistance(target) > W2.Range - 20 && Player.LSDistance(target) < W2.Range + 20 && !CustomLib.isWInLongRangeMode())
                    {
                        W1.Cast(PacketCast);
                    }
                }

                if (UseRKS && Player.LSDistance(target) <= R.Range && R.LSIsReady() && target.Health < CustomLib.RDamage(target) && Player.Mana >= R.ManaCost)
                {
                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                }

                if (UseIgniteKS && SkyLv_AurelionSol.Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.LSDistance(target) <= SkyLv_AurelionSol.Ignite.Range)
                {
                    SkyLv_AurelionSol.Ignite.Cast(target, true);
                }
            }
        }
    }
}
