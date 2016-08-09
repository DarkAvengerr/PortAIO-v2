using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;
using UnderratedAIO.Helpers;
using Color = System.Drawing.Color;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Shen
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        private static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, EFlash, R;
        private static float bladeRadius = 325f;
        public static bool PingCasted = false;
        private const int XOffset = 36;
        private const int YOffset = 9;
        private const int Width = 103;
        private const int Height = 8;
        public static Vector3 blade, bladeOnCast;
        public static bool justW;

        private static readonly Render.Text Text = new Render.Text(
            0, 0, "", 11, new ColorBGRA(255, 0, 0, 255), "monospace");

        public static AutoLeveler autoLeveler;

        public Shen()
        {
            InitShen();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Shen</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
            Obj_AI_Base.OnDamage += Obj_AI_Base_OnDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
        }

        private void Obj_AI_Base_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            var t = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.NetworkId == args.Source.NetworkId);
            var s = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h => h.NetworkId == args.Target.NetworkId);
            if (t != null && s != null &&
                (t.IsMe &&
                 ObjectManager.Get<Obj_AI_Turret>()
                     .FirstOrDefault(tw => tw.Distance(t) < 750 && tw.Distance(s) < 750 && tw.IsAlly) != null))
            {
                if (config.Item("autotauntattower", true).GetValue<bool>() && E.CanCast(s))
                {
                    E.Cast(s);
                }
            }
        }


        private void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!config.Item("useeint", true).GetValue<bool>())
            {
                return;
            }
            if (unit.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(unit);
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("draweeflash", true).GetValue<Circle>(), EFlash.Range);
            if (config.Item("drawallyhp", true).GetValue<bool>())
            {
                DrawHealths();
            }
            if (config.Item("drawincdmg", true).GetValue<bool>())
            {
                getIncDmg();
            }
            if (true)
            {
                Render.Circle.DrawCircle(blade, bladeRadius, Color.BlueViolet, 7);
            }
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private static void DrawHealths()
        {
            float i = 0;
            foreach (
                var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead))
            {
                var playername = hero.Name;
                if (playername.Length > 13)
                {
                    playername = playername.Remove(9) + "...";
                }
                var champion = hero.BaseSkinName;
                if (champion.Length > 12)
                {
                    champion = champion.Remove(7) + "...";
                }
                var percent = (int) (hero.Health / hero.MaxHealth * 100);
                var color = Color.Red;
                if (percent > 25)
                {
                    color = Color.Orange;
                }
                if (percent > 50)
                {
                    color = Color.Yellow;
                }
                if (percent > 75)
                {
                    color = Color.LimeGreen;
                }
                Drawing.DrawText(
                    Drawing.Width * 0.8f, Drawing.Height * 0.15f + i, color, playername + "(" + champion + ")");
                Drawing.DrawText(
                    Drawing.Width * 0.9f, Drawing.Height * 0.15f + i, color,
                    ((int) hero.Health).ToString() + " (" + percent.ToString() + "%)");
                i += 20f;
            }
        }

        private static void getIncDmg()
        {
            var color = Color.Red;
            float result = CombatHelper.getIncDmg();
            var barPos = player.HPBarPosition;
            var damage = (float) result;
            if (damage == 0)
            {
                return;
            }
            var percentHealthAfterDamage = Math.Max(0, player.Health - damage) / player.MaxHealth;
            var xPos = barPos.X + XOffset + Width * percentHealthAfterDamage;

            if (damage > player.Health)
            {
                Text.X = (int) barPos.X + XOffset;
                Text.Y = (int) barPos.Y + YOffset - 13;
                Text.text = ((int) (player.Health - damage)).ToString();
                Text.OnEndScene();
            }

            Drawing.DrawLine(xPos, barPos.Y + YOffset, xPos, barPos.Y + YOffset + Height, 3, color);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            Ulti();
            if (config.Item("useeflash", true).GetValue<KeyBind>().Active &&
                player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) == SpellState.Ready)
            {
                FlashCombo();
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
            var bladeObj =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        o => (o.Name == "ShenSpiritUnit" || o.Name == "ShenArrowVfxHostMinion") && o.Team == player.Team)
                    .OrderBy(o => o.Distance(bladeOnCast))
                    .FirstOrDefault();
            if (bladeObj != null)
            {
                blade = bladeObj.Position;
            }
            if (W.IsReady() && blade.IsValid())
            {
                foreach (var ally in HeroManager.Allies.Where(a => a.Distance(blade) < bladeRadius))
                {
                    var data = Program.IncDamages.GetAllyData(ally.NetworkId);
                    if (config.Item("autowAgg", true).GetValue<Slider>().Value <= data.AADamageCount)
                    {
                        W.Cast();
                    }
                    if (data.AADamageTaken >= ally.Health * 0.2f && config.Item("autow", true).GetValue<bool>())
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static bool HasShield
        {
            get { return player.HasBuff("shenpassiveshield"); }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!config.Item("useeagc", true).GetValue<bool>())
            {
                return;
            }
            if (gapcloser.Sender.IsValidTarget(E.Range) && E.IsReady() &&
                player.Distance(gapcloser.Sender.Position) < 400)
            {
                E.Cast(gapcloser.End);
            }
        }

        private static void Clear()
        {
            var minionsHP = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(400)).Sum(m => m.Health);

            if (config.Item("useqLC", true).GetValue<bool>() && minionsHP > 300 && CheckQDef())
            {
                Q.Cast();
            }
        }

        private static void Ulti()
        {
            if (!R.IsReady() || player.IsDead)
            {
                return;
            }

            foreach (var allyObj in
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        i =>
                            i.IsAlly && !i.IsMe && !i.IsDead &&
                            ((((Program.IncDamages.GetAllyData(i.NetworkId).DamageTaken > i.Health ||
                                (i.Health - Program.IncDamages.GetAllyData(i.NetworkId).DamageTaken) * 100f /
                                i.MaxHealth <= config.Item("atpercent", true).GetValue<Slider>().Value) &&
                               i.CountEnemiesInRange(700) > 0) ||
                              Program.IncDamages.GetAllyData(i.NetworkId).SkillShotDamage > i.Health))))

            {
                if (config.Item("user", true).GetValue<bool>() &&
                    orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && R.IsReady() &&
                    player.CountEnemiesInRange(EFlash.Range + 50) < 1 &&
                    !config.Item("ult" + allyObj.BaseSkinName).GetValue<bool>())
                {
                    R.Cast(allyObj);
                    return;
                }
                if (!PingCasted)
                {
                    //ping
                    DrawHelper.popUp("Use R to help " + allyObj.ChampionName, 3000, Color.White, Color.Black, Color.Red);
                    PingCasted = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(5000, () => PingCasted = false);
                }
            }
        }

        private static void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target != null && Q.IsReady() && config.Item("harassq", true).GetValue<bool>() &&
                Orbwalking.CanMove(100))
            {
                HandleQ(target);
            }
        }

        private static void Combo()
        {
            var minHit = config.Item("useemin", true).GetValue<Slider>().Value;
            AIHeroClient target = TargetSelector.GetTarget(E.Range + 400, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var useE = config.Item("usee", true).GetValue<bool>() && E.IsReady() &&
                       player.Distance(target.Position) < E.Range;
            if (useE)
            {
                if (minHit > 1)
                {
                    CastEmin(target, minHit);
                }
                else if ((player.Distance(target.Position) > Orbwalking.GetRealAutoAttackRange(target) ||
                          player.HealthPercent < 45 || player.CountEnemiesInRange(1000) == 1) &&
                         E.GetPrediction(target).Hitchance >= HitChance.High)
                {
                    CastETarget(target);
                }
            }
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !E.CanCast(target) && !Q.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (Q.IsReady() && config.Item("useq", true).GetValue<bool>() && Orbwalking.CanMove(100))
            {
                HandleQ(target);
            }
            if (config.Item("usew", true).GetValue<bool>())
            {
                foreach (var ally in HeroManager.Allies.Where(a => a.Distance(blade) < bladeRadius))
                {
                    var data = Program.IncDamages.GetAllyData(ally.NetworkId);
                    if (data.AADamageTaken >= target.GetAutoAttackDamage(ally) - 10)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void CastETarget(AIHeroClient target)
        {
            if (Program.IsSPrediction)
            {
                var pred = E.GetPrediction(target);
                var poly = CombatHelper.GetPoly(pred.UnitPosition, E.Range, E.Width);
                var enemiesBehind =
                    HeroManager.Enemies.Count(
                        e =>
                            e.NetworkId != target.NetworkId && e.IsValidTarget(E.Range) &&
                            (poly.IsInside(E.GetPrediction(e).UnitPosition) || poly.IsInside(e.Position)) &&
                            e.Position.Distance(player.Position) > player.Distance(pred.UnitPosition));
                if (pred.Hitchance >= HitChance.High)
                {
                    if (enemiesBehind > 0)
                    {
                        E.Cast(player.ServerPosition.Extend(pred.CastPosition, E.Range));
                    }
                    else
                    {
                        if (poly.IsInside(pred.UnitPosition) && poly.IsInside(target.Position))
                        {
                            E.Cast(
                                player.ServerPosition.Extend(
                                    pred.CastPosition,
                                    player.Distance(pred.CastPosition) + Orbwalking.GetRealAutoAttackRange(target)));
                        }
                        else
                        {
                            E.Cast(
                                player.ServerPosition.Extend(
                                    pred.CastPosition,
                                    player.Distance(pred.CastPosition) + Orbwalking.GetRealAutoAttackRange(target)));
                        }
                    }
                }
            }
            else
            {
                var pred = E.GetSPrediction(target);
                var poly = CombatHelper.GetPoly(pred.UnitPosition.To3D2(), E.Range, E.Width);
                var enemiesBehind =
                    HeroManager.Enemies.Count(
                        e =>
                            e.NetworkId != target.NetworkId && e.IsValidTarget(E.Range) &&
                            (poly.IsInside(E.GetPrediction(e).UnitPosition) || poly.IsInside(e.Position)) &&
                            e.Position.Distance(player.Position) > player.Distance(pred.UnitPosition));
                if (pred.HitChance >= HitChance.High)
                {
                    if (enemiesBehind > 0)
                    {
                        E.Cast(player.ServerPosition.Extend(pred.CastPosition.To3D2(), E.Range));
                    }
                    else
                    {
                        if (poly.IsInside(pred.UnitPosition) && poly.IsInside(target.Position))
                        {
                            E.Cast(
                                player.ServerPosition.Extend(
                                    pred.CastPosition.To3D2(),
                                    player.Distance(pred.CastPosition) + Orbwalking.GetRealAutoAttackRange(target)));
                        }
                        else
                        {
                            E.Cast(
                                player.ServerPosition.Extend(
                                    pred.CastPosition.To3D2(),
                                    player.Distance(pred.CastPosition) + Orbwalking.GetRealAutoAttackRange(target)));
                        }
                    }
                }
            }
        }

        private static void HandleQ(AIHeroClient target)
        {
            Q.UpdateSourcePosition(blade);
            var pred = Q.GetPrediction(target);
            var poly = CombatHelper.GetPoly(blade.Extend(player.Position, 30), player.Distance(blade), 150);
            if (((pred.Hitchance >= HitChance.VeryHigh && poly.IsInside(pred.UnitPosition)) ||
                 (target.Distance(blade) < 100) || (target.Distance(blade) < 500 && poly.IsInside(target.Position)) ||
                 player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) || player.Spellbook.IsAutoAttacking) &&
                CheckQDef())
            {
                Q.Cast();
            }
        }

        private static bool CheckQDef()
        {
            if (blade.CountAlliesInRange(bladeRadius) == 0 || !justW)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CastEmin(AIHeroClient target, int min)
        {
            var MaxEnemy = player.CountEnemiesInRange(1580);
            if (MaxEnemy == 1)
            {
                CastETarget(target);
            }
            else
            {
                var MinEnemy = Math.Min(min, MaxEnemy);
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(i => i.Distance(player) < E.Range && i.IsEnemy && !i.IsDead && i.IsValidTarget()))
                {
                    for (int i = MaxEnemy; i > MinEnemy - 1; i--)
                    {
                        if (Program.IsSPrediction)
                        {
                            var pred = E.GetSPrediction(enemy);
                            if (E.SPredictionCast(enemy, HitChance.High, 0, (byte) i))
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (E.CastIfWillHit(enemy, i))
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void FlashCombo()
        {
            AIHeroClient target = TargetSelector.GetTarget(EFlash.Range, TargetSelector.DamageType.Magical);
            if (target != null && E.IsReady() && E.ManaCost < player.Mana &&
                player.Distance(target.Position) < EFlash.Range && player.Distance(target.Position) > 480 &&
                !((getPosToEflash(target.Position)).IsWall()))
            {
                var pred = EFlash.GetPrediction(target);
                var poly = CombatHelper.GetPolyFromVector(getPosToEflash(target.Position), pred.UnitPosition, E.Width);
                var enemiesBehind =
                    HeroManager.Enemies.Count(
                        e =>
                            e.NetworkId != target.NetworkId && e.IsValidTarget(E.Range) &&
                            (poly.IsInside(E.GetPrediction(e).UnitPosition) || poly.IsInside(e.Position)) &&
                            e.Position.Distance(player.Position) > player.Distance(pred.UnitPosition));
                if (pred.Hitchance >= HitChance.High)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        30, () =>
                        {
                            if (enemiesBehind > 0)
                            {
                                E.Cast(player.ServerPosition.Extend(pred.CastPosition, E.Range));
                            }
                            else
                            {
                                E.Cast(
                                    player.ServerPosition.Extend(
                                        pred.CastPosition,
                                        player.Distance(pred.CastPosition) + Orbwalking.GetRealAutoAttackRange(target)));
                            }
                        });
                    player.Spellbook.CastSpell(player.GetSpellSlot("SummonerFlash"), getPosToEflash(target.Position));
                }
            }
            ItemHandler.UseItems(target, config);
            Orbwalking.MoveTo(Game.CursorPos);
        }


        public static Vector3 getPosToEflash(Vector3 target)
        {
            return target + (player.Position - target) / 2;
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            float damage = 0;
            if (Q.IsReady() && player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana < player.Mana)
            {
                damage += (float) Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (E.IsReady() && player.Spellbook.GetSpell(SpellSlot.E).SData.Mana < player.Mana)
            {
                damage += (float) Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health - damage < (float) player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite))
            {
                damage += (float) player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return damage;
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.R)
                {
                    bladeOnCast = args.End;
                }
                if (args.SData.Name == "ShenW")
                {
                    justW = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(1750, () => { justW = false; });
                }
                if (args.SData.Name == "ShenE" && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Q.IsReady() && CheckQDef() && blade.Distance(args.End) > bladeRadius / 2f)
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void InitShen()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W); //2500f
            E = new Spell(SpellSlot.E, 600);
            E.SetSkillshot(0.25f, 95f, 1250f, false, SkillshotType.SkillshotLine);
            EFlash = new Spell(SpellSlot.E, 990);
            EFlash.SetSkillshot(0.25f, 95f, 2500f, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, float.MaxValue);
        }

        private static void InitMenu()
        {
            config = new Menu("Shen", "SRS_Shen", true);
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
                .SetValue(new Circle(false, Color.FromArgb(150, 150, 62, 172)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(150, 150, 62, 172)));
            menuD.AddItem(new MenuItem("draweeflash", "Draw E+flash range", true))
                .SetValue(new Circle(true, Color.FromArgb(50, 250, 248, 110)));
            menuD.AddItem(new MenuItem("drawallyhp", "Draw teammates' HP", true)).SetValue(true);
            menuD.AddItem(new MenuItem("drawincdmg", "Draw incoming damage", true)).SetValue(true);
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);

            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Block AA from target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useeflash", "Flash+E", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("useemin", "   Min target in teamfight", true)).SetValue(new Slider(1, 1, 5));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);

            // Harass Settings
            Menu menuH = new Menu("Harass ", "hsettings");
            menuH.AddItem(new MenuItem("harassq", "Harass with Q", true)).SetValue(true);
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuU = new Menu("Misc ", "usettings");
            menuU.AddItem(new MenuItem("autow", "Auto block high dmg AA", true)).SetValue(true);
            menuU.AddItem(new MenuItem("autowAgg", "W on aggro", true)).SetValue(new Slider(4, 1, 10));
            menuU.AddItem(new MenuItem("autotauntattower", "Auto taunt in tower range", true)).SetValue(true);
            menuU.AddItem(new MenuItem("useeagc", "Use E to anti gap closer", true)).SetValue(false);
            menuU.AddItem(new MenuItem("useeint", "Use E to interrupt", true)).SetValue(true);
            menuU.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuU.AddItem(new MenuItem("atpercent", "   Under % health", true)).SetValue(new Slider(20, 0, 100));
            menuU = DrawHelper.AddMisc(menuU);

            config.AddSubMenu(menuU);
            var sulti = new Menu("Don't ult on ", "dontult");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly))
            {
                if (hero.BaseSkinName != player.BaseSkinName)
                {
                    sulti.AddItem(new MenuItem("ult" + hero.BaseSkinName, hero.BaseSkinName)).SetValue(false);
                }
            }
            config.AddSubMenu(sulti);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}