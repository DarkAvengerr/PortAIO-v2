using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WindWalker_Cho._._.gath
{
    internal class Program
    {
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static Orbwalking.Orbwalker Orbwalker;
        private static Spell Q, W, E, R;
        private static Menu Menu;
        private static AIHeroClient target;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Chogath")
                return;

            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 500f);
            R = new Spell(SpellSlot.R, 175f, TargetSelector.DamageType.True);

            Q.SetSkillshot(1200f, 250f, 1300, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 60f, float.MaxValue, false, SkillshotType.SkillshotCone);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));

            TargetSelector.AddToMenu(ts);
            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));

            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));

            Menu farmMenu = Menu.AddSubMenu(new Menu("Farm", "Farm"));
            farmMenu.AddItem(new MenuItem("qFarm", "Cast Q if >= minions hit").SetValue(new Slider(3, 1, 8)));
            farmMenu.AddItem(new MenuItem("wFarm", "Cast W if >= minions hit").SetValue(new Slider(4, 1, 15)));

            Menu Drawings = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            Drawings.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            Drawings.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            Drawings.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));

            
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;

            Chat.Print("Dooooooooooooooooooope soap2");
            Menu.AddToMainMenu();
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && W.LSIsReady() && sender.LSDistance(Player) <= W.Range)
            {
                W.Cast(sender);
            }
            else
            {
                if (sender.IsEnemy && Q.LSIsReady() && sender.LSDistance(Player) <= Q.Range)
                {
                    Q.Cast(sender);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target.IsValid)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    CastQ();
                    CastW();
                    CastR();

                    checkE(true);
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                farmQ();
                farmW();

                checkE(true);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                checkE(false);
            }
        }

        private static void checkE(bool shouldBeOn)
        {
            if (shouldBeOn)
            {
                if (!Player.LSHasBuff("VorpalSpikes", true))
                {
                    E.Cast();
                }
            }
            else
            {
                if (Player.LSHasBuff("VorpalSpikes", true))
                {
                    E.Cast();
                }
            }
        }

        private static void farmQ()
        {
            if (Q.LSIsReady())
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.LSDistance(Player) <= Q.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.LSDistance(enemyMinion) <= 185)
                            .Count();
                    if (enemyMinionsInRange >= Menu.Item("qFarm").GetValue<Slider>().Value)
                    {
                        Q.Cast(enemyMinion);
                    }
                }
            }
        }

        private static void farmW()
        {
            if (W.LSIsReady())
            {
                var minionPos =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.IsEnemy && x.LSDistance(Player) <= W.Range)
                        .Select(y => y.Position.LSTo2D())
                        .ToList();
                //foreach (
                //    var enemyMinion in
                //        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.LSDistance(Player) <= W.Range))
                //{
                var wFarmPred = MinionManager.GetBestCircularFarmLocation(minionPos, W.Width, W.Range);
                if (wFarmPred.MinionsHit >= Menu.Item("wFarm").GetValue<Slider>().Value)
                    //MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All).Count > Menu.Item("wFarm").GetValue<Slider>().Value)
                {
                    W.Cast(wFarmPred.Position);
                }
                //}  
            }
        }

        private static void CastQ()
        {
            if (Q.LSIsReady())
            {
                var qPrediction = Q.GetPrediction(target);

                if (qPrediction.Hitchance == HitChance.Immobile || qPrediction.Hitchance == HitChance.VeryHigh ||
                    qPrediction.Hitchance == HitChance.Dashing)
                {
                    Q.Cast(target, false, true);
                }
            }
        }

        private static void CastW()
        {
            if (W.LSIsReady())
            {
                W.Cast(target);
            }
        }

        private static void CastR()
        {
            if (R.LSIsReady())
            {
                if (Player.LSGetSpellDamage(target, SpellSlot.R, 0) > target.Health)
                {
                    R.Cast(target);
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (target != null && target.IsValid)
            {
                var qPrediction = Q.GetPrediction(target);
                var y = Drawing.WorldToScreen(target.Position);
                Drawing.DrawText(y[0], y[1], System.Drawing.Color.Red, qPrediction.Hitchance.ToString());
            }

            if (Q.LSIsReady() && Menu.Item("drawQ").GetValue<bool>())
            {
                Drawing.DrawCircle(Player.Position, Q.Range, Color.Green);
            }
            else
            {
                if (Menu.Item("drawQ").GetValue<bool>())
                    Drawing.DrawCircle(Player.Position, Q.Range, Color.Red);
            }

            if (W.LSIsReady() && Menu.Item("drawW").GetValue<bool>())
            {
                Drawing.DrawCircle(Player.Position, W.Range, Color.Green);
            }
            else
            {
                if (Menu.Item("drawW").GetValue<bool>())
                    Drawing.DrawCircle(Player.Position, W.Range, Color.Red);
            }
            
            if (R.LSIsReady() && Menu.Item("drawR").GetValue<bool>())
            {
                Drawing.DrawCircle(Player.Position, R.Range, Color.Green);
            }
            else
            {
                if (Menu.Item("drawR").GetValue<bool>())
                    Drawing.DrawCircle(Player.Position, R.Range, Color.Red);
            }
        }
    }
}