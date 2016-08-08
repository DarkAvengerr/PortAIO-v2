using System;
using System.Linq;
using AutoJungle.Data;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace AutoJungle
{
    internal class Champdata
    {
        public AIHeroClient Hero = null;
        public BuildType Type;

        public Func<bool> JungleClear;
        public Func<bool> Combo;
        public static Spell Q, W, E, R;
        public AutoLeveler Autolvl;

        public Champdata()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "MasterYi":
                    Hero = ObjectManager.Player;
                    Type = BuildType.YI;

                    Q = new Spell(SpellSlot.Q, 600);
                    Q.SetTargetted(0.5f, float.MaxValue);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 0, 2, 1, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = MasteryiJungleClear;
                    Combo = MasteryiCombo;
                    Console.WriteLine("Masteryi loaded");
                    break;

                case "Warwick":
                    Hero = ObjectManager.Player;
                    Type = BuildType.AS;

                    Q = new Spell(SpellSlot.Q, 400, TargetSelector.DamageType.Magical);
                    Q.SetTargetted(0.5f, float.MaxValue);
                    W = new Spell(SpellSlot.W, 1250);
                    E = new Spell(SpellSlot.E);
                    R = new Spell(SpellSlot.R, 700, TargetSelector.DamageType.Magical);
                    R.SetTargetted(0.5f, float.MaxValue);

                    Autolvl = new AutoLeveler(new int[] { 0, 1, 2, 0, 0, 3, 0, 1, 0, 1, 3, 1, 1, 2, 2, 3, 2, 2 });

                    JungleClear = WarwickJungleClear;
                    Combo = WarwickCombo;

                    Console.WriteLine("Warwick loaded");
                    break;

                case "Shyvana":
                    Hero = ObjectManager.Player;
                    Type = BuildType.AS;

                    Q = new Spell(SpellSlot.Q);
                    W = new Spell(SpellSlot.W, 350f);
                    E = new Spell(SpellSlot.E, 925f);
                    E.SetSkillshot(0.25f, 60f, 1500, false, SkillshotType.SkillshotLine);
                    R = new Spell(SpellSlot.R, 1000f);
                    R.SetSkillshot(0.25f, 150f, 1500, false, SkillshotType.SkillshotLine);

                    Autolvl = new AutoLeveler(new int[] { 1, 2, 0, 1, 1, 3, 1, 0, 1, 0, 3, 0, 0, 2, 2, 3, 2, 2 });

                    JungleClear = ShyvanaJungleClear;
                    Combo = ShyvanaCombo;

                    Console.WriteLine("Shyvana loaded");
                    break;

                case "SkarnerNOTWORKINGYET":
                    Hero = ObjectManager.Player;
                    Type = BuildType.AS;

                    Q = new Spell(SpellSlot.Q, 325);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 985);
                    E.SetSkillshot(0.5f, 60, 1200, false, SkillshotType.SkillshotLine);
                    R = new Spell(SpellSlot.R, 325);

                    Autolvl = new AutoLeveler(new int[] { 0, 1, 2, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = SkarnerJungleClear;
                    Combo = SkarnerCombo;

                    Console.WriteLine("Skarner loaded");
                    break;
                case "Jax":
                    Hero = ObjectManager.Player;
                    Type = BuildType.ASMANA;

                    Q = new Spell(SpellSlot.Q, 680f);
                    Q.SetTargetted(0.50f, 75f);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 2, 1, 0, 0, 0, 3, 0, 1, 0, 1, 3, 1, 1, 2, 2, 3, 2, 2 });
                    JungleClear = JaxJungleClear;
                    Combo = JaxCombo;

                    Console.WriteLine("Jax loaded");
                    break;
                case "XinZhao":
                    Hero = ObjectManager.Player;
                    Type = BuildType.AS;

                    Q = new Spell(SpellSlot.Q);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 600);
                    R = new Spell(SpellSlot.R, 450f);

                    Autolvl = new AutoLeveler(new int[] { 0, 1, 2, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = XinJungleClear;
                    Combo = XinCombo;
                    Console.WriteLine("Xin Zhao loaded");
                    break;

                case "Nocturne":
                    Hero = ObjectManager.Player;
                    Type = BuildType.NOC;

                    Q = new Spell(SpellSlot.Q, 1150);
                    Q.SetSkillshot(0.25f, 60f, 1350, false, SkillshotType.SkillshotLine);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 400, TargetSelector.DamageType.Magical);
                    E.SetTargetted(0.50f, 75f);
                    R = new Spell(SpellSlot.R, 4000);
                    R.SetTargetted(0.75f, 4000f);

                    Autolvl = new AutoLeveler(new int[] { 0, 2, 1, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = NocturneJungleClear;
                    Combo = NocturneCombo;
                    Console.WriteLine("Nocturne loaded");
                    break;

                case "Evelynn":
                    Hero = ObjectManager.Player;
                    Type = BuildType.EVE;

                    Q = new Spell(SpellSlot.Q, 500f);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 225);
                    R = new Spell(SpellSlot.R, 650);
                    R.SetSkillshot(
                        R.Instance.SData.SpellCastTime, R.Instance.SData.LineWidth, R.Speed, false,
                        SkillshotType.SkillshotCone);

                    Autolvl = new AutoLeveler(new int[] { 0, 2, 1, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = EveJungleClear;
                    Combo = EveCombo;
                    Console.WriteLine("Evelynn loaded");
                    break;

                case "Volibear":
                    Hero = ObjectManager.Player;
                    Type = BuildType.VB;

                    Q = new Spell(SpellSlot.Q);
                    W = new Spell(SpellSlot.W, 400);
                    E = new Spell(SpellSlot.E, 425);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 2, 1, 0, 1, 1, 3, 1, 0, 1, 0, 3, 0, 0, 2, 2, 3, 2, 2 });

                    JungleClear = VbJungleClear;
                    Combo = VoliCombo;
                    Console.WriteLine("Volibear loaded");
                    break;

                case "Tryndamere":
                    Hero = ObjectManager.Player;
                    Type = BuildType.Manwang;

                    Q = new Spell(SpellSlot.Q);
                    W = new Spell(SpellSlot.W, 850);
                    E = new Spell(SpellSlot.E, 600);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 0, 2, 0, 1, 0, 3, 0, 0, 2, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = MWJungleClear;
                    Combo = MWCombo;
                    Console.WriteLine("Tryndamere loaded");
                    break;

                case "Olaf":
                    Hero = ObjectManager.Player;
                    Type = BuildType.Bro;

                    Q = new Spell(SpellSlot.Q, 1000);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 325);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 0, 1, 2, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = BroJungleClear;
                    Combo = BroCombo;
                    Console.WriteLine("Brolaf loaded");
                    break;

                case "Nunu":
                    Hero = ObjectManager.Player;
                    Type = BuildType.Nu;

                    Q = new Spell(SpellSlot.Q, 1000);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 325);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 0, 1, 2, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = NuJungleClear;
                    Combo = NuCombo;
                    Console.WriteLine("Nunu loaded");
                    break;

                case "Udyr":
                    Hero = ObjectManager.Player;
                    Type = BuildType.UD;

                    Q = new Spell(SpellSlot.Q);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E);
                    R = new Spell(SpellSlot.R, 250);

                    Autolvl = new AutoLeveler(new int[] { 3, 1, 2, 3, 3, 0, 3, 1, 3, 1, 1, 1, 2, 2, 2, 2, 0, 0 });

                    JungleClear = UDJungleClear;
                    Combo = UDCombo;
                    Console.WriteLine("Udyr loaded");
                    break;

                case "KogMaw":
                    Hero = ObjectManager.Player;
                    Type = BuildType.KOG;

                    Q = new Spell(SpellSlot.Q, 1175);
                    W = new Spell(SpellSlot.W, 710);
                    E = new Spell(SpellSlot.E, 1280);
                    R = new Spell(SpellSlot.R, 1800);

                    Autolvl = new AutoLeveler(new int[] { 1, 0, 2, 1, 1, 3, 0, 2, 1, 1, 3, 0, 0, 2, 2, 3, 0, 2 });

                    JungleClear = KogJungleClear;
                    Combo = KogCombo;
                    Console.WriteLine("KogMaw loaded");
                    break;
                default:
                    Console.WriteLine(ObjectManager.Player.ChampionName + " not supported");
                    break;
                //nidale w buff?(优先）) | sej，结束skr，amumu？ graves！
            }
        }

        public static void UseSpellsCombo()
        {
            var target = Program._GameInfo.Target;
            var igniteDmg = Program.player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (target != null && Program.menu.Item("UseIgniteG").GetValue<Boolean>() &&
                target.LSDistance(Program.player) < 600 && target.Health < igniteDmg - 15)
            {
                GameInfo.CastSpell(Program._GameInfo.Ignite, target);
            }
            var goingToDie = Program.player.Health - Program._GameInfo.DamageTaken <= 0;
            var healSlider = Program.menu.Item("UseHealG").GetValue<Slider>().Value;
            if (healSlider >= 0 &&
                ((healSlider > Program.player.HealthPercent && Program._GameInfo.DamageTaken > 0) || goingToDie))
            {
                GameInfo.CastSpell(Program._GameInfo.Heal);
            }

            var barrierSlider = Program.menu.Item("UseBarrierG").GetValue<Slider>().Value;
            if (barrierSlider >= 0 &&
                ((barrierSlider > Program.player.HealthPercent && Program._GameInfo.DamageTaken > 0) || goingToDie))
            {
                GameInfo.CastSpell(Program._GameInfo.Barrier);
            }
        }

        public static void UseSpellsDef()
        {
            var goingToDie = Program.player.Health - Program._GameInfo.DamageTaken <= 0;
            var healSlider = Program.menu.Item("UseHealJ").GetValue<Slider>().Value;
            if (healSlider >= 0 &&
                ((healSlider > Program.player.HealthPercent && Program._GameInfo.DamageTaken > 0) || goingToDie))
            {
                GameInfo.CastSpell(Program._GameInfo.Heal);
            }

            var barrierSlider = Program.menu.Item("UseBarrierJ").GetValue<Slider>().Value;
            if (barrierSlider >= 0 &&
                ((barrierSlider > Program.player.HealthPercent && Program._GameInfo.DamageTaken > 0) || goingToDie))
            {
                GameInfo.CastSpell(Program._GameInfo.Barrier);
            }
        }

         private bool KogCombo()
         {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (Q.LSIsReady() && targetHero.LSIsValidTarget(900) || Hero.ManaPercent > 50)
            {
                Q.CastIfHitchanceEquals(targetHero, HitChance.High);
            }
            if (W.LSIsReady() && targetHero.LSIsValidTarget(710))
            {
                W.Cast();
            }
            if (E.LSIsReady() && targetHero.LSIsValidTarget(1000))
            {
                E.Cast(targetHero);
            }
            if (R.LSIsReady() && Hero.ManaPercent > 60 && !Hero.LSHasBuff("KogMawBioArcaneBarrage"))
            {
                R.CastIfHitchanceEquals(targetHero, HitChance.High);
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
         }

         private bool KogJungleClear()
         {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (Q.LSIsReady() && Hero.LSDistance(targetMob) < Q.Range &&
                (Helpers.getMobs(Hero.Position, Q.Range).Count >= 1 || targetMob.MaxHealth > 125))
            {
                Q.Cast(targetMob);
            }
            if (W.LSIsReady() && (Helpers.getMobs(Hero.Position, Q.Range).Count >= 1 || targetMob.MaxHealth > 700))
            {
                W.Cast();
            }
            if (E.LSIsReady() && Hero.LSDistance(targetMob) < E.Range &&
                (Helpers.getMobs(Hero.Position, E.Range).Count >= 2 || targetMob.MaxHealth > 700))
            {
                E.Cast(targetMob);
            }
            if (R.LSIsReady() && Hero.ManaPercent > 70 &&
                (Helpers.getMobs(Hero.Position, R.Range).Count >= 2 || targetMob.MaxHealth > 700))
            {
                R.Cast(targetMob);
            }
            ItemHandler.UseItemsJungle();
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
         }

        private bool UDCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (E.LSIsReady() && targetHero.LSIsValidTarget(700))
            {
                E.Cast();
            }
            if (R.LSIsReady())
            {
                R.Cast();
            }
            if (W.LSIsReady())
            {
                W.Cast();
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool UDJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (R.LSIsReady() && Hero.LSDistance(targetMob) < 135 || targetMob.MaxHealth > 700)
            {
                R.Cast();
            }
            if (W.LSIsReady() && Hero.HealthPercent < 75)
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool NuCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            /*
            if (Q.LSIsReady() && targetmob != null && Hero.LSDistance(targetmob) < 700 || Hero.HealthPercent > 97)
            {
                Q.Cast();
            }
            */
            ItemHandler.UseItemsCombo(targetHero, true);
            if (W.LSIsReady() && !Hero.LSHasBuff("AbsoluteZero"))
            {
                W.Cast();
            }
            if (E.LSIsReady() && !Hero.LSHasBuff("AbsoluteZero") && E.CanCast(targetHero))
            {
                E.CastOnUnit(targetHero);
            }
            if (R.LSIsReady() && !Hero.LSHasBuff("AbsoluteZero") && targetHero.LSIsValidTarget(125))
            {
                R.Cast();
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool NuJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (Q.LSIsReady() && Hero.LSDistance(targetMob) < Q.Range &&
                (Helpers.getMobs(Hero.Position, Q.Range).Count >= 1 || targetMob.MaxHealth > 125))
            {
                Q.Cast(targetMob);
            }
            if (W.LSIsReady())
            {
                W.Cast();
            }
            if (E.LSIsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || targetMob.MaxHealth > 700))
            {
                E.CastOnUnit(targetMob);
            }
            ItemHandler.UseItemsJungle();
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool BroCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady() && targetHero.LSIsValidTarget(1000))
            {
                Q.Cast(targetHero);
            }
            if (E.LSIsReady() && Hero.HealthPercent > 30 && targetHero.LSIsValidTarget(325))
            {
                E.Cast(targetHero);
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (W.LSIsReady() && targetHero.LSIsValidTarget(325))
            {
                W.Cast();
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool BroJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (Q.LSIsReady() && targetMob.LSIsValidTarget(325))
            {
                Q.Cast(targetMob);
            }
            ItemHandler.UseItemsJungle();
            if (E.LSIsReady() && Hero.LSDistance(targetMob) < 300 &&
                (Program._GameInfo.SmiteableMob != null || Program._GameInfo.MinionsAround > 3 || structure != null))
                if (Hero.HealthPercent > 45)
                {
                    E.Cast(targetMob);
                }
            if (W.LSIsReady() && Hero.LSDistance(targetMob) < 300 &&
                (Program._GameInfo.SmiteableMob != null || Program._GameInfo.MinionsAround > 3 || structure != null))
            {
                if (Hero.Mana > Q.ManaCost + W.ManaCost || Hero.HealthPercent > 45)
                {
                    W.Cast();
                }
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool MWCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (E.LSIsReady() && targetHero.LSIsValidTarget(600))
            {
                E.Cast(targetHero);
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (W.LSIsReady() && targetHero.LSIsValidTarget(850))
            {
                W.Cast();
            }
            if (Q.LSIsReady() && !Hero.LSHasBuff("UndyingRage") && Hero.HealthPercent < 20)
            {
                Q.Cast();
            }
            if (R.LSIsReady() && Hero.HealthPercent < 15 && targetHero.LSCountEnemiesInRange(700) >= 1)
            {
                R.Cast();
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool MWJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            ItemHandler.UseItemsJungle();
            if (E.LSIsReady() && targetMob.LSIsValidTarget(600))
            {
                E.Cast(targetMob);
            }
            if (Q.LSIsReady() && Hero.HealthPercent < 30)
            {
                Q.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool VoliCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady() && targetHero.LSIsValidTarget(550))
            {
                Q.Cast();
            }
            if (E.LSIsReady() && targetHero.LSIsValidTarget(425))
            {
                E.Cast();
            }
            if (R.LSIsReady() && Hero.LSDistance(targetHero) < 400 && Hero.Mana > 100)
            {
                R.Cast();
            }
            if (W.LSIsReady() && targetHero.LSIsValidTarget(400))
            {
                W.CastOnUnit(targetHero);
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool VbJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            ItemHandler.UseItemsJungle();
            if (E.LSIsReady() && targetMob.LSIsValidTarget(425) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                E.Cast();
            }
            if (Q.LSIsReady() && targetMob.LSIsValidTarget(550))
            {
                Q.Cast();
            }
            if (W.LSIsReady() && targetMob.LSIsValidTarget(400))
            {
                W.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool EveCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady() && Q.CanCast(targetHero))
            {
                Q.CastOnUnit(targetHero);
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (W.LSIsReady() && targetHero.LSIsValidTarget(750))
            {
                W.Cast();
            }
            if (R.LSIsReady() && Hero.LSDistance(targetHero) < 650 && Hero.Mana > 100)
            {
                R.Cast(targetHero);
            }
            if (E.LSIsReady() && E.CanCast(targetHero))
            {
                E.CastOnUnit(targetHero);
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool EveJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            ItemHandler.UseItemsJungle();
            if (Q.LSIsReady() && Hero.LSDistance(targetMob) < Q.Range &&
                (Helpers.getMobs(Hero.Position, Q.Range).Count >= 2 || targetMob.MaxHealth > 700))
            {
                Q.Cast(targetMob);
            }
            if (E.LSIsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || targetMob.MaxHealth > 700))
            {
                E.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool JaxCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (targetHero == null)
            {
                return false;
            }
            if (R.LSIsReady() && Hero.LSDistance(targetHero) < 300 && Hero.Mana > 250)
            {
                R.Cast();
            }
            if (W.LSIsReady() && targetHero.LSIsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !Q.LSIsReady());
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady() && Q.CanCast(targetHero) &&
                (targetHero.LSDistance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) || Hero.HealthPercent < 40))
            {
                Q.CastOnUnit(targetHero);
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool XinJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.LSIsReady() && targetMob.LSIsValidTarget(300) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.LSIsReady() && targetMob.LSIsValidTarget(300))
            {
                Q.Cast();
            }
            if (E.LSIsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                E.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool XinCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (targetHero == null)
            {
                return false;
            }
            if (R.LSIsReady() && Hero.LSDistance(targetHero) < R.Range && targetHero.LSHasBuff("xenzhaointimidate") &&
                targetHero.Health > R.GetDamage(targetHero) + Hero.LSGetAutoAttackDamage(targetHero, true) * 4)
            {
                R.Cast();
            }
            if (W.LSIsReady() && targetHero.LSIsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !E.LSIsReady());
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady() && targetHero.LSDistance(Hero) < Orbwalking.GetRealAutoAttackRange(targetHero) + 50)
            {
                Q.Cast();
            }
            if (E.LSIsReady() && E.CanCast(targetHero) &&
                (Hero.HealthPercent < 40 || targetHero.LSDistance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) ||
                 Prediction.GetPrediction(targetHero, 1f).UnitPosition.LSUnderTurret(true)))
            {
                E.CastOnUnit(targetHero);
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool JaxJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.LSIsReady() && targetMob.LSIsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.LSIsReady() && Q.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                Q.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool SkarnerCombo()
        {
            var targetHero = Program._GameInfo.Target;
            var rActive = Hero.LSHasBuff("skarnerimpalevo");
            if (W.LSIsReady() && targetHero != null && Hero.LSDistance(targetHero) < 700)
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !E.LSIsReady());
            if (Q.LSIsReady() && ((targetHero != null && Q.CanCast(targetHero)) || rActive))
            {
                Q.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (E.LSIsReady() && !rActive && targetHero != null && E.CanCast(targetHero) &&
                Hero.LSDistance(targetHero) < 700)
            {
                E.CastIfHitchanceEquals(targetHero, HitChance.High);
            }
            if (R.LSIsReady() && targetHero != null && R.CanCast(targetHero) && !targetHero.LSHasBuff("SkarnerImpale"))
            {
                R.CastOnUnit(targetHero);
            }
            if (rActive)
            {
                var allyTower =
                    Program._GameInfo.AllyStructures.OrderBy(a => a.LSDistance(Hero.Position)).FirstOrDefault();
                if (allyTower != null && allyTower.LSDistance(Hero.Position) < 2000 &&
                    allyTower.LSDistance(Hero.Position) > 300)
                {
                    Console.WriteLine(2);
                    Console.WriteLine(allyTower.LSDistance(Hero.Position));
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, allyTower.LSExtend(Program._GameInfo.SpawnPoint, 300));
                    Program.pos = allyTower.LSExtend(Program._GameInfo.SpawnPoint, 300);
                    return false;
                }
                var ally =
                    HeroManager.Allies.Where(a => a.LSDistance(Hero.Position) < 1500)
                        .OrderBy(a => a.LSDistance(Hero))
                        .FirstOrDefault();
                if (ally != null && ally.LSDistance(Hero) > 300)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ally.Position);
                    Console.WriteLine(1);
                    Program.pos = ally.Position;
                    return false;
                }
                var enemyTower =
                    Program._GameInfo.EnemyStructures.OrderBy(a => a.LSDistance(Hero.Position)).FirstOrDefault();
                if (enemyTower != null && enemyTower.LSDistance(Hero.Position) < 2000 &&
                    enemyTower.LSDistance(Hero.Position) > 300)
                {
                    Console.WriteLine(3);
                    Program.pos = targetHero.Position.LSExtend(enemyTower, 2500);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Hero.Position.LSExtend(enemyTower, 2500));
                    return false;
                }
            }
            else if (targetHero != null)
            {
                OrbwalkingForBots.Orbwalk(targetHero);
            }
            return false;
        }

        private bool SkarnerJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.LSIsReady() && Hero.LSDistance(targetMob) < Q.Range &&
                (Helpers.getMobs(Hero.Position, W.Range).Count >= 2 ||
                 targetMob.Health > Hero.LSGetAutoAttackDamage(targetMob, true) * 5))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.LSIsReady() && Q.CanCast(targetMob))
            {
                Q.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (E.LSIsReady() && E.CanCast(targetMob))
            {
                var pred = E.GetLineFarmLocation(Helpers.getMobs(Hero.Position, E.Range));
                if (pred.MinionsHit >= 2 || targetMob.Health > Hero.LSGetAutoAttackDamage(targetMob, true) * 5)
                {
                    E.CastIfHitchanceEquals(targetMob, HitChance.VeryHigh);
                }
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool ShyvanaCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (W.LSIsReady() && Hero.LSDistance(targetHero) < W.Range + 100)
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady() && Orbwalking.GetRealAutoAttackRange(targetHero) > Hero.LSDistance(targetHero))
            {
                Q.Cast();
            }
            if (E.LSIsReady() && E.CanCast(targetHero))
            {
                E.Cast(targetHero);
            }
            if (R.LSIsReady() && Hero.Mana == 100 &&
                targetHero.LSCountEnemiesInRange(GameInfo.ChampionRange) <=
                targetHero.LSCountAlliesInRange(GameInfo.ChampionRange) &&
                !Hero.Position.LSExtend(targetHero.Position, GameInfo.ChampionRange).LSUnderTurret(true))
            {
                R.CastIfHitchanceEquals(targetHero, HitChance.VeryHigh);
            }

            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool ShyvanaJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.LSIsReady() && Hero.LSDistance(targetMob) < W.Range &&
                (Helpers.getMobs(Hero.Position, W.Range).Count >= 2 ||
                 targetMob.Health > W.GetDamage(targetMob) * 7 + Hero.LSGetAutoAttackDamage(targetMob, true) * 2))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady())
            {
                Q.Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, targetMob);
            }
            if (E.LSIsReady() && E.CanCast(targetMob))
            {
                E.Cast(targetMob);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool WarwickCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady() && Q.CanCast(targetHero))
            {
                Q.CastOnUnit(targetHero);
            }
            if (W.LSIsReady() && Hero.LSDistance(targetHero) < 300)
            {
                if (Hero.Mana > Q.ManaCost + W.ManaCost || Hero.HealthPercent > 70)
                {
                    W.Cast();
                }
            }
            if (R.LSIsReady() && R.CanCast(targetHero) && !targetHero.MagicImmune)
            {
                R.CastOnUnit(targetHero);
            }
            if (E.LSIsReady() && Hero.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 && Hero.LSDistance(targetHero) < 1000)
            {
                E.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !R.LSIsReady());
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool WarwickJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (Q.LSIsReady() && Q.CanCast(targetMob) &&
                (Hero.ManaPercent > 50 || Hero.MaxHealth - Hero.Health > Q.GetDamage(targetMob) * 0.8f))
            {
                Q.CastOnUnit(targetMob);
            }
            if (W.LSIsReady() && Hero.LSDistance(targetMob) < 300 &&
                (Program._GameInfo.SmiteableMob != null || Program._GameInfo.MinionsAround > 3 || structure != null))
            {
                if (Hero.Mana > Q.ManaCost + W.ManaCost || Hero.HealthPercent > 70)
                {
                    W.Cast();
                }
            }
            if (E.LSIsReady() && Hero.Spellbook.GetSpell(SpellSlot.E).ToggleState != 1 && Hero.LSDistance(targetMob) < 500)
            {
                E.Cast();
            }
            ItemHandler.UseItemsJungle();
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool MasteryiJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (E.LSIsReady() && Hero.Spellbook.IsAutoAttacking)
            {
                E.Cast();
            }
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (R.LSIsReady() && Hero.Position.LSDistance(Hero.Position) < 300 &&
                Jungle.bosses.Any(n => targetMob.Name.Contains(n)))
            {
                R.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady() && Q.CanCast(targetMob) && targetMob.Health < targetMob.MaxHealth)
            {
                Q.CastOnUnit(targetMob);
            }
            if (W.LSIsReady() && Hero.HealthPercent < 50)
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool MasteryiCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling &&
                targetHero.Health > Program.player.LSGetAutoAttackDamage(targetHero, true) * 2)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (E.LSIsReady() && Hero.Spellbook.IsAutoAttacking)
            {
                E.Cast();
            }
            if (R.LSIsReady() && Hero.LSDistance(targetHero) < 600)
            {
                R.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.LSIsReady())
            {
                Q.CastOnUnit(targetHero);
            }
            if ((Hero.Spellbook.IsChanneling &&
                 targetHero.Health > Program.player.LSGetAutoAttackDamage(targetHero, true) * 2) || targetHero == null)
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !Q.LSIsReady());
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool NocturneCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            /* check under tower? r active 1sec delay move to target
                        if (R.LSIsReady() && Hero.LSDistance(targetHero) < 1300 &&
                            (targetHero.LSDistance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) &&
                            targetHero.LSUnderTurret(true))
            {
                R.CastOnUnit(targetHero);
            }
            */
            if (R.LSIsReady() && Hero.LSDistance(targetHero) < 900)
            {
                R.CastOnUnit(targetHero);
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (Q.LSIsReady() && Q.CanCast(targetHero))
            {
                Q.Cast(targetHero);
            }
            if (W.LSIsReady() && targetHero.LSIsValidTarget(300))
            {
                W.Cast();
            }
            if (E.LSIsReady() && E.CanCast(targetHero))
            {
                E.CastOnUnit(targetHero);
            }
            OrbwalkingForBots.Orbwalk(targetHero);
            return false;
        }

        private bool NocturneJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            ItemHandler.UseItemsJungle();
            if (Q.LSIsReady() && targetMob.LSIsValidTarget(400))
            {
                Q.Cast(targetMob);
            }
            if (E.LSIsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                E.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }
    }
}