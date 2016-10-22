using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
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
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").AddSubMenu(new Menu("Jungle KS Mode", "Jungle KS Mode"));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddItem(new MenuItem("AurelionSol.JungleKS", "Jungle KS").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddItem(new MenuItem("AurelionSol.JungleKSPacketCast", "Jungle KS PacketCast").SetValue(true));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            SkyLv_AurelionSol.Menu.SubMenu("JungleClear").SubMenu("Jungle KS Mode").SubMenu("Advanced Settings").AddItem(new MenuItem("AurelionSol.UseQJungleKS", "KS With Q").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleKS();
        }

        public static void JungleKS()
        {
            if (SkyLv_AurelionSol.Menu.Item("AurelionSol.JungleKS").GetValue<bool>())
            {
                var useQKS = SkyLv_AurelionSol.Menu.Item("AurelionSol.UseQJungleKS").GetValue<bool>();
                var PacketCast = SkyLv_AurelionSol.Menu.Item("AurelionSol.JungleKSPacketCast").GetValue<bool>();

                foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => Monsters.Contains(target.BaseSkinName)))
                {
                    if (useQKS && Q.IsReady() && target.Health < CustomLib.QDamage(target) && Player.Distance(target) < Q.Range && !target.IsDead && target.IsValidTarget())
                    {
                        var prediction = Q.GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            Q.Cast(prediction.CastPosition, PacketCast);
                        }
                    }
                }
            }
        }
    }
}
