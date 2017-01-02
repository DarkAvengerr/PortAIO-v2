using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Lissandra_the_Ice_Goddess.Evade;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.Common.Menu;
using Lissandra_the_Ice_Goddess.Handlers;
using Lissandra_the_Ice_Goddess.Utility;
using Lissandra_the_Ice_Goddess.Utility.Damage;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Lissandra_the_Ice_Goddess
{
    internal class Lissandra
    {
        #region Static Fields

        public static Menu Menu;
        public static ManaManager ManaManager;
        public static Orbwalking.Orbwalker Orbwalker;

        private const string ChampionName = "Lissandra";
        private static AIHeroClient player;

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        #region E Related

        private static bool eActive = false;

        public static bool EActive
        {
            get { return eActive && EDuration > 0; }
        }

        public static int ECastTime = 0;

        public static float EDuration
        {
            get { return (ECastTime + 1500 - Environment.TickCount) / 1000f; }
        }

        public static Vector3 EStart { get; set; }
        public static Vector3 EEnd { get; set; }

        public static float EStartTick { get; set; }

        public static float EEndTick
        {
            get { return (EStartTick + 1500) / 1000f; }
        }

        public static Vector3 CurrentEPosition
        {
            get
            {
                var currentPoint =(float) Math.Floor(
                        ((Environment.TickCount - EStartTick) / 1000f - SkillsHandler.Spells[SpellSlot.E].Delay) *
                        SkillsHandler.Spells[SpellSlot.E].Speed);
                return EStart.Extend(EEnd,  currentPoint < SkillsHandler.Spells[SpellSlot.E].Range ? currentPoint : SkillsHandler.Spells[SpellSlot.E].Range); 
            }
        }

        public static float TimeFromCurrentToEnd
        {
            get { return (Vector3.Distance(CurrentEPosition, EEnd) / SkillsHandler.Spells[SpellSlot.E].Speed) * 1000f; }
        }

        #endregion

        #endregion

        #region OnLoad

        public static void OnLoad()
        {
            try
            {
                player = ObjectManager.Player;

                if (player.ChampionName != ChampionName)
                {
                    return;
                }

                ShowNotification("Lissandra the Ice Goddess", Color.DeepSkyBlue, 10000);
                ShowNotification("by blacky & Asuna - Loaded", Color.DeepSkyBlue, 10000);

                //DamageIndicator.Initialize(GetComboDamage);
                //DamageIndicator.Enabled = true;
                //DamageIndicator.DrawingColor = Color.GreenYellow;

                ManaManager = new ManaManager();
                MenuGenerator.Load();
                SkillsHandler.Load();
                AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
                Drawing.OnDraw += DrawHandler.OnDraw;
                Game.OnUpdate += OnUpdate;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                DamagePrediction.OnTargettedSpellWillKill += DamagePrediction_OnTargettedSpellWillKill;
                EStartTick = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }


        #endregion

        #region Event Delegates
        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && ObjectManager.Player.GetSpellSlot(args.SData.Name) == SpellSlot.E)
            {
                if (EEndTick < Utils.TickCount)
                {
                    EStart = Vector3.Zero;
                    EEnd = Vector3.Zero;
                    EStartTick = -1;
                }
                else
                {
                    EStart = args.Start;
                    EEnd = player.ServerPosition.Extend(args.End, SkillsHandler.Spells[SpellSlot.E].Range);
                    EStartTick = Utils.TickCount;
                }

                if (EDuration > -1)
                {
                    eActive = false;
                }
                else
                {
                    ECastTime = Environment.TickCount;
                    eActive = true;
                }
            }
        }

        private static void DamagePrediction_OnTargettedSpellWillKill(AIHeroClient sender, AIHeroClient target, EloBuddy.SpellData sData)
        {
            if (GetMenuValue<bool>("lissandra.misc.saveR") && SkillsHandler.Spells[SpellSlot.R].IsReady())
            {
                SkillsHandler.Spells[SpellSlot.R].Cast(ObjectManager.Player);
            }
        }

        #region OnEnemyGapcloser

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("lissandra.misc.gapcloseW").GetValue<bool>() && SkillsHandler.Spells[SpellSlot.W].IsReady()
                && SkillsHandler.Spells[SpellSlot.W].IsInRange(gapcloser.Sender))
            {
                SkillsHandler.Spells[SpellSlot.W].Cast();
            }
        }

        #endregion

        #region OnInterruptableTarget

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("lissandra.misc.interruptR").GetValue<bool>())
            {
                if (args.DangerLevel == Interrupter2.DangerLevel.High
                    && sender.IsValidTarget(SkillsHandler.Spells[SpellSlot.R].Range)
                    && SkillsHandler.Spells[SpellSlot.R].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.R].CastOnUnit(sender);
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

            if (GetMenuValue<KeyBind>("lissandra.flee.activated").Active)
            {
                OnFlee(Game.CursorPos);
            }

            OnUpdateMethods();
        }

        #endregion

        #endregion

        #region OnUpdateMethods

        private static void OnUpdateMethods()
        {
            CheckEvade();
            StunUnderTower();
        }

        private static void StunUnderTower()
        {
            if (GetMenuValue<bool>("lissandra.misc.stunUnderTower"))
            {
                foreach (
                    var stunTarget in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                enemy =>
                                enemy.IsEnemy
                                && player.Distance(enemy.ServerPosition) <= SkillsHandler.Spells[SpellSlot.R].Range)
                            .Where(
                                enemy =>
                                ObjectManager.Get<Obj_AI_Turret>()
                                    .Where(turret => turret.IsValid && turret.IsAlly)
                                    .Any(
                                        turret =>
                                        Vector2.Distance(enemy.Position.To2D(), turret.Position.To2D()) < 750
                                        && SkillsHandler.Spells[SpellSlot.R].IsReady())))
                {
                    SkillsHandler.Spells[SpellSlot.R].Cast(stunTarget);
                }
            }
        }

        private static void CheckEvade()
        {
            if (EStart != Vector3.Zero && 
                EEnd != Vector3.Zero &&
                EvadeHelper.EvadeDetectedSkillshots.Any(
                    skillshot => 
                        skillshot.SpellData.IsDangerous 
                     && skillshot.SpellData.DangerValue >= 3
                     && skillshot.IsAboutToHit((int)TimeFromCurrentToEnd, EEnd)))
            {
                if (CurrentEPosition.IsSafePosition())
                {
                    SkillsHandler.Spells[SpellSlot.E].Cast();
                }
            }
            var rSkills = EvadeHelper.EvadeDetectedSkillshots.Where(
                skillshot =>
                    skillshot.SpellData.IsDangerous
                    && skillshot.SpellData.DangerValue >= 3
                    && skillshot.IsAboutToHit(300, ObjectManager.Player.ServerPosition)).ToList();

            if (GetMenuValue<bool>("lissandra.misc.saveR") && SkillsHandler.Spells[SpellSlot.R].IsReady() && rSkills.Any())
            {
                //Let's try this, never done it ayy lmao
                if (
                    rSkills.Any(
                        skillshot =>
                            skillshot.Caster.GetSpellDamage(ObjectManager.Player, skillshot.SpellData.SpellName) >=
                            ObjectManager.Player.Health + 15))
                {
                    //Found at least 1 spell that is gonna kill me. Better immune Kappa.
                    SkillsHandler.Spells[SpellSlot.R].Cast();
                }
            }

        }
        #endregion

        #region Combo

        private static void OnCombo()
        {
            //TODO The All In Combo.
            var comboTarget = TargetSelector.GetTarget(SkillsHandler.QShard.Range,
                TargetSelector.DamageType.Magical);

            if (comboTarget.IsValidTarget())
            {
                if (GetMenuValue<bool>("lissandra.combo.useQ") 
                    && comboTarget.IsValidTarget(SkillsHandler.QShard.Range) 
                    && SkillsHandler.Spells[SpellSlot.Q].IsReady())
                {
                    var predictionPosition = SkillsHandler.GetQPrediction(comboTarget);
                    if (predictionPosition != null)
                    {
                        //Found a valid Q prediction
                        SkillsHandler.Spells[SpellSlot.Q].Cast((Vector3) predictionPosition);
                    }
                }

                if (GetMenuValue<bool>("lissandra.combo.useW")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.W].Range)
                    && SkillsHandler.Spells[SpellSlot.W].IsReady() && !comboTarget.IsStunned)
                {
                    SkillsHandler.Spells[SpellSlot.W].Cast();
                }

                if (GetMenuValue<bool>("lissandra.combo.useE")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                    && SkillsHandler.Spells[SpellSlot.E].IsReady())
                {
                    if (!EActive)
                    {
                        var comboTargetPosition = Prediction.GetPrediction(comboTarget,
                            TimeToEEnd(ObjectManager.Player.ServerPosition, comboTarget.ServerPosition)).UnitPosition;
                        //TODO This will probably fail horribly because it's such a long delay ayy lmao
                        if (comboTargetPosition.IsSafePositionEx() && comboTargetPosition.PassesNoEIntoEnemiesCheck())
                        {
                            SkillsHandler.Spells[SpellSlot.E].Cast(comboTargetPosition);
                        }
                        else
                        {
                            if (SkillsHandler.Spells[SpellSlot.E].IsInRange(comboTargetPosition) && CurrentEPosition.IsSafePositionEx())
                            {
                                SkillsHandler.Spells[SpellSlot.E].Cast(comboTarget);
                            }
                        }
                    }
                }

                if (GetMenuValue<bool>("lissandra.combo.useR")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.R].Range)
                    && SkillsHandler.Spells[SpellSlot.R].IsReady())
                {
                    var selfR = GetMenuValue<Slider>("lissandra.combo.options.selfR").Value;
                    var defensiveR = GetMenuValue<Slider>("lissandra.combo.options.defensiveR").Value;

                    if (player.CountEnemiesInRange(250) >= defensiveR)
                    {
                        SkillsHandler.Spells[SpellSlot.R].CastOnUnit(player);
                    }

                    if (player.Health / player.MaxHealth * 100 < selfR && player.CountEnemiesInRange(SkillsHandler.Spells[SpellSlot.R].Range) > 0)
                    {
                        SkillsHandler.Spells[SpellSlot.R].CastOnUnit(player);
                    }

                    if (GetMenuValue<bool>("lissandra.combo.options.alwaysR"))
                    {
                        SkillsHandler.Spells[SpellSlot.R].CastOnUnit(comboTarget);
                    }

                    if ((player.GetSpellDamage(comboTarget, SpellSlot.R) * 1.2) > comboTarget.Health + 10
                        && Menu.Item("lissandra.combo.options.whitelistR" + comboTarget.CharData.BaseSkinName) != null
                        && Menu.Item("lissandra.combo.options.whitelistR" + comboTarget.CharData.BaseSkinName)
                               .GetValue<bool>() == false)
                    {
                        SkillsHandler.Spells[SpellSlot.R].CastOnUnit(comboTarget);
                    }

                    if (GetComboDamage(comboTarget) > comboTarget.Health + 10
                        && Menu.Item("lissandra.combo.options.whitelistR" + comboTarget.CharData.BaseSkinName) != null
                        && Menu.Item("lissandra.combo.options.whitelistR" + comboTarget.CharData.BaseSkinName)
                               .GetValue<bool>() == false)
                    {
                        SkillsHandler.Spells[SpellSlot.R].CastOnUnit(comboTarget);
                    }
                }

                if (ShouldUseIgnite(comboTarget) && player.Distance(comboTarget) <= 600)
                {
                    player.Spellbook.CastSpell(SkillsHandler.IgniteSlot, comboTarget);
                }
            }
        }

        private static float TimeToEEnd(Vector3 from, Vector3 to)
        {
            return (Vector3.Distance(from, to) / SkillsHandler.Spells[SpellSlot.E].Speed) * 1000f; 
        }

        
        #endregion

        #region Harass

        private static void OnHarass()
        {
            var harassTarget = TargetSelector.GetTarget(SkillsHandler.QShard.Range, TargetSelector.DamageType.Magical);

            if (!ManaManager.CanHarass() && !ManaManager.PlayerHasPassive())
            {
                return;
            }

            if (harassTarget.IsValidTarget())
            {
                if (GetMenuValue<bool>("lissandra.harass.useQ")
                    && harassTarget.IsValidTarget(SkillsHandler.QShard.Range)
                    && SkillsHandler.Spells[SpellSlot.Q].IsReady())
                {
                    var predictionPosition = SkillsHandler.GetQPrediction(harassTarget);
                    if (predictionPosition != null)
                    {
                        //Found a valid Q prediction
                        SkillsHandler.Spells[SpellSlot.Q].Cast((Vector3)predictionPosition);
                    }
                }

                if (GetMenuValue<bool>("lissandra.harass.useW")
                    && harassTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.W].Range)
                    && SkillsHandler.Spells[SpellSlot.W].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.W].Cast();
                }

                if (GetMenuValue<bool>("lissandra.harass.useE")
                    && harassTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                    && SkillsHandler.Spells[SpellSlot.E].IsReady())
                {
                    if (!EActive)
                    {
                        SkillsHandler.Spells[SpellSlot.E].CastIfHitchanceEquals(harassTarget, CustomHitChance);
                    }
                }
            }
        }

        #endregion

        #region Flee

        private static void OnFlee(Vector3 position)
        {
            if (SkillsHandler.Spells[SpellSlot.W].IsReady()
                && player.CountEnemiesInRange(SkillsHandler.Spells[SpellSlot.W].Range) > 0)
            {
                SkillsHandler.Spells[SpellSlot.W].Cast();
            }

            if (SkillsHandler.Spells[SpellSlot.E].IsReady())
            {
                if (!EActive)
                {
                    SkillsHandler.Spells[SpellSlot.E].Cast(position);
                }
                else
                {
                    if (CurrentEPosition.Distance(position) < 25)
                    {
                        if (CurrentEPosition.IsSafePositionEx() && CurrentEPosition.PassesNoEIntoEnemiesCheck())
                        {
                            SkillsHandler.Spells[SpellSlot.E].Cast();
                        }
                    }
                }
            }

            Orbwalking.Orbwalk(null, Game.CursorPos);
        }

        #endregion

        #region Waveclear

        private static void OnWaveclear()
        {
            var minionsInRange = MinionManager.GetMinions(
                player.ServerPosition,
                SkillsHandler.QShard.Range,
                MinionTypes.All,
                MinionTeam.NotAlly);
            var minionsInRangeW = MinionManager.GetMinions(
                player.ServerPosition,
                SkillsHandler.Spells[SpellSlot.W].Range,
                MinionTypes.All,
                MinionTeam.NotAlly);

            if (!ManaManager.CanLaneclear() && !ManaManager.PlayerHasPassive())
            {
                return;
            }

            if (GetMenuValue<bool>("lissandra.waveclear.useQ") && SkillsHandler.Spells[SpellSlot.Q].IsReady())
            {
                var qLineFarm = SkillsHandler.Spells[SpellSlot.Q].GetLineFarmLocation(minionsInRange);

                if (qLineFarm.MinionsHit >= 3)
                {
                    SkillsHandler.Spells[SpellSlot.Q].Cast(qLineFarm.Position);
                }
            }

            foreach (var minion in minionsInRangeW.Where(x => minionsInRangeW.Count >= 2 && player.GetSpellDamage(x, SpellSlot.W) - 10 >= 
                                          HealthPrediction.GetHealthPrediction(x, (int)(SkillsHandler.Spells[SpellSlot.W].Delay))))
            {
                if (GetMenuValue<bool>("lissandra.waveclear.useW"))
                {
                    SkillsHandler.Spells[SpellSlot.W].Cast(minion);
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

        public static float GetComboDamage(AIHeroClient target)
        {
            var damage = 0f;

            damage += (float)player.GetAutoAttackDamage(target, true) * 2;
            damage += (SkillsHandler.GetQPrediction(target) != null && SkillsHandler.Spells[SpellSlot.Q].IsReady()
                       && GetMenuValue<bool>("lissandra.combo.useQ"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.Q)
                          : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.W].Range)
                       && SkillsHandler.Spells[SpellSlot.W].IsReady() && GetMenuValue<bool>("lissandra.combo.useW"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.W)
                          : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                       && SkillsHandler.Spells[SpellSlot.E].IsReady() && GetMenuValue<bool>("lissandra.combo.useE"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.E)
                          : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.R].Range)
                       && SkillsHandler.Spells[SpellSlot.R].IsReady() && GetMenuValue<bool>("lissandra.combo.useR"))
                          ? (float)player.GetSpellDamage(target, SpellSlot.R)
                          : 0f;

            return damage;
        }

        private static HitChance GetHitchance()
        {
            switch (Menu.Item("lissandra.misc.hitChance").GetValue<StringList>().SelectedIndex)
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

        public static T GetMenuValue<T>(String menuItem)
        {
            return Menu.Item(menuItem).GetValue<T>();
        }

        private static bool CanCastIgnite()
        {
            return GetMenuValue<bool>("lissandra.combo.options.useIgnite") && SkillsHandler.IgniteSlot != SpellSlot.Unknown || player.Spellbook.CanUseSpell(SkillsHandler.IgniteSlot) != SpellState.Ready;
        }

        private static bool ShouldUseIgnite(AIHeroClient target)
        {
            if (!CanCastIgnite())
            {
                return false;
            }
            var damage = 0f;

            damage += (float)ObjectManager.Player.GetAutoAttackDamage(target, true) * 2;
            damage += (SkillsHandler.GetQPrediction(target) != null && SkillsHandler.Spells[SpellSlot.Q].IsReady() && GetMenuValue<bool>("lissandra.combo.useQ"))
                ? (float)player.GetSpellDamage(target, SpellSlot.Q)
                : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.W].Range) && SkillsHandler.Spells[SpellSlot.W].IsReady() && GetMenuValue<bool>("lissandra.combo.useW"))
                ? (float)player.GetSpellDamage(target, SpellSlot.W)
                : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range) && SkillsHandler.Spells[SpellSlot.E].IsReady() && GetMenuValue<bool>("lissandra.combo.useE"))
                ? (float)player.GetSpellDamage(target, SpellSlot.E)
                : 0f;
            damage += (target.IsValidTarget(SkillsHandler.Spells[SpellSlot.R].Range) && SkillsHandler.Spells[SpellSlot.R].IsReady() && GetMenuValue<bool>("lissandra.combo.useR"))
                ? (float)player.GetSpellDamage(target, SpellSlot.R)
                : 0f;
            var damageWithIgnite = damage + player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (damage < target.Health + 15 && damageWithIgnite > target.Health + 15)
            {
                return true;
            }

            return false;
        }
        #endregion

    }
}