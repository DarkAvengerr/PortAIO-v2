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
using SPrediction;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Galio
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public bool justR, justQ, justE;

        public Galio()
        {
            InitGalio();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Galio</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && args.Slot == SpellSlot.R && W.IsReady())
            {
                if (player.Mana > R.Instance.SData.Mana + W.Instance.SData.Mana)
                {
                    W.Cast(player);
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && config.Item("Interrupt", true).GetValue<bool>() && sender.Distance(player) < R.Range)
            {
                CastR();
            }
        }

        private void InitGalio()
        {
            Q = new Spell(SpellSlot.Q, 940);
            Q.SetSkillshot(0.25f, 125, 1300, false, SkillshotType.SkillshotCircle);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 1180);
            E.SetSkillshot(0.25f, 140, 1200, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 575);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (rActive || justR)
            {
                orbwalker.SetAttack(false);
                orbwalker.SetMovement(false);
                return;
            }
            else
            {
                orbwalker.SetAttack(true);
                orbwalker.SetMovement(true);
            }
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            if (config.Item("manualRflash", true).GetValue<KeyBind>().Active)
            {
                FlashCombo();
            }
            if (config.Item("AutoW", true).GetValue<bool>() && W.IsReady())
            {
                CastW(false);
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

        private bool CheckAutoW()
        {
            return config.Item("AutoWmana", true).GetValue<Slider>().Value < player.ManaPercent &&
                   config.Item("AutoWhealth", true).GetValue<Slider>().Value > player.HealthPercent;
        }

        private void FlashCombo()
        {
            if (R.IsReady() && player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerFlash")) == SpellState.Ready)
            {
                var points = CombatHelper.PointsAroundTheTarget(player.Position, 425);
                var best =
                    points.Where(
                        p =>
                            !p.IsWall() && p.Distance(player.Position) > 200 && p.Distance(player.Position) < 425 &&
                            p.IsValid() && p.CountEnemiesInRange(R.Range) > 0 &&
                            config.Item("Rminflash", true).GetValue<Slider>().Value <=
                            p.CountEnemiesInRange(R.Range - 150))
                        .OrderByDescending(p => p.CountEnemiesInRange(R.Range - 100))
                        .FirstOrDefault();
                if (best.CountEnemiesInRange(R.Range - 150) > player.CountEnemiesInRange(R.Range) &&
                    CombatHelper.CheckInterrupt(best, R.Range))
                {
                    player.Spellbook.CastSpell(player.GetSpellSlot("SummonerFlash"), best);
                    LeagueSharp.Common.Utility.DelayAction.Add(50, () => { R.Cast(); });
                    justR = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => justR = false);
                    orbwalker.SetAttack(false);
                    orbwalker.SetMovement(false);
                    return;
                }
            }
            if (!rActive && Orbwalking.CanMove(100))
            {
                if (!justR)
                {
                    Orbwalking.MoveTo(Game.CursorPos, 80f);
                    Combo();
                }
            }
        }

        private void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || target == null)
            {
                return;
            }
            var hitC = HitChance.High;
            if (config.Item("useHigherHit", true).GetValue<bool>())
            {
                hitC = HitChance.VeryHigh;
            }
            if (config.Item("useqH", true).GetValue<bool>() && Q.CanCast(target))
            {
                if (Program.IsSPrediction)
                {
                    Q.SPredictionCast(target, hitC);
                }
                else
                {
                    Q.CastIfHitchanceEquals(target, hitC);
                }
            }
            if (config.Item("useeH", true).GetValue<bool>() && E.CanCast(target))
            {
                if (Program.IsSPrediction)
                {
                    E.SPredictionCast(target, hitC);
                }
                else
                {
                    E.CastIfHitchanceEquals(target, hitC);
                }
            }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady())
            {
                MinionManager.FarmLocation bestPositionQ =
                    Q.GetCircularFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));

                if (bestPositionQ.MinionsHit >= config.Item("qMinHit", true).GetValue<Slider>().Value)
                {
                    Q.Cast(bestPositionQ.Position);
                }
            }
            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady())
            {
                MinionManager.FarmLocation bestPositionE =
                    E.GetLineFarmLocation(
                        MinionManager.GetMinions(
                            ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly));

                if (bestPositionE.MinionsHit >= config.Item("eMinHit", true).GetValue<Slider>().Value)
                {
                    E.Cast(bestPositionE.Position);
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(
                E.Range, TargetSelector.DamageType.Magical, true, HeroManager.Enemies.Where(h => h.IsInvulnerable));
            if (target == null)
            {
                return;
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) && !Q.IsReady() && !justQ && !justE && !rActive)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (config.Item("usew", true).GetValue<bool>() && W.IsReady())
            {
                CastW(true);
            }
            if (rActive || justR)
            {
                return;
            }
            if (R.IsReady() && config.Item("user", true).GetValue<bool>() &&
                config.Item("Rmin", true).GetValue<Slider>().Value <= player.CountEnemiesInRange(R.Range))
            {
                CastR();
                justR = true;
                LeagueSharp.Common.Utility.DelayAction.Add(200, () => justR = false);
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config);
            }
            var hitC = HitChance.High;
            if (config.Item("useHigherHit", true).GetValue<bool>())
            {
                hitC = HitChance.VeryHigh;
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target) &&
                player.Distance(target) < config.Item("useqRange", true).GetValue<Slider>().Value)
            {
                if (Program.IsSPrediction)
                {
                    Q.SPredictionCast(target, hitC);
                }
                else
                {
                    Q.CastIfHitchanceEquals(target, hitC);
                }
            }
            if (config.Item("usee", true).GetValue<bool>() && E.CanCast(target))
            {
                if (Program.IsSPrediction)
                {
                    E.SPredictionCast(target, hitC);
                }
                else
                {
                    E.CastIfHitchanceEquals(target, hitC);
                }
            }
        }

        private void CastW(bool combo)
        {
            foreach (var h in
                HeroManager.Allies.Where(i => i.IsValid && i.Distance(player) < W.Range)
                    .OrderByDescending(i => TargetSelector.GetPriority(i)))
            {
                var incDamage = Program.IncDamages.GetAllyData(h.NetworkId);
                if (incDamage != null &&
                    (incDamage.DamageCount >= config.Item("Wmin", true).GetValue<Slider>().Value ||
                     CheckDamageToW(incDamage)) && (combo || (!combo && CheckAutoW())))
                {
                    W.Cast(incDamage.Hero);
                    return;
                }
            }
        }

        private bool CheckDamageToW(IncData incDamage)
        {
            switch (config.Item("Wdam", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (incDamage.DamageTaken > player.TotalAttackDamage / 2)
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (incDamage.DamageTaken > player.TotalAttackDamage)
                    {
                        return true;
                    }
                    break;
                case 2:
                    if (incDamage.DamageTaken > player.TotalAttackDamage * 2)
                    {
                        return true;
                    }
                    break;
                case 3:
                    return false;
                    break;
            }
            return false;
        }

        private void CastR()
        {
            if (CombatHelper.CheckInterrupt(player.Position, R.Range))
            {
                R.Cast();
            }
        }

        private static bool rActive
        {
            get { return player.Buffs.Any(buff => buff.Name == "GalioIdolOfDurand"); }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R);
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
            if (sender.IsMe && args.SData.Name == "GalioIdolOfDurand" && !justR)
            {
                justR = true;
                LeagueSharp.Common.Utility.DelayAction.Add(200, () => justR = false);
            }
            if (sender.IsMe && args.SData.Name == "GalioResoluteSmite")
            {
                justQ = true;
                LeagueSharp.Common.Utility.DelayAction.Add(getDelay(Q, args.End), () => justQ = false);
            }
            if (sender.IsMe && args.SData.Name == "GalioRighteousGust")
            {
                justE = true;
                LeagueSharp.Common.Utility.DelayAction.Add(getDelay(E, args.End), () => justE = false);
            }
        }

        private int getDelay(Spell spell, Vector3 pos)
        {
            return (int) (spell.Delay * 1000 + player.Distance(pos) / spell.Speed);
        }

        private void InitMenu()
        {
            config = new Menu("Galio ", "Galio", true);
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
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqRange", "   Max range", true))
                .SetValue(new Slider((int) Q.Range, 0, (int) Q.Range));
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(false);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("Rmin", "   R min", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("manualRflash", "Flash R", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("Rminflash", "   R min", true)).SetValue(new Slider(3, 1, 5));
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
            menuLC.AddItem(new MenuItem("qMinHit", "   Q min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("eMinHit", "   E min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("Interrupt", "Cast R to interrupt spells", true)).SetValue(false);
            menuM.AddItem(new MenuItem("useHigherHit", "Higher HitChance(Q-E)", true)).SetValue(true);
            menuM.AddItem(new MenuItem("AutoW", "Auto cast W", true)).SetValue(true);
            menuM.AddItem(new MenuItem("Wmin", "W min hits", true)).SetValue(new Slider(3, 1, 10));
            menuM.AddItem(new MenuItem("Wdam", "W to damage", true))
                .SetValue(new StringList(new[] { "Low", "Mid", "High", "Off" }, 1));
            menuM.AddItem(new MenuItem("AutoWmana", "   Min mana", true)).SetValue(new Slider(50, 1, 100));
            menuM.AddItem(new MenuItem("AutoWhealth", "   Under health", true)).SetValue(new Slider(70, 1, 100));
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}