/*                                                          __                          .-'''-.                                                            
       .-''-.                        .---.           ...-'  |`.                      '   _    \                                                          
     .' .-.  )           .--.        |   |           |      |  |   __  __   ___    /   /` '.   \    _..._        .           __.....__                   
    / .'  / /     .--./) |__|        |   |           ....   |  |  |  |/  `.'   `. .   |     \  '  .'     '.    .'|       .-''         '. .-.          .- 
   (_/   / /     /.''\\  .--..-,.--. |   |             -|   |  |  |   .-.  .-.   '|   '      |  '.   .-.   . .'  |      /     .-''"'-.  `.\ \        / / 
        / /     | |  | | |  ||  .-. ||   |              |   |  |  |  |  |  |  |  |\    \     / / |  '   '  |<    |     /     /________\   \\ \      / /  
       / /       \`-' /  |  || |  | ||   |       _   ...'   `--'  |  |  |  |  |  | `.   ` ..' /  |  |   |  | |   | ____|                  | \ \    / /   
      . '        /("'`   |  || |  | ||   |     .' |  |         |`.|  |  |  |  |  |    '-...-'`   |  |   |  | |   | \ .'\    .-------------'  \ \  / /    
     / /    _.-')\ '---. |  || |  '- |   |    .   | /` --------\ ||  |  |  |  |  |               |  |   |  | |   |/  .  \    '-.____...---.   \ `  /     
   .' '  _.'.-''  /'""'.\|__|| |     |   |  .'.'| |// `---------' |__|  |__|  |__|               |  |   |  | |    /\  \  `.             .'     \  /      
  /  /.-'_.'     ||     ||   | |     '---'.'.'.-'  /                                             |  |   |  | |   |  \  \   `''-...... -'       / /       
 /    _.'        \'. __//    |_|          .'   \_.'                                              |  |   |  | '    \  \  \                  |`-' /        
( _.-'            `'---'                                                                         '--'   '--''------'  '---'                 '..'         

*/
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Two_Girls_One_Monkey
{
    class Program
    {
        public const string Champion = "MonkeyKing";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Menu Config;
        public static Items.Item TIA;
        public static Items.Item HYD;
        public static Items.Item BOTRK;
        public static Items.Item YOM;
        public static AIHeroClient Player;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != Champion)
                return;

            #region spells
            
            Notifications.AddNotification("2Girls1Monkey Loaded", 3000);
            
            Q = new Spell(SpellSlot.Q, 420f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 640f);
            R = new Spell(SpellSlot.R, 355f);
            E.SetTargetted(0.5f, 2000f);

            TIA = new Items.Item(3077, 375f);
            HYD = new Items.Item(3074, 375f);
            BOTRK = new Items.Item(3153, 425f);
            YOM = new Items.Item(3142, 0f);
            #endregion spells

            #region Menu
            //MenuConfig
            Config = new Menu("SuckMyBanana", "2Girls1Monkey", true);
            
            //Target Selector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            
            //Orbwalk
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //LaneClear
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearQ", "Use Q").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearE", "Use E").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneMana", "Minimum Mana for clear")).SetValue(new Slider(50, 0, 100));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearActive", "Laneclear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //C-C-C-Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(false));
            Config.SubMenu("Combo").AddItem(new MenuItem("AutoUlt", "Auto Spin2Win").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("RCount", "Minimum Enemies to AutoUlt").SetValue(new Slider(3, 1, 5)));
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //JungleClear
            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("UseQJGClear", "Use Q").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("UseEJGClear", "Use E").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("JungleClearActive", "JungleClear!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //Items
            Config.AddSubMenu(new Menu("Item", "Item"));
            Config.SubMenu("Item").AddItem(new MenuItem("Tiamat", "Use Tiamat").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("Hydra", "Use Hydra").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("BOTRK", "Use BoTRK").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("YOM", "Use Youmuu's Ghostblade").SetValue(true));
            
            //MISCMENU
            Config.SubMenu("Misc").AddItem(new MenuItem("AntiGap", "Anti Gapcloser").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Interrupt", "Interrupt Spells").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KillstealR", "Killsteal with R").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KillstealQ", "Killsteal with Q").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KillstealE", "Killsteal with E").SetValue(false));


            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E Range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q Range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R Range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            Config.AddToMainMenu();
            #endregion Menu


            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapCloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            
        }

        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Base sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.Item("Interrupt").GetValue<bool>()) return;

            if (Player.Distance(sender) < R.Range && R.IsReady())
            {
                R.Cast();
            }
        }

        private static void AntiGapCloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var fullHP = Player.MaxHealth;
            var HP = Player.Health;
            var critHP = fullHP / 4;
            if (W.IsReady() && gapcloser.Sender.IsValidTarget() && Config.Item("AntiGap").GetValue<bool>())
                W.Cast(gapcloser.End);
            else if (!W.IsReady() && gapcloser.Sender.IsValidTarget(R.Range) && R.IsReady() && (HP <= critHP) && Config.Item("AntiGap").GetValue<bool>())
                R.Cast(gapcloser.End);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) 
                return;
            Orbwalker.SetAttack(true);
            Orbwalker.SetMovement(true);

            var useRks = Config.Item("KillstealR").GetValue<bool>() && R.IsReady();
            var useQks = Config.Item("KillstealQ").GetValue<bool>() && Q.IsReady();
            var useEks = Config.Item("KillstealE").GetValue<bool>() && E.IsReady();
            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
                Combo();
            if (Config.Item("LaneClearActive").GetValue<KeyBind>().Active)
                LaneClear();
            if (useRks)
                KillstealR();
            if (useQks)
                KillstealQ();
            if (useEks)
                KillstealE();
            if (Config.Item("JungleClearActive").GetValue<KeyBind>().Active)
                JungleClear();
            if (Config.Item("AutoUlt").GetValue<bool>() && LeagueSharp.Common.Utility.CountEnemiesInRange((int)R.Range) >= Config.Item("RCount").GetValue<Slider>().Value && R.IsReady())
                R.Cast();
        }
        

        private static void Combo()
        {
            Orbwalker.SetAttack(true);
            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            bool useQ = Config.Item("UseQCombo").GetValue<bool>();
            bool useW = Config.Item("UseWCombo").GetValue<bool>();
            bool useE = Config.Item("UseECombo").GetValue<bool>();
            bool useR = Config.Item("UseRCombo").GetValue<bool>();
            bool useT = Config.Item("Tiamat").GetValue<bool>();
            bool useH = Config.Item("Hydra").GetValue<bool>();
            bool useB = Config.Item("BOTRK").GetValue<bool>();
            bool useY = Config.Item("YOM").GetValue<bool>();


                if (target != null && useQ && Q.IsReady() && Player.Distance(target) <= 420)
                {
                    Q.Cast();
                }

                if ((target != null && useE && E.IsReady() && Player.Distance(target) > 420) || (target != null && !Q.IsReady() && Player.Distance(target) <= 420))
                {
                    E.CastOnUnit(target);
                }
                if ((target != null && useW && W.IsReady()) && (Player.HealthPercent <= 80 || target.HealthPercent <= 65))
                {
                    W.Cast();
                }

                if (target != null && useR && R.IsReady())
                {
                    R.Cast();
                }

                if (target != null && useT && TIA.IsReady() && Player.Distance(target) > 350)
                {
                    TIA.Cast();
                }

                if (target != null && useH && HYD.IsReady() && Player.Distance(target) > 350)
                {
                    HYD.Cast();
                }

                if (target != null && useB && BOTRK.IsReady())
                {
                    BOTRK.Cast(target);
                }
                if (target != null && useY && YOM.IsReady())
                {
                    YOM.Cast();
                }
            }


        private static void Drawing_OnDraw(EventArgs args)
        {

            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }
        }
        private static void KillstealR()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValidTarget(R.Range)))
            {
                if (R.IsReady() && hero.Distance(ObjectManager.Player) <= R.Range &&
                    Player.GetSpellDamage(hero, SpellSlot.R) >= hero.Health)
                    R.Cast();
            }
        }

        private static void KillstealQ()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValidTarget(Q.Range)))
            {
                if (Q.IsReady() && hero.Distance(ObjectManager.Player) <= Q.Range &&
                    Player.GetSpellDamage(hero, SpellSlot.Q) >= hero.Health)
                    Q.Cast();
            }
        }

        private static void KillstealE()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValidTarget(E.Range)))
            {
                if (E.IsReady() && hero.Distance(ObjectManager.Player) <= E.Range &&
                    Player.GetSpellDamage(hero, SpellSlot.E) >= hero.Health)
                    E.Cast(hero);
            }
        }
        static void LaneClear()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);
            var lanemana = Config.Item("LaneMana").GetValue<Slider>().Value;
            if (Player.ManaPercent >= lanemana && (minion.Count > 0))
            {
                var minions = minion[0];
                if (Config.Item("LaneClearQ").GetValue<bool>() && Q.IsReady() && minions.IsValidTarget(Q.Range) && Q.IsKillable(minions))
                {
                    Q.Cast();
                }
                if (Config.Item("Tiamat").GetValue<bool>() && TIA.IsReady() && minion.Count >= 2 && Player.Distance(minions) <= 350)
                    TIA.Cast();

                if (Config.Item("Hydra").GetValue<bool>() && HYD.IsReady() && minion.Count >= 2 && Player.Distance(minions) <= 350)
                    HYD.Cast();
            }
            if (Player.ManaPercent >= lanemana && (minion.Count > 0))
            if (minion.Count > 2)
            {
                var minions = minion[2];
                if (Config.Item("LaneClearE").GetValue<bool>() && E.IsReady() && minions.IsValidTarget(E.Range))
                {
                    E.Cast(minions);
                }
                if (Config.Item("Tiamat").GetValue<bool>() && TIA.IsReady() && minion.Count >= 2 && Player.Distance(minions) <= 350)
                    TIA.Cast();

                if (Config.Item("Hydra").GetValue<bool>() && HYD.IsReady() && minion.Count >= 2 && Player.Distance(minions) <= 350)
                    HYD.Cast();
            }
        }
        static void JungleClear()
        {
            var jngm = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (jngm.Count > 0)
            {
                var jngms = jngm[0];
                if (Config.Item("UseQJGClear").GetValue<bool>() && Q.IsReady() && jngms.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
                if (Config.Item("Tiamat").GetValue<bool>() && TIA.IsReady() && TIA.IsReady() && jngm.Count >= 2 && Player.Distance(jngms) <= 350)
                    TIA.Cast();

                if (Config.Item("Hydra").GetValue<bool>() && HYD.IsReady() && HYD.IsReady() && jngm.Count >= 2 && Player.Distance(jngms) <= 350)
                    HYD.Cast();
            }

            if (jngm.Count > 1)
            {
                var jngms = jngm[1];
                if (Config.Item("UseEJGClear").GetValue<bool>() && E.IsReady() && jngms.IsValidTarget(E.Range))
                {
                    E.Cast(jngms);
                }
                if (Config.Item("Tiamat").GetValue<bool>() && TIA.IsReady() && TIA.IsReady() && jngm.Count >= 2 && Player.Distance(jngms) <= 350)
                    TIA.Cast();

                if (Config.Item("Hydra").GetValue<bool>() && HYD.IsReady() && HYD.IsReady() && jngm.Count >= 2 && Player.Distance(jngms) <= 350)
                    HYD.Cast();
            }

        }
    }
}