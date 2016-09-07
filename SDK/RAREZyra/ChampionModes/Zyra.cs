// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Zyra.cs" company="Marvin Piekarek">
//  Copyright (c) 2016 RAREZyra. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RAREZyra.ChampionModes
{
    #region directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using LeagueSharp;
    using LeagueSharp.Data;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Menu = LeagueSharp.SDK.UI.Menu;
    using SpellDatabase = LeagueSharp.Data.DataTypes.SpellDatabase;
    using Utilities = Utilities;

    #endregion

    public class Zyra
    {
        #region Properties

        public static readonly Render.Text Text = new Render.Text(
            0, 0, "", 12, new ColorBGRA(255, 255, 255, 255), "Verdana");

        /// <summary>
        ///     Class to refer on all plants/seeds
        /// </summary>
        public class Plant
        {
            public int Count { get; set; }
            public Vector3 Place { get; set; }

            public Plant(int count, Vector3 pos)
            {
                Count = count;
                Place = pos;
            }
        }

        #endregion

        #region method

        /// <summary>
        ///     Initialization for Karthus.
        ///     Should be called at first.
        /// </summary>
        public void Init()
        {
            #region skills setup

            // initializing all skills with all attributes
            //Q
            var q =
                Data.Get<SpellDatabase>()
                    .Spells.Single(spell => (spell.ChampionName == "Zyra") && (spell.Slot == SpellSlot.Q));
            Utilities.Q = new Spell(SpellSlot.Q, q.Range)
                .SetSkillshot(q.Delay, q.Width, q.MissileSpeed, q.CollisionObjects.Any(), SkillshotType.SkillshotCircle);

            //W
            var w =
                Data.Get<SpellDatabase>()
                    .Spells.Single(spell => (spell.ChampionName == "Zyra") && (spell.Slot == SpellSlot.W));
            Utilities.W = new Spell(SpellSlot.W, w.Range);

            //E
            var e =
                Data.Get<SpellDatabase>()
                    .Spells.Single(spell => (spell.ChampionName == "Zyra") && (spell.Slot == SpellSlot.E));
            Utilities.E = new Spell(SpellSlot.E, 1100)
                .SetSkillshot(e.Delay, e.Width, e.MissileSpeed, e.CollisionObjects.Any(), SkillshotType.SkillshotLine);

            //R
            var r =
                Data.Get<SpellDatabase>()
                    .Spells.Single(spell => (spell.ChampionName == "Zyra") && (spell.Slot == SpellSlot.R));
            Utilities.R = new Spell(SpellSlot.R, 700)
                .SetSkillshot(r.Delay, r.Width, r.MissileSpeed, r.CollisionObjects.Any(), SkillshotType.SkillshotCircle);

            // setting up there damage type
            Utilities.Q.DamageType = Utilities.W.DamageType = Utilities.E.DamageType = Utilities.R.DamageType =
                DamageType.Magical;
            // setting up the standart hitchange for Q
            Utilities.Q.MinHitChance = HitChance.Medium;
            Utilities.E.MinHitChance = HitChance.Medium;
            Utilities.R.MinHitChance = HitChance.Medium;

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
            #region DrawingSkills

            if (Utilities.Player.IsDead)
                return;

            if (Utilities.MainMenu["Draw"]["Q"] && (Utilities.Q.Level > 0) && Utilities.Q.IsReady())
                Render.Circle.DrawCircle(Utilities.Player.Position, Utilities.Q.Range, Color.AntiqueWhite, 2);

            if (Utilities.MainMenu["Draw"]["W"] && (Utilities.W.Level > 0) && Utilities.W.IsReady())
                Render.Circle.DrawCircle(Utilities.Player.Position, Utilities.W.Range, Color.ForestGreen, 2);

            if (Utilities.MainMenu["Draw"]["E"] && (Utilities.E.Level > 0) && Utilities.E.IsReady())
                Render.Circle.DrawCircle(Utilities.Player.Position, Utilities.E.Range, Color.CornflowerBlue, 2);

            if (Utilities.MainMenu["Draw"]["R"] && (Utilities.R.Level > 0) && Utilities.R.IsReady())
                Render.Circle.DrawCircle(Utilities.Player.Position, Utilities.R.Range, Color.Tomato, 2);

            if (Utilities.MainMenu["Draw"]["Plants"])
                foreach (var obj in ObjectManager.Get<GameObject>().Where(x => x.Position.DistanceToPlayer() < 2000))
                {
                    if (!(obj is Obj_AI_Minion)) continue;

                    if ((obj as Obj_AI_Minion).CharData.BaseSkinName == "zyraseed")
                        Render.Circle.DrawCircle((obj as Obj_AI_Minion).Position, 50, Color.Tomato, 2);
                }

            #endregion
        }


        /// <summary>
        ///     Main ticking rotation which decides what method will be checked during the game.
        /// </summary>
        /// <param name="args">parameter that are given by the process itself. (not needed yet)</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            Utilities.E.Range = Utilities.MainMenu["Utilities"]["ERange"];

            if (Utilities.MainMenu["E"]["EFC"].GetValue<MenuKeyBind>().Active)
                ForceESpell();

            if ((Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear) ||
                (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LastHit))
                Variables.Orbwalker.AttackState = Utilities.MainMenu["Utilities"]["AA"];

            if (Utilities.Q.Level >= 1)
                HandleQ(Variables.Orbwalker.ActiveMode);
            if (Utilities.E.Level >= 1)
                HandleE(Variables.Orbwalker.ActiveMode);
            if (Utilities.R.Level >= 1)
                HandleR(Variables.Orbwalker.ActiveMode);
        }

        public static void ForceESpell()
        {
            Variables.Orbwalker.Move(Game.CursorPos);
            var target = Variables.TargetSelector.GetTarget(Utilities.E, false);

            if (Utilities.E.IsReady() && target.IsValid())
                Utilities.E.Cast(target);
        }

        /// <summary>
        ///     spikes shot
        /// </summary>
        /// <param name="cMode">your current orbwalker mode</param>
        public static void HandleQ(OrbwalkingMode cMode)
        {
            if ((cMode == OrbwalkingMode.LaneClear) && Utilities.MainMenu["Q"]["FarmQ"])
            {
                var minions =
                    GameObjects.EnemyMinions.Where(x => (x.DistanceToPlayer() < Utilities.Q.Range) && x.IsValid)
                        .ToList();

                if (!minions.Any()) return;

                var farmloc = Core.GetBestQPos(minions);

                if (farmloc.IsValid())
                {
                    if (Utilities.W.IsReady()) HandleW(cMode, "Q", farmloc);
                    Utilities.Q.Cast(farmloc);
                }
            }
            else if ((cMode == OrbwalkingMode.LastHit) && Utilities.MainMenu["Q"]["LastQ"])
            {
                var minions =
                    GameObjects.EnemyMinions.Where(
                        x =>
                            (x.DistanceToPlayer() < Utilities.Q.Range) && x.IsValid &&
                            (x.Health < Core.CustomQDamage())).ToList();

                if (!minions.Any()) return;

                var farmloc = Core.GetBestQPos(minions);

                if (farmloc.IsValid())
                    Utilities.Q.Cast(farmloc);
            }
            else if ((cMode == OrbwalkingMode.Combo) && Utilities.MainMenu["Q"]["ComboQ"])
            {
                var heroes =
                    GameObjects.EnemyHeroes.Where(x => (x.DistanceToPlayer() < Utilities.Q.Range) && x.IsValid).ToList();

                if (!heroes.Any()) return;

                var loc = Core.GetBestQPos(null, heroes);

                if (loc.IsValid())
                {
                    if (Utilities.W.IsReady()) HandleW(Variables.Orbwalker.ActiveMode, "Q", loc);
                    Utilities.Q.Cast(loc);
                }
            }
            else if ((cMode == OrbwalkingMode.Hybrid) && Utilities.MainMenu["Q"]["HybridQ"])
            {
                var loc = Vector2.Zero;
                var heroes =
                    GameObjects.EnemyHeroes.Where(x => (x.DistanceToPlayer() < Utilities.Q.Range) && x.IsValid).ToList();
                if (heroes.Any())
                    loc = Core.GetBestQPos(null, heroes);

                var minions =
                    GameObjects.EnemyMinions.Where(
                        x =>
                            (x.DistanceToPlayer() < Utilities.Q.Range) && x.IsValid &&
                            (x.Health < Core.CustomQDamage())).ToList();
                if (!minions.Any()) return;

                var farmloc = Core.GetBestQPos(minions);

                if (minions.Any() && farmloc.IsValid() && (minions.Count >= 3))
                {
                    Utilities.Q.Cast(farmloc);
                }
                else if (heroes.Any() && loc.IsValid())
                {
                    if (Utilities.W.IsReady() && Utilities.MainMenu["W"]["HybridW"])
                        HandleW(Variables.Orbwalker.ActiveMode, "Q", loc);
                    Utilities.Q.Cast(loc);
                }
            }
        }

        /// <summary>
        ///     seeds handler
        /// </summary>
        /// <param name="cMode">your current orbwalker mode</param>
        /// <param name="spell">spell where its coming from.</param>
        /// <param name="pos">pos where it should be casted.</param>
        public static void HandleW(OrbwalkingMode cMode, string spell, Vector2 pos)
        {
            if (Utilities.W.Level == 0) return;

            if ((cMode == OrbwalkingMode.LaneClear) && Utilities.MainMenu["W"]["FarmW"])
            {
                if ((spell == "Q") && pos.IsValid() && !pos.IsUnderEnemyTurret())
                    DelayAction.Add(10, () => { Utilities.W.Cast(pos); });
            }
            else if ((cMode == OrbwalkingMode.Combo) && Utilities.MainMenu["W"]["ComboW"])
            {
                if ((spell == "Q") && pos.IsValid() && !pos.IsUnderEnemyTurret())
                    DelayAction.Add(10, () =>
                    {
                        Utilities.W.Cast(pos);
                        if (Utilities.W.IsReady())
                            Utilities.W.Cast(pos);
                    });
            }
        }

        /// <summary>
        ///     binding
        /// </summary>
        /// <param name="cMode">your current orbwalker mode</param>
        public static void HandleE(OrbwalkingMode cMode)
        {
            if ((cMode == OrbwalkingMode.LaneClear) && Utilities.MainMenu["E"]["FarmE"])
            {
                var minions =
                    GameObjects.EnemyMinions.Where(x => x.IsValid && (x.DistanceToPlayer() < Utilities.E.Range))
                        .ToList();

                if (!minions.Any()) return;

                var loc = Core.GetBestEPos(minions);

                if (loc.IsValid() && minions.Any(x => !x.IsDead))
                    Utilities.E.Cast(loc);
            }
            else if ((cMode == OrbwalkingMode.LastHit) && Utilities.MainMenu["E"]["LastE"])
            {
                var minions =
                    GameObjects.EnemyMinions.Where(
                        x =>
                            x.IsValid && (x.DistanceToPlayer() < Utilities.E.Range) &&
                            (x.Health < Core.CustomEDamage())).ToList();

                if (!minions.Any()) return;

                var loc = Core.GetBestEPos(minions);

                if (loc.IsValid() && minions.Any(x => !x.IsDead))
                    Utilities.E.Cast(loc);
            }
            else if ((cMode == OrbwalkingMode.Hybrid) && Utilities.MainMenu["E"]["LastE"])
            {
                var minions =
                    GameObjects.EnemyMinions.Where(
                        x =>
                            x.IsValid && (x.DistanceToPlayer() < Utilities.E.Range) &&
                            (x.Health < Core.CustomEDamage())).ToList();

                if (!minions.Any()) return;

                var loc = Core.GetBestEPos(minions);

                if (loc.IsValid() && minions.Any(x => !x.IsDead))
                    Utilities.E.Cast(loc);
            }
            else if ((cMode == OrbwalkingMode.Combo) && Utilities.MainMenu["E"]["ComboE"])
            {
                var heroes =
                    GameObjects.EnemyHeroes.Where(x => x.IsValid && (x.DistanceToPlayer() < Utilities.E.Range)).ToList();

                if (!heroes.Any()) return;

                var loc = Core.GetBestEPos(null, heroes);

                if (loc.IsValid())
                    Utilities.E.Cast(loc);
            }
        }

        /// <summary>
        ///     Handling casting the ultimate
        /// </summary>
        /// <param name="cMode">your current orbwalker mode</param>
        public static void HandleR(OrbwalkingMode cMode)
        {
            if ((cMode == OrbwalkingMode.Combo) && Utilities.MainMenu["R"]["ComboR"])
            {
                var heroes = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Utilities.R.Range)).ToList();

                if (!heroes.Any()) return;

                if (heroes.Count == 1)
                    heroes = heroes.Where(x => x.Health < Core.CustomRDamage()).ToList();

                var pos = Core.GetBestRPos(heroes);

                if (pos != Vector2.Zero)
                    Utilities.R.Cast(pos);
            }
        }

        /// <summary>
        ///     Initialize the champion menu for zyra.
        /// </summary>
        public static void ChampionMenu()
        {
            /*var modesSettings = Utilities.MainMenu.Add(new Menu("Modes", "Modes"));
            {
                modesSettings.List("LaneMode", "LaningModes", new[] {"passive", "aggressiv", "support"});
            }*/

            var qMenu = Utilities.MainMenu.Add(new Menu("Q", "Q spell"));
            {
                qMenu.Separator("Combo");
                qMenu.Bool("ComboQ", "Use Q");
                qMenu.Separator("Hybrid");
                qMenu.Bool("HybridQ", "Use Q");
                qMenu.Separator("Farm");
                qMenu.Bool("FarmQ", "Use Q");
                qMenu.Separator("LastHit");
                qMenu.Bool("LastQ", "Use Q");
            }

            var wMenu = Utilities.MainMenu.Add(new Menu("W", "W spell"));
            {
                wMenu.Separator("Combo");
                wMenu.Bool("ComboW", "Use W");
                wMenu.Separator("Hybrid");
                wMenu.Bool("HybridW", "Use W");
                wMenu.Separator("Farm");
                wMenu.Bool("FarmW", "Use W");
            }

            var eMenu = Utilities.MainMenu.Add(new Menu("E", "E spell"));
            {
                eMenu.Separator("Combo");
                eMenu.Bool("ComboE", "Use E");
                eMenu.Separator("Hybrid");
                eMenu.Bool("HybridE", "Use E");
                eMenu.Separator("Farm");
                eMenu.Bool("FarmE", "Use E");
                eMenu.Slider("CountFarm", "minions to be casted", 3, 1, 5);
                eMenu.Separator("LastHit");
                eMenu.Bool("LastE", "Use E");
                eMenu.Separator("Misc");
                eMenu.KeyBind("EFC", "Force E cast", Keys.H);
            }

            var rMenu = Utilities.MainMenu.Add(new Menu("R", "R spell"));
            {
                rMenu.Separator("Combo");
                rMenu.Bool("ComboR", "Use R");
                rMenu.Separator("Others");
                rMenu.Slider("Count", "Champs to Cast", 2, 1, 5);
            }

            var comboMenu = Utilities.MainMenu.Add(new Menu("Utilities", "Utilities"));
            {
                comboMenu.Bool("AA", "Using AA in LastHit or LaneClear");
                comboMenu.Slider("ERange", "Customize E Range", 980, 800, 1100);
            }

            var drawMenu = Utilities.MainMenu.Add(new Menu("Draw", "Draw"));
            {
                drawMenu.Bool("Q", "Draws Q");
                drawMenu.Bool("W", "Draws W");
                drawMenu.Bool("E", "Draws E");
                drawMenu.Bool("R", "Draws R");
                drawMenu.Bool("Plants", "Draws Plants");
            }
        }

        #endregion
    }

    public class Core
    {
        #region Core fnct

        /// <summary>
        ///     Getting the your current your Mana Percentage
        /// </summary>
        /// <returns>returns your percentage in float</returns>
        public static float GetManaPercent()
        {
            return Utilities.Player.Mana/Utilities.Player.MaxMana*100f;
        }

        /// <summary>
        ///     gets your current Q Damage with checking obj_AI_base around
        /// </summary>
        /// <returns> returns your damage in double</returns>
        public static double CustomQDamage()
        {
            var qdamage = new[] {60, 90, 120, 150, 180};

            if (Utilities.Q.Level >= 1)
                return (qdamage[Utilities.Q.Level - 1] + Utilities.Player.FlatMagicDamageMod*0.55)*0.90;

            return 0.0;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static double CustomEDamage()
        {
            var damageE = new[] {60, 95, 130, 165, 200};

            if (Utilities.E.Level >= 1)
                return (damageE[Utilities.E.Level - 1] + Utilities.Player.FlatMagicDamageMod*0.50)*0.90;

            return 0.0;
        }

        public static double CustomRDamage()
        {
            var damageR = new[] {180, 265, 350};

            if (Utilities.R.Level >= 1)
                return (damageR[Utilities.E.Level - 1] + Utilities.Player.FlatMagicDamageMod*0.70)*0.90;

            return 0.0;
        }

        public static Vector2 GetBestQPos(List<Obj_AI_Minion> m = null, List<AIHeroClient> h = null)
        {
            if (h != null)
            {
                var planties = new Dictionary<Obj_AI_Base, Zyra.Plant>();

                foreach (var hero in h.Where(x => x.IsValidTarget(Utilities.Q.Range)))
                {
                    var temppred = Utilities.Q.GetPrediction(hero, true);
                    var count = temppred.AoeTargetsHitCount +
                                GetPlants(temppred.CastPosition, Utilities.Q.Range).Count;
                    planties.Add(hero, new Zyra.Plant(count, temppred.CastPosition));
                }

                var sorted = (from kv in planties orderby kv.Value.Count select kv).ToList();

                if (sorted.Any())
                    return sorted.Last().Value.Place.ToVector2();

                return Vector2.Zero;
            }

            if ((m != null) && (m.Count >= 4))
            {
                var planites = GetPlants(Utilities.Player.Position, Utilities.Q.Range);
                m.AddRange(planites);
                var pos = Utilities.Q.GetCircularFarmLocation(m);
                return pos.Position;
            }

            return Vector2.Zero;
        }

        public static Vector2 GetBestEPos(List<Obj_AI_Minion> m = null, List<AIHeroClient> h = null)
        {
            if (h != null)
            {
                h = h.Where(x => x.IsValidTarget(Utilities.E.Range)).ToList();
                if (!h.Any()) return new Vector2(0, 0); // thanks media.

                var temp = Utilities.E.GetPrediction(h.FirstOrDefault(), true);
                return temp.CastPosition.ToVector2();
            }

            if ((m != null) && (m.Count >= Utilities.MainMenu["E"]["CountFarm"]))
            {
                var planites = GetPlants(Utilities.Player.Position, Utilities.E.Range);
                m.AddRange(planites);
                var pos = Utilities.E.GetLineFarmLocation(m);
                return pos.Position;
            }

            return Vector2.Zero;
        }

        public static Vector2 GetBestRPos(List<AIHeroClient> h = null)
        {
            if ((h != null) && (h.Count > 1))
            {
                var planties = new Dictionary<Obj_AI_Base, Zyra.Plant>();

                foreach (var hero in h.Where(x => x.IsValidTarget(Utilities.R.Range)))
                {
                    var temppred = Utilities.R.GetPrediction(hero, true);
                    if (temppred.AoeTargetsHitCount >= Utilities.MainMenu["R"]["Count"])
                    {
                        var count = temppred.AoeTargetsHitCount +
                                    GetPlants(temppred.CastPosition, Utilities.R.Range).Count;
                        planties.Add(hero, new Zyra.Plant(count, temppred.CastPosition));
                    }
                }

                var sorted =
                    (from plant in planties orderby plant.Value.Count select plant).ToList();

                if (sorted.Any())
                    return sorted.Last().Value.Place.ToVector2();

                return Vector2.Zero;
            }

            return h != null
                ? Utilities.R.GetPrediction(h.FirstOrDefault(), true).CastPosition.ToVector2()
                : Vector2.Zero;
        }

        public static List<Obj_AI_Minion> GetPlants(Vector3 pos, float range)
        {
            var list = new List<Obj_AI_Minion>();

            foreach (var obj in ObjectManager.Get<GameObject>().Where(x => x.Position.Distance(pos) < range))
            {
                if (!(obj is Obj_AI_Minion)) continue;

                if ((obj as Obj_AI_Minion).CharData.BaseSkinName == "zyraseed")
                    list.Add(obj as Obj_AI_Minion);
            }

            return list;
        }

        #endregion
    }
}