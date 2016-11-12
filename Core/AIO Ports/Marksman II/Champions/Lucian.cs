#region
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Common;

using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;


#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    internal class Lucian : Champion
    {
        private static bool havePassive => ObjectManager.Player.HasBuff("lucianpassivebuff");
        private int PassiveStartedTime;

        public static Spell Q, qExtended;

        public static Spell W, wNoCollision;

        public static Spell E;

        public static Spell R;

        public static bool DoubleHit = false;
        private int aaCount;

        private static CommonGeometry.Polygon toPolygon;


        private bool hasPassive;

        public Lucian()
        {
            Q = new Spell(SpellSlot.Q, 750f, TargetSelector.DamageType.Physical) { MinHitChance = HitChance.High };

            qExtended = new Spell(SpellSlot.Q, 1100f, TargetSelector.DamageType.Physical);
            qExtended.SetSkillshot(0.5f, 65f, float.MaxValue, false, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 1000f, TargetSelector.DamageType.Physical) { MinHitChance = HitChance.High };
            W.SetSkillshot(0.30f, 55f, 1600f, true, SkillshotType.SkillshotLine);

            wNoCollision = new Spell(SpellSlot.W, 1000f, TargetSelector.DamageType.Physical);
            wNoCollision.SetSkillshot(0.30f, 55f, 1600f, false, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 475f);

            R = new Spell(SpellSlot.R, 1400f);

            Utils.Utils.PrintMessage("Lucian");
        }

        public override void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
                if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E ||
                    args.Slot == SpellSlot.R)
                {
                    hasPassive = true;
                    aaCount = 2;
                    PassiveStartedTime = Environment.TickCount;
                }

        }


        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!E.IsReady()) return;

            if (args.Target != null && args.Target.IsMe && sender.Type == GameObjectType.AIHeroClient && sender.IsEnemy && sender.IsMelee && args.SData.IsAutoAttack())
                //if (MenuProvider.Champion.Misc.GetBoolValue("Use Anti-Melee (E)"))
                E.Cast(ObjectManager.Player.Position.Extend(sender.Position, -E.Range));
        }

        public override void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
                if (args.Animation == "Spell1" || args.Animation == "Spell2")
                    if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        public override void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!args.Unit.IsMe) return;
            if (ObjectManager.Player.HasBuff("LucianR"))
                args.Process = false;
        }

        public override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            aaCount -= 1;
            return;
            if (unit.IsMe)
            {
                hasPassive = false;

                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                    {
                        if (GetValue<bool>("Combo.E.Use"))
                            if (E.IsReady())
                                if (ObjectManager.Player.Position.Extend(Game.CursorPos, 700).CountEnemiesInRange(700) <= 1)
                                    E.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, 700));
                        break;
                    }
                }
            }
        }

        public override void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.Buff.Name.ToLower() == "lucianpassivebuff")
            {
                hasPassive = false;
            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (GetValue<bool>("Misc.E.Antigapcloser"))
                if (gapcloser.End.Distance(ObjectManager.Player.Position) <= 200)
                    if (gapcloser.Sender.IsValidTarget())
                        if (gapcloser.Sender.ChampionName.ToLowerInvariant() != "masteryi")
                            if (E.IsReady())
                                E.Cast(ObjectManager.Player.Position.Extend(gapcloser.Sender.Position, -E.Range));
        }

        public static Obj_AI_Base QMinion(AIHeroClient t)
        {
            var m = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None);

            return (from vM
                        in m.Where(vM => vM.IsValidTarget(Q.Range))
                    let endPoint = vM.ServerPosition.To2D().Extend(ObjectManager.Player.ServerPosition.To2D(), -qExtended.Range).To3D()
                    where
                        vM.Distance(t) <= t.Distance(ObjectManager.Player) &&
                        Intersection(ObjectManager.Player.ServerPosition.To2D(), endPoint.To2D(), t.ServerPosition.To2D(), vM.BoundingRadius)
                    select vM).FirstOrDefault();
        }

        public static bool IsPositionSafeForE(AIHeroClient target, Spell spell)
        {
            var predPos = spell.GetPrediction(target).UnitPosition.To2D();
            var myPos = ObjectManager.Player.Position.To2D();
            var newPos = (target.Position.To2D() - myPos);
            newPos.Normalize();

            var checkPos = predPos + newPos * (spell.Range - Vector2.Distance(predPos, myPos));
            Obj_Turret closestTower = null;

            foreach (var tower in ObjectManager.Get<Obj_Turret>()
                .Where(tower => tower.IsValid && !tower.IsDead && Math.Abs(tower.Health) > float.Epsilon)
                .Where(tower => Vector3.Distance(tower.Position, ObjectManager.Player.Position) < 1450))
            {
                closestTower = tower;
            }

            if (closestTower == null)
                return true;

            if (Vector2.Distance(closestTower.Position.To2D(), checkPos) <= 910)
                return false;

            return true;
        }

        public override void DrawingOnEndScene(EventArgs args)
        {
            return;
            var nClosesEnemy = HeroManager.Enemies.Find(e => e.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null)));
            if (nClosesEnemy != null)
            {

                var aaRange = Orbwalking.GetRealAutoAttackRange(null) + 65 - ObjectManager.Player.Distance(nClosesEnemy);
                Render.Circle.DrawCircle(ObjectManager.Player.Position, aaRange, Color.BurlyWood);

                Vector2 wcPositive = ObjectManager.Player.Position.To2D() - Vector2.Normalize(nClosesEnemy.Position.To2D() - ObjectManager.Player.Position.To2D()).Rotated((float)Math.PI / 180) * aaRange;
                Vector2 wcPositive2 = ObjectManager.Player.Position.To2D() - Vector2.Normalize(nClosesEnemy.Position.To2D() - ObjectManager.Player.Position.To2D()).Rotated(30 * (float)Math.PI / 180) * aaRange;
                Vector2 wcPositive3 = ObjectManager.Player.Position.To2D() - Vector2.Normalize(nClosesEnemy.Position.To2D() - ObjectManager.Player.Position.To2D()).Rotated(-30 * (float)Math.PI / 180) * aaRange;

                Vector2 wcPositive2x = ObjectManager.Player.Position.To2D() - Vector2.Normalize(nClosesEnemy.Position.To2D() - ObjectManager.Player.Position.To2D()).Rotated(60 * (float)Math.PI / 180) * aaRange;
                Vector2 wcPositive3x = ObjectManager.Player.Position.To2D() - Vector2.Normalize(nClosesEnemy.Position.To2D() - ObjectManager.Player.Position.To2D()).Rotated(-60 * (float)Math.PI / 180) * aaRange;

                if (E.IsReady())
                {
                    var runHere = Vector2.Zero;
                    if (!wcPositive.IsWall())
                        runHere = wcPositive;
                    else if (!wcPositive2.IsWall())
                        runHere = wcPositive2;
                    else if (!wcPositive3.IsWall())
                        runHere = wcPositive3;
                    else if (!wcPositive2x.IsWall())
                        runHere = wcPositive2x;
                    else if (!wcPositive3x.IsWall())
                        runHere = wcPositive3x;

                    if (runHere != Vector2.Zero && ObjectManager.Player.Distance(runHere) > ObjectManager.Player.BoundingRadius * 2)
                        E.Cast(runHere);
                }

                Render.Circle.DrawCircle(wcPositive2.To3D(), 80f, Color.Red);
                Render.Circle.DrawCircle(wcPositive3.To3D(), 80f, Color.Yellow);
                Render.Circle.DrawCircle(wcPositive.To3D(), 80, Color.BurlyWood);

                Render.Circle.DrawCircle(wcPositive2x.To3D(), 80f, Color.Red);
                Render.Circle.DrawCircle(wcPositive3x.To3D(), 80f, Color.Yellow);

            }
            //if (Q.IsReady())
            //{
            return;
                foreach (var t in HeroManager.Enemies.Where(e => e.IsValidTarget(1100)))
                {

                    var toPolygon =
                        new CommonGeometry.Rectangle(ObjectManager.Player.Position.To2D(),
                            ObjectManager.Player.Position.To2D()
                                .Extend(t.Position.To2D(), t.Distance(ObjectManager.Player.Position)), 30).ToPolygon();
                    toPolygon.Draw(System.Drawing.Color.Red, 1);

                    var o = ObjectManager
                        .Get<Obj_AI_Base>(
                            ).FirstOrDefault(e => e.IsEnemy && !e.IsDead && e.NetworkId != t.NetworkId && toPolygon.IsInside(e) &&
                                ObjectManager.Player.Distance(t.Position) > ObjectManager.Player.Distance(e) && e.Distance(t) > t.BoundingRadius && e.Distance(ObjectManager.Player) > ObjectManager.Player.BoundingRadius);

                    if (o != null)
                    {
                        Render.Circle.DrawCircle(o.Position, 105f, Color.GreenYellow);
                        Q.CastOnUnit(o);
                    }

                    Vector2 wcPositive = ObjectManager.Player.Position.To2D() - Vector2.Normalize(t.Position.To2D() - ObjectManager.Player.Position.To2D()).Rotated((float)Math.PI / 180) * (E.Range - 50);
                    Render.Circle.DrawCircle(wcPositive.To3D(), 60, Color.BurlyWood);
                    Render.Circle.DrawCircle(wcPositive.To3D(), 80f, Color.BurlyWood);
                    Render.Circle.DrawCircle(wcPositive.To3D(), 100f, Color.BurlyWood);

                }
            //}

            return;
            foreach (var t in HeroManager.Enemies.Where(e => e.IsValidTarget(1100)))
            {

                var toPolygon = new CommonGeometry.Rectangle(ObjectManager.Player.Position.To2D(), ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), t.Distance(ObjectManager.Player.Position)), 40).ToPolygon();
                toPolygon.Draw(System.Drawing.Color.Red, 1);


                foreach (var obj in ObjectManager.Get<Obj_AI_Base>())
                {
                    
                }

                //Console.WriteLine(hero.ChampionName);

                for (int j = 20; j < 361; j += 20)
                {
                    Vector2 wcPositive = ObjectManager.Player.Position.To2D() + Vector2.Normalize(t.Position.To2D() - ObjectManager.Player.Position.To2D()).Rotated(j * (float)Math.PI / 180) * E.Range;
                    if (!wcPositive.IsWall() && t.Distance(wcPositive) > E.Range)
                        Render.Circle.DrawCircle(wcPositive.To3D(), 105f, Color.GreenYellow);
                    //if (!wcPositive.IsWall())
                    //{
                    //    ListWJumpPositions.Add(wcPositive);
                    //}

                    //Vector2 wcNegative = ObjectManager.Player.Position.To2D() +
                    //                     Vector2.Normalize(hero.Position.To2D() - ObjectManager.Player.Position.To2D())
                    //                         .Rotated(-j * (float)Math.PI / 180) * E.Range;

                    //Render.Circle.DrawCircle(wcNegative.To3D(), 105f, Color.White);
                    //if (!wcNegative.IsWall())
                    //{
                    //    ListWJumpPositions.Add(wcNegative);
                    //}
                }


            }

        }

    
        public static bool Intersection(Vector2 p1, Vector2 p2, Vector2 pC, float radius)
        {
            var p3 = new Vector2(pC.X + radius, pC.Y + radius);

            var m = ((p2.Y - p1.Y) / (p2.X - p1.X));
            var constant = (m * p1.X) - p1.Y;
            var b = -(2f * ((m * constant) + p3.X + (m * p3.Y)));
            var a = (1 + (m * m));
            var c = ((p3.X * p3.X) + (p3.Y * p3.Y) - (radius * radius) + (2f * constant * p3.Y) + (constant * constant));
            var d = ((b * b) - (4f * a * c));

            return d > 0;
        }

        public override void ExecuteCombo()
        {
            var t =
                HeroManager.Enemies.Find(
                    e => e.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) && e.CanGetDamage());

            if (hasPassive && t != null)
            {
                Orbwalker.ForceTarget(t);
            }

            if (!hasPassive && t.IsValidTarget() && Q.IsReady())
            {
                Q.CastOnUnit(t);
            }

            if (GetValue<bool>("Combo.W.Use") && !hasPassive && t.IsValidTarget() && W.IsReady())
            {
                W.Cast(t.Position);
            }

            if (t.IsValidTarget() && GetValue<bool>("Combo.E.Use") && E.IsReady() && !hasPassive)
            {
                    var x = CommonUtils.GetDashPosition(E, t, 200);
                    E.Cast(x);
            }

            if (!t.IsValidTarget() && t.IsValidTarget(qExtended.Range))
            {
                var useQExtended = GetValue<StringList>("UseQExtendedC").SelectedIndex;
                if (useQExtended != 0)
                {
                    switch (useQExtended)
                    {
                        case 1:
                        {
                            var tx = QMinion(t);
                            if (tx.IsValidTarget(Q.Range))
                            {
                                Q.CastOnUnit(tx);
                            }
                            break;
                        }

                        case 2:
                        {
                            var enemy = HeroManager.Enemies.Find(e => e.IsValidTarget(qExtended.Range) && !e.IsZombie);
                            if (enemy != null)
                            {
                                var tx = QMinion(enemy);
                                if (tx.IsValidTarget())
                                {
                                    Q.CastOnUnit(tx);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        public override void GameOnUpdate(EventArgs args)
        {
            return;
            // Auto turn off Ghostblade Item if Ultimate active
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level > 0)
            {
                Config.Item("GHOSTBLADE")
                    .SetValue(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LucianR");
            }

            //if (useQExtended && Q.IsReady())
            //{
            //    var t = TargetSelector.GetTarget(qExtended.Range, TargetSelector.DamageType.Physical);
            //    if (t.IsValidTarget() && QMinion.IsValidTarget())
            //    {
            //        if (!Orbwalking.InAutoAttackRange(t))
            //            Q.CastOnUnit(QMinion);
            //    }
            //}
        }

        public override void ExecuteLane()
        {
            int laneQValue = GetValue<StringList>("Lane.UseQ").SelectedIndex;
            if (laneQValue != 0)
            {
                var minion = Q.GetLineCollisionMinions(laneQValue);
                if (minion != null && !hasPassive)
                {
                    Q.CastOnUnit(minion);
                }

                var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                minion = allMinions.FirstOrDefault(minionn => minionn.Distance(ObjectManager.Player.Position) <= Q.Range && HealthPrediction.LaneClearHealthPrediction(minionn, (int)Q.Delay * 2) > 0);
                if (minion != null && !hasPassive)
                {
                    Q.CastOnUnit(minion);
                }
            }

            int laneWValue = GetValue<StringList>("Lane.UseW").SelectedIndex;
            if (laneWValue != 0 && W.IsReady() && !hasPassive)
            {
                Vector2 minions = W.GetLineFarmMinions(laneWValue);
                if (minions != Vector2.Zero)
                {
                    W.Cast(minions);
                }
            }
        }

        public override void ExecuteJungle()
        {
            var jungleQValue = GetValue<StringList>("Jungle.UseQ").SelectedIndex;
            if (jungleQValue != 0 && Q.IsReady())
            {
                var bigMobsQ = Utils.Utils.GetMobs(Q.Range, jungleQValue == 2 ? Utils.Utils.MobTypes.BigBoys : Utils.Utils.MobTypes.All);
                if (bigMobsQ != null && bigMobsQ.Health > ObjectManager.Player.TotalAttackDamage * 2 && !hasPassive)
                {
                    Q.CastOnUnit(bigMobsQ);
                }
            }

            var jungleWValue = GetValue<StringList>("Jungle.UseW").SelectedIndex;
            if (jungleWValue != 0 && W.IsReady())
            {
                var bigMobsQ = Utils.Utils.GetMobs(W.Range, jungleWValue == 2 ? Utils.Utils.MobTypes.BigBoys : Utils.Utils.MobTypes.All);
                if (bigMobsQ != null && bigMobsQ.Health > ObjectManager.Player.TotalAttackDamage * 2 && !hasPassive)
                {
                    W.Cast(bigMobsQ.Position);
                }
            }

            var jungleEValue = GetValue<StringList>("Jungle.UseE").SelectedIndex;
            if (jungleEValue != 0 && E.IsReady())
            {
                var jungleMobs =
                    Marksman.Utils.Utils.GetMobs(Q.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                        Marksman.Utils.Utils.MobTypes.All);

                if (jungleMobs != null && !hasPassive)
                {
                    switch (GetValue<StringList>("Jungle.UseE").SelectedIndex)
                    {
                        case 1:
                            {
                                if (!jungleMobs.BaseSkinName.ToLower().Contains("baron") ||
                                    !jungleMobs.BaseSkinName.ToLower().Contains("dragon"))
                                {
                                    if (jungleMobs.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                                        E.Cast(
                                            jungleMobs.IsValidTarget(
                                                Orbwalking.GetRealAutoAttackRange(null) + 65)
                                                ? Game.CursorPos
                                                : jungleMobs.Position);
                                }
                                break;
                            }

                        case 2:
                            {
                                if (!jungleMobs.BaseSkinName.ToLower().Contains("baron") ||
                                    !jungleMobs.BaseSkinName.ToLower().Contains("dragon"))
                                {
                                    jungleMobs =
                                        Marksman.Utils.Utils.GetMobs(
                                            E.Range + Orbwalking.GetRealAutoAttackRange(null) + 65,
                                            Marksman.Utils.Utils.MobTypes.BigBoys);
                                    if (jungleMobs != null)
                                    {
                                        E.Cast(
                                            jungleMobs.IsValidTarget(
                                                Orbwalking.GetRealAutoAttackRange(null) + 65)
                                                ? Game.CursorPos
                                                : jungleMobs.Position);
                                    }
                                }
                                break;
                            }
                    }
                }
            }
        }

        private static float GetRTotalDamage(AIHeroClient t)
        {
            if (!R.IsReady())
                return 0f;

            var baseAttackSpeed = 0.638;
            var wCdTime = 3;
            var passiveDamage = 0;

            var attackSpeed = (float)Math.Round(Math.Floor(1 / ObjectManager.Player.AttackDelay * 100) / 100, 2, MidpointRounding.ToEven);

            var RLevel = new[] { 7.5, 9, 10.5 };
            var shoots = 7.5 + RLevel[R.Level - 1];
            var shoots2 = shoots * attackSpeed;

            var aDmg = Math.Round(Math.Floor(ObjectManager.Player.GetAutoAttackDamage(t) * 100) / 100, 2, MidpointRounding.ToEven);
            aDmg = Math.Floor(aDmg);

            var totalAttackSpeedWithWActive = (float)Math.Round((attackSpeed + baseAttackSpeed / 100) * 100 / 100, 2, MidpointRounding.ToEven);

            var totalPossibleDamage = (float)Math.Round((totalAttackSpeedWithWActive * wCdTime * aDmg) * 100 / 100, 2, MidpointRounding.ToEven);

            return totalPossibleDamage + (float)passiveDamage;
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQC" + Id, "Q:").SetValue(true));
            config.AddItem(new MenuItem("UseQExtendedC" + Id, "Q Extended:").SetValue(new StringList(new[] { "Off", "Use for Selected Target", "Use for Any Target" }, 1)));
            config.AddItem(new MenuItem("Combo.W.Use" + Id, "W:").SetValue(true));
            //config.AddItem(new MenuItem("Combo.E.Use" + Id, "E:").SetValue(new StringList(new []{ "Off", "On", "On: Protect AA Range" }, 2)));
            config.AddItem(new MenuItem("Combo.E.Use" + Id, "E:").SetValue(true));
            //config.AddItem(new MenuItem("UseRC" + Id, "E:").SetValue(true));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQTH" + Id, "Use Q (Toggle)").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle))).Permashow(true);
            config.AddItem(new MenuItem("UseQExtendedTH" + Id, "Use Ext. Q (Toggle)").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle))).Permashow(true);
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddItem(new MenuItem("Passive" + Id, "Check Passive").SetValue(true));
            config.AddItem(new MenuItem("Misc.E.Antigapcloser" + Id, "E: Antigapcloser").SetValue(true));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("DrawQ" + Id, "Q range").SetValue(new Circle(true, Color.Gray)));
            config.AddItem(new MenuItem("DrawQ2" + Id, "Ext. Q range").SetValue(new Circle(true, Color.Gray)));
            config.AddItem(new MenuItem("DrawW" + Id, "W range").SetValue(new Circle(false, Color.Gray)));
            config.AddItem(new MenuItem("DrawE" + Id, "E range").SetValue(new Circle(false, Color.Gray)));
            config.AddItem(new MenuItem("DrawR" + Id, "R range").SetValue(new Circle(false, Color.Chocolate)));

            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Damage After Combo").SetValue(true);
            config.AddItem(dmgAfterComboItem);
            
            //LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged +=
                (sender, args) => { LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = args.GetNewValue<bool>(); };
            return true;
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                var toPolygon = new CommonGeometry.Rectangle(ObjectManager.Player.Position.To2D(), ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), R.Range), 80).ToPolygon();
                toPolygon.Draw(System.Drawing.Color.Red, 1);
            }

            var enemyCount =
                HeroManager.Enemies.Where(e => e.Distance(ObjectManager.Player) < 1100 && e.IsValidTarget(1100))
                    .Count(e => e.NetworkId != t.NetworkId && !e.IsDead && toPolygon.IsInside(e.ServerPosition));


            foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget(R.Range * 2)))
            {
                var heropos = Drawing.WorldToScreen(e.Position);
                Drawing.DrawText(heropos.X, heropos.Y, Color.Red, GetRTotalDamage(e).ToString(CultureInfo.InvariantCulture));
            }

            Spell[] spellList = { Q, qExtended, W, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (!menuItem.Active || spell.Level < 0 && spell.IsReady()) return;

                Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }
        }


        public override bool LaneClearMenu(Menu menuLane)
        {
            string[] strQ = new string[5];
            strQ[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strQ[i] = "Minion Count >= " + i;
            }

            menuLane.AddItem(new MenuItem("Lane.UseQ" + Id, "Q:").SetValue(new StringList(strQ, 3))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            menuLane.AddItem(new MenuItem("Lane.UseQ2" + Id, "Q Extended:").SetValue(new StringList(new[] { "Off", "Out of AA Range" }, 1))).SetFontStyle(FontStyle.Regular, Q.MenuColor());

            string[] strW = new string[5];
            strW[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strW[i] = "Minion Count >= " + i;
            }

            menuLane.AddItem(new MenuItem("Lane.UseW" + Id, "W:").SetValue(new StringList(strW, 3))).SetFontStyle(FontStyle.Regular, W.MenuColor());

            menuLane.AddItem(new MenuItem("Lane.UseE" + Id, "E:").SetValue(new StringList(new[] { "Off", "Under Ally Turrent Farm", "Out of AA Range", "Both" }, 1))).SetFontStyle(FontStyle.Regular, E.MenuColor());


            string[] strR = new string[4];
            strR[0] = "Off";

            for (var i = 1; i < 4; i++)
            {
                strR[i] = "Minion Count >= Ulti Attack Count x " + i.ToString();
            }
            menuLane.AddItem(new MenuItem("Lane.UseR" + Id, "R:").SetValue(new StringList(strR, 2))).SetFontStyle(FontStyle.Regular, R.MenuColor());


            return true;
        }

        public override bool JungleClearMenu(Menu menuJungle)
        {
            menuJungle.AddItem(new MenuItem("Jungle.UseQ" + Id, "Q:").SetValue(new StringList(new[] { "Off", "On", "Just big Monsters" }, 2)));
            menuJungle.AddItem(new MenuItem("Jungle.UseW" + Id, "W:").SetValue(new StringList(new[] { "Off", "On", "Just big Monsters" }, 2)));
            menuJungle.AddItem(new MenuItem("Jungle.UseE" + Id, "E:").SetValue(new StringList(new[] { "Off", "On", "Just big Monsters" }, 2)));

            return true;
        }

        public override void PermaActive()
        {
            if (aaCount <= 0)
            {
                hasPassive = false;
            }
            if (Environment.TickCount > PassiveStartedTime + 3000)
            {
                hasPassive = false;
                aaCount = 0;
            }

            base.PermaActive();
        }
    }
}
