using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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


using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class MonkeyKing
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public bool justQ, justE, justW;
        public static float UltiCheck;
        public static Vector3 point;

        public MonkeyKing()
        {
            InitWukong();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Wukong</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }


        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (config.Item("useq", true).GetValue<bool>() && Q.IsReady() && target is AIHeroClient)
                    {
                        Q.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, target);
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (config.Item("useqH", true).GetValue<bool>() && Q.IsReady() && target is AIHeroClient &&
                        config.Item("minmanaH", true).GetValue<Slider>().Value < player.ManaPercent)
                    {
                        Q.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, target);
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady() &&
                        config.Item("minmana", true).GetValue<Slider>().Value < player.ManaPercent &&
                        (target.Health <
                         player.GetAutoAttackDamage((Obj_AI_Base) target) + Q.GetDamage((Obj_AI_Base) target) ||
                         target.Health > player.GetAutoAttackDamage((Obj_AI_Base) target, true) * 4))
                    {
                        Q.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, target);
                    }
                    break;
                default:
                    break;
            }
        }


        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && config.Item("Interrupt", true).GetValue<bool>() && sender.Distance(player) < R.Range)
            {
                R.Cast(player);
            }
        }

        private void InitWukong()
        {
            Q = new Spell(SpellSlot.Q, 375f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 625f);
            E.SetTargetted(0.5f, 2000f);
            R = new Spell(SpellSlot.R, 325f);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if ((wActive && (!Q.IsReady() || justW)) || rActive)
            {
                orbwalker.SetAttack(false);
            }
            else
            {
                orbwalker.SetAttack(true);
            }
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            Rmovement();
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

        private void Rmovement()
        {
            if (rActive && Game.CursorPos.CountEnemiesInRange(300) > 1)
            {
                AIHeroClient target = DrawHelper.GetBetterTarget(
                    E.Range, TargetSelector.DamageType.Physical, true, HeroManager.Enemies.Where(h => h.IsInvulnerable));
                if (target != null && target.CountEnemiesInRange(R.Range) > 1)
                {
                    if (System.Environment.TickCount - UltiCheck > 250 || UltiCheck == 0f)
                    {
                        var enemies =
                            HeroManager.Enemies.Where(e => e.IsValidTarget() && e.Distance(player) < 600)
                                .Select(e => Prediction.GetPrediction(e, 0.35f));
                        switch (config.Item("rType", true).GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                point =
                                    CombatHelper.PointsAroundTheTarget(player.Position, R.Range)
                                        .Where(p => p.CountEnemiesInRange(R.Range + 100) > 0)
                                        .OrderByDescending(
                                            p => enemies.Count(e => e.UnitPosition.Distance(p) <= R.Range))
                                        .ThenBy(p => p.Distance(Game.CursorPos))
                                        .FirstOrDefault();
                                break;
                            case 1:
                                point =
                                    CombatHelper.PointsAroundTheTarget(target.Position, R.Range)
                                        .Where(p => p.CountEnemiesInRange(R.Range + 100) > 0)
                                        .OrderByDescending(
                                            p => enemies.Count(e => e.UnitPosition.Distance(p) <= R.Range))
                                        .ThenBy(p => p.Distance(Game.CursorPos))
                                        .FirstOrDefault();
                                break;
                            case 2:
                                point = Game.CursorPos;
                                break;
                        }
                    }

                    if (point.IsValid() && player.Distance(point) > 10 &&
                        point.CountEnemiesInRange(R.Range) > player.CountEnemiesInRange(R.Range))
                    {
                        orbwalker.SetMovement(false);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, point);
                        UltiCheck = System.Environment.TickCount;
                    }
                }
            }
            else
            {
                orbwalker.SetMovement(true);
            }
        }

        private void Harass()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Magical);
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || target == null)
            {
                return;
            }
            if (config.Item("useeH", true).GetValue<bool>() && E.CanCast(target))
            {
                E.CastOnUnit(target);
            }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady())
            {
                var bestPos =
                    MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .OrderByDescending(m => Environment.Minion.countMinionsInrange(m.Position, 180f))
                        .FirstOrDefault();

                if (bestPos != null &&
                    Environment.Minion.countMinionsInrange(bestPos.Position, 180f) >=
                    config.Item("eMinHit", true).GetValue<Slider>().Value)
                {
                    E.CastOnUnit(bestPos);
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(
                E.Range, TargetSelector.DamageType.Physical, true, HeroManager.Enemies.Where(h => h.IsInvulnerable));
            if (target == null)
            {
                return;
            }
            bool canKill = ComboDamage(target) + ItemHandler.GetItemsDamage(target) +
                           player.GetAutoAttackDamage(target) * 2 > target.Health;
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) && !E.CanCast(target) && !justQ && !justE &&
                (target.Distance(player) > 500 || player.HealthPercent < 25))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (rActive)
            {
                return;
            }
            if (canKill)
            {
                orbwalker.SetAttack(true);
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target) &&
                target.Health < player.GetAutoAttackDamage(target) + Q.GetDamage(target))
            {
                Q.Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, target);
            }
            if (config.Item("usew", true).GetValue<bool>() && !Q.IsReady() && !qActive && !player.UnderTurret(true) &&
                W.IsReady() && !canKill &&
                ((!Q.IsReady() && !E.IsReady() && !justE && target.HealthPercent > 20 &&
                  config.Item("wHealth", true).GetValue<Slider>().Value > player.HealthPercent &&
                  Orbwalking.GetRealAutoAttackRange(target) > player.Distance(target) &&
                  CombatHelper.IsFacing(target, player.Position, 45)) ||
                 (config.Item("wOnFocus", true).GetValue<bool>() &&
                  Program.IncDamages.GetAllyData(player.NetworkId).DamageCount >= 3)))
            {
                W.Cast();
            }
            if (R.IsReady() && config.Item("userone", true).GetValue<bool>() && canKill && !eActive && !Q.IsReady() &&
                player.Distance(target) < R.Range && player.HealthPercent < 55 && player.HealthPercent > 10)
            {
                R.Cast();
            }
            if (R.IsReady() && config.Item("Rmin", true).GetValue<Slider>().Value <= player.CountEnemiesInRange(R.Range))
            {
                R.Cast();
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config);
            }
            if (config.Item("usee", true).GetValue<bool>() && E.CanCast(target) && Orbwalking.CanMove(100) &&
                (config.Item("eMinRange", true).GetValue<Slider>().Value < player.Distance(target) ||
                 player.HealthPercent < 20 || (player.CountEnemiesInRange(800) == 1 && target.HealthPercent < 20)))
            {
                E.CastOnUnit(target);
            }
        }

        private static bool rActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "MonkeyKingSpinToWin"); }
        }

        private static bool wActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "monkeykingdecoystealth"); }
        }

        private static bool eActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "monkeykingnimbusas"); }
        }

        private static bool qActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "MonkeyKingDoubleAttack"); }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += player.GetAutoAttackDamage(hero) + Q.GetDamage(hero) +
                          Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R) * 4;
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

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Base))
            {
                return;
            }
            if (sender.IsMe && args.SData.Name == "MonkeyKingDoubleAttack")
            {
                justQ = true;
                LeagueSharp.Common.Utility.DelayAction.Add(200, () => justQ = false);
            }
            if (sender.IsMe && args.SData.Name == "MonkeyKingNimbus")
            {
                justE = true;
                LeagueSharp.Common.Utility.DelayAction.Add(500, () => justE = false);
            }
            if (sender.IsMe && args.SData.Name == "MonkeyKingDecoy")
            {
                justW = true;
                LeagueSharp.Common.Utility.DelayAction.Add(config.Item("wMinTime", true).GetValue<Slider>().Value, () => justW = false);
            }
        }

        private void InitMenu()
        {
            config = new Menu("Wukong ", "Wukong", true);
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
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("wHealth", "   Under health", true)).SetValue(new Slider(50, 0, 100));
            menuC.AddItem(new MenuItem("wOnFocus", "   On focus", true)).SetValue(true);
            menuC.AddItem(new MenuItem("wMinTime", "   Min time(ms)", true)).SetValue(new Slider(800, 0, 1500));
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("eMinRange", "   Min range", true)).SetValue(new Slider(400, 0, (int) E.Range));
            menuC.AddItem(new MenuItem("userone", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("Rmin", "R min", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("rType", "R type", true))
                .SetValue(new StringList(new[] { "Most enemy", "Focus selected", "To Cursor" }, 1));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("eMinHit", "   E min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("Interrupt", "Cast R to interrupt spells", true)).SetValue(false);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}