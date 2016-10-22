namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class JungleClear
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



        static JungleClear()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("Q Settings JungleClear").AddItem(new MenuItem("Taric.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("Q Settings JungleClear").AddItem(new MenuItem("Taric.QMiniManaJungleClear", "Minimum Mana To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("W Settings JungleClear").AddItem(new MenuItem("Taric.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("W Settings JungleClear").AddItem(new MenuItem("Taric.WMiniManaJungleClear", "Minimum Mana To Use W In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("E Settings JungleClear").AddItem(new MenuItem("Taric.UseEJungleClear", "Use E In JungleClear").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("JungleClear").SubMenu("E Settings JungleClear").AddItem(new MenuItem("Taric.EMiniManaJungleClear", "Minimum Mana To Use E In JungleClear").SetValue(new Slider(0, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Taric.SafeJungleClear", "Dont Use Spell In Jungle Clear If Enemy in Dangerous Range").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Taric.SpellOnlyBigMonster", "Use Spell Only On Big Monster").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Taric.UseTaricAAPassiveJungleClear", "Use All Taric AA Passive In JungleClear").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleClearLogic();
        }

        public static void JungleClearLogic()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();
            var useQ = SkyLv_Taric.Menu.Item("Taric.UseQJungleClear").GetValue<bool>();
            var useW = SkyLv_Taric.Menu.Item("Taric.UseWJungleClear").GetValue<bool>();
            var useE = SkyLv_Taric.Menu.Item("Taric.UseEJungleClear").GetValue<bool>();

            var MiniManaQ = SkyLv_Taric.Menu.Item("Taric.QMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaW = SkyLv_Taric.Menu.Item("Taric.WMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaE = SkyLv_Taric.Menu.Item("Taric.EMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionN = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (MinionN.IsValidTarget() && SkyLv_Taric.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_Taric.Menu.Item("Taric.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (useE && E.IsReady() && Player.ManaPercent > MiniManaE)
                {
                    if (SkyLv_Taric.Menu.Item("Taric.SpellOnlyBigMonster").GetValue<bool>())
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Taric.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                        {
                            if (target.IsValidTarget() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveJungleClear").GetValue<bool>()))
                                E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                        }
                    }
                    else if(!SkyLv_Taric.Menu.Item("Taric.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveJungleClear").GetValue<bool>()))
                        E.CastIfHitchanceEquals(MinionN, HitChance.VeryHigh, PacketCast);
                }

                if (useW && W.IsReady() && Player.ManaPercent > MiniManaW && (!E.IsReady() || !useE))
                {
                    if (SkyLv_Taric.Menu.Item("Taric.SpellOnlyBigMonster").GetValue<bool>())
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Taric.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                        {
                            if (target.IsValidTarget() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveJungleClear").GetValue<bool>()))
                                W.Cast(Player, PacketCast);
                        }
                    }
                    else if (!SkyLv_Taric.Menu.Item("Taric.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveJungleClear").GetValue<bool>()))
                        W.Cast(Player, PacketCast);
                }

                if (useQ && Q.IsReady() && Player.ManaPercent > MiniManaQ && (!E.IsReady() || !useE) && (!W.IsReady() || !useW))
                {
                    if (SkyLv_Taric.Menu.Item("Taric.SpellOnlyBigMonster").GetValue<bool>())
                    {
                        foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Taric.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                        {
                            if (target.IsValidTarget() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveJungleClear").GetValue<bool>()))
                                Q.Cast(Player, PacketCast);
                        }
                    }
                    else if (!SkyLv_Taric.Menu.Item("Taric.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveJungleClear").GetValue<bool>()))
                        Q.Cast(Player, PacketCast);
                }
            }
        }
    }
}
