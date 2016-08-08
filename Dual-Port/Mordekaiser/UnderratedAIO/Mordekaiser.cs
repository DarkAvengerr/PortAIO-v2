using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Mordekaiser
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static bool justW;
        public static int wWidth = 300;
        public static AutoLeveler autoLeveler;
        public AIHeroClient IgniteTarget;

        public Mordekaiser()
        {
            InitMordekaiser();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Mordekaiser</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.BeforeAttack += BeforeAttack;
            Drawing.OnDraw += Game_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Helpers.Jungle.setSmiteSlot();
        }


        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (hero.IsMe && args.SData.Name == "MordekaiserCreepingDeathCast")
            {
                if (!justW)
                {
                    justW = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () => justW = false);
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
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
            if (MordeGhost && R.LSIsReady())
            {
                PetHandler.MovePet(config, orbwalker.ActiveMode);
            }
        }

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && Q.LSIsReady() && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear &&
                config.Item("useqLC", true).GetValue<bool>() &&
                Environment.Minion.countMinionsInrange(player.Position, 600f) >
                config.Item("qhitLC", true).GetValue<Slider>().Value)
            {
                Q.Cast();
                Orbwalking.ResetAutoAttackTimer();
                //player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var junglemob = Jungle.GetNearest(player.Position);
            if (unit.IsMe && Q.LSIsReady() &&
                ((orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && config.Item("useq", true).GetValue<bool>() &&
                  target.IsEnemy && target.Team != player.Team) ||
                 (config.Item("useqLC", true).GetValue<bool>() && junglemob != null &&
                  junglemob.LSDistance(player.Position) < player.AttackRange + 30)))
            {
                Q.Cast();
                Orbwalking.ResetAutoAttackTimer();
                //player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (config.Item("useItems").GetValue<bool>())
                {
                    ItemHandler.UseItems(target, config, ComboDamage(target));
                }
                bool hasIgnite = player.Spellbook.CanUseSpell(player.LSGetSpellSlot("SummonerDot")) == SpellState.Ready;
                if (config.Item("usew", true).GetValue<bool>() && W.LSIsReady())
                {
                    CastW(target);
                }
                if (config.Item("usee", true).GetValue<bool>() && E.CanCast(target) && player.LSDistance(target) < E.Range)
                {
                    E.CastIfHitchanceEquals(target, HitChance.High);
                }
                var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                var canUlt = config.Item("user", true).GetValue<bool>() && !MordeGhost &&
                             !config.Item("ult" + target.BaseSkinName, true).GetValue<bool>() &&
                             (!config.Item("ultDef", true).GetValue<bool>() ||
                              (config.Item("ultDef", true).GetValue<bool>() && !CombatHelper.HasDef(target)));
                if (canUlt &&
                    (player.LSDistance(target.Position) <= 400f ||
                     (R.CanCast(target) && target.Health < 250f && target.Position.LSCountAlliesInRange(600f) >= 1)) &&
                    R.GetDamage(target) * 0.8f > target.Health)
                {
                    R.CastOnUnit(target);
                }
                if (canUlt && hasIgnite && player.LSDistance(target) < 600 &&
                    R.GetDamage(target) * 0.8f + ignitedmg > HealthPrediction.GetHealthPrediction(target, 400))
                {
                    IgniteTarget = target;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        150, () =>
                        {
                            player.Spellbook.CastSpell(player.LSGetSpellSlot("SummonerDot"), IgniteTarget);
                            IgniteTarget = null;
                        });
                    R.CastOnUnit(target);
                }
                if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite)
                {
                    if (IgniteTarget != null)
                    {
                        player.Spellbook.CastSpell(player.LSGetSpellSlot("SummonerDot"), IgniteTarget);
                        return;
                    }
                    player.Spellbook.CastSpell(player.LSGetSpellSlot("SummonerDot"), target);
                }
            }
        }

        private void CastW(AIHeroClient target)
        {
            if (justW)
            {
                return;
            }
            var allyWs =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(o => o.LSDistance(player) < 1500 && o.HasBuff("mordekaisercreepingdeath"));
            if (allyWs.Any())
            {
                foreach (var allyW in allyWs)
                {
                    var targethero =
                        HeroManager.Enemies.Where(e => e.LSDistance(allyW.Position) < wWidth && e.LSIsValidTarget())
                            .OrderByDescending(e => e.LSDistance(allyW.Position))
                            .FirstOrDefault();
                    if (targethero != null)
                    {
                        var pred =
                            Prediction.GetPrediction(targethero, 0.1f)
                                .UnitPosition.LSDistance(Prediction.GetPrediction(allyW, 0.1f).UnitPosition);
                        if ((pred > wWidth &&
                             (allyW.LSCountEnemiesInRange(wWidth) == 1 || target.NetworkId == targethero.NetworkId)) &&
                            !allyWs.Any(
                                a =>
                                    Prediction.GetPrediction(targethero, 0.1f)
                                        .UnitPosition.LSDistance(Prediction.GetPrediction(a, 0.1f).UnitPosition) < wWidth))
                        {
                            W.Cast();
                        }
                    }
                    if (allyW is AIHeroClient)
                    {
                        var data = Program.IncDamages.GetAllyData(allyW.NetworkId);
                        if (data != null)
                        {
                            if (data.IsAboutToDie)
                            {
                                W.Cast();
                            }
                        }
                    }
                }
            }
            else
            {
                Obj_AI_Base wTarget = Environment.Hero.mostEnemyAtFriend(player, W.Range, wWidth);
                if (MordeGhost && PetHandler.Pet != null)
                {
                    if (wTarget == null || PetHandler.Pet.LSCountEnemiesInRange(250f) > wTarget.LSCountEnemiesInRange(250f))
                    {
                        W.Cast(PetHandler.Pet);
                        return;
                    }
                }
                if (wTarget != null && (wTarget.LSCountEnemiesInRange(250) > 0 || player.LSCountEnemiesInRange(250) > 0))
                {
                    W.Cast(wTarget);
                }
            }
        }

        private static bool MordeGhost
        {
            get { return player.Spellbook.GetSpell(SpellSlot.R).Name.ToLower() == "mordekaisercotgguide"; }
        }

        private void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (config.Item("useeH", true).GetValue<bool>() && E.CanCast(target))
            {
                E.Cast(target);
            }
        }

        private void Clear()
        {
            MinionManager.FarmLocation bestPosition =
                E.GetCircularFarmLocation(
                    MinionManager.GetMinions(E.Range - 100f, MinionTypes.All, MinionTeam.NotAlly), 200f);
            if (config.Item("useeLC", true).GetValue<bool>() && E.LSIsReady() &&
                bestPosition.MinionsHit > config.Item("ehitLC", true).GetValue<Slider>().Value)
            {
                E.Cast(bestPosition.Position);
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawaa", true).GetValue<Circle>(), player.AttackRange);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
        }

        private float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.LSIsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.LSIsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.W);
            }
            if (E.LSIsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.LSIsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.R);
            }

            damage += ItemHandler.GetItemsDamage(hero);

            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.LSGetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private void InitMordekaiser()
        {
            Q = new Spell(SpellSlot.Q, player.AttackRange);
            W = new Spell(SpellSlot.W, 750);
            E = new Spell(SpellSlot.E, 650);
            E.SetSkillshot(0.5f, 45, 1500, false, SkillshotType.SkillshotCone);
            R = new Spell(SpellSlot.R, 850);
        }

        private void InitMenu()
        {
            config = new Menu("Mordekaiser", "Mordekaiser", true);
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
            menuD.AddItem(new MenuItem("drawaa", "Draw AA range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("ultDef", "   Don't use on Qss/barrier etc...", true)).SetValue(true);
            menuC.AddItem(new MenuItem("selected", "Focus Selected target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(true);
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("qhitLC", "   Min hit", true).SetValue(new Slider(2, 1, 3)));
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("ehitLC", "   Min hit", true).SetValue(new Slider(2, 1, 5)));
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            var sulti = new Menu("Don't ult on ", "dontult");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                sulti.AddItem(new MenuItem("ult" + hero.BaseSkinName, hero.BaseSkinName, true)).SetValue(false);
            }
            config.AddSubMenu(sulti);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}