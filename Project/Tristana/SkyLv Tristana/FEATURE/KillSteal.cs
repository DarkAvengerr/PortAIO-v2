using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
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
                return SkyLv_Tristana.Player;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Tristana.E;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Tristana.R;
            }
        }
        #endregion




        static KillSteal()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            SkyLv_Tristana.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Tristana.KS", "Kill Steal").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Tristana.PacketCastKS", "PacketCast KS").SetValue(false));
            SkyLv_Tristana.Menu.SubMenu("Combo").SubMenu("KS Mode").AddSubMenu(new Menu("Spell Settings", "Spell Settings"));
            SkyLv_Tristana.Menu.SubMenu("Combo").SubMenu("KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Tristana.UseAAKS", "KS With AA").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Combo").SubMenu("KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Tristana.UseIgniteKS", "KS With Ignite").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Combo").SubMenu("KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Tristana.UseEKS", "KS With E").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Combo").SubMenu("KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Tristana.UseRKS", "KS With R").SetValue(true));
            

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Tristana.Menu.Item("Tristana.KS").GetValue<bool>())
            {
                HeroKillSteal();
            }
        }

        public static void HeroKillSteal()
        {
            var PacketCastKS = SkyLv_Tristana.Menu.Item("Tristana.PacketCastKS").GetValue<bool>();
            var UseAAKS = SkyLv_Tristana.Menu.Item("Tristana.UseAAKS").GetValue<bool>();
            var UseIgniteKS = SkyLv_Tristana.Menu.Item("Tristana.UseIgniteKS").GetValue<bool>();
            var UseEKS = SkyLv_Tristana.Menu.Item("Tristana.UseEKS").GetValue<bool>();
            var UseRKS = SkyLv_Tristana.Menu.Item("Tristana.UseRKS").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && !target.IsDead && target.Team != ObjectManager.Player.Team && !target.IsZombie && (SkyLv_Tristana.Ignite.Slot != SpellSlot.Unknown || !target.HasBuff("summonerdot"))))
            {
                if (UseAAKS && Orbwalking.CanAttack() && Player.GetAutoAttackDamage(target) > target.Health && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) * 1.2f))
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
                
                if (UseEKS && E.GetDamage(target) > target.Health && target.Distance(Player) <= E.Range)
                {
                    E.Cast(target, PacketCastKS);
                }

                if (UseRKS && target.Distance(Player) <= R.Range && (R.GetDamage(target) > target.Health || (SkyLv_Tristana.ERKSState == true && target.HasBuff("TristanaECharge"))))
                {
                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCastKS);
                    SkyLv_Tristana.ERKSState = false;
                }

                if (UseEKS && target.Distance(Player) <= E.Range && UseRKS && E.GetDamage(target) + R.GetDamage(target) > target.Health && E.IsReady() && R.IsReady() && Player.Mana > E.ManaCost + R.ManaCost)
                {
                    E.Cast(target, PacketCastKS);
                    SkyLv_Tristana.ERKSState = true;
                }

                if (UseIgniteKS && SkyLv_Tristana.Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= SkyLv_Tristana.Ignite.Range)
                {
                    SkyLv_Tristana.Ignite.Cast(target, true);
                }

            }
        }
    }
}
