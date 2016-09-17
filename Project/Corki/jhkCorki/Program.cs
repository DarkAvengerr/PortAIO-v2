using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace jhkCorki
{
    class Program
    {
        private static Spell Q, W, E, R1, R2;
        private static AIHeroClient Player = ObjectManager.Player;
        private static Menu Config;
        private static Orbwalking.Orbwalker _Ob;

        private static void Game_OnGameLoad(EventArgs args)
        {

            if (Player.BaseSkinName != "Corki")
                return;

            Q = new Spell(SpellSlot.Q, 825f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 600f);
            R1 = new Spell(SpellSlot.R, 1300f);
            R2 = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.35f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0f, (float)(45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);
            R1.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
            R2.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            Config = new Menu("jhkCorki", "jhkCorki", true);
            var tsMenu = new Menu("Target Selector", "jhkCorki.ts");
            Config.AddSubMenu(tsMenu);
            TargetSelector.AddToMenu(tsMenu);
            var obMenu = new Menu("OrbWalker", "jhkCorki.OrbWalker");
            _Ob = new Orbwalking.Orbwalker(obMenu);
            Config.AddSubMenu(obMenu);

            var MiscMenu = Config.AddSubMenu(new Menu("Misc", "jhkCorki.Misc"));
            {
                MiscMenu.AddItem(new MenuItem("useKS", "KS with R").SetValue(true));
                MiscMenu.AddItem(new MenuItem("HarInClear", "Harrass while Lane Clear").SetValue(true));
            }

            var QMenu = Config.AddSubMenu(new Menu("Q", "jhkCorki.Q"));
            var QComboMenu = QMenu.AddSubMenu(new Menu("Combo", "QComboMenu"));
            {
                QComboMenu.AddItem(new MenuItem("QuseCombo", "Combo").SetValue(true));
                QComboMenu.AddItem(new MenuItem("qHitChance", "Hit Chance Combo").SetValue(new Slider(3, 0, 3)));
            }
            var WMenu = Config.AddSubMenu(new Menu("W", "jhkCorki.W"));
            {
                WMenu.AddItem(new MenuItem("gapclose", "Anti Gap Close").SetValue(true));
            }
            var GapCloseMenu = WMenu.AddSubMenu(new Menu("Gap Close Against", "GapCloseList"));
            {
                foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy))
                    GapCloseMenu.AddItem(new MenuItem("GC." + x.BaseSkinName, x.BaseSkinName).SetValue(true));
            }
            var EMenu = Config.AddSubMenu(new Menu("E", "jhkCorki.E"));
            var EComboMenu = EMenu.AddSubMenu(new Menu("Combo", "EComboMenu"));
            {
                EComboMenu.AddItem(new MenuItem("EuseCombo", "Combo").SetValue(true));
            }
            var RMenu = Config.AddSubMenu(new Menu("R", "jhkCorki.R"));
            var RComboMenu = RMenu.AddSubMenu(new Menu("Combo", "EComboMenu"));
            {
                RComboMenu.AddItem(new MenuItem("RuseCombo", "Combo").SetValue(true));
                RComboMenu.AddItem(new MenuItem("rHitChance", "Hit Chance Combo").SetValue(new Slider(3, 0, 3)));
            }
            var RAutoHarMenu = RMenu.AddSubMenu(new Menu("Auto Harrass", "R.Auto"));
            {
                RAutoHarMenu.AddItem(new MenuItem("AutoRHar", "Auto R Harrass Max Hit Chance").SetValue(true));
                RAutoHarMenu.AddItem(new MenuItem("ccdOnly", "Auto Harrass R CC'd only champ").SetValue(false));
                RAutoHarMenu.AddItem(new MenuItem("AutoRMisCount", "Harrass Only When Missiles >=").SetValue(new Slider(4, 0, 7)));
                RAutoHarMenu.AddItem(new MenuItem("AutoRMana", "Auto Harrass Mana >=").SetValue(new Slider(20, 0, 100)));
                foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy))
                    RAutoHarMenu.AddItem(new MenuItem("Har." + x.BaseSkinName, x.BaseSkinName).SetValue(true));
            }
            var QFarmMenu = QMenu.AddSubMenu(new Menu("Lane Clear", "QFarm"));
            {
                QFarmMenu.AddItem(new MenuItem("QLC", "Lane Clear").SetValue(true));
                QFarmMenu.AddItem(new MenuItem("QFarmCount", "Min Minions for Q").SetValue(new Slider(3, 1, 6)));
                QFarmMenu.AddItem(new MenuItem("ManaFarmQ", "Mana >= % To Farm").SetValue(new Slider(60, 0, 100)));
            }
            var EFarmMenu = EMenu.AddSubMenu(new Menu("Lane Clear", "EFarm"));
            {
                EFarmMenu.AddItem(new MenuItem("ELC", "Lane Clear").SetValue(false));
                EFarmMenu.AddItem(new MenuItem("EFarmCount", "Min Minions for E").SetValue(new Slider(3, 1, 6)));
                EFarmMenu.AddItem(new MenuItem("ManaFarmE", "Mana >= % To Farm").SetValue(new Slider(75, 0, 100)));
            }
            var RFarmMenu = RMenu.AddSubMenu(new Menu("Lane Clear", "RFarm"));
            {
                RFarmMenu.AddItem(new MenuItem("RLC", "Lane Clear").SetValue(true));
                RFarmMenu.AddItem(new MenuItem("BigRClear", "Clear with Big R").SetValue(true));
                RFarmMenu.AddItem(new MenuItem("RFarmCount", "Min Minions for R").SetValue(new Slider(3, 1, 6)));
                RFarmMenu.AddItem(new MenuItem("RMisCount", "Clear if Missile Count >").SetValue(new Slider(4, 1, 7)));
                //RFarmMenu.AddItem(new MenuItem("miniClear", "Use mini R to get last hit about to miss").SetValue(true));
                RFarmMenu.AddItem(new MenuItem("ManaFarmR", "Mana >= % To Farm").SetValue(new Slider(15, 0, 100)));
            }
            var QHarMenu = QMenu.AddSubMenu(new Menu("Harass", "QHarassMenu"));
            {
                QHarMenu.AddItem(new MenuItem("QHar", "Harrass").SetValue(true));
                QHarMenu.AddItem(new MenuItem("QHarMana", "Mana >= % To Harass").SetValue(new Slider(40, 0, 100)));
            }
            var EHarMenu = EMenu.AddSubMenu(new Menu("Harass", "EHarassMenu"));
            {
                EHarMenu.AddItem(new MenuItem("EHar", "Harrass").SetValue(true));
                EHarMenu.AddItem(new MenuItem("EHarMana", "Mana >= % To Harass").SetValue(new Slider(65, 0, 100)));
            }
            var RHarMenu = RMenu.AddSubMenu(new Menu("Harass", "rHarassMenu"));
            {
                RHarMenu.AddItem(new MenuItem("RHar", "Harrass").SetValue(true));
                RHarMenu.AddItem(new MenuItem("RHarMana", "Mana >= % To Harass").SetValue(new Slider(25, 0, 100)));
            }
            var DrawMenu = Config.AddSubMenu(new Menu("Draw", "jhkDraw"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQRange", "Draw Q Range").SetValue(new Circle(true, Color.White)));
                DrawMenu.AddItem(new MenuItem("DrawERange", "Draw E Range").SetValue(new Circle(true, Color.White)));
                DrawMenu.AddItem(new MenuItem("DrawRRange", "Draw R Range").SetValue(new Circle(true, Color.White)));
            }
            Config.AddToMainMenu();

            Game.OnUpdate += GameOnUpdate;
            Orbwalking.AfterAttack += afterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void GameOnUpdate(EventArgs args)
        {
            switch (_Ob.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.LaneClear:
                    laneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harrass();
                    break;
            }
            checkKS();
            checkHar();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var DrawQRange= Config.Item("DrawQRange").GetValue<Circle>();
            var DrawERange = Config.Item("DrawERange").GetValue<Circle>();
            var DrawRRange = Config.Item("DrawRRange").GetValue<Circle>();

            if (DrawQRange.Active)
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.White);
            if (DrawERange.Active)
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.White);
            if (DrawRRange.Active)
                Render.Circle.DrawCircle(Player.Position, R1.Range, Color.White);

        }

        private static void laneClear()
        {
            var Qmana = Config.Item("ManaFarmQ").GetValue<Slider>().Value;
            var Emana = Config.Item("ManaFarmE").GetValue<Slider>().Value;
            var Rmana = Config.Item("ManaFarmR").GetValue<Slider>().Value;
            var useQ = Config.Item("QLC").GetValue<bool>();
            var useE = Config.Item("ELC").GetValue<bool>();
            var useR = Config.Item("RLC").GetValue<bool>();
            var useR2 = Config.Item("BigRClear").GetValue<bool>();
            //var miniR = Config.Item("miniClear").GetValue<bool>();
            var Qcount = Config.Item("QFarmCount").GetValue<Slider>().Value;
            var Ecount = Config.Item("EFarmCount").GetValue<Slider>().Value;
            var Rcount = Config.Item("RFarmCount").GetValue<Slider>().Value;
            var MisCount = Config.Item("RMisCount").GetValue<Slider>().Value;
            var Ammo = Player.Spellbook.GetSpell(SpellSlot.R).Ammo;                     
            var BigMissile = isBigMissle();
            var harrassinclear = Config.Item("HarInClear").GetValue<bool>();
            var missiles = Config.Item("AutoRMisCount").GetValue<Slider>().Value;
            var useQHar = Config.Item("QHar").GetValue<bool>();
            var useEHar = Config.Item("EHar").GetValue<bool>();
            var useRHar = Config.Item("RHar").GetValue<bool>();
            var useQHarMana = Config.Item("QHarMana").GetValue<Slider>().Value;
            var useRHarMana = Config.Item("RHarMana").GetValue<Slider>().Value;

            if (_Ob.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && !Orbwalking.CanAttack() && R1.IsReady() && useR && Player.ManaPercentage() > Rmana)
            {
                if (BigMissile)
                    return;

                if (Ammo > MisCount)
                {
                    List<Obj_AI_Base> killableMinions = MinionManager.GetMinions(R1.Range, MinionTypes.All).Where(x => x.Health <= Player.GetAutoAttackDamage(x)).ToList();
                    if (killableMinions.Count >= 2)
                    {
                        var farthestMinion = killableMinions.OrderBy(x => x.Distance(Player.Position)).First();
                        if (R1.GetDamage(farthestMinion) >= farthestMinion.Health)
                            R1.Cast(farthestMinion);
                    }
                }
            }

            if (Q.IsReady() && useQ && Player.ManaPercentage() > Qmana)
            {
                List<Obj_AI_Base> QMinionsRange = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);         
                List<int> qRangeCount = new List<int>();
                foreach(var minion in QMinionsRange)
                {                   
                    List<Obj_AI_Base> qMinions = MinionManager.GetMinions(minion.Position, Q.Width, MinionTypes.All, MinionTeam.NotAlly);
                    qRangeCount.Add(qMinions.Count());
                }                
                if(qRangeCount.Max() >= Qcount)
                    Q.Cast(QMinionsRange[qRangeCount.IndexOf(qRangeCount.Max())]);
            }

            if (E.IsReady() && useE && Player.ManaPercentage() > Emana)
            {
                List<Obj_AI_Base> EMinions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);
                if (EMinions.Count >= Ecount)
                    E.Cast(EMinions[0]);
            }

            if (BigMissile ? R2.IsReady() : R1.IsReady() && useR && (Ammo > MisCount) && Player.ManaPercentage() > Rmana)
            {
                if (BigMissile && !useR2)
                    return;
        
                List<Obj_AI_Base> RMinionsRange = MinionManager.GetMinions(Player.ServerPosition, (BigMissile ? R2.Range : R1.Range), MinionTypes.All, MinionTeam.NotAlly);
                List<int> rRangeCount = new List<int>();
                foreach (var minion in RMinionsRange)
                {
                    List<Obj_AI_Base> rMinions = MinionManager.GetMinions(minion.Position, BigMissile ? 300f : 200f, MinionTypes.All, MinionTeam.NotAlly);
                    rRangeCount.Add(rMinions.Count());
                }
                if (rRangeCount.Max() >= Rcount)
                    if (BigMissile)
                        R2.Cast(RMinionsRange[rRangeCount.IndexOf(rRangeCount.Max())], false, true);
                    else
                        R1.Cast(RMinionsRange[rRangeCount.IndexOf(rRangeCount.Max())], false, true);
            }

            if (Q.IsReady() && harrassinclear && useQHar && Player.ManaPercentage() >= useQHarMana)
            {
                if (Player.ServerPosition.UnderTurret(true))
                    return;

                var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (t == null)
                    return;
                Q.CastIfHitchanceEquals(t, HitChance.VeryHigh);
            }

            if (BigMissile ? R2.IsReady() : R1.IsReady() && harrassinclear && useRHar && Player.Spellbook.GetSpell(SpellSlot.R).Ammo >= missiles && Player.ManaPercentage() >= useRHarMana)
            {
                if (Player.ServerPosition.UnderTurret(true))
                    return;

                var t = TargetSelector.GetTarget(BigMissile ? R2.Range : R1.Range, TargetSelector.DamageType.Magical);
                if (t == null)
                    return;
                if (!BigMissile)
                    R1.CastIfHitchanceEquals(t, HitChance.VeryHigh);
                else
                    R2.CastIfHitchanceEquals(t, HitChance.VeryHigh);
            }
        }

        private static void Harrass()
        {
            var useQHar = Config.Item("QHar").GetValue<bool>();
            var useEHar = Config.Item("EHar").GetValue<bool>();
            var useRHar = Config.Item("RHar").GetValue<bool>();
            var missiles = Config.Item("AutoRMisCount").GetValue<Slider>().Value;
            var useQHarMana = Config.Item("QHarMana").GetValue<Slider>().Value;
            var useEHarMana = Config.Item("EHarMana").GetValue<Slider>().Value;
            var useRHarMana = Config.Item("RHarMana").GetValue<Slider>().Value;

            if (Q.IsReady() && useQHar && Player.ManaPercentage() >= useQHarMana)
            {
                var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (t == null)
                    return;
                Q.Cast(t);
            }
            if (E.IsReady() && useEHar && Player.ManaPercentage() >= useEHarMana)
            {
                var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (t == null)
                    return;
                E.Cast(t);
            }
            var bigMiss = isBigMissle();
            if (bigMiss ? R2.IsReady() : R1.IsReady() && useRHar && Player.Spellbook.GetSpell(SpellSlot.R).Ammo >= missiles && Player.ManaPercentage() >= useRHarMana)
            {
                var t = TargetSelector.GetTarget(bigMiss ? R2.Range : R1.Range, TargetSelector.DamageType.Magical);
                if (t == null)
                    return;
                if (!bigMiss)
                    R1.Cast(t);
                else
                    R2.Cast(t);
            }
        }

        private static void Combo()
        {
            var useQCombo = Config.Item("QuseCombo").GetValue<bool>();
            var useECombo = Config.Item("EuseCombo").GetValue<bool>();
            var useRCombo = Config.Item("RuseCombo").GetValue<bool>();

            if(Q.IsReady() && useQCombo)
            {
                var t =  TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if(t == null)
                return;
                Q.CastIfHitchanceEquals(t, qHitChance);
            }

            if (E.IsReady() && useECombo)
            {
                var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (t == null)
                    return;
                E.Cast(t);
            }
            var bigMiss = isBigMissle();
            if (bigMiss ? R2.IsReady() : R1.IsReady() && useRCombo)
                {
                var t = TargetSelector.GetTarget(bigMiss ? R2.Range : R1.Range, TargetSelector.DamageType.Magical);
                if (t == null)
                    return;
                    if (!bigMiss)
                        R1.CastIfHitchanceEquals(t, rHitChance);
                    else
                        R2.CastIfHitchanceEquals(t, rHitChance);
                }
        }

        private static void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
          
            if (_Ob.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var useQCombo = Config.Item("QuseCombo").GetValue<bool>();
                var useRCombo = Config.Item("RuseCombo").GetValue<bool>();

                if (Q.IsReady() && useQCombo)
                {
                    var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    if (t == null)
                        return;
                    Q.Cast(t);
                }

                var bigMiss = isBigMissle();
                if (bigMiss ? R2.IsReady() : R1.IsReady() && useRCombo)
                {
                    var t = TargetSelector.GetTarget(bigMiss ? R2.Range : R1.Range, TargetSelector.DamageType.Magical);
                    if (t == null)
                        return;
                    if (!bigMiss)
                        R1.Cast(t, false, true);
                    else
                        R2.Cast(t, false, true);
                }
            }
        }

        private static void checkHar()
        {
            if (!Config.Item("AutoRHar").GetValue<bool>() || Config.Item("AutoRMana").GetValue<Slider>().Value > Player.ManaPercentage() || Player.ServerPosition.UnderTurret(true))
                return;
            if (R1.IsReady())
            {
                var bigMiss = isBigMissle();
                List<AIHeroClient> targets =  ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValidTarget(bigMiss ? R2.Range : R1.Range) && !x.IsZombie && !x.HasBuffOfType(BuffType.Invulnerability) && R1.CanCast(x)).ToList();
                foreach (var target in targets)
                {
                    if (Config.Item("Har." + target.BaseSkinName).GetValue<bool>())
                    {
                        if (!ccd(target) && Config.Item("ccdOnly").GetValue<bool>() || Config.Item("AutoRMisCount").GetValue<Slider>().Value > Player.Spellbook.GetSpell(SpellSlot.R).Ammo)
                            return;
                        else
                        {                           
                            if (bigMiss)
                                R2.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                            else
                                R1.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                        }
                    }
                }
            }
        }

        private static void checkKS()
        {
            if (!Config.Item("useKS").GetValue<bool>())
                return;
            if (R1.IsReady())
            {
                var bigMiss = isBigMissle();
                List<AIHeroClient> targets =  ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValidTarget(bigMiss ? R2.Range : R1.Range) && !x.IsZombie && !x.HasBuffOfType(BuffType.Invulnerability) && R1.CanCast(x) && R1.GetDamage(x) * (bigMiss ? 1.5f : 1f) >= x.Health).ToList();
                foreach (var target in targets)
                {
                    if (bigMiss)
                        R2.Cast(target, false, true);
                    else
                        R1.Cast(target, false, true);
                }
            }
        }

        private static bool ccd(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Polymorph) || target.HasBuffOfType(BuffType.Slow) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt);
        }

        private static HitChance qHitChance
        {
            get
            {
                return getHitChance(Config.Item("qHitChance").GetValue<Slider>().Value);
            }
        }

        private static HitChance rHitChance
        {
            get
            {
                return getHitChance(Config.Item("rHitChance").GetValue<Slider>().Value);
            }
        }

        private static HitChance getHitChance(int val)
        {
            switch (val)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        private static bool isBigMissle()
        {
            return ObjectManager.Player.HasBuff("corkimissilebarragecounterbig");
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser g)
        {
            var doGap = Config.Item("gapclose").GetValue<bool>();
            if (!doGap)
                return;
            if (!Config.Item("GC." + g.Sender.BaseSkinName).GetValue<bool>())
                return;
            //possible can kill  
            if (g.Sender.IsValidTarget() && !Player.Position.Extend(Game.CursorPos, W.Range).IsWall() && W.IsReady() && Player.HealthPercentage() < 70 && Player.HealthPercentage() < g.Sender.HealthPercentage() + 20 && !Q.IsReady() && Player.Position.CountEnemiesInRange(600) > 1 && Player.Position.Extend(Game.CursorPos, W.Range).CountEnemiesInRange(400) < 1)
                W.Cast(Player.Position.Extend(Game.CursorPos, W.Range));
            else if (g.Sender.IsValidTarget() && !Player.Position.Extend(Game.CursorPos, W.Range).IsWall() && W.IsReady() && Player.CountEnemiesInRange(400) >= 3 && Player.Position.Extend(Game.CursorPos, W.Range).CountEnemiesInRange(500) < 2)
                W.Cast(Player.Position.Extend(Game.CursorPos, W.Range));
        }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
    }
}
