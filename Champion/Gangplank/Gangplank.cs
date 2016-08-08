using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Gangplank
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justQ, justE, chain, blockedE, movingToBarrel;
        public Vector3 ePos;
        public const int BarrelExplosionRange = 325;
        public const int BarrelConnectionRange = 670;
        public List<Barrel> savedBarrels = new List<Barrel>();
        public List<CastedBarrel> castedBarrels = new List<CastedBarrel>();
        public double[] Rwave = new double[] { 50, 70, 90 };
        public double[] EDamage = new double[] { 60, 90, 120, 150, 180 };
        public Obj_AI_Minion NeedToBeDestroyed;

        public Gangplank()
        {
            InitGangPlank();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Gangplank</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && config.Item("useeH", true).GetValue<bool>()))
            {
                return;
            }

            if (args.Slot == SpellSlot.E && config.Item("barrelCorrection", true).GetValue<bool>() &&
                Game.CursorPos.LSDistance(args.StartPosition) < 50)
            {
                var barrel =
                    GetBarrels()
                        .Where(
                            b =>
                                b.LSDistance(Game.CursorPos) > BarrelConnectionRange &&
                                b.LSDistance(Game.CursorPos) < BarrelConnectionRange * 2 &&
                                b.Position.LSExtend(Game.CursorPos, BarrelConnectionRange).LSDistance(args.StartPosition) <
                                BarrelExplosionRange)
                        .OrderBy(b => b.LSDistance(Game.CursorPos))
                        .FirstOrDefault();
                if (barrel != null && !blockedE)
                {
                    args.Process = false;
                    blockedE = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        5, () =>
                        {
                            E.Cast(barrel.Position.LSExtend(Game.CursorPos, BarrelConnectionRange));
                            blockedE = false;
                        });
                }
            }
        }


        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            for (int i = 0; i < savedBarrels.Count; i++)
            {
                if (savedBarrels[i].barrel != null &&
                    (savedBarrels[i].barrel.NetworkId == sender.NetworkId || savedBarrels[i].barrel.IsDead))
                {
                    savedBarrels.RemoveAt(i);
                    return;
                }
            }
        }

        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                savedBarrels.Add(new Barrel(sender as Obj_AI_Minion, System.Environment.TickCount));
            }
        }

        private IEnumerable<Obj_AI_Minion> GetBarrels()
        {
            return savedBarrels.Where(b => b.barrel != null).Select(b => b.barrel);
        }

        private bool KillableBarrel(Obj_AI_Base targetB,
            bool melee = false,
            float delay = 0,
            AIHeroClient sender = null,
            float missileTravelTime = -1)
        {
            if (targetB.Health < 2)
            {
                return true;
            }
            if (sender == null)
            {
                sender = player;
            }
            if (missileTravelTime == -1)
            {
                missileTravelTime = GetQTime(targetB);
            }
            var barrel = savedBarrels.FirstOrDefault(b => b.barrel.NetworkId == targetB.NetworkId);
            if (barrel != null)
            {
                var t = System.Environment.TickCount - barrel.time - 2 * getEActivationDelay() * 1000;
                t = Math.Abs(Math.Min(t, 0)) + delay;
                if (t - ((melee ? (sender.AttackCastDelay) : missileTravelTime) * 1000) <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private float GetQTime(Obj_AI_Base targetB)
        {
            return player.LSDistance(targetB) / (player.Crit < 0.05f ? 2600f : 3000f) + Q.Delay;
        }

        private void InitGangPlank()
        {
            Q = new Spell(SpellSlot.Q, 590f); //2600f
            Q.SetTargetted(0.25f, 2600f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 950);
            E.SetSkillshot(0.8f, 50, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R);
            R.SetSkillshot(1f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            orbwalker.SetOrbwalkingPoint(Game.CursorPos);
            orbwalker.SetAttack(true);
            orbwalker.SetMovement(true);
            movingToBarrel = false;
            var barrels =
                GetBarrels()
                    .Where(
                        o =>
                            o.IsValid && !o.IsDead && o.LSDistance(player) < 3000 && o.BaseSkinName == "GangplankBarrel" &&
                            o.GetBuff("gangplankebarrellife").Caster.IsMe)
                    .ToList();
            var QMana = Q.ManaCost < player.Mana;
            var shouldAAbarrel = (!Q.LSIsReady() ||
                                  config.Item("comboPrior", true).GetValue<StringList>().SelectedIndex == 1 ||
                                  (Q.LSIsReady() && !QMana) || !config.Item("useq", true).GetValue<bool>());

            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(barrels, shouldAAbarrel, QMana);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass(barrels, shouldAAbarrel, QMana);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    if (config.Item("useqLHH", true).GetValue<bool>() && !justE)
                    {
                        Lasthit();
                    }
                    break;
                default:
                    break;
            }
            if (config.Item("AutoR", true).GetValue<bool>() && R.LSIsReady())
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        e =>
                            ((e.LSUnderTurret(true) &&
                              e.MaxHealth / 100 * config.Item("Rhealt", true).GetValue<Slider>().Value * 0.75f >
                              e.Health - Program.IncDamages.GetEnemyData(e.NetworkId).DamageTaken) ||
                             (!e.LSUnderTurret(true) &&
                              e.MaxHealth / 100 * config.Item("Rhealt", true).GetValue<Slider>().Value >
                              e.Health - Program.IncDamages.GetEnemyData(e.NetworkId).DamageTaken)) &&
                            e.HealthPercent > config.Item("RhealtMin", true).GetValue<Slider>().Value &&
                            e.LSIsValidTarget() && e.LSDistance(player) > 1500))
                {
                    var pred = Program.IncDamages.GetEnemyData(enemy.NetworkId);
                    if (pred != null && pred.DamageTaken < enemy.Health)
                    {
                        var ally =
                            HeroManager.Allies.OrderBy(a => a.Health).FirstOrDefault(a => enemy.LSDistance(a) < 1000);
                        if (ally != null)
                        {
                            var pos = Prediction.GetPrediction(enemy, 0.75f);
                            if (pos.CastPosition.LSDistance(enemy.Position) < 450 && pos.Hitchance >= HitChance.VeryHigh)
                            {
                                if (enemy.IsMoving)
                                {
                                    R.Cast(enemy.Position.LSExtend(pos.CastPosition, 450));
                                }
                                else
                                {
                                    R.Cast(enemy.ServerPosition);
                                }
                            }
                        }
                    }
                }
            }
            if (config.Item("EQtoCursor", true).GetValue<KeyBind>().Active && E.LSIsReady() && Q.LSIsReady())
            {
                orbwalker.SetMovement(false);
                var barrel =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.LSDistance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o, false, -260))
                        .OrderBy(o => o.LSDistance(Game.CursorPos))
                        .FirstOrDefault();
                if (barrel != null)
                {
                    var cp = Game.CursorPos;
                    var cursorPos = barrel.LSDistance(cp) > BarrelConnectionRange
                        ? barrel.Position.LSExtend(cp, BarrelConnectionRange)
                        : cp;
                    var points =
                        CombatHelper.PointsAroundTheTarget(player.Position, E.Range - 200, 15, 6)
                            .Where(p => p.LSDistance(player.Position) < E.Range);
                    var cursorPos2 = cursorPos.LSDistance(cp) > BarrelConnectionRange
                        ? cursorPos.LSExtend(cp, BarrelConnectionRange)
                        : cp;
                    var middle = GetMiddleBarrel(barrel, points, cursorPos);
                    var threeBarrel = cursorPos.LSDistance(cp) > BarrelExplosionRange && E.Instance.Ammo >= 2 &&
                                      Game.CursorPos.LSDistance(player.Position) < E.Range && middle.LSIsValid();
                    var firsDelay = threeBarrel ? 500 : 265;
                    if (cursorPos.LSIsValid() && cursorPos.LSDistance(player.Position) < E.Range)
                    {
                        E.Cast(threeBarrel ? middle : cursorPos);
                        LeagueSharp.Common.Utility.DelayAction.Add(firsDelay, () => Q.CastOnUnit(barrel));
                        if (threeBarrel)
                        {
                            if (player.IsMoving)
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, player.Position);
                            }
                            LeagueSharp.Common.Utility.DelayAction.Add(801, () => E.Cast(middle.LSExtend(cp, BarrelConnectionRange)));
                        }
                        else
                        {
                            if (Orbwalking.CanMove(100))
                            {
                                orbwalker.SetMovement(true);
                                Orbwalking.MoveTo(Game.CursorPos);
                            }
                        }
                    }
                }
                else
                {
                    if (Orbwalking.CanMove(100))
                    {
                        orbwalker.SetMovement(true);
                        Orbwalking.MoveTo(Game.CursorPos, 60);
                    }
                }
            }
            else if (config.Item("EQtoCursor", true).GetValue<KeyBind>().Active)
            {
                if (Orbwalking.CanMove(100))
                {
                    orbwalker.SetMovement(true);
                    Orbwalking.MoveTo(Game.CursorPos, 60);
                }
            }
            if (config.Item("QbarrelCursor", true).GetValue<KeyBind>().Active && Q.LSIsReady())
            {
                var meleeRangeBarrel =
                    GetBarrels()
                        .OrderBy(o => o.LSDistance(Game.CursorPos))
                        .FirstOrDefault(
                            o =>
                                o.Health > 1 && o.LSDistance(player) < Orbwalking.GetRealAutoAttackRange(o) &&
                                !KillableBarrel(o, true, 265));
                if (meleeRangeBarrel != null && Orbwalking.CanAttack())
                {
                    orbwalker.ForceTarget(meleeRangeBarrel);
                    return;
                }
                var barrel =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.LSDistance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o))
                        .OrderBy(o => o.LSDistance(Game.CursorPos))
                        .FirstOrDefault();
                if (barrel != null)
                {
                    Q.CastOnUnit(barrel);
                }
            }
            if (NeedToBeDestroyed != null && NeedToBeDestroyed.LSIsValidTarget() && Orbwalking.CanAttack() &&
                NeedToBeDestroyed.IsInAttackRange())
            {
                Console.WriteLine("NeedToBeDestroyed");
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, NeedToBeDestroyed);
            }
            if (config.Item("AutoQBarrel", true).GetValue<bool>() && !movingToBarrel)
            {
                var target = TargetSelector.GetTarget(
                    1650, TargetSelector.DamageType.Physical, true, HeroManager.Enemies.Where(h => h.IsInvulnerable));
                if (target != null && BlowUpBarrel(barrels, shouldAAbarrel, false, target))
                {
                    if (!chain)
                    {
                        chain = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(450, () => chain = false);
                    }
                }
                if (Q.LSIsReady())
                {
                    foreach (var barrel in
                        barrels.Where(b => KillableBarrel(b) && b.LSCountEnemiesInRange(BarrelExplosionRange) > 0))
                    {
                        Q.Cast(barrel);
                        return;
                    }
                }
            }
            for (int i = 0; i < castedBarrels.Count; i++)
            {
                if (castedBarrels[i].shouldDie())
                {
                    castedBarrels.RemoveAt(i);
                    break;
                }
            }
        }

        private Vector3 GetMiddleBarrel(Obj_AI_Minion barrel, IEnumerable<Vector3> points, Vector3 cursorPos)
        {
            var middle =
                points.Where(
                    p =>
                        !p.LSIsWall() && p.LSDistance(barrel.Position) < BarrelConnectionRange &&
                        p.LSDistance(barrel.Position) > BarrelExplosionRange &&
                        p.LSDistance(cursorPos) < BarrelConnectionRange && p.LSDistance(cursorPos) > BarrelExplosionRange &&
                        p.LSDistance(barrel.Position) + p.LSDistance(cursorPos) > BarrelExplosionRange * 2 - 100)
                    .OrderByDescending(p => p.LSCountEnemiesInRange(BarrelExplosionRange))
                    .ThenByDescending(p => p.LSDistance(barrel.Position))
                    .FirstOrDefault();
            return middle;
        }

        private void Lasthit()
        {
            if (Q.LSIsReady())
            {
                var mini =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(m => m.Health < Q.GetDamage(m) && m.BaseSkinName != "GangplankBarrel")
                        .OrderByDescending(m => m.MaxHealth)
                        .ThenByDescending(m => m.LSDistance(player))
                        .FirstOrDefault();

                if (mini != null && !justE)
                {
                    Q.CastOnUnit(mini);
                }
            }
        }


        private void Harass(List<Obj_AI_Minion> barrels, bool shouldAAbarrel, bool qMana)
        {
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            AIHeroClient target = TargetSelector.GetTarget(
                Q.Range + BarrelExplosionRange, TargetSelector.DamageType.Physical);

            if (config.Item("useqLHH", true).GetValue<bool>())
            {
                var mini =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(m => m.Health < Q.GetDamage(m) && m.BaseSkinName != "GangplankBarrel")
                        .OrderByDescending(m => m.MaxHealth)
                        .ThenByDescending(m => m.LSDistance(player))
                        .FirstOrDefault();

                if (mini != null)
                {
                    Q.CastOnUnit(mini);
                    return;
                }
            }

            if (target == null || Environment.Minion.KillableMinion(player.AttackRange + 50))
            {
                return;
            }
            var dontQ = false;
            //Blow up barrels
            if (config.Item("useqH", true).GetValue<bool>() &&
                BlowUpBarrel(barrels, shouldAAbarrel, config.Item("movetoBarrel", true).GetValue<bool>(), target))
            {
                if (!chain)
                {
                    chain = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(450, () => chain = false);
                }
                return;
            }

            //Cast E to chain
            if (E.LSIsReady() && E.Instance.Ammo > 0 && !justQ && !chain && config.Item("useeH", true).GetValue<bool>() &&
                config.Item("eStacksH", true).GetValue<Slider>().Value < E.Instance.Ammo)
            {
                if (barrels.Any())
                {
                    var bestEMelee = GetEPos(barrels, target, true);
                    var bestEQ = GetEPos(barrels, target, false);
                    if (bestEMelee.LSIsValid() && shouldAAbarrel)
                    {
                        dontQ = true;
                        E.Cast(bestEMelee);
                    }
                    else if (bestEQ.LSIsValid() && config.Item("useqH", true).GetValue<bool>() && Q.LSIsReady())
                    {
                        dontQ = true;
                        E.Cast(bestEQ);
                    }
                }
            }
            if (config.Item("useqH", true).GetValue<bool>() && Q.LSIsReady() && !dontQ)
            {
                Q.CastOnUnit(target);
            }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (config.Item("useqLC", true).GetValue<bool>())
            {
                var barrel =
                    GetBarrels()
                        .FirstOrDefault(
                            o =>
                                o.IsValid && !o.IsDead && o.LSDistance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                Environment.Minion.countMinionsInrange(o.Position, BarrelExplosionRange) >= 1);
                if (barrel != null)
                {
                    var minis = MinionManager.GetMinions(
                        barrel.Position, BarrelExplosionRange, MinionTypes.All, MinionTeam.NotAlly);
                    var Killable =
                        minis.Where(e => Q.GetDamage(e) + ItemHandler.GetSheenDmg(e) >= e.Health && e.Health > 3);
                    if (Q.LSIsReady() && KillableBarrel(barrel) &&
                        Killable.Any(t => HealthPrediction.LaneClearHealthPrediction(t, 1000) <= 0))
                    {
                        Q.CastOnUnit(barrel);
                    }


                    if (config.Item("ePrep", true).GetValue<bool>())
                    {
                        if (Q.LSIsReady() && minis.Count == Killable.Count() && KillableBarrel(barrel))
                        {
                            Q.CastOnUnit(barrel);
                        }
                        else
                        {
                            foreach (var m in
                                minis.Where(
                                    e => Q.GetDamage(e) + ItemHandler.GetSheenDmg(e) <= e.Health && e.Health > 3)
                                    .OrderBy(t => t.LSDistance(player))
                                    .ThenByDescending(t => t.Health))
                            {
                                orbwalker.ForceTarget(m);
                                return;
                            }
                        }
                    }
                    else if (Q.LSIsReady() && KillableBarrel(barrel) &&
                             minis.Count >= config.Item("eMinHit", true).GetValue<Slider>().Value)
                    {
                        Q.CastOnUnit(barrel);
                    }

                    return;
                }
            }
            if (config.Item("useqLC", true).GetValue<bool>() && !justE)
            {
                Lasthit();
            }
            if (config.Item("useeLC", true).GetValue<bool>() && E.LSIsReady() &&
                config.Item("eStacksLC", true).GetValue<Slider>().Value < E.Instance.Ammo)
            {
                MinionManager.FarmLocation bestPositionE =
                    E.GetCircularFarmLocation(
                        MinionManager.GetMinions(
                            ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly),
                        BarrelExplosionRange);

                if (bestPositionE.MinionsHit >= config.Item("eMinHit", true).GetValue<Slider>().Value &&
                    bestPositionE.Position.LSDistance(ePos) > 400)
                {
                    E.Cast(bestPositionE.Position);
                }
            }
        }

        private void Combo(List<Obj_AI_Minion> barrels, bool shouldAAbarrel, bool Qmana)
        {
            var target = TargetSelector.GetTarget(
                1650, TargetSelector.DamageType.Physical, true, HeroManager.Enemies.Where(h => h.IsInvulnerable));
            if (target == null)
            {
                return;
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.LSGetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) && !Q.LSIsReady() && !justQ)
            {
                player.Spellbook.CastSpell(player.LSGetSpellSlot("SummonerDot"), target);
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (!data.AnyCC &&
                (config.Item("usew", true).GetValue<Slider>().Value / 100f) * player.MaxHealth >
                player.Health - data.DamageTaken && player.LSCountEnemiesInRange(500) > 0)
            {
                W.Cast();
            }
            if (R.LSIsReady() && config.Item("user", true).GetValue<bool>())
            {
                var Rtarget =
                    HeroManager.Enemies.FirstOrDefault(e => e.HealthPercent < 50 && e.LSCountAlliesInRange(660) > 0);
                if (Rtarget != null)
                {
                    R.CastIfWillHit(Rtarget, config.Item("Rmin", true).GetValue<Slider>().Value);
                }
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target), config.Item("AutoW", true).GetValue<bool>());
            }
            var dontQ = false;

            //Blow up barrels
            if (BlowUpBarrel(barrels, shouldAAbarrel, config.Item("movetoBarrel", true).GetValue<bool>(), target))
            {
                if (!chain)
                {
                    chain = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(450, () => chain = false);
                }
                return;
            }

            //Cast E to chain
            if (E.LSIsReady() && E.Instance.Ammo > 0 && !justQ && !chain &&
                config.Item("detoneateTarget", true).GetValue<bool>())
            {
                if (barrels.Any())
                {
                    var bestEMelee = GetEPos(barrels, target, true);
                    var bestEQ = GetEPos(barrels, target, false);
                    if (bestEMelee.LSIsValid() && shouldAAbarrel)
                    {
                        dontQ = true;
                        E.Cast(bestEMelee);
                    }
                    else if (bestEQ.LSIsValid() && config.Item("useq", true).GetValue<bool>() && Q.LSIsReady())
                    {
                        dontQ = true;
                        E.Cast(bestEQ);
                    }
                }
            }


            if (config.Item("useeAlways", true).GetValue<bool>() && E.LSIsReady() && player.LSDistance(target) < E.Range &&
                !justE && target.Health > Q.GetDamage(target) + player.LSGetAutoAttackDamage(target) &&
                Orbwalking.CanMove(100) && config.Item("eStacksC", true).GetValue<Slider>().Value < E.Instance.Ammo)
            {
                CastE(target, barrels);
            }
            var Qbarrelsb =
                GetBarrels()
                    .FirstOrDefault(
                        o =>
                            o.LSDistance(player) < Q.Range &&
                            o.LSDistance(target) < BarrelConnectionRange + BarrelExplosionRange);
            if (Qbarrelsb != null && E.Instance.Ammo > 0 && Q.LSIsReady() && target.Health > Q.GetDamage(target))
            {
                dontQ = true;
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target) && Orbwalking.CanMove(100) && !justE &&
                (!config.Item("useqBlock", true).GetValue<bool>() || !dontQ))
            {
                CastQonHero(target, barrels);
            }
        }

        private bool BlowUpBarrel(List<Obj_AI_Minion> barrels,
            bool shouldAAbarrel,
            bool movetoBarrel,
            AIHeroClient target)
        {
            if (barrels.Any())
            {
                var moveDist = movetoBarrel ? config.Item("movetoBarrelDist", true).GetValue<Slider>().Value : 0;
                var bestBarrelMelee = GetBestBarrel(barrels, true, target, moveDist);
                var bestBarrelQ = GetBestBarrel(barrels, false, target);
                if (bestBarrelMelee != null && shouldAAbarrel &&
                    HeroManager.Enemies.FirstOrDefault(
                        e => e.LSDistance(bestBarrelMelee) < player.LSDistance(bestBarrelMelee)) == null)
                {
                    if (Orbwalking.GetRealAutoAttackRange(bestBarrelMelee) < player.LSDistance(bestBarrelMelee))
                    {
                        movingToBarrel = true;
                        orbwalker.SetOrbwalkingPoint(bestBarrelMelee.Position);
                        orbwalker.SetAttack(false);
                    }
                    else
                    {
                        if (KillableBarrel(bestBarrelMelee, true) && Orbwalking.CanAttack())
                        {
                            orbwalker.ForceTarget(bestBarrelMelee);
                        }
                    }
                    return true;
                }
                if (bestBarrelQ != null && config.Item("useq", true).GetValue<bool>())
                {
                    Q.CastOnUnit(bestBarrelQ);
                    return true;
                }
            }
            return false;
        }

        private bool EnemiesInBarrelRange(Vector3 barrel, float delay)
        {
            if (
                HeroManager.Enemies.Count(
                    enemy => enemy.LSIsValidTarget() && enemy.LSDistance(barrel) < BarrelExplosionRange) > 0)
            {
                return true;
            }
            return false;
        }

        private Obj_AI_Minion GetBestBarrel(List<Obj_AI_Minion> barrels,
            bool isMelee,
            AIHeroClient target,
            float moveDist = 0f)
        {
            var meleeBarrels =
                barrels.Where(
                    b =>
                        player.LSDistance(b) <
                        (isMelee
                            ? Orbwalking.GetRealAutoAttackRange(b) +
                              (CombatHelper.IsFacing(player, b.Position) ? moveDist : 0f)
                            : Q.Range) && KillableBarrel(b, isMelee, 265));
            var secondaryBarrels = barrels.Select(b => b.Position).Concat(castedBarrels.Select(c => c.pos));
            var meleeDelay = isMelee ? 0.25f : 0;
            if (moveDist > 0f)
            {
                meleeDelay -= (moveDist / player.MoveSpeed);
            }
            foreach (var melee in meleeBarrels)
            {
                var secondBarrels =
                    secondaryBarrels.Where(
                        b =>
                            !meleeBarrels.Any(n => b.LSDistance(n.Position) < 10) &&
                            melee.LSDistance(b) < BarrelConnectionRange);
                foreach (var second in secondBarrels)
                {
                    var thirdBarrels =
                        secondaryBarrels.Where(
                            b =>
                                !secondBarrels.Any(n => b.LSDistance(n) < 10) &&
                                !meleeBarrels.Any(n => b.LSDistance(n.Position) < 10) &&
                                second.LSDistance(b) < BarrelConnectionRange);
                    foreach (var third in thirdBarrels)
                    {
                        if (EnemiesInBarrelRange(third, 1.25f - meleeDelay))
                        {
                            return melee;
                        }
                    }
                    if (EnemiesInBarrelRange(second, 1f - meleeDelay))
                    {
                        return melee;
                    }
                }
                if (EnemiesInBarrelRange(melee.Position, 0.75f - meleeDelay))
                {
                    return melee;
                }
            }
            return null;
        }

        private Vector3 GetE(Vector3 barrel, AIHeroClient target, float delay, List<Vector3> barrels)
        {
            var enemies =
                HeroManager.Enemies.Where(
                    e =>
                        e.LSIsValidTarget(1650) && e.LSDistance(barrel) > BarrelExplosionRange &&
                        !barrels.Any(b => b.LSDistance(e.Position) < BarrelExplosionRange));
            var targetPred = Prediction.GetPrediction(target, delay);
            var pos = Vector3.Zero;
            pos =
                GetBarrelPoints(barrel)
                    .Where(
                        p =>
                            !p.LSIsWall() && p.LSDistance(barrel) < BarrelConnectionRange &&
                            p.LSDistance(player.Position) < E.Range &&
                            barrels.Count(b => b.LSDistance(p) < BarrelExplosionRange) == 0 &&
                            targetPred.CastPosition.LSDistance(p) < BarrelExplosionRange &&
                            target.LSDistance(p) < BarrelExplosionRange)
                    .OrderByDescending(p => enemies.Count(e => e.LSDistance(p) < BarrelExplosionRange))
                    .ThenBy(p => p.LSDistance(targetPred.CastPosition))
                    .FirstOrDefault();
            return pos;
        }

        private Vector3 GetMiddleE(Vector3 barrel, AIHeroClient target, float delay, List<Vector3> barrels)
        {
            if (E.Instance.Ammo < 2)
            {
                return Vector3.Zero;
            }
            var enemies =
                HeroManager.Enemies.Where(
                    e =>
                        e.LSIsValidTarget(1650) && e.LSDistance(barrel) > BarrelExplosionRange &&
                        !barrels.Any(b => b.LSDistance(e.Position) < BarrelExplosionRange));
            var targetPred = Prediction.GetPrediction(target, delay);
            var pos = Vector3.Zero;
            pos =
                GetBarrelPoints(barrel)
                    .Where(
                        p =>
                            p.LSDistance(barrel) < BarrelConnectionRange && p.LSDistance(player.Position) < E.Range &&
                            barrels.Count(b => b.LSDistance(p) < BarrelExplosionRange) == 0 &&
                            targetPred.CastPosition.LSDistance(p) < (BarrelExplosionRange - 25) * 2)
                    .OrderByDescending(p => enemies.Count(e => e.LSDistance(p) < BarrelExplosionRange))
                    .ThenBy(p => p.LSDistance(targetPred.CastPosition))
                    .FirstOrDefault();
            return pos;
        }

        private Vector3 GetEPos(List<Obj_AI_Minion> barrels, AIHeroClient target, bool isMelee)
        {
            var barrelPositions = barrels.Select(b => b.Position).Concat(castedBarrels.Select(c => c.pos)).ToList();
            var moveDist = config.Item("movetoBarrel", true).GetValue<bool>()
                ? config.Item("movetoBarrelDist", true).GetValue<Slider>().Value
                : 0;
            var barrelsInCloseRange =
                barrels.Where(
                    b =>
                        player.LSDistance(b) < target.LSDistance(b) &&
                        player.LSDistance(b) <
                        (isMelee
                            ? Orbwalking.GetRealAutoAttackRange(b) +
                              (CombatHelper.IsFacing(player, b.Position) ? moveDist : 0f)
                            : Q.Range) && KillableBarrel(b, isMelee, -265))
                    .Select(b => b.Position)
                    .Concat(castedBarrels.Select(c => c.pos));
            var meleeDelay = isMelee ? 0.25f : 0;

            foreach (var melee in barrelsInCloseRange)
            {
                var secondPos = GetE(melee, target, 1.265f - meleeDelay, barrelPositions);
                var middle = GetMiddleE(melee, target, 1.465f - meleeDelay, barrelPositions);
                if (secondPos.LSIsValid())
                {
                    return secondPos;
                }
                var secondBarrels = barrelPositions.Where(b => melee.LSDistance(b) < BarrelConnectionRange).ToList();
                foreach (var secondBarrel in secondBarrels)
                {
                    var thirdE = GetE(secondBarrel, target, 1.265f - meleeDelay, barrelPositions);
                    if (thirdE.LSIsValid())
                    {
                        return thirdE;
                    }
                }
                if (middle.LSIsValid())
                {
                    return middle;
                }
            }
            return Vector3.Zero;
        }


        private void CastQonHero(AIHeroClient target, List<Obj_AI_Minion> barrels)
        {
            if (barrels.FirstOrDefault(b => target.LSDistance(b.Position) < BarrelExplosionRange) != null &&
                target.Health > Q.GetDamage(target))
            {
                return;
            }
            Q.CastOnUnit(target);
        }

        private void CastE(AIHeroClient target, List<Obj_AI_Minion> barrels)
        {
            if (barrels.Count(b => b.LSCountEnemiesInRange(BarrelConnectionRange) > 0) < 1)
            {
                if (config.Item("useeAlways", true).GetValue<bool>())
                {
                    CastEtarget(target);
                }
                return;
            }
            var enemies =
                HeroManager.Enemies.Where(e => e.LSIsValidTarget() && e.LSDistance(player) < E.Range)
                    .Select(e => Prediction.GetPrediction(e, 0.35f));
            List<Vector3> points = new List<Vector3>();
            foreach (var barrel in
                barrels.Where(b => b.LSDistance(player) < Q.Range && KillableBarrel(b)))
            {
                if (barrel != null)
                {
                    var newP = GetBarrelPoints(barrel.Position).Where(p => !p.LSIsWall());
                    if (newP.Any())
                    {
                        points.AddRange(newP.Where(p => p.LSDistance(player.Position) < E.Range));
                    }
                }
            }
            var bestPoint =
                points.Where(b => enemies.Count(e => e.UnitPosition.LSDistance(b) < BarrelExplosionRange) > 0)
                    .OrderByDescending(b => enemies.Count(e => e.UnitPosition.LSDistance(b) < BarrelExplosionRange))
                    .FirstOrDefault();
            if (bestPoint.LSIsValid() &&
                !savedBarrels.Any(b => b.barrel.Position.LSDistance(bestPoint) < BarrelConnectionRange))
            {
                E.Cast(bestPoint);
            }
        }

        private void CastEtarget(AIHeroClient target)
        {
            var ePred = Prediction.GetPrediction(target, 1);
            var pos = target.Position.LSExtend(ePred.CastPosition, BarrelExplosionRange);
            if (pos.LSDistance(ePos) > 400 && !justE)
            {
                E.Cast(pos);
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            var drawecr = config.Item("draweecr", true).GetValue<Circle>();
            if (drawecr.Active)
            {
                foreach (var barrel in GetBarrels().Where(b => b.LSDistance(player) < E.Range + BarrelConnectionRange))
                {
                    Render.Circle.DrawCircle(barrel.Position, BarrelConnectionRange, drawecr.Color, 7);
                }
            }
            if (config.Item("drawMTB", true).GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                    player.Position,
                    Math.Max(
                        config.Item("movetoBarrelDist", true).GetValue<Slider>().Value + 200 - player.BoundingRadius -
                        60, 250), Color.DarkSlateGray, 5);
            }
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
            if (config.Item("drawW", true).GetValue<bool>())
            {
                if (W.LSIsReady() && player.HealthPercent < 100)
                {
                    float Heal = new int[] { 50, 75, 100, 125, 150 }[W.Level - 1] +
                                 (player.MaxHealth - player.Health) * 0.15f + player.FlatMagicDamageMod * 0.9f;
                    float mod = Math.Max(100f, player.Health + Heal) / player.MaxHealth;
                    float xPos = (float) ((double) player.HPBarPosition.X + 36 + 103.0 * mod);
                    Drawing.DrawLine(
                        xPos, player.HPBarPosition.Y + 8, xPos, (float) ((double) player.HPBarPosition.Y + 17), 2f,
                        Color.Coral);
                }
            }
            var tokens = player.GetBuff("gangplankbilgewatertoken");
            if (player.LSInFountain() && config.Item("drawQpass", true).GetValue<bool>() && tokens != null &&
                tokens.Count > 500)
            {
                var second = DateTime.Now.Second.ToString();
                var time = int.Parse(second[second.Length - 1].ToString());
                var color = Color.DeepSkyBlue;
                if (time >= 3 && time < 6)
                {
                    color = Color.GreenYellow;
                }
                if (time >= 6 && time < 8)
                {
                    color = Color.Yellow;
                }
                if (time >= 8)
                {
                    color = Color.Orange;
                }
                Drawing.DrawText(
                    Drawing.WorldToScreen(Game.CursorPos).X - 150, Drawing.WorldToScreen(Game.CursorPos).Y - 50, color,
                    "Spend your Silver Serpents, landlubber!");
            }
            if (config.Item("drawKillableSL", true).GetValue<StringList>().SelectedIndex != 0 && R.LSIsReady())
            {
                var text = new List<string>();
                foreach (var enemy in HeroManager.Enemies.Where(e => e.LSIsValidTarget()))
                {
                    if (getRDamage(enemy) > enemy.Health)
                    {
                        text.Add(enemy.ChampionName + "(" + Math.Ceiling(enemy.Health / Rwave[R.Level - 1]) + " wave)");
                    }
                }
                if (text.Count > 0)
                {
                    var result = string.Join(", ", text);
                    switch (config.Item("drawKillableSL", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 2:
                            drawText(2, result);
                            break;
                        case 1:
                            drawText(1, result);
                            break;
                        default:
                            return;
                    }
                }
            }

            try
            {
                if (Q.LSIsReady() && config.Item("drawEQ", true).GetValue<bool>())
                {
                    var points =
                        CombatHelper.PointsAroundTheTarget(player.Position, E.Range - 200, 15, 6)
                            .Where(p => p.LSDistance(player.Position) < E.Range);


                    var barrel =
                        GetBarrels()
                            .Where(
                                o =>
                                    o.IsValid && !o.IsDead && o.LSDistance(player) < Q.Range &&
                                    o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                    KillableBarrel(o))
                            .OrderBy(o => o.LSDistance(Game.CursorPos))
                            .FirstOrDefault();
                    if (barrel != null)
                    {
                        var cp = Game.CursorPos;
                        var cursorPos = barrel.LSDistance(cp) > BarrelConnectionRange
                            ? barrel.Position.LSExtend(cp, BarrelConnectionRange)
                            : cp;
                        var cursorPos2 = cursorPos.LSDistance(cp) > BarrelConnectionRange
                            ? cursorPos.LSExtend(cp, BarrelConnectionRange)
                            : cp;
                        var middle = GetMiddleBarrel(barrel, points, cursorPos);
                        var threeBarrel = cursorPos.LSDistance(cp) > BarrelExplosionRange && E.Instance.Ammo >= 2 &&
                                          cursorPos2.LSDistance(player.Position) < E.Range && middle.LSIsValid();
                        if (threeBarrel)
                        {
                            Render.Circle.DrawCircle(
                                middle.LSExtend(cp, BarrelConnectionRange), BarrelExplosionRange, Color.DarkOrange, 6);
                            Render.Circle.DrawCircle(middle, BarrelExplosionRange, Color.DarkOrange, 6);
                            Drawing.DrawLine(
                                Drawing.WorldToScreen(barrel.Position),
                                Drawing.WorldToScreen(middle.LSExtend(barrel.Position, BarrelExplosionRange)), 2,
                                Color.DarkOrange);
                        }
                        else if (E.Instance.Ammo >= 1)
                        {
                            Drawing.DrawLine(
                                Drawing.WorldToScreen(barrel.Position),
                                Drawing.WorldToScreen(cursorPos.LSExtend(barrel.Position, BarrelExplosionRange)), 2,
                                Color.DarkOrange);
                            Render.Circle.DrawCircle(cursorPos, BarrelExplosionRange, Color.DarkOrange, 6);
                        }
                    }
                }
            }
            catch (Exception) {}
            if (config.Item("drawWcd", true).GetValue<bool>())
            {
                foreach (var barrelData in savedBarrels)
                {
                    float time =
                        Math.Min(
                            System.Environment.TickCount - barrelData.time -
                            barrelData.barrel.Health * getEActivationDelay() * 1000f, 0) / 1000f;
                    if (time < 0)
                    {
                        Drawing.DrawText(
                            barrelData.barrel.HPBarPosition.X - -20, barrelData.barrel.HPBarPosition.Y - 20,
                            Color.DarkOrange, string.Format("{0:0.00}", time).Replace("-", ""));
                    }
                }
            }

            if (config.Item("drawEmini", true).GetValue<bool>())
            {
                try
                {
                    var barrels =
                        GetBarrels()
                            .Where(
                                o =>
                                    o.IsValid && !o.IsDead && o.LSDistance(player) < E.Range &&
                                    o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe);
                    foreach (var b in barrels)
                    {
                        var minis = MinionManager.GetMinions(
                            b.Position, BarrelExplosionRange, MinionTypes.All, MinionTeam.NotAlly);
                        foreach (var m in
                            minis.Where(e => Q.GetDamage(e) + ItemHandler.GetSheenDmg(e) >= e.Health && e.Health > 3))
                        {
                            Render.Circle.DrawCircle(m.Position, 57, Color.Yellow, 7);
                        }
                    }
                }
                catch (Exception) {}
            }
        }

        public void drawText(int mode, string result)
        {
            const string baseText = "Killable with R: ";
            if (mode == 1)
            {
                Drawing.DrawText(
                    Drawing.Width / 2 - (baseText + result).Length * 5, Drawing.Height * 0.75f, Color.Red,
                    baseText + result);
            }
            else
            {
                Drawing.DrawText(
                    player.HPBarPosition.X - (baseText + result).Length * 5 + 110, player.HPBarPosition.Y + 250,
                    Color.Red, baseText + result);
            }
        }

        private float getRDamage(AIHeroClient enemy)
        {
            return
                (float)
                    Damage.CalcDamage(
                        player, enemy, Damage.DamageType.Magical,
                        (Rwave[R.Level - 1] + 0.1 * player.FlatMagicDamageMod) * waveLength());
        }

        public int waveLength()
        {
            if (player.HasBuff("GangplankRUpgrade1"))
            {
                return 18;
            }
            else
            {
                return 12;
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.LSIsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
            }
            //damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.LSGetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "GangplankQWrapper")
                {
                    if (!justQ)
                    {
                        justQ = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(200, () => justQ = false);
                    }
                }
                if (args.SData.Name == "GangplankE")
                {
                    ePos = args.End;
                    if (!justE)
                    {
                        justE = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => justE = false);
                        castedBarrels.Add(new CastedBarrel(ePos, System.Environment.TickCount));
                    }
                }
            }
            if (sender.IsEnemy && args.Target != null && sender is AIHeroClient && sender.LSDistance(player) < 2000)
            {
                var targetBarrel =
                    savedBarrels.FirstOrDefault(b => b.barrel != null && b.barrel.NetworkId == args.Target.NetworkId);
                if (targetBarrel != null)
                {
                    if (KillableBarrel(targetBarrel.barrel, true, 0, (AIHeroClient) sender, args.SData.MissileSpeed))
                    {
                        savedBarrels.Remove(targetBarrel);
                        return;
                    }


                    var delay = (int) (sender.LSDistance(targetBarrel.barrel.Position) / args.SData.MissileSpeed * 1000f);
                    var t = System.Environment.TickCount - targetBarrel.time - 1 * getEActivationDelay() * 1000;
                    t = Math.Abs(Math.Min(t, 0)) - GetQTime(targetBarrel.barrel);
                    if (t - delay <= 1 * getEActivationDelay() &&
                        !KillableBarrel(targetBarrel.barrel, true, 0, (AIHeroClient) sender))
                    {
                        targetBarrel.time -= 5000;
                    }
                }
            }
        }

        private IEnumerable<Vector3> GetBarrelPoints(Vector3 point)
        {
            return
                CombatHelper.PointsAroundTheTarget(point, BarrelConnectionRange, 15f)
                    .Where(p => !p.LSIsWall() && p.LSDistance(point) > BarrelExplosionRange);
        }

        private float getEActivationDelay()
        {
            if (player.Level >= 13)
            {
                return 0.475f;
            }
            if (player.Level >= 7)
            {
                return 0.975f;
            }
            return 1.975f;
        }

        private void InitMenu()
        {
            config = new Menu("Gangplank ", "Gangplank", true);
            // Target Selector
            Menu menuTS = new Menu("Selector", "tselect");
            TargetSelector.AddToMenu(menuTS);
            config.AddSubMenu(menuTS);
            // Orbwalker
            Menu menuOrb = new Menu("Orbwalker", "orbwalker");
            orbwalker = new Orbwalking.Orbwalker(menuOrb);
            config.AddSubMenu(menuOrb);
            // Draw settings
            Menu menuD = new Menu("Drawings ", "dsettings");
            menuD.AddItem(new MenuItem("drawqq", "Draw Q range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawW", "Draw W", true)).SetValue(true);
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("draweecr", "Draw Connection ranges", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 167, 141, 56)));
            menuD.AddItem(new MenuItem("drawWcd", "Draw E countdown", true)).SetValue(true);
            menuD.AddItem(new MenuItem("drawEmini", "Draw killable minions around E", true)).SetValue(true);
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            menuD.AddItem(new MenuItem("drawEQ", "Draw EQ to cursor", true)).SetValue(false);
            menuD.AddItem(new MenuItem("drawMTB", "Draw Move to barrel range", true)).SetValue(false);
            menuD.AddItem(new MenuItem("drawKillableSL", "Show killable targets with R", true))
                .SetValue(new StringList(new[] { "OFF", "Above HUD", "Under GP" }, 1));
            menuD.AddItem(new MenuItem("drawQpass", "Draw notification about Silver serpents", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqBlock", "   Block Q to save for EQ", true)).SetValue(false);
            menuC.AddItem(new MenuItem("detoneateTarget", "   Blow up target with E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W under health", true)).SetValue(new Slider(20, 0, 100));
            menuC.AddItem(new MenuItem("AutoW", "Use W with QSS options", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useeAlways", "Use E always", true))
                .SetTooltip(
                    "NOT RECOMMENDED, If there is no barrel around the target, this will put one to a predicted position")
                .SetValue(false);
            menuC.AddItem(new MenuItem("eStacksC", "   Keep stacks", true))
                .SetTooltip("You can set up how many barrels you want to keep to \"Use E to extend range\"")
                .SetValue(new Slider(0, 0, 5));
            menuC.AddItem(new MenuItem("movetoBarrel", "Move to barrel to AA", true)).SetValue(false);
            menuC.AddItem(new MenuItem("movetoBarrelDist", "   Max distance", true)).SetValue(new Slider(300, 0, 450));
            menuC.AddItem(new MenuItem("EQtoCursor", "EQ to cursor", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("QbarrelCursor", "Q barrel at cursor", true))
                .SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("Rmin", "   R min", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q harass", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useqLHH", "Use Q lasthit", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(true);
            menuH.AddItem(new MenuItem("eStacksH", "   Keep stacks", true)).SetValue(new Slider(0, 0, 5));
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("eMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("eStacksLC", "   Keep stacks", true)).SetValue(new Slider(0, 0, 5));
            menuLC.AddItem(new MenuItem("ePrep", "   Prepare minions", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("AutoR", "Cast R to get assists", true)).SetValue(false);
            menuM.AddItem(new MenuItem("Rhealt", "   Enemy health %", true))
                .SetTooltip("Max enemy healthpercent to cast R")
                .SetValue(new Slider(35, 0, 100));
            menuM.AddItem(new MenuItem("RhealtMin", "   Enemy min health %", true))
                .SetTooltip("min enemy healthpercent to prevent KS or casting too late")
                .SetValue(new Slider(10, 0, 100));
            menuM.AddItem(new MenuItem("AutoQBarrel", "AutoQ barrel near enemies", true)).SetValue(false);
            menuM.AddItem(new MenuItem("comboPrior", "   Combo priority", true))
                .SetValue(new StringList(new[] { "E-Q", "E-AA", }, 0));
            menuM.AddItem(new MenuItem("barrelCorrection", "Barrel placement correction", true)).SetValue(true);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }

    internal class Barrel
    {
        public Obj_AI_Minion barrel;
        public float time;
        public Vector3 pos;

        public Barrel(Obj_AI_Minion objAiBase, int tickCount)
        {
            barrel = objAiBase;
            time = tickCount;
        }
    }

    internal class CastedBarrel
    {
        public float time;
        public Vector3 pos;

        public CastedBarrel(Vector3 position, int tickCount)
        {
            pos = position;
            time = tickCount;
        }

        public bool shouldDie()
        {
            return System.Environment.TickCount - time > 260;
        }
    }
}