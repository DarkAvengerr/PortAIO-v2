// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Karthus.cs" company="">
//  Copyright (c) RAREKarthus. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RAREKarthus.ChampionModes
{
    #region directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Additions;
    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Utilities = Utilities;

    #endregion

    internal class Karthus
    {
        /// <summary>
        ///     Initialization for Karthus.
        ///     Should be called at first.
        /// </summary>
        public void Init()
        {
            #region skills setup

            // initializing all skills with all attributes
            Utilities.Q = new Spell(SpellSlot.Q, 875)
                .SetSkillshot(1f, 160, int.MaxValue, false, SkillshotType.SkillshotCircle);
            Utilities.W = new Spell(SpellSlot.W, 1000)
                .SetSkillshot(.5f, 70, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Utilities.E = new Spell(SpellSlot.E, 505)
                .SetSkillshot(1f, 505, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Utilities.R = new Spell(SpellSlot.R)
                .SetSkillshot(3f, float.MaxValue, float.MaxValue, false, SkillshotType.SkillshotCircle);
            // setting up there damage type
            Utilities.Q.DamageType = Utilities.W.DamageType = Utilities.E.DamageType = Utilities.R.DamageType =
                DamageType.Magical;
            // setting up the standart hitchange for Q
            Utilities.Q.MinHitChance = HitChance.High;

            #endregion

            // init the menu for karthus, maybe more champs soon.
            ChampionMenu();

            //game update events - combo etc.
            Game.OnUpdate += Game_OnUpdate;
            // drawing event to draw something => lower refresh rate than game update.
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        ///     Is drawing all needed drawings to your world.
        /// </summary>
        /// <param name="args">parameter that are given by the process itself. (not needed yet)</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Utilities.Player.IsDead)
            {
                return;
            }

            if (Utilities.MainMenu["Draw"]["Q"] && Utilities.Q.Level > 0)
            {
                Render.Circle.DrawCircle(Utilities.Player.Position, Utilities.Q.Range, Color.AntiqueWhite, 2);
            }

            if (Utilities.MainMenu["Draw"]["W"] && Utilities.W.Level > 0)
            {
                Render.Circle.DrawCircle(Utilities.Player.Position, Utilities.W.Range, Color.ForestGreen, 2);
            }

            if (Utilities.MainMenu["Draw"]["E"] && Utilities.E.Level > 0)
            {
                Render.Circle.DrawCircle(Utilities.Player.Position, Utilities.E.Range, Color.OrangeRed, 2);
            }

            // no need for ult, because it's global Fappa
        }

        /// <summary>
        ///     Main ticking rotation which decides what method will be checked during the game.
        /// </summary>
        /// <param name="args">parameter that are given by the process itself. (not needed yet)</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            //checking if UltKS is enabled.
            if (GameObjects.EnemyHeroes.Count(x => Utilities.Player.Distance(x) <= 500f) <= 0
                && (Utilities.MainMenu["R"]["KS"] || Utilities.MainMenu["R"]["Save"]))
            {
                //start ultks method
                AutoUlt();
            }

            // check if ping notifying is enabled.
            if (Utilities.MainMenu["Utilities"]["NotifyPing"])
            {
                // for each player, who's killable with R gets pinged.
                foreach (var enemy in GameObjects.EnemyHeroes.Where(
                    t => Utilities.R.IsReady() && t.IsValidTarget() &&
                         Utilities.R.GetDamage(t, DamageStage.Detonation) > t.Health &&
                         t.Distance(ObjectManager.Player.Position) > Utilities.Q.Range))
                {
                    TacticalMap.ShowPing(PingCategory.Danger, enemy);
                    //internal pings :D
                }
            }

            if (Core.IsInPassiveForm())
            {
                Combo();
                LaneClear();
            }

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Variables.Orbwalker.AttackState = Utilities.MainMenu["Modes"]["useaa"] ||
                                                      ObjectManager.Player.Mana < 100;
                    //if no mana, allow auto attacks!
                    Variables.Orbwalker.MovementState = Utilities.MainMenu["Modes"]["usemm"];
                    Combo();
                    break;
                case OrbwalkingMode.Hybrid:
                    Variables.Orbwalker.AttackState = true;
                    Variables.Orbwalker.MovementState = Utilities.MainMenu["Modes"]["usemm"];
                    Harass();
                    break;
                case OrbwalkingMode.LaneClear:
                    Variables.Orbwalker.AttackState = Utilities.MainMenu["Modes"]["useaa"] ||
                                                      ObjectManager.Player.Mana < 100;
                    Variables.Orbwalker.MovementState = Utilities.MainMenu["Modes"]["usemm"];
                    LaneClear();
                    break;
                case OrbwalkingMode.LastHit:
                    Variables.Orbwalker.AttackState = Utilities.MainMenu["Utilities"]["LastAA"];
                    Variables.Orbwalker.MovementState = Utilities.MainMenu["Modes"]["usemm"];
                    LastHit();
                    break;
                default:
                    Variables.Orbwalker.AttackState = true;
                    Variables.Orbwalker.MovementState = true;
                    EState(OrbwalkingMode.None);
                    break;
            }
        }

        /// <summary>
        ///     The current EState handler for Karthus. Auto ON/OFF
        /// </summary>
        /// <param name="mymode">The parameter which defines in what mode you currently are.</param>
        /// <param name="ty">Only necessary if you are in the farming mode.</param>
        private static void EState(OrbwalkingMode mymode, IEnumerable<Obj_AI_Base> ty = null)
        {
            if (!Utilities.E.IsReady() || Core.IsInPassiveForm())
                return;

            switch (mymode)
            {
                case OrbwalkingMode.LaneClear:
                    if (ty != null)
                    {
                        var objAiBases = ty as IList<Obj_AI_Base> ?? ty.ToList();
                        if ((objAiBases.Any() &&
                             objAiBases.Count(x => x.Health < Utilities.E.GetDamage(x)*3 && x.IsValidTarget(Utilities.E.Range)) >= 2) ||
                            objAiBases.Count(x => x.IsValidTarget(Utilities.E.Range)) >= 5)
                        {
                            if (!Utilities.Player.HasBuff("KarthusDefile"))
                                Utilities.E.Cast();
                        }
                        else
                        {
                            if (Utilities.Player.HasBuff("KarthusDefile"))
                                Utilities.E.Cast();
                        }
                    }
                    break;

                case OrbwalkingMode.Combo:
                    if (Utilities.Player.CountEnemyHeroesInRange(Utilities.E.Range) >= 1)
                    {
                        if (!Utilities.Player.HasBuff("KarthusDefile"))
                            Utilities.E.Cast();
                    }
                    else
                    {
                        if (Utilities.Player.HasBuff("KarthusDefile") && Utilities.MainMenu["E"]["ToggleE"])
                            Utilities.E.Cast();
                    }
                    break;

                default:
                    if (Utilities.Player.HasBuff("KarthusDefile") && Utilities.MainMenu["E"]["ToggleE"])
                        Utilities.E.Cast();
                    break;
            }
        }

        /// <summary>
        ///     The method, which handles the AutoUlt process.
        /// </summary>
        private static void AutoUlt()
        {
            var count = -1;

            if (Utilities.MainMenu["R"]["KS"])
            {
                count =
                    GameObjects.EnemyHeroes.Count(
                        champ => champ.Health < Utilities.R.GetDamage(champ, DamageStage.Detonation)*0.90 &&
                                 (!champ.HasBuffOfType(BuffType.SpellShield) ||
                                  !champ.HasBuffOfType(BuffType.Invulnerability)) &&
                                 HealthPrediction.GetHealthPrediction(champ, 1500) <=
                                 Utilities.R.GetDamage(champ, DamageStage.Detonation) &&
                                 HealthPrediction.GetHealthPrediction(champ, 1500) >= 100);
            }
            else if (Utilities.MainMenu["R"]["Save"])
            {
                count =
                    GameObjects.EnemyHeroes.Count(
                        champ => champ.Health < Utilities.R.GetDamage(champ, DamageStage.Detonation)*0.90
                                 && champ.CountEnemyHeroesInRange(250f) == 0 &&
                                 (!champ.HasBuffOfType(BuffType.SpellShield) ||
                                  !champ.HasBuffOfType(BuffType.Invulnerability))
                                 && champ.IsValidTarget(Utilities.R.Range));
            }

            if (count >= Utilities.MainMenu["R"]["CountKS"])
            {
                Utilities.R.Cast();
            }
        }

        /// <summary>
        ///     The method, which handles the Combo process.
        /// </summary>
        // ReSharper disable once CyclomaticComplexity
        private static void Combo()
        {
            Obj_AI_Base seltarget = Utilities.targetSelector.GetSelectedTarget();

            if (Utilities.MainMenu["Q"]["ComboQ"])
            {
                if (seltarget == null)
                {
                    var target = Utilities.targetSelector.GetTargetNoCollision(Utilities.Q);
                    if (target.IsValidTarget(Utilities.Q.Range) && !target.IsDead && target.IsValid)
                    {
                        var prediction = Utilities.Q.GetPrediction(target).CastPosition;
                        Core.CastQ(target, prediction);
                    }
                }
                else
                {
                    if (seltarget.IsValidTarget(Utilities.Q.Range) && !seltarget.IsDead && seltarget.IsValid)
                    {
                        var prediction = Utilities.Q.GetPrediction(seltarget, true, Utilities.Q.Range).CastPosition;
                        Core.CastQ(seltarget, prediction);
                    }
                    else
                    {
                        var target = Utilities.targetSelector.GetTargetNoCollision(Utilities.Q);
                        if (target.IsValidTarget(Utilities.Q.Range) && !target.IsDead && target.IsValid)
                        {
                            var prediction = Utilities.Q.GetPrediction(seltarget, true, Utilities.Q.Range).CastPosition;
                            Core.CastQ(target, prediction);
                        }
                    }
                }
            }

            if (Utilities.MainMenu["W"]["ComboW"])
            {
                if (seltarget == null)
                {
                    var target = Utilities.targetSelector.GetTargetNoCollision(Utilities.W);
                    if (target.IsValidTarget(Utilities.W.Range) && !target.IsDead && target.IsValid)
                    {
                        Core.CastW(target);
                    }
                }
                else
                {
                    if (seltarget.IsValidTarget(Utilities.W.Range) && !seltarget.IsDead && seltarget.IsValid)
                    {
                        Core.CastW(seltarget);
                    }
                    else
                    {
                        var target = Utilities.targetSelector.GetTargetNoCollision(Utilities.W);
                        if (target.IsValidTarget(Utilities.W.Range) && !target.IsDead && target.IsValid)
                        {
                            Core.CastW(target);
                        }
                    }
                }
            }

            if (Utilities.MainMenu["E"]["ComboE"])
            {
                EState(OrbwalkingMode.Combo);
            }
        }

        /// <summary>
        ///     The method, which handles the Harass process.
        /// </summary>
        private static void Harass()
        {
            Obj_AI_Base seltarget = Utilities.targetSelector.GetSelectedTarget();

            if (Utilities.MainMenu["Q"]["HarassQ"])
            {
                if (seltarget == null)
                {
                    var target = Utilities.targetSelector.GetTargetNoCollision(Utilities.Q);
                    if (target.IsValidTarget(Utilities.Q.Range) && !target.IsDead && target.IsValid)
                    {
                        var prediction = Utilities.Q.GetPrediction(target).CastPosition;
                        Core.CastQ(target, prediction);
                    }
                }
                else
                {
                    if (seltarget.IsValidTarget(Utilities.Q.Range) && !seltarget.IsDead && seltarget.IsValid)
                    {
                        var prediction = Utilities.Q.GetPrediction(seltarget, true, Utilities.Q.Range).CastPosition;
                        Core.CastQ(seltarget, prediction);
                    }
                    else
                    {
                        var target = Utilities.targetSelector.GetTargetNoCollision(Utilities.Q);
                        if (target.IsValidTarget(Utilities.Q.Range) && !target.IsDead && target.IsValid)
                        {
                            var prediction = Utilities.Q.GetPrediction(seltarget, true, Utilities.Q.Range).CastPosition;
                            Core.CastQ(target, prediction);
                        }
                    }
                }
            }

            if (Utilities.MainMenu["W"]["HarassW"])
            {
                if (seltarget == null)
                {
                    var target = Utilities.targetSelector.GetTargetNoCollision(Utilities.W);
                    if (target.IsValidTarget(Utilities.W.Range) && !target.IsDead && target.IsValid)
                    {
                        Core.CastW(target);
                    }
                }
                else
                {
                    if (seltarget.IsValidTarget(Utilities.W.Range) && !seltarget.IsDead && seltarget.IsValid)
                    {
                        Core.CastW(seltarget);
                    }
                    else
                    {
                        var target = Utilities.targetSelector.GetTargetNoCollision(Utilities.W);
                        if (target.IsValidTarget(Utilities.W.Range) && !target.IsDead && target.IsValid)
                        {
                            Core.CastW(target);
                        }
                    }
                }
            }

            if (Utilities.MainMenu["E"]["HarassE"])
            {
                EState(OrbwalkingMode.Combo);
            }

            if (!Utilities.MainMenu["Q"]["HarassQ"])
                return;

            // get all minions with costum equalations
            var allMinions = GameObjects.EnemyMinions.Where(m => Utilities.Q.IsInRange(m));

            // if minions is NOT empty or  there are any minions
            var objAiMinions = allMinions as Obj_AI_Minion[] ?? allMinions.ToArray();
            if (!objAiMinions.Any()) return;

            //var minions = allMinions.OrderBy(i => i.Health).FirstOrDefault(); // => get first one or default.
            foreach (
                var minion in
                    objAiMinions.Where(
                        minion =>
                            minion.IsValidTarget(Utilities.Q.Range) && !minion.InAutoAttackRange() ||
                            !minion.IsUnderAllyTurret()))
            {
                var hpred = HealthPrediction.GetHealthPrediction(minion, 1000);
                if (minion != null && hpred < Core.GetQDamage(minion) && hpred > minion.Health - hpred*2)
                {
                    var prediction = Utilities.Q.GetPrediction(minion, true, Utilities.Q.Range).CastPosition;
                    Core.CastQ(minion, prediction, Utilities.MainMenu["Q"]["LastMana"]);
                    // => CastQ on it, while refering on your mana.
                }
            }
        }

        /// <summary>
        ///     The method, which handles the LaneClear process.
        /// </summary>
        private static void LaneClear()
        {
            var lowies = GameObjects.EnemyMinions.Where(x => Utilities.Q.IsInRange(x) && x.Health <= Core.GetQDamage(x));
            var highs = GameObjects.EnemyMinions.Where(x => Utilities.Q.IsInRange(x) && x.Health >= Core.GetQDamage(x));

            IEnumerable<Obj_AI_Minion> objAiBases = highs as Obj_AI_Minion[] ?? highs.ToArray();

            var all = GameObjects.EnemyMinions.Where(x => Utilities.Q.IsInRange(x));

            if (Utilities.MainMenu["Q"]["FarmQ"])
            {
                var objAiMinions = lowies as Obj_AI_Minion[] ?? lowies.ToArray();
                if (objAiMinions.Any())
                {
                    foreach (var lo in objAiMinions)
                    {
                        var hpred = HealthPrediction.GetHealthPrediction(lo, 1000);
                        if (lo != null && hpred < Core.GetQDamage(lo) && hpred > lo.Health - hpred*2)
                        {
                            Core.CastQ(lo, Vector3.Zero, Utilities.MainMenu["Q"]["FarmMana"]);
                            // => CastQ on it, while refering on your mana.
                        }
                    }
                }
                else
                {
                    Core.CastQ(objAiBases.FirstOrDefault(), Vector3.Zero, Utilities.MainMenu["Q"]["FarmMana"]);
                }
            }

            if (Utilities.MainMenu["E"]["FarmE"])
            {
                EState(OrbwalkingMode.LaneClear, all);
            }
        }

        /// <summary>
        ///     The method, which handles the LastHitting process.
        /// </summary>
        public static void LastHit()
        {
            // look up if enabled and Q is ready and isNot in passive form. (before dead)
            if (!Utilities.MainMenu["Q"]["LastQ"])
                return;

            // get all minions with costum equalations
            var allMinions = GameObjects.EnemyMinions.Where(m => Utilities.Q.IsInRange(m));

            // if minions is NOT empty or  there are any minions
            var objAiMinions = allMinions as Obj_AI_Minion[] ?? allMinions.ToArray();
            if (!objAiMinions.Any()) return;

            //var minions = allMinions.OrderBy(i => i.Health).FirstOrDefault(); // => get first one or default.
            foreach (
                var minion in
                    objAiMinions.Where(
                        minion =>
                            minion.IsValidTarget(Utilities.Q.Range) && !minion.InAutoAttackRange() ||
                            !minion.IsUnderAllyTurret()))
            {
                var hpred = HealthPrediction.GetHealthPrediction(minion, 1000);
                if (minion != null && hpred < Core.GetQDamage(minion) && hpred > minion.Health - hpred*2)
                {
                    var prediction = Utilities.Q.GetPrediction(minion, true, Utilities.Q.Range).CastPosition;
                    Core.CastQ(minion, prediction, Utilities.MainMenu["Q"]["LastMana"]);
                    // => CastQ on it, while refering on your mana.
                }
            }
        }

        /// <summary>
        ///     Initialize the champion menu for karthus.
        /// </summary>
        public static void ChampionMenu()
        {
            var modesSettings = Utilities.MainMenu.Add(new Menu("Modes", "Modes"));
            {
                modesSettings.Bool("useaa", "Use AutoAttacks in Modes");
                modesSettings.Bool("usemm", "Able to Move in Modes");
            }

            var qMenu = Utilities.MainMenu.Add(new Menu("Q", "Q spell"));
            {
                qMenu.Separator("Combo");
                qMenu.Bool("ComboQ", "Use Q");
                qMenu.Separator("Harass");
                qMenu.Bool("HarassQ", "Use Q");
                qMenu.Separator("Farm");
                qMenu.Bool("FarmQ", "Use Q");
                qMenu.Slider("FarmMana", "min. mana x%", 50);
                qMenu.Separator("LastHit");
                qMenu.Bool("LastQ", "Use Q");
                qMenu.Slider("LastMana", "min. mana x%", 50);
            }

            var wMenu = Utilities.MainMenu.Add(new Menu("W", "W spell"));
            {
                wMenu.Separator("Combo");
                wMenu.Bool("ComboW", "Use W");
                wMenu.Separator("Harass");
                wMenu.Bool("HarassW", "Use W");
            }

            var eMenu = Utilities.MainMenu.Add(new Menu("E", "E spell"));
            {
                eMenu.Separator("Combo");
                eMenu.Bool("ComboE", "Use E");
                eMenu.Separator("Harass");
                eMenu.Bool("HarassE", "Use E");
                eMenu.Separator("Farm");
                eMenu.Bool("FarmE", "Use E");
                eMenu.Separator("Misc");
                eMenu.Bool("ToggleE", "Auto Disable E");
            }

            var rMenu = Utilities.MainMenu.Add(new Menu("R", "R spell"));
            {
                rMenu.Separator("Combo");
                rMenu.Bool("ComboR", "Use R");
                rMenu.Separator("Others");
                rMenu.Bool("KS", "Auto KSing kills");
                rMenu.Bool("Save", "Auto saving kills");
                rMenu.Slider("CountKS", "At how many champs KS-Ult", 1, 1, 5);
            }

            var comboMenu = Utilities.MainMenu.Add(new Menu("Utilities", "Utilities"));
            {
                comboMenu.Bool("NotifyPing", "On killable ping");
                comboMenu.Bool("LastAA", "Combine last hitting with AA's");
            }

            var drawMenu = Utilities.MainMenu.Add(new Menu("Draw", "Draw"));
            {
                drawMenu.Bool("Q", "Draws Q");
                drawMenu.Bool("W", "Draws W");
                drawMenu.Bool("E", "Draws E");
            }
        }
    }

    internal static class Core
    {
        /// <summary>
        ///     to Cast your Q custom for karthus. Call this method.
        /// </summary>
        /// <param name="target">target as <see cref="Obj_AI_Base" /></param>
        /// <param name="pred"></param>
        /// <param name="minManaPercent"></param>
        public static void CastQ(Obj_AI_Base target, Vector3 pred, int minManaPercent = 0)
        {
            if (!Utilities.Q.IsReady() || !(GetManaPercent() >= minManaPercent) || target == null ||
                !target.IsValidTarget(Utilities.Q.Range) || target.IsDead)
                return;

            Utilities.Q.Width = GetDynamicWWidth(target);

            if (pred.IsValid())
            {
                Utilities.Q.Cast(pred);
            }
        }

        private static float GetDynamicWWidth(Obj_AI_Base target)
        {
            return Math.Max(70, (1f - ObjectManager.Player.Distance(target)/Utilities.W.Range)*Utilities.W.Width);
        }

        /// <summary>
        ///     gets your current Q Damage with checking obj_AI_base around
        /// </summary>
        /// <param name="t"> your target</param>
        /// <returns> returns your damage in double</returns>
        public static double GetQDamage(Obj_AI_Base t)
        {
            var minions = GameObjects.EnemyMinions.Where(x => t.Distance(x.Position) <= Utilities.Q.Width/2);
            var damagetwo = new[] {40, 60, 80, 100, 120};
            var damageone = new[] {80, 120, 160, 200, 240};

            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();

            if (objAiMinions.Any() && Utilities.Q.Level >= 1)
            {
                return damagetwo[Utilities.Q.Level - 1] + 0.3*Utilities.Player.FlatMagicDamageMod;
            }

            if (Utilities.Q.Level >= 1)
            {
                return damageone[Utilities.Q.Level - 1] + 0.6*Utilities.Player.FlatMagicDamageMod;
            }

            return 0.0;
        }

        /// <summary>
        ///     Casts your W on the current unit.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="minManaPercent"></param>
        public static void CastW(Obj_AI_Base target, int minManaPercent = 0)
        {
            if (!Utilities.W.IsReady() || !(GetManaPercent() >= minManaPercent) || target == null)
                return;
            Utilities.W.Width = GetDynamicWWidth(target);
            Utilities.W.CastOnUnit(target);
        }

        /// <summary>
        ///     Getting the your current your Mana Percentage
        /// </summary>
        /// <returns>returns your percentage in float</returns>
        public static float GetManaPercent()
        {
            return Utilities.Player.Mana/Utilities.Player.MaxMana*100f;
        }

        /// <summary>
        ///     Getting your your passive Status
        /// </summary>
        /// <returns>returns if your passive is active - bool</returns>
        public static bool IsInPassiveForm()
        {
            return Utilities.Player.HasBuff("KarthusDeathDefiedBuff");
        }
    }
}