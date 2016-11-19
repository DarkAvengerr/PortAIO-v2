using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Volibear
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R;
        public static float[] MsBuff = new float[5] { 0.3f, 0.35f, 0.4f, 0.45f, 0.5f };
        private float passivetime = 0f;
        private bool passivecd = false;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public Volibear()
        {
            InitVolibear();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Volibear</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            var hasbuff = player.HasBuff("volibearpassivecd");
            if (hasbuff && !passivecd)
            {
                passivecd = true;
                passivetime = Game.Time;
            }
            if (!hasbuff)
            {
                passivecd = false;
                passivetime = 0f;
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
            var enemyForKs = HeroManager.Enemies.FirstOrDefault(h => W.CanCast(h) && Wdmg(h) > h.Health);
            if (enemyForKs != null && W.IsReady() && config.Item("ksW").GetValue<bool>())
            {
                W.CastOnUnit(enemyForKs);
            }
        }

        private void Harass()
        {
            float perc = config.Item("minmanaH").GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (config.Item("usewH").GetValue<bool>() && W.CanCast(target) && CanW &&
                (config.Item("maxHealthH").GetValue<Slider>().Value / 100f) * target.MaxHealth > target.Health)
            {
                W.Cast(target);
            }
            if (config.Item("useeH").GetValue<bool>() && E.CanCast(target))
            {
                E.Cast(target);
            }
        }

        private void Clear()
        {
            var mob = Jungle.GetNearest(player.Position);
            if (mob != null && config.Item("usewLCSteal").GetValue<bool>() && CanW && W.CanCast(mob) &&
                player.CalcDamage(mob, Damage.DamageType.Physical, Wdmg(mob)) > mob.Health)
            {
                W.Cast(mob);
            }
            if (mob != null && config.Item("usewbsmite").GetValue<bool>() && CanW && W.CanCast(mob) &&
                Jungle.SmiteReady(config.Item("useSmite").GetValue<KeyBind>().Active) &&
                player.CalcDamage(mob, Damage.DamageType.Physical, Wdmg(mob)) + Jungle.smiteDamage(mob) > mob.Health)
            {
                W.Cast(mob);
            }
            float perc = config.Item("minmana").GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            var minions = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (config.Item("useeLC").GetValue<bool>() && E.IsReady() &&
                config.Item("ehitLC").GetValue<Slider>().Value <= minions.Count)
            {
                E.Cast();
            }
        }

        public static float MsBonus(AIHeroClient target)
        {
            float msBonus = 1f;

            if (Q.IsReady() && !QEnabled)
            {
                if (
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(h => h.IsEnemy && player.Distance(h) < 2000 && player.IsFacing(h)) != null)
                {
                    msBonus += MsBuff[Q.Level - 1];
                }
                else
                {
                    msBonus += 0.15f;
                }
            }
            return msBonus;
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(1490, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (config.Item("selected").GetValue<bool>())
            {
                target = CombatHelper.SetTarget(target, TargetSelector.GetSelectedTarget());
                orbwalker.ForceTarget(target);
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            if (config.Item("useq").GetValue<bool>() && Q.IsReady() && !QEnabled &&
                player.Distance(target) >= config.Item("useqmin").GetValue<Slider>().Value &&
                player.Distance(target) < (player.MoveSpeed * MsBonus(target)) * 3.0f)
            {
                Q.Cast();
            }
            if (config.Item("usew").GetValue<bool>() && CanW && W.CanCast(target) &&
                (player.CalcDamage(target, Damage.DamageType.Physical, Wdmg(target)) > target.Health ||
                 player.HealthPercent < 10))
            {
                W.Cast(target);
            }
            if (config.Item("usee").GetValue<bool>() && E.CanCast(target) &&
                ((config.Item("useenotccd").GetValue<bool>() &&
                  (!target.HasBuffOfType(BuffType.Snare) && !target.HasBuffOfType(BuffType.Slow) &&
                   !target.HasBuffOfType(BuffType.Stun) && !target.HasBuffOfType(BuffType.Suppression))) ||
                 !config.Item("useenotccd").GetValue<bool>()))
            {
                E.Cast();
            }
            if (R.IsReady() && player.HealthPercent > 20 &&
                ((config.Item("user").GetValue<bool>() && player.Distance(target) < 200 &&
                  ComboDamage(target) + R.GetDamage(target) * 10 > target.Health && ComboDamage(target) < target.Health) ||
                 (config.Item("usertf").GetValue<Slider>().Value <= player.CountEnemiesInRange(300))))
            {
                R.Cast();
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !W.CanCast(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private static bool QEnabled
        {
            get { return player.Buffs.Any(buff => buff.Name == "VolibearQ"); }
        }

        private static bool CanW
        {
            get { return player.Buffs.Any(buff => buff.Name == "volibearwparticle"); }
        }

        private void Game_OnDraw(EventArgs args)
        {
            float msBonus = 1f;
            if (Q.IsReady() && !QEnabled)
            {
                if (
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(h => h.IsEnemy && player.Distance(h) < 2000 && player.IsFacing(h)) != null)
                {
                    msBonus += MsBuff[Q.Level - 1];
                }
                else
                {
                    msBonus += 0.15f;
                }
            }
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), (player.MoveSpeed * msBonus) * 4.0f);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            if (config.Item("drawpass").GetValue<Circle>().Active && !player.IsDead)
            {
                DrawPassive();
            }
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), 300);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
        }

        private void DrawPassive()
        {
            float baseTime = 0.3f;
            if (player.HasBuff("volibearpassivecd") && passivecd)
            {
                var time = Game.Time - passivetime;
                if (time <= 6f)
                {
                    baseTime = baseTime - time * 0.05f;
                }
                else
                {
                    return;
                }
            }
            var percentHealth = Math.Max(0, player.MaxHealth - player.Health) / player.MaxHealth;
            var barPos = player.HPBarPosition;
            var xPos = barPos.X + 36 + 103 * (1 - percentHealth);
            Drawing.DrawLine(
                xPos, barPos.Y + 9, xPos, barPos.Y + 17, -105f * baseTime,
                config.Item("drawpass").GetValue<Circle>().Color);
        }

        public static double Wdmg(Obj_AI_Base target)
        {
            return ((new double[] { 60, 110, 160, 210, 260 }[W.Level - 1] +
                     ((player.MaxHealth - (498.48f + (86f * (player.Level - 1f)))) * 0.15f)) *
                    ((target.MaxHealth - target.Health) / target.MaxHealth + 1)) * 0.95f;
        }

        private void InitVolibear()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R);
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady() || player.GetSpell(SpellSlot.W).State == SpellState.Surpressed)
            {
                damage += player.CalcDamage(hero, Damage.DamageType.Physical, Wdmg(hero));
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if ((Items.HasItem(ItemHandler.Bft.Id) && Items.CanUseItem(ItemHandler.Bft.Id)) ||
                (Items.HasItem(ItemHandler.Dfg.Id) && Items.CanUseItem(ItemHandler.Dfg.Id)))
            {
                damage = (damage * 1.2);
            }
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite))
            {
                damage += player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            damage += ItemHandler.GetItemsDamage(hero);
            return (float) damage;
        }

        private void InitMenu()
        {
            config = new Menu("Volibear", "Volibear", true);
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
            menuD.AddItem(new MenuItem("drawpass", "Draw passive"))
                .SetValue(new Circle(true, Color.FromArgb(140, 30, 197, 22)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q")).SetValue(true);
            menuC.AddItem(new MenuItem("useqmin", "   Min distance")).SetValue(new Slider(200, 0, 1000));
            menuC.AddItem(new MenuItem("usew", "Use W")).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E")).SetValue(true);
            menuC.AddItem(new MenuItem("useenotccd", "   Wait if the target stunned, slowed...")).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R (1v1)")).SetValue(true);
            menuC.AddItem(new MenuItem("usertf", "Use R min (teamfight)")).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("selected", "Focus Selected target")).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("usewH", "Use W")).SetValue(true);
            menuH.AddItem(new MenuItem("maxHealthH", "Target health less than")).SetValue(new Slider(50, 1, 100));
            menuH.AddItem(new MenuItem("useeH", "Use E")).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana")).SetValue(new Slider(0, 0, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("Clear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("usewLCSteal", "Use W to steal in jungle")).SetValue(true);
            menuLC.AddItem(new MenuItem("usewbsmite", "Use W before smite")).SetValue(true);
            menuLC.AddItem(new MenuItem("useeLC", "Use E")).SetValue(true);
            menuLC.AddItem(new MenuItem("ehitLC", "   More than x minion").SetValue(new Slider(2, 1, 10)));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana")).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            // Misc settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("ksW", "KS with W")).SetValue(false);
            menuM = DrawHelper.AddMisc(menuM);

            config.AddSubMenu(menuM);

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}