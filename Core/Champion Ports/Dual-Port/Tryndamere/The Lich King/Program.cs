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
            Game.OnUpdate += Game_OnGameUpdate;
            Chat.Print("I'm a DK");
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            {
                target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical) ?? null;
                RunNigger();
                if (target != null)
                {
                    if (target.IsValidTarget() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        MockingShout();
                    }
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
            if (Q.IsReady() && (Player.Health / Player.MaxHealth) * 100 < Menu.Item("qBelow").GetValue<Slider>().Value && !Player.HasBuff("UndyingRage", true))
            {
                if (ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.Distance(Player.Position) <= 1200).Count() > 0)
                {
                    Q.Cast();
                }
            }
        }

        private static void MockingShout()
        {
            if (W.IsReady() && !target.IsFacing(Player))
            {
                W.Cast();
            }
        }

        private static void UndyingRage()
        {
            if (R.IsReady() && ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.Distance(Player.Position) <= 1200).Count() > 0 && (Player.Health / Player.MaxHealth) * 100 < Menu.Item("rBelow").GetValue<Slider>().Value)
            {
                R.Cast();
            }
        }
    }
}
