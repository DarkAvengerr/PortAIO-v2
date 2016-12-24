using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using SharpDX;
using UnderratedAIO.Helpers;
using Color = System.Drawing.Color;
using Environment = UnderratedAIO.Helpers.Environment;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Sejuani
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        private static readonly AIHeroClient me = ObjectManager.Player;
        public static Spell Q, W, E, R;
        

        public Sejuani()
        {
            InitSejuani();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Sejuani</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
            Orbwalking.AfterAttack += AfterAttack;
            Jungle.setSmiteSlot();
            
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && W.IsReady() &&
                config.Item("usew", true).GetValue<bool>())
            {
                W.Cast();
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (config.Item("useqint", true).GetValue<bool>())
            {
                if (unit.IsValidTarget(Q.Range) && Q.IsReady() && me.Distance(unit) < Q.Range)
                {
                    Q.Cast(unit.Position);
                }
            }
            if (config.Item("userint", true).GetValue<bool>())
            {
                if (unit.IsValidTarget(R.Range) && R.IsReady() && me.Distance(unit) < R.Range)
                {
                    R.Cast(unit.Position);
                }
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);

            
        }

        private static void Game_OnGameUpdate(EventArgs args)
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
                    //if (!minionBlock) Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }

            if (config.Item("manualR", true).GetValue<KeyBind>().Active && R.IsReady())
            {
                CastR();
            }
        }

        private static void CastR()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            foreach (var enemy in
                HeroManager.Enemies.Where(i => !i.IsDead && me.Distance(i) < R.Range)
                    .OrderByDescending(l => l.CountEnemiesInRange(350f)))
            {
                R.CastIfHitchanceEquals(enemy, HitChance.High);
                break;
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (config.Item("useqgc", true).GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(Q.Range) && Q.IsReady() && me.Distance(gapcloser.End) < Q.Range)
                {
                    Q.Cast(gapcloser.End);
                }
            }
            if (config.Item("usergc", true).GetValue<bool>())
            {
                if (gapcloser.Sender.IsValidTarget(R.Range) && R.IsReady() && me.Distance(gapcloser.End) < R.Range)
                {
                    R.Cast(gapcloser.End);
                }
            }
        }

        private static void Clear()
        {
            var minions =
                MinionManager.GetMinions(400, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(m => m.IsValidTarget(400))
                    .ToList();
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
            if (me.Mana < me.MaxMana * perc)
            {
                return;
            }

            Q.SetSkillshot(
                Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, false,
                SkillshotType.SkillshotLine);
            var minionsSpells = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (W.IsReady() && minionsSpells.Count() > 1 && config.Item("usewC", true).GetValue<bool>() &&
                me.Spellbook.GetSpell(SpellSlot.W).SData.Mana <= me.Mana)
            {
                W.Cast();
            }
            var minHit = config.Item("useeCmin", true).GetValue<Slider>().Value;
            if (E.IsReady() && me.Spellbook.GetSpell(SpellSlot.Q).SData.Mana <= me.Mana &&
                CombatHelper.SejuaniCountFrostMinion(E.Range) >= minHit &&
                (!(!Q.IsReady() && me.Mana - me.Spellbook.GetSpell(SpellSlot.Q).SData.Mana < me.MaxMana * perc) ||
                 !(!W.IsReady() && me.Mana - me.Spellbook.GetSpell(SpellSlot.W).SData.Mana < me.MaxMana * perc)))
            {
                E.Cast();
            }
            if (config.Item("useqC", true).GetValue<bool>() && Q.IsReady() &&
                me.Spellbook.GetSpell(SpellSlot.Q).SData.Mana <= me.Mana)
            {
                var minionsForQ = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPosition = Q.GetLineFarmLocation(minionsForQ);
                if (bestPosition.Position.IsValid())
                {
                    if (bestPosition.MinionsHit >= 2)
                    {
                        Q.Cast(bestPosition.Position);
                    }
                }
            }

            Q.SetSkillshot(
                Q.Instance.SData.SpellCastTime, Q.Instance.SData.LineWidth, Q.Instance.SData.MissileSpeed, true,
                SkillshotType.SkillshotLine);
        }

        private static void Ulti()
        {
            if (!R.IsReady() || config.Item("useRmin", true).GetValue<Slider>().Value == 0)
            {
                return;
            }
            var target = DrawHelper.GetBetterTarget(R.Range, TargetSelector.DamageType.Magical);
            if (config.Item("useRmin", true).GetValue<Slider>().Value == 1)
            {
                if (target != null && !config.Item("ult" + target.BaseSkinName, true).GetValue<bool>())
                {
                    R.Cast(target);
                }
            }
            else
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        i =>
                            i.IsValidTarget() && me.Distance(i) < R.Range &&
                            me.Distance(i) >= config.Item("useRminr", true).GetValue<Slider>().Value &&
                            !config.Item("ult" + i.BaseSkinName, true).GetValue<bool>() &&
                            i.Position.CountEnemiesInRange(350f) >=
                            config.Item("useRmin", true).GetValue<Slider>().Value && target.Distance(i.Position) < 350f)
                        .OrderByDescending(l => l.Position.CountEnemiesInRange(350f)))
                {
                    R.Cast(enemy);
                    return;
                }
            }
        }

        private static void Combo()
        {
            Ulti();
            float perc = (float) config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            var minHit = config.Item("useemin", true).GetValue<Slider>().Value;
            AIHeroClient target = DrawHelper.GetBetterTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }

            var buffs = CombatHelper.SejuaniCountFrostHero(E.Range);
            if (E.IsReady() && me.Distance(target.Position) < E.Range && buffs > 0 &&
                ((buffs > minHit) || (Damage.GetSpellDamage(me, target, SpellSlot.E) >= target.Health) ||
                 (me.Distance(target) > config.Item("useEminr", true).GetValue<Slider>().Value &&
                  me.Distance(target) < E.Range && buffs == 1)))
            {
                if (!(Q.IsReady() && me.Mana - me.Spellbook.GetSpell(SpellSlot.Q).SData.Mana < me.MaxMana * perc) ||
                    !(W.IsReady() && me.Mana - me.Spellbook.GetSpell(SpellSlot.W).SData.Mana < me.MaxMana * perc))
                {
                    E.Cast();
                }
            }
            if (Q.IsReady() && config.Item("useq", true).GetValue<bool>() &&
                me.Distance(target.Position) > config.Item("useQminr", true).GetValue<Slider>().Value)
            {
                var hits = Q.GetHitCount(HitChance.High);
                if (target.CountEnemiesInRange(Q.Width) >= hits)
                {
                    if (Program.IsSPrediction)
                    {
                        Q.SPredictionCast(target, HitChance.High);
                    }
                    else
                    {
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
            }
            bool hasIgnite = me.Spellbook.CanUseSpell(me.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var ignitedmg = (float) me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (ignitedmg > target.Health && hasIgnite && !E.CanCast(target) && !W.CanCast(target) && !Q.CanCast(target))
            {
                me.Spellbook.CastSpell(me.GetSpellSlot("SummonerDot"), target);
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            float damage = 0;
            if (Q.IsReady())
            {
                damage += (float) Damage.GetSpellDamage(me, hero, SpellSlot.Q);
            }
            if (E.IsReady() || E.Instance.State == SpellState.Surpressed)
            {
                damage += (float) Damage.GetSpellDamage(me, hero, SpellSlot.E);
            }
            if (W.IsReady())
            {
                double watk = (new double[] { 4, 4.5, 5, 5.5, 6 }[W.Level - 1] + hero.FlatMagicDamageMod * 0.03) / 100 *
                              hero.Health;
                double wdot = new double[] { 40, 70, 100, 130, 160 }[W.Level - 1] +
                              (new double[] { 4, 6, 8, 10, 12 }[W.Level - 1] / 100) * me.MaxHealth;
                damage += (float) me.CalcDamage(hero, Damage.DamageType.Magical, wdot);
                damage += (float) me.CalcDamage(hero, Damage.DamageType.Magical, watk);
            }
            if (R.IsReady())
            {
                damage += (float) Damage.GetSpellDamage(me, hero, SpellSlot.R);
            }

            if ((Items.HasItem(ItemHandler.Bft.Id) && Items.CanUseItem(ItemHandler.Bft.Id)) ||
                (Items.HasItem(ItemHandler.Dfg.Id) && Items.CanUseItem(ItemHandler.Dfg.Id)))
            {
                damage = (float) (damage * 1.2);
            }
            if (me.Spellbook.CanUseSpell(me.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + me.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite))
            {
                damage += (float) me.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            damage += ItemHandler.GetItemsDamage(hero);
            return damage;
        }

        private static void InitSejuani()
        {
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 1175);
            Q.SetSkillshot(0, 70, 1600, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 110, 1600, true, SkillshotType.SkillshotLine);
        }

        private static void InitMenu()
        {
            config = new Menu("Sejuani", "Sejuani", true);
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
                .SetValue(new Circle(false, Color.FromArgb(150, 150, 177, 208)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(150, 150, 177, 208)));
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(150, 150, 177, 208)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(150, 150, 177, 208)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);

            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useQminr", "   Minimum range", true)).SetValue(new Slider(250, 0, 650));
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useemin", "Use E min", true)).SetValue(new Slider(1, 1, 5));
            menuC.AddItem(new MenuItem("useEminr", "   Minimum range", true)).SetValue(new Slider(250, 0, 900));
            menuC.AddItem(new MenuItem("useRmin", "R minimum target", true)).SetValue(new Slider(1, 1, 5));
            menuC.AddItem(new MenuItem("useRminr", "   Minimum range", true)).SetValue(new Slider(0, 0, 350));
            menuC.AddItem(new MenuItem("manualR", "Cast R asap", true))
                .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);

            config.AddSubMenu(menuC);
            // Clear/Jungle
            Menu menuJ = new Menu("Clear ", "jsettings");
            menuJ.AddItem(new MenuItem("useqC", "Use Q", true)).SetValue(true);
            menuJ.AddItem(new MenuItem("usewC", "Use W", true)).SetValue(true);
            menuJ.AddItem(new MenuItem("useeCmin", "Use E min", true)).SetValue(new Slider(1, 1, 5));
            menuJ.AddItem(new MenuItem("useiC", "Use Items")).SetValue(true);
            menuJ.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuJ);
            // Misc Settings
            Menu menuU = new Menu("Misc ", "usettings");
            menuU.AddItem(new MenuItem("useqgc", "Use Q to anti gap closer", true)).SetValue(false);
            menuU.AddItem(new MenuItem("useqint", "Use Q to interrupt", true)).SetValue(true);
            menuU.AddItem(new MenuItem("usergc", "Use R to anti gap closer", true)).SetValue(false);
            menuU.AddItem(new MenuItem("userint", "Use R to interrupt", true)).SetValue(false);
            menuU = DrawHelper.AddMisc(menuU);

            config.AddSubMenu(menuU);
            var sulti = new Menu("Don't ult on ", "dontult");
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                sulti.AddItem(new MenuItem("ult" + hero.BaseSkinName, hero.BaseSkinName, true)).SetValue(false);
            }
            config.AddSubMenu(sulti);

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddSubMenu(Program.SPredictionMenu);
            config.AddToMainMenu();
        }
    }
}