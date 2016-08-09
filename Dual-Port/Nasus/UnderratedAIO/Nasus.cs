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
    internal class Nasus
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public Nasus()
        {
            InitNocturne();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Nasus</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Orbwalking.OnAttack += Orbwalking_OnAttack;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var tar = target as Obj_AI_Base;
            if (Q.IsReady() && unit.IsMe && tar is AIHeroClient &&
                orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && tar.HasBuffOfType(BuffType.Slow) &&
                target.Health > Q.GetDamage(tar) + player.GetAutoAttackDamage(tar) + 50)
            {
                Q.Cast();
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private void Orbwalking_OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && Q.IsReady() &&
                ((config.Item("autoQ").GetValue<bool>() &&
                  target.Health < Q.GetDamage((Obj_AI_Base) target) + player.GetAutoAttackDamage((Obj_AI_Base) target)) ||
                 (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && target.Health > 1000 &&
                  config.Item("useqLC").GetValue<bool>())))
            {
                Q.Cast();
            }
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var target = args.Target as Obj_AI_Base;
            if (Q.IsReady() && target != null &&
                ((target is AIHeroClient && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                  !target.HasBuffOfType(BuffType.Slow)) ||
                 target.Health < Q.GetDamage(target) + player.GetAutoAttackDamage(target)))
            {
                Q.Cast();
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            AIHeroClient target = TargetSelector.GetTarget(950, TargetSelector.DamageType.Physical);
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(target);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass(target);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    if (Q.IsReady())
                    {
                        useQ();
                    }
                    break;
                default:
                    break;
            }
        }

        private void Combo(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }
            var cmbdmg = ComboDamage(target) + ItemHandler.GetItemsDamage(target);
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, cmbdmg);
            }
            if (!config.Item("Rdamage").GetValue<bool>())
            {
                cmbdmg += R.GetDamage(target) * 15;
            }
            var bonusDmg = Environment.Hero.GetAdOverTime(player, target, 5);
            if ((config.Item("user").GetValue<bool>() && player.Distance(target) < player.AttackRange + 50 &&
                 cmbdmg + bonusDmg > target.Health && target.Health > bonusDmg + 200 && player.HealthPercent < 50) ||
                (config.Item("usertf").GetValue<Slider>().Value <= player.CountEnemiesInRange(600) &&
                 player.HealthPercent < 80))
            {
                R.Cast();
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !E.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (config.Item("usew").GetValue<bool>() && W.CanCast(target))
            {
                if (((config.Item("keepManaForR").GetValue<bool>() && R.IsReady()) || !R.IsReady()) &&
                    player.Mana > R.Instance.SData.Mana + W.Instance.SData.Mana)
                {
                    W.Cast(target);
                }
            }
            if (((config.Item("keepManaForR").GetValue<bool>() && R.IsReady()) || !R.IsReady()) &&
                (player.Mana > R.Instance.SData.Mana + E.Instance.SData.Mana ||
                 (E.IsReady() && E.GetDamage(target) > target.Health)))
            {
                if (config.Item("usee").GetValue<bool>() && E.IsReady() &&
                    ((config.Item("useeslow").GetValue<bool>() && NasusW(target)) ||
                     !config.Item("useeslow").GetValue<bool>()))
                {
                    var ePred = E.GetPrediction(target);
                    if (E.Range > ePred.CastPosition.Distance(player.Position) &&
                        target.Distance(ePred.CastPosition) < 400)
                    {
                        E.Cast(ePred.CastPosition);
                    }
                    else
                    {
                        if (ePred.CastPosition.Distance(player.Position) < 925 &&
                            target.Distance(ePred.CastPosition) < 400)
                        {
                            E.Cast(
                                player.Position.Extend(target.Position, E.Range));
                        }
                    }
                }
            }
        }

        private void Clear()
        {
            if (Q.IsReady())
            {
                useQ();
            }
            if (NasusQ && player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(player)) == 0)
            {
                var minion =
                    MinionManager.GetMinions(
                        Orbwalking.GetRealAutoAttackRange(player), MinionTypes.All, MinionTeam.NotAlly)
                        .FirstOrDefault(m => m.Health > 5 && m.Health < Q.GetDamage(m) + player.GetAutoAttackDamage(m));
                orbwalker.ForceTarget(minion);
            }
            float perc = config.Item("minmana").GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            MinionManager.FarmLocation bestPositionE =
                E.GetCircularFarmLocation(MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly), 400f);
            if (config.Item("useeLC").GetValue<bool>() && E.IsReady() &&
                bestPositionE.MinionsHit >= config.Item("ehitLC").GetValue<Slider>().Value)
            {
                E.Cast(bestPositionE.Position);
            }
        }

        private void useQ()
        {
            var minions =
                MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(player), MinionTypes.All, MinionTeam.NotAlly)
                    .FirstOrDefault(
                        m =>
                            m.Health > 5 &&
                            HealthPrediction.LaneClearHealthPrediction(m, 1000) <
                            Q.GetDamage(m) + player.GetAutoAttackDamage(m));
            if (minions != null)
            {
                Q.Cast();
            }
        }

        private void Harass(AIHeroClient target)
        {
            if (Q.IsReady())
            {
                useQ();
            }
            float perc = config.Item("minmanaH").GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (target == null)
            {
                return;
            }
            if (config.Item("useeH").GetValue<bool>() && E.IsReady())
            {
                var ePred = E.GetPrediction(target);
                if (E.Range > ePred.CastPosition.Distance(player.Position) && target.Distance(ePred.CastPosition) < 400)
                {
                    E.Cast(ePred.CastPosition);
                }
                else
                {
                    if (ePred.CastPosition.Distance(player.Position) < 925 && target.Distance(ePred.CastPosition) < 400)
                    {
                        E.Cast(
                            player.Position.Extend(target.Position, E.Range));
                    }
                }
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (E.IsReady() && E.Instance.SData.Mana < player.Mana)
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady() && config.Item("Rdamage").GetValue<bool>())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R) * 15;
            }
            if (Q.IsReady() && config.Item("Qdamage").GetValue<bool>())
            {
                damage += Q.GetDamage(hero) + player.GetAutoAttackDamage(hero);
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

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
        }

        private void InitNocturne()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 600);
            E.SetSkillshot(
                E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Speed, false,
                SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 350f);
        }

        private static bool NasusW(AIHeroClient target)
        {
            return target.Buffs.Any(buff => buff.Name == "NasusW");
        }

        private static bool NasusQ
        {
            get { return player.Buffs.Any(buff => buff.Name == "NasusQ"); }
        }

        private void InitMenu()
        {
            config = new Menu("Nasus ", "Nasus", true);
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
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q")).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W")).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E")).SetValue(true);
            menuC.AddItem(new MenuItem("keepManaForR", "   Keep mana for R")).SetValue(true);
            menuC.AddItem(new MenuItem("useeslow", "   Only for slowed enemy")).SetValue(false);
            menuC.AddItem(new MenuItem("user", "Use R in 1v1")).SetValue(true);
            menuC.AddItem(new MenuItem("usertf", "R min enemy in teamfight")).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useeH", "Use E")).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana")).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q")).SetValue(true);
            menuLC.AddItem(new MenuItem("useeLC", "Use E")).SetValue(true);
            menuLC.AddItem(new MenuItem("ehitLC", "   Min hit").SetValue(new Slider(4, 1, 10)));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana")).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("autoQ", "Auto Q")).SetValue(true);
            menuM.AddItem(new MenuItem("Rdamage", "Combo damage with R")).SetValue(true);
            menuM.AddItem(new MenuItem("Qdamage", "Combo damage with Q")).SetValue(true);
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}