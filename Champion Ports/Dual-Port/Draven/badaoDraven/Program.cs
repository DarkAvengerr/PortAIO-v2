using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoDraven
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Orbwalking.Orbwalker Orbwalker;

        public static Spell Q, W, E, R;

        public static Menu Menu;

        public static List<Riu> Riu = new List<Riu>();

        public static RiuNo1 RiuNo1 = null;

        public static float Rcount;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Draven")
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E,1050);
            R = new Spell(SpellSlot.R);
            E.SetSkillshot(250, 130, 1400, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(400, 160, 2000, false, SkillshotType.SkillshotLine);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);
            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));

            Menu Harass = spellMenu.AddSubMenu(new Menu("Harass", "Harass"));

            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));

            Menu LaneClear = spellMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));

            Menu JungClear = spellMenu.AddSubMenu(new Menu("JungClear", "JungClear"));

            Combo.AddItem(new MenuItem("Use Q Combo", "Use Q Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use W Combo", "Use W Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use W to gap Combo", "Use W to gap Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use E Combo", "Use E Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use R Combo", "Use R Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use R Return Combo", "Use R Return Combo").SetValue(true));
            Combo.AddItem(new MenuItem("Use R If EnemyHP below", "Use R If EnemyHP below").SetValue(new Slider(50, 0, 100)));
            Harass.AddItem(new MenuItem("Use Q Harass", "Use Q Harass").SetValue(true));
            Harass.AddItem(new MenuItem("Use W Harass", "Use W Harass").SetValue(true));
            Harass.AddItem(new MenuItem("Use W to gap Harass", "Use W to gap Harass").SetValue(true));
            Harass.AddItem(new MenuItem("Use E Harass", "Use E Harass").SetValue(true));
            Harass.AddItem(new MenuItem("minimum Mana HR", "minimum Mana HR").SetValue(new Slider(40, 0, 100)));
            LaneClear.AddItem(new MenuItem("Use Q LaneClear", "Use Q LaneClear").SetValue(true));
            LaneClear.AddItem(new MenuItem("Use W LaneClear", "Use W LaneClear").SetValue(true));
            LaneClear.AddItem(new MenuItem("minimum Mana LC", "minimum Mana LC").SetValue(new Slider(40, 0, 100)));
            JungClear.AddItem(new MenuItem("Use Q JungClear", "Use Q LaneClear").SetValue(true));
            JungClear.AddItem(new MenuItem("Use W JungClear", "Use W LaneClear").SetValue(true));
            JungClear.AddItem(new MenuItem("minimum Mana JC", "minimum Mana JC").SetValue(new Slider(40, 0, 100)));
            spellMenu.AddItem(new MenuItem("Use E on gapcloser", "Use E on gapcloser").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use E to interrupt", "Use E to interrupt").SetValue(true));
            spellMenu.AddItem(new MenuItem("LeftClick Remove Catching Axe", "LeftClick Remove Catching Axe").SetValue(true));
            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            //Orbwalking.AfterAttack += AfterAttack;
            //Orbwalking.OnAttack += OnAttack;
            Obj_AI_Base.OnProcessSpellCast += oncast;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Interrupter2.OnInterruptableTarget += interrupt;
            AntiGapcloser.OnEnemyGapcloser += gapcloser;
            Game.OnWndProc += onclick;
            Drawing.OnDraw += Drawing_OnDraw;
            //Missile.OnCreate += Missle;
            Chat.Print("Welcome to Draven World");
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            //if (RiuNo1 != null)
            //    Render.Circle.DrawCircle(RiuNo1.Position, 200, Color.Red);
        }
        public static void onclick(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }
            if(Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && Menu.Item("LeftClick Remove Catching Axe").GetValue<bool>())
            { foreach (var riu in Program.Riu) { if (riu.NetworkId == Program.RiuNo1.NetworkId) { Program.Riu.Remove(riu); } } }
        }
        public static void interrupt (AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && E.IsReady() && sender.IsValidTarget() && !sender.IsZombie && Menu.Item("Use E to interrupt").GetValue<bool>())
            {
                var prediction = E.GetPrediction(sender);
                if (prediction.Hitchance >= HitChance.Medium){ E.Cast(sender); }
            }
        }
        public static void gapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;
            if (target.IsEnemy && E.IsReady() && target.IsValidTarget() && !target.IsZombie && Menu.Item("Use E on gapcloser").GetValue<bool>())
            {
                var prediction = E.GetPrediction(target);
                if (prediction.Hitchance >= HitChance.Medium) { E.Cast(target); }
            }
        }

        public static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (/* args.SData.Name.ToLower().Contains("basicattack") || args.SData.Name.ToLower().Contains("critattack") ||*/ args.SData.Name.ToLower().Contains("dravenrcast"))
            {
                Rcount = Utils.GameTimeTickCount;
                args.SData.IsAutoAttack();
            }
        }
        public static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("q_reticle_self")) {Riu.Add(new Riu(sender, Utils.GameTimeTickCount, sender.Position, sender.NetworkId)); }
        }
        public static void OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("q_reticle_self")) {foreach (var riu in Riu) { if (riu.NetworkId == sender.NetworkId) { Riu.Remove(riu); } } }
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            RiuNo1 = RiuNo1.RiuSo1();
            //LuomRiu.LuomRiuTest();
            LuomRiu.lumriu();
            //checkbuff();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo.useCombo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Combo.Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Combo.LaneClear();
                Combo.JungClear();
            }
        }

        public static void checkbuff()
        {
            String temp = "";
            foreach (var buff in Player.Buffs)
            {
                    temp += (buff.Name + "(" + buff.Count + ")" + ", ");
            }
            Chat.Print(temp);
        }
    }

}
