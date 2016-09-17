using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Jax
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;

    internal class AfterAttack
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

        static AfterAttack()
        {
            //Menu
            SkyLv_Jax.Menu.SubMenu("Combo").AddItem(new MenuItem("Jax.AfterAttackModeCombo", "Cancel AA With W In Combo").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("Harass").AddItem(new MenuItem("Jax.AfterAttackModeHarass", "Cancel AA With W In Harass").SetValue(true));
            SkyLv_Jax.Menu.SubMenu("JungleClear").AddItem(new MenuItem("Jax.AfterAttackModeJungleClear", "Cancel AA With Spell In JungleClear").SetValue(true));

            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        public static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var PacketCast = SkyLv_Jax.Menu.Item("Jax.UsePacketCast").GetValue<bool>();

            var thero = target as AIHeroClient;

            if (!unit.IsMe)
            {
                return;
            }

            #region Combo
            if (thero != null && SkyLv_Jax.Menu.Item("Jax.AfterAttackModeCombo").GetValue<bool>() && SkyLv_Jax.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var UseWCombo = SkyLv_Jax.Menu.Item("Jax.UseWCombo").GetValue<bool>();

                if (thero.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                {
                    if (UseWCombo && W.IsReady())
                        W.Cast(PacketCast);
                }
                else return;

            }
            #endregion

            #region Harass
            if (thero != null && SkyLv_Jax.Menu.Item("Jax.AfterAttackModeHarass").GetValue<bool>() && (SkyLv_Jax.Menu.Item("Jax.HarassActive").GetValue<KeyBind>().Active || SkyLv_Jax.Menu.Item("Jax.HarassActiveT").GetValue<KeyBind>().Active))
            {
                var UseWHarass = SkyLv_Jax.Menu.Item("Jax.UseWHarass").GetValue<bool>();

                var WMiniManaHarass = SkyLv_Jax.Menu.Item("Jax.WMiniManaHarass").GetValue<Slider>().Value;

                if (thero.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                {
                    if (UseWHarass && W.IsReady() && Player.Mana >= W.ManaCost)
                        W.Cast(PacketCast);
                }
                else return;

            }
            #endregion

            #region JungleClear
            var useQ = SkyLv_Jax.Menu.Item("Jax.UseQJungleClear").GetValue<bool>();
            var useW = SkyLv_Jax.Menu.Item("Jax.UseWJungleClear").GetValue<bool>();
            var useE = SkyLv_Jax.Menu.Item("Jax.UseEJungleClear").GetValue<bool>();

            var MiniManaQ = SkyLv_Jax.Menu.Item("Jax.QMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaW = SkyLv_Jax.Menu.Item("Jax.WMiniManaJungleClear").GetValue<Slider>().Value;
            var MiniManaE = SkyLv_Jax.Menu.Item("Jax.EMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionN = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (target != null && SkyLv_Jax.Menu.Item("Jax.AfterAttackModeJungleClear").GetValue<bool>() && target.NetworkId == MinionN.NetworkId && SkyLv_Jax.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (SkyLv_Jax.Menu.Item("Jax.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                if (useQ && Q.IsReady() && Player.ManaPercent > MiniManaQ)
                {
                    if (SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>())
                    {
                        foreach (var minion in ObjectManager.Get<Obj_AI_Base>().Where(minion => SkyLv_Jax.Monsters.Contains(minion.BaseSkinName) && !minion.IsDead))
                        {
                            if (minion.IsValidTarget() && minion.Health > Player.GetAutoAttackDamage(minion) * 2)
                                Q.CastOnUnit(minion, PacketCast);
                        }
                    }
                    else if (!SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget() && MinionN.Health > Player.GetAutoAttackDamage(MinionN) * 2)
                        Q.CastOnUnit(MinionN, PacketCast);
                }

                if (useE && E.IsReady() && Player.ManaPercent > MiniManaE && !CustomLib.iSJaxEActive())
                {
                    if (SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>())
                    {
                        foreach (var minion in ObjectManager.Get<Obj_AI_Base>().Where(minion => SkyLv_Jax.Monsters.Contains(minion.BaseSkinName) && !minion.IsDead))
                        {
                            if (minion.IsValidTarget() && minion.Health > Player.GetAutoAttackDamage(minion) * 4)
                                E.Cast(PacketCast);
                        }
                    }
                    else if (!SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget() && MinionN.Health > Player.GetAutoAttackDamage(MinionN) * 4)
                        E.Cast(PacketCast);
                }

                if (useE && CustomLib.iSJaxEActive() && MinionN.IsValidTarget(E.Range) && MinionN.Health > Player.GetAutoAttackDamage(MinionN) * 2)
                {
                    E.Cast(PacketCast);
                }

                if (useW && W.IsReady() && Player.Mana >= W.ManaCost)
                {
                    if (SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>())
                    {
                        foreach (var minion in ObjectManager.Get<Obj_AI_Base>().Where(minion => SkyLv_Jax.Monsters.Contains(minion.BaseSkinName) && !minion.IsDead))
                        {
                            if (minion.IsValidTarget(W.Range) && minion.Health > Player.GetAutoAttackDamage(minion) * 1.5)
                                W.Cast(PacketCast);
                        }
                    }
                    else if (!SkyLv_Jax.Menu.Item("Jax.SpellOnlyBigMonster").GetValue<bool>() && MinionN.IsValidTarget(W.Range) && MinionN.Health > Player.GetAutoAttackDamage(MinionN))
                        W.Cast(PacketCast);
                }
            }
            #endregion



        }
    }
}
