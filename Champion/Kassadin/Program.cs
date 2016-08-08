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

namespace PainInMyKass
{
    internal class Program
    {
        public static Menu Menu { get; set; }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, R;

        private static int Combo;

        private static int Mixed;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Kassadin")
                return;

            Q = new Spell(SpellSlot.Q, 650f);
            W = new Spell(SpellSlot.W, 150f);
            E = new Spell(SpellSlot.E, 400f);
            R = new Spell(SpellSlot.R, 500f);

            Q.SetTargetted(0.5f, 1400f);
            E.SetSkillshot(0.5f, 10f, float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.5f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);

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

            Menu comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("dontR", "Don't R if >= X Enemies").SetValue(new Slider(3, 1, 5)));
            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Menu mixedMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            mixedMenu.AddItem(new MenuItem("useQHarass", "Use Q").SetValue(true));
            mixedMenu.AddItem(new MenuItem("Harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));

            Menu fleeMenu = Menu.AddSubMenu(new Menu("flee", "Flee"));
            fleeMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            fleeMenu.AddItem(new MenuItem("flee", "Flee").SetValue(new KeyBind('T', KeyBindType.Press)));

            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
           // Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && sender.LSDistance(Player) < Q.Range)
            {
                Q.CastOnUnit(sender); 
            }
        }

        //static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        //{
        //    if (sender is AIHeroClient && sender.IsEnemy && sender.Distance(Player) < Q.Range)
        //    {
        //        Console.WriteLine("Spell Name: {0}  Channel Duration: {1}",args.SData.Name,args.SData.c);
        //        if(args.SData.ChannelDuration > .35)
        //        {
        //            Q.CastOnUnit(sender);
        //        }
        //    }
        //}


       
        private static void Flee()
        {
            if (!Menu.Item("useR").GetValue<bool>())
                return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (R.LSIsReady())
            {
                R.Cast(Game.CursorPos);
            }
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Nullsphere();
                Netherblade();
                Forcepulse();
                RiftWalk();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Nullsphere();
                Netherblade();
                Forcepulse();
            }

            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                Flee();
            }
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Q.LSIsReady())
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, 650f, Color.Green);
            }
            else
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, 650f, Color.Crimson);
            }
            if (R.LSIsReady())
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, Color.Green);
            }
            else
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, Color.Crimson);
            }
        }

        private static void Nullsphere()
        {
            if (!Menu.Item("useQ").GetValue<bool>())
                return;         

            if (Q.LSIsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(650f, TargetSelector.DamageType.Magical);

                if (target.LSIsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(target);
                }
            }
        }


        private static void Netherblade()
        {
            if (!Menu.Item("useW").GetValue<bool>())
                return;

            if (W.LSIsReady())
            {
                int enemies = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.LSDistance(Player, false) < 300).Count();

                if (enemies > 0)
                {
                    W.Cast();
                }
            }
        }


        private static void Forcepulse()
        {
            if (!Menu.Item("useE").GetValue<bool>())
                return;   

            if (E.LSIsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(400f, TargetSelector.DamageType.Magical);

                if (target.LSIsValidTarget(E.Range))
                {
                    E.Cast(target.Position);
                }
            }
        }

        private static void RiftWalk()
        {
            if (!Menu.Item("useR").GetValue<bool>())
                return;

            if (R.LSIsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(500f, TargetSelector.DamageType.Magical);

                var extraEnemies =
                    ObjectManager.Get<AIHeroClient>().Where(x => x != target && x.IsEnemy && x.LSDistance(target) < 800).Count();
                if (target.LSIsValidTarget(R.Range) && extraEnemies < Menu.Item("dontR").GetValue<Slider>().Value)
                {
                    R.Cast(target.Position);
                }
            }          
        }
    }
}
    
            

 




