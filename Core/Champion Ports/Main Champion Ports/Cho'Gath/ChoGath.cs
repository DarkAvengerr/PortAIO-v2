using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Chogath
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R, RFlash;
        public static List<int> silence = new List<int>(new int[] { 1500, 1750, 2000, 2250, 2500 });
        public static int knockUp = 1000;
        public static bool flashRblock = false;
        public static AutoLeveler autoLeveler;

        public Chogath()
        {
            InitChoGath();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Cho'Gath</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
            Helpers.Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
        }


        private void OnPossibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (config.Item("useQint", true).GetValue<bool>())
            {
                if (Q.CanCast(sender))
                {
                    Q.Cast(sender);
                }
            }
            if (config.Item("useWint", true).GetValue<bool>())
            {
                if (W.CanCast(sender))
                {
                    W.Cast(sender);
                }
            }
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            /*
            vSpikes = VorpalSpikes;
            if (Environment.Turret.countTurretsInRange(player) > 0 && vSpikes && E.GetHitCount() > 0)
            {
                E.Cast();
            }*/
            if (config.Item("useRJ", true).GetValue<bool>() || config.Item("useSmite").GetValue<KeyBind>().Active)
            {
                Jungle();
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

        private static bool VorpalSpikes
        {
            get { return player.Buffs.Any(buff => buff.Name == "VorpalSpikes"); }
        }

        private static void Jungle()
        {
            var target = Helpers.Jungle.GetNearest(player.Position);
            bool smiteReady = Helpers.Jungle.SmiteReady(config.Item("useSmite").GetValue<KeyBind>().Active);
            if (target != null)
            {
                if (target.CountEnemiesInRange(760f) > 0)
                {
                    bool hasFlash = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) ==
                                    SpellState.Ready;
                    if (config.Item("useRJ", true).GetValue<bool>() && config.Item("useFlashJ", true).GetValue<bool>() &&
                        R.IsReady() && hasFlash && 1000 + player.FlatMagicDamageMod * 0.7f >= target.Health &&
                        player.GetSpell(SpellSlot.R).SData.Mana <= player.Mana && player.Distance(target.Position) > 400 &&
                        player.Distance(target.Position) <= RFlash.Range &&
                        !player.Position.Extend(target.Position, 400).IsWall())
                    {
                        player.Spellbook.CastSpell(
                            player.GetSpellSlot("SummonerFlash"), player.Position.Extend(target.Position, 400));
                        //Utility.DelayAction.Add(50, () => R.Cast(target));
                    }
                }
                if (config.Item("useRJ", true).GetValue<bool>() && R.CanCast(target) &&
                    !(config.Item("priorizeSmite", true).GetValue<bool>() && smiteReady) &&
                    player.GetSpell(SpellSlot.R).SData.Mana <= player.Mana &&
                    1000f + player.FlatMagicDamageMod * 0.7f >= target.Health)
                {
                    R.Cast(target);
                }
                if (Helpers.Jungle.smiteSlot == SpellSlot.Unknown)
                {
                    return;
                }
                if (R.CanCast(target) && config.Item("useSmite").GetValue<KeyBind>().Active &&
                    target.CountEnemiesInRange(750f) > 0 && config.Item("useRSJ", true).GetValue<bool>() && smiteReady &&
                    1000f + player.FlatMagicDamageMod * 0.7f + Helpers.Jungle.smiteDamage(target) >= target.Health)
                {
                    R.Cast(target);
                }
            }
        }

        private static void Clear()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(400)).ToList();
            if (minions.Count() > 2)
            {
                if (Items.HasItem(3077) && Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                }
                if (Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }
            }

            float perc = (float) config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (config.Item("usewLC", true).GetValue<bool>() && W.IsReady() &&
                player.Spellbook.GetSpell(SpellSlot.W).SData.Mana <= player.Mana)
            {
                var minionsForW = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPositionW = W.GetLineFarmLocation(minionsForW);
                if (bestPositionW.Position.IsValid())
                {
                    if (bestPositionW.MinionsHit >= config.Item("whitLC", true).GetValue<Slider>().Value)
                    {
                        W.Cast(bestPositionW.Position);
                    }
                }
            }

            if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady() &&
                player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana <= player.Mana)
            {
                var minionsForQ = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPositionQ = Q.GetCircularFarmLocation(minionsForQ);
                if (Q.IsReady() && bestPositionQ.MinionsHit > config.Item("qhitLC", true).GetValue<Slider>().Value)
                {
                    Q.Cast(bestPositionQ.Position);
                }
            }
        }

        private static void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (config.Item("useqH", true).GetValue<bool>())
            {
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(target);
                }
            }
            if (config.Item("useeH", true).GetValue<bool>())
            {
                if (target.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast(target);
                }
            }
            if (config.Item("useeH", true).GetValue<bool>() && !VorpalSpikes && E.GetHitCount() > 0)
            {
                E.Cast();
            }
        }

        private static void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
            if (config.Item("usee", true).GetValue<bool>() && !VorpalSpikes && E.GetHitCount() > 0 &&
                (Environment.Turret.countTurretsInRange(player) < 1 || target.Health < 150))
            {
                E.Cast();
            }
            if (target == null)
            {
                return;
            }
            if (config.Item("selected", true).GetValue<bool>())
            {
                target = CombatHelper.SetTarget(target, TargetSelector.GetSelectedTarget());
            }
            var combodmg = ComboDamage(target);
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, combodmg);
            }
            bool hasFlash = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) == SpellState.Ready;
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (hasIgnite && ignitedmg > target.Health && !R.CanCast(target) && !W.CanCast(target) && !Q.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (hasIgnite && combodmg > target.Health && R.CanCast(target) &&
                (float) Damage.GetSpellDamage(player, target, SpellSlot.R) < target.Health)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (hasIgnite)
            {
                flashRblock = true;
            }
            else
            {
                flashRblock = false;
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.IsReady())
            {
                int qHit = config.Item("qHit", true).GetValue<Slider>().Value;
                var hitC = HitChance.VeryHigh;
                switch (qHit)
                {
                    case 1:
                        hitC = HitChance.Low;
                        break;
                    case 2:
                        hitC = HitChance.Medium;
                        break;
                    case 3:
                        hitC = HitChance.High;
                        break;
                    case 4:
                        hitC = HitChance.VeryHigh;
                        break;
                }
                if (Program.IsSPrediction)
                {
                    Q.SPredictionCast(target, hitC);
                }
                else
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= hitC)
                    {
                        if (target.IsMoving)
                        {
                            if (pred.CastPosition.Distance(target.ServerPosition) > 250f)
                            {
                                Q.Cast(target.Position.Extend(pred.CastPosition, 250f));
                            }
                            else
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }
                        else
                        {
                            Q.CastIfHitchanceEquals(target, hitC);
                        }
                    }
                }
            }
            if (config.Item("usew", true).GetValue<bool>() && W.CanCast(target))
            {
                if (Program.IsSPrediction)
                {
                    W.SPredictionCast(target, HitChance.High);
                }
                else
                {
                    W.Cast(target);
                }
            }
            if (config.Item("UseFlashC", true).GetValue<bool>() && !flashRblock && R.IsReady() && hasFlash &&
                !CombatHelper.CheckCriticalBuffs(target) && player.GetSpell(SpellSlot.R).SData.Mana <= player.Mana &&
                player.Distance(target.Position) >= 400 && player.GetSpellDamage(target, SpellSlot.R) > target.Health &&
                !Q.IsReady() && !W.IsReady() && player.Distance(target.Position) <= RFlash.Range &&
                !player.Position.Extend(target.Position, 400).IsWall())
            {
                player.Spellbook.CastSpell(
                    player.GetSpellSlot("SummonerFlash"), player.Position.Extend(target.Position, 400));
                LeagueSharp.Common.Utility.DelayAction.Add(50, () => R.Cast(target));
            }
            var rtarget =
                HeroManager.Enemies.Where(e => e.IsValidTarget() && R.CanCast(e))
                    .OrderByDescending(e => TargetSelector.GetPriority(e))
                    .FirstOrDefault();
            if (config.Item("user", true).GetValue<bool>() && rtarget != null &&
                player.GetSpellDamage(target, SpellSlot.R) > rtarget.Health)
            {
                R.Cast(rtarget);
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (config.Item("useQgc", true).GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(gapcloser.End);
                }
            }
            if (config.Item("useWgc", true).GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast(gapcloser.End);
                }
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawaa", true).GetValue<Circle>(), player.AttackRange);
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrrflash", true).GetValue<Circle>(), RFlash.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        public static float ComboDamage(AIHeroClient hero)
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
            if ((Items.HasItem(ItemHandler.Bft.Id) && Items.CanUseItem(ItemHandler.Bft.Id)) ||
                (Items.HasItem(ItemHandler.Dfg.Id) && Items.CanUseItem(ItemHandler.Dfg.Id)))
            {
                damage = (float) (damage * 1.2);
            }
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                damage += player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R);
            }
            damage += ItemHandler.GetItemsDamage(hero);
            return (float) damage;
        }

        private static void InitChoGath()
        {
            Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(1.2f, 175f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W = new Spell(SpellSlot.W, 300);
            W.SetSkillshot(0.25f, 250f, float.MaxValue, false, SkillshotType.SkillshotCone);
            E = new Spell(SpellSlot.E, 500);
            E.SetSkillshot(
                E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Speed, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 175);
            RFlash = new Spell(SpellSlot.R, 555);
        }

        private static void InitMenu()
        {
            config = new Menu("Cho'Gath", "ChoGath", true);
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
                .SetValue(new Circle(false, Color.FromArgb(180, 200, 46, 66)));
            menuD.AddItem(new MenuItem("drawqq", "Draw Q range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 200, 46, 66)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 200, 46, 66)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 200, 46, 66)));
            menuD.AddItem(new MenuItem("drawrrflash", "Draw R+flash range", true))
                .SetValue(new Circle(true, Color.FromArgb(150, 250, 248, 110)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("qHit", "Q hitChance", true).SetValue(new Slider(4, 1, 4)));
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("UseFlashC", "Use flash", true)).SetValue(false);
            menuC.AddItem(new MenuItem("selected", "Focus Selected target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("usewH", "Use W", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(true);
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("qhitLC", "More than x minion", true).SetValue(new Slider(2, 1, 10)));
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("whitLC", "More than x minion", true).SetValue(new Slider(2, 1, 10)));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("useQint", "Use Q to interrupt", true)).SetValue(true);
            menuM.AddItem(new MenuItem("useQgc", "Use Q on gapclosers", true)).SetValue(false);
            menuM.AddItem(new MenuItem("useWint", "Use W to interrupt", true)).SetValue(true);
            menuM.AddItem(new MenuItem("useWgc", "Use W on gapclosers", true)).SetValue(false);
            menuM.AddItem(new MenuItem("useRJ", "Use R in jungle", true)).SetValue(false);
            menuM.AddItem(new MenuItem("useRSJ", "Use R+Smite", true)).SetValue(false);
            menuM.AddItem(new MenuItem("priorizeSmite", "Use smite if possible", true)).SetValue(false);
            menuM.AddItem(new MenuItem("useFlashJ", "Use Flash+R to steal buffs", true)).SetValue(true);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}