using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;


    internal class AfterAttack
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Tristana.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Tristana.Q;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Tristana.E;
            }
        }
        #endregion

        static AfterAttack()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Combo").AddItem(new MenuItem("Tristana.AfterAttackModeCombo", "Cancel Spell With AA In Combo").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Harass").AddItem(new MenuItem("Tristana.AfterAttackModeHarass", "Cancel Spell With AA In Harass").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("LaneClear").AddItem(new MenuItem("Tristana.AfterAttackModeLaneClear", "Cancel Spell With AA In LaneClear").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Tristana.AfterAttackModeJungleClear", "Cancel Spell With AA In JungleClear").SetValue(true));

            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        public static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            #region Combo
            if (SkyLv_Tristana.Menu.Item("Tristana.AfterAttackModeCombo").GetValue<bool>() && SkyLv_Tristana.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastCombo").GetValue<bool>();

                if (target is AIHeroClient)
                {
                    var t = (AIHeroClient)target;
                    if (t.IsValidTarget())
                    {
                        if (SkyLv_Tristana.Menu.Item("Tristana.UseQCombo").GetValue<bool>() && Q.IsReady() && t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                            Q.Cast(PacketCast);
                        if (SkyLv_Tristana.Menu.Item("Tristana.UseECombo").GetValue<bool>() && E.IsReady() && t.IsValidTarget(E.Range))
                            E.CastOnUnit(t, PacketCast);
                    }
                }
            }
            #endregion

            #region Harass
            if (SkyLv_Tristana.Menu.Item("Tristana.AfterAttackModeHarass").GetValue<bool>() && (SkyLv_Tristana.Menu.Item("Tristana.HarassActive").GetValue<KeyBind>().Active || SkyLv_Tristana.Menu.Item("Tristana.HarassActive2").GetValue<KeyBind>().Active || SkyLv_Tristana.Menu.Item("Tristana.HarassActiveT").GetValue<KeyBind>().Active))
            {

                var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastHarass").GetValue<bool>();

                var useQ = SkyLv_Tristana.Menu.Item("Tristana.UseQHarass").GetValue<bool>();
                var MiniManaQ = SkyLv_Tristana.Menu.Item("Tristana.QMiniManaHarass").GetValue<Slider>().Value;
                var useE = SkyLv_Tristana.Menu.Item("Tristana.UseEHarass").GetValue<bool>();
                var MiniManaE = SkyLv_Tristana.Menu.Item("Tristana.EMiniManaHarass").GetValue<Slider>().Value;

                if (target is AIHeroClient)
                {
                    var t = (AIHeroClient)target;
                    if (t.IsValidTarget())
                    {
                        if (useE && E.IsReady() && target.IsValidTarget(E.Range) && Player.ManaPercent > MiniManaE)
                            E.CastOnUnit(t, PacketCast);

                        if (useQ && Q.IsReady() && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)) && Player.ManaPercent > MiniManaQ)
                            Q.Cast(PacketCast);
                    }
                }
            }
            #endregion

            #region LaneClear
            if (SkyLv_Tristana.Menu.Item("Tristana.AfterAttackModeLaneClear").GetValue<bool>() && SkyLv_Tristana.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastLaneClear").GetValue<bool>();

                var useQ = SkyLv_Tristana.Menu.Item("Tristana.UseQLaneClear").GetValue<bool>();
                var useE = SkyLv_Tristana.Menu.Item("Tristana.UseELaneClear").GetValue<bool>();

                var MiniManaQ = SkyLv_Tristana.Menu.Item("Tristana.QMiniManaLaneClear").GetValue<Slider>().Value;
                var MiniManaE = SkyLv_Tristana.Menu.Item("Tristana.EMiniManaLaneClear").GetValue<Slider>().Value;

                var MiniCountQ = SkyLv_Tristana.Menu.Item("Tristana.QLaneClearCount").GetValue<Slider>().Value;
                var MiniCountE = SkyLv_Tristana.Menu.Item("Tristana.ELaneClearCount").GetValue<Slider>().Value;


                    var Minion = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy).FirstOrDefault();

                    if (Minion.IsValidTarget() && SkyLv_Tristana.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                    {
                        if (SkyLv_Tristana.Menu.Item("Tristana.SafeLaneClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                        if (useE && Player.ManaPercent > MiniManaE && E.IsReady())
                        {
                            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.Team != ObjectManager.Player.Team && !m.IsDead && Player.Distance(m) <= E.Range))
                            {
                                if (CustomLib.EnemyMinionInMinionRange(minion, 300) >= MiniCountE)
                                {
                                    if (Player.GetAutoAttackDamage(minion) * 2 > minion.Health && SkyLv_Tristana.Menu.Item("Tristana.UseELaneClearOnlyLastHitable").GetValue<bool>())
                                        E.CastOnUnit(minion, PacketCast);
                                    if (!SkyLv_Tristana.Menu.Item("Tristana.UseELaneClearOnlyLastHitable").GetValue<bool>())
                                        E.CastOnUnit(minion, PacketCast);
                                }
                            }
                        }

                        if (useQ && Player.ManaPercent > MiniManaQ && Q.IsReady())
                        {
                            if (CustomLib.EnemyMinionInPlayerRange(Orbwalking.GetRealAutoAttackRange(Player)) >= MiniCountQ)
                            {
                                Q.Cast(PacketCast);
                            }
                        }
                    }
                
            }
            #endregion

            #region JungleClear
            if (SkyLv_Tristana.Menu.Item("Tristana.AfterAttackModeJungleClear").GetValue<bool>() && SkyLv_Tristana.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastJungleClear").GetValue<bool>();
                var useQ = SkyLv_Tristana.Menu.Item("Tristana.UseQJungleClear").GetValue<bool>();
                var useE = SkyLv_Tristana.Menu.Item("Tristana.UseEJungleClear").GetValue<bool>();

                var MiniManaQ = SkyLv_Tristana.Menu.Item("Tristana.QMiniManaJungleClear").GetValue<Slider>().Value;
                var MiniManaE = SkyLv_Tristana.Menu.Item("Tristana.EMiniManaJungleClear").GetValue<Slider>().Value;

                    var MinionN = MinionManager.GetMinions(E.Range + 200, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

                    if (MinionN.IsValidTarget() && SkyLv_Tristana.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                    {
                        if (SkyLv_Tristana.Menu.Item("Tristana.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                        if (useE && Player.ManaPercent > MiniManaE && E.IsReady())
                        {
                            if (SkyLv_Tristana.Menu.Item("Tristana.SpellOnlyBigMonster").GetValue<bool>())
                            {
                                foreach (var monster in ObjectManager.Get<Obj_AI_Base>().Where(monster => SkyLv_Tristana.Monsters.Contains(monster.BaseSkinName) && !monster.IsDead))
                                {
                                    E.CastOnUnit(monster, PacketCast);
                                }
                            }
                            else if (!SkyLv_Tristana.Menu.Item("Tristana.SpellOnlyBigMonster").GetValue<bool>())
                                E.CastOnUnit(MinionN, PacketCast);
                        }

                        if (useQ && Q.IsReady() && Player.ManaPercent > MiniManaQ)
                        {
                            if (SkyLv_Tristana.Menu.Item("Tristana.SpellOnlyBigMonster").GetValue<bool>())
                            {
                                foreach (var monster in ObjectManager.Get<Obj_AI_Base>().Where(monster => SkyLv_Tristana.Monsters.Contains(monster.BaseSkinName) && !monster.IsDead))
                                {
                                    Q.Cast(PacketCast);
                                }
                            }
                            else if (!SkyLv_Tristana.Menu.Item("Tristana.SpellOnlyBigMonster").GetValue<bool>())
                                Q.Cast(PacketCast);
                        }
                    }
                
            }
            #endregion
        }
    }
}
