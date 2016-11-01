using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;



using EloBuddy; 
 using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Maokai
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, Qint, W, E, R;
        public static bool turnOff = false;
        public static AutoLeveler autoLeveler;

        public Maokai()
        {
            InitMao();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Maokai</font>");
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
                if (Qint.CanCast(sender))
                {
                    Q.Cast(sender);
                }
            }
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (config.Item("useQgc", true).GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(Qint.Range) && Q.IsReady())
                {
                    Q.Cast(gapcloser.End);
                }
            }
        }

        private static bool maoR
        {
            get { return player.Buffs.Any(buff => buff.Name == "MaokaiDrain3"); }
        }

        private static int maoRStack
        {
            get { return R.Instance.Ammo; }
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
            AutoE();
        }

        private void AutoE()
        {
            if (config.Item("autoe", true).GetValue<bool>() && E.IsReady())
            {
                AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Magical);
                if (E.CanCast(target) &&
                    (target.HasBuff("zhonyasringshield") || target.HasBuffOfType(BuffType.Snare) ||
                     target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Stun) ||
                     target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Fear)))
                {
                    E.Cast(target);
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
            MinionManager.FarmLocation bestPositionE =
                E.GetCircularFarmLocation(MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly));
            MinionManager.FarmLocation bestPositionQ =
                Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly));
            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady() &&
                bestPositionE.MinionsHit > config.Item("ehitLC", true).GetValue<Slider>().Value)
            {
                E.Cast(bestPositionE.Position);
            }
            if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady() &&
                bestPositionQ.MinionsHit > config.Item("qhitLC", true).GetValue<Slider>().Value)
            {
                Q.Cast(bestPositionQ.Position);
            }
        }

        private void Harass()
        {
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (config.Item("useqH", true).GetValue<bool>() && Q.CanCast(target))
            {
                Q.Cast(target);
            }
            if (config.Item("useeH", true).GetValue<bool>() && E.CanCast(target))
            {
                E.Cast(target);
            }
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                if (maoR)
                {
                    if (!turnOff)
                    {
                        turnOff = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(2600, () => turnOffUlt());
                    }
                }
                return;
            }
            if (config.Item("selected", true).GetValue<bool>())
            {
                target = CombatHelper.SetTarget(target, TargetSelector.GetSelectedTarget());
                orbwalker.ForceTarget(target);
            }
            var manaperc = player.Mana / player.MaxMana * 100;
            if (player.HasBuff("MaokaiSapMagicMelee") &&
                player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) + 75)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target) && !player.Spellbook.IsAutoAttacking &&
                ((config.Item("useqroot", true).GetValue<bool>() &&
                  (!target.HasBuffOfType(BuffType.Snare) && !target.HasBuffOfType(BuffType.Stun) &&
                   !target.HasBuffOfType(BuffType.Suppression))) || !config.Item("useqroot", true).GetValue<bool>()) &&
                !W.CanCast(target))
            {
                Q.Cast(target);
            }
            if (config.Item("usew", true).GetValue<bool>())
            {
                if (config.Item("blocke", true).GetValue<bool>() && player.Distance(target) < W.Range && W.IsReady() &&
                    E.CanCast(target))
                {
                    E.Cast(target);
                    CastR(target);
                    LeagueSharp.Common.Utility.DelayAction.Add(100, () => W.Cast(target));
                }
                else if (W.CanCast(target))
                {
                    CastR(target);
                    W.Cast(target);
                }
            }
            if (config.Item("usee", true).GetValue<bool>() && E.CanCast(target))
            {
                if (!config.Item("blocke", true).GetValue<bool>() ||
                    config.Item("blocke", true).GetValue<bool>() && !W.IsReady())
                {
                    E.Cast(target);
                }
            }

            if (R.IsReady())
            {
                bool enoughEnemies = config.Item("user", true).GetValue<Slider>().Value <=
                                     player.CountEnemiesInRange(R.Range - 50);
                AIHeroClient targetR = DrawHelper.GetBetterTarget(R.Range, TargetSelector.DamageType.Magical);

                if (maoR && targetR != null &&
                    ((config.Item("rks", true).GetValue<bool>() &&
                      (Damage.GetSpellDamage(player, targetR, SpellSlot.R) +
                       player.CalcDamage(target, Damage.DamageType.Magical, maoRStack)) > targetR.Health) ||
                     manaperc < config.Item("rmana", true).GetValue<Slider>().Value ||
                     (!enoughEnemies && player.Distance(targetR) > R.Range - 50)))
                {
                    R.Cast();
                }

                if (targetR != null && !maoR && manaperc > config.Item("rmana", true).GetValue<Slider>().Value &&
                    (enoughEnemies || R.IsInRange(targetR)))
                {
                    R.Cast();
                }
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !E.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private void turnOffUlt()
        {
            turnOff = false;
            if (maoR && config.Item("user", true).GetValue<Slider>().Value > player.CountEnemiesInRange(R.Range - 50))
            {
                R.Cast();
            }
        }

        private void CastR(AIHeroClient target)
        {
            if (R.IsReady() && !maoR &&
                player.Mana / player.MaxMana * 100 > config.Item("rmana", true).GetValue<Slider>().Value &&
                config.Item("user", true).GetValue<Slider>().Value <= target.CountEnemiesInRange(R.Range - 50))
            {
                R.Cast();
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            float damage = 0;
            if (Q.IsReady())
            {
                damage += (float) Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += (float) Damage.GetSpellDamage(player, hero, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += (float) Damage.GetSpellDamage(player, hero, SpellSlot.E);
                damage += (float) Damage.GetSpellDamage(player, hero, SpellSlot.E, 1);
            }
            if (R.IsReady())
            {
                damage += (float) Damage.GetSpellDamage(player, hero, SpellSlot.R);
                damage += (float) player.CalcDamage(hero, Damage.DamageType.Magical, maoRStack);
            }
            if ((Items.HasItem(ItemHandler.Bft.Id) && Items.CanUseItem(ItemHandler.Bft.Id)) ||
                (Items.HasItem(ItemHandler.Dfg.Id) && Items.CanUseItem(ItemHandler.Dfg.Id)))
            {
                damage = (float) (damage * 1.2);
            }
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite))
            {
                damage += (float) player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            damage += ItemHandler.GetItemsDamage(hero);
            return damage;
        }

        private void InitMao()
        {
            Q = new Spell(SpellSlot.Q, 600);
            Q.SetSkillshot(0.50f, 110f, 1200f, false, SkillshotType.SkillshotLine);
            Qint = new Spell(SpellSlot.Q, 250f);
            W = new Spell(SpellSlot.W, 500);
            E = new Spell(SpellSlot.E, 1100);
            E.SetSkillshot(1f, 250f, 1500f, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 450);
        }

        private void InitMenu()
        {
            config = new Menu("Maokai", "Maokai", true);
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
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqroot", "   Wait if the target stunned", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useqrange", "   Q max range", true))
                .SetValue(new Slider((int) Q.Range, 0, (int) Q.Range));
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("blocke", "   EW Combo if possible", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R min", true)).SetValue(new Slider(1, 1, 5));
            menuC.AddItem(new MenuItem("rks", "   Deactivate to KS target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("rmana", "   Deactivate min mana", true)).SetValue(new Slider(20, 0, 100));
            menuC.AddItem(new MenuItem("selected", "Focus Selected target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
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
            menuLC.AddItem(new MenuItem("qhitLC", "   More than x minion", true).SetValue(new Slider(2, 1, 10)));
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("ehitLC", "   More than x minion", true).SetValue(new Slider(2, 1, 10)));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("autoe", "Auto E target (Stun/snare...)", true)).SetValue(true);
            menuM.AddItem(new MenuItem("useQgc", "Use Q on gapclosers", true)).SetValue(false);
            menuM.AddItem(new MenuItem("useQint", "Use W to interrupt", true)).SetValue(true);
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}