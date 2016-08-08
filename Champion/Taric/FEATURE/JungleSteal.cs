namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class JungleSteal
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

        static JungleSteal()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("JungleClear").AddSubMenu(new Menu("Jungle KS Mode", "Jungle KS Mode"));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddItem(new MenuItem("Taric.JungleKS", "Jungle KS").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.UseAAJungleKS", "KS With AA").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.UseEJungleKS", "KS With E").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleKS();
        }

        public static void JungleKS()
        {
            if (SkyLv_Taric.Menu.Item("Taric.JungleKS").GetValue<bool>())
            {
                var UseAAJungleKS = SkyLv_Taric.Menu.Item("Taric.UseAAJungleKS").GetValue<bool>();
                var UseEJungleKS = SkyLv_Taric.Menu.Item("Taric.UseEJungleKS").GetValue<bool>();
                var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();

                if (Player.LSIsRecalling()) return;

                foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Taric.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                {
                    if (UseAAJungleKS && Orbwalking.CanAttack() && Player.LSGetAutoAttackDamage(target) > target.Health && target.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }

                    if (UseEJungleKS && E.GetDamage(target) > target.Health && Player.LSDistance(target) <= E.Range && Player.Mana >= E.ManaCost)
                    {
                        E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                    }
                }
            }
        }
    }
}
