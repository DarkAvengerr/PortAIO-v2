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
using Orbwalking = LeagueSharp.Common.Orbwalking;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    internal class Caitlyn : Champion
    {
        public static Spell R;

        public static Spell E;

        public static Spell W;

        private static AIHeroClient blitz = null;

        private static int LastCastWTick;

        private static readonly List<GameObject> trapList = new List<GameObject>();

        private bool canCastR = true;

        // private static bool headshotReady = ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "CaitlynHeadshotReady");

        private string[] dangerousEnemies =
        {
            "Alistar", "Garen", "Zed", "Fizz", "Rengar", "JarvanIV", "Irelia", "Amumu", "DrMundo", "Ryze", "Fiora", "KhaZix", "LeeSin", "Riven", "Lissandra", "Vayne", "Lucian", "Zyra"
        };

        public Caitlyn()
        {
            Q = new Spell(SpellSlot.Q, 1240);
            W = new Spell(SpellSlot.W, 820);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 2000);

            Q.SetSkillshot(0.50f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

            foreach (var hero in HeroManager.Allies)
            {
                if (hero.ChampionName.Equals("Blitzcrank"))
                {
                    blitz = hero;
                    Chat.Print("Blitz Detected!");
                }
            }

            Obj_AI_Base.OnBuffGain += (sender, args) =>
            {
                // return;

                if (W.IsReady())
                {
                    var aBuff =
                        (from fBuffs in
                            sender.Buffs.Where(
                                s =>
                                    sender.IsEnemy
                                    && sender.Distance(ObjectManager.Player.Position) <= W.Range)
                            from b in new[]
                            {
                                "teleport", /* Teleport */
                                "pantheon_grandskyfall_jump", /* Pantheon */ 
                                "crowstorm", /* FiddleScitck */
                                "zhonya", "katarinar", /* Katarita */
                                "MissFortuneBulletTime", /* MissFortune */
                                "gate", /* Twisted Fate */
                                "chronorevive" /* Zilean */
                            }
                            where args.Buff.Name.ToLower().Contains(b)
                            select fBuffs).FirstOrDefault();

                    if (aBuff != null && aBuff.StartTime + CommonUtils.GetRandomDelay(250, 1000) <= Game.Time)
                        //if (aBuff != null && aBuff.StartTime + CommonUtils.GetRandomDelay(250, 1000) <= Game.Time)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(CommonUtils.GetRandomDelay(250, 1000), () =>
                        {
                            CastW(sender.Position);
                        });
                        //W.Cast(sender.Position);
                    }
                }
            };

            Utils.Utils.PrintMessage("Caitlyn");
        }


        public override void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (GetValue<bool>("Misc.W.Interrupt"))
                if (args.DangerLevel >= Interrupter2.DangerLevel.Medium)
                    if (sender.IsValidTarget(W.Range))
                        if (W.IsReady())
                            if (!trapList.Any(x => x.IsValid && sender.Position.Distance(x.Position) <= 100))
                                W.Cast(sender.Position);
        }
        public static Spell Q { get; set; }

        public override void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly)
                if (sender.Name == "Cupcake Trap")
                    trapList.RemoveAll(x => x.NetworkId == sender.NetworkId);
        }

        public override void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly)
                if (sender.Name == "Cupcake Trap")
                    trapList.Add(sender);
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Program.Config.Item("Misc.AntiGapCloser").GetValue<bool>())
            {
                return;
            }

                if (gapcloser.End.Distance(ObjectManager.Player.Position) <= 200)
                    if (gapcloser.Sender.IsValidTarget(E.Range))
                        if (E.IsReady())
                            E.Cast(gapcloser.Sender.Position);

        }

        public override void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            //if (args.Slot == SpellSlot.W && LastCastWTick + 2000 > Utils.TickCount)
            //{
            //    args.Process = false;
            //}
            //else
            //{
            //    args.Process = true;
            //}

            //if (args.Slot == SpellSlot.Q)
            //{
            //    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && GetValue<bool>("UseQC"))
            //    {
            //        var t = TargetSelector.GetTarget(Q.Range - 20, TargetSelector.DamageType.Physical);
            //        if (!t.IsValidTarget())
            //        {
            //            args.Process = false;
            //        }
            //        else
            //        {
            //            args.Process = true;
            //            //CastQ(t);
            //        }
            //    }
            //}
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender is Obj_AI_Turret && args.Target.IsMe)
            {
                canCastR = false;
            }
            else
            {
                canCastR = true;
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (t.IsValidTarget())
            {
                Render.Circle.DrawCircle(t.Position, 105f, Color.GreenYellow);

                var wcCenter = ObjectManager.Player.Position.Extend(t.Position,
                    ObjectManager.Player.Distance(t.Position)/2);

                var wcLeft = ObjectManager.Player.Position.To2D() +
                             Vector2.Normalize(t.Position.To2D() - ObjectManager.Player.Position.To2D())
                                 .Rotated(ObjectManager.Player.Distance(t.Position) < 300
                                     ? 45
                                     : 37*(float) Math.PI/180)*ObjectManager.Player.Distance(t.Position)/2;

                var wcRight = ObjectManager.Player.Position.To2D() +
                              Vector2.Normalize(t.Position.To2D() - ObjectManager.Player.Position.To2D())
                                  .Rotated(ObjectManager.Player.Distance(t.Position) < 300
                                      ? -45
                                      : -37*(float) Math.PI/180)*ObjectManager.Player.Distance(t.Position)/2;

                Render.Circle.DrawCircle(wcCenter, 50f, Color.Red);
                Render.Circle.DrawCircle(wcLeft.To3D(), 50f, Color.Green);
                Render.Circle.DrawCircle(wcRight.To3D(), 50f, Color.Yellow);
            }
            //var bx = HeroManager.Enemies.Where(e => e.IsValidTarget(E.Range * 3));
            //foreach (var n in bx)
            //{
            //    if (n.IsValidTarget(800) && ObjectManager.Player.Distance(n) < 450)
            //    {
            //        Vector3[] x = new[] { ObjectManager.Player.Position, n.Position };
            //        Vector2 aX =
            //            Drawing.WorldToScreen(new Vector3(CommonGeometry.CenterOfVectors(x).X,
            //                CommonGeometry.CenterOfVectors(x).Y, CommonGeometry.CenterOfVectors(x).Z));

            //        Render.Circle.DrawCircle(CommonGeometry.CenterOfVectors(x), 85f, Color.White );
            //        Drawing.DrawText(aX.X - 15, aX.Y - 15, Color.GreenYellow, n.ChampionName);

            //    }
            //}

            //var enemies = HeroManager.Enemies.Where(e => e.IsValidTarget(1500));
            //var objAiHeroes = enemies as AIHeroClient[] ?? enemies.ToArray();
            //IEnumerable<AIHeroClient> nResult =
            //    (from e in objAiHeroes join d in dangerousEnemies on e.ChampionName equals d select e)
            //        .Distinct();

            //foreach (var n in nResult)
            //{
            //    var x = E.GetPrediction(n).CollisionObjects.Count;
            //    Render.Circle.DrawCircle(n.Position, (Orbwalking.GetRealAutoAttackRange(null) + 65) - 300, Color.GreenYellow);
            //}

            var nResult = HeroManager.Enemies.Where(e => e.IsValidTarget(E.Range*2));
            foreach (var n in nResult.Where(n => n.IsFacing(ObjectManager.Player)))
            {
                Render.Circle.DrawCircle(n.Position, E.Range - 200, Color.Red, 1);
            }

            Spell[] spellList = {Q, W, E, R};
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }
            }
        }

        private static void CastQ(Obj_AI_Base t)
        {
            if (Q.CanCast(t))
            {
                var qPrediction = Q.GetPrediction(t);
                var hithere = qPrediction.CastPosition.Extend(ObjectManager.Player.Position, -100);

                if (qPrediction.Hitchance >= Q.GetHitchance())
                {
                    Q.Cast(hithere);
                }
            }
        }

        bool CastE()
        {
            if (!E.IsReady())
            {
                return false;
            }

            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (t != null)
            {
                var nPrediction = E.GetPrediction(t);
                var nHitPosition = nPrediction.CastPosition.Extend(ObjectManager.Player.Position, -130);
                if (nPrediction.Hitchance >= HitChance.High)
                {
                    E.Cast(nHitPosition);
                }
            }
            if (t != null)
            {
                return E.Cast(t) == Spell.CastStates.SuccessfullyCasted;
            }

            return false;
        }

        private static void CastE(Obj_AI_Base t)
        {
            if (E.CanCast(t))
            {
                var pred = E.GetPrediction(t);
                var hithere = pred.CastPosition.Extend(ObjectManager.Player.Position, -100);

                if (pred.Hitchance >= E.GetHitchance())
                {
                    E.Cast(hithere);
                }
            }
        }


        private static void CastW(Vector3 pos, bool delayControl = true)
        {
            var enemy =
                HeroManager.Enemies.Find(
                    e =>
                        e.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) &&
                        e.Health < ObjectManager.Player.TotalAttackDamage*2);
            if (enemy != null)
            {
                return;
            }

            if (!W.IsReady())
            {
                return;
            }

            //if (headshotReady)
            //{
            //    return;
            //}

            if (delayControl && LastCastWTick + 2000 > LeagueSharp.Common.Utils.TickCount)
            {
                return;
            }

            if (!trapList.Any(x => x.IsValid && pos.Distance(x.Position) <= 100))
                W.Cast(pos);

            //W.Cast(pos);
            LastCastWTick = LeagueSharp.Common.Utils.TickCount;
        }

        private static void CastW2(Obj_AI_Base t)
        {
            if (t.IsValidTarget(W.Range))
            {
                BuffType[] buffList =
                {
                    BuffType.Fear,
                    BuffType.Taunt,
                    BuffType.Stun,
                    BuffType.Slow,
                    BuffType.Snare
                };

                foreach (var b in buffList.Where(t.HasBuffOfType))
                {
                    CastW(t.Position, false);
                }
            }
        }

        private static bool EnemyHasBuff(AIHeroClient t)
        {
            BuffType[] buffList =
            {
                BuffType.Knockup,
                BuffType.Taunt,
                BuffType.Stun,
                BuffType.Snare
            };

            return buffList.Where(t.HasBuffOfType).Any();
        }

        public override void DrawingOnEndScene(EventArgs args)
        {
            //if (GetValue<bool>("UseEC") && E.IsReady())
            //{
            //    var nResult = HeroManager.Enemies.Where(e => e.IsValidTarget(E.Range) && E.GetPrediction(e).CollisionObjects.Count == 0).OrderBy(e => e.Distance(ObjectManager.Player)).FirstOrDefault();
            //    if (nResult != null)
            //    {
            //        var isMelee = nResult.AttackRange < 400;
            //        if (isMelee)
            //        {
            //            Render.Circle.DrawCircle(nResult.Position, nResult.BoundingRadius, Color.DarkCyan, 3);
            //            if (ObjectManager.Player.Distance(nResult) < nResult.AttackRange * 2)
            //            {
            //                CastE(nResult);
            //            }
            //        }
            //        else
            //        {
            //            Render.Circle.DrawCircle(nResult.Position, nResult.BoundingRadius, Color.Gold, 3);
            //            if (nResult.IsValidTarget(nResult.IsFacing(ObjectManager.Player) ? E.Range - 200 : E.Range - 400))
            //            {
            //                CastE(nResult);
            //            }
            //        }
            //    }
            //}


            //var enemies =
            //    HeroManager.Enemies.Count(
            //        e => e.Health <= R.GetDamage(e) && !e.IsDead && !e.IsZombie && e.IsValidTarget(R.Range));
            //if (enemies > 0)
            //{
            //    for (var i = 0; i < enemies; i++)

            //    {
            //        var a1 = (i + 1) * 0.025f;

            //        CommonGeometry.DrawBox(new Vector2(Drawing.Width * 0.43f, Drawing.Height * (0.700f + a1)), 150, 18,
            //            Color.FromArgb(170, 255, 0, 0), 1, Color.Black);

            //        CommonGeometry.Text.DrawTextCentered(HeroManager.Enemies[i].ChampionName + " Killable",
            //            (int)(Drawing.Width * 0.475f), (int)(Drawing.Height * (0.803f + a1 - 0.093f)), SharpDX.Color.Wheat);
            //    }
            //}
            foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget(W.Range)))
            {
                if (EnemyHasBuff(e))
                {
                    var pos =
                        e.Position.To2D()
                            .Extend(ObjectManager.Player.Position.To2D(),
                                ObjectManager.Player.HealthPercent < e.HealthPercent ? 100 : -100)
                            .To3D();
                    Render.Circle.DrawCircle(pos, 50f, Color.Chartreuse);

                    //if (pos.Distance(ObjectManager.Player.Position) <= W.Range)
                    //{
                    //    CastW(pos);
                    //}
                }
            }

            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            var x = 0;
            foreach (var b in ObjectManager.Player.Buffs.Where(buff => buff.DisplayName == "CaitlynHeadshotCount"))
            {
                x = b.Count;
            }

            for (var i = 1; i < 7; i++)
            {
                CommonGeometry.DrawBox(
                    new Vector2(ObjectManager.Player.HPBarPosition.X + 23 + i*17,
                        ObjectManager.Player.HPBarPosition.Y + 25), 15, 4, Color.Transparent, 1, Color.Black);
            }
            var headshotReady = ObjectManager.Player.Buffs.Any(buff => buff.DisplayName == "CaitlynHeadshotReady");
            for (var i = 1; i < (headshotReady ? 7 : x + 1); i++)
            {
                CommonGeometry.DrawBox(
                    new Vector2(ObjectManager.Player.HPBarPosition.X + 24 + i*17,
                        ObjectManager.Player.HPBarPosition.Y + 26), 13, 3, headshotReady ? Color.Red : Color.LightGreen,
                    0, Color.Black);
            }

            var rCircle2 = Program.Config.Item("Draw.UltiMiniMap").GetValue<Circle>();
            if (rCircle2.Active)
            {
#pragma warning disable 618
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, rCircle2.Color, 1, 23, true);
#pragma warning restore 618
            }
        }

      
        public override void GameOnUpdate(EventArgs args)
        {

            R.Range = 500*(R.Level == 0 ? 1 : R.Level) + 1500;

            AIHeroClient t;

            if (GetValue<StringList>("AutoWI").SelectedIndex != 0 && W.IsReady())
            {
                foreach (
                    var hero in
                        HeroManager.Enemies.Where(h => h.IsValidTarget(W.Range) && h.HasBuffOfType(BuffType.Stun)))
                {
                    CastW(hero.Position, false);
                }
            }

            if (W.IsReady() &&
                (GetValue<StringList>("AutoWI").SelectedIndex == 1 ||
                 (GetValue<StringList>("AutoWI").SelectedIndex == 2 && ComboActive)))
            {
                t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget(W.Range))
                {
                    if (t.HasBuffOfType(BuffType.Stun) || t.HasBuffOfType(BuffType.Snare) ||
                        t.HasBuffOfType(BuffType.Taunt) || t.HasBuffOfType(BuffType.Knockup) ||
                        t.HasBuff("zhonyasringshield") || t.HasBuff("Recall"))
                    {
                        CastW(t.Position);
                    }

                    if (t.HasBuffOfType(BuffType.Slow) && t.IsValidTarget(E.Range - 200))
                    {
                        //W.Cast(t.Position.Extend(ObjectManager.Player.Position, +200));
                        //W.Cast(t.Position.Extend(ObjectManager.Player.Position, -200));

                        var hit = t.IsFacing(ObjectManager.Player)
                            ? t.Position.Extend(ObjectManager.Player.Position, +200)
                            : t.Position.Extend(ObjectManager.Player.Position, -200);
                        CastW(hit);
                    }
                }
            }

            if (Q.IsReady() &&
                (GetValue<StringList>("AutoQI").SelectedIndex == 1 ||
                 (GetValue<StringList>("AutoQI").SelectedIndex == 2 && ComboActive)))
            {
                t = TargetSelector.GetTarget(Q.Range - 30, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget(Q.Range)
                    &&
                    (t.HasBuffOfType(BuffType.Stun) ||
                     t.HasBuffOfType(BuffType.Snare) ||
                     t.HasBuffOfType(BuffType.Taunt) ||
                     (t.Health <= ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q)
                      && !t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))))
                {
                    CastQ(t);
                }
            }

            if (GetValue<KeyBind>("UseQMC").Active)
            {
                t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                CastQ(t);
            }

            //if (GetValue<KeyBind>("UseEMC").Active)
            //{
            //    t = TargetSelector.GetTarget(E.Range - 50, TargetSelector.DamageType.Physical);
            //    E.Cast(t);
            //}

            if (GetValue<KeyBind>("UseRMC").Active && R.IsReady())
            {
                foreach (
                    var e in
                        HeroManager.Enemies.Where(
                            e =>
                                e.IsValidTarget(R.Range) &&
                                ObjectManager.Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(null) + 65) <=
                                1).OrderBy(e => e.Health))
                {
                    //Utils.MPing.Ping(enemy.Position.To2D());
                    R.CastOnUnit(e);
                }
            }

            //for (int i = 1; i < HeroManager.Enemies.Count(e => e.Health < R.GetDamage(e)); i++)
            //{

            //    Common.CommonGeometry.DrawBox(new Vector2(Drawing.Width * 0.45f, Drawing.Height * 0.80f), 125, 18, Color.Transparent, 1, System.Drawing.Color.Black);
            //    Common.CommonGeometry.DrawText(CommonGeometry.Text, HeroManager.Enemies[i].ChampionName + " Killable", Drawing.Width * 0.455f, Drawing.Height * (0.803f + i * 50), SharpDX.Color.Wheat);
            //}

            if (GetValue<KeyBind>("UseEQC").Active && E.IsReady() && Q.IsReady())
            {
                t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget(E.Range)
                    && t.Health
                    < ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q)
                    + ObjectManager.Player.GetSpellDamage(t, SpellSlot.E) + 20 && E.CanCast(t))
                {
                    CastE();
                    //if (E.Cast(t) == Spell.CastStates.SuccessfullyCasted)
                    //    return;
                    //                    E.Cast(t);
                    CastQ(t);
                }
            }

            if ((!ComboActive && !HarassActive) || !Orbwalking.CanMove(100))
            {
                return;
            }

            //var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
            var useW = GetValue<bool>("UseWC");
            var useE = GetValue<bool>("UseEC");
            var useR = GetValue<bool>("UseRC");

            //if (Q.IsReady() && useQ)
            //{
            //    t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            //    if (t != null)
            //    {
            //        CastQ(t);
            //    }
            //}

            if (useE && E.IsReady())
            {
                //var enemies = HeroManager.Enemies.Where(e => e.IsValidTarget(E.Range));
                //var objAiHeroes = enemies as AIHeroClient[] ?? enemies.ToArray();
                //IEnumerable<AIHeroClient> nResult =
                //    (from e in objAiHeroes join d in dangerousEnemies on e.ChampionName equals d select e)
                //        .Distinct();

                //foreach (var n in nResult.Where(n => n.IsFacing(ObjectManager.Player)))
                //{
                //    if (n.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65 - 300) && E.GetPrediction(n).CollisionObjects.Count == 0)
                //    {
                //        E.Cast(n.Position);
                //        if (W.IsReady())
                //            W.Cast(n.Position);
                //    }
                //}
                //if (GetValue<bool>("E.ProtectDistance"))
                //{
                //    foreach (var n in HeroManager.Enemies)
                //    {
                //        if (GetValue<bool>("E." + n.ChampionName + ".ProtectDistance") &&
                //            n.Distance(ObjectManager.Player) < E.Range - 100)
                //        {
                //            E.Cast(n.Position);
                //        }

                //    }
                //}
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            e =>
                                e.IsValidTarget(E.Range) && e.Health >= ObjectManager.Player.TotalAttackDamage*2 &&
                                e.IsFacing(ObjectManager.Player) && e.IsValidTarget(E.Range - 300) &&
                                E.GetPrediction(e).CollisionObjects.Count == 0))
                {
                    E.Cast(enemy.Position);
                    var targetBehind = enemy.Position.To2D().Extend(ObjectManager.Player.Position.To2D(), -140);
                    if (W.IsReady() && ObjectManager.Player.Distance(targetBehind) <= W.Range)
                    {
                        W.Cast(enemy.Position);
                    }
                    if (Q.IsReady())
                    {
                        Q.Cast(enemy.Position);
                    }
                }

            }

            if (useW && W.IsReady())
            {
                var nResult = HeroManager.Enemies.Where(e => e.IsValidTarget(W.Range));
                foreach (var n in nResult)
                {
                    if (ObjectManager.Player.Distance(n) < 450 && n.IsFacing(ObjectManager.Player))
                    {
                        CastW(CommonGeometry.CenterOfVectors(new[] {ObjectManager.Player.Position, n.Position}));
                    }
                }
            }

            if (R.IsReady() && useR)
            {
                foreach (
                    var e in
                        HeroManager.Enemies.Where(
                            e =>
                                e.IsValidTarget(R.Range) && e.Health <= R.GetDamage(e) &&
                                ObjectManager.Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(null) + 350) ==
                                0 &&
                                !Orbwalking.InAutoAttackRange(e) && canCastR))
                {
                    R.CastOnUnit(e);
                }
            }
        }

        public override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var t = target as AIHeroClient;
            if (t == null || (!ComboActive && !HarassActive) || unit.IsMe) return;

            //var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
            //if (useQ) Q.Cast(t, false, true);

            base.Orbwalking_AfterAttack(unit, target);
        }

        public override bool MainMenu(Menu config)
        {
            return base.MainMenu(config);
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQC" + Id, "Q:").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            config.AddItem(new MenuItem("Combo.Q.Use.Urf" + Id, "Q: Urf Mode").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            config.AddItem(new MenuItem("UseQMC" + Id, "Q: Semi-Manual").SetValue(new KeyBind("G".ToCharArray()[0],KeyBindType.Press))).SetFontStyle(FontStyle.Regular, Q.MenuColor()).Permashow();
            config.AddItem(new MenuItem("UseWC" + Id, "W:").SetValue(true)).SetFontStyle(FontStyle.Regular, W.MenuColor());
            config.AddItem(new MenuItem("UseEC" + Id, "E:").SetValue(true)).SetFontStyle(FontStyle.Regular, E.MenuColor());

            //config.AddItem(new MenuItem("UseEMC" + Id, "E: Semi-Manual").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Press))).SetFontStyle(FontStyle.Regular, E.MenuColor()).Permashow();
            config.AddItem(new MenuItem("UseRC" + Id, "R:").SetValue(true)).SetFontStyle(FontStyle.Regular, R.MenuColor());
            config.AddItem(new MenuItem("UseRMC" + Id, "R: Semi-Manual").SetValue(new KeyBind("R".ToCharArray()[0],KeyBindType.Press))).SetFontStyle(FontStyle.Regular, R.MenuColor()).Permashow();

            var nMenu = new Menu("E Protect Distance:", "Combo.E");
            nMenu.AddItem(new MenuItem("E.ProtectDistance", "Enabled:").SetValue(true));
            foreach (var e in HeroManager.Enemies)
            {
                var nMenuItem = new MenuItem("E." + e.ChampionName + ".ProtectDistance", e.ChampionName).SetValue(false);
                nMenu.AddItem(nMenuItem);
            }
            config.AddSubMenu(nMenu);

            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQH" + Id, "Use Q").SetValue(true));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("Champion.Drawings", ObjectManager.Player.ChampionName + " Draw Options"));
            config.AddItem(
                new MenuItem("DrawQ" + Id, Utils.Utils.Tab + "Q:").SetValue(new Circle(false,
                    Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(
                new MenuItem("DrawW" + Id, Utils.Utils.Tab + "W:").SetValue(new Circle(false,
                    Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(
                new MenuItem("DrawE" + Id, Utils.Utils.Tab + "E:").SetValue(new Circle(false,
                    Color.FromArgb(100, 255, 255, 255))));
            config.AddItem(
                new MenuItem("DrawR" + Id, Utils.Utils.Tab + "R:").SetValue(new Circle(false,
                    Color.FromArgb(100, 255, 255, 255))));
            config.AddItem(
                new MenuItem("Draw.UltiMiniMap", Utils.Utils.Tab + "Draw Ulti Minimap").SetValue(new Circle(true,
                    Color.FromArgb(255, 255, 255, 255))));
            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            
            config.AddItem(new MenuItem("Misc.W.Interrupt" + Id, "E Anti Gapcloser").SetValue(true));
            config.AddItem(new MenuItem("Misc.AntiGapCloser", "E Anti Gapcloser").SetValue(true));
            config.AddItem(new MenuItem("UseEQC" + Id, "Use E-Q Combo").SetValue(new KeyBind("T".ToCharArray()[0],KeyBindType.Press)));
            config.AddItem(new MenuItem("Dash" + Id, "Dash to Mouse").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
            config.AddItem(new MenuItem("AutoQI" + Id, "Auto Q (Stun/Snare/Taunt/Slow)").SetValue(new StringList(new[] {"Off", "On: Everytime", "On: Combo Mode"}, 2)));
            config.AddItem(new MenuItem("AutoWI" + Id, "Auto W (Stun/Snare/Taunt)").SetValue(new StringList(new[] {"Off", "On: Everytime", "On: Combo Mode"}, 2)));
            if (blitz != null)
            {
                config.AddItem(new MenuItem("AutoWB" + Id, "Auto W (Blitz)").SetValue(true));
            }

            return true;
        }

        public override bool LaneClearMenu(Menu menuLane)
        {
            string[] strQ = new string[7];
            strQ[0] = "Off";

            for (var i = 1; i < 7; i++)
            {
                strQ[i] = i.ToString();
            }


            menuLane.AddItem(new MenuItem("Lane.Q.Hit" + Id, "Q: Min Hit >=").SetValue(new StringList(strQ, 4))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            menuLane.AddItem(new MenuItem("Lane.Q.Kill" + Id, "Q: Min. Kill >=").SetValue(new StringList(strQ, 2))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            menuLane.AddItem(new MenuItem("Lane.Q.AA" + Id, "Q: Out of AA Range >=").SetValue(new StringList(strQ, 2))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            return true;
        }

        public override bool JungleClearMenu(Menu menuJungle)
        {
            menuJungle.AddItem(
                new MenuItem("Jungle.Q" + Id, "Q:").SetValue(new StringList(new[] {"Off", "On", "Just big Monsters"}, 1)))
                .SetFontStyle(FontStyle.Regular, Q.MenuColor());
            menuJungle.AddItem(
                new MenuItem("Jungle.E" + Id, "E:").SetValue(new StringList(new[] {"Off", "On", "Just big Monsters"}, 1)))
                .SetFontStyle(FontStyle.Regular, Q.MenuColor());

            return true;
        }

        public override void PermaActive()
        {
            if (blitz != null && GetValue<bool>("AutoWB") && W.IsReady())
            {
                if (blitz.Distance(ObjectManager.Player.Position) < W.Range)
                {
                    foreach (
                        var enemy in
                            HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(2000) && enemy.HasBuff("RocketGrab"))
                        )
                    {
                        W.Cast(blitz.Position.Extend(enemy.Position, 50));
                        return;
                    }
                }
            }
        }

        public override void ExecuteLane()
        {
            var qLaneHit = GetValue<StringList>("Lane.Q.Hit").SelectedIndex;
            if (qLaneHit != 0)
            {
                var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All);
                var locQ = Q.GetLineFarmLocation(allMinionsQ);
                if (allMinionsQ.Count == allMinionsQ.Count(m => ObjectManager.Player.Distance(m) < Q.Range) && locQ.MinionsHit >= qLaneHit && locQ.Position.IsValid())
                    Q.Cast(locQ.Position);
            }

            var qLaneKill = GetValue<StringList>("Lane.Q.Kill").SelectedIndex;
            if (qLaneKill != 0)
            {
                var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All).Where(m => m.Health <= Q.GetDamage(m)).ToList();
                var locQ = Q.GetLineFarmLocation(allMinionsQ);
                if (allMinionsQ.Count == allMinionsQ.Count(m => ObjectManager.Player.Distance(m) < Q.Range) && locQ.MinionsHit >= qLaneKill && locQ.Position.IsValid())
                    Q.Cast(locQ.Position);
            }
        }

        public override void ExecuteJungle()
        {
            if (!Orbwalking.CanMove(1))
            {
                return;
            }

            var jungleMobs = Utils.Utils.GetMobs(Q.Range, Utils.Utils.MobTypes.BigBoys);

            if (jungleMobs != null)
            {
                if (Q.CanCast(jungleMobs) && GetValue<StringList>("Jungle.Q").SelectedIndex != 0)
                {
                    Q.Cast(jungleMobs);
                }

                if (E.CanCast(jungleMobs) && GetValue<StringList>("Jungle.E").SelectedIndex != 0)
                {
                    E.Cast(jungleMobs);
                }

            }
        }

        public override void ExecuteCombo()
        {
            if (GetValue<bool>("Combo.Q.Use.Urf"))
            {
                var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (t.IsValidTarget() && !t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
                    CastQ(t);
            }
            base.ExecuteCombo();
        }

        public override void ExecuteFlee()
        {
            if (E.IsReady())
            {
                var pos = Vector3.Zero;
                var enemy =
                    HeroManager.Enemies.FirstOrDefault(
                        e =>
                            e.IsValidTarget(E.Range +
                                            (ObjectManager.Player.MoveSpeed > e.MoveSpeed
                                                ? ObjectManager.Player.MoveSpeed - e.MoveSpeed
                                                : e.MoveSpeed - ObjectManager.Player.MoveSpeed)) && E.CanCast(e));

                pos = enemy?.Position ??
                      ObjectManager.Player.ServerPosition.To2D().Extend(Game.CursorPos.To2D(), -300).To3D();
                //E.Cast(pos);
            }

            PermaActive();
        }
    }
}