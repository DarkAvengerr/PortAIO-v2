// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="SVIrelia">
//      Copyright (c) SVIrelia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using Color = SharpDX.Color;


using EloBuddy; 
using LeagueSharp.Common; 
namespace SVIrelia
{
    internal class Program
    {
        /// <summary>
        ///     Declarations
        /// </summary>
        public static Spell Q, W, E, R;

        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;

        public static Items.Item
            Sheen = new Items.Item(2057),
            Trinity = new Items.Item(3078),
            Gauntlet = new Items.Item(3025),
            LichBane = new Items.Item(3100);

        /// <summary>
        ///     ObjectManager => Player
        /// </summary>
        public static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Run Irelia script on gameload
        /// </summary>
        /// <param name="args"></param>
        public static void Main()
        {
            Irelia(new EventArgs());
        }

        /// <summary>
        ///     Main Module
        /// </summary>
        /// <param name="args"></param>
        private static void Irelia(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Irelia")
                return;

            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 425);
            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(0.25f, 45, 1600, false, SkillshotType.SkillshotLine);

            Init();
            Chat.Print("<font color='#800040'>[SurvivorSeries] Irelia</font> <font color='#ff6600'>Loaded.</font>");
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += WUsage;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        ///     W Reset/Usage - Improved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void WUsage(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack() && args.Target is AIHeroClient &&
                (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) && W.IsReady())
                W.Cast();
        }

        /// <summary>
        ///     Initialize Extens
        /// </summary>
        public static void Init()
        {
            Menu = new Menu(":: SurvivorIrelia", "SurvivorIrelia", true);
            {
                var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "orbwalker"));
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                var combomenu = new Menu(":: Combo Settings", "combosettings");
                {
                    combomenu.AddItem(new MenuItem("q.combo", "Use (Q)")).SetValue(true);
                    combomenu.AddItem(new MenuItem("w.combo", "Use (W)")).SetValue(true);
                    combomenu.AddItem(new MenuItem("e.combo", "Use (E)")).SetValue(true);
                    combomenu.AddItem(new MenuItem("r.combo", "Use (R)")).SetValue(true);
                    /*combomenu.AddItem(
                        new MenuItem("rinfo", ":: R Info").SetTooltip(
                                "It'll cast R's at enemy once you've activated the ult once.")
                            .SetFontStyle(FontStyle.Bold, SharpDX.Color.DeepPink));
                    combomenu.AddItem(new MenuItem("combo.style", "(COMBO STYLE) -> ").SetValue(new StringList(new[] { "Normal", "Burst" })).SetTooltip("Soon TM!"))
                     .ValueChanged += (s, ar) =>
                     {
                         combomenu.Item("burst.enemy.killable.check").Show(ar.GetNewValue<StringList>().SelectedIndex == 0);
                     };
                    combomenu.AddItem(new MenuItem("burst.enemy.killable.check", "Only Enemy Killable")
                        .SetValue(true).SetFontStyle(System.Drawing.FontStyle.Bold))
                        .Show(combomenu.Item("combo.style").GetValue<StringList>().SelectedIndex == 0);*/
                    Menu.AddSubMenu(combomenu);
                }
                var laneclearmenu = new Menu(":: LaneClear Settings", "laneclearsettings");
                {
                    laneclearmenu.AddItem(new MenuItem("LCQ", "Use (Q)").SetValue(true));
                    laneclearmenu.AddItem(new MenuItem("LCW", "Use (W)").SetValue(true));
                    laneclearmenu.AddItem(new MenuItem("LCWSlider", "Minimum Minions in Range to (W)")
                        .SetValue(new Slider(2, 0, 10)));
                    laneclearmenu.AddItem(new MenuItem("LCE", "Use (E)").SetValue(false));
                    laneclearmenu.AddItem(
                        new MenuItem("LaneClearManaManager", "LaneClear Mana Manager (%)").SetValue(new Slider(30, 0,
                            100)));
                    Menu.AddSubMenu(laneclearmenu);
                }
                var jungleclearmenu = new Menu(":: Jungle Clear Settings", "jungleclearsettings");
                {
                    jungleclearmenu.AddItem(new MenuItem("JGCQ", "Use (Q)").SetValue(true));
                    jungleclearmenu.AddItem(new MenuItem("JGCW", "Use (W)").SetValue(true));
                    jungleclearmenu.AddItem(new MenuItem("JGCE", "Use (E)").SetValue(false));
                    jungleclearmenu.AddItem(
                        new MenuItem("JungleClearManaManager", "JungleClear Mana Manager (%)").SetValue(new Slider(30, 0,
                            100)));
                    Menu.AddSubMenu(jungleclearmenu);
                }
                var harassmenu = new Menu(":: Harass Settings", "harasssettings");
                {
                    harassmenu.AddItem(new MenuItem("HarassQ", "Use (Q)").SetValue(true));
                    harassmenu.AddItem(new MenuItem("HarassE", "Use (E)").SetValue(true));
                    harassmenu.AddItem(
                        new MenuItem("HarassManaManager", "Harass Mana Manager (%)").SetValue(new Slider(30, 0, 100)));
                    Menu.AddSubMenu(harassmenu);
                }
                var drawingmenu = new Menu(":: Drawings Menu", "drawingsmenu");
                {
                    drawingmenu.AddItem(new MenuItem("DrawAA", "Draw (AA) Range").SetValue(false));
                    drawingmenu.AddItem(new MenuItem("DrawQ", "Draw (Q) Range").SetValue(true));
                    drawingmenu.AddItem(new MenuItem("DrawR", "Draw (R) Range").SetValue(true));
                    drawingmenu.AddItem(new MenuItem("DrawStunnable", "Draw Stunnable?").SetValue(true));
                    drawingmenu.AddItem(
                        new MenuItem("QMinionDrawHelper", "Draw Q Circle on Minion if killable?").SetValue(true));
                    Menu.AddSubMenu(drawingmenu);
                }
                var miscmenu = new Menu(":: Misc Menu", "miscmenu");
                {
                    miscmenu.AddItem(
                        new MenuItem("HitChance", "Hit Chance").SetValue(
                            new StringList(new[] {"Medium", "High", "Very High"}, 1)));
                    miscmenu.AddItem(new MenuItem("KSCheck", "KS if Possible?").SetValue(true));
                    Menu.AddSubMenu(miscmenu);
                }

                #region Skin Changer

                var SkinChangerMenu =
                    Menu.AddSubMenu(new Menu(":: Skin Changer", "SkinChanger").SetFontStyle(FontStyle.Bold,
                        Color.Chartreuse));
                var SkinChanger =
                    SkinChangerMenu.AddItem(
                        new MenuItem("UseSkinChanger", ":: Use SkinChanger?").SetValue(true)
                            .SetFontStyle(FontStyle.Bold, Color.Crimson));
                var SkinID =
                    SkinChangerMenu.AddItem(
                        new MenuItem("SkinID", ":: Skin").SetValue(new Slider(6, 0, 6))
                            .SetFontStyle(FontStyle.Bold, Color.Crimson));
                SkinID.ValueChanged += (sender, eventArgs) =>
                {
                    if (!SkinChanger.GetValue<bool>())
                        return;

                    //Player.SetSkin(Player.BaseSkinName, eventArgs.GetNewValue<Slider>().Value);
                };

                #endregion

                #region DrawHPDamage

                var drawdamage = drawingmenu.AddSubMenu(new Menu(":: Draw Damage", "drawdamage"));
                {
                    var dmgAfterShave =
                        drawingmenu.AddItem(
                            new MenuItem("SurvivorIrelia.DrawComboDamage", "Draw Damage on Enemy's HP Bar").SetValue(
                                true));
                    var drawFill =
                        drawingmenu.AddItem(new MenuItem("SurvivorIrelia.DrawColour", "Fill Color", true).SetValue(
                            new Circle(true, System.Drawing.Color.Chartreuse)));
                    DrawDamage.DamageToUnit = CalculateDamage;
                    DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
                    DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
                    DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
                    dmgAfterShave.ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                        {
                            DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                        };

                    drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                }

                #endregion

                Menu.AddToMainMenu();
            }
        }

        /// <summary>
        ///     CalculateDamage Amount
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private static float CalculateDamage(AIHeroClient enemy)
        {
            float damage = 0;

            if (Q.Instance.IsReady() && (Player.Mana > Q.Instance.SData.Mana))
                damage += Q.GetDamage(enemy) + GetSheenDamage(enemy);
            if (W.Instance.IsReady() && (Player.Mana > W.Instance.SData.Mana))
                damage += W.GetDamage(enemy);
            if (E.Instance.IsReady() && (Player.Mana > E.Instance.SData.Mana))
                damage += E.GetDamage(enemy);
            if (HasRBuff() || R.Instance.IsReady())
                damage += R.GetDamage(enemy);
            damage += (float) Player.GetAutoAttackDamage(enemy);

            return damage;
        }

        /// <summary>
        ///     OnUpdate Event
        /// </summary>
        /// <param name="args"></param>
        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
            KSChecking();
        }

        private static void JungleClear()
        {
            if (Player.ManaPercent < Menu.Item("JungleClearManaManager").GetValue<Slider>().Value)
                return;

            var jgcq = Menu.Item("JGCQ").GetValue<bool>();
            var jgcw = Menu.Item("JGCW").GetValue<bool>();
            var jgce = Menu.Item("JGCE").GetValue<bool>();

            var mob =
                MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (mob == null)
                return;

            if (jgcq && Q.Instance.IsReady())
                Q.CastOnUnit(mob);
            if (jgce && E.Instance.IsReady())
                E.CastOnUnit(mob);
            if (jgcw && W.Instance.IsReady())
                W.Cast();
        }

        /// <summary>
        ///     Drawings
        /// </summary>
        /// <param name="args"></param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (Menu.Item("DrawAA").GetValue<bool>())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Orbwalking.GetRealAutoAttackRange(null),
                    System.Drawing.Color.BlueViolet);
            if (Menu.Item("DrawQ").GetValue<bool>())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Chartreuse);
            if (Menu.Item("DrawR").GetValue<bool>() && (R.Level > 0) && R.Instance.IsReady())
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.DeepPink);

            if (Menu.Item("QMinionDrawHelper").GetValue<bool>())
                foreach (
                    var creature in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            x =>
                                x.Name.ToLower().Contains("minion") && x.IsHPBarRendered && x.IsValidTarget(1000) &&
                                (x.Health < Q.GetDamage(x) + GetSheenDamage(x)) && x.IsEnemy))
                    Render.Circle.DrawCircle(creature.Position, 35, System.Drawing.Color.Chartreuse);

            if (!Menu.Item("DrawStunnable").GetValue<bool>())
                return;

            foreach (var enemy in
                ObjectManager.Get<AIHeroClient>().Where(x => Stunnable(x) && x.IsValidTarget()))
            {
                var drawPos = Drawing.WorldToScreen(enemy.Position);
                var textSize = Drawing.GetTextEntent(("Stunnable"), 15);
                Drawing.DrawText(drawPos.X - textSize.Width/2f, drawPos.Y, System.Drawing.Color.DeepPink, "Stunnable");
            }
        }

        /// <summary>
        ///     Player has RBuff :?
        /// </summary>
        /// <returns></returns>
        public static bool HasRBuff() => Player.HasBuff("ireliatranscendentbladesspell");

        /// <summary>
        ///     Stunnable?
        /// </summary>
        /// <param></param>
        /// <returns></returns>
