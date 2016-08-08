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
    internal class Yorick
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static bool hasGhost = false;
        public static int LastAATick;
        public static AutoLeveler autoLeveler;

        public Yorick()
        {
            InitYorick();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Yorick</font>");
            Jungle.setSmiteSlot();
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.BeforeAttack += beforeAttack;
            Drawing.OnDraw += Game_OnDraw;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
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
            if (Yorickghost && R.LSIsReady())
            {
                PetHandler.MovePet(config, orbwalker.ActiveMode);
            }
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var nearestMob = Jungle.GetNearest(player.Position);
            if (unit.IsMe && Q.LSIsReady() &&
                ((orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && config.Item("useq").GetValue<bool>() &&
                  target is AIHeroClient) ||
                 (config.Item("useqLC").GetValue<bool>() && nearestMob != null &&
                  nearestMob.LSDistance(player.Position) < player.AttackRange + 30)))
            {
                Q.Cast();
                Orbwalking.ResetAutoAttackTimer();
                //EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private void beforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && Q.LSIsReady() && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear &&
                config.Item("useqLC").GetValue<bool>() && !(args.Target is AIHeroClient) && (args.Target.Health > 700))
            {
                Q.Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, args.Target);
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            var combodmg = ComboDamage(target);
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, combodmg);
            }
            bool hasIgnite = player.Spellbook.CanUseSpell(player.LSGetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("usew").GetValue<bool>() && W.CanCast(target))
            {
                W.Cast(target.Position);
            }
            if (config.Item("usee").GetValue<bool>() && E.CanCast(target))
            {
                E.CastOnUnit(target);
            }
            if (!Yorickghost && config.Item("user").GetValue<bool>() && R.LSIsReady())
            {
                var ally =
                    HeroManager.Allies.Where(
                        i =>
                            !i.IsDead && player.LSDistance(i) < R.Range &&
                            ((i.Health * 100 / i.MaxHealth) <= config.Item("atpercenty").GetValue<Slider>().Value ||
                             Program.IncDamages.GetAllyData(i.NetworkId).IsAboutToDie) &&
                            !config.Item("ulty" + i.BaseSkinName).GetValue<bool>() && i.LSCountEnemiesInRange(750) > 0)
                        .OrderByDescending(i => Environment.Hero.GetAdOverTime(player, i, 5))
                        .FirstOrDefault();
                if (ally != null && R.IsInRange(ally))
                {
                    R.Cast(ally);
                }
            }
            if (config.Item("useIgnite").GetValue<bool>() && combodmg > target.Health && hasIgnite)
            {
                player.Spellbook.CastSpell(player.LSGetSpellSlot("SummonerDot"), target);
            }
        }

        private static bool Yorickghost
        {
            get { return player.Spellbook.GetSpell(SpellSlot.R).Name == "YorickReviveAllyGuide"; }
        }

        private void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (config.Item("usewH").GetValue<bool>() && W.CanCast(target))
            {
                W.Cast(target);
            }
            if (config.Item("useeH").GetValue<bool>() && E.CanCast(target))
            {
                E.CastOnUnit(target);
            }
        }

        private void Clear()
        {
            float perc = (float) config.Item("minmana").GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            var bestpos =
                W.GetCircularFarmLocation(
                    MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth),
                    100);
            if (config.Item("usewLC").GetValue<bool>() && W.LSIsReady() &&
                config.Item("usewLChit").GetValue<Slider>().Value <= bestpos.MinionsHit)
            {
                W.Cast(bestpos.Position);
            }
            var target =
                MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(i => i.Health < E.GetDamage(i) || i.Health > 600f)
                    .OrderByDescending(i => i.LSDistance(player))
                    .FirstOrDefault();
            if (config.Item("useeLC").GetValue<bool>() && E.CanCast(target))
            {
                E.CastOnUnit(target);
            }
            if (config.Item("useqLC").GetValue<bool>() && Q.LSIsReady())
            {
                var targetQ =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(
                            i =>
                                (i.Health < Damage.LSGetSpellDamage(player, i, SpellSlot.Q) &&
                                 !(i.Health < player.LSGetAutoAttackDamage(i))))
                        .OrderByDescending(i => i.Health)
                        .FirstOrDefault();
                if (targetQ == null)
                {
                    return;
                }
                Q.Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, targetQ);
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawaa").GetValue<Circle>(), player.AttackRange);
            DrawHelper.DrawCircle(config.Item("drawww").GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee").GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrr").GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
        }

        private float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (W.LSIsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.W);
            }
            if ((Items.HasItem(ItemHandler.Bft.Id) && Items.CanUseItem(ItemHandler.Bft.Id)) ||
                (Items.HasItem(ItemHandler.Dfg.Id) && Items.CanUseItem(ItemHandler.Dfg.Id)))
            {
                damage = (float) (damage * 1.2);
            }
            if (Q.LSIsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
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

        private void InitYorick()
        {
            Q = new Spell(SpellSlot.Q, player.AttackRange);
            W = new Spell(SpellSlot.W, 600);
            W.SetSkillshot(
                W.Instance.SData.SpellCastTime, W.Instance.SData.LineWidth, W.Speed, false,
                SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 850);
        }

        private void InitMenu()
        {
            config = new Menu("Yorick", "Yorick", true);
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
            menuD.AddItem(new MenuItem("drawaa", "Draw AA range"))
                .SetValue(new Circle(false, Color.FromArgb(180, 116, 99, 45)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range"))
                .SetValue(new Circle(false, Color.FromArgb(180, 116, 99, 45)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range"))
                .SetValue(new Circle(false, Color.FromArgb(180, 116, 99, 45)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range"))
                .SetValue(new Circle(false, Color.FromArgb(180, 116, 99, 45)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q")).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W")).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E")).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R")).SetValue(true);
            menuC.AddItem(new MenuItem("atpercenty", "Ult friend under")).SetValue(new Slider(30, 0, 100));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("usewH", "Use W")).SetValue(true);
            menuH.AddItem(new MenuItem("useeH", "Use E")).SetValue(true);
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q")).SetValue(true);
            menuLC.AddItem(new MenuItem("usewLC", "Use W")).SetValue(true);
            menuLC.AddItem(new MenuItem("usewLChit", "Min hit")).SetValue(new Slider(3, 1, 8));
            menuLC.AddItem(new MenuItem("useeLC", "Use E")).SetValue(true);
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana")).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            var sulti = new Menu("Don't ult on ", "dontult");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly))
            {
                sulti.AddItem(new MenuItem("ulty" + hero.BaseSkinName, hero.BaseSkinName)).SetValue(false);
            }
            config.AddSubMenu(sulti);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}