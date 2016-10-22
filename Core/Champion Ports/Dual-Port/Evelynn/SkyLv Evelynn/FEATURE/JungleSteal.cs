using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
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
        #endregion

        private static readonly string[] Monsters =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", "TT_NGolem5",
            "TT_NGolem2", "TT_NWolf6", "TT_NWolf3","TT_NWraith1", "TT_Spider"
        };


        static JungleSteal()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").AddSubMenu(new Menu("Jungle KS Mode", "Jungle KS Mode"));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddItem(new MenuItem("Evelynn.JungleKS", "Jungle KS").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddItem(new MenuItem("Evelynn.JungleKSPacketCast", "Jungle KS PacketCast").SetValue(false));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.UseQJungleKS", "KS With Q").SetValue(true));
            SkyLv_Evelynn.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.UseEJungleKS", "KS With E").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleKS();
        }

        public static void JungleKS()
        {
            if (SkyLv_Evelynn.Menu.Item("Evelynn.JungleKS").GetValue<bool>())
            {
                var useQKS = SkyLv_Evelynn.Menu.Item("Evelynn.UseQJungleKS").GetValue<bool>();
                var useEKS = SkyLv_Evelynn.Menu.Item("Evelynn.UseEJungleKS").GetValue<bool>();
                var PacketCast = SkyLv_Evelynn.Menu.Item("Evelynn.JungleKSPacketCast").GetValue<bool>();

                foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                {
                    if (useQKS && Q.GetDamage(target) > target.Health)
                    {
                        Q.Cast(target, PacketCast);
                    }

                    if (useEKS && E.GetDamage(target) > target.Health)
                    {
                        E.Cast(target, PacketCast);
                    }
                }
            }
        }
    }
}
