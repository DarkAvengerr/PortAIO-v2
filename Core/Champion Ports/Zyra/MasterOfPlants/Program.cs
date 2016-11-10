using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace MasterOfThorns
{
    class Program : Modes
    {        
        private Menu menu;
        private String name;
        private String version;
        private AIHeroClient player;
       
        public Program()
        {
            menu = new Menu("Master Of Thorns", "MasterOfPlants", true);
            name = "[Zyra]Master Of Plants";
            version = "0.1.0.2";
        }

        public AIHeroClient getPlayer()
        {
            return player;
        }
        public Menu getMenu()
        {
            return menu;
        }
        public String getName()
        {
            return name;
        }
        public String getVersion()
        {
            return version;
        }

        public void loadMenu()
        {
            var orbWalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalking.Orbwalker orb = new Orbwalking.Orbwalker(orbWalkerMenu);
            var TargetSelectorMenu = new Menu("TargetSelector", "TargetSelector");
            var comboMenu = new Menu("Combo", "Combo");
            {
                comboMenu.AddItem(new MenuItem("QC", "Use Q in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("WC", "Use W in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("EC", "Use E in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("comboR", "Use R to finish the enemy").SetValue(true));
                comboMenu.AddItem(new MenuItem("Ignite", "Use ignite for kill").SetValue(true));
                comboMenu.AddItem(new MenuItem("combokey", "Combo key").SetValue(new KeyBind(32, KeyBindType.Press)));
            }
            var settingMenu = new Menu("setting", "HitChance settings");
            {
                comboMenu.AddItem(new MenuItem("seth", "E hitchance")).SetValue(new Slider(3, 1, 4));
                comboMenu.AddItem(new MenuItem("sethQ", "Q hitchance")).SetValue(new Slider(3, 1, 4));
                comboMenu.AddItem(new MenuItem("sethW", "W hitchance")).SetValue(new Slider(3, 1, 4));
                comboMenu.AddItem(new MenuItem("sethR", "R hitchance")).SetValue(new Slider(3, 1, 4));
                comboMenu.AddItem(new MenuItem("sethR", "Passive hitchance")).SetValue(new Slider(3, 1, 4));
            }
            var comboRMenu = new Menu("RCombokey", "Burst Combo"); 
            {
                comboRMenu.AddItem(new MenuItem("QrC", "Use Q in combo with R").SetValue(true));
                comboRMenu.AddItem(new MenuItem("ErC", "Use E in combo with R").SetValue(true));
                comboRMenu.AddItem(new MenuItem("rcombokey", "RCombo key").SetValue(new KeyBind('X', KeyBindType.Press)));
            }
            var harrashMenu = new Menu("Harrash", "Harrash");
            {
                harrashMenu.AddItem(new MenuItem("QH", "Use Q in Harrash").SetValue(true));
                harrashMenu.AddItem(new MenuItem("WH", "Use W for go out").SetValue(true));
                harrashMenu.AddItem(new MenuItem("Harrash key", "Harrash key").SetValue(new KeyBind('C', KeyBindType.Press)));
            }
            var ultimateSettingsMenu = new Menu("Ultimate Settings", "Ultimate Settings");
            {
                ultimateSettingsMenu.AddItem(new MenuItem("minEnemys", "Min. enemys to hit")).SetValue(new Slider(1, 1, 5));
                ultimateSettingsMenu.AddItem(new MenuItem("Ultimate Key", "Ultimate key").SetValue(new KeyBind('T', KeyBindType.Press)));
            //  UltimateSettingsMenu.AddItem(new MenuItem("useflash", "Use flash").SetValue(true));
            }
            var fleeMenu = new Menu("Flee", "Flee");
            {
                fleeMenu.AddItem(new MenuItem("fleekey", "Flee key").SetValue(new KeyBind('Z', KeyBindType.Press)));
                fleeMenu.AddItem(new MenuItem("flee", "Flee only use e"));
            }
            var laneclearMenu = new Menu("Laneclear", "Laneclear");
            {
                laneclearMenu.AddItem(new MenuItem("QL", "Use Q in Laneclear").SetValue(true));
                laneclearMenu.AddItem(new MenuItem("WL", "Use W in Laneclear").SetValue(true));
                laneclearMenu.AddItem(new MenuItem("EL", "Use E in Laneclear").SetValue(true));
                laneclearMenu.AddItem(new MenuItem("laneclearkey", "LaneClear key").SetValue(new KeyBind('V', KeyBindType.Press)));
            }
            var jungleclearMenu = new Menu("Jungleclear", "Jungleclear");
            {
                jungleclearMenu.AddItem(new MenuItem("QJ", "Use Q in JungleClear").SetValue(true));
                jungleclearMenu.AddItem(new MenuItem("WJ", "Use W in JungleClear").SetValue(true));
                jungleclearMenu.AddItem(new MenuItem("EJ", "Use E in JungleClear").SetValue(true));
                jungleclearMenu.AddItem(new MenuItem("jungleclearkey", "JungleClear key").SetValue(new KeyBind('V', KeyBindType.Press)));
            }
     /*       var itemMenu = new Menu("Item Menu", "Itemmenu");
            {
                itemMenu.AddItem(new MenuItem("Zhonias", "Zhonias").SetValue(true));
             //   ItemMenu.AddItem(new MenuItem("xxxx", "xxxx").SetValue(true));
             //   ItemMenu.AddItem(new MenuItem("xxxx", "xxxx").SetValue(true));
            }
       */     var drawSettingsMenu = new Menu("Draw Settings", "Draw Settings");
            {
                drawSettingsMenu.AddItem(new MenuItem("DrawUltimate", "DrawUltimate").SetValue(true));
                drawSettingsMenu.AddItem(new MenuItem("DrawKilleableText", "Draw Killeable Text").SetValue(true));
                drawSettingsMenu.AddItem(new MenuItem("Draw Q Range", "Draw Q Range").SetValue(true));
                drawSettingsMenu.AddItem(new MenuItem("Draw W Range", "Draw W Range").SetValue(true));
                drawSettingsMenu.AddItem(new MenuItem("Draw E Range", "Draw E Range").SetValue(true));
                drawSettingsMenu.AddItem(new MenuItem("Draw R Range", "Draw R Range").SetValue(true));
            }

            TargetSelector.AddToMenu(TargetSelectorMenu);
            menu.AddSubMenu(orbWalkerMenu);        //ORBWALKER
            menu.AddSubMenu(TargetSelectorMenu);   //TS
            menu.AddSubMenu(comboMenu); //COMBO
            menu.AddSubMenu(comboRMenu);
            menu.AddSubMenu(harrashMenu);  //Harrash
            menu.AddSubMenu(ultimateSettingsMenu);  //Ultimate
            menu.AddSubMenu(fleeMenu); 
           // menu.AddSubMenu(itemMenu);
            menu.AddSubMenu(laneclearMenu);        //LANECLEAR
            menu.AddSubMenu(jungleclearMenu);      //JUNGLECLEAR
            menu.AddSubMenu(drawSettingsMenu);     //DRAWS
            menu.AddToMainMenu();
        }

        public void draw(EventArgs args)
        {
            if (player.IsDead) return;
            if (menu.Item("Draw Q Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position,800f, System.Drawing.Color.Blue, 2);
            if (menu.Item("Draw W Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position,850f, System.Drawing.Color.Blue, 2);
            if (menu.Item("Draw E Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position,1100f, System.Drawing.Color.Blue, 2);
            if (menu.Item("Draw R Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position,700f, System.Drawing.Color.Blue,2);           
        }
        
        public void load()
        {
            player = ObjectManager.Player;
            if (player.ChampionName != "Zyra") return;
            Chat.Print(getName()+" load good luck ;) " + getVersion());
            loadMenu();
            Drawing.OnDraw += draw;
            Game.OnUpdate += update;
        }
    
        public void update(EventArgs args)
        {
            if (getPlayer().IsDead) return;
            if (base.zyraZombie())  base.getSkills().passiveCast(getMenu().Item("sethQ").GetValue<Slider>().Value);     
            updateModes();
        }

        public  void cast(AIHeroClient target, Spell spell, int h)
        {

                spell.CastIfHitchanceEquals(
                    target,
                    base.getSkills().hitchanceCheck(h));
            
        }
        public void updateModes()
        {
            if (getMenu().Item("combokey").GetValue<KeyBind>().Active)  base.combo(base.getTarget());
            if (getMenu().Item("rcombokey").GetValue<KeyBind>().Active) base.rCombo(base.getTarget());
            if (getMenu().Item("fleekey").GetValue<KeyBind>().Active) base.flee(base.getTarget());
            if (getMenu().Item("Ultimate Key").GetValue<KeyBind>().Active) base.onlyR(base.getTarget());
            if (getMenu().Item("laneclearkey").GetValue<KeyBind>().Active) base.laneClear();
        //    if (getMenu().Item("Zhonias").GetValue<KeyBind>().Active) base.items();
            if (getMenu().Item("jungleclearkey").GetValue<KeyBind>().Active) base.jungleClear();
            if (getMenu().Item("Harrash key").GetValue<KeyBind>().Active) base.harrash(base.getTarget());
            base.getSkills().igniteCast(base.getTarget());
        }

        public static void Main()
        {
            Program p = new Program();
            p.load(p); 
            p.load();
        }
    }
}
