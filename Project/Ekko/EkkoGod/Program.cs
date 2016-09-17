using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace EkkoGod
{
    class Program
    {
        private static AIHeroClient Player;
        private static Menu Config;
        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker Orbwalker;
        private static SpellSlot ignite;
        private static Obj_AI_Minion jumpfar;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != "Ekko")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 850);
            Q.SetSkillshot(0.25f, 60f, 1650, false, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 1650);
            W.SetSkillshot(3f, 500f, int.MaxValue, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 450);

            R = new Spell(SpellSlot.R, 375);
            R.SetSkillshot(.3f, 375, int.MaxValue, false, SkillshotType.SkillshotCircle);

            ignite = Player.GetSpellSlot("summonerdot");

            Config = new Menu("Ekko God", "EkkoGod", true);

            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);

            var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Config.AddSubMenu(TargetSelectorMenu);

            var comboMenu = new Menu("Combo", "Combo");
            comboMenu.AddItem(new MenuItem("QMode", "QMode").SetValue(new StringList(new[] { "QE", "EQ", "EQ Hyper Speed (test)" }, 0)));
            comboMenu.AddItem(new MenuItem("UseQCombo", "Use Q in combo").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseWCombo", "Cast W before R in AoE").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseWCombo2", "Cast W before R in combo killable").SetValue(true));
            comboMenu.AddItem(new MenuItem("UseECombo", "Use E in combo").SetValue(true));
            Config.AddSubMenu(comboMenu);

            var RMenu = new Menu("R Options", "RMenu");
            RMenu.AddItem(new MenuItem("UseRKillable", "Use R if combo killable").SetValue(true));
            RMenu.AddItem(new MenuItem("UseRatHP", "Use R at %HP").SetValue(true));
            RMenu.AddItem(new MenuItem("HP", "HP").SetValue(new Slider(30, 0, 100)));
            RMenu.AddItem(new MenuItem("UseRAoE", "Use R AoE").SetValue(true));
            RMenu.AddItem(new MenuItem("AoECount", "Minimum targets to R").SetValue(new Slider(3, 1, 5)));
            RMenu.AddItem(new MenuItem("UseRifDie", "Use R if ability will kill me").SetValue(true));
            RMenu.AddItem(new MenuItem("UseRDangerous", "Use R on ZedR, ViR, etc.").SetValue(true));
            comboMenu.AddSubMenu(RMenu);

            var harassMenu = new Menu("Harass", "Harass");
            harassMenu.AddItem(new MenuItem("UseQHarass", "Use Q in harass").SetValue(true));
            harassMenu.AddItem(new MenuItem("UseEHarass", "Use E in harass").SetValue(true));
            harassMenu.AddItem(new MenuItem("HarassMana", "Mana manager (%)").SetValue(new Slider(40, 1, 100)));
            Config.AddSubMenu(harassMenu);

            var drawingsMenu = new Menu("Drawings", "Drawings");
            drawingsMenu.AddItem(new MenuItem("drawQ", "Q range (also is dash+leap range)").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawingsMenu.AddItem(new MenuItem("drawW", "W range").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawingsMenu.AddItem(new MenuItem("drawE", "E (leap) range").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawingsMenu.AddItem(new MenuItem("drawGhost", "R range (around ghost)").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawingsMenu.AddItem(new MenuItem("drawPassiveStacks", "Passive stacks").SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            var dmgAfterCombo = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterCombo.GetValue<bool>();
            drawingsMenu.AddItem(dmgAfterCombo);
            Config.AddSubMenu(drawingsMenu);

            var fleeMenu = new Menu("Flee", "Flee");
            fleeMenu.AddItem(new MenuItem("Escape", "Escape").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
            fleeMenu.AddItem(new MenuItem("QFlee", "Q enemy while fleeing").SetValue(true));
            fleeMenu.AddItem(new MenuItem("EFlee", "Jump to furthest minion w/ E").SetValue(true));
            Config.AddSubMenu(fleeMenu);

            var miscMenu = new Menu("Misc", "Misc");
            miscMenu.AddItem(new MenuItem("Killsteal", "KS with Q").SetValue(true));
            miscMenu.AddItem(new MenuItem("WSelf", "W Self on Gapclose").SetValue(true));
            miscMenu.AddItem(new MenuItem("WCC", "Cast W on Immobile").SetValue(true));
            miscMenu.AddItem(new MenuItem("UseIgnite", "Ignite if Combo Killable").SetValue(true));
            miscMenu.AddItem(new MenuItem("---", "--underneath not functional--"));
            miscMenu.AddItem(new MenuItem("123", "E Minion After Manual E if Target Far").SetValue(false));
            Config.AddSubMenu(miscMenu);

            //Config.AddItem(new MenuItem("eToMinion", "E Minion After Manual E if Target Far").SetValue(true));


            Config.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        #region damagecalcscreditto1337/worstping
        // WorstPing
        public static double GetDamageQ(Obj_AI_Base target) // fixed
        {
            return Q.IsReady()
                       ? Player.CalcDamage(
                           target,
                           Damage.DamageType.Magical,
                           new double[] { 120, 160, 200, 240, 280 }[Q.Level - 1]
                           + Player.TotalMagicalDamage * .8f)
                       : 0d;
        }

        // 1337
        public static double GetDamageE(Obj_AI_Base target)
        {
            return E.IsReady()
                       ? Player.CalcDamage(
                           target,
                           Damage.DamageType.Magical,
                           new double[] { 50, 80, 110, 140, 170 }[E.Level - 1]
                           + Player.TotalMagicalDamage * .2f)
                       : 0d;
        }


        // WorstPing
        public static double GetDamageR(Obj_AI_Base target)
        {
            return R.IsReady()
                       ? Player.CalcDamage(
                           target,
                           Damage.DamageType.Magical,
                           new double[] { 200, 350, 500 }[R.Level - 1]
                           + Player.TotalMagicalDamage * 1.3f)
                       : 0d;
        }

        #endregion


        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {

            var WSelf = Config.Item("WSelf").GetValue<bool>();

            if (WSelf && W.IsReady() && Player.Distance(gapcloser.Sender.ServerPosition) < E.Range)
            {
                W.Cast(Player.Position);
            }
        }

        private static float ComboDamage(AIHeroClient hero)
        {

            var dmg = 0d;

            dmg += GetDamageQ(hero);
            dmg += GetDamageE(hero);
            dmg += GetDamageR(hero);
            if (!hero.HasBuff("EkkoStunMarker"))
            {
                dmg += 15 + (12 * Player.Level) + Player.TotalMagicalDamage * .7f; // passive damage
            }
            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                dmg += Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return (float)dmg;
        }

        private static void OnUpdate(EventArgs args)
        {

            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Config.Item("Escape").GetValue<KeyBind>().Active)
                    {
                        Escape();
                    }
                    break;
            }

            WCC();
            Killsteal();
            RSafe();
        }

        private static void Escape() // some credits to 1337 :v) (also not as good, me suck)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var enemies = HeroManager.Enemies.Where(t => t.IsValidTarget() && t.Distance(Player.Position) <= W.Range);

            if (Q.IsReady() && target.IsValidTarget() && Config.Item("QFlee").GetValue<bool>())
            {
                Q.Cast(target);
            }

            if (E.IsReady() && Config.Item("EFlee").GetValue<bool>())
            {
                E.Cast(Game.CursorPos);
                var enemy = enemies.OrderBy(t => t.Distance(Player.Position)).FirstOrDefault();
                if (enemy != null)
                {
                    var minion = ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => m.Distance(enemy.Position) <= Q.Range)
                        .OrderByDescending(m => m.Distance(Player.Position)).FirstOrDefault();

                    if (minion.IsValidTarget() && minion != null)
                    {
                        jumpfar = minion;
                    }
                }

                else if (enemy == null)
                {
                    var minion = ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => m.Distance(Player.Position) <= Q.Range)
                        .OrderByDescending(m => m.Distance(Player.Position)).FirstOrDefault();

                    if (minion.IsValidTarget() && minion != null)
                    {
                        jumpfar = minion;
                    }
                }
            }

            if (Player.AttackRange == 425 && jumpfar.IsValidTarget())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, jumpfar);
            }
            Orbwalking.MoveTo(Game.CursorPos);
        }

        private static void WCC()
       {
           var WCC = Config.Item("WCC").GetValue<bool>();
           if (WCC)
           {
               foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsVisible && !enemy.IsDead && Player.Distance(enemy.Position) <= W.Range && W.IsReady() && Player.Distance(enemy.Position) < W.Range))
               {
                   if (target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Stun))
                   {
                       W.Cast(target.ServerPosition);
                   }
               }
           }
       }

        private static void Killsteal()
       {
           var KS = Config.Item("Killsteal").GetValue<bool>();
           if (KS)
           {
               foreach (var target in HeroManager.Enemies.Where(enemy => enemy.IsVisible && enemy.IsValidTarget() && GetDamageQ(enemy) > enemy.Health && Player.Distance(enemy.Position) <= Q.Range && Q.IsReady()))
               {
                   Q.Cast(target);
               }
           }
       }
            
        private static void RSafe()
        {
            var danger = Config.Item("UseRDangerous").GetValue<bool>();
            if (R.IsReady() && Player.HasBuff("zedulttargetmark") && danger) //stupid idea idk what to do tho
            {
                LeagueSharp.Common.Utility.DelayAction.Add(3500, () => R.Cast());
            }
        }

        private static void Combo()
        {
            var useQ = Config.Item("UseQCombo").GetValue<bool>();
            var useW = Config.Item("UseWCombo").GetValue<bool>();
            var useE = Config.Item("UseECombo").GetValue<bool>();
            var useRKillable = Config.Item("UseRKillable").GetValue<bool>();
            var useRatHP = Config.Item("UseRatHP").GetValue<bool>();
            var HP = Config.Item("HP").GetValue<Slider>();
            var useRAoE = Config.Item("UseRAoE").GetValue<bool>();
            var AoECount = Config.Item("AoECount").GetValue<Slider>();
            var alone = HeroManager.Enemies.Count(scared => scared.Distance(Player.Position) <= 1000);
            var enemyCount = 0;
            var useW2 = Config.Item("UseWCombo2").GetValue<bool>();
            var UseIgnite = Config.Item("UseIgnite").GetValue<bool>();
            if (ghost != null)
            {
                enemyCount += HeroManager.Enemies.Count(enemy => enemy.Distance(ghost.Position) <= 375);
            }

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            var rdelay = R.GetPrediction(target).UnitPosition;

            if (Config.Item("QMode").GetValue<StringList>().SelectedValue == "QE")
            {
                if (useQ && Q.IsReady())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }

                if (useE && E.IsReady())
                {
                    E.Cast(Game.CursorPos);
                }
            }

            else if (Config.Item("QMode").GetValue<StringList>().SelectedValue == "EQ")
            {
                if (useE && E.IsReady())
                {
                    E.Cast(Game.CursorPos);
                }

                if (useQ && Q.IsReady() && !E.IsReady() && !Player.HasBuff("ekkoeattackbuff"))
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);         
                }
            }

            else if (Config.Item("QMode").GetValue<StringList>().SelectedValue == "EQ Hyper Speed (test)")
            {
                if (useE && E.IsReady())
                {
                    E.Cast(Game.CursorPos);
                }

                if (useQ && Q.IsReady() && !E.IsReady() && Player.HasBuff("ekkoeattackbuff"))
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }

                else if (useQ && Q.IsReady() && !E.IsReady())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }
            }


            if (useRKillable && R.IsReady() && useW2)
            {
                if (rdelay.Distance(ghost.Position) <= R.Range)
                {
                    if (ComboDamage(target) >= target.Health && Player.Distance(ghost.Position) < W.Range)
                    {
                        W.Cast(ghost.Position);
                        R.Cast();
                    }
                }
            }

            else if (useRKillable && R.IsReady())
            {
                if (rdelay.Distance(ghost.Position) <= R.Range)
                {
                    if (ComboDamage(target) >= target.Health)
                    {
                        R.Cast();
                    }
                }
            }

            if (useRatHP && R.IsReady())
            {
                if (Player.HealthPercent <= HP.Value && alone >= 1)
                {
                    R.Cast();
                }
            }

            if (useRAoE && R.IsReady() && enemyCount >= AoECount.Value && useW && W.IsReady() && Player.Distance(ghost.Position) < W.Range)
            {
                W.Cast(ghost.Position);
                R.Cast();
            }

            else if (useRAoE && R.IsReady() && enemyCount >= AoECount.Value)
            {
                R.Cast();
            }

            if (Player.Distance(target.ServerPosition) <= 600 && ComboDamage(target) >= target.Health && UseIgnite)
            {
                Player.Spellbook.CastSpell(ignite, target);
            }
        }

        private static void Harass()
        {

            var mana = Config.Item("HarassMana").GetValue<Slider>().Value;
            var useQ = Config.Item("UseQHarass").GetValue<bool>();
            var useE = Config.Item("UseEHarass").GetValue<bool>();

            if (Player.ManaPercent < mana)
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            if (useQ && Q.IsReady())
            {
                Q.Cast(target);
            }

            if (useE && E.IsReady())
            {
                E.Cast(Game.CursorPos);
            }
        }

        private static Obj_AI_Base ghost
        {
            get
            {
                return
                ObjectManager.Get<Obj_AI_Base>()
                                .FirstOrDefault(ghost => !ghost.IsEnemy && ghost.Name.Contains("Ekko"));
            }
        }


        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            var enemy = TargetSelector.GetTarget(E.Range + 425, TargetSelector.DamageType.Magical);
            var userdie = Config.Item("UseRifDie").GetValue<bool>();
            var danger = Config.Item("UseRDangerous").GetValue<bool>();        

            if (sender.IsMe && args.SData.Name == "EkkoE")
            {
                // make sure orbwalker doesnt mess up after casting E

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (enemy == null)
                        return;

                    LeagueSharp.Common.Utility.DelayAction.Add((int)(Math.Ceiling(Game.Ping / 2f) + 350), () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy));
                }     
            }

            if (args.Target == null)
                return;

            if (R.IsReady() && args.End.Distance(Player.Position) < 150 && args.SData.Name == "ViR" && danger)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(250, () => R.Cast());
            }

            if (R.IsReady() && sender.IsEnemy && args.Target.IsMe && userdie)
            {
                var dmg = sender.GetDamageSpell(Player, args.SData.Name);
                if (dmg.CalculatedDamage >= Player.Health - 50)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(0, () => R.Cast());
                }
            }
        }

       

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var drawQ = Config.Item("drawQ").GetValue<Circle>();
            var drawW = Config.Item("drawW").GetValue<Circle>();
            var drawE = Config.Item("drawE").GetValue<Circle>();
            var drawPassive = Config.Item("drawPassiveStacks").GetValue<Circle>();
            var drawGhost = Config.Item("drawGhost").GetValue<Circle>();


            if (drawQ.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, drawQ.Color);
            }
            else if (drawQ.Active && !Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Maroon);
            }

            if (drawW.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, drawW.Color);
            }

            if (drawE.Active && E.IsReady() || Player.Spellbook.GetSpell(SpellSlot.E).State == SpellState.Surpressed)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, drawE.Color);
            }

            else if (drawE.Active && !E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Maroon);
            }

            if (drawPassive.Active)
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.Buffs.Any(buff1 => buff1.Name == "EkkoStacks" && buff1.Count == 2) && enemy.IsVisible)
                    {
                        var enemypos = Drawing.WorldToScreen(enemy.Position);
                        Render.Circle.DrawCircle(enemy.Position, 150, Color.Red);
                        Drawing.DrawText(enemypos.X, enemypos.Y + 15, Color.Red, "2 Stacks");
                    }

                    else if (enemy.Buffs.Any(buff1 => buff1.Name == "EkkoStacks" && buff1.Count == 1) && enemy.IsVisible)
                    {
                        var enemypos = Drawing.WorldToScreen(enemy.Position);
                        Render.Circle.DrawCircle(enemy.Position, 150, drawPassive.Color);
                        Drawing.DrawText(enemypos.X, enemypos.Y + 15, drawPassive.Color, "1 Stack");
                    }
                }
            }

            if (drawGhost.Active && ghost != null)
            {
                if (R.IsReady())
                {
                    Render.Circle.DrawCircle(ghost.Position, R.Range, drawGhost.Color);
                }
            }
        }
    }
}