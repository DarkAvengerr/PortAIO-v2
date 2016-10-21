using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class JungleClear
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



        static JungleClear()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("Q Settings JungleClear").AddItem(new MenuItem("Jax.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("Q Settings JungleClear").AddItem(new MenuItem("Jax.QMiniManaJungleClear", "Minimum Mana To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("W Settings JungleClear").AddItem(new MenuItem("Jax.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("W Settings JungleClear").AddItem(new MenuItem("Jax.WMiniManaJungleClear", "Minimum Mana To Use W In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("E Settings JungleClear").AddItem(new MenuItem("Jax.UseEJungleClear", "Use E In JungleClear").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("JungleClear").SubMenu("E Settings JungleClear").AddItem(new MenuItem("Jax.EMiniManaJungleClear", "Minimum Mana To Use E In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Jax.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Jax.SafeJungleClear", "Dont Use Spell In Jungle Clear If Enemy in Dangerous Range").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Jax.SpellOnlyBigMonster", "Use Spell Only On Big Monster").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleClearLogic();
        }

        public static void JungleClearLogic()
        {
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();
            var useQ = SkyLv_Jax.Menu.Item("Jax.UseQJungleClear").GetValue<bool>();
            var useW = SkyLv_Jax.Menu.Item("Jax.UseWJungleClear").GetValue<bool>();
            var useE = SkyLv_Jax.Menu.Item("Jax.UseEJungleClear").GetValue<bool>();

            var MiniManaQ = SkyLv_Jax.Menu.Item("Jax.QMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaW = SkyLv_Jax.Menu.Item("Jax.WMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaE = SkyLv_Jax.Menu.Item("Jax.EMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionN = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (MinionN.IsValidTarget() && SkyLv_Jax.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_Jax.Menu.Item("Jax.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (!SkyLv_Jax.Menu.Item("Jax.AfterAttackModeJungleClear").GetValue<bool>())
                {
                    if (useQ && Q.IsReady() && Player.ManaPercent > MiniManaQ)
                    {
                        if (SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>())
                        {
                            foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Jax.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                            {
                                if (target.IsValidTarget())
                                    Q.CastOnUnit(target, PacketCast);
                            }
                        }
                        else if (!SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget())
                            Q.CastOnUnit(MinionN, PacketCast);
                    }

                    if (useE && E.IsReady() && Player.ManaPercent > MiniManaE && !CustomLib.iSJaxEActive())
                    {
                        if (SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>())
                        {
                            foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Jax.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                            {
                                if (target.IsValidTarget())
                                    E.Cast(PacketCast);
                            }
                        }
                        else if (!SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget())
                            E.Cast(PacketCast);
                    }

                    if (useE && CustomLib.iSJaxEActive() && MinionN.IsValidTarget(E.Range))
                    {
                        E.Cast(PacketCast);
                    }

                    if (useW && W.IsReady() && Player.Mana >= W.ManaCost)
                    {
                        if (SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>())
                        {
                            foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Jax.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                            {
                                if (target.IsValidTarget(W.Range) && Orbwalking.CanAttack())
                                    W.Cast(PacketCast);
                            }
                        }
                        else if (!SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget(W.Range) && Orbwalking.CanAttack())
                            W.Cast(PacketCast);
                    }
                }

                if (SkyLv_Jax.Menu.Item("Jax.AfterAttackModeJungleClear").GetValue<bool>())
                {
                    if (useQ && Q.IsReady() && Player.ManaPercent > MiniManaQ)
                    {
                        if (SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>())
                        {
                            foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Jax.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                            {
                                if (target.IsValidTarget() && target.Distance(Player) > Orbwalking.GetRealAutoAttackRange(Player))
                                    Q.CastOnUnit(target, PacketCast);
                            }
                        }
                        else if (!SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget() && MinionN.Distance(Player) > Orbwalking.GetRealAutoAttackRange(Player))
                            Q.CastOnUnit(MinionN, PacketCast);
                    }
                }
            }
        }
    }
}