#pragma warning disable 1573
        public static bool Stunnable(AIHeroClient enemy)
#pragma warning restore 1573
        {
            if (enemy == null) throw new ArgumentNullException(nameof(enemy));
            return enemy.HealthPercent > Player.HealthPercent;
        }

        /// <summary>
        ///     Custom Made GetSheenDamage function -> Including all Sheen buildable items.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float GetSheenDamage(Obj_AI_Base target)
        {
            float totalDamage = 0;

            // Sheen
            if (Items.HasItem(3057, Player))
            {
                if (Items.CanUseItem(3057))
                    totalDamage +=
                        (float) Player.CalcDamage(target, Damage.DamageType.Physical, Player.TotalAttackDamage);
            }
            else if (Items.HasItem(3025, Player)) // Iceborn
            {
                if (Items.CanUseItem(3025))
                    totalDamage +=
                        (float) Player.CalcDamage(target, Damage.DamageType.Physical, Player.TotalAttackDamage);
            }
            else if (Items.HasItem(3078, Player)) // Trinity
            {
                if (Items.CanUseItem(3078))
                    totalDamage +=
                        (float) Player.CalcDamage(target, Damage.DamageType.Physical, Player.TotalAttackDamage*2);
            }
            else if (Items.HasItem(3100, Player)) // Lich Bane
            {
                if (Items.CanUseItem(3100))
                    totalDamage +=
                        (float)
                        Player.CalcDamage(target, Damage.DamageType.Magical,
                            75/Player.TotalAttackDamage*100 + 50/Player.TotalMagicalDamage*100);
            }
            return totalDamage;
        }

        private static void KSChecking()
        {
            if (Menu.Item("KSCheck").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                if ((target == null) || !target.IsValidTarget())
                    return;

                if ((target.Health < Q.GetDamage(target) + GetSheenDamage(target)) && Q.IsReady())
                    Q.CastOnUnit(target);

                if ((target.Health < E.GetDamage(target)) && E.IsReady())
                    E.CastOnUnit(target);
            }
        }

        /// <summary>
        ///     Combo Mode
        /// </summary>
        public static void Combo()
        {
            var useq = Menu.Item("q.combo").GetValue<bool>();
            var usew = Menu.Item("w.combo").GetValue<bool>();
            var usee = Menu.Item("e.combo").GetValue<bool>();
            var useR = Menu.Item("r.combo").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            //var targetextend = TargetSelector.GetTarget();
            if ((target == null) || !target.IsValidTarget())
                return;

            //Chat.Print("Q Damage: " + Q.GetDamage(target) + " Sheen Damage:" + GetSheenDamage(target));
            //var distancebetweenmeandtarget = ObjectManager.Player.ServerPosition.Distance(target.ServerPosition);
            /*if (Q.IsReady() && target.Distance(Player.Position) > 650)
            {
                var gapclosingMinion = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 650 &&
                    m.IsEnemy && m.ServerPosition.Distance(target.ServerPosition) < distancebetweenmeandtarget && m.Health > 1 && m.Health < Q.GetDamage(m) + GetSheenDamage(m)).OrderBy(m => m.Position.Distance(target.ServerPosition)).FirstOrDefault();
                if (gapclosingMinion != null)
                {
                    Q.Cast(gapclosingMinion);
                }
            }*/

            if (target.IsValidTarget(Q.Range) && (target.Distance(Player.ServerPosition) > E.Range) && Q.IsReady() &&
                useq)
            {
                Q.CastOnUnit(target);
                if (E.Instance.IsReady() && target.IsValidTarget(E.Range) && usee)
                    E.CastOnUnit(target);
                if (W.Instance.IsReady() && usew && target.IsValidTarget(Player.AttackRange))
                    // Yes lads E.Range is intentionally here :gosh:
                    W.Cast();
                if (target.IsValidTarget(R.Range) && useR)
                {
                    var pred = R.GetPrediction(target);
                    if ((pred.Hitchance >= HitChance.High) && HasRBuff())
                    {
                        if (OktwCommon.CollisionYasuo(Player.Position, pred.CastPosition))
                            return;
                        R.Cast(pred.CastPosition);
                    }
                }
            }
            else if (target.IsValidTarget(E.Range))
            {
                if (E.Instance.IsReady() && usee && target.IsValidTarget(E.Range))
                    E.CastOnUnit(target);
                if (W.Instance.IsReady() && usew && target.IsValidTarget(Player.AttackRange))
                    // Yes lads E.Range is intentionally here :gosh:
                    W.Cast();
                if (R.Instance.IsReady() && useR && target.IsValidTarget(R.Range))
                {
                    var pred = R.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        if (OktwCommon.CollisionYasuo(Player.Position, pred.CastPosition))
                            return;
                        R.Cast(pred.CastPosition);
                    }
                }
                if ((target.Distance(Player.ServerPosition) > E.Range) && useq && target.IsValidTarget(Q.Range))
                    Q.CastOnUnit(target);
            }
        }

        /// <summary>
        ///     LaneClear Mode
        /// </summary>
        public static void LaneClear()
        {
            if (Player.ManaPercent < Menu.Item("LaneClearManaManager").GetValue<Slider>().Value)
                return;
            var lcq = Menu.Item("LCQ").GetValue<bool>();
            var lcw = Menu.Item("LCW").GetValue<bool>();
            var lce = Menu.Item("LCE").GetValue<bool>();
            var lcwslider = Menu.Item("LCWSlider").GetValue<Slider>().Value;

            if (lcq && Q.Instance.IsReady())
            {
                var minionq =
                    MinionManager.GetMinions(Player.Position, Q.Range)
                        .FirstOrDefault(x => Q.GetDamage(x) + GetSheenDamage(x) > x.Health + 30);

                if (minionq != null)
                    Q.Cast(minionq);
            }

            var minionwe = Cache.GetMinions(Player.Position, 275);

            if (minionwe == null)
                return;

            foreach (var minion in minionwe)
            {
                if (W.Instance.IsReady() && lcw && (minion.HealthPercent > 5) && (minionwe.Count > lcwslider))
                    W.Cast();
                if (E.Instance.IsReady() && lce && (minion.Health < E.GetDamage(minion)))
                    E.CastOnUnit(minion);
            }
        }

        /// <summary>
        ///     Harass Mode
        /// </summary>
        public static void Harass()
        {
            if (Player.ManaPercent < Menu.Item("HarassManaManager").GetValue<Slider>().Value)
                return;

            var harassq = Menu.Item("HarassQ").GetValue<bool>();
            var harasse = Menu.Item("HarassE").GetValue<bool>();

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if ((target == null) || !target.IsValidTarget())
                return;

            if (Q.IsReady() && harassq)
            {
                Q.CastOnUnit(target);
                if (harasse && E.IsReady())
                {
                    E.CastOnUnit(target);
                    Orbwalker.ForceTarget(target);
                }
                else
                {
                    Orbwalker.ForceTarget(target);
                }
            }

            if (harasse && E.IsReady() && !Q.IsReady())
                if (target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target);
                    Orbwalker.ForceTarget(target);
                }
        }
    }
}