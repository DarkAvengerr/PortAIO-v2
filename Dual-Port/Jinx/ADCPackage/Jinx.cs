using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;

using EloBuddy; namespace ADCPackage.Plugins
{
    /*
    CREDITS TO:
       
        - Sebby - OnProcessSpell stuff, CantMove code & spell Values because Im lazy & blitz idea
        - xcsoft - R Damage, R Collision, TP/Zhonyas W/E
        - 
    */

    internal static class Jinx
    {
        public static Spell Q, W, E, R;

        private static bool EnemyInRange
            => TargetSelector.GetTarget(525 + Player.BoundingRadius, TargetSelector.DamageType.Physical) != null;

        public static bool FishBones => Player.HasBuff("JinxQ");
        private static AIHeroClient Player => ObjectManager.Player;

        public static void Load()
        {
            Chat.Print("[<font color='#F8F46D'>ADC Package</font>] by <font color='#79BAEC'>God</font> - <font color='#FFFFFF'>Jinx</font> loaded");
            CustomOrbwalker.BeforeAttack += CustomOrbwalker_BeforeAttack;
            Game.OnUpdate += PermaActive;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;

            InitSpells();
            InitMenu();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Config.Item("draw.w").IsActive()) return;
            switch (W.IsReady())
            {
                case true:
                    Drawing.DrawCircle(Player.Position, W.Range, Color.Sienna);
                    break;
                case false:
                    Drawing.DrawCircle(Player.Position, W.Range, Color.Maroon);
                    break;
            }
        }

        private static void CustomOrbwalker_BeforeAttack(CustomOrbwalker.BeforeAttackEventArgs args)
        {
            if (args.Target.Type == GameObjectType.AIHeroClient)
            {
                var target = (AIHeroClient) args.Target;
                if (target == null)
                {
                    return;
                }

                if (target.Health <= Player.GetAutoAttackDamage(target, true) &&
                    target.Health >= Player.GetAutoAttackDamage(target))
                {
                    if (!FishBones && Q.IsReady())
                    {
                        Q.Cast();
                    }
                }

                if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("q.aoe").GetValue<Slider>().Value != 6)
                {
                    if (target.CountEnemiesInRange(150) >=
                        Menu.Config.SubMenu("adcpackage.jinx")
                            .SubMenu("combo")
                            .Item("q.aoe")
                            .GetValue<Slider>()
                            .Value)
                    {
                        if (!FishBones && Q.IsReady())
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsValidTarget(E.Range) && E.IsReady())
            {
                if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("agc.e").GetValue<bool>())
                {
                    E.Cast(gapcloser.End);
                }
            }

            if (gapcloser.Sender.IsValidTarget(W.Range) && W.IsReady())
            {
                if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("agc.w").GetValue<bool>())
                {
                    W.Cast(gapcloser.Sender);
                }
            }
        }

        public static bool ShouldUseE(string spellName)
        {
            switch (spellName)
            {
                case "ThreshQ":
                    return true;
                case "KatarinaR":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "GalioIdolOfDurand":
                    return true;
                case "LuxMaliceCannon":
                    return true;
                case "MissFortuneBulletTime":
                    return true;
                case "RocketGrabMissile":
                    return true;
                case "CaitlynPiltoverPeacemaker":
                    return true;
                case "EzrealTrueshotBarrage":
                    return true;
                case "InfiniteDuress":
                    return true;
                case "VelkozR":
                    return true;
            }
            return false;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (E.IsReady() && ShouldUseE(args.SData.Name) && sender.IsValidTarget(E.Range))
            {
                E.Cast(sender.Position);
            }
        }

        private static void PermaActive(EventArgs args)
        {
            if (E.IsReady() || W.IsReady())
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range))
                            .OrderBy(TargetSelector.GetPriority))
                {
                    if (E.IsReady() && E.IsInRange(enemy)) /* e Logic */
                    {
                        if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("cc.e").GetValue<bool>())
                        {
                            if (enemy.HasBuff("RocketGrab") || enemy.HasBuff("rocketgrab2"))
                            {
                                var blitzcrank = HeroManager.Allies.FirstOrDefault(a => a.ChampionName == "Blitzcrank");

                                if (blitzcrank != null)
                                {
                                    E.Cast(blitzcrank.Position.Extend(enemy.Position, 30));
                                }
                                return;
                            }

                            if (CantMove(enemy) && !enemy.HasBuff("RocketGrab") && !enemy.HasBuff("rocketgrab2"))
                            {
                                E.Cast(enemy.Position);
                                return;
                            }

                            var pred = E.GetPrediction(enemy);
                            if (pred.Hitchance >= HitChance.Dashing)
                            {
                                E.Cast(pred.CastPosition);
                                return;
                            }
                        }

                        if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("tp.e").GetValue<bool>())
                        {
                            CastWithExtraTrapLogic(E);
                            return;
                        }
                    }

                    if (W.IsReady())
                    {
                        if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("ks.w").GetValue<bool>())
                        {
                            if (enemy.Health <= W.GetDamage(enemy))
                            {
                                W.Cast(enemy);
                                return;
                            }
                        }

                        if (enemy.HasBuff("RocketGrab") || enemy.HasBuff("rocketgrab2"))
                        {
                            var blitzcrank = HeroManager.Allies.FirstOrDefault(a => a.ChampionName == "Blitzcrank");

                            if (blitzcrank != null)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(250, () => W.Cast(enemy));
                            }
                            return;
                        }


                        if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("cc.w").GetValue<bool>())
                        {
                            if (CantMove(enemy))
                            {
                                W.Cast(enemy);
                                return;
                            }
                        }
                    }
                }
            }
            //if (W.IsReady())
            //{
            //    AutoW();
            //}

            if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("ks.r").GetValue<bool>())
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(e => e.Distance(Player) <= 2500)
                            .Where(enemy => R.IsReady() && enemy.IsValidTarget(R.Range)))
                {
                    var pred = R.GetPrediction(enemy);

                    if (W.IsReady() && W.GetDamage(enemy) >= enemy.Health && W.IsInRange(enemy) &&
                        W.GetPrediction(enemy).Hitchance >= HitChance.VeryHigh)
                    {
                        return;
                    }

                    if (GetRDamage(enemy) > enemy.Health && !HitsEnemyInTravel(enemy))
                    {
                        R.Cast(pred.CastPosition);
                        return;
                    }
                }
            }

            // fix knockup/knockback/stun auto e/w stuff
        }

        private static bool CantMove(AIHeroClient target)
        {
            return (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                    target.HasBuffOfType(BuffType.Knockup) ||
                    target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                    target.HasBuffOfType(BuffType.Knockback) ||
                    target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                    target.IsStunned || target.IsChannelingImportantSpell() || target.MoveSpeed <= 50f);
        }

        //private static void AutoE()
        //{
        //    foreach (
        //        var enemy in
        //            HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(E.Range))
        //                .OrderBy(TargetSelector.GetPriority))
        //    {
        //        if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("cc.e").GetValue<bool>())
        //        {
        //            if (enemy.HasBuff("RocketGrab") || enemy.HasBuff("rocketgrab2"))
        //            {
        //                var blitzcrank = HeroManager.Allies.FirstOrDefault(a => a.ChampionName == "Blitzcrank");

        //                if (blitzcrank != null)
        //                {
        //                    E.Cast(blitzcrank.Position.Extend(enemy.Position, 30));
        //                }
        //                return;
        //            }

        //            if (CantMove(enemy) && !enemy.HasBuff("RocketGrab") && enemy.HasBuff("rocketgrab2"))
        //            {
        //                E.Cast(enemy.Position);
        //                return;
        //            }

        //            var pred = E.GetPrediction(enemy);
        //            if (pred.Hitchance >= HitChance.Dashing)
        //            {
        //                E.Cast(pred.CastPosition);
        //                return;
        //            }
        //        }

        //        if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("tp.e").GetValue<bool>())
        //        {
        //            CastWithExtraTrapLogic(E);
        //            return;
        //        }
        //    }
        //}

        //private static void AutoW()
        //{
        //    foreach (
        //        var enemy in
        //            HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(W.Range))
        //                .OrderBy(TargetSelector.GetPriority))
        //    {
        //        if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("ks.w").GetValue<bool>())
        //        {
        //            if (enemy.Health <= W.GetDamage(enemy))
        //            {
        //                W.Cast(enemy);
        //                return;
        //            }
        //        }

        //        if (enemy.HasBuff("RocketGrab") || enemy.HasBuff("rocketgrab2"))
        //        {
        //            var blitzcrank = HeroManager.Allies.FirstOrDefault(a => a.ChampionName == "Blitzcrank");

        //            if (blitzcrank != null)
        //            {
        //                LeagueSharp.Common.Utility.DelayAction.Add(250, () => W.Cast(enemy));
        //            }
        //            return;
        //        }


        //        if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("extras").Item("cc.w").GetValue<bool>())
        //        {
        //            if (CantMove(enemy))
        //            {
        //                W.Cast(enemy);
        //                return;
        //            }
        //        }
        //    }
        //}

        internal static Spell.CastStates CastWithExtraTrapLogic(this Spell spell)
        {
            if (spell.IsReady())
            {
                var teleport = MinionManager.GetMinions(spell.Range).FirstOrDefault(x => x.HasBuff("teleport_target"));
                var zhonya =
                    HeroManager.Enemies.FirstOrDefault(
                        x => ObjectManager.Player.Distance(x) <= spell.Range && x.HasBuff("zhonyasringshield"));

                if (teleport != null)
                    return spell.Cast(teleport);

                if (zhonya != null)
                    return spell.Cast(zhonya);
            }
            return Spell.CastStates.NotCasted;
        }

        private static double GetRDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical,
                new double[] {0, 25, 30, 35}[R.Level]/100*(target.MaxHealth - target.Health) +
                ((new double[] {0, 25, 35, 45}[R.Level] + 0.1*ObjectManager.Player.FlatPhysicalDamageMod)*
                 Math.Min((1 + ObjectManager.Player.Distance(target.ServerPosition)/15*0.09d), 10)));
        }

        private static bool HitsEnemyInTravel(Obj_AI_Base source)
        {
            var pred = R.GetPrediction(source);
            var collision =
                Collision.GetCollision(new List<Vector3> {pred.UnitPosition},
                    new PredictionInput
                    {
                        Unit = ObjectManager.Player,
                        Delay = R.Delay,
                        Speed = R.Speed,
                        Radius = R.Width,
                        CollisionObjects = new[] {CollisionableObjects.Heroes}
                    })
                    .Any(x => x.NetworkId != source.NetworkId);
            return (collision);
        }

        private static void InitSpells()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1490);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 2500);

            W.SetSkillshot(0.6f, 75f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);
        }

        public static void Combo()
        {
            QHandler();

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }


            // todo: W casting
            if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("w.range").GetValue<Slider>().Value != -1 &&
                W.IsReady() &&
                !Player.Spellbook.IsAutoAttacking && target.Distance(Player) >
                Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("w.range").GetValue<Slider>().Value &&
                W.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
            {
                W.Cast(target);
            }

            // todo: E casting
            if (E.IsReady())
            {
                if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("e.slowed").GetValue<bool>())
                {
                    if (E.IsInRange(target) && target.HasBuffOfType(BuffType.Slow))
                    {
                        E.Cast(target);
                        return;
                    }
                }

                if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("e.imlow").GetValue<bool>())
                {
                    if (E.IsInRange(target) && Player.HealthPercent <= 40)
                    {
                        E.Cast(target);
                        return;
                    }
                }

                if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("e.moreally").GetValue<bool>())
                {
                    if (E.IsInRange(target) && Player.CountAlliesInRange(1000) > Player.CountEnemiesInRange(1500))
                    {
                        E.Cast(target);
                        return;
                    }
                }

                if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("e.aoe").GetValue<Slider>().Value != 6)
                {
                    if (target.CountEnemiesInRange(300) >=
                        Menu.Config.SubMenu("adcpackage.jinx")
                            .SubMenu("combo")
                            .Item("e.aoe")
                            .GetValue<Slider>()
                            .Value)
                    {
                        E.Cast(target, true);
                    }
                }
            }

            if (R.IsReady())
            {
                if (Menu.Config.SubMenu("adcpackage.jinx")
                    .SubMenu("combo")
                    .Item("r.aoe")
                    .GetValue<bool>())
                {
                    if (target.HealthPercent < 75)
                    {
                        R.CastIfWillHit(target, 3);
                    }
                }
            }
        }

        public static void Harass()
        {
            QHandler();

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }


            // todo: W casting
            if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("harass").Item("w.range").GetValue<Slider>().Value != -1 &&
                W.IsReady() &&
                !Player.Spellbook.IsAutoAttacking && target.Distance(Player) >
                Menu.Config.SubMenu("adcpackage.jinx").SubMenu("harass").Item("w.range").GetValue<Slider>().Value &&
                W.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
            {
                W.Cast(target);
            }
        }

        public static void LaneClear()
        {
            if (!Player.Spellbook.IsAutoAttacking && FishBones && Q.IsReady())
            {
                Q.Cast();
            }
        }

        private static void QHandler()
        {
            if (Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Menu.Orbwalker.ActiveMode == CustomOrbwalker.OrbwalkingMode.Mixed && Menu.Orbwalker.GetTarget() != null &&
                Menu.Orbwalker.GetTarget().Type == GameObjectType.obj_AI_Minion)
            {
                if (Q.IsReady() && FishBones)
                {
                    Q.Cast();
                }

                return;
            }

            // todo: fishbones when no enemy

            if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("q.range").GetValue<bool>() &&
                Menu.Orbwalker.ActiveMode == CustomOrbwalker.OrbwalkingMode.Combo ||
                Menu.Config.SubMenu("adcpackage.jinx").SubMenu("harass").Item("q.range").GetValue<bool>() &&
                Menu.Orbwalker.ActiveMode == CustomOrbwalker.OrbwalkingMode.Mixed)
            {
                if (!EnemyInRange && !FishBones && Q.IsReady()) // go rocket
                {
                    if (Menu.Orbwalker.ActiveMode == CustomOrbwalker.OrbwalkingMode.Mixed &&
                        Menu.Orbwalker.GetTarget() == null)
                    {
                        return;
                    }

                    Q.Cast();
                }
            }

            // todo: force target and go fishbones if AOE

            if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("q.aoe").GetValue<Slider>().Value != 6 &&
                Menu.Orbwalker.ActiveMode == CustomOrbwalker.OrbwalkingMode.Combo ||
                Menu.Config.SubMenu("adcpackage.jinx").SubMenu("harass").Item("q.aoe").GetValue<Slider>().Value != 6 &&
                Menu.Orbwalker.ActiveMode == CustomOrbwalker.OrbwalkingMode.Mixed)
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            a => a.IsValidTarget(525 + (50 + ((Q.Level)*25) + Player.BoundingRadius + 50)))
                            .OrderBy(TargetSelector.GetPriority)
                            .Where(
                                enemy =>
                                    enemy.CountEnemiesInRange(150) >= (Menu.Orbwalker.ActiveMode ==
                                                                       CustomOrbwalker.OrbwalkingMode.Combo
                                        ? Menu.Config.SubMenu("adcpackage.jinx")
                                            .SubMenu("combo")
                                            .Item("q.aoe")
                                            .GetValue<Slider>()
                                            .Value
                                        : Menu.Config.SubMenu("adcpackage.jinx")
                                            .SubMenu("harass")
                                            .Item("q.aoe")
                                            .GetValue<Slider>()
                                            .Value)))
                {
                    if (!FishBones)
                    {
                        Q.Cast();
                    }
                    Menu.Orbwalker.ForceTarget(enemy);
                    return;
                }
            }

            var qtarget = TargetSelector.GetTarget(525 + (50 + ((Q.Level)*25) + Player.BoundingRadius + 50),
                TargetSelector.DamageType.Physical);
            var target = TargetSelector.GetTarget(525 + Player.BoundingRadius, TargetSelector.DamageType.Physical);

            if (Menu.Config.SubMenu("adcpackage.jinx").SubMenu("combo").Item("q.range").GetValue<bool>() &&
                Menu.Orbwalker.ActiveMode == CustomOrbwalker.OrbwalkingMode.Combo ||
                Menu.Config.SubMenu("adcpackage.jinx").SubMenu("harass").Item("q.range").GetValue<bool>() &&
                Menu.Orbwalker.ActiveMode == CustomOrbwalker.OrbwalkingMode.Mixed)
            {
                if (qtarget != null && target != null && qtarget != target && !FishBones && Q.IsReady())
                    // maybe just switch to if qtarget isnt in range..? idk
                {
                    Q.Cast();
                }
                else
                {
                    if (target != null && qtarget == target && FishBones && Q.IsReady())
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void InitMenu()
        {
            Menu.Config.AddSubMenu(new LeagueSharp.Common.Menu("Jinx", "adcpackage.jinx"));

            // todo: combo -----------------------------
            // todo: q settings
            var comboMenu =
                Menu.Config.SubMenu("adcpackage.jinx").AddSubMenu(new LeagueSharp.Common.Menu("Combo Menu", "combo"));
            {
                comboMenu.Color = SharpDX.Color.MediumVioletRed;
            }

            comboMenu.AddItem(new MenuItem("q.settings", "Q Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            comboMenu.AddItem(new MenuItem("q.when", "Use Q when:"));
            comboMenu.AddItem(new MenuItem("q.range", "   out of range").SetValue(true));
            comboMenu.AddItem(new MenuItem("q.aoe", "   AOE if X targets hit").SetValue(new Slider(3, 2, 6))
                .SetTooltip("Set to 6 to disable", SharpDX.Color.Yellow));

            // todo: w settings
            comboMenu.AddItem(new MenuItem("w.settings", "W Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            comboMenu.AddItem(new MenuItem("w.when", "Use W when:"));
            comboMenu.AddItem(new MenuItem("w.range", "   target > X range"))
                .SetValue(new Slider(525, -1, 1000))
                .SetTooltip("Set to -1 to disable.", SharpDX.Color.Yellow);

            // todo: e settings
            comboMenu.AddItem(new MenuItem("e.settings", "E Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            comboMenu.AddItem(new MenuItem("e.when", "Use E when:"));
            comboMenu.AddItem(new MenuItem("e.slowed", "   target slowed")).SetValue(true);
            comboMenu.AddItem(new MenuItem("e.imlow", "   im low")).SetValue(true);
            comboMenu.AddItem(new MenuItem("e.moreally", "   ally count > enemy")).SetValue(true);
            comboMenu.AddItem(new MenuItem("e.aoe", "   enemy count >="))
                .SetValue(new Slider(3, 2, 6))
                .SetTooltip("Set to 6 to disable.", SharpDX.Color.Yellow);

            // todo: r settings
            comboMenu.AddItem(new MenuItem("r.settings", "R Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            comboMenu.AddItem(new MenuItem("r.when", "Use R when:"));
            comboMenu.AddItem(new MenuItem("r.killable", "   target killable")).SetValue(true);
            comboMenu.AddItem(new MenuItem("r.aoe", "   AOE")).SetValue(true);


            //
            // todo: harass
            //

            var harassMenu =
                Menu.Config.SubMenu("adcpackage.jinx").AddSubMenu(new LeagueSharp.Common.Menu("Harass Menu", "harass"));
            {
                harassMenu.Color = SharpDX.Color.MediumVioletRed;
            }

            harassMenu.AddItem(new MenuItem("q.settings", "Q Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            harassMenu.AddItem(new MenuItem("q.when", "Use Q when:"));
            harassMenu.AddItem(new MenuItem("q.range", "   out of range").SetValue(true));
            harassMenu.AddItem(new MenuItem("q.aoe", "   AOE if X targets hit").SetValue(new Slider(3, 2, 6))
                .SetTooltip("Set to 6 to disable", SharpDX.Color.Yellow));

            // todo: w settings
            harassMenu.AddItem(new MenuItem("w.settings", "W Settings"))
                .SetFontStyle(FontStyle.Regular, SharpDX.Color.Yellow);
            harassMenu.AddItem(new MenuItem("w.when", "Use W when:"));
            harassMenu.AddItem(new MenuItem("w.range", "   target > X range"))
                .SetValue(new Slider(525, -1, 1000))
                .SetTooltip("Set to -1 to disable.", SharpDX.Color.Yellow);

            //
            // todo: extras
            //
            var extrasMenu =
                Menu.Config.SubMenu("adcpackage.jinx")
                    .AddSubMenu(new LeagueSharp.Common.Menu("Extras Menu", "extras"));
            {
                extrasMenu.Color = SharpDX.Color.Aquamarine;
            }
            extrasMenu.AddItem(new MenuItem("extras.ks", "Killsteal settings").SetFontStyle(FontStyle.Regular,
                SharpDX.Color.Yellow));
            extrasMenu.AddItem(new MenuItem("ks.w", "Use W to KS")).SetValue(true);
            extrasMenu.AddItem(new MenuItem("ks.r", "Use R to KS")).SetValue(true);
            extrasMenu.AddItem(new MenuItem("extras.cc", "Auto spell on:").SetFontStyle(FontStyle.Regular,
                SharpDX.Color.Yellow));
            extrasMenu.AddItem(new MenuItem("cc.w", "Use W on CC'd")).SetValue(true);
            extrasMenu.AddItem(new MenuItem("cc.e", "Use E on CC'd")).SetValue(true);
            extrasMenu.AddItem(new MenuItem("tp.e", "Use E on TP")).SetValue(true);

            extrasMenu.AddItem(new MenuItem("extras.antigc", "Anti-Gapcloser settings").SetFontStyle(FontStyle.Regular,
                SharpDX.Color.Yellow));
            extrasMenu.AddItem(new MenuItem("agc.w", "Use W on gapclose")).SetValue(false);
            extrasMenu.AddItem(new MenuItem("agc.e", "Use E on gapclose")).SetValue(true);

            //
            // todo: DRAWINGS
            //

            var drawingsMenu =
                Menu.Config.SubMenu("adcpackage.JINX")
                    .AddSubMenu(new LeagueSharp.Common.Menu("Drawings Menu", "drawings"));
            drawingsMenu.AddItem(new MenuItem("draw.w", "Draw W range"))
                .SetValue(true);
        }
    }
}