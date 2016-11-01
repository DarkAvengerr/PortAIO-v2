using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Remoting.Messaging;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Renekton
    {
        public static Menu config;
        private static Orbwalking.Orbwalker orbwalker;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        private static float lastE;
        private static Vector3 lastEpos;
        private static Bool wChancel = false;
        public static AutoLeveler autoLeveler;

        public Renekton()
        {
            InitRenekton();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Renekton</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.BeforeAttack += beforeAttack;
            Orbwalking.AfterAttack += afterAttack;
            Drawing.OnDraw += Game_OnDraw;
            Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
        }


        private void Game_OnGameUpdate(EventArgs args)
        {
            if (System.Environment.TickCount - lastE > 4100)
            {
                lastE = 0;
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

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && target is AIHeroClient &&
                ((orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                  checkFuryMode(SpellSlot.W, (Obj_AI_Base) target)) ||
                 orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed))
            {
                var time = Game.Time - W.Instance.CooldownExpires;
                if (config.Item("hyd").GetValue<bool>() &&
                    (W.Instance.Cooldown - Math.Abs(time) < 1 || time < -6 || player.HealthPercent < 50))
                {
                    ItemHandler.castHydra((AIHeroClient) target);
                }
            }
            if (unit.IsMe && target is AIHeroClient && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                config.Item("usew", true).GetValue<bool>() && checkFuryMode(SpellSlot.W, (Obj_AI_Base) target))
            {
                W.Cast();
                Orbwalking.ResetAutoAttackTimer();
                return;
            }
            if (unit.IsMe && target is AIHeroClient && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                config.Item("useCH", true).GetValue<StringList>().SelectedIndex == 0)
            {
                if (W.IsReady())
                {
                    W.Cast();
                    Orbwalking.ResetAutoAttackTimer();
                    return;
                }
                if (Q.IsReady())
                {
                    Q.Cast();
                    return;
                }
                if (E.CanCast((Obj_AI_Base) target))
                {
                    E.Cast(target.Position);
                    return;
                }
            }
        }

        private void beforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && W.IsReady() && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                args.Target is AIHeroClient && checkFuryMode(SpellSlot.W, (Obj_AI_Base) args.Target) &&
                config.Item("usew", true).GetValue<bool>())
            {
                if ((player.Mana > 40 && !fury) || (Q.IsReady() && canBeOpWIthQ(player.Position)))
                {
                    return;
                }

                W.Cast();
                return;
            }
            if (args.Unit.IsMe && W.IsReady() && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                config.Item("usewH", true).GetValue<bool>() && args.Target is AIHeroClient &&
                config.Item("useCH", true).GetValue<StringList>().SelectedIndex != 0)
            {
                W.Cast();
            }
        }

        private static bool rene
        {
            get { return player.Buffs.Any(buff => buff.Name == "renektonsliceanddicedelay"); }
        }

        private static bool fury
        {
            get { return player.Buffs.Any(buff => buff.Name == "renektonrageready"); }
        }

        private static bool renw
        {
            get { return player.Buffs.Any(buff => buff.Name == "renektonpreexecute"); }
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range * 2, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var FuryQ = Damage.GetSpellDamage(player, target, SpellSlot.Q) * 0.5;
            var FuryW = Damage.GetSpellDamage(player, target, SpellSlot.W) * 0.5;
            var eDmg = Damage.GetSpellDamage(player, target, SpellSlot.E);
            var combodamage = ComboDamage(target);
            if (config.Item("useIgnite").GetValue<bool>() && hasIgnite &&
                player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (player.Distance(target) > E.Range && E.IsReady() && (W.IsReady() || Q.IsReady()) && lastE.Equals(0) &&
                config.Item("usee", true).GetValue<bool>())
            {
                var closeGapTarget =
                    MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(i => i.Distance(target.ServerPosition) < Q.Range - 40)
                        .OrderByDescending(i => Environment.Minion.countMinionsInrange(i.Position, Q.Range))
                        .FirstOrDefault();
                if (closeGapTarget != null)
                {
                    if ((canBeOpWIthQ(closeGapTarget.Position) || fury) && !rene)
                    {
                        if (E.CanCast(closeGapTarget))
                        {
                            E.Cast(closeGapTarget.Position);
                            lastE = System.Environment.TickCount;
                            return;
                        }
                    }
                }
            }
            if (config.Item("useq", true).GetValue<bool>() && Q.CanCast(target) && !renw && !player.IsDashing() &&
                checkFuryMode(SpellSlot.Q, target))
            {
                Q.Cast();
            }
            var distance = player.Distance(target.Position);
            if (config.Item("usee", true).GetValue<bool>() && lastE.Equals(0) && E.CanCast(target) &&
                (eDmg > target.Health ||
                 (((W.IsReady() && canBeOpWIthQ(target.Position) && !rene) ||
                   (distance > target.Distance(player.Position.Extend(target.Position, E.Range)) - distance)))))
            {
                E.Cast(target.Position);
                lastE = System.Environment.TickCount;
                return;
            }
            if (config.Item("usee", true).GetValue<bool>() && checkFuryMode(SpellSlot.E, target) && !lastE.Equals(0) &&
                (eDmg + player.GetAutoAttackDamage(target) > target.Health ||
                 (((W.IsReady() && canBeOpWIthQ(target.Position) && !rene) ||
                   (distance < target.Distance(player.Position.Extend(target.Position, E.Range)) - distance) ||
                   player.Distance(target) > E.Range - 100))))
            {
                var time = System.Environment.TickCount - lastE;
                if (time > 3600f || combodamage > target.Health || (player.Distance(target) > E.Range - 100))
                {
                    E.Cast(target.Position);
                    lastE = 0;
                }
            }
            var data = Program.IncDamages.GetAllyData(player.NetworkId);
            if (((player.Health * 100 / player.MaxHealth) <= config.Item("user", true).GetValue<Slider>().Value &&
                 data.DamageTaken > 30) ||
                config.Item("userindanger", true).GetValue<Slider>().Value <= player.CountEnemiesInRange(R.Range))
            {
                R.Cast();
            }
        }

        private bool canBeOpWIthQ(Vector3 vector3)
        {
            if (fury)
            {
                return false;
            }
            if ((player.Mana > 45 && !fury) ||
                (Q.IsReady() &&
                 player.Mana + Environment.Minion.countMinionsInrange(vector3, Q.Range) * 2.5 +
                 player.CountEnemiesInRange(Q.Range) * 10 > 50))
            {
                return true;
            }
            return false;
        }

        private bool canBeOpwithW()
        {
            if (player.Mana + 20 > 50)
            {
                return true;
            }
            return false;
        }

        private void Harass()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            switch (config.Item("useCH", true).GetValue<StringList>().SelectedIndex)
            {
                case 1:
                    if (Q.IsReady() && E.IsReady() && lastE.Equals(0) && fury && !rene)
                    {
                        if (config.Item("donteqwebtower", true).GetValue<bool>() &&
                            player.Position.Extend(target.Position, E.Range).UnderTurret(true))
                        {
                            return;
                        }
                        var closeGapTarget =
                            MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly)
                                .Where(i => i.Distance(target.ServerPosition) < Q.Range - 40)
                                .OrderByDescending(i => Environment.Minion.countMinionsInrange(i.Position, Q.Range))
                                .FirstOrDefault();
                        if (closeGapTarget != null)
                        {
                            lastEpos = player.ServerPosition;
                            LeagueSharp.Common.Utility.DelayAction.Add(4100, () => lastEpos = new Vector3());
                            E.Cast(closeGapTarget.Position);
                            lastE = System.Environment.TickCount;
                            return;
                        }
                        else
                        {
                            lastEpos = player.ServerPosition;
                            LeagueSharp.Common.Utility.DelayAction.Add(4100, () => lastEpos = new Vector3());
                            E.Cast(target.Position);
                            lastE = System.Environment.TickCount;
                            return;
                        }
                    }
                    if (player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) && Q.IsReady() &&
                        E.IsReady() && E.IsReady())
                    {
                        orbwalker.ForceTarget(target);
                    }
                    return;
                    break;
                case 0:
                    if (Q.IsReady() && W.IsReady() && !rene && E.CanCast(target))
                    {
                        if (config.Item("donteqwebtower", true).GetValue<bool>() &&
                            player.Position.Extend(target.Position, E.Range).UnderTurret(true))
                        {
                            return;
                        }
                        if (E.CastIfHitchanceEquals(target, HitChance.High))
                        {
                            lastE = System.Environment.TickCount;
                        }
                    }
                    if (rene && E.CanCast(target) && !lastE.Equals(0) && System.Environment.TickCount - lastE > 3600)
                    {
                        E.CastIfHitchanceEquals(target, HitChance.High);
                    }
                    if (player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) && Q.IsReady() &&
                        E.IsReady() && E.IsReady())
                    {
                        orbwalker.ForceTarget(target);
                    }
                    return;
                    break;
                default:
                    break;
            }

            if (config.Item("useqH", true).GetValue<bool>() && Q.CanCast(target))
            {
                Q.Cast();
            }

            if (config.Item("useCH", true).GetValue<StringList>().SelectedIndex == 0 && !lastE.Equals(0) && rene &&
                !Q.IsReady() && !renw)
            {
                if (lastEpos.IsValid())
                {
                    E.Cast(player.Position.Extend(lastEpos, 350f));
                }
            }
        }

        private void Clear()
        {
            if (player.Spellbook.IsAutoAttacking)
            {
                return;
            }
            if (config.Item("useqLC", true).GetValue<bool>() && Q.IsReady() && !player.IsDashing())
            {
                var minis = MinionManager.GetMinions(player.AttackRange + 50, MinionTypes.All, MinionTeam.NotAlly);
                if (Environment.Minion.countMinionsInrange(player.Position, Q.Range) >=
                    config.Item("minimumMini", true).GetValue<Slider>().Value &&
                    minis.Count(m => m.Health - Q.GetDamage(m) < 50 && m.Health - Q.GetDamage(m) > 0) == 0 &&
                    (!Environment.Minion.KillableMinion(player.AttackRange) || !Orbwalking.CanAttack()))
                {
                    Q.Cast();
                    return;
                }
            }
            if (config.Item("useeLC", true).GetValue<bool>() && E.IsReady())
            {
                var minionsForE = MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                MinionManager.FarmLocation bestPosition = E.GetLineFarmLocation(minionsForE);
                if (bestPosition.Position.IsValid() &&
                    !player.Position.Extend(bestPosition.Position.To3D(), E.Range).UnderTurret(true) &&
                    !bestPosition.Position.IsWall())
                {
                    if (bestPosition.MinionsHit >= 2)
                    {
                        E.Cast(bestPosition.Position);
                    }
                }
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawee", true).GetValue<Circle>(), E.Range);
            DrawHelper.DrawCircle(config.Item("drawrr", true).GetValue<Circle>(), R.Range);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo").GetValue<bool>();
        }

        private float ComboDamage(AIHeroClient hero)
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
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            if (R.IsReady())
            {
                if (config.Item("rDamage", true).GetValue<bool>())
                {
                    damage += Damage.GetSpellDamage(player, hero, SpellSlot.R) * 15;
                }
            }

            damage += ItemHandler.GetItemsDamage(hero);

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

        public bool checkFuryMode(SpellSlot spellSlot, Obj_AI_Base target)
        {
            if (Damage.GetSpellDamage(player, target, spellSlot) > target.Health)
            {
                return true;
            }
            if (canBeOpWIthQ(player.Position) && spellSlot != SpellSlot.Q)
            {
                return false;
            }
            if (!fury)
            {
                return true;
            }
            if (player.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            switch (config.Item("furyMode", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return true;
                    break;
                case 1:
                    if (spellSlot != SpellSlot.Q && Q.IsReady(500))
                    {
                        return false;
                    }
                    break;
                case 2:
                    if (spellSlot != SpellSlot.W && (W.IsReady(500) || renw) && target.IsInAttackRange())
                    {
                        return false;
                    }
                    break;
                case 3:
                    if (spellSlot != SpellSlot.E && rene && E.IsReady(500))
                    {
                        return false;
                    }
                    break;
                default:
                    return true;
                    break;
            }
            return true;
        }

        private void InitRenekton()
        {
            Q = new Spell(SpellSlot.Q, 300);
            W = new Spell(SpellSlot.W, player.AttackRange + 55);
            E = new Spell(SpellSlot.E, 450);
            E.SetSkillshot(
                E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Speed, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 300);
        }

        private void InitMenu()
        {
            config = new Menu("Renekton", "Renekton", true);
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
            menuD.AddItem(new MenuItem("drawee", "Draw E range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 109, 111, 126)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage")).SetValue(true);
            menuD.AddItem(new MenuItem("rDamage", "Calc R damge too", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usee", "Use E", true)).SetValue(true);
            menuC.AddItem(new MenuItem("user", "Use R under", true)).SetValue(new Slider(20, 0, 100));
            menuC.AddItem(new MenuItem("userindanger", "Use R min X enemy", true)).SetValue(new Slider(2, 1, 5));
            menuC.AddItem(new MenuItem("furyMode", "Fury priority", true))
                .SetValue(new StringList(new[] { "No priority", "Q", "W", "E" }, 0));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite")).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("usewH", "Use W", true)).SetValue(true);
            menuH.AddItem(new MenuItem("useCH", "Harass mode", true))
                .SetValue(new StringList(new[] { "Use harass combo", "E-furyQ-Eback if possible", "Basic" }, 1));
            menuH.AddItem(new MenuItem("donteqwebtower", "Don't dash under tower", true)).SetValue(true);
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("minimumMini", "Use Q min minion", true)).SetValue(new Slider(2, 1, 6));
            menuLC.AddItem(new MenuItem("usewLC", "Use W", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("useeLC", "Use E", true)).SetValue(true);
            config.AddSubMenu(menuLC);
            // Misc Settings
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);

            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}