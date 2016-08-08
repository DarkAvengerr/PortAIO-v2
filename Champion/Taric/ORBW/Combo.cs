namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Combo
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


        static Combo()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Q Settings Combo").AddItem(new MenuItem("Taric.UseQCombo", "Use Self Q In Combo").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Q Settings Combo").AddItem(new MenuItem("Taric.MinimumStackSelfQCombo", "Minimum Q Stack To Use Self Q In Combo").SetValue(new Slider(2, 1, 3)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Q Settings Combo").AddItem(new MenuItem("Taric.MinimumHpPercentSelfQCombo", "Minimum Hp Percent To Use Self Q In Combo").SetValue(new Slider(100, 0, 100)));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Q Settings Combo").SubMenu("Use Q On Ally").AddItem(new MenuItem("Taric.UseQAlly", "Use Q On Ally").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Q Settings Combo").SubMenu("Use Q On Ally").AddItem(new MenuItem("Taric.UseQAllyMode", "Q On Ally Mode").SetValue(new StringList(new[] { "On Combo Key", "Auto Cast" }, 1)));

            foreach (var Ally in ObjectManager.Get<AIHeroClient>().Where(a => a.Team == Player.Team && a.NetworkId != Player.NetworkId))
            {
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Q Settings Combo").SubMenu("Use Q On Ally").SubMenu("Use Q On Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "MinimumHpQAlly", "Q Ally If Health Percent Under").SetValue(new Slider(60, 0, 100)));
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("Q Settings Combo").SubMenu("Use Q On Ally").SubMenu("Use Q On Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "MinimumStacksQAlly", "Minimum Q Stack To Use Q On Ally").SetValue(new Slider(2, 1, 3)));
            }

            //
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("W Settings Combo").AddItem(new MenuItem("Taric.UseWCombo", "Use Self W In Combo").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("W Settings Combo").AddItem(new MenuItem("Taric.UseWIncomingDamageCombo", "Use Self W Only On Incoming Damages In Combo").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("W Settings Combo").SubMenu("Use W On Ally").AddItem(new MenuItem("Taric.UseWAlly", "Use W On Ally").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("W Settings Combo").SubMenu("Use W On Ally").AddItem(new MenuItem("Taric.UseWAllyMode", "W On Ally Mode").SetValue(new StringList(new[] { "On Combo Key", "Auto Cast" }, 1)));

            foreach (var Ally in ObjectManager.Get<AIHeroClient>().Where(a => a.Team == Player.Team && a.NetworkId != Player.NetworkId))
            {
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("W Settings Combo").SubMenu("Use W On Ally").SubMenu("Use W On Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "MinimumHpWAlly", "W Ally If Health Percent Under").SetValue(new Slider(100, 0, 100)));
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("W Settings Combo").SubMenu("Use W On Ally").SubMenu("Use W On Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "IncomingDamageWAlly", "Only On Ally Incoming Damage", true).SetValue(true));
            }

            //
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("E Settings Combo").AddItem(new MenuItem("Taric.UseECombo", "Use Self E In Combo").SetValue(true));
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("E Settings Combo").SubMenu("Use E From Ally").AddItem(new MenuItem("Taric.UseEFromAlly", "Use E From Ally In Combo").SetValue(true));

            foreach (var Ally in ObjectManager.Get<AIHeroClient>().Where(a => a.Team == Player.Team && a.NetworkId != Player.NetworkId))
            {
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("E Settings Combo").SubMenu("Use E From Ally").SubMenu("Use E From Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "AlwaysComboFromAlly", "Always E From This Ally If Can't Cast MySelf", true).SetValue(true));
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("E Settings Combo").SubMenu("Use E From Ally").SubMenu("Use E From Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "AllyCCEComboFromAlly", "On Ally CC'ed", true).SetValue(true));
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("E Settings Combo").SubMenu("Use E From Ally").SubMenu("Use E From Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "TargetCCEComboFromAlly", "On Target CC'ed", true).SetValue(true));
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("E Settings Combo").SubMenu("Use E From Ally").SubMenu("Use E From Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "TargetInterruptEComboFromAlly", "Auto E From Ally On Interruptable Target", true).SetValue(true));
                SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("E Settings Combo").SubMenu("Use E From Ally").SubMenu("Use E From Ally TS").SubMenu(Ally.ChampionName).AddItem(new MenuItem(Ally.ChampionName + "TargetDashEPEComboFromAlly", "On Target Dash End Position", true).SetValue(true));
            }
            //
            SkyLv_Taric.Menu.SubMenu("Combo").AddItem(new MenuItem("Taric.UseTaricAAPassiveCombo", "Use All Taric AA Passive In Combo").SetValue(true));
            

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            ComboLogic();
        }

        public static void ComboLogic()
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();
            var useQ = SkyLv_Taric.Menu.Item("Taric.UseQCombo").GetValue<bool>();
            var useW = SkyLv_Taric.Menu.Item("Taric.UseWCombo").GetValue<bool>();
            var useE = SkyLv_Taric.Menu.Item("Taric.UseECombo").GetValue<bool>();
            var MinimumStackSelfQCombo = SkyLv_Taric.Menu.Item("Taric.MinimumStackSelfQCombo").GetValue<Slider>().Value;
            var MinimumHpPercentSelfQCombo = SkyLv_Taric.Menu.Item("Taric.MinimumHpPercentSelfQCombo").GetValue<Slider>().Value;

            if (SkyLv_Taric.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
               var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (target.LSIsValidTarget(E.Range))
                {
                    if (useE && E.LSIsReady() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveCombo").GetValue<bool>() || Player.LSDistance(target) > Orbwalking.GetRealAutoAttackRange(Player)))
                    {
                        if (Player.LSDistance(target) < E.Range)
                        {
                            E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                            return;
                        }
                    }

                    if (target.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                    {
                        if (!SkyLv_Taric.Menu.Item("Taric.UseWIncomingDamageCombo").GetValue<bool>() && useW && W.LSIsReady() && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveCombo").GetValue<bool>()) && (!E.LSIsReady() || !useE))
                        {
                            W.Cast(Player, PacketCast);
                            return;
                        }

                        if (useQ && Q.LSIsReady() && Q.Instance.Ammo >= MinimumStackSelfQCombo && Player.HealthPercent <= MinimumHpPercentSelfQCombo && (!CustomLib.HavePassiveAA() || !SkyLv_Taric.Menu.Item("Taric.UseTaricAAPassiveCombo").GetValue<bool>()) && (!E.LSIsReady() || !useE) && (Player.HealthPercent < 100 || (!W.LSIsReady() || !useW)))
                        {
                            Q.Cast(Player, PacketCast);
                            return;
                        }
                    }
                }

                #region Ally E
                if (SkyLv_Taric.Menu.Item("Taric.UseEFromAlly").GetValue<bool>() && E.LSIsReady() && Player.Mana >= E.ManaCost)
                {
                    foreach (var AllyHeroE in ObjectManager.Get<AIHeroClient>().Where(a => !a.IsMe && !a.IsDead && a.Team == ObjectManager.Player.Team && Player.LSDistance(a) < 1600 && (a.HasBuff("TaricWAllyBuff") || a.HasBuff("TaricW"))))
                    {
                        var Allytarget = ObjectManager.Get<AIHeroClient>().Where(t => !t.IsDead && t.Team != ObjectManager.Player.Team && AllyHeroE.LSDistance(t) < E.Range).FirstOrDefault();

                        if (Allytarget.LSIsValidTarget())
                        {
                            if (SkyLv_Taric.Menu.Item(AllyHeroE.ChampionName + "AllyCCEComboFromAlly", true).GetValue<bool>() && (AllyHeroE.IsCharmed || AllyHeroE.IsStunned || AllyHeroE.IsRooted || AllyHeroE.Spellbook.IsAutoAttacking))
                            {
                                E.Cast(Allytarget.ServerPosition, PacketCast);
                                return;
                            }

                            if (SkyLv_Taric.Menu.Item(AllyHeroE.ChampionName + "TargetCCEComboFromAlly", true).GetValue<bool>() && (Allytarget.IsCharmed || Allytarget.IsStunned || Allytarget.IsRooted || Allytarget.Spellbook.IsAutoAttacking))
                            {
                                E.Cast(Allytarget.ServerPosition, PacketCast);
                                return;
                            }

                            if (SkyLv_Taric.Menu.Item(AllyHeroE.ChampionName + "AlwaysComboFromAlly", true).GetValue<bool>())
                            {
                                E.Cast(Allytarget.ServerPosition, PacketCast);
                                return;
                            }
                        }
                    }
                }
                #endregion

                #region UseQAlly
                if (SkyLv_Taric.Menu.Item("Taric.UseQAlly").GetValue<bool>() && SkyLv_Taric.Menu.Item("Taric.UseQAllyMode").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (Q.LSIsReady() && Player.Mana >= Q.ManaCost)
                    {
                        foreach (var AllyHeroQ in HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && Player.LSDistance(x) < Q.Range &&
                        Q.Instance.Ammo >= SkyLv_Taric.Menu.Item(x.ChampionName + "MinimumStacksQAlly").GetValue<Slider>().Value &&
                        x.HealthPercent <= SkyLv_Taric.Menu.Item(x.ChampionName + "MinimumHpQAlly").GetValue<Slider>().Value))
                        {
                            if (AllyHeroQ.LSIsValidTarget())
                            {
                                Q.Cast(PacketCast);
                                return;
                            }
                        }
                    }
                }
                #endregion

                #region UseWAlly
                if (SkyLv_Taric.Menu.Item("Taric.UseWAlly").GetValue<bool>() && SkyLv_Taric.Menu.Item("Taric.UseWAllyMode").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (W.LSIsReady() && Player.Mana >= W.ManaCost)
                    {
                        var AllyHeroW = HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && Player.LSDistance(x) <= W.Range &&
                        !SkyLv_Taric.Menu.Item(x.ChampionName + "IncomingDamageWAlly", true).GetValue<bool>() &&
                        x.HealthPercent <= SkyLv_Taric.Menu.Item(x.ChampionName + "MinimumHpWAlly").GetValue<Slider>().Value).MinOrDefault(t => t.HealthPercent);

                        if (AllyHeroW.LSIsValidTarget())
                        {
                            W.Cast(AllyHeroW, PacketCast);
                            return;
                        }
                    }
                }
                #endregion

            }
        }
    }
}
