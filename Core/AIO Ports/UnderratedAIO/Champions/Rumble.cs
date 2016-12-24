using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;

using Prediction = LeagueSharp.Common.Prediction;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Rumble
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justE;
        public Vector3 qPos, lastpos;
        public float lastE;

        public Rumble()
        {
            InitRumble();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Rumble</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (config.Item("usewgc", true).GetValue<bool>() && gapcloser.End.Distance(player.Position) < 200)
            {
                W.Cast();
            }
        }

        private void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsEnemy && config.Item("useegc", true).GetValue<bool>() && sender is AIHeroClient &&
                args.EndPos.Distance(player.Position) < E.Range && E.CanCast(sender))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(args.Duration, () => { E.Cast(args.EndPos); });
            }
        }

        private void InitRumble()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 800);
            E.SetSkillshot(0.25f, 70, 1200, true, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 1700);
            R.SetSkillshot(0.4f, 200, 1600, false, SkillshotType.SkillshotLine);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if(false)
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
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (data != null && W.IsReady() && config.Item("usew", true).GetValue<bool>() &&
                (preventSilence(W) || (!config.Item("blockW", true).GetValue<bool>() && !preventSilence(W))) &&
                (data.DamageTaken > getShield() * config.Item("shieldPercent", true).GetValue<Slider>().Value / 100 ||
                 config.Item("Aggro", true).GetValue<Slider>().Value <= data.DamageCount))
            {
                W.Cast();
            }
            if (config.Item("castR", true).GetValue<KeyBind>().Active)
            {
                AIHeroClient target = DrawHelper.GetBetterTarget(1700, TargetSelector.DamageType.Magical, true);
                if (target != null)
                {
                    HandleR(target, true);
                }
            }
        }


        private void Harass()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(1300, TargetSelector.DamageType.Magical, true);
            if (target == null || target.IsInvulnerable)
            {
                return;
            }
            if (Qhit(target.Position) && config.Item("useqH", true).GetValue<bool>() && preventSilence(Q))
            {
                Q.Cast(target.Position);
            }
            if (E.CanCast(target) && config.Item("useeH", true).GetValue<bool>() && preventSilence(E) &&
                (!ActiveE ||
                 System.Environment.TickCount - lastE > config.Item("HeDelay", true).GetValue<Slider>().Value ||
                 getEdamage(target) > target.Health))
            {
                E.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
        }

        private static float getEdamage(AIHeroClient target)
        {
            if (!E.IsReady())
            {
                return 0;
            }
            var num = ActiveE ? 1 : 2;
            var dmg = Damage.GetSpellDamage(player, target, SpellSlot.E) * num;
            return (float) (Enhanced ? dmg * 1.5f : dmg);
        }

        private static float getQdamage(AIHeroClient target)
        {
            if (!Q.IsReady() && !ActiveQ)
            {
                return 0;
            }
            var dmg = QDamage(target, true);
            return (float) (Enhanced ? dmg * 1.5f : dmg);
        }

        private double getShield()
        {
            return new double[] { 50, 80, 110, 140, 170 }[W.Level - 1] + 0.4f * player.TotalMagicalDamage;
        }

        private static float getRdamage(AIHeroClient target)
        {
            var dmg = new double[] { 130, 185, 240 }[R.Level - 1] + 0.3f * player.TotalMagicalDamage;
            return (float) dmg;
        }

        private bool Qhit(Vector3 target)
        {
            return Q.IsReady() &&
                   (CombatHelper.IsFacing(player, target, 80) && target.Distance(player.Position) < Q.Range);
        }

        private static bool Enhanced
        {
            get { return player.Mana >= 50 && player.Mana < 100; }
        }


        private static bool Silenced
        {
            get { return player.HasBuff("rumbleoverheat"); }
        }

        private static bool ActiveQ
        {
            get { return player.HasBuff("RumbleFlameThrower"); }
        }

        private static bool ActiveW
        {
            get { return player.HasBuff("RumbleShield"); }
        }

        private static bool ActiveE
        {
            get { return player.HasBuff("RumbleGrenade"); }
        }

        private void Clear()
        {
            if (Q.IsReady() && config.Item("useqLC", true).GetValue<bool>() && preventSilence(Q))
            {
                var minons = MinionManager.GetMinions(player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                if (minons.Count(m => Qhit(m.Position)) >= config.Item("qMinHit", true).GetValue<Slider>().Value)
                {
                    Q.Cast(Game.CursorPos);
                    return;
                }
            }
        }

        private bool preventSilence(Spell spell)
        {
            return (spell.Slot == SpellSlot.E ? 10 : 20) + player.Mana < 100;
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(1700, TargetSelector.DamageType.Magical, true);
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var edmg = getEdamage(target);
            var qdmg = getQdamage(target);
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 700) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                (!ActiveQ ||
                 (!(CombatHelper.IsFacing(player, target.Position, 30) && target.Distance(player) < Q.Range))))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (Q.CanCast(target) && config.Item("useq", true).GetValue<bool>() && Qhit(target.Position) &&
                (preventSilence(Q) ||
                 (target.Health < PassiveDmg(target) * 2 || qdmg > target.Health) &&
                 target.Distance(player) < Orbwalking.GetRealAutoAttackRange(target)))
            {
                Q.Cast(target.Position);
            }
            if (config.Item("usee", true).GetValue<bool>() && E.CanCast(target) &&
                (((preventSilence(E) ||
                   (target.Health < PassiveDmg(target) * 2 &&
                    target.Distance(player) < Orbwalking.GetRealAutoAttackRange(target))) &&
                  (!ActiveE ||
                   System.Environment.TickCount - lastE > config.Item("eDelay", true).GetValue<Slider>().Value)) ||
                 edmg > target.Health))
            {
                E.CastIfHitchanceEquals(target, HitChance.High);
            }
            if (W.IsReady() && config.Item("wSpeed", true).GetValue<bool>() && ActiveQ && preventSilence(W) &&
                target.Distance(player) < Q.Range &&
                Prediction.GetPrediction(target, 0.2f).UnitPosition.Distance(player.Position) > Q.Range)
            {
                W.Cast();
            }
            var canR = ComboDamage(target) > target.Health && qdmg < target.Health && target.Distance(player) < Q.Range &&
                       !Silenced;
            if (R.IsReady() &&
                (((target.Health <
                   getRdamage(target) * ((target.CountAlliesInRange(600) > 0 && target.HealthPercent > 15) ? 5 : 3) &&
                   target.Distance(player) > Q.Range) ||
                  (target.Distance(player) < Q.Range && target.Health < getRdamage(target) * 3 + edmg &&
                   target.Health > qdmg)) ||
                 player.CountEnemiesInRange(R.Range) >= config.Item("Rmin", true).GetValue<Slider>().Value))
            {
                HandleR(target, canR);
            }
        }

        private void HandleR(Obj_AI_Base target, bool manual = false)
        {
            if (Program.IsSPrediction)
            {
                if (config.Item("userEnabled", true).GetValue<bool>())
                {
                    R.SPredictionCastVector(target as AIHeroClient, 1000f, HitChance.High);
                }
            }
            else
            {
                var targE = R.GetPrediction(target);
                if ((config.Item("user", true).GetValue<bool>() && player.CountEnemiesInRange(R.Range + 175) <= 1 &&
                     config.Item("userEnabled", true).GetValue<bool>()) || manual)
                {
                    if (target.IsMoving)
                    {
                        var pos = targE.CastPosition;
                        if (pos.IsValid() && pos.Distance(player.Position) < R.Range + 1000 &&
                            targE.Hitchance >= HitChance.High)
                        {
                            R.Cast(target.Position.Extend(pos, -500), pos);
                        }
                    }
                    else
                    {
                        R.Cast(target.Position.Extend(player.Position, 500), target.Position);
                    }
                }
                else if (targE.Hitchance >= HitChance.High && config.Item("userEnabled", true).GetValue<bool>())
                {
                    var pred = getBestRVector3(target, targE);
                    if (pred != Vector3.Zero &&
                        CombatHelper.GetCollisionCount(
                            target, target.Position.Extend(pred, 1000), R.Width, new[] { CollisionableObjects.Heroes, }) >=
                        config.Item("Rmin", true).GetValue<Slider>().Value)
                    {
                        R.Cast(target.Position.Extend(pred, -target.MoveSpeed), pred);
                    }
                }
            }
        }

        private Vector3 getBestRVector3(Obj_AI_Base target, PredictionOutput targE)
        {
            var otherHeroes =
                HeroManager.Enemies.Where(
                    e => e.IsValidTarget() && e.NetworkId != target.NetworkId && player.Distance(e) < 1000)
                    .Select(e => R.GetPrediction(e))
                    .Where(o => o.Hitchance > HitChance.High && o.CastPosition.Distance(targE.UnitPosition) < 1000);
            if (otherHeroes.Any())
            {
                var best =
                    otherHeroes.OrderByDescending(
                        hero =>
                            CombatHelper.GetCollisionCount(
                                target, target.Position.Extend(hero.CastPosition, 1000), R.Width,
                                new[] { CollisionableObjects.Heroes, })).FirstOrDefault();
                if (best != null)
                {
                    return best.CastPosition;
                }
            }
            return Vector3.Zero;
        }

        private IEnumerable<Vector3> GetRpoints(Obj_AI_Base target)
        {
            var targetPos = R.GetPrediction(target);
            return
                CombatHelper.PointsAroundTheTargetOuterRing(targetPos.CastPosition, 345, 16)
                    .Where(p => player.Distance(p) < R.Range);
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady() || player.HasBuff("RumbleFlameThrower"))
            {
                damage += getQdamage(hero);
            }
            if (E.IsReady())
            {
                damage += getEdamage(hero);
            }
            if (R.IsReady())
            {
                damage += getRdamage(hero) * 4;
            }
            //damage += ItemHandler.GetItemsDamage(target);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }


        private static float PassiveDmg(Obj_AI_Base hero)
        {
            return
                (float)
                    (Damage.CalcDamage(
                        player, hero, Damage.DamageType.Magical,
                        20 + (5 * player.Level) + player.TotalMagicalDamage * 0.3f) + player.GetAutoAttackDamage(hero));
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "RumbleGrenade")
                {
                    var dist = player.Distance(args.End);
                    justE = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        (int) ((dist > E.Range ? E.Range : dist) / E.Speed * 1000), () => justE = false);
                    lastE = System.Environment.TickCount;
                }
            }
            if (config.Item("usew", true).GetValue<bool>() && sender is AIHeroClient && sender.IsEnemy &&
                player.Distance(sender) < Q.Range && Program.IncDamages.GetAllyData(player.NetworkId).AnyCC)
            {
                W.Cast();
            }
        }

        private static double QDamage(AIHeroClient target, bool bufftime = false)
        {
            var buff = player.GetBuff("RumbleFlameThrower");
            var percentage = 1d;
            if (bufftime && buff != null)
            {
                percentage = CombatHelper.GetBuffTime(buff) / 3f;
            }
            var dmg = Q.GetDamage(target);
            return dmg * percentage;
        }

        private void InitMenu()
        {
            config = new Menu("Rumble ", "Rumble", true);
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
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings 
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("eDelay", "   Delay between E", true)).SetValue(new Slider(2000, 0, 2990));
            menuC.AddItem(new MenuItem("wSpeed", "Use W to speed up", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userEnabled", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "   1v1", true)).SetValue(true);
            menuC.AddItem(new MenuItem("Rmin", "   Teamfight", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("castR", "R manual cast", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(true);
            menuH.AddItem(new MenuItem("HeDelay", "   Delay between E", true)).SetValue(new Slider(1500, 0, 2990));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("qMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            config.AddSubMenu(menuLC);

            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("usewgc", "Use W gapclosers", true)).SetValue(false);
            menuM.AddItem(new MenuItem("useegc", "Use E gapclosers", true)).SetValue(true);
            menuM.AddItem(new MenuItem("usew", "Use W to shield", true)).SetValue(true);
            menuM.AddItem(new MenuItem("shieldPercent", "   Shield %", true)).SetValue(new Slider(50, 0, 100));
            menuM.AddItem(new MenuItem("Aggro", "   Aggro", true)).SetValue(new Slider(3, 0, 10));
            menuM.AddItem(new MenuItem("blockW", "   Don't silence me pls", true)).SetValue(true);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);


            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}