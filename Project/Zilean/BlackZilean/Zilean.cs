using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using BlackZilean.Handlers;
using BlackZilean.Utility;
using SharpDX;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.Common.Menu;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BlackZilean
{
    internal class Zilean
    {
        #region Static Fields

        public static Menu Menu;
        public static ManaManager ManaManager;
        public static Orbwalking.Orbwalker Orbwalker;

        private const string ChampionName = "Zilean";
        private static AIHeroClient player;

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        #endregion  

        #region OnGameLoad

        public static void OnLoad(EventArgs args)
        {
            try
            {
                player = ObjectManager.Player;

                if (player.ChampionName != ChampionName)
                {
                    return;
                }

                ShowNotification("BlackZilean by blacky - Loaded", Color.Crimson, 10000);

                DamageIndicator.Initialize(GetComboDamage);
                DamageIndicator.Enabled = true;
                DamageIndicator.DrawingColor = Color.GreenYellow;

                ManaManager = new ManaManager();
                MenuGenerator.Load();
                SkillsHandler.Load();
                AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
                Drawing.OnDraw += DrawHandler.OnDraw;
                Game.OnUpdate += OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (GetMenuValue<bool>("zilean.misc.gapcloseE") && SkillsHandler.Spells[SpellSlot.E].IsReady()
                && SkillsHandler.Spells[SpellSlot.E].IsInRange(gapcloser.Sender))
            {
                SkillsHandler.Spells[SpellSlot.E].Cast(gapcloser.Sender);
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
                    OnWaveClear();
                    break;
            }

            if (GetMenuValue<KeyBind>("zilean.flee.activated").Active)
            {
                OnFlee(Game.CursorPos);
            }

            OnUpdateMethods();
        }

        #endregion

        #region OnUpdateMethods

        private static void OnUpdateMethods()
        {
            AllyUlt();
            SelfUlt();
            OnImmobile();
        }

        #endregion


        #region OnImmobile

        private static void OnImmobile()
        {
            if (GetMenuValue<bool>("zilean.misc.immobileQ"))
            {
                foreach (var pred in
                    HeroManager.Enemies.Where(
                        hero => hero.IsValidTarget() && hero.Distance(player.Position) <= SkillsHandler.Spells[SpellSlot.Q].Range)
                        .Select(target => SkillsHandler.Spells[SpellSlot.Q].GetPrediction(target))
                        .Where(pred => pred.Hitchance == HitChance.Immobile))
                {
                    SkillsHandler.Spells[SpellSlot.Q].Cast(pred.CastPosition);
                }
            }
        }

        #endregion

        #region Combo

        private static void OnCombo()
        {
            var comboTarget = TargetSelector.GetTarget(
                SkillsHandler.Spells[SpellSlot.Q].Range,
                TargetSelector.DamageType.Magical);

            if (comboTarget.InFountain() || comboTarget.IsInvulnerable || comboTarget.IsZombie)
            {
                return;
            }

            if (comboTarget.IsValidTarget())
            {
                if (GetMenuValue<bool>("zilean.combo.useQ")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range)
                    && SkillsHandler.Spells[SpellSlot.Q].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.Q].CastIfHitchanceEquals(comboTarget, CustomHitChance);
                }

                if (GetMenuValue<bool>("zilean.combo.useW") && comboTarget.HasBuff("ZileanQEnemyBomb")
                    && SkillsHandler.Spells[SpellSlot.W].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.W].Cast();
                }

                if (GetMenuValue<bool>("zilean.combo.useE")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                    && SkillsHandler.Spells[SpellSlot.E].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.E].Cast(comboTarget);
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
                if (GetMenuValue<bool>("zilean.harass.useQ")
                    && harassTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range)
                    && SkillsHandler.Spells[SpellSlot.Q].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.Q].CastIfHitchanceEquals(harassTarget, CustomHitChance);
                }

                if (GetMenuValue<bool>("zilean.harass.useE")
                    && harassTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                    && SkillsHandler.Spells[SpellSlot.E].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.E].Cast(harassTarget);
                }
            }
        }

        #endregion

        #region WaveClear

        private static void OnWaveClear()
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

            if (GetMenuValue<bool>("zilean.waveclear.useQ") && SkillsHandler.Spells[SpellSlot.Q].IsReady())
            {
                var qFarm =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(
                            SkillsHandler.Spells[SpellSlot.Q].Range,
                            MinionTypes.All,
                            MinionTeam.Enemy).Select(m => m.ServerPosition.To2D()).ToList(),
                        SkillsHandler.Spells[SpellSlot.Q].Width,
                        SkillsHandler.Spells[SpellSlot.Q].Range);

                if (minionsInRange.FirstOrDefault().IsValidTarget())
                {
                    SkillsHandler.Spells[SpellSlot.Q].Cast(qFarm.Position);
                }
            }

            if (GetMenuValue<bool>("zilean.waveclear.useW") && SkillsHandler.Spells[SpellSlot.W].IsReady() && !SkillsHandler.Spells[SpellSlot.Q].IsReady())
            {
                SkillsHandler.Spells[SpellSlot.W].Cast();
            }
        }

        #endregion

        #region Flee

        private static void OnFlee(Vector3 position)
        {
            if (SkillsHandler.Spells[SpellSlot.E].IsReady())
            {
                SkillsHandler.Spells[SpellSlot.E].Cast(player);
            }

            if (SkillsHandler.Spells[SpellSlot.W].IsReady() && !SkillsHandler.Spells[SpellSlot.E].IsReady())
            {
                SkillsHandler.Spells[SpellSlot.W].Cast();
            }

            Orbwalking.Orbwalk(null, position);
        }

        #endregion

        #region SelfUlt
        // took something from you jQuery because i was too lazy :p
        private static void SelfUlt()
        {
            var ultSelf = GetMenuValue<bool>("zilean.combo.options.ultSelf");
            var ultSelfHp = GetMenuValue<Slider>("zilean.combo.options.ultSelfHp").Value;

            if (player.IsRecalling() || player.InFountain())
                return;

            if (ultSelf && (player.Health / player.MaxHealth) * 100 <= ultSelfHp && SkillsHandler.Spells[SpellSlot.R].IsReady() && player.CountEnemiesInRange(650) > 0)
            {
                SkillsHandler.Spells[SpellSlot.R].Cast(player);
            }
        }

        #endregion

        #region AllyUlt
        // took something from you jQuery because i was too lazy :p
        private static void AllyUlt()
        {
            var ultAlly = GetMenuValue<bool>("zilean.combo.options.ultAlly");
            var ultAllyHp = GetMenuValue<Slider>("zilean.combo.options.ultAllyHp").Value;

            foreach (var aChamp in ObjectManager.Get<AIHeroClient>().Where(aChamp => aChamp.IsAlly && !aChamp.IsMe))
            {
                var allys = Menu.Item("zilean.combo.options.ultCastAlly" + aChamp.CharData.BaseSkinName);

                if (player.InFountain() || player.IsRecalling())
                    return;

                if (ultAlly && ((aChamp.Health / aChamp.MaxHealth) * 100 <= ultAllyHp) && SkillsHandler.Spells[SpellSlot.R].IsReady() &&
                    player.CountEnemiesInRange(900) > 0 && (aChamp.Distance(player.Position) <= SkillsHandler.Spells[SpellSlot.R].Range))
                {
                    if (allys != null && allys.GetValue<bool>())
                    {
                        SkillsHandler.Spells[SpellSlot.R].Cast(aChamp);
                    }
                }
            }
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

        public static HitChance GetHitchance()
        {
            switch (GetMenuValue<StringList>("zilean.misc.hitchance").SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        public static float GetComboDamage(AIHeroClient target)
        {
            var damage = 0f;

            damage += (float)player.GetAutoAttackDamage(target, true) * 2;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range)
                       && SkillsHandler.Spells[SpellSlot.Q].IsReady() && GetMenuValue<bool>("zilean.combo.useQ"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.Q)
                          : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range) && target.HasBuff("ZileanQEnemyBomb")
                       && SkillsHandler.Spells[SpellSlot.W].IsReady() && GetMenuValue<bool>("zilean.combo.useW"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.Q)
                          : 0f;

            return damage;
        }

        public static T GetMenuValue<T>(String menuItem)
        {
            return Menu.Item(menuItem).GetValue<T>();
        }

        private static bool CanCastIgnite()
        {
            return GetMenuValue<bool>("zilean.combo.options.useIgnite") && SkillsHandler.IgniteSlot != SpellSlot.Unknown || player.Spellbook.CanUseSpell(SkillsHandler.IgniteSlot) != SpellState.Ready;
        }

        private static bool ShouldUseIgnite(AIHeroClient target)
        {
            if (!CanCastIgnite())
            {
                return false;
            }

            var damage = 0f;

            damage += (float)player.GetAutoAttackDamage(target, true) * 2;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range) && SkillsHandler.Spells[SpellSlot.Q].IsReady() && GetMenuValue<bool>("zilean.combo.useQ"))
                ? (float)player.GetSpellDamage(target, SpellSlot.Q)
                : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.Q].Range) && target.HasBuff("ZileanQEnemyBomb") && SkillsHandler.Spells[SpellSlot.W].IsReady() && GetMenuValue<bool>("zilean.combo.useW"))
                ? (float)player.GetSpellDamage(target, SpellSlot.Q)
                : 0f;
            var damageWithIgnite = damage +
                                   player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (damage < target.Health + 15 && damageWithIgnite > target.Health + 15)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
