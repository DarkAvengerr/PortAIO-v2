using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Remoting.Messaging;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using SharpDX;
using SharpDX.Win32;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Kennen
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static AutoLeveler autoLeveler;
        public static Obj_AI_Minion LastAttackedminiMinion;
        public static float LastAttackedminiMinionTime;

        public Kennen()
        {
            InitKennen();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Kennen</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Orbwalking.OnAttack += Orbwalking_OnAttack;
        }

        private void Orbwalking_OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && target is Obj_AI_Minion)
            {
                LastAttackedminiMinion = (Obj_AI_Minion) target;
                LastAttackedminiMinionTime = Utils.GameTimeTickCount;
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            orbwalker.SetMovement(true);
            if (player.HasBuff("KennenLightningRush"))
            {
                orbwalker.SetAttack(false);
            }
            else
            {
                orbwalker.SetAttack(true);
            }
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            AIHeroClient target = getTarget();
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
                    LastHit();
                    break;
                default:
                    break;
            }
            if (target == null)
            {
                return;
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (data != null && player.HasBuff("KennenShurikenStorm") &&
                (config.Item("Minhelath", true).GetValue<Slider>().Value > player.Health / player.MaxHealth * 100 ||
                 (data.IsAboutToDie && config.Item("Minhelath", true).GetValue<Slider>().Value > 0)))
            {
                if (Items.HasItem(ItemHandler.Wooglet.Id) && Items.CanUseItem(ItemHandler.Wooglet.Id))
                {
                    ItemHandler.Wooglet.Cast();
                }
                if (Items.HasItem(ItemHandler.Zhonya.Id) && Items.CanUseItem(ItemHandler.Zhonya.Id))
                {
                    ItemHandler.Zhonya.Cast();
                }
            }
            if (config.Item("autoq", true).GetValue<bool>())
            {
                if (Q.CanCast(target) && !target.IsDashing() &&
                    (MarkOfStorm(target) > 1 || (MarkOfStorm(target) > 0 && player.Distance(target) < W.Range)))
                {
                    Q.Cast(target);
                }
            }
            if (config.Item("autow", true).GetValue<bool>() && W.IsReady() && MarkOfStorm(target) > 1 &&
                !player.HasBuff("KennenShurikenStorm"))
            {
                if (player.Distance(target) < W.Range)
                {
                    W.Cast();
                }
            }
            if (config.Item("KenAutoQ", true).GetValue<KeyBind>().Active && Q.IsReady() &&
                config.Item("KenminmanaaQ", true).GetValue<Slider>().Value < player.ManaPercent &&
                orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && Orbwalking.CanMove(100) &&
                !player.UnderTurret(true))
            {
                if (target != null && Q.CanCast(target) && target.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(
                        target, CombatHelper.GetHitChance(config.Item("qHit", true).GetValue<Slider>().Value));
                }
            }
        }

        private void LastHit()
        {
            if (config.Item("useqLH", true).GetValue<bool>())
            {
                LastHitQ();
            }
            var targetW =
                MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .FirstOrDefault(
                        m =>
                            m.HasBuff("kennenmarkofstorm") && m.Health < W.GetDamage(m, 1) &&
                            player.Distance(m) < W.Range);
            if (config.Item("usewLH", true).GetValue<bool>() && W.IsReady() && targetW != null)
            {
                W.Cast();
            }
        }

        private void Clear()
        {
            var targetW =
                MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(m => m.HasBuff("kennenmarkofstorm"));
            var targetE =
                MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(m => m.Health > 5 && !m.IsDead && !m.HasBuff("kennenmarkofstorm") && !m.UnderTurret(true))
                    .OrderBy(m => player.Distance(m));
            if (config.Item("useeClear", true).GetValue<bool>() && E.IsReady() &&
                ((targetE.FirstOrDefault() != null && player.Position.CountEnemiesInRange(1200f) < 1 &&
                  !player.HasBuff("KennenLightningRush") && targetE.Count() > 1) ||
                 (player.HasBuff("KennenLightningRush") && targetE.FirstOrDefault() == null)))
            {
                E.Cast();
                return;
            }
            if (config.Item("useqClear", true).GetValue<bool>() && Q.IsReady())
            {
                LastHitQ();
            }
            if (W.IsReady() && targetW.Count() >= config.Item("minw", true).GetValue<Slider>().Value &&
                !player.HasBuff("KennenLightningRush"))
            {
                W.Cast();
            }
            var moveTo = targetE.FirstOrDefault();

            if (player.HasBuff("KennenLightningRush"))
            {
                if (moveTo == null)
                {
                    orbwalker.SetMovement(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                else
                {
                    orbwalker.SetMovement(false);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, moveTo);
                }
            }
        }

        private void LastHitQ()
        {
            var targetQ =
                MinionManager.GetMinions(Q.Range)
                    .Where(
                        m =>
                            m.Health > 5 && m.IsEnemy && m.Health < Q.GetDamage(m) && Q.CanCast(m) &&
                            HealthPrediction.GetHealthPrediction(
                                m, (int) ((player.Distance(m) / Q.Speed * 1000) + Q.Delay)) > 0);
            if (targetQ.Any() && LastAttackedminiMinion != null)
            {
                foreach (var target in
                    targetQ.Where(
                        m =>
                            m.NetworkId != LastAttackedminiMinion.NetworkId ||
                            (m.NetworkId == LastAttackedminiMinion.NetworkId &&
                             Utils.GameTimeTickCount - LastAttackedminiMinionTime > 700)))
                {
                    if (target.Distance(player) < Orbwalking.GetRealAutoAttackRange(target) && !Orbwalking.CanAttack() &&
                        Orbwalking.CanMove(100))
                    {
                        if (Q.Cast(target).IsCasted())
                        {
                            //Orbwalking.Orbwalker.AddToBlackList(target.NetworkId);
                        }
                    }
                    else if (target.Distance(player) > Orbwalking.GetRealAutoAttackRange(target))
                    {
                        if (Q.Cast(target).IsCasted())
                        {
                            //Orbwalking.Orbwalker.AddToBlackList(target.NetworkId);
                        }
                    }
                }
            }
        }

        private void Harass()
        {
            if (config.Item("useqLH", true).GetValue<bool>() && Q.IsReady())
            {
                LastHitQ();
            }
            AIHeroClient target = getTarget();
            if (target == null)
            {
                return;
            }
            if (config.Item("useqLC", true).GetValue<bool>() && Q.CanCast(target) && Orbwalking.CanMove(100) &&
                !target.IsDashing())
            {
                Q.Cast(target);
            }
            if (config.Item("usewLC", true).GetValue<bool>() && W.IsReady() && W.Range < player.Distance(target) &&
                target.HasBuff("kennenmarkofstorm"))
            {
                W.Cast();
            }
        }

        private void Combo()
        {
            AIHeroClient target = getTarget();
            if (target == null)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            if (config.Item("usee", true).GetValue<bool>() && player.HasBuff("KennenLightningRush") &&
                player.Health > target.Health && !target.UnderTurret(true) && target.Distance(Game.CursorPos) < 250f)
            {
                orbwalker.SetMovement(false);
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
            }
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var combodamage = ComboDamage(target);
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                !Q.CanCast(target) && !W.IsReady())
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }

            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target) && Orbwalking.CanMove(100) &&
                !target.IsDashing())
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
            if (config.Item("usew", true).GetValue<bool>() && W.IsReady())
            {
                if (player.HasBuff("KennenShurikenStorm"))
                {
                    if (HeroManager.Enemies.Count(e => e.Distance(player) < R.Range && MarkOfStorm(e) > 0) ==
                        player.CountEnemiesInRange(R.Range))
                    {
                        W.Cast();
                    }
                }
                else if (W.Range > player.Distance(target) && MarkOfStorm(target) > 0)
                {
                    W.Cast();
                }
            }
            if (config.Item("usee", true).GetValue<bool>() && !target.UnderTurret(true) && E.IsReady() &&
                (player.Distance(target) < 80 ||
                 (!player.HasBuff("KennenLightningRush") && !Q.CanCast(target) &&
                  config.Item("useemin", true).GetValue<Slider>().Value < player.Health / player.MaxHealth * 100 &&
                  MarkOfStorm(target) > 0 &&
                  CombatHelper.IsPossibleToReachHim(target, 1f, new float[5] { 2f, 2f, 2f, 2f, 2f }[Q.Level - 1]))))
            {
                E.Cast();
            }

            if (R.IsReady() && !player.HasBuffOfType(BuffType.Snare) &&
                (config.Item("user", true).GetValue<Slider>().Value <=
                 player.CountEnemiesInRange(config.Item("userrange", true).GetValue<Slider>().Value) ||
                 (config.Item("usertarget", true).GetValue<bool>() &&
                  player.CountEnemiesInRange(config.Item("userrange", true).GetValue<Slider>().Value) == 1 &&
                  combodamage + player.GetAutoAttackDamage(target) * 3 > target.Health && !Q.CanCast(target) &&
                  player.Distance(target) < config.Item("userrange", true).GetValue<Slider>().Value)) ||
                (config.Item("userLow", true).GetValue<Slider>().Value <=
                 HeroManager.Enemies.Count(
                     e =>
                         e.IsValidTarget(config.Item("userrange", true).GetValue<Slider>().Value) &&
                         e.HealthPercent < 75)))
            {
                R.Cast();
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            DrawHelper.DrawCircle(
                config.Item("drawrrr", true).GetValue<Circle>(), config.Item("userrange", true).GetValue<Slider>().Value);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
            if (config.Item("ShowState", true).GetValue<bool>())
            {
                config.Item("KenAutoQ", true).Permashow(true, "Auto Q");
            }
            else
            {
                config.Item("KenAutoQ", true).Permashow(false, "Auto Q");
            }
        }

        private float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.R) * 2;
            }
            damage += ItemHandler.GetItemsDamage(hero);
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.W, 1);
            }
            if ((Items.HasItem(ItemHandler.Bft.Id) && Items.CanUseItem(ItemHandler.Bft.Id)) ||
                (Items.HasItem(ItemHandler.Dfg.Id) && Items.CanUseItem(ItemHandler.Dfg.Id)))
            {
                damage = (float) (damage * 1.2);
            }
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }

        private int MarkOfStorm(Obj_AI_Base target)
        {
            var buff = target.GetBuff("kennenmarkofstorm");
            if (buff != null)
            {
                return buff.Count;
            }
            return 0;
        }

        private void InitKennen()
        {
            Q = new Spell(SpellSlot.Q, 950);
            Q.SetSkillshot(0.5f, 50, 1700, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 500);
        }

        private AIHeroClient getTarget()
        {
            switch (config.Item("DmgType", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return DrawHelper.GetBetterTarget(Q.Range, TargetSelector.DamageType.Magical);
                    break;
                case 1:
                    return DrawHelper.GetBetterTarget(Q.Range, TargetSelector.DamageType.Physical);
                    break;
                default:
                    return DrawHelper.GetBetterTarget(Q.Range, TargetSelector.DamageType.Magical);
                    break;
            }
        }

        private void InitMenu()
        {
            config = new Menu("Kennen", "Kennen", true);
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
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawrrr", "Draw R activate range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("useemin", "Min healt to E", true)).SetValue(new Slider(50, 0, 100));
            menuC.AddItem(new MenuItem("user", "Use R min", true)).SetValue(new Slider(4, 1, 5));
            menuC.AddItem(new MenuItem("userLow", "Or enemies under 75%", true)).SetValue(new Slider(3, 1, 5));
            menuC.AddItem(new MenuItem("usertarget", "Use R in 1v1", true)).SetValue(true);
            menuC.AddItem(new MenuItem("userrange", "R activate range", true)).SetValue(new Slider(350, 0, 550));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuLC = new Menu("Harass ", "Hcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            config.AddSubMenu(menuLC);
            // Clear Settings
            Menu menuClear = new Menu("Clear ", "Clearsettings");
            menuClear.AddItem(new MenuItem("useqClear", "Use Q", true)).SetValue(true);
            menuClear.AddItem(new MenuItem("minw", "Min to W", true)).SetValue(new Slider(3, 1, 8));
            menuClear.AddItem(new MenuItem("useeClear", "Use E", true)).SetValue(true);
            config.AddSubMenu(menuClear);
            // LastHitQ Settings
            Menu menuLH = new Menu("LastHit ", "Lcsettings");
            menuLH.AddItem(new MenuItem("useqLH", "Use Q", true)).SetValue(true);
            menuLH.AddItem(new MenuItem("usewLH", "Use W", true)).SetValue(true);
            config.AddSubMenu(menuLH);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("Minhelath", "Use Zhonya under x health", true)).SetValue(new Slider(35, 0, 100));
            menuM.AddItem(new MenuItem("autoq", "Auto Q to prepare stun", true)).SetValue(true);
            menuM.AddItem(new MenuItem("autow", "Auto W to stun", true)).SetValue(true);
            menuM.AddItem(new MenuItem("DmgType", "Damage Type", true))
                .SetValue(new StringList(new[] { "AP", "AD" }, 0));

            menuM = DrawHelper.AddMisc(menuM);

            Menu autoQ = new Menu("Auto Harass", "autoQ");
            autoQ.AddItem(
                new MenuItem("KenAutoQ", "Auto Q toggle", true).SetShared()
                    .SetValue(new KeyBind('H', KeyBindType.Toggle)))
                .SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            autoQ.AddItem(new MenuItem("KenminmanaaQ", "Keep X% energy", true)).SetValue(new Slider(40, 1, 100));
            autoQ.AddItem(new MenuItem("qHit", "Q hitChance", true).SetValue(new Slider(4, 1, 4)));
            autoQ.AddItem(new MenuItem("ShowState", "Show always", true)).SetValue(true);
            menuM.AddSubMenu(autoQ);

            config.AddSubMenu(menuM);
            config.Item("KenAutoQ", true).Permashow(true, "Auto Q");

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}