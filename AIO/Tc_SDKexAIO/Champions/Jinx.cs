using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Champions
{

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Color = System.Drawing.Color;

    using Common;
    using static Common.Manager;
    using Config;
    using static Core.TCommon;

    using Menu = LeagueSharp.SDK.UI.Menu;
    using Geometry = Common.Geometry;

    internal static class Jinx
    {

        private static Spell Q, W, E, R;
        private static Menu Menu => PlaySharp.ChampionMenu;
        private static AIHeroClient Player => PlaySharp.Player;
        private static bool BigGun => Player.HasBuff("JinxQ");

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public static float LatFocusTime = Game.Time;

        internal static void Init()
        {

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1490f).SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 920f).SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 3000f).SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);


            var QMenu = Menu.Add(new Menu("Q", "Q.Set"));
            {
                QMenu.GetSeparator("Q: Always On");
                QMenu.Add(new MenuBool("ComboQ", "Combo Q", true));
                QMenu.Add(new MenuBool("HarassQ", "Harass Q", true));
                QMenu.Add(new MenuBool("LaneClearQ", "LaneClear Q", true));
            }

            var WMenu = Menu.Add(new Menu("W", "W.Set"));
            {
                WMenu.Add(new MenuBool("ComboW", "Combo W", true));
                WMenu.Add(new MenuBool("HarassW", "Harass W", true));
                WMenu.Add(new MenuSlider("ManaW", "W Min Mana >= ", 40));
                WMenu.Add(new MenuBool("KSW", "W Ks", true));
                var WList = WMenu.Add(new Menu("WList", "W List:"));
                {
                    if (GameObjects.EnemyHeroes.Any())
                    {
                        GameObjects.EnemyHeroes.ForEach(i => WList.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                    }
                }
            }

            var EMenu = Menu.Add(new Menu("E", "E.Set"));
            {
                EMenu.GetSeparator("E: Mobe");
                EMenu.Add(new MenuBool("ComboE", "Combo E", true));
                EMenu.GetSeparator("E: Gapcloser | Melee Modes");
                EMenu.Add(new MenuBool("Gapcloser", "Gapcloser E", true));
                EMenu.GetSeparator("Auto E Set");
                EMenu.Add(new MenuBool("SlowE", "Slow E", true));
                EMenu.Add(new MenuBool("StunE", "Stun E", true));
                EMenu.Add(new MenuBool("TelE", "Tel E", true));
                EMenu.Add(new MenuBool("ImmeE", "Imm E", true));
                EMenu.Add(new MenuBool("ProtectE", "Protect E", true));
            }

            var RMenu = Menu.Add(new Menu("R", "R.Set"));
            {
                RMenu.GetSeparator("R: Mobe");
                RMenu.Add(new MenuBool("Auto", "Auto R ", true));
            }
            ModeBaseUlti.Init(Menu);

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("RDKs", "Draw R KS", true));
            }

            PlaySharp.Write(GameObjects.Player.ChampionName + "OK! :)");

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Events.OnGapCloser += OnGapCloser;
            Variables.Orbwalker.OnAction += OnAction;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs args)
        {

               if (Player.IsDead)
                    return;

                QLogic(args);

                WLogic(args);

                ELogic(args);

                AutoRLogic(args);

                TSMode(args);
        }

        private static void TSMode(EventArgs args)
        {

            var orbT = Variables.Orbwalker.GetTarget();

            if (orbT != null && orbT.Type == GameObjectType.AIHeroClient)
            {
                var bestTarget = (AIHeroClient)orbT;
                var hitToBestTarget = bestTarget.Health / Player.GetAutoAttackDamage(bestTarget);

                foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget()
                    && InAutoAttackRange(enemy)))
                {
                    if (enemy.Health / Player.GetAutoAttackDamage(enemy) < hitToBestTarget)
                    {
                        bestTarget = enemy;
                    }
                }
            }
        }

        private static void QLogic(EventArgs args)
        {
            var QHarass = Menu["Q"]["HarassQ"];
            if ((Harass || LaneClear) && !BigGun && QHarass && !Player.Spellbook.IsAutoAttacking && Variables.Orbwalker.CanAttack
                && Variables.Orbwalker.GetTarget() == null && Player.Mana > R.Instance.SData.Mana + W.Instance.SData.Mana + E.Instance.SData.Mana + 10)
            {
                foreach (var minion in GameObjects.EnemyMinions.Where(minion => minion.IsValidTarget(Q2Range() + 30)
                && !InAutoAttackRange(minion) && Q1Range(minion) < GetRealDistance(minion) && Q2Range() < GetRealDistance(minion)))
                {
                    var hp = Health.GetPrediction(minion, 350);
                    if (hp < Player.GetAutoAttackDamage(minion) * 1.2 && hp > 3)
                    {
                        Variables.Orbwalker.ForceTarget = minion;
                        Q.Cast();
                        return;
                    }
                }
            }
            var t = GetTarget(Q2Range() + 60, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (!BigGun && (!InAutoAttackRange(t) || t.CountEnemyHeroesInRange(250) > 2) && Variables.Orbwalker.GetTarget() == null)
                {
                    var disstance = GetRealDistance(t);

                    if (Combo && (Player.Mana > R.Instance.SData.Mana + W.Instance.SData.Mana + 10 || Player.GetAutoAttackDamage(t) * 3 > t.Health))
                    {
                        Q.Cast();
                    }
                    else if (Harass && QHarass && !Player.Spellbook.IsAutoAttacking && Variables.Orbwalker.CanAttack && !Player.IsUnderEnemyTurret()
                        && Player.Mana > R.Instance.SData.Mana + W.Instance.SData.Mana + E.Instance.SData.Mana + 20 && disstance < Q2Range()
                        + t.BoundingRadius + Player.BoundingRadius)
                    {
                        Q.Cast();
                    }
                }
            }
            else if (!BigGun && Combo && Player.Mana > R.Instance.SData.Mana + W.Instance.SData.Mana + 20 && Player.CountEnemyHeroesInRange(2000) > 0)
            {
                Q.Cast();
            }
            else if (BigGun && Combo && Player.Mana < R.Instance.SData.Mana + W.Instance.SData.Mana + 20)
            {
                Q.Cast();
            }
            else if (BigGun && Combo && Player.CountEnemyHeroesInRange(2000) == 0)
            {
                Q.Cast();
            }
            else if (BigGun && (LaneClear || Harass || Hit))
            {
                Q.Cast();
            }
        }

        private static void WLogic(EventArgs args)
        {
            if (Harass)
            {
                if (Menu["W"]["HarassW"].GetValue<MenuBool>() && W.IsReady())
                {
                    foreach (var e in GetEnemies(W.Range))
                    {
                        if (Menu["W"]["WList"][e.ChampionName.ToLower()].GetValue<MenuBool>() && !Player.IsUnderEnemyTurret()
                            && Player.ManaPercent >= Menu["W"]["ManaW"].GetValue<MenuSlider>().Value)
                        {
                            W.Cast(e);
                        }
                    }
                }
            }
            if (Combo)
            {
                if (Menu["W"]["ComboW"].GetValue<MenuBool>() && W.IsReady())
                {
                    var WTarget = GetTarget(W);
                    {
                        if (WTarget != null && WTarget.IsValidTarget(W.Range) && WTarget.IsHPBarRendered)
                        {
                            W.Cast(WTarget);
                        }
                    }
                }
            }
            if (Menu["W"]["KSW"].GetValue<MenuBool>())
            {
                foreach (var eneny in GetEnemies(W.Range))
                {
                    if (W.GetDamage(eneny) > eneny.Health)
                    {
                        if (eneny.IsValidTarget(W.Range))
                            W.Cast(eneny);
                    }
                }
            }
        }
        
        private static void ELogic(EventArgs args)
        {
            if (!E.IsReady())
                return;

            if (Player.Mana < (E.Instance.SData.Mana + R.Instance.SData.Mana + W.Instance.SData.Mana))
                return;

            if (Combo && Menu["E"]["ComboE"].GetValue<MenuBool>())
            {
                var target = GetTarget(E.Range, DamageType.Physical);

                if (target != null && target.IsHPBarRendered)
                {
                    if (target.IsValidTarget(E.Range) &&
                        E.GetPrediction(target).CastPosition.Distance(target.Position) > 200 &&
                        E.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
                    {
                        if (target.HasBuffOfType(BuffType.Slow) || CountEnemiesInRangeDeley(E.GetPrediction(target).CastPosition, 250, E.Delay) > 1)
                        {
                            E.Cast(target);
                        }
                        else
                        {
                            if (E.GetPrediction(target).CastPosition.Distance(target.Position) > 200)
                            {
                                if (Player.Position.Distance(target.ServerPosition) > Player.Position.Distance(target.Position))
                                {
                                    if (target.Position.Distance(Player.ServerPosition) < target.Position.Distance(Player.Position))
                                        E.Cast(target);
                                }
                                else
                                {
                                    if (target.Position.Distance(Player.ServerPosition) > target.Position.Distance(Player.Position))
                                        E.Cast(target);
                                }
                            }
                        }
                    }
                }

            }
            foreach (var e in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && x.IsHPBarRendered))
            {
                if (CheckTarget(e))
                {
                    if (Menu["E"]["StunE"].GetValue<MenuBool>())
                    {
                        if (e.HasBuffOfType(BuffType.Stun))
                        {
                            if (e.IsValidTarget(E.Range))
                            {
                                if (E.GetPrediction(e).Hitchance >= HitChance.VeryHigh)
                                {
                                    E.Cast(e);
                                }
                            }
                        }
                    }
                    if (Menu["E"]["SlowE"].GetValue<MenuBool>())
                    {
                        if (e.HasBuffOfType(BuffType.Slow))
                        {
                            if (e.IsValidTarget(E.Range))
                            {
                                if (E.GetPrediction(e).Hitchance >= HitChance.VeryHigh)
                                {
                                    E.Cast(e);
                                }
                            }
                        }
                    }
                    if (Menu["E"]["ImmeE"].GetValue<MenuBool>())
                    {
                        if (!CanMove(e))
                        {
                            if (E.GetPrediction(e).Hitchance >= HitChance.VeryHigh)
                            {
                                E.Cast(e);
                            }
                        }
                        else
                        {
                            E.CastIfHitchanceEquals(e, HitChance.Immobile);
                        }
                    }
                }
            }

            if (Menu["E"]["TelE"].GetValue<MenuBool>())
            {
                foreach (var Obj in ObjectManager.Get<Obj_AI_Base>().Where(Obj => Obj.IsEnemy && Obj.Distance(Player.ServerPosition) < E.Range && (Obj.HasBuff("teleport_target") || Obj.HasBuff("Pantheon_GrandSkyfall_Jump"))))
                {
                    E.Cast(Obj.Position);
                }
            }
        }
    
        private static void AutoRLogic(EventArgs args)
        {

            if (!R.IsReady())
                return;

            if (Menu["R"]["Auto"])
            {
                bool cast = false;

                foreach (var target in GameObjects.EnemyHeroes.Where(target => target.IsValidTarget() && CanKill(target)))
                {
                    float predictedHealth = target.Health + target.HPRegenRate * 2;

                    var Rdmg = R.GetDamage(target);

                    if (Rdmg > predictedHealth)
                    {
                        cast = true;

                        PredictionOutput output = R.GetPrediction(target);

                        Vector2 direction = output.CastPosition.ToVector2() - Player.Position.ToVector2();

                        direction.Normalize();

                        List<AIHeroClient> enemies = GameObjects.EnemyHeroes.Where(x => x.IsEnemy && x.IsValidTarget()).ToList();

                        foreach (var enemy in enemies)
                        {
                            if (enemy.BaseSkinName == target.BaseSkinName || !cast)
                                continue;

                            PredictionOutput prediction = R.GetPrediction(enemy);

                            Vector3 predictedPosition = prediction.CastPosition;
                            Vector3 v = output.CastPosition - Player.ServerPosition;
                            Vector3 w = predictedPosition - Player.ServerPosition;

                            double c1 = Vector3.Dot(w, v);
                            double c2 = Vector3.Dot(v, v);
                            double b = c1 / c2;

                            Vector3 pb = Player.ServerPosition + ((float)b * v);

                            float length = Vector3.Distance(predictedPosition, pb);

                            if (length < (R.Width + 150 + enemy.BoundingRadius / 2)
                                && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                                cast = false;
                        }

                        if (cast && (Player.ServerPosition.Distance(target.ServerPosition)
                            + Player.BoundingRadius + target.BoundingRadius)
                            > (670f + Player.BoundingRadius + 25 * Player.Spellbook.GetSpell(SpellSlot.Q).Level)
                            + 300 + target.BoundingRadius && target.CountAllyHeroesInRange(600) == 0
                            && Player.CountEnemyHeroesInRange(400) == 0)
                        {
                            List<Vector2> waypoints = target.GetWaypoints();

                            if ((Player.Distance(waypoints.Last().ToVector3()) - Player.Distance(target.Position)) > 400)
                            R.Cast(target);
                        }

                        else if (cast && target.CountEnemyHeroesInRange(200) > 2
                            && (Player.ServerPosition.Distance(target.ServerPosition) + Player.BoundingRadius + target.BoundingRadius)
                            > (670f + Player.BoundingRadius + 25 * Player.Spellbook.GetSpell(SpellSlot.Q).Level) + 200 + target.BoundingRadius)
                        {
                            R.Cast(target);
                        }
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (W != null && W.IsReady() && Menu["Draw"]["W"] != null && Menu["Draw"]["W"].GetValue<MenuBool>().Value)
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, Color.Purple, 2);
                }

                if (E != null && E.IsReady() && Menu["Draw"]["E"] != null && Menu["Draw"]["E"].GetValue<MenuBool>().Value)
                {
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.Cyan, 2);
                }

                if (R != null && R.IsReady())
                {
                    if (Menu["Draw"]["R"] != null && Menu["Draw"]["R"].GetValue<MenuBool>().Value)
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range, Color.Red, 2);

                    }
                }

                if (Menu["Draw"]["RDKs"].GetValue<MenuBool>() && R.IsReady() && R.Level >= 1)
                {
                    var spos = Drawing.WorldToScreen(Player.Position);
                    var target = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.Health <= R.GetDamage(x) * 3
                    && !x.IsZombie && !x.IsDead);
                    int addpos = 0;
                    foreach (var killable in target)
                    {
                        Drawing.DrawText(spos.X - 50, spos.Y + 35 + addpos, Color.Red, killable.ChampionName + "Is Killable !!!");
                        addpos = addpos + 15;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in On Draw" + ex);
            }
        }
        private static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            try
            {
                var ComboQ = Menu["Q"]["ComboQ"].GetValue<MenuBool>().Value;
                var HarassQ = Menu["Q"]["HarassQ"].GetValue<MenuBool>().Value;
                var LaneClearQ = Menu["Q"]["LaneClearQ"].GetValue<MenuBool>().Value;

                if (args.Type == OrbwalkingType.BeforeAttack)
                {
                    if (!(args.Target is AIHeroClient))
                        return;

                    if (!Q.IsReady()) return;

                    var t = (AIHeroClient)args.Target;

                    if (BigGun && t.IsValidTarget())
                    {
                        var RealDistance = GetRealDistance(t) - 50;
                        if (Combo && ComboQ)
                        {
                            if (RealDistance < (Q1Range(t)))
                            {
                                if (Player.Mana < R.Instance.SData.Mana + 20 || Player.GetAutoAttackDamage(t) * 3
                                    < t.Health)
                                {
                                    Q.Cast();
                                }
                            }
                        }
                        else if ((LaneClear || Harass) && HarassQ)
                        {
                            if ((RealDistance > Q2Range() || RealDistance < Q1Range(t) || Player.Mana <
                                R.Instance.SData.Mana + E.Instance.SData.Mana + W.Instance.SData.Mana + W.Instance.SData.Mana))
                            {
                                Q.Cast();
                            }
                        }
                    }
                    if (LaneClear && !BigGun && LaneClearQ)
                    {
                        if (Player.Mana > R.Instance.SData.Mana + E.Instance.SData.Mana + W.Instance.SData.Mana + 30)
                        {
                            var MinionQ = GetMinions(Player.ServerPosition, (Q2Range()));
                            foreach (var minion in MinionQ.Where(minion =>
                            args.Target.NetworkId != minion.NetworkId && minion.Distance(args.Target.Position) < 200 && (5 - Q.Level)
                            * Player.GetAutoAttackDamage(minion) < args.Target.Health && (5 - Q.Level) * Player.GetAutoAttackDamage(minion) < minion.Health))
                            {
                                Q.Cast();
                            }
                        }
                    }
                    if (!(Combo))
                    {
                        return;
                    }
                    if (args.Target is AIHeroClient)
                    {
                        var newTarget = (AIHeroClient)args.Target;
                        var forceFocusEnemy = newTarget;
                        var aaRange = Player.AttackRange * 525f + Player.BoundingRadius + 350;
                        foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(aaRange)))
                        {
                            if (enemy.Health / Player.GetAutoAttackDamage(enemy) + 1 < forceFocusEnemy.GetAutoAttackDamage(forceFocusEnemy))
                            {
                                forceFocusEnemy = enemy;
                            }
                        }
                        if (forceFocusEnemy.NetworkId != newTarget.NetworkId && Game.Time - LatFocusTime < 2)
                        {
                            args.Process = false;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Before Attack Events " + ex);
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (!E.IsReady()) return;

            if (Menu["E"]["Gapcloser"].GetValue<MenuBool>())
            {
                if (args.Sender.IsValidTarget(E.Range))
                {
                    if (E.GetPrediction(args.Sender).Hitchance >= HitChance.VeryHigh)
                    {
                        E.Cast(args.Sender);
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            try
            {
                if (sender.IsMinion) return;
                if (!E.IsReady()) return;
                if (sender.IsEnemy)
                {
                    if (Menu["E"]["ProtectE"].GetValue<MenuBool>())
                    {
                        if (sender.IsValidTarget(E.Range))
                        {
                            if (ShouldUseE(args.SData.Name))
                            {
                                E.Cast(sender.ServerPosition);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Process Spell Cast" + ex);
            }
        }

        private static int CountEnemiesInRangeDeley(Vector3 position, float range, float delay)
        {
            int count = 0;

            foreach (var t in GameObjects.EnemyHeroes.Where(t => t.IsValidTarget()))
            {
                Vector3 prepos = Movement.GetPrediction(t, delay).CastPosition;

                if (position.Distance(prepos) < range)
                    count++;
            }

            return count;
        }

        private static float Q2Range()
        {
            return 670f + Player.BoundingRadius + 25 * Player.Spellbook.GetSpell(SpellSlot.Q).Level;
        }

        private static float Q1Range(GameObject target)
        {
            return 650f + Player.BoundingRadius + target.BoundingRadius;
        }

        private static float GetRealDistance(Obj_AI_Base target)
        {
            return Player.ServerPosition.Distance(Movement.GetPrediction(target, 0.05f).CastPosition) + Player.BoundingRadius + target.BoundingRadius;
        }

        private static bool ShouldUseE(string SpellName)
        {
            switch (SpellName)
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

        private static float GetDamage(AIHeroClient t, Spell spell)
        {

            var Dmg = spell.GetDamage(t);

            var hp = Health.GetPrediction(t, 500);

            Dmg += hp;
            Dmg -= t.HPRegenRate;
            Dmg -= t.PercentLifeStealMod * 0.005f * t.FlatPhysicalDamageMod;
            if (Dmg > t.Health)
            {
                if (Player.HasBuff("SummonerexHaust"))
                    Dmg = Dmg * 0.6f;

                if (t.HasBuff("FerociousHowl"))
                    Dmg = Dmg * 0.7f;

                if (t.ChampionName == "Blitzcrank" && !t.HasBuff("BlitzcrankManaBarrierCD") && !t.HasBuff("ManaBarrier"))
                {
                    Dmg -= t.Mana / 2f;
                }
            }
            Dmg += (float)GetIncomingDamage(t);
            return Dmg;
        }
    }
}