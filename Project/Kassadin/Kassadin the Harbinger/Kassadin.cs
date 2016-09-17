using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Kassadin_the_Harbinger.Handlers;
using Kassadin_the_Harbinger.Utility;
using SharpDX;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.Common.Menu;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kassadin_the_Harbinger
{


    internal class Kassadin
    {
        #region Static Fields

        public static Menu Menu;
        public static ManaManager ManaManager;
        public static Orbwalking.Orbwalker Orbwalker;

        private const string ChampionName = "Kassadin";
        private static AIHeroClient player;

        private static bool ECanCast { get { return player.HasBuff("forcepulsecancast"); } }
        private static int EBuffCount { get { return player.Buffs.Count(x => x.Name == "forcepulsecounter"); } }

        #endregion  

        #region OnLoad

        public static void OnLoad(EventArgs args)
        {
            try
            {
                player = ObjectManager.Player;

                if (player.ChampionName != ChampionName)
                {
                    return;
                }

                ShowNotification("Kassadin the Harbinger", Color.DarkOrange, 10000);
                ShowNotification("by blacky - Loaded", Color.DarkOrange, 10000);

                DamageIndicator.Initialize(GetComboDamage);
                DamageIndicator.Enabled = true;
                DamageIndicator.DrawingColor = Color.GreenYellow;

                ManaManager = new ManaManager();
                MenuGenerator.Load();
                SkillsHandler.Load();
                AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
                Orbwalking.BeforeAttack += OnBeforeAttack;
                Drawing.OnDraw += DrawHandler.OnDraw;
                Game.OnUpdate += OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region OnBeforeAttack

        private static void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!args.Unit.IsMe)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (SkillsHandler.Spells[SpellSlot.W].IsReady() && GetMenuValue<bool>("kassadin.combo.useW"))
                    {
                        SkillsHandler.Spells[SpellSlot.W].Cast();
                    }
                    break;
            }
        }

        #endregion  

        #region OnEnemyGapcloser

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("kassadin.misc.gapcloseE").GetValue<bool>() && SkillsHandler.Spells[SpellSlot.E].IsReady()
                && SkillsHandler.Spells[SpellSlot.E].IsInRange(gapcloser.Sender) && ECanCast)
            {
                SkillsHandler.Spells[SpellSlot.E].Cast(gapcloser.Sender);
            }
        }

        #endregion

        #region OnInterruptableTarget

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("kassadin.misc.interruptQ").GetValue<bool>())
            {
                if (args.DangerLevel == Interrupter2.DangerLevel.High
                    && sender.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range)
                    && SkillsHandler.Spells[SpellSlot.Q].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.Q].CastOnUnit(sender);
                }
            }

        }

        #endregion

        #region OnUpdate

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnWaveclear();
                    break;
            }

            if (GetMenuValue<KeyBind>("kassadin.flee.activated").Active)
            {
                OnFlee(Game.CursorPos);
            }
        }

        #endregion

        #region Combo

        private static void OnCombo()
        {
            var comboTarget = TargetSelector.GetTarget(SkillsHandler.Spells[SpellSlot.Q].Range,
                TargetSelector.DamageType.Magical);

            if (comboTarget.InFountain() || comboTarget.IsInvulnerable || comboTarget.IsZombie)
            {
                return;
            }

            if (comboTarget.IsValidTarget())
            {
                if (GetMenuValue<bool>("kassadin.combo.useQ")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range)
                    && SkillsHandler.Spells[SpellSlot.Q].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.Q].CastOnUnit(comboTarget);
                }

                if (GetMenuValue<bool>("kassadin.combo.useE")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                    && SkillsHandler.Spells[SpellSlot.E].IsReady() && ECanCast && !player.IsDashing())
                {
                    SkillsHandler.Spells[SpellSlot.E].CastIfHitchanceEquals(comboTarget, HitChance.High);
                }

                if (GetMenuValue<bool>("kassadin.combo.useR")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.R].Range)
                    && SkillsHandler.Spells[SpellSlot.R].IsReady())
                {
                    if (comboTarget.UnderTurret(true) && !GetMenuValue<bool>("kassadin.combo.options.turretDive"))
                    {
                        return;
                    }

                    if (Vector3.Distance(player.ServerPosition, comboTarget.ServerPosition)
                        <= SkillsHandler.Spells[SpellSlot.R].Range
                        && (comboTarget.ServerPosition.CountEnemiesInRange(SkillsHandler.Spells[SpellSlot.R].Range))
                        < GetMenuValue<Slider>("kassadin.combo.options.dontR").Value)
                    {
                        if (SkillsHandler.Spells[SpellSlot.R].Instance.SData.Mana < 300)
                        {
                            switch (GetMenuValue<StringList>("kassadin.combo.options.rMode").SelectedIndex)
                            {
                                case 0:
                                    SkillsHandler.Spells[SpellSlot.R].Cast(comboTarget.ServerPosition);
                                    break;
                                case 1:
                                    if (ECanCast || EBuffCount >= 3)
                                    {
                                        SkillsHandler.Spells[SpellSlot.R].Cast(comboTarget.ServerPosition);
                                    }
                                    break;
                            }
                        }

                        if (SkillsHandler.Spells[SpellSlot.R].Instance.SData.Mana > 300 && SkillsHandler.Spells[SpellSlot.R].IsKillable(comboTarget) && player.Mana > SkillsHandler.Spells[SpellSlot.R].Instance.SData.Mana)
                        {
                            SkillsHandler.Spells[SpellSlot.R].Cast(comboTarget.ServerPosition);
                        }
                    }
                }

                if (ShouldUseIgnite(comboTarget) && player.Distance(comboTarget) <= 600)
                {
                    player.Spellbook.CastSpell(SkillsHandler.IgniteSlot, comboTarget);
                }
            }
        }

        #endregion

        #region Harass

        private static void OnHarass()
        {
            var harassTarget = TargetSelector.GetTarget(SkillsHandler.Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);
        
            if (!ManaManager.CanHarass())
            {
                return;
            }

            if (harassTarget.IsValidTarget())
            {
                if (GetMenuValue<bool>("kassadin.harass.useQ")
                    && harassTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range)
                    && SkillsHandler.Spells[SpellSlot.Q].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.Q].CastOnUnit(harassTarget);
                }

                if (GetMenuValue<bool>("kassadin.harass.useE")
                    && harassTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                    && SkillsHandler.Spells[SpellSlot.E].IsReady() && ECanCast)
                {
                    SkillsHandler.Spells[SpellSlot.E].Cast(harassTarget.ServerPosition);
                }
            }
        }

        #endregion

        #region Waveclear

        private static void OnWaveclear()
        {
            var minionsInRange = MinionManager.GetMinions(
                player.ServerPosition,
                SkillsHandler.Spells[SpellSlot.Q].Range,
                MinionTypes.All,
                MinionTeam.NotAlly);
            var minionsInRangeE = MinionManager.GetMinions(
                player.ServerPosition,
                SkillsHandler.Spells[SpellSlot.E].Range,
                MinionTypes.All,
                MinionTeam.NotAlly);

            if (!ManaManager.CanLaneclear())
            {
                return;
            }

            if (GetMenuValue<bool>("kassadin.waveclear.useQ") && SkillsHandler.Spells[SpellSlot.Q].IsReady())
            {
                var qFarm = minionsInRange.FirstOrDefault(x => SkillsHandler.Spells[SpellSlot.Q].IsKillable(x));

                if (qFarm.IsValidTarget())
                {
                    SkillsHandler.Spells[SpellSlot.Q].CastOnUnit(qFarm);
                }
            }

            if (GetMenuValue<bool>("kassadin.waveclear.useE") && SkillsHandler.Spells[SpellSlot.E].IsReady() && ECanCast)
            {

                if (minionsInRangeE.Count >= 1)
                {
                    foreach (var x in minionsInRangeE)
                    {
                        if (x.IsValidTarget() && MinionManager.GetMinions(x.ServerPosition, 275).Count >= 3)
                        {
                            SkillsHandler.Spells[SpellSlot.E].Cast(x.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion

        #region Flee

        private static void OnFlee(Vector3 position)
        {
            if (SkillsHandler.Spells[SpellSlot.R].IsReady())
            {
                SkillsHandler.Spells[SpellSlot.R].Cast(position);
            }

            Orbwalking.Orbwalk(null, position);
        }

        #endregion

        #region Notifications Credits to Beaving.

        public static Notification ShowNotification(string message, Color color, int duration = -1, bool dispose = true)
        {
            var notif = new Notification(message).SetTextColor(color);
            Notifications.AddNotification(notif);
            if (dispose)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(duration, () => notif.Dispose());
            }
            return notif;
        }

        #endregion

        #region Utility Methods

        public static float GetComboDamage(AIHeroClient target)
        {
            var damage = 0f;

            damage += (float)player.GetAutoAttackDamage(target, true) * 2;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range)
                       && SkillsHandler.Spells[SpellSlot.Q].IsReady() && GetMenuValue<bool>("kassadin.combo.useQ"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.Q)
                          : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.W].Range)
                       && SkillsHandler.Spells[SpellSlot.W].IsReady() && GetMenuValue<bool>("kassadin.combo.useW"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.W)
                          : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                       && SkillsHandler.Spells[SpellSlot.E].IsReady() && GetMenuValue<bool>("kassadin.combo.useE"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.E)
                          : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.R].Range)
                       && SkillsHandler.Spells[SpellSlot.R].IsReady() && GetMenuValue<bool>("kassadin.combo.useR"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.R)
                          : 0f;

            return damage;
        }

        public static T GetMenuValue<T>(String menuItem)
        {
            return Menu.Item(menuItem).GetValue<T>();
        }

        private static bool CanCastIgnite()
        {
            return GetMenuValue<bool>("kassadin.combo.options.useIgnite") && SkillsHandler.IgniteSlot != SpellSlot.Unknown || player.Spellbook.CanUseSpell(SkillsHandler.IgniteSlot) != SpellState.Ready;
        }

        private static bool ShouldUseIgnite(AIHeroClient target)
        {
            if (!CanCastIgnite())
            {
                return false;
            }

            var damage = 0f;

            damage += (float)ObjectManager.Player.GetAutoAttackDamage(target, true) * 2;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range) && SkillsHandler.Spells[SpellSlot.Q].IsReady() && GetMenuValue<bool>("kassadin.combo.useQ"))
                ? (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q)
                : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.W].Range) && SkillsHandler.Spells[SpellSlot.W].IsReady() && GetMenuValue<bool>("kassadin.combo.useW"))
                ? (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.W)
                : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range) && SkillsHandler.Spells[SpellSlot.E].IsReady() && GetMenuValue<bool>("kassadin.combo.useE"))
                ? (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.E)
                : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.R].Range) && SkillsHandler.Spells[SpellSlot.R].IsReady() && GetMenuValue<bool>("kassadin.combo.useR"))
                ? (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.R)
                : 0f;
            var damageWithIgnite = damage +
                                   ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (damage < target.Health + 15 && damageWithIgnite > target.Health + 15)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
