namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class KillSteal
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
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




        static KillSteal()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Taric.KS", "Kill Steal").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("KS Mode").AddSubMenu(new Menu("Spell Settings", "Spell Settings"));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Taric.UseAAKS", "KS With AA").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Taric.UseIgniteKS", "KS With Ignite").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Taric.UseEKS", "KS With E").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            KS();
        }

        public static void KS()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();
            var UseIgniteKS = SkyLv_Taric.Menu.Item("Taric.UseIgniteKS").GetValue<bool>();
            var UseAAKS = SkyLv_Taric.Menu.Item("Taric.UseAAKS").GetValue<bool>();
            var UseEKS = SkyLv_Taric.Menu.Item("Taric.UseEKS").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && !target.IsDead && target.Team != ObjectManager.Player.Team && !target.IsZombie && (SkyLv_Taric.Ignite.Slot != SpellSlot.Unknown || !target.HasBuff("summonerdot"))))
            {
                if (UseAAKS && Orbwalking.CanAttack() && Player.GetAutoAttackDamage(target) > target.Health && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }

                if (UseEKS && E.GetDamage(target) > target.Health && Player.Distance(target) <= E.Range && Player.Mana >= E.ManaCost)
                {
                    E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                }

                if (UseIgniteKS && SkyLv_Taric.Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= SkyLv_Taric.Ignite.Range)
                {
                    SkyLv_Taric.Ignite.Cast(target, PacketCast);
                }
            }
        }

    }
}
