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
 namespace Nautilus_Is_Meme
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Orbwalking.Orbwalker Orbwalker;
        private static Spell Q, W, E, R;
        private static Menu Menu;
        private static AIHeroClient target;
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Nautilus")
                return;

            Q = new Spell(SpellSlot.Q, 1100f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 300f);
            R = new Spell(SpellSlot.R, 825f);

            Q.SetSkillshot(.25f, 90f, 1600f, true, SkillshotType.SkillshotLine);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);
            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));

            spellMenu.AddItem(new MenuItem("useQ", "use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "use R").SetValue(true));

            Menu Drawings = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            Drawings.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            Drawings.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            Drawings.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Chat.Print("Nautilus the Memer Lord");

            var lel = Menu.AddSubMenu((new Menu("Dont use R on", "DontR")));

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                lel.AddItem(new MenuItem("DontR" + enemy.ChampionName, enemy.ChampionName).SetValue(false));

            Menu.AddToMainMenu();
        }

        static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            target = TargetSelector.GetTarget(1100f, TargetSelector.DamageType.Magical);

            if (target.IsValid)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    CastQ();
                    CastW();
                    CastE();
                    CastR();
                }
            }
        }

        private static void CastQ()
        {
            var hitchance = Q.GetPrediction(target, false, 0, new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.Walls, CollisionableObjects.YasuoWall }).Hitchance;
            if (hitchance == HitChance.High || hitchance == HitChance.Immobile || hitchance == HitChance.Dashing ||
                hitchance == HitChance.VeryHigh && !target.IsStunned && !target.CanMove)
            {
                Q.Cast(target);
            }
        }

        private static void CastW()
        {
            if (W.IsReady() && ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.Distance(Player) <= 1000).Count() > 0)
            {
                W.Cast();
            }
        }

        private static void CastE()
        {
            if (E.IsReady() && target.Distance(Player) <= E.Range)
            {
                E.Cast();
            }
        }

        private static void CastR()
        {
            if (R.IsReady() && target.Distance(Player) <= R.Range && !target.IsStunned && !target.CanMove)
            {
                var useR = (Menu.Item("DontR" + target.ChampionName) != null &&
                            Menu.Item("DontR" + target.ChampionName).GetValue<bool>() == false);
                if (useR)
                {
                    R.CastOnUnit(target);
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (R.IsReady() && Menu.Item("drawR").GetValue<bool>())
            {
                Drawing.DrawCircle(Player.Position, R.Range, Color.Green);
            }
            else
            {
                if (Menu.Item("drawR").GetValue<bool>())
                    Drawing.DrawCircle(Player.Position, R.Range, Color.Red);
            }
            if (Q.IsReady() && Menu.Item("drawQ").GetValue<bool>())
            {
                Drawing.DrawCircle(Player.Position, Q.Range, Color.Green);
            }
            else
            {
                if (Menu.Item("drawQ").GetValue<bool>())
                    Drawing.DrawCircle(Player.Position, Q.Range, Color.Red);
            }
            if (E.IsReady() && Menu.Item("drawE").GetValue<bool>())
            {
                Drawing.DrawCircle(Player.Position, E.Range, Color.Green);
            }
            else
            {
                if (Menu.Item("drawE").GetValue<bool>())
                    Drawing.DrawCircle(Player.Position, E.Range, Color.Red);
            }
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && sender.Distance(Player) <= Q.Range)
            {
                var hitchance = Q.GetPrediction(target, false, 0, new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.Walls, CollisionableObjects.YasuoWall }).Hitchance;
                if (hitchance == HitChance.High || hitchance == HitChance.Immobile || hitchance == HitChance.Dashing ||
                    hitchance == HitChance.VeryHigh)
                {
                    Q.Cast(target);
                }
            }
        }
    }
}