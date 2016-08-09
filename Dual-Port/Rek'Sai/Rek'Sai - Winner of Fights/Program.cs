using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RekSai
{
    class Program
    {
        public static string cName = "RekSai";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        public static Menu Config;
        private static AIHeroClient Player = ObjectManager.Player;

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Q2;
        public static Spell E2;

        public static float rangeQ;
        public static float rangeW;
        public static float rangeE;
        private static bool burrowed;
        private static bool unburrowed;

        public static void Game_OnGameLoad()
        {
            //Activator Start
            Activator.Hydra.hikiHydra = true;
            Activator.Potion.hikiPotion = true;
            Activator.Randuin.hikiRanduin = true;
            Activator.Solari.hikiSolari = true;
            //Activator Finish

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 250);
            R = new Spell(SpellSlot.R);
            Q2 = new Spell(SpellSlot.Q, 1450);
            E2 = new Spell(SpellSlot.E, 500);


            Q2.SetSkillshot(0.5f, 60, 1950, true, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
            SpellList.Add(Q2);
            SpellList.Add(E2);

            Config = new Menu("Rek'Sai - Winner of Fights", "Rek'Sai - Winner of Fights", true);
            TargetSelector.AddToMenu(Config.SubMenu("Target Selector Settings"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("q.combo", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("w.combo", "Use W [Auto Switch]").SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("q.combo.burrowed", "Use Q [Burrowed]").SetValue(true));
                comboMenu.AddItem(new MenuItem("e.combo.burrowed", "Use E [Burrowed]").SetValue(true));
                Config.AddSubMenu(comboMenu);
            }
            var harassMenu = new Menu("Harass Settings", "Harass Settings");
            {
                harassMenu.AddItem(new MenuItem("e.harass", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("q.harass.burrowed", "Use Q [Burrowed]").SetValue(true));
                Config.AddSubMenu(harassMenu);
            }
            var jungleMenu = new Menu("Jungle Clear Settings", "Jungle Clear Settings");
            {
                jungleMenu.AddItem(new MenuItem("q.jungle", "Use Q").SetValue(true));
                jungleMenu.AddItem(new MenuItem("w.jungle", "Use W [Auto Switch]").SetValue(true));
                jungleMenu.AddItem(new MenuItem("e.jungle", "Use E").SetValue(true));
                jungleMenu.AddItem(new MenuItem("q.jungle.burrowed", "Use Q [Burrowed]").SetValue(true));

                Config.AddSubMenu(jungleMenu);
            }
            var furyMenu = new Menu("Fury Settings", "Fury Settings");
            {
                furyMenu.AddItem(new MenuItem("saveReksai", "                  Save RekSai Settings"));
                furyMenu.AddItem(new MenuItem("protect.reksai", "Protect Rek'Sai with [W]").SetValue(true));
                furyMenu.AddItem(new MenuItem("protect.reksai.hp", "If RekSai HP <=").SetValue(new Slider(10, 0, 100)));
                furyMenu.AddItem(new MenuItem("protect.reksai.fury", "If Reksai Fury >= ").SetValue(new Slider(60, 0, 100)));
                Config.AddSubMenu(furyMenu);
            }
            var hikiActivator = new Menu("Hiki Activator", "Hiki Activator");
            {
                var randuinMenu = new Menu("Randuin Settings", "Randuin Settings");
                {
                    randuinMenu.AddItem(new MenuItem("use.randuin", "Use Randuin").SetValue(true));
                    randuinMenu.AddItem(new MenuItem("randuin.count", "If Enemy Count >=").SetValue(new Slider(2, 1, 5)));
                    hikiActivator.AddSubMenu(randuinMenu);
                }
                var hydraMenu = new Menu("Hydra - Tiamat Settings", "Hydra - Tiamat Settings");
                {
                    hydraMenu.AddItem(new MenuItem("use.hydra", "Use Hydra").SetValue(true));
                    hydraMenu.AddItem(new MenuItem("use.tiamat", "Use Tiamat").SetValue(true));
                    hikiActivator.AddSubMenu(hydraMenu);
                }
                var solariMenu = new Menu("Iron Solari Settings", "Iron Solari Settings");
                {
                    solariMenu.AddItem(new MenuItem("use.solari", "Use Iron Solari").SetValue(true));
                    solariMenu.AddItem(new MenuItem("solari.ally.hp", "If Ally Hp <= %").SetValue(new Slider(20, 0, 100)));
                    hikiActivator.AddSubMenu(solariMenu);
                }
                var healthMenu = new Menu("Health Potion Settings", "Health Potion Settings");
                {
                    healthMenu.AddItem(new MenuItem("useHealth", "Use Health Potion").SetValue(true));
                    healthMenu.AddItem(new MenuItem("myhp", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    hikiActivator.AddSubMenu(healthMenu);
                }
                var manaMenu = new Menu("Mana Potion Settings", "Mana Potion Settings");
                {
                    manaMenu.AddItem(new MenuItem("useMana", "Use Mana Potion").SetValue(true));
                    manaMenu.AddItem(new MenuItem("mymana", "Use if my mana < %").SetValue(new Slider(20, 0, 100)));
                    hikiActivator.AddSubMenu(manaMenu);
                }
                Config.AddSubMenu(hikiActivator);
            }
            var drawMenu = new Menu("Draw Settings", "Draw Settings");
            {
                drawMenu.AddItem(new MenuItem("qDraw", "Q Range").SetValue(new Circle(true, Color.White)));
                drawMenu.AddItem(new MenuItem("wDraw", "W Range").SetValue(new Circle(true, Color.Silver)));
                drawMenu.AddItem(new MenuItem("eDraw", "E Range").SetValue(new Circle(true, Color.Yellow)));
                drawMenu.AddItem(new MenuItem("rDraw", "R Range").SetValue(new Circle(true, Color.Gold)));
                Config.AddSubMenu(drawMenu);
            }
            Config.AddToMainMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }
        private static void burrowCheck()
        {
            if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "reksaiwburrowed" ||
                Player.Spellbook.GetSpell(SpellSlot.Q).Name == "reksaiqburrowed" ||
                Player.Spellbook.GetSpell(SpellSlot.Q).Name == "reksaieburrowed")
            {
                burrowed = true;
                unburrowed = false;
                rangeQ = Q2.Range;
                rangeW = W.Range;
                rangeE = E2.Range;
            }
            if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "RekSaiW" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "RekSaiQ" ||
                Player.Spellbook.GetSpell(SpellSlot.W).Name == "reksaie")
            {
                burrowed = false;
                unburrowed = true;
                rangeQ = Q.Range;
                rangeW = W.Range;
                rangeE = E.Range;
            }
        } // RDY
        private static void Game_OnGameUpdate(EventArgs args)
        {
            burrowCheck();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Jungle();
            }
            furyManager();
        } // RDY
        private static void Combo()
        {
            var useQ = Config.Item("q.combo").GetValue<bool>();
            var useE = Config.Item("e.combo").GetValue<bool>();
            var autoSwitch = Config.Item("w.combo").GetValue<bool>();
            var useQ2 = Config.Item("q.combo.burrowed").GetValue<bool>();
            var useE2 = Config.Item("e.combo.burrowed").GetValue<bool>();
            if (burrowed)
            {
                if (Q2.IsReady() && useQ2)
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsZombie && x.IsValidTarget(Q2.Range)))
                    {
                        if (Q2.GetPrediction(enemy).Hitchance >= HitChance.High)
                        {
                            Q2.Cast(enemy.Position);
                        }
                    }
                }
                if (E2.IsReady() && useE2)
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsZombie && x.IsValidTarget(E2.Range)))
                    {
                        E.Cast(enemy.Position - 50);
                    }
                }
                if (W.IsReady() && !Q2.IsReady() && !E2.IsReady() && autoSwitch) // Auto Switch
                {
                    W.Cast();
                }
            }
            if (unburrowed)
            {
                if (Q.IsReady() && useQ)
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsZombie))
                    {
                        if (Player.Distance(enemy.Position) < E.Range)
                        {
                            Q.Cast();
                        }
                    }
                }
                if (E.IsReady() && useE)
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsZombie && x.IsValidTarget(E.Range)))
                    {
                        E.Cast(enemy);
                    }
                }
                if (W.IsReady() && !Q.IsReady() && !E.IsReady() && autoSwitch) // Auto Switch
                {
                    W.Cast();
                }
            }
        }// RDY
        private static void Harass()
        {
            var useE = Config.Item("e.harass").GetValue<bool>();
            var useQ = Config.Item("q.harass.burrowed").GetValue<bool>();

            if (burrowed)
            {
                if (Q2.IsReady() && useQ)
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsZombie && x.IsValidTarget(Q2.Range)))
                    {
                        if (Q2.GetPrediction(enemy).Hitchance >= HitChance.High)
                        {
                            Q2.Cast(enemy.Position);
                        }
                    }
                }
            }
            if (unburrowed)
            {
                if (E.IsReady() && useE)
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsZombie && x.IsValidTarget(E.Range)))
                    {
                        E.Cast(enemy);
                    }
                }
            }
            
           
        } // RDY
        private static void furyManager()
        {
            var hpPercent = Config.Item("protect.reksai.hp").GetValue<Slider>().Value;
            var furyPercent = Config.Item("protect.reksai.fury").GetValue<Slider>().Value;
            var protectReksai = Config.Item("protect.reksai").GetValue<bool>();
            
            if (unburrowed && protectReksai) //
            {
                if (W.IsReady() && ObjectManager.Player.HealthPercent < hpPercent &&
                    ObjectManager.Player.ManaPercent > furyPercent)
                {
                    W.Cast(); 
                }
            }
        } // RDY
        private static void Jungle()
        {
            var useQ = Config.Item("q.jungle").GetValue<bool>();
            var autoSwitch = Config.Item("w.jungle").GetValue<bool>();
            var useE = Config.Item("e.jungle").GetValue<bool>();
            var useQ2 = Config.Item("q.jungle.burrowed").GetValue<bool>();
           
            var mob = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mob == null || (mob != null && mob.Count == 0))
            {
                return;
            }

            if (burrowed)
            {
                if (Q2.IsReady() && useQ2)
                {
                    Q2.Cast(mob[0].Position);
                }
                if (!Q2.IsReady() && W.IsReady() && autoSwitch)
                {
                    W.Cast();
                }
            }
            if (unburrowed)
            {
                if (Q.IsReady() && useQ && ObjectManager.Player.Distance(mob[0].Position) < E.Range)
                {
                    Q.Cast();
                }
                if (E.IsReady() && useE)
                {
                    E.Cast(mob[0]);
                }
                if (!Q.IsReady() && !E.IsReady() && W.IsReady() && autoSwitch)
                {
                    W.Cast();
                }
            }
        } // RDY
        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = Config.Item("qDraw").GetValue<Circle>();
            var menuItem2 = Config.Item("wDraw").GetValue<Circle>();
            var menuItem3 = Config.Item("eDraw").GetValue<Circle>();
            var menuItem4 = Config.Item("rDraw").GetValue<Circle>();

            if (menuItem1.Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), rangeQ, menuItem1.Color, 5);
            }
            if (menuItem2.Active && W.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), rangeW, menuItem2.Color, 5);
            }
            if (menuItem3.Active && E.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), rangeE, menuItem3.Color, 5);
            }
            if (menuItem4.Active && R.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Player.Position.X, Player.Position.Y, Player.Position.Z), R.Range, menuItem4.Color, 5);
            }
        } // RDY
    }
}
