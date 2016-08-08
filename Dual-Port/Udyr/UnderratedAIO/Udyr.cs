using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Multimedia;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;
using Orbwalking = UnderratedAIO.Helpers.Orbwalking;
using EloBuddy;

namespace UnderratedAIO.Champions
{
    internal class Udyr
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, E, R, R2;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justR2;
        public static Stance stance;

        internal enum Stance
        {
            Tiger,
            Turtle,
            Bear,
            Phoenix
        }

        public Udyr()
        {
            InitUdyr();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Udyr</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Helpers.Jungle.setSmiteSlot();
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
        }


        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && stance == Stance.Phoenix && player.GetBuff("UdyrPhoenixStance").Count == 3)
            {
                justR2 = true;
                LeagueSharp.Common.Utility.DelayAction.Add((int) (player.AttackDelay * 1000), () => justR2 = false);
            }
            if (!args.Unit.IsMe || !R.LSIsReady())
            {
                return;
            }
            var target =
                HeroManager.Enemies.FirstOrDefault(
                    h => h.LSDistance(player) < R2.Range && CombatHelper.IsFacing(player, h.Position, 45f));
            if (target != null && orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                Harass();
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var dist = sender.LSDistance(player) < 750;
            if (E.LSIsReady() && config.Item("Interrupt", true).GetValue<bool>() && dist && CanStun(sender))
            {
                E.Cast();
            }
            if (stance == Stance.Bear && dist && CanStun(sender))
            {
                orbwalker.ForceTarget(sender);
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, sender);
            }
        }


        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.SData.Name)
                {
                    case "UdyrTigerStance":
                        stance = Stance.Tiger;
                        break;
                    case "UdyrTurtleStance":
                        stance = Stance.Turtle;
                        break;
                    case "UdyrBearStance":
                        stance = Stance.Bear;
                        break;
                    case "UdyrPhoenixStance":
                        stance = Stance.Phoenix;
                        break;
                }
            }
            if (!(sender is Obj_AI_Base))
            {
                return;
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
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            if (config.Item("usewH", true).GetValue<bool>())
            {
                castR();
            }
        }

        private static bool CanStun(Obj_AI_Base target)
        {
            return !target.HasBuff("UdyrBearStunCheck");
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            var target =
                MinionManager.GetMinions(700, MinionTypes.All, MinionTeam.NotAlly)
                    .FirstOrDefault(m => m.MaxHealth > 1000 && m.Health > 300);
            if (target != null && W.LSIsReady() && player.HealthPercent < 25 &&
                ComboDamage(target) + player.LSGetAutoAttackDamage(target) * 3 < target.Health)
            {
                castW();
                return;
            }
            if (R.LSIsReady() && (stance == Stance.Phoenix && player.GetBuff("UdyrPhoenixStance").Count == 3 || justR2) &&
                (target == null || (target != null && target.Position.LSDistance(player.Position) < 300)))
            {
                return;
            }
            if (target != null && (stance == Stance.Tiger || stance == Stance.Bear))
            {
                orbwalker.ForceTarget(target);
                if (stance == Stance.Bear && CanStun(target))
                {
                    return;
                }
            }
            if (R.LSIsReady() && config.Item("userLC", true).GetValue<bool>() &&
                ((target != null && (player.ManaPercent > 20 || (player.ManaPercent < 20 && stance == Stance.Turtle))) ||
                 config.Item("rMinHit", true).GetValue<Slider>().Value <=
                 Environment.Minion.countMinionsInrange(player.Position, R.Range)))
            {
                castR();
                return;
            }
            bool CanUseW = config.Item("usewLC", true).GetValue<bool>() && W.LSIsReady();
            if (CanUseW &&
                (DangerLevel() >= 2.5 || (DangerLevel() <= 2.5 && player.HealthPercent < 60 && stance == Stance.Tiger)))
            {
                castW();
                return;
            }
            if (target != null && config.Item("useqLC", true).GetValue<bool>() && Q.LSIsReady() && target.Health > 550f &&
                (player.ManaPercent > 50 || player.ManaPercent < 50 && stance == Stance.Turtle))
            {
                castQ();
                return;
            }
            if (CanUseW && DangerLevel() >= 2)
            {
                castW();
                return;
            }
        }

        private double DangerLevel()
        {
            var damageTaken = Program.IncDamages.GetAllyData(player.NetworkId).DamageTaken;
            var shield = getShield();
            if (damageTaken > player.Health)
            {
                return 5;
            }
            if (damageTaken > player.Health * 0.6f)
            {
                return 4;
            }
            if (damageTaken > player.Health * 0.4f)
            {
                return 3.5;
            }
            if (damageTaken > shield)
            {
                return 3;
            }
            if (damageTaken > shield / 1.4f && damageTaken > 60)
            {
                return 2.5;
            }
            if (damageTaken > shield / 2f)
            {
                return 2;
            }
            return 1;
        }

        private void Combo()
        {
            AIHeroClient target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                var t2 =
                    HeroManager.Enemies.Where(
                        e => e.LSDistance(player) > player.MoveSpeed * 2.3 && e.LSDistance(player) < player.MoveSpeed * 2.6)
                        .OrderBy(e => e.LSDistance(player))
                        .FirstOrDefault();
                if (W.LSIsReady() && t2 != null)
                {
                    castW();
                }
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config);
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.LSGetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                (player.HealthPercent < 35 || player.LSDistance(target) > Orbwalking.GetRealAutoAttackRange(target) + 50))
            {
                player.Spellbook.CastSpell(player.LSGetSpellSlot("SummonerDot"), target);
            }
            if (DontChangeStance(target))
            {
                return;
            }
            var isInRange = Orbwalking.GetRealAutoAttackRange(target) > player.LSDistance(target);
            if (W.LSIsReady() && CheckDmg(target) < target.Health && player.HealthPercent < 25)
            {
                castW();
            }
            if (config.Item("useeOthers", true).GetValue<bool>() && !CanStun(target))
            {
                var others =
                    HeroManager.Enemies.Where(
                        e => e.LSDistance(player) < 250f && CanStun(e) && e.NetworkId != target.NetworkId);
                if (others.Any())
                {
                    orbwalker.ForceTarget(others.OrderBy(o => player.LSDistance(o)).FirstOrDefault());
                }
            }
            var dist = player.LSDistance(target);
            var inSpellrange = dist < 300;

            if (config.Item("usew", true).GetValue<bool>() && W.LSIsReady() && DangerLevel() >= 2.5f)
            {
                castW();
            }
            if (config.Item("usee", true).GetValue<bool>() && E.LSIsReady() && CanStun(target) &&
                ((!inSpellrange && player.ManaPercent < 55) || player.ManaPercent > 55) &&
                CombatHelper.IsPossibleToReachHim2(
                    target, new float[5] { 0.15f, 0.2f, 0.25f, 0.3f, 0.35f }[Q.Level - 1],
                    new float[5] { 2f, 2.25f, 2.5f, 2.75f, 3f }[Q.Level - 1]) && !justR2)
            {
                E.Cast();
                return;
            }
            if ((target.Health > R.GetDamage(target) || player.Mana < Q.ManaCost * 2) && isInRange)
            {
                if (config.Item("useq", true).GetValue<bool>() && Q.LSIsReady())
                {
                    castQ();
                    return;
                }
                if (config.Item("user", true).GetValue<bool>() && R.LSIsReady())
                {
                    castR();
                    return;
                }
            }
            else
            {
                if (config.Item("user", true).GetValue<bool>() && R.LSIsReady())
                {
                    castR();
                    return;
                }
                if (config.Item("useq", true).GetValue<bool>() && Q.LSIsReady())
                {
                    castQ();
                    return;
                }
            }
        }

        private static float CheckDmg(Obj_AI_Base target)
        {
            return (float) (ComboDamage(target) + player.LSGetAutoAttackDamage(target) * 3);
        }

        private void castQ()
        {
            if (!player.HasBuff("udyrtigerpunch"))
            {
                Q.Cast();
            }
        }

        private void castW()
        {
            if (!player.HasBuff("udyrturtleactivation"))
            {
                W.Cast();
            }
        }

        private void castR()
        {
            if (!player.HasBuff("udyrphoenixactivation"))
            {
                R.Cast();
            }
        }


        private float getShield()
        {
            if (!W.LSIsReady())
            {
                return 0;
            }
            return new float[5] { 60, 100, 140, 180, 220 }[W.Level - 1] + player.TotalMagicalDamage / 2;
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private static float ComboDamage(Obj_AI_Base hero)
        {
            double damage = 0;
            if (Q.LSIsReady())
            {
                damage += Damage.LSGetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (R.LSIsReady())
            {
                damage += GetRDmagage(hero);
            }
            //damage += ItemHandler.GetItemsDamage(hero);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.LSGetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private static double GetRDmagage(Obj_AI_Base hero)
        {
            return Damage.LSGetSpellDamage(player, hero, SpellSlot.R) * 5 +
                   Damage.CalcDamage(
                       player, hero, Damage.DamageType.Magical,
                       new double[5] { 40, 80, 120, 160, 200 }[R.Level - 1] + 0.45 * (double) player.FlatMagicDamageMod);
        }

        private bool DontChangeStance(Obj_AI_Base target)
        {
            var killable = CheckDmg(target) > target.Health;
            if (DangerLevel() > 2.5)
            {
                return false;
            }
            switch (stance)
            {
                case Stance.Tiger:
                    if (Q.LSIsReady() && target is AIHeroClient && player.ManaPercent < 50 && target.HealthPercent > 40 &&
                        !killable)
                    {
                        return true;
                    }
                    break;
                case Stance.Turtle:
                    if (player.HasBuff("udyrtigerpunch"))
                    {
                        return true;
                    }
                    break;
                case Stance.Bear:
                    if (!E.LSIsReady() && target is AIHeroClient && CanStun(target) &&
                        !Program.IncDamages.GetAllyData(player.NetworkId).AnyCC)
                    {
                        return true;
                    }
                    break;
                case Stance.Phoenix:
                    if (R.LSIsReady() && (player.GetBuff("UdyrPhoenixStance").Count == 3 || justR2) &&
                        (target == null || target.Position.LSDistance(player.Position) < 300))
                    {
                        return true;
                    }
                    if (R.LSIsReady() && player.ManaPercent < 50 && target.HealthPercent > 40 && !killable)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void InitUdyr()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 300);
            R2 = new Spell(SpellSlot.R, 600);
            R2.SetSkillshot(0.3f, 90f * 2 * (float) Math.PI / 180, 1800f, false, SkillshotType.SkillshotCone);
        }

        private void InitMenu()
        {
            config = new Menu("Udyr ", "Udyr", true);
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
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useeOthers", "   Stun nearby enemies too", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings

            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("usewH", "Use R", true)).SetValue(true);
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);

            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("userLC", "Use R", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("rMinHit", "   R min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("Interrupt", "Use E to interupt", true)).SetValue(true);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}