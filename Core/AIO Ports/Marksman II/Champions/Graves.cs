#region

using System;
using System.Collections.Generic;
using System.Drawing;
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
    //gravesbasicattackammo1
    //gravesbasicattackammo2
    internal class Graves : Champion
    {
        public Spell Q { get; private set; }
        public Spell W { get; private set; }
        public Spell E { get; private set; }
        public Spell R { get; private set; }
        public Spell R2 { get; private set; }

        private bool isReloading;
        private int reloadingTime;

        private static bool HaveAmmo1 => ObjectManager.Player.HasBuff("gravesbasicattackammo1");
        private static bool HaveAmmo2 => ObjectManager.Player.HasBuff("gravesbasicattackammo2");
        

        public Graves()
        {
            Q = new Spell(SpellSlot.Q, 920f); // Q likes to shoot a bit too far away, so moving the range inward.
            Q.SetSkillshot(0.26f, 10f*2*(float) Math.PI/180, 1950, false, SkillshotType.SkillshotCone);

            W = new Spell(SpellSlot.W, 1100f);
            W.SetSkillshot(0.30f, 250f, 1650f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 425f);

            R = new Spell(SpellSlot.R, 1100f);
            R.SetSkillshot(0.25f, 110f, 2100f, false, SkillshotType.SkillshotLine);

            R2 = new Spell(SpellSlot.R, 700f);
            R2.SetSkillshot(0f, 110f, 1500f, false, SkillshotType.SkillshotCone);

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;

            Drawing.OnDraw += DrawingOnOnDraw;

            Utils.Utils.PrintMessage("Graves.");
        }

        public override void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (E.IsReady())
            {
                BuffInstance aBuff =
                    (from fBuffs in
                        sender.Buffs.Where(
                            s =>
                                sender.Team != ObjectManager.Player.Team
                                && sender.Distance(ObjectManager.Player.Position) < W.Range)
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
                    W.Cast(sender.Position);
                }
            }
        }

        public override void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation.ToLower().Contains("gravesreloadupper"))
                {
                    reloadingTime = Environment.TickCount;
                    isReloading = true;
                }
            }
        }

        public override void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (GetValue<bool>("Combo.E.Use") && E.IsReady() && HaveAmmo1 && !HaveAmmo2 && !isReloading)
            {
                var enemy = HeroManager.Enemies.FirstOrDefault(e => e.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65));
                if (enemy != null)
                {
                    E.Cast(CommonUtils.GetDashPosition(E, enemy, 200));
                }
            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser args)
        {
            if (GetValue<bool>("Misc.W.Gapcloser") && W.IsReady())
            {
                if (args.End.Distance(ObjectManager.Player.Position) <= 300)
                {
                    W.Cast(args.End);
                }
            }

            if (GetValue<bool>("Misc.E.Gapcloser") && E.IsReady())
            {
                if (args.End.Distance(ObjectManager.Player.Position) <= 200)
                {
                    if (args.Sender.IsValidTarget())
                    {
                        if (args.Sender.ChampionName.ToLowerInvariant() != "masteryi")
                        {
                            E.Cast(ObjectManager.Player.Position.Extend(args.Sender.Position, -E.Range));
                        }
                        //E.Cast(args.End.Extend(ObjectManager.Player.Position, args.End.Distance(ObjectManager.Player.Position) + E.Range));
                    }
                }
            }
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (GetValue<bool>("Misc.E.AntiMelee") && E.IsReady())
            {
                if (sender != null && args.Target != null && args.Target.IsMe && sender.Type == GameObjectType.AIHeroClient && sender.IsEnemy && sender.IsMelee && args.SData.IsAutoAttack())
                {
                    E.Cast(ObjectManager.Player.Position.Extend(sender.Position, -E.Range));
                }
            }
        }

        private void QFarmLogic(List<Obj_AI_Base> minions, int min)
        {
            if (!Q.IsReady() || minions.Count == 0)
            {
                return;
            }
            var totalHits = 0;
            var castPos = Vector3.Zero;

            var positions = (from minion in minions
                let pred = Q.GetPrediction(minion)
                where pred.Hitchance >= HitChance.Medium
                where !CommonUtils.IsWallBetween(ObjectManager.Player.Position, pred.UnitPosition)
                select new Tuple<Obj_AI_Base, Vector3>(minion, pred.UnitPosition)).ToList();

            if (positions.Any())
            {
                foreach (var position in positions)
                {
                    var rect = new Geometry.Polygon.Rectangle(ObjectManager.Player.Position,
                        ObjectManager.Player.Position.Extend(position.Item2, Q.Range), Q.Width);
                    var count =
                        positions.Select(
                            position2 =>
                                new Geometry.Polygon.Circle(position2.Item2, position2.Item1.BoundingRadius*0.9f))
                            .Count(circle => circle.Points.Any(p => rect.IsInside(p)));
                    if (count > totalHits)
                    {
                        totalHits = count;
                        castPos = position.Item2;
                    }
                    if (totalHits == minions.Count)
                    {
                        break;
                    }
                }
                if (!castPos.Equals(Vector3.Zero) && totalHits >= min)
                {
                    Q.Cast(castPos);
                }
            }
        }

        private bool RLogic(AIHeroClient target)
        {
            var hits = GetRHits(target);
            if ((hits.Item2.Any(h => R.GetDamage(h) * 0.95f > h.Health) ||
                    hits.Item2.Any(h => h.Distance(ObjectManager.Player) + 300 < Orbwalking.GetRealAutoAttackRange(h) * 0.9f)))
            {
                R.Cast(hits.Item3);
                return true;
            }
            return false;
        }

        private Tuple<int, List<AIHeroClient>, Vector3> GetRHits(AIHeroClient target)
        {
            var hits = new List<AIHeroClient>();
            var castPos = Vector3.Zero;
            var pred = R.GetPrediction(target);
            if (pred.Hitchance >= R.GetHitchance())
            {
                castPos = pred.CastPosition;
                hits.Add(target);
                var pos = ObjectManager.Player.Position.Extend(castPos,
                    Math.Min(ObjectManager.Player.Distance(pred.UnitPosition), R.Range));
                var pos2 = ObjectManager.Player.Position.Extend(pos, ObjectManager.Player.Distance(pos) + R2.Range);

                var input = new PredictionInput
                {
                    Range = R2.Range,
                    Delay = ObjectManager.Player.Position.Distance(pred.UnitPosition)/R.Speed + 0.1f,
                    From = pos,
                    RangeCheckFrom = pos,
                    Radius = R2.Width,
                    Type = SkillshotType.SkillshotLine,
                    Speed = R2.Speed
                };

                var rect = new Geometry.Polygon.Rectangle(pos, pos2, R2.Width);

                foreach (var enemy in
                    HeroManager.Enemies.Where(e => e.IsValidTarget() && e.NetworkId != target.NetworkId))
                {
                    input.Unit = enemy;
                    var pred2 = Prediction.GetPrediction(input);
                    if (!pred2.UnitPosition.Equals(Vector3.Zero))
                    {
                        if (
                            new Geometry.Polygon.Circle(enemy.Position, enemy.BoundingRadius).Points.Any(
                                p => rect.IsInside(p)))
                        {
                            hits.Add(enemy);
                        }
                    }
                }
            }
            return new Tuple<int, List<AIHeroClient>, Vector3>(hits.Count, hits, castPos);
        }


        private void DrawingOnOnDraw(EventArgs args)
        {
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (isReloading)
                Drawing.DrawText(heropos.X, heropos.Y, Color.Red, "Reloading...");
            return;
            var t = TargetSelector.GetTarget(Q.Range * 5, TargetSelector.DamageType.Magical);
            if (!t.IsValidTarget())
            {
                return;
            }

            if (Q.IsReady())
            {

                var toPolygon = new Marksman.Common.CommonGeometry.Rectangle(ObjectManager.Player.Position.To2D(), ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), Q.Range - 200), 50).ToPolygon();
                toPolygon.Draw(System.Drawing.Color.Red, 2);
                
                if (toPolygon.IsInside(t))
                {
                    Render.Circle.DrawCircle(t.Position, t.BoundingRadius, Color.Black);
                    Q.Cast(t);
                }

                var xPos = ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), Q.Range);

                var toPolygon2 = new Marksman.Common.CommonGeometry.Rectangle(xPos, ObjectManager.Player.Position.To2D().Extend(t.Position.To2D(), Q.Range - 195), 260).ToPolygon();
                toPolygon2.Draw(System.Drawing.Color.Red, 2);

                if (toPolygon2.IsInside(t))
                {
                    Render.Circle.DrawCircle(t.Position, t.BoundingRadius, Color.Black);
                    Q.Cast(t);
                }
            }

        }

        private float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;
            if (!ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                fComboDamage += (float) ObjectManager.Player.GetAutoAttackDamage(t, true);
            }

            if (Q.IsReady())
            {
                fComboDamage += Q.GetDamage(t);
            }

            if (W.IsReady())
            {
                fComboDamage += W.GetDamage(t);
            }

            if (R.IsReady())
            {
                fComboDamage += R.GetDamage(t);
            }

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot")) ==
                SpellState.Ready && ObjectManager.Player.Distance(t) < 550)
            {
                fComboDamage += (float) ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);
            }

            if (Items.CanUseItem(3144) && ObjectManager.Player.Distance(t) < 550)
            {
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Bilgewater);
            }

            if (Items.CanUseItem(3153) && ObjectManager.Player.Distance(t) < 550)
            {
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Botrk);
            }

            return fComboDamage;
        }

        protected void Killsteal()
        {
            if (GetValue<bool>("Misc.Q.KillSteal") && Q.IsReady())
            {
                var fPredEnemy =
                    HeroManager.Enemies.Where(e => e.IsValidTarget(Q.Range * 1.2f) && Q.IsKillable(e))
                        .Select(enemy => Q.GetPrediction(enemy, true))
                        .FirstOrDefault(pred => pred.Hitchance >= HitChance.High);
                if (fPredEnemy != null && !CommonUtils.IsWallBetween(ObjectManager.Player.Position, fPredEnemy.CastPosition))
                {
                    Q.Cast(fPredEnemy.CastPosition);
                }
            }
        }

        public override void GameOnUpdate(EventArgs args)
        {
            if (Environment.TickCount > reloadingTime + 2000)
            {
                isReloading = false;
            }
          
            if (GetValue<KeyBind>("UseQTH").Active && Q.IsReady() && !ComboActive && !ObjectManager.Player.HasBuff("Recall"))
            {
                var fPredEnemy =
                    HeroManager.Enemies.Where(e => e.IsValidTarget(Q.Range * 1.2f) && Q.IsKillable(e))
                        .Select(enemy => Q.GetPrediction(enemy, true))
                        .FirstOrDefault(pred => pred.Hitchance >= HitChance.High);
                if (fPredEnemy != null && !CommonUtils.IsWallBetween(ObjectManager.Player.Position, fPredEnemy.CastPosition))
                {
                    Q.Cast(fPredEnemy.CastPosition);
                }
            }
        }

        public override void ExecuteCombo()
        {
            if (GetValue<bool>("Combo.Q.Use.Urf") && Q.IsReady())
            {
                var fPredEnemy =
                    HeroManager.Enemies.Where(e => e.IsValidTarget(Q.Range * 1.2f))
                        .Select(enemy => Q.GetPrediction(enemy, true))
                        .FirstOrDefault(pred => pred.Hitchance >= HitChance.High);
                if (fPredEnemy != null && !CommonUtils.IsWallBetween(ObjectManager.Player.Position, fPredEnemy.CastPosition))
                {
                    Q.Cast(fPredEnemy.CastPosition);
                }
            }


            if (GetValue<bool>("Combo.Q.Use") && Q.IsReady())
            {
                var fPredEnemy =
                    HeroManager.Enemies.Where(e => e.IsValidTarget(Q.Range * 1.2f) && Q.IsKillable(e))
                        .Select(enemy => Q.GetPrediction(enemy, true))
                        .FirstOrDefault(pred => pred.Hitchance >= HitChance.High);
                if (fPredEnemy != null && !CommonUtils.IsWallBetween(ObjectManager.Player.Position, fPredEnemy.CastPosition))
                {
                    Q.Cast(fPredEnemy.CastPosition);
                }
            }

            if (GetValue<bool>("Combo.Q.Use") && Q.IsReady() && !HaveAmmo1)
            {
                var fPredEnemy =
                    HeroManager.Enemies.Where(e => e.IsValidTarget(Q.Range * 1.2f))
                        .Select(enemy => Q.GetPrediction(enemy, true))
                        .FirstOrDefault(pred => pred.Hitchance >= HitChance.High);
                if (fPredEnemy != null && !CommonUtils.IsWallBetween(ObjectManager.Player.Position, fPredEnemy.CastPosition))
                {
                    Q.Cast(fPredEnemy.CastPosition);
                }
            }
            if (GetValue<bool>("Combo.E.Use") && !Q.IsReady() && !HaveAmmo1 && E.IsReady())
            {
                var t = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget())
                {
                    E.Cast(CommonUtils.GetDashPosition(E, t, 200));
                }
            }

            if (GetValue<bool>("Combo.W.Use") && W.IsReady() && ObjectManager.Player.Mana > (Q.ManaCost + E.ManaCost + W.ManaCost + (R.Cooldown < 10 ? R.ManaCost:0)))
            {
                var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget(W.Range) && (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) || t.HasBuffOfType(BuffType.Taunt)))
                {
                    W.Cast(t, false, true);
                }
            }

            if (GetValue<bool>("Combo.W.Use") && W.IsReady() &&
                ObjectManager.Player.Mana > (Q.ManaCost + E.ManaCost + W.ManaCost + (R.Cooldown < 10 ? R.ManaCost : 0)))
            {
                var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget())
                {
                    W.Cast(t, false, true);
                }
            }

            if (GetValue<bool>("Combo.R.Use") && R.IsReady())
            {
                CastR();
            }

            base.ExecuteCombo();
        }

        void CastR()
        {
            foreach (var target in HeroManager.Enemies.Where(t => R.GetDamage(t)*0.95f > t.Health))
            {
                var hits = GetRHits(target);
                if (hits.Item1 > 0)
                {
                    R.Cast(hits.Item3);
                    break;
                }
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("Combo.Q.Use" + Id, "Q").SetValue(true));
            config.AddItem(new MenuItem("Combo.Q.Use.Urf" + Id, "Q: Urf Mode").SetValue(true));
            config.AddItem(new MenuItem("Combo.W.Use" + Id, "W").SetValue(true));
            config.AddItem(new MenuItem("Combo.E.Use" + Id, "E:").SetValue(true));
            config.AddItem(new MenuItem("Combo.R.Use" + Id, "R:").SetValue(true));
            config.AddItem(new MenuItem("CastR" + Id, "Cast R (Manual)").SetValue(new KeyBind("T".ToCharArray()[0],KeyBindType.Press)));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "Use Q").SetValue(true));
            config.AddItem(new MenuItem("UseQTH" + Id, "Use Q (Toggle)").SetValue(new KeyBind("H".ToCharArray()[0],KeyBindType.Toggle))).Permashow(true, "Marksman | Toggle Q");
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("Draw.Q" + Id, "Q:").SetValue(new Circle(true,Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(new MenuItem("Draw.W" + Id, "W:").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(new MenuItem("Draw.E" + Id, "E:").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(new MenuItem("Draw.R" + Id, "R:").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            return true;
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            Spell[] spellList = { Q, W, E, R };
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw." + spell.Slot);
                if (!menuItem.Active || spell.Level < 0 && spell.IsReady())
                {
                    return;
                }

                Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, spell.IsReady() ? menuItem.Color: Color.Gray);
            }
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddItem(new MenuItem("Misc.Q.KillSteal" + Id, "R: Kill Steal").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            config.AddItem(new MenuItem("Misc.W.Gapcloser" + Id, "W: Gapcloser").SetValue(true)).SetFontStyle(FontStyle.Regular, W.MenuColor());
            config.AddItem(new MenuItem("Misc.E.Gapcloser" + Id, "E: Gapcloser").SetValue(true)).SetFontStyle(FontStyle.Regular, E.MenuColor());
            config.AddItem(new MenuItem("Misc.E.AntiMelee" + Id, "E: Anti-Melee").SetValue(true)).SetFontStyle(FontStyle.Regular, E.MenuColor());
            return true;
        }


        public override bool LaneClearMenu(Menu menuLane)
        {
            string[] strQ = new string[5];
            strQ[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strQ[i] = "Minion Count >= " + i;
            }

            menuLane.AddItem(new MenuItem("Lane.Q.Use" + Id, "Q:").SetValue(new StringList(strQ, 4))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            return true;
        }

        public override void ExecuteLane()
        {
            if (GetValue<StringList>("Lane.Q.Use").SelectedIndex != 0)
            {
                QFarmLogic(MinionManager.GetMinions(Q.Range), GetValue<StringList>("Lane.Q.Use").SelectedIndex);
            }
            base.ExecuteLane();
        }

        public override bool JungleClearMenu(Menu menuJungle)
        {
            string[] strQ = new string[5];
            strQ[0] = "Off";
            strQ[1] = "Just for Big Mobs";

            for (var i = 2; i < 5; i++)
            {
                strQ[i] = "Mob Count >= " + i;
            }

            menuJungle.AddItem(new MenuItem("Jungle.Q.Use" + Id, "Q:").SetValue(new StringList(strQ, 4))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            menuJungle.AddItem(new MenuItem("Jungle.E.Use" + Id, "E:").SetValue(true)).SetFontStyle(FontStyle.Regular, E.MenuColor());
            return true;
        }

        public override void ExecuteJungle()
        {
            var useQ = GetValue<StringList>("Jungle.Q.Use").SelectedIndex;
            if (useQ != 0 && Q.IsReady() && isReloading)
            {
                var jungleMobs = Utils.Utils.GetMobs(E.Range);

                if (useQ == 1 && !CommonUtils.IsWallBetween(ObjectManager.Player.Position, jungleMobs.Position))
                {
                    Q.Cast(jungleMobs.Position);
                }
                else
                {
                    QFarmLogic(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth), useQ);
                }
            }
            
            if (GetValue<bool>("Jungle.E.Use") && E.IsReady() && HaveAmmo1 && !HaveAmmo2 && !isReloading)
            {
                E.Cast(Game.CursorPos);
            }
            base.ExecuteJungle();
        }

        public override void PermaActive()
        {
            Orbwalker.SetAttack(!isReloading);
            Killsteal();

            base.PermaActive();
        }
    }
}
