using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using UnderratedAIO.Helpers;
using UnderratedAIO.Helpers.SkillShot;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Tryndamere
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public Team lastWtarget = Team.Null;
        public static bool justE, justR;

        public Tryndamere()
        {
            InitTryndamere();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Tryndamere</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnSpellCast += Game_ProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (config.Item("useegc", true).GetValue<bool>() && Q.IsReady() &&
                gapcloser.End.Distance(player.Position) < 200 && !gapcloser.Sender.ChampionName.ToLower().Contains("yi"))
            {
                E.Cast(gapcloser.End);
            }
        }

        private void InitTryndamere()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 660);
            E.SetSkillshot(0, 165f, 1300f, true, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R);
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
        }

        private void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical, true);
            if (config.Item("useeH", true).GetValue<bool>() && E.CanCast(target) && E.IsReady() && !player.Spellbook.IsAutoAttacking &&
                Orbwalking.CanMove(100))
            {
                CastEtarget(target);
            }
        }

        private void CastEtarget(AIHeroClient target)
        {
            var pred = E.GetPrediction(target);
            var pos = target.Position.Extend(pred.CastPosition, Orbwalking.GetRealAutoAttackRange(target));
            var poly = CombatHelper.GetPoly(pred.CastPosition, player.Distance(pos), E.Width);
            if (pred.Hitchance >= HitChance.High && poly.IsInside(pred.UnitPosition) &&
                poly.IsInside(target.ServerPosition))
            {
                E.Cast(pos);
            }
        }

        private void Clear()
        {
            MinionManager.FarmLocation bestPosition =
                E.GetLineFarmLocation(
                    MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (E.IsReady() && config.Item("useeLC", true).GetValue<bool>() &&
                bestPosition.MinionsHit >= config.Item("eMinHit", true).GetValue<Slider>().Value)
            {
                E.Cast(bestPosition.Position);
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical, true);
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 700) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) &&
                (target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target) + 25 || player.HealthPercent < 30) &&
                !justE)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (Q.CanCast(target) && config.Item("useq", true).GetValue<bool>() &&
                (data.IsAboutToDie || player.HealthPercent < 20 && data.AnyCC) && !R.IsReady() &&
                (!player.HasBuff("UndyingRage") || CombatHelper.GetBuffTime(player.GetBuff("UndyingRage")) < 0.4f) &&
                !justR)
            {
                Q.Cast();
            }

            if (E.IsReady() && config.Item("usee", true).GetValue<bool>() && !player.Spellbook.IsAutoAttacking &&
                Orbwalking.CanMove(100))
            {
                if (!config.Item("useeLimited", true).GetValue<bool>() ||
                    Orbwalking.GetRealAutoAttackRange(target) + 25 < player.Distance(target))
                {
                    CastEtarget(target);
                }
            }
            if (config.Item("usew", true).GetValue<bool>() && W.IsReady() && !Orbwalking.CanAttack() &&
                Orbwalking.CanMove(100))
            {
                W.Cast();
            }
            if (config.Item("user", true).GetValue<bool>() && R.IsReady() && data.IsAboutToDie)
            {
                R.Cast();
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            damage = Environment.Hero.GetAdOverTime(player, hero, 3);
            //damage += ItemHandler.GetItemsDamage(target);
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
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.E)
                {
                    justE = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(500, () => { justE = false; });
                }
                if (args.Slot == SpellSlot.R)
                {
                    justR = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(500, () => { justR = false; });
                }
            }
        }

        private void InitMenu()
        {
            config = new Menu("Tryndamere ", "Tryndamere", true);
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
                .SetValue(new Circle(false, Color.FromArgb(180, 255, 12, 10)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw damage over 3 sec", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings 
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useeLimited", "   Limit usage", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useeH", "Use E", true)).SetValue(false);
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(false);
            menuLC.AddItem(new MenuItem("eMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            config.AddSubMenu(menuLC);

            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("useegc", "Use E gapclosers", true)).SetValue(false);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);

            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}