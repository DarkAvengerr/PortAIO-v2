using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using SPrediction;
using UnderratedAIO.Helpers;
using UnderratedAIO.Helpers.SkillShot;
using Environment = UnderratedAIO.Helpers.Environment;

using Prediction = LeagueSharp.Common.Prediction;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Trundle
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public Trundle()
        {
            InitTrundle();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Trundle</font>");
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Jungle.setSmiteSlot();
            Obj_AI_Base.OnDamage += Obj_AI_Base_OnDamage;
            Orbwalking.AfterAttack += AfterAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!config.Item("AutoEDash", true).GetValue<bool>())
            {
                return;
            }
            var hero = sender as AIHeroClient;
            if (!E.IsReady() || hero == null || sender.IsAlly)
            {
                return;
            }
            if (!DrawHelper.dashEnabled(hero.ChampionName))
            {
                return;
            }
            if (sender.Position.Distance(args.End) > 250 &&
                CombatHelper.DashDatas.Any(d => d.ChampionName == hero.ChampionName && d.Slot == args.Slot))
            {
                var dashIntPoint = sender.Position.Extend(args.End, 250);
                if (dashIntPoint.Distance(player.Position) < 1000)
                {
                    E.Cast(dashIntPoint);
                }
            }
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            AIHeroClient targetO = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Physical, true);

            if (unit != null && unit.IsMe && Q.IsReady() && target != null)
            {
                if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear &&
                    config.Item("useqLC", true).GetValue<bool>())
                {
                    var minis = MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition, 600, MinionTypes.All, MinionTeam.NotAlly);

                    float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
                    if (player.Mana > player.MaxMana * perc &&
                        (minis.Count() > 1 || player.GetAutoAttackDamage((Obj_AI_Base) target, true) < target.Health))
                    {
                        Q.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
                if (targetO != null && targetO.NetworkId != target.NetworkId)
                {
                    return;
                }
                if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                    config.Item("useqH", true).GetValue<bool>() && target is AIHeroClient)
                {
                    float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
                    if (player.Mana > player.MaxMana * perc)
                    {
                        Q.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                    }
                }
                if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                    config.Item("useq", true).GetValue<bool>())
                {
                    Q.Cast();
                    Orbwalking.ResetAutoAttackTimer();
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender == null)
            {
                return;
            }
            if (!DrawHelper.IntEnabled(sender.ChampionName))
            {
                return;
            }
            if (config.Item("AutoEinterrupt", true).GetValue<bool>() && E.CanCast(sender))
            {
                E.Cast(sender.Position);
            }
        }

        private void Obj_AI_Base_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (config.Item("AutoETower", true).GetValue<bool>() && E.IsReady())
            {
                var t = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.NetworkId == args.Source.NetworkId);
                var s = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.NetworkId == args.Target.NetworkId);
                if (t == null || s == null)
                {
                    return;
                }
                var turret =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .FirstOrDefault(tw => tw.Distance(t) < 1000 && tw.Distance(s) < 1000 && tw.IsAlly);
                if (s is AIHeroClient && t is AIHeroClient && s.IsAlly && turret != null && E.CanCast(t))
                {
                    E.Cast(t.Position.Extend(turret.Position, -(t.BoundingRadius + E.Width)));
                }
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(
                config.Item("drawee", true).GetValue<Circle>(), config.Item("useeRange", true).GetValue<Slider>().Value);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if(false)
            {
                return;
            }
            orbwalker.SetOrbwalkingPoint(Vector3.Zero);
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }
            if (false && E.IsReady() && config.Item("AutoEDash", true).GetValue<bool>())
            {
                foreach (var data in HeroManager.Allies.Select(a => Program.IncDamages.GetAllyData(a.NetworkId)))
                {
                    foreach (var skillshot in
                        data.Damages.Where(
                            d =>
                                d.SkillShot != null && d.SkillShot.SkillshotData.Slot == SpellSlot.R &&
                                d.SkillShot.SkillshotData.ChampionName == "Blitzcrank"))
                    {
                        E.Cast(skillshot.Target.Position.Extend(skillshot.SkillShot.StartPosition.To3D(), E.Range * 2));
                    }
                }
            }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady() && !Orbwalking.CanAttack())
            {
                Q.Cast();
            }
            if (config.Item("usewLC", true).GetValue<bool>() && W.IsReady())
            {
                var minis = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, 600, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPositionE = W.GetCircularFarmLocation(minis, 600);
                if (bestPositionE.MinionsHit >= config.Item("wMinHit", true).GetValue<Slider>().Value ||
                    minis.Any(m => m.MaxHealth > 1500))
                {
                    W.Cast(bestPositionE.Position);
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Physical, true);
            if (player.Spellbook.IsAutoAttacking || target == null || !Orbwalking.CanMove(100))
            {
                return;
            }
            if (config.Item("usee", true).GetValue<bool>() &&
                player.Distance(target) < config.Item("useeRange", true).GetValue<Slider>().Value && E.IsReady())
            {
                var pos = GetVectorE(target);
                if (player.Distance(pos) < E.Range)
                {
                    E.Cast(pos);
                }
            }
            if (config.Item("usew", true).GetValue<bool>() && W.IsReady() &&
                (!target.UnderTurret(true) || player.UnderTurret(true)))
            {
                var pos = player.Position.Extend(Prediction.GetPrediction(target, 700).UnitPosition, W.Range / 2);
                if (player.Distance(pos) < W.Range)
                {
                    W.Cast(pos);
                }
            }
            if (config.Item("user", true).GetValue<bool>() && R.IsReady())
            {
                AIHeroClient targetR = null;
                switch (config.Item("userTarget", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        targetR =
                            HeroManager.Enemies.Where(e => e.IsValidTarget(R.Range))
                                .OrderByDescending(e => (e.Armor + e.FlatMagicReduction))
                                .FirstOrDefault();
                        break;
                    case 1:
                        targetR = target.IsValidTarget(R.Range) ? target : null;
                        break;
                }

                if (targetR != null)
                {
                    var userTime = config.Item("userTime", true).GetValue<StringList>().SelectedIndex;
                    if (userTime == 0 || userTime == 2)
                    {
                        if (player.CountEnemiesInRange(R.Range) >= 2)
                        {
                            R.Cast(targetR);
                        }
                    }
                    if (userTime == 1 || userTime == 2)
                    {
                        var data = Program.IncDamages.GetAllyData(player.NetworkId);
                        if (data.DamageTaken > player.Health * 0.4 || data.IsAboutToDie ||
                            (player.HealthPercent < 60 && target.HealthPercent < 60 &&
                             player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target)))
                        {
                            R.Cast(targetR);
                        }
                    }
                }
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 1000) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                ((player.HealthPercent < 35) ||
                 (target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target) + 25)))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private Vector3 GetVectorE(AIHeroClient target)
        {
            var pos = Vector3.Zero;
            var pred = Prediction.GetPrediction(target, 0.28f);
            if (!target.IsMoving)
            {
                return pos;
            }
            var distW = E.Width / 2 + target.BoundingRadius;
            var points = CombatHelper.PointsAroundTheTarget(pred.UnitPosition, distW);
            var walls =
                points.Where(p => p.IsWall() && player.Distance(target) > target.BoundingRadius)
                    .OrderBy(p => p.Distance(pred.UnitPosition));
            var wall = walls.FirstOrDefault();
            if (wall.IsValid() && wall.Distance(target.Position) < 350 &&
                walls.Any(w => w.Distance(target.Position) < distW))
            {
                pos = wall.Extend(pred.UnitPosition, (target.BoundingRadius + distW));
            }
            if (config.Item("useeWall", true).GetValue<bool>())
            {
                return pos;
            }
            if (pred.Hitchance < HitChance.Medium || target.Distance(player) < Orbwalking.GetRealAutoAttackRange(target))
            {
                return pos;
            }
            if (pred.UnitPosition.Distance(player.Position) > player.Distance(target))
            {
                var dist = target.BoundingRadius + E.Width;
                var predPos = pred.UnitPosition;
                if (target.Distance(predPos) < dist)
                {
                    predPos = target.Position.Extend(predPos, dist);
                }
                pos = predPos.Extend(target.Position, -dist);
            }
            return pos;
        }

        private void InitMenu()
        {
            config = new Menu("Trundle ", "Trundle", true);
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
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 87, 244, 255)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 87, 244, 255)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 87, 244, 255)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings 
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useeWall", "   Only Near wall", true)).SetValue(false);
            menuC.AddItem(new MenuItem("useeRange", "Max range", true))
                .SetValue(new Slider(600, (int) player.AttackRange, 1000));
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userTarget", "   R target", true))
                .SetValue(new StringList(new[] { "Highest def", "Only target" }, 0));
            menuC.AddItem(new MenuItem("userTime", "   R usage", true))
                .SetValue(new StringList(new[] { ">= 2 enemy", "Before high damage", "Both" }, 1));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(false);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("wMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(20, 1, 100));
            config.AddSubMenu(menuLC);

            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("AutoETower", "Use E on tower aggro", true)).SetValue(true);
            menuM.AddItem(new MenuItem("AutoEinterrupt", "Use E to interrupt", true)).SetValue(true);
            menuM.AddItem(new MenuItem("AutoEDash", "Use E ond Dash", true)).SetValue(true);


            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R);
            }
            damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void InitTrundle()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1000);
            W.SetSkillshot(0.25f, 1000, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 1000);
            E.SetSkillshot(0.25f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 650);
        }
    }
}