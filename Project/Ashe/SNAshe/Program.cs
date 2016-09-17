using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SNAshe
{
    class Program
    {
        private static AIHeroClient myPlayer { get { return ObjectManager.Player; } }
        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker orb;
        private static Menu menu;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += onGameLoad;
        }

        private static void onGameLoad(EventArgs args)
        {
            if (myPlayer.ChampionName != "Ashe")
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1200);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            W.SetSkillshot(0.5f, 100, 902, true, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.5f, 100, 1600, false, SkillshotType.SkillshotLine);

            menu = new Menu(myPlayer.ChampionName, myPlayer.ChampionName, true);

            Menu ts = new Menu("Target Selector", "ts");
            menu.AddSubMenu(ts);

            TargetSelector.AddToMenu(ts);
            Menu orbwalker = new Menu("Orbwalker", "orbwalk");

            menu.AddSubMenu(orbwalker);
            orb = new Orbwalking.Orbwalker(orbwalker);

            menu.AddSubMenu(new Menu("Drawings", "drawings", false));
            menu.SubMenu("drawings").AddItem(new MenuItem("drawW", "Draw W").SetValue(true));

            menu.AddSubMenu(new Menu("Harass", "harass", false));
            menu.SubMenu("harass").AddItem(new MenuItem("hUseW", "Use W").SetValue(true));
            menu.SubMenu("harass").AddItem(new MenuItem("hActive", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));

            menu.AddSubMenu(new Menu("Combo", "combo", false));
            menu.SubMenu("combo").AddItem(new MenuItem("cUseQ", "Use Q").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("cUseW", "Use W").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("cUseR", "Use R").SetValue(true));
            menu.SubMenu("combo").AddItem(new MenuItem("cActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            menu.AddSubMenu(new Menu("Lane Clear", "laneclear", false));
            menu.SubMenu("laneclear").AddItem(new MenuItem("lcUseW", "Use W").SetValue(true));
            menu.SubMenu("laneclear").AddItem(new MenuItem("lcActive", "Lane Clear").SetValue(new KeyBind('V', KeyBindType.Press)));

            menu.AddSubMenu(new Menu("Last Hit", "lasthit", false));
            menu.SubMenu("lasthit").AddItem(new MenuItem("lhActive", "Last Hit").SetValue(new KeyBind('X', KeyBindType.Press)));

            menu.AddSubMenu(new Menu("Misc", "misc", false));

            menu.AddToMainMenu();

            Game.OnUpdate += onGameUpdate;
            Drawing.OnDraw += onDraw;
        }

        private static void onGameUpdate(EventArgs args)
        {
            if (myPlayer.IsDead)
                return;

            if (orb.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();

            }

            if (orb.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }

            if (orb.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget())
            {
                if (menu.Item("cUseQ").GetValue<bool>() && Q.IsReady())
                {
                    if (myPlayer.GetBuffCount("AsheQ") >= 5 || myPlayer.GetBuffCount("asheqcastready") >= 5)
                    {
                        Q.Cast();
                    }
                }

                if (menu.Item("cUseW").GetValue<bool>() && W.IsReady())
                    W.Cast(target);

                if (menu.Item("cUseR").GetValue<bool>() && R.IsReady())
                {
                    var waypoints = target.GetWaypoints();
                    if ((myPlayer.Distance(waypoints.Last().To3D()) - myPlayer.Distance(target.Position)) > 400)
                    {
                        R.Cast(target);
                    }
                    else if (myPlayer.Distance(target.Position) < 2500 && R.IsKillable(target))
                    {
                        R.Cast(target);
                    }
                }
            }

        }

        public static void Harass()
        {

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget())
            {
                if (menu.Item("hUseW").GetValue<bool>())
                    W.Cast(target);
            }
        }

        public static void LaneClear()
        {
           

        }

        public static void onDraw(EventArgs args)
        {
            if (!myPlayer.IsDead && W.Level > 0 && W.IsReady() && menu.Item("drawW").GetValue<bool>())
            {
                Render.Circle.DrawCircle(myPlayer.Position, W.Range, System.Drawing.Color.Blue);
            }
        }
    
    }
}
