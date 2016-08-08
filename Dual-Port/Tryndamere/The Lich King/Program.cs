using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheLichKing
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Orbwalking.Orbwalker Orbwalker;
        private static Spell Q, W, E, R;
        private static Menu Menu;
        private static AIHeroClient target;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Tryndamere")
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 400f);
            E = new Spell(SpellSlot.E, 660f);
            R = new Spell(SpellSlot.R);

            E.SetSkillshot(0f, 93f, 600, false, SkillshotType.SkillshotLine);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);
            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));

            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("qBelow", "Q below % hp").SetValue(new Slider(20, 0, 95)));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "Use R Before Death").SetValue(true));
            spellMenu.AddItem(new MenuItem("rBelow", "R below % hp").SetValue(new Slider(15, 0, 95)));

            Menu fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Run Nigger"));
            fleeMenu.AddItem(new MenuItem("useEFlee", "Use E")).SetValue(new KeyBind('Z', KeyBindType.Press));

            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Chat.Print("I'm a DK");
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            {


                target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                RunNigger();
                if (target.IsValid && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    MockingShout();
                    
                }
                UndyingRage();
                Bloodlust();
            }
        }

        private static void RunNigger()
        {
            if (Menu.Item("useEFlee").GetValue<KeyBind>().Active)
            {
                E.Cast(Game.CursorPos);
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static void Bloodlust()
        {
            if (Q.LSIsReady() && (Player.Health / Player.MaxHealth) * 100 < Menu.Item("qBelow").GetValue<Slider>().Value && !Player.LSHasBuff("UndyingRage",true))
            {
                if (ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.LSDistance(Player.Position) <= 1200).Count() > 0)
                {
                    Q.Cast();
                }
            }
        }

        private static void MockingShout()
        {
            if (W.LSIsReady() && !target.LSIsFacing(Player))
            {
                W.Cast();
            }
        }

        private static void SpinningSlash()
        {
            if (E.LSIsReady() && Player.LSDistance(target) <= E.Range - 125)
            {      
                //var extendPos = target.ServerPosition.LSTo2D().LSExtend(target.ServerPosition.LSTo2D(), 200).To3D();       

                E.Cast(target);
            }
        }

        private static void UndyingRage()
        {
            if (R.LSIsReady() && ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.LSDistance(Player.Position) <= 1200).Count() > 0 && (Player.Health / Player.MaxHealth) * 100 < Menu.Item("rBelow").GetValue<Slider>().Value)
            {
                R.Cast();
            }
        }
    }
}
