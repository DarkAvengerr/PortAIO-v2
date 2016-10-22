using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
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
    internal class Singed
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, Qp, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justW, ChaseFix, ToCursor;
        public static float poisonTime;
        public static Vector3 wPos, forcedPos;
        public static Obj_AI_Base cgTarg;

        public Singed()
        {
            InitSinged();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Singed</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnSpellCast += Game_ProcessSpell;
            Helpers.Jungle.setSmiteSlot();
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            AIHeroClient.OnSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        private void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (config.Item("GapCloser", true).GetValue<bool>())
            {
                var spellName = args.SData.Name;
                if (spellName == "TristanaR" || spellName == "BlindMonkRKick" || spellName == "AlZaharNetherGrasp" ||
                    spellName == "GalioIdolOfDurand" || spellName == "VayneCondemn" ||
                    spellName == "JayceThunderingBlow" || spellName == "Headbutt")
                {
                    if (args.Target.IsMe && E.CanCast(sender))
                    {
                        E.CastOnUnit(sender);
                    }
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args) {}


        private void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsEnemy && sender is AIHeroClient && config.Item("OnDash", true).GetValue<bool>() && E.IsReady() &&
                args.StartPos.Distance(player.Position) < E.Range)
            {
                E.CastOnUnit(sender);
            }
            if (config.Item("GapCloser", true).GetValue<bool>() && sender.IsEnemy && sender is AIHeroClient &&
                args.EndPos.Distance(player.Position) < E.Range && !forcedPos.IsValid())
            {
                forcedPos = args.StartPos.To3D();
                cgTarg = sender;
                LeagueSharp.Common.Utility.DelayAction.Add(
                    600, () =>
                    {
                        forcedPos = Vector3.Zero;
                        E.CastOnUnit(
                            HeroManager.Enemies.FirstOrDefault(h => h.NetworkId == sender.NetworkId));
                    });
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && config.Item("Interrupt", true).GetValue<bool>() && sender.Distance(player) < 500)
            {
                if (E.CanCast(sender))
                {
                    E.CastOnUnit(sender);
                }
                else
                {
                    orbwalker.ForceTarget(sender);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, sender);
                }
            }
        }


        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "MegaAdhesive")
                {
                    if (!justW)
                    {
                        wPos = args.End;
                        justW = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            5000, () =>
                            {
                                justW = false;
                                wPos = Vector3.Zero;
                            });
                    }
                }
            }
        }


        private void CastQ()
        {
            if (Q.Instance.ToggleState == 1 && System.Environment.TickCount - poisonTime > 1200)
            {
                poisonTime = System.Environment.TickCount + 1200;
                Q.Cast();
            }
            if (Q.Instance.ToggleState == 2)
            {
                poisonTime = System.Environment.TickCount + 1200;
            }
        }

        private void TurnOffQ()
        {
            if (config.Item("DontOffQ", true).GetValue<bool>())
            {
                return;
            }
            if (Q.Instance.ToggleState == 2 && System.Environment.TickCount - poisonTime > 1200)
            {
                Q.Cast();
            }
        }

        private void InitSinged()
        {
            Q = new Spell(SpellSlot.Q);
            Qp = new Spell(SpellSlot.Q, 200);
            Qp.SetSkillshot(0.5f, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 1000);
            W.SetSkillshot(0.5f, 300, 700, false, SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 125);
            R = new Spell(SpellSlot.R);
        }

        private static bool Qenabled
        {
            get { return player.Buffs.Any(buff => buff.Name == "PoisonTrail"); }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            if (forcedPos.IsValid() && !ToCursor)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, cgTarg.Position.Extend(forcedPos, 50));
            }
            if (forcedPos.IsValid() && ToCursor)
            {
                var pos = cgTarg.Position.Extend(forcedPos, 115);
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos);
                if (player.Distance(pos) < 10)
                {
                    E.CastOnUnit(cgTarg);

                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, cgTarg);
                }
                ToCursor = false;
                forcedPos = Vector3.Zero;
            }
            Orbwalking.Move = true;
            Orbwalking.Attack = true;
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    TurnOffQ();
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    TurnOffQ();
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    TurnOffQ();
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }
            Throw();
            if (config.Item("autoW", true).GetValue<bool>() && W.IsReady() && !player.IsRecalling())
            {
                var targ =
                    HeroManager.Enemies.Where(
                        hero =>
                            W.CanCast(hero) &&
                            (hero.HasBuffOfType(BuffType.Snare) || hero.HasBuffOfType(BuffType.Stun) ||
                             hero.HasBuffOfType(BuffType.Taunt) || hero.HasBuffOfType(BuffType.Suppression)))
                        .OrderBy(hero => hero.Health)
                        .FirstOrDefault();
                if (targ != null)
                {
                    W.Cast(targ);
                }
            }
            if (config.Item("singedFlee", true).GetValue<KeyBind>().Active)
            {
                Orbwalking.MoveTo(Game.CursorPos);
                CastQ();
            }
        }

        private void Throw()
        {
            AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (config.Item("useEkey", true).GetValue<KeyBind>().Active && E.IsReady() &&
                player.Distance(target) < config.Item("targRange", true).GetValue<Slider>().Value && !target.IsMoving)
            {
                var pos =
                    GetEpoints(target)
                        .OrderBy(p => p.Distance(target.Position.Extend(Game.CursorPos, 125)))
                        .FirstOrDefault();
                Orbwalking.Move = false;
                if (!ToCursor)
                {
                    forcedPos = pos;
                    cgTarg = target;
                    ToCursor = true;
                }
            }
        }

        private void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(200, TargetSelector.DamageType.Magical);
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || target == null)
            {
                return;
            }
            if (config.Item("useqH", true).GetValue<bool>() && target.Distance(player) < 200 && Q.IsReady())
            {
                Q.Cast();
            }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            var buff = Jungle.GetNearest(player.Position);
            if (buff != null && config.Item("useeLCsteal", true).GetValue<bool>() && E.IsReady())
            {
                var dmg = new double[] { 50, 65, 80, 95, 110 }[E.Level] + 0.75 * player.FlatMagicDamageMod +
                          Math.Min(300, new double[] { 6, 6.5, 7, 7.5, 8 }[E.Level] / 100 * buff.MaxHealth);
                if (E.CanCast(buff) && Damage.CalcDamage(player, buff, Damage.DamageType.Magical, dmg) > buff.Health)
                {
                    E.CastOnUnit(buff);
                }
            }
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            var pos = Prediction.GetPrediction(player, 1);
            var coll = Qp.GetCollision(
                player.Position.To2D(),
                new List<Vector2>
                {
                    player.Position.Extend(pos.UnitPosition, player.Distance(pos.CastPosition) * -1).To2D()
                });
            if ((coll.Count > 0 || Environment.Minion.countMinionsInrange(player.Position, 250f) > 0) &&
                config.Item("useqLC", true).GetValue<bool>())
            {
                CastQ();
            }
            if (config.Item("moveLC", true).GetValue<bool>() && player.CountEnemiesInRange(1000) < 1 &&
                Orbwalking.CanMove(100) && player.Mana > 30 && !player.Spellbook.IsAutoAttacking)
            {
                var mini =
                    MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(m => !m.HasBuff("poisontrailtarget") && !m.UnderTurret(true))
                        .OrderBy(m => m.Distance(player))
                        .FirstOrDefault();

                if (mini != null && !Environment.Minion.KillableMinion(player.AttackRange))
                {
                    EloBuddy.Player.IssueOrder(
                        GameObjectOrder.MoveTo, player.Position.Extend(mini.Position, player.Distance(mini) + 100));
                    Orbwalking.Attack = false;
                    Orbwalking.Move = false;
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config);
            }
            if (config.Item("RunFOTT", true).GetValue<bool>() &&
                (!config.Item("RunFOTTHP", true).GetValue<bool>() ||
                 (config.Item("RunFOTTHP", true).GetValue<bool>() && player.Health > target.Health)))
            {
                Vector3 pos;
                if (target.IsMoving)
                {
                    var rand = new Random();
                    if (ChaseFix)
                    {
                        pos = target.Position.Extend(
                            Prediction.GetPrediction(target, 0.6f).UnitPosition, rand.Next(480, 550));
                    }
                    else
                    {
                        var positions = CombatHelper.PointsAroundTheTargetOuterRing(target.ServerPosition, 130, 16);
                        pos = positions[rand.Next(positions.Count)];
                    }
                    if (player.Distance(pos) < 90)
                    {
                        if (!ChaseFix)
                        {
                            ChaseFix = true;
                        }
                        else
                        {
                            ChaseFix = false;
                        }
                    }
                }
                else
                {
                    pos = Vector3.Zero;
                }
                if (Orbwalking.CanMove(100))
                {
                    if (player.Distance(pos) > 40 && pos.IsValid())
                    {
                        Orbwalking.Move = false;
                        Orbwalking.Attack = false;
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos);
                    }
                }
            }
            var qTarget =
                HeroManager.Enemies.FirstOrDefault(
                    enemy =>
                        enemy.IsValidTarget() && enemy.Distance(player) < 200 &&
                        CombatHelper.IsFacing(enemy, player.Position, 90f) &&
                        !CombatHelper.IsFacing(player, enemy.Position, 90f) && player.IsMoving && enemy.IsMoving);
            if (config.Item("useq", true).GetValue<bool>() &&
                (qTarget != null || target.HasBuff("poisontrailtarget") || player.Distance(target) <= 500))
            {
                CastQ();
            }
            if (config.Item("usew", true).GetValue<bool>() && !config.Item("WwithE", true).GetValue<bool>() &&
                W.IsReady() && W.CanCast(target))
            {
                var tarPered = W.GetPrediction(target);
                if (W.Range - 80 > tarPered.CastPosition.Distance(player.Position) &&
                    tarPered.Hitchance >= HitChance.VeryHigh)
                {
                    W.Cast(tarPered.CastPosition);
                }
            }
            if (R.IsReady() && config.Item("user", true).GetValue<bool>() &&
                (((config.Item("rUnderHealt", true).GetValue<Slider>().Value > player.HealthPercent &&
                   0 < player.CountEnemiesInRange(750)) ||
                  config.Item("rMinEnemy", true).GetValue<Slider>().Value <= player.CountEnemiesInRange(750)) &&
                 (!config.Item("rkeepManaE", true).GetValue<bool>() ||
                  (config.Item("rkeepManaE", true).GetValue<bool>() &&
                   player.Mana - R.Instance.SData.Mana > E.Instance.SData.Mana))))
            {
                R.Cast();
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() && ignitedmg > target.Health && hasIgnite)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            var blockOrb = false;
            var throwPos = target.Position.Extend(player.Position, 500);
            if (config.Item("usee", true).GetValue<bool>() && E.IsReady() &&
                ((throwPos.CountAlliesInRange(700) > target.CountAlliesInRange(700) &&
                  HeroManager.Allies.FirstOrDefault(a => a.Distance(throwPos) < 700 && a.HealthPercent < 25) == null) ||
                 W.GetDamage(target) > target.Health || !target.HasBuff("poisontrailtarget") ||
                 config.Item("WwithE", true).GetValue<bool>()))
            {
                var pos = Prediction.GetPrediction(target, W.Delay / 2)
                    .UnitPosition.Extend(player.Position, 515 + player.Distance(target.Position));
                if (config.Item("WwithE", true).GetValue<bool>() && E.CanCast(target) && W.IsReady() &&
                    player.Mana > E.Instance.SData.Mana + W.Instance.SData.Mana + 15 && !pos.IsWall() &&
                    target.Health > E.GetDamage(target) + Q.GetDamage(target))
                {
                    W.Cast(pos);
                    return;
                }
                if (E.CanCast(target))
                {
                    E.CastOnUnit(target);
                }
                else if (target.Distance(player) < E.Range + 100)
                {
                    blockOrb = true;
                }
            }
            if (blockOrb)
            {
                Orbwalking.Attack = false;
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
            AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (target != null && config.Item("targCircle", true).GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(
                    target.Position, config.Item("targRange", true).GetValue<Slider>().Value,
                    config.Item("targCircle", true).GetValue<Circle>().Color, 7);
            }
        }

        private IEnumerable<Vector3> GetEpoints(AIHeroClient target)
        {
            return CombatHelper.PointsAroundTheTargetOuterRing(target.ServerPosition, 175, 16);
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (config.Item("drawcomboQ", true).GetValue<bool>())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q) * 5;
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
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
            config = new Menu("Singed ", "Singed", true);
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
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("targCircle", "Target indicator", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 30, 200, 40)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            menuD.AddItem(new MenuItem("drawcomboQ", "Calc Q dmg over 5 sec", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(false);
            menuC.AddItem(new MenuItem("WwithE", "   W+E combo", true)).SetValue(false);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(false);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("rUnderHealt", "   Under health", true)).SetValue(new Slider(60, 0, 100));
            menuC.AddItem(new MenuItem("rMinEnemy", "   Minimum enemy", true)).SetValue(new Slider(2, 1, 6));
            menuC.AddItem(new MenuItem("rkeepManaE", "   Keep mana for E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useEkey", "Throw enemy to cursor", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("targRange", "Target indicator", true)).SetValue(new Slider(300, 20, 600));
            menuC.AddItem(new MenuItem("RunFOTT", "Run front of the target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("RunFOTTHP", "   Only with more health", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("moveLC", "Move if no enemy", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("useeLCsteal", "Steal with E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("autoW", "Auto W on stun", true)).SetValue(true);
            menuM.AddItem(new MenuItem("Interrupt", "Cast E to interrupt spells", true)).SetValue(true);
            menuM.AddItem(new MenuItem("GapCloser", "Throw back gapclosers", true)).SetValue(true);
            menuM.AddItem(new MenuItem("OnDash", "Cast E on escape dash", true)).SetValue(true);
            menuM.AddItem(new MenuItem("singedFlee", "Flee", true))
                .SetValue(new KeyBind("F".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("targRange", "Target indicator", true)).SetValue(new Slider(300, 20, 600));
            menuM.AddItem(new MenuItem("DontOffQ", "Do not turn off Q", true)).SetValue(false);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}