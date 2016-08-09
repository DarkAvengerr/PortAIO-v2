using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text;
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
    internal class Zac
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static int[] eRanges = new int[] { 1150, 1300, 1450, 1600, 1750 };
        public static float[] eChannelTimes = new float[] { 0.9f, 1.05f, 1.2f, 1.35f, 1.5f };
        public static Vector3 farmPos, pos;
        public static float zacETime;

        public Zac()
        {
            InitZac();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Zac</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "ZacE")
            {
                if (zacETime == 0f)
                {
                    zacETime = System.Environment.TickCount;
                    LeagueSharp.Common.Utility.DelayAction.Add(4000, () => { zacETime = 0f; });
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && config.Item("Interrupt", true).GetValue<bool>() && sender.Distance(player) < R.Range)
            {
                R.Cast();
            }
        }

        private void InitZac()
        {
            Q = new Spell(SpellSlot.Q, 550);
            Q.SetSkillshot(0.55f, 120, float.MaxValue, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 320);
            E = new Spell(SpellSlot.E);
            E.SetSkillshot(0.75f, 230, 1500, false, SkillshotType.SkillshotCircle);
            E.SetCharged("ZacE", "ZacE", 295, eRanges[0], eChannelTimes[0]);
            R = new Spell(SpellSlot.R, 300);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (E.IsCharging || eActive)
            {
                orbwalker.SetAttack(false);
                orbwalker.SetMovement(false);
                Orbwalking.Move = false;
                Orbwalking.Attack = false;
            }
            else
            {
                orbwalker.SetAttack(true);
                orbwalker.SetMovement(true);
                Orbwalking.Move = true;
                Orbwalking.Attack = true;
            }
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }
        }

        private void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (config.Item("useqH", true).GetValue<bool>() && Q.CanCast(target))
            {
                Q.CastIfHitchanceEquals(target, HitChance.Medium);
            }
            if (config.Item("usewH", true).GetValue<bool>() && W.IsReady())
            {
                if (player.Distance(target) < W.Range)
                {
                    W.Cast();
                }
            }
        }

        private void Clear()
        {
            var target = Jungle.GetNearest(player.Position, GetTargetRange());
            if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady() && !E.IsCharging)
            {
                if (target != null && Q.CanCast(target))
                {
                    Q.Cast(target.Position);
                }
                else
                {
                    MinionManager.FarmLocation bestPositionQ =
                        Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
                    if (bestPositionQ.MinionsHit >= config.Item("qMinHit", true).GetValue<Slider>().Value)
                    {
                        Q.Cast(bestPositionQ.Position);
                    }
                }
            }
            if (config.Item("usewLC", true).GetValue<bool>() && W.IsReady() && !E.IsCharging)
            {
                if (target != null && target.Distance(player) < W.Range)
                {
                    W.Cast();
                }
                else
                {
                    if (Environment.Minion.countMinionsInrange(player.Position, W.Range) >=
                        config.Item("wMinHit", true).GetValue<Slider>().Value)
                    {
                        W.Cast();
                    }
                }
            }
            if (config.Item("collectBlobs", true).GetValue<bool>() && !E.IsCharging)
            {
                var blob =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            o =>
                                !o.IsDead && o.IsValid && o.Name == "BlobDrop" && o.Team == player.Team &&
                                o.Distance(player) < Orbwalking.GetRealAutoAttackRange(player))
                        .OrderBy(o => o.Distance(player))
                        .FirstOrDefault();
                if (blob != null && Orbwalking.CanMove(300) && !Orbwalking.CanAttack() && !player.Spellbook.IsAutoAttacking)
                {
                    orbwalker.SetMovement(false);
                    Orbwalking.Move = false;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, blob.Position);
                }
            }
            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady())
            {
                if (target != null && target.IsValidTarget())
                {
                    CastE(target);
                }
                else
                {
                    MinionManager.FarmLocation bestPositionE =
                        E.GetCircularFarmLocation(
                            MinionManager.GetMinions(eRanges[E.Level - 1], MinionTypes.All, MinionTeam.NotAlly));
                    var castPos = Vector3.Zero;
                    if (bestPositionE.MinionsHit < config.Item("eMinHit", true).GetValue<Slider>().Value &&
                        farmPos.IsValid())
                    {
                        castPos = farmPos;
                    }
                    if (bestPositionE.MinionsHit >= config.Item("eMinHit", true).GetValue<Slider>().Value)
                    {
                        castPos = bestPositionE.Position.To3D();
                    }
                    if (castPos.IsValid())
                    {
                        farmPos = bestPositionE.Position.To3D();
                        LeagueSharp.Common.Utility.DelayAction.Add(5000, () => { farmPos = Vector3.Zero; });
                        CastE(castPos);
                    }
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = null;
            if (E.IsCharging)
            {
                target = TargetSelector.GetTarget(
                    GetTargetRange(), TargetSelector.DamageType.Magical, true,
                    HeroManager.Enemies.Where(
                        h => h.IsInvulnerable && CombatHelper.GetAngle(player, target.Position) > 50));
            }
            else
            {
                target = TargetSelector.GetTarget(
                    GetTargetRange(), TargetSelector.DamageType.Magical, true,
                    HeroManager.Enemies.Where(h => h.IsInvulnerable));
            }
            if (target == null)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config);
            }
            if (config.Item("usew", true).GetValue<bool>() && W.CanCast(target) && !E.IsCharging)
            {
                W.Cast();
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) && !Q.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }

            if (rActive)
            {
                orbwalker.SetAttack(false);
                Orbwalking.Attack = false;
                return;
            }

            if (config.Item("usee", true).GetValue<bool>() && E.IsReady() && player.CanMove)
            {
                CastE(target);
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target) && target.IsValidTarget() &&
                !E.IsCharging)
            {
                Q.CastIfHitchanceEquals(target, HitChance.Medium);
            }

            if (R.IsReady() && config.Item("user", true).GetValue<bool>() &&
                config.Item("Rmin", true).GetValue<Slider>().Value <= player.CountEnemiesInRange(R.Range) &&
                !target.HasBuffOfType(BuffType.Knockback) && !target.HasBuffOfType(BuffType.Knockup) &&
                !target.HasBuffOfType(BuffType.Stun))
            {
                R.Cast();
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (target.Distance(player) > eRanges[E.Level - 1])
            {
                return;
            }
            var eFlyPred = E.GetPrediction(target);
            var enemyPred = Prediction.GetPrediction(
                target, eChannelTimes[E.Level - 1] + target.Distance(player) / E.Speed / 1000);
            if (E.IsCharging)
            {
                if (!eFlyPred.CastPosition.IsValid() || eFlyPred.CastPosition.IsWall())
                {
                    return;
                }
                if (eFlyPred.CastPosition.Distance(player.Position) < E.Range)
                {
                    E.CastIfHitchanceEquals(target, HitChance.High);
                }
                else if (eFlyPred.UnitPosition.Distance(player.Position) < E.Range && target.Distance(player) < 500f)
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                else if ((eFlyPred.CastPosition.Distance(player.Position) < E.Range &&
                          eRanges[E.Level - 1] - eFlyPred.CastPosition.Distance(player.Position) < 200) ||
                         (CombatHelper.GetAngle(player, eFlyPred.CastPosition) > 35))
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
                else if (eFlyPred.CastPosition.Distance(player.Position) < E.Range && zacETime != 0 &&
                         System.Environment.TickCount - zacETime > 2500)
                {
                    E.CastIfHitchanceEquals(target, HitChance.Medium);
                }
            }
            else if (enemyPred.UnitPosition.Distance(player.Position) < eRanges[E.Level - 1] &&
                     config.Item("Emin", true).GetValue<Slider>().Value < target.Distance(player.Position))
            {
                E.SetCharged("ZacE", "ZacE", 300, eRanges[E.Level - 1], eChannelTimes[E.Level - 1]);
                E.StartCharging(eFlyPred.UnitPosition);
            }
        }

        private void CastE(Vector3 target)
        {
            if (target.Distance(player.Position) > eRanges[E.Level - 1])
            {
                return;
            }
            if (E.IsCharging)
            {
                if (target.Distance(player.Position) < E.Range)
                {
                    E.Cast(target);
                }
            }
            else if (target.Distance(player.Position) < eRanges[E.Level - 1])
            {
                E.SetCharged("ZacE", "ZacE", 295, eRanges[E.Level - 1], eChannelTimes[E.Level - 1]);
                E.StartCharging(target);
            }
        }

        private float GetTargetRange()
        {
            if (E.IsReady())
            {
                return eRanges[E.Level - 1];
            }
            else
            {
                return 600;
            }
        }

        private float GetERange()
        {
            if (E.Level > 0)
            {
                return eRanges[E.Level - 1];
            }
            else
            {
                return eRanges[0];
            }
        }

        private static bool rActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "ZacR"); }
        }

        private static bool eActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "ZacE"); }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), GetERange());
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
            if (pos.IsValid())
            {
                //Render.Circle.DrawCircle(pos, 100, Color.Aqua, 7);
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R) * 2;
            }
            //damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void InitMenu()
        {
            config = new Menu("Zac ", "Zac", true);
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
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(false);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("Emin", "   E min range", true)).SetValue(new Slider(300, 0, 1550));
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("Rmin", "   R min", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("usewH", "Use W", true)).SetValue(true);
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("qMinHit", "   Q min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("wMinHit", "   W min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("eMinHit", "   E min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("collectBlobs", "Collect nearby blobs", true)).SetValue(true);
            config.AddSubMenu(menuLC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("Interrupt", "Cast R to interrupt spells", true)).SetValue(true);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}