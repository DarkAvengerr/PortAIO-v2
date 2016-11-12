namespace ElTrundle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;

    internal enum Spells
    {
        Q,

        W,

        E,

        R
    }

    internal static class Trundle
    {
        #region Static Fields

        public static Orbwalking.Orbwalker Orbwalker;

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
                                                             {
                                                                 { Spells.Q, new Spell(SpellSlot.Q, 550f) },
                                                                 { Spells.W, new Spell(SpellSlot.W, 900f) },
                                                                 { Spells.E, new Spell(SpellSlot.E, 1000f) },
                                                                 { Spells.R, new Spell(SpellSlot.R, 700f) }
                                                             };

        private static SpellSlot ignite;

        private static Vector3 pillarPosition;

        #endregion

        #region Public Properties

        public static string ScriptVersion
        {
            get
            {
                return typeof(Trundle).Assembly.GetName().Version.ToString();
            }
        }

        #endregion

        #region Properties

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void OnLoad()
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Trundle")
            {
                return;
            }

            spells[Spells.E].SetSkillshot(0.5f, 188f, 1600f, false, SkillshotType.SkillshotCircle);
            ignite = Player.GetSpellSlot("summonerdot");

            Notifications.AddNotification(string.Format("ElTrundle by jQuery v{0}", ScriptVersion), 8000);
            Chat.Print(
                "[00:00] <font color='#f9eb0b'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery");
            ElTrundleMenu.Initialize();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        #endregion

        #region Methods

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.Distance(Player) > spells[Spells.E].Range || !IsActive("ElTrundle.Antigapcloser"))
            {
                return;
            }

            if (gapcloser.Sender.IsValidTarget(spells[Spells.E].Range))
            {
                if (spells[Spells.E].IsReady())
                {
                    spells[Spells.E].Cast(gapcloser.Sender);
                }
            }
        }

        private static Vector3 GetPillarPosition(AIHeroClient target)
        {
            pillarPosition = Player.Position;

            return V2E(pillarPosition, target.Position, target.Distance(pillarPosition) + 230).To3D();
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!IsActive("ElTrundle.Interrupter"))
            {
                return;
            }

            if (args.DangerLevel != Interrupter2.DangerLevel.High || sender.Distance(Player) > spells[Spells.E].Range)
            {
                return;
            }

            if (spells[Spells.E].CanCast(sender) && args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                spells[Spells.E].Cast(sender.Position);
            }
        }

        private static bool IsActive(string menuItem)
        {
            return ElTrundleMenu.Menu.Item(menuItem).GetValue<bool>();
        }

        private static void Jungleclear()
        {
            var minion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    spells[Spells.W].Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minion == null)
            {
                return;
            }

            UseItems(minion);

            if (Player.ManaPercent < ElTrundleMenu.Menu.Item("ElTrundle.JungleClear.Mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (IsActive("ElTrundle.JungleClear.Q") && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
                return;
            }

            if (IsActive("ElTrundle.JungleClear.W") && spells[Spells.W].IsReady()
                && minion.IsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void Laneclear()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.W].Range).FirstOrDefault();
            if (minion == null)
            {
                return;
            }

            UseItems(minion);

            if (Player.ManaPercent < ElTrundleMenu.Menu.Item("ElTrundle.LaneClear.Mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (IsActive("ElTrundle.LaneClear.Q") && spells[Spells.Q].IsReady()
                && minion.IsValidTarget(spells[Spells.Q].Range))
            {
                if (IsActive("ElTrundle.LaneClear.Q.Lasthit") && minion.Health < spells[Spells.Q].GetDamage(minion))
                {
                    spells[Spells.Q].Cast();
                    return;
                }

                spells[Spells.Q].Cast();
                return;
            }

            if (IsActive("ElTrundle.LaneClear.W") && spells[Spells.W].IsReady()
                && minion.IsValidTarget(700))
            {
                spells[Spells.W].Cast(minion.Position);
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (IsActive("ElTrundle.Combo.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            UseItems(target);

            if (IsActive("ElTrundle.Combo.W") && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }

            if (IsActive("ElTrundle.Combo.Q") && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
            }

            if (spells[Spells.R].IsReady() && IsActive("ElTrundle.Combo.R"))
            {
                foreach (var hero in ObjectManager.Get<AIHeroClient>())
                {
                    if (hero.IsEnemy)
                    {
                        var getEnemies = ElTrundleMenu.Menu.Item("ElTrundle.R.On" + hero.CharData.BaseSkinName);
                        if (getEnemies != null && getEnemies.GetValue<bool>())
                        {
                            spells[Spells.R].Cast(hero);
                        }

                        if (getEnemies != null && !getEnemies.GetValue<bool>() && Player.CountEnemiesInRange(1500) == 1)
                        {
                            spells[Spells.R].Cast(hero);
                        }
                    }
                }
            }

            if (Player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health
                && IsActive("ElTrundle.Combo.Ignite"))
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var newTarget = TargetSelector.GetTarget(spells[Spells.E].Range + 200, TargetSelector.DamageType.Physical);
            var drawOff = ElTrundleMenu.Menu.Item("ElTrundle.Draw.off").GetValue<bool>();
            var drawQ = ElTrundleMenu.Menu.Item("ElTrundle.Draw.Q").GetValue<Circle>();
            var drawW = ElTrundleMenu.Menu.Item("ElTrundle.Draw.W").GetValue<Circle>();
            var drawE = ElTrundleMenu.Menu.Item("ElTrundle.Draw.E").GetValue<Circle>();
            var drawR = ElTrundleMenu.Menu.Item("ElTrundle.Draw.R").GetValue<Circle>();

            if (newTarget != null && newTarget.IsHPBarRendered && newTarget.IsValidTarget() && !newTarget.IsDead
                && Player.Distance(newTarget) < 3000)
            {
                Drawing.DrawCircle(GetPillarPosition(newTarget), 188, Color.DeepPink);
            }

            if (drawOff)
            {
                return;
            }

            if (drawQ.Active)
            {
                if (spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawW.Active)
            {
                if (spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.White);
                }
            }

            if (drawE.Active)
            {
                if (spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR.Active)
            {
                if (spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White);
                }
            }
        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (Player.ManaPercent < ElTrundleMenu.Menu.Item("ElTrundle.Harass.Mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (IsActive("ElTrundle.Harass.Q") && target.IsValidTarget(spells[Spells.Q].Range))
            {
                spells[Spells.Q].Cast();
            }

            if (IsActive("ElTrundle.Harass.E") && spells[Spells.E].IsReady()
                && target.IsValidTarget(spells[Spells.E].Range))
            {
                spells[Spells.E].Cast(GetPillarPosition(target));
            }

            if (IsActive("ElTrundle.Harass.W") && spells[Spells.W].IsReady()
                && target.IsValidTarget(spells[Spells.W].Range))
            {
                spells[Spells.W].Cast(target.Position);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling() || MenuGUI.IsChatOpen || Shop.IsOpen)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    Jungleclear();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
            }
        }

        private static void UseItems(Obj_AI_Base target)
        {
            if (IsActive("ElTrundle.Items.Hydra"))
            {
                if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady()
                    && ItemData.Ravenous_Hydra_Melee_Only.Range < Player.Distance(target))
                {
                    ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
                }
                if (ItemData.Tiamat_Melee_Only.GetItem().IsReady()
                    && ItemData.Tiamat_Melee_Only.Range < Player.Distance(target))
                {
                    ItemData.Tiamat_Melee_Only.GetItem().Cast();
                }
            }

            if (IsActive("ElTrundle.Items.Titanic"))
            {
                if (Items.HasItem(3748) && Player.Distance(target) < 400)
                {
                    Items.UseItem(3748, target);
                }
            }

            if (IsActive("ElTrundle.Items.Blade"))
            {
                if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady()
                    && ItemData.Blade_of_the_Ruined_King.Range < Player.Distance(target))
                {
                    ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
                }

                if (ItemData.Bilgewater_Cutlass.GetItem().IsReady()
                    && ItemData.Bilgewater_Cutlass.Range < Player.Distance(target))
                {
                    ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (IsActive("ElTrundle.Items.Youmuu"))
                {
                    if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()
                        && Orbwalking.GetRealAutoAttackRange(Player) < Player.Distance(target))
                    {
                        ItemData.Youmuus_Ghostblade.GetItem().Cast();
                    }
                }
            }
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance * Vector3.Normalize(direction - from).To2D();
        }

        #endregion
    }
}