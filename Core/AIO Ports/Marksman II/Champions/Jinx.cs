#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using Marksman.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using Orbwalking = LeagueSharp.Common.Orbwalking;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    internal class Jinx : Champion
    {
        public static Spell Q, W, E, R;
        private int wCastTime;
        private Obj_AI_Minion aMinion;
        public Jinx()
        {
            Q = new Spell(SpellSlot.Q);

            W = new Spell(SpellSlot.W, 1450f);
            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 900f);
            E.SetSkillshot(1.1f, 1f, 1750f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 4000f);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);
            
            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                if (E.IsReady())
                {
                    BuffInstance aBuff =
                        (from fBuffs in
                             sender.Buffs.Where(
                                 s =>
                                 sender.Team != ObjectManager.Player.Team
                                 && sender.Distance(ObjectManager.Player.Position) < E.Range)
                         from b in new[]
                                           {
                                                "teleport_", /* Teleport */
                                                "pantheon_grandskyfall_jump", /* Pantheon */ 
                                                "crowstorm", /* FiddleScitck */
                                                "zhonya", "katarinar", /* Katarita */
                                                "MissFortuneBulletTime", /* MissFortune */
                                                "gate", /* Twisted Fate */
                                                "chronorevive" /* Zilean */
                                           }
                         where args.Buff.Name.ToLower().Contains(b)
                         select fBuffs).FirstOrDefault();

                    if (aBuff != null && aBuff.StartTime + CommonUtils.GetRandomDelay(1000, 1500) <= Game.Time)
                    {
                        E.Cast(sender.Position);
                    }
                }
            };

            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit = GetComboDamage;
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = true;

            Utils.Utils.PrintMessage("Jinx");
        }

        public override void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            /*
            var minion = args.Target as Obj_AI_Minion;
            if (minion != null && LaneClearActive)
            {
                var mobs =
                    MinionManager.GetMinions(ObjectManager.Player.Position, Orbwalking.GetRealAutoAttackRange(null) + 65,
                        MinionTypes.All, MinionTeam.Enemy).Count(m => m.Distance(minion) < 155);
                if (mobs < 2 && FishBoneActive)
                {
                    args.Process = false;
                    Q.Cast();
                }
            }
            */
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!E.IsReady()) return;
            if (gapcloser.End.Distance(ObjectManager.Player.Position) <= 200)
                E.Cast(gapcloser.End);
        }


        public override void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!E.IsReady()) return;
            if (args.DangerLevel >= Interrupter2.DangerLevel.Medium)
                E.Cast(sender);
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.Slot == SpellSlot.W)
                wCastTime = Environment.TickCount;
        }


        private static float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            fComboDamage += (float) GetRDamage(t);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot"))
                == SpellState.Ready && ObjectManager.Player.Distance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.Distance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.Distance(t) < 550) fComboDamage += (float)ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Botrk);

            return fComboDamage;
        }


        public float QExtraRange => 50 + 25 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;

        private bool FishBoneActive => ObjectManager.Player.AttackRange > 565f;

        private int PowPowStacks
        {
            get
            {
                return
                    ObjectManager.Player.Buffs.Where(buff => buff.DisplayName.ToLower() == "jinxqramp")
                        .Select(buff => buff.Count)
                        .FirstOrDefault();
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (aMinion != null)
                Render.Circle.DrawCircle(aMinion.Position, 50f, System.Drawing.Color.Red);
            Render.Circle.DrawCircle(ObjectManager.Player.Position, 125, System.Drawing.Color.Red);
            return;
            /*----------------------------------------------------*/
            var drawQbound = GetValue<Circle>("DrawQBound");
            foreach (var spell in new[] { W, E })
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }
            }

            if (drawQbound.Active)
            {
                if (FishBoneActive)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 525f + ObjectManager.Player.BoundingRadius + 65f, drawQbound.Color);
                }
                else
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 525f + ObjectManager.Player.BoundingRadius + 65f + QExtraRange + 20f, drawQbound.Color);
                }
            }
        }

        public override void GameOnUpdate(EventArgs args)
        {
            if (GetValue<bool>("PingCH"))
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        enemy =>
                            R.IsReady() && enemy.IsValidTarget() && R.GetDamage(enemy) > enemy.Health))
                {
                    //Marksman.Utils.Utils.MPing.Ping(enemy.Position.To2D());
                }
            }

            if (Q.IsReady() && GetValue<bool>("SwapDistance") && Program.ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {

                var nKillableEnemy = HeroManager.Enemies.Find(e => e.Health <= Q.GetDamage(e)*2 && e.Health >= ObjectManager.Player.TotalAttackDamage*4);

                var activeQ = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 25 + 650;
                var t = TargetSelector.GetTarget(activeQ, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget() && !t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                {
                    if (!FishBoneActive)
                    {
                        Q.Cast();
                        Orbwalker.ForceTarget(t);
                        return;
                    }
                }
            }

            if (GetValue<bool>("PingCH"))
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        t =>
                        R.IsReady() && t.IsValidTarget() && R.GetDamage(t) > t.Health
                        && t.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(null) + 65 + QExtraRange))
                {
                    Utils.Utils.MPing.Ping(enemy.Position.To2D(), 2, PingCategory.Normal);
                }
            }

         

            if (GetValue<bool>("SwapQ") && FishBoneActive && !ComboActive)
            {
                Q.Cast();
            }

            if ((!ComboActive && !HarassActive) || !Orbwalking.CanMove(100))
            {
                return;
            }

            var useR = GetValue<bool>("UseRC");
            
                foreach (var t in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(t => t.IsValidTarget(GetRealPowPowRange(t) + QExtraRange + 20f)))
                {
                    var swapDistance = GetValue<bool>("SwapDistance");
                    var swapAoe = GetValue<bool>("SwapAOE");
                    var distance = GetRealDistance(t);
                    var powPowRange = GetRealPowPowRange(t);

                    if (swapDistance && Q.IsReady())
                    {
                        if (distance > powPowRange && !FishBoneActive)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                        else if (distance < powPowRange && FishBoneActive)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                    }

                    if (swapAoe && Q.IsReady())
                    {
                        if (distance > powPowRange && PowPowStacks > 2 && !FishBoneActive && CountEnemies(t, 150) > 1)
                        {
                            if (Q.Cast())
                            {
                                return;
                            }
                        }
                    }
                }

            
            //if (useR && R.IsReady())
            //{
            //    var checkRok = GetValue<bool>("ROverKill");
            //    var minR = GetValue<Slider>("MinRRange").Value;
            //    var maxR = GetValue<Slider>("MaxRRange").Value;
            //    var t = TargetSelector.GetTarget(maxR, TargetSelector.DamageType.Physical);

            //    if (t.IsValidTarget() && !t.HasKindredUltiBuff())
            //    {
            //        var distance = GetRealDistance(t);

            //        if (!checkRok)
            //        {
            //            if (ObjectManager.Player.GetSpellDamage(t, SpellSlot.R, 1) > t.Health && !t.IsZombie)
            //            {
            //                R.CastIfHitchanceGreaterOrEqual(t);
            //                //R.CastIfHitchanceEquals(t, HitChance.High, false);
            //                //if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
            //            }
            //        }
            //        else if (distance > minR)
            //        {
            //            var aDamage = ObjectManager.Player.GetAutoAttackDamage(t);
            //            var wDamage = ObjectManager.Player.GetSpellDamage(t, SpellSlot.W);
            //            var rDamage = ObjectManager.Player.GetSpellDamage(t, SpellSlot.R);
            //            var powPowRange = GetRealPowPowRange(t);

            //            if (distance < (powPowRange + QExtraRange) && !(aDamage * 3.5 > t.Health))
            //            {
            //                if (!W.IsReady() || !(wDamage > t.Health) || W.GetPrediction(t).CollisionObjects.Count > 0)
            //                {
            //                    if (CountAlliesNearTarget(t, 500) <= 3)
            //                    {
            //                        if (rDamage > t.Health && !t.IsZombie /*&& !ObjectManager.Player.IsAutoAttacking &&
            //                            !ObjectManager.Player.IsChanneling*/)
            //                        {
            //                            R.CastIfHitchanceGreaterOrEqual(t);
            //                            //R.CastIfHitchanceEquals(t, HitChance.High, false);
            //                            //if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
            //                        }
            //                    }
            //                }
            //            }
            //            else if (distance > (powPowRange + QExtraRange))
            //            {
            //                if (!W.IsReady() || !(wDamage > t.Health) || distance > W.Range
            //                    || W.GetPrediction(t).CollisionObjects.Count > 0)
            //                {
            //                    if (CountAlliesNearTarget(t, 500) <= 3)
            //                    {
            //                        if (rDamage > t.Health && !t.IsZombie /*&& !ObjectManager.Player.IsAutoAttacking &&
            //                            !ObjectManager.Player.IsChanneling*/)
            //                        {
            //                            R.CastIfHitchanceGreaterOrEqual(t);
            //                            //R.CastIfHitchanceEquals(t, HitChance.High, false);
            //                            //if (R.Cast(t) == Spell.CastStates.SuccessfullyCasted) { }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if ((ComboActive || HarassActive) && unit.IsMe && (target is AIHeroClient))
            {
                var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));

                if (useQ)
                {
                    foreach (var t in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(t => t.IsValidTarget(GetRealPowPowRange(t) + QExtraRange + 20f) && !t.HasKindredUltiBuff()))
                    {
                        var swapDistance = GetValue<bool>("SwapDistance");
                        var swapAoe = GetValue<bool>("SwapAOE");
                        var distance = GetRealDistance(t);
                        var powPowRange = GetRealPowPowRange(t);

                        if (swapDistance && Q.IsReady())
                        {
                            if (distance > powPowRange && !FishBoneActive)
                            {
                                if (Q.Cast())
                                {
                                    return;
                                }
                            }
                            else if (distance < powPowRange && FishBoneActive)
                            {
                                if (Q.Cast())
                                {
                                    return;
                                }
                            }
                        }

                        if (swapAoe && Q.IsReady())
                        {
                            if (distance > powPowRange && PowPowStacks > 2 && !FishBoneActive
                                && CountEnemies(t, 150) > 1)
                            {
                                if (Q.Cast())
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static int CountEnemies(Obj_AI_Base target, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(
                        hero =>
                        hero.IsValidTarget() && hero.Team != ObjectManager.Player.Team
                        && hero.ServerPosition.Distance(target.ServerPosition) <= range);
        }

        private int CountAlliesNearTarget(Obj_AI_Base target, float range)
        {
            return
                ObjectManager.Get<AIHeroClient>()
                    .Count(
                        hero =>
                        hero.Team == ObjectManager.Player.Team
                        && hero.ServerPosition.Distance(target.ServerPosition) <= range);
        }

        private static float GetRealPowPowRange(GameObject target)
        {
            //return Orbwalking.GetRealAutoAttackRange(null) + 65;
            return 525f + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }

        private static float GetRealDistance(GameObject target)
        {
            return ObjectManager.Player.Position.Distance(target.Position) + ObjectManager.Player.BoundingRadius
                   + target.BoundingRadius;
        }

       


        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQC" + Id, "Use Q").SetValue(true));
            config.AddItem(new MenuItem("UseWC" + Id, "Use W").SetValue(true));
            config.AddItem(new MenuItem("UseEC" + Id, "Use E").SetValue(true));
            config.AddItem(new MenuItem("UseRC" + Id, "Use R").SetValue(true));
            config.AddItem(new MenuItem("PingCH" + Id, "Ping Killable Enemy with R").SetValue(true));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "Use Q").SetValue(true));
            config.AddItem(new MenuItem("UseQMH" + Id, "Use Q Nearby Minions").SetValue(true));
            config.AddItem(new MenuItem("UseEH" + Id, "Use E").SetValue(true));
            config.AddItem(new MenuItem("UseWH" + Id, "Use W").SetValue(false));
            return true;
        }

        public override bool LaneClearMenu(Menu menuLane)
        {
            // Q
            string[] strQ = new string[5];
            {
                strQ[0] = "Off";

                for (var i = 1; i < 5; i++)
                {
                    strQ[i] = "Minion Count >= " + i;
                }
                menuLane.AddItem(new MenuItem("Lane.UseQ" + Id, "Q:").SetValue(new StringList(strQ))).SetFontStyle(FontStyle.Regular, W.MenuColor());
            }
            menuLane.AddItem(new MenuItem("Lane.UseQ.Mode" + Id, "Q Mode:").SetValue(new StringList(new[] { "Under Ally Turret", "Out of AA Range", "Both" }, 2))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            
            // W
            menuLane.AddItem(new MenuItem("Lane.UseW" + Id, "W:").SetValue(new StringList(new[] { "Off", "Out of AA Range" }, 1))).SetFontStyle(FontStyle.Regular, W.MenuColor());
            return true;
        }

        public override bool JungleClearMenu(Menu menuJungle)
        {
            // Q
            string[] strQ = new string[4];
            {
                strQ[0] = "Off";
                strQ[1] = "Just for big Monsters";

                for (var i = 2; i < 4; i++)
                {
                    strQ[i] = "Mobs Count >= " + i;
                }
                menuJungle.AddItem(new MenuItem("Lane.UseQ", "Q:").SetValue(new StringList(strQ, 3))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            }
            
            // W
            menuJungle.AddItem(new MenuItem("Lane.UseW", "W [Just Big Mobs]:").SetValue(new StringList(new[] { "Off", "On", "Just Slows the Mob" }))).SetFontStyle(FontStyle.Regular, W.MenuColor());

            // R
            menuJungle.AddItem(new MenuItem("Lane.UseR", "R:").SetValue(new StringList(new[] { "Off", "Baron/Dragon Steal"}, 1))).SetFontStyle(FontStyle.Regular, R.MenuColor());

            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddItem(new MenuItem("SwapQ" + Id, "Q: Always swap to Minigun").SetValue(false)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            config.AddItem(new MenuItem("SwapDistance" + Id, "Q: Swap for distance").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            config.AddItem(new MenuItem("SwapAOE" + Id, "Q: Swap for AOE").SetValue(false)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            config.AddItem(new MenuItem("MinWRange" + Id, "W: Min. range").SetValue(new Slider(525 + 65 * 2, 0, 1200))).SetFontStyle(FontStyle.Regular, W.MenuColor());
            config.AddItem(new MenuItem("Misc.UseE.Immobile" + Id, "E: on immobile").SetValue(true)).SetFontStyle(FontStyle.Regular, E.MenuColor());
            config.AddItem(new MenuItem("AutoES" + Id, "E: on slowed").SetValue(true)).SetFontStyle(FontStyle.Regular, E.MenuColor());
            config.AddItem(new MenuItem("AutoED" + Id, "E: on dashing").SetValue(false)).SetFontStyle(FontStyle.Regular, E.MenuColor());
            config.AddItem(new MenuItem("CastR" + Id, "R: (2000 Range)").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))).SetFontStyle(FontStyle.Regular, R.MenuColor());
            config.AddItem(new MenuItem("ROverKill" + Id, "R: Kill Steal").SetValue(true)).SetFontStyle(FontStyle.Regular, R.MenuColor());
            config.AddItem(new MenuItem("MinRRange" + Id, "R: Min. range").SetValue(new Slider(300, 0, 1500))).SetFontStyle(FontStyle.Regular, R.MenuColor());
            config.AddItem(new MenuItem("MaxRRange" + Id, "R: Max. range").SetValue(new Slider(1700, 0, 4000))).SetFontStyle(FontStyle.Regular, R.MenuColor());
            config.AddItem(new MenuItem("PingCH" + Id, "R: Ping Killable Enemy with R").SetValue(true));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("DrawQBound" + Id, "Draw Q bound").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 0))));
            config.AddItem(new MenuItem("DrawE" + Id, "E range").SetValue(new Circle(false, System.Drawing.Color.CornflowerBlue)));
            config.AddItem(new MenuItem("DrawW" + Id, "W range").SetValue(new Circle(false, System.Drawing.Color.CornflowerBlue)));
            config.AddItem(new MenuItem("DrawCH" + Id, "Draw Killable Enemy with R").SetValue(true));
            return true;
        }

        public override void ExecuteCombo()
        {
            var useE = GetValue<bool>("UseE" + (ComboActive ? "C" : "H"));
            if (E.IsReady() && useE) // Credits DZ191
            {
                var target = HeroManager.Enemies.Find(x => x.IsValidTarget(E.Range));
                if (target != null)
                {
                    if (target.HasBuffOfType(BuffType.Slow))
                    {
                        if (target.Path.Count() > 1)
                        {
                            var slowEndTime = target.GetSlowEndTime();
                            if (slowEndTime >= E.Delay + 0.5f + Game.Ping/2f)
                            {
                                E.CastIfHitchanceGreaterOrEqual(target);
                            }
                        }
                    }

                    if (target.IsHeavilyImpaired())
                    {
                        var immobileEndTime = target.GetImpairedEndTime();
                        if (immobileEndTime >= E.Delay + 0.5f + Game.Ping / 2f)
                        {
                            E.CastIfHitchanceGreaterOrEqual(target);
                        }
                    }
                }
            }
            
            var useW = GetValue<bool>("UseW" + (ComboActive ? "C" : "H"));
            if (W.IsReady() && useW)
            {
                var target = TargetSelector.GetTargetNoCollision(W);
                if (target == null) return;
                if (!ObjectManager.Player.UnderTurret(true) || !target.UnderTurret(true))
                {
                    W.CastIfHitchanceGreaterOrEqual(target);
                }
            }

            if ((GetValue<KeyBind>("CastR").Active || GetValue<bool>("UseRC")) && R.IsReady())
            {
                if (wCastTime + 1060 <= Environment.TickCount)
                {
                    var target =
                        HeroManager.Enemies.FirstOrDefault(
                            x =>
                                !x.IsZombie && x.CountAlliesInRange(500) < 2 &&
                                HealthPrediction.GetHealthPrediction(x, 5000) > 0 &&
                                ObjectManager.Player.Distance(x) >= Orbwalking.GetRealAutoAttackRange(null) + 65 + QExtraRange &&
                                x.IsKillableAndValidTarget(GetRDamage(x), TargetSelector.DamageType.Physical, R.Range) &&
                                R.GetPrediction(x).Hitchance >= HitChance.High);
                    if (target != null)
                    {
                        var prediction = R.GetPrediction(target);
                        var collision =
                            Collision.GetCollision(new List<Vector3> { prediction.UnitPosition },
                                new PredictionInput
                                {
                                    UseBoundingRadius = true,
                                    Unit = ObjectManager.Player,
                                    Delay = R.Delay,
                                    Speed = R.Speed,
                                    Radius = 200,
                                    CollisionObjects = new[] { CollisionableObjects.Heroes }
                                })
                                .Any(x => x.NetworkId != target.NetworkId);
                        if (!collision)
                            R.Cast(target);
                    }
                }
            }
            base.ExecuteCombo();
        }

        private static double GetRDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical,
                new double[] {0, 25, 30, 35}[R.Level]/100*(target.MaxHealth - target.Health) +
                (new double[] {0, 25, 35, 45}[R.Level] + 0.1*ObjectManager.Player.FlatPhysicalDamageMod)*
                Math.Min(1 + ObjectManager.Player.Distance(target.ServerPosition)/15*0.09d, 10));
        }

        public override void ExecuteFlee()
        {
            foreach (
                AIHeroClient unit in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(e => e.IsValidTarget(E.Range) && !e.IsDead && e.IsEnemy)
                        .OrderBy(e => ObjectManager.Player.Distance(e)))
            {
                PredictionOutput ePred = E.GetPrediction(unit);
                Vector3 eBehind = ePred.CastPosition - Vector3.Normalize(unit.ServerPosition - ObjectManager.Player.ServerPosition)*150;

                if (E.IsReady())
                    E.Cast(eBehind);
            }

            base.ExecuteFlee();
        }

        public override void PermaActive()
        {
            if (GetValue<bool>("Misc.UseE.Immobile") && E.IsReady())
            {
                var target =
                    HeroManager.Enemies.FirstOrDefault(
                        x => x.IsValidTarget(E.Range) && x.IsImmobileUntil() > 0.5f);
                if (target != null)
                    E.Cast(target, false, true);
            }

            return;
            var nCount = ObjectManager.Player.CountEnemiesInRange(565) >= 1;
            var nEnemy =
                HeroManager.Enemies.Find(
                    e => e.IsValidTarget(565 + QExtraRange + 20) && e.Health < ObjectManager.Player.GetAutoAttackDamage(e));
            
                if (nCount && nEnemy == null && FishBoneActive )
            {
                Q.Cast();
            }


        }


    }

    public static class JinxX
    {
        public static float FishboneRange => 75f + Jinx.Q.Level * 25f;

        public static float MiniGunRange(this AIHeroClient t)
        {
            return 525f + ObjectManager.Player.BoundingRadius + (t?.BoundingRadius ?? 0f);
        }

        public static float MegaGunRange => ObjectManager.Player.MiniGunRange() + FishboneRange + 25f;
    }
}
