using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
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


        static JungleSteal()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddSubMenu(new Menu("Jungle KS Mode", "Jungle KS Mode"));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddItem(new MenuItem("Tristana.JungleKS", "Jungle KS").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddItem(new MenuItem("Tristana.JungleKSPacketCast", "Jungle KS PacketCast").SetValue(false));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddSubMenu(new Menu("Spell Settings", "Spell Settings"));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Tristana.UseEJungleKS", "KS With E").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Spell Settings").AddItem(new MenuItem("Tristana.UseRJungleKS", "KS With R").SetValue(false));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Tristana.Menu.Item("Tristana.JungleKS").GetValue<bool>())
            {
                JungleKS();
            }
        }

        #region KillSteal
        public static void JungleKS()
        {
            var UseEJungleKS = SkyLv_Tristana.Menu.Item("Tristana.UseEJungleKS").GetValue<bool>();
            var UseRJungleKS = SkyLv_Tristana.Menu.Item("Tristana.UseRJungleKS").GetValue<bool>();
            var JungleKSPacketCast = SkyLv_Tristana.Menu.Item("Tristana.JungleKSPacketCast").GetValue<bool>();

            foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Tristana.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
            {
                if (UseEJungleKS && E.GetDamage(target) > target.Health)
                {
                    E.Cast(target, JungleKSPacketCast);
                }

                if (UseRJungleKS && R.GetDamage(target) > target.Health)
                {
                    R.Cast(target, JungleKSPacketCast);
                }
            }
        }
        #endregion
    }
}
