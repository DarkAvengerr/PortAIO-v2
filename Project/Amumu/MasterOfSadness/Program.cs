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
 namespace MasterOfSadness
{
    class Program : Modes
    {
        private Menu menu;
        private String name;
        private String version;
        private AIHeroClient player;

        public Program()
        {
            menu = new Menu("Master Of Sadness", "MasterOfSadness", true);
            name = "[Amumu]Master Of Sadness";
            version = "1.0.0.0";
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
            var targetSelectorMenu = new Menu("TargetSelector", "TargetSelector");
            var comboMenu = new Menu("Combo", "Combo");
            {
                comboMenu.AddItem(new MenuItem("seth", "Q Hitchance")).SetValue(new Slider(4, 1, 4));
                comboMenu.AddItem(new MenuItem("QC", "Use Q in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("WC", "Use W in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("EC", "Use E in combo").SetValue(true));        
                comboMenu.AddItem(new MenuItem("Ignite", "Use ignite for kill").SetValue(true));
                comboMenu.AddItem(new MenuItem("combokey", "Combo key").SetValue(new KeyBind(32, KeyBindType.Press)));
            }
            var rMenu = new Menu("R", "R");
            {
                rMenu.AddItem(new MenuItem("RC", "Use R in combo").SetValue(true));
                rMenu.AddItem(new MenuItem("RqC", "Use Q in minion to engage with R").SetValue(true));
                rMenu.AddItem(new MenuItem("RcC", "Number of enemies on R  >=")).SetValue(new Slider(1, 1, 5));
            //    comboMenu.AddItem(new MenuItem("RcC", "Use flash for put your self on r range")).SetValue(true);
            }
            var HarrashMenu = new Menu("Harrash", "Harrash");
            {
                HarrashMenu.AddItem(new MenuItem("QH", "Use Q in Harrash").SetValue(true));
                HarrashMenu.AddItem(new MenuItem("WH", "Use W in Harrash").SetValue(true));
                HarrashMenu.AddItem(new MenuItem("EH", "Use E in Harrash").SetValue(true));
                HarrashMenu.AddItem(new MenuItem("Harrash key", "Harrash key").SetValue(new KeyBind('C', KeyBindType.Press)));
            }
            var LaneclearMenu = new Menu("Laneclear", "Laneclear");
            {
                LaneclearMenu.AddItem(new MenuItem("QL", "Use Q in Laneclear").SetValue(false));
                LaneclearMenu.AddItem(new MenuItem("WL", "Use W in Laneclear").SetValue(true));
                LaneclearMenu.AddItem(new MenuItem("EL", "Use E in Laneclear").SetValue(true));           
                LaneclearMenu.AddItem(new MenuItem("laneclearkey", "LaneClear key").SetValue(new KeyBind('V', KeyBindType.Press)));
            }
            var JungleclearMenu = new Menu("Jungleclear", "Jungleclear");
            {
                JungleclearMenu.AddItem(new MenuItem("QJ", "Use Q in JungleClear").SetValue(false));
                JungleclearMenu.AddItem(new MenuItem("WJ", "Use W in JungleClear").SetValue(true));
                JungleclearMenu.AddItem(new MenuItem("EJ", "Use E in JungleClear").SetValue(true));
                JungleclearMenu.AddItem(new MenuItem("jungleclearkey", "JungleClear key").SetValue(new KeyBind('V', KeyBindType.Press)));
            }
            /*  var ItemMenu = new Menu("Item Menu", "itemmenu");
              {
                  ItemMenu.AddItem(new MenuItem("Zhonias", "zhonias").SetValue(true));
              //    ItemMenu.AddItem(new MenuItem("xxxx", "xxxx").SetValue(true));
              //    ItemMenu.AddItem(new MenuItem("xxxx", "xxxx").SetValue(true));
              }*/
            var DrawSettingsMenu = new Menu("Draw Settings", "Draw Settings");
            {
               
                DrawSettingsMenu.AddItem(new MenuItem("DrawKilleableText", "Draw Killeable Text").SetValue(true));
                DrawSettingsMenu.AddItem(new MenuItem("Draw Q Range", "Draw Q Range").SetValue(true));
                DrawSettingsMenu.AddItem(new MenuItem("Draw W Range", "Draw W Range").SetValue(true));
                DrawSettingsMenu.AddItem(new MenuItem("Draw E Range", "Draw E Range").SetValue(true));
                DrawSettingsMenu.AddItem(new MenuItem("Draw R Range", "Draw R Range").SetValue(true));
                DrawSettingsMenu.AddItem(new MenuItem("Draw Special", "Draw Special").SetValue(true));
            }

            TargetSelector.AddToMenu(targetSelectorMenu);
            menu.AddSubMenu(orbWalkerMenu);        //ORBWALKER
            menu.AddSubMenu(targetSelectorMenu);   //TS
            comboMenu.AddSubMenu(rMenu);
            menu.AddSubMenu(comboMenu);//COMBO            
            menu.AddSubMenu(HarrashMenu);  //Harrash
            //  menu.AddSubMenu(ItemMenu);
            menu.AddSubMenu(LaneclearMenu);        //LANECLEAR
            menu.AddSubMenu(JungleclearMenu);      //JUNGLECLEAR
            menu.AddSubMenu(DrawSettingsMenu);     //DRAWS
            menu.AddToMainMenu();
        }
        int x;
        public void draw(EventArgs args)
        {
            if (player.IsDead) return;
            if (menu.Item("Draw Q Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position, 1100f, System.Drawing.Color.Purple, 2);
            if (menu.Item("Draw W Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position, 300f, System.Drawing.Color.Purple, 2);
            if (menu.Item("Draw E Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position, 350f, System.Drawing.Color.Purple, 2);     
            if (menu.Item("Draw R Range").GetValue<bool>())
                Render.Circle.DrawCircle(getPlayer().Position, 550f, System.Drawing.Color.Purple, 2);
            if (menu.Item("Draw Special").GetValue<bool>())
            {
                var players = Drawing.WorldToScreen(player.Position);
                Drawing.DrawText(players[0] - 35, players[1] + 10, System.Drawing.Color.Yellow, "Number of enemys R : " + base.getSkills().min);
                var minion = Drawing.WorldToScreen(base.getSkills().miniondraw.Position);
                if (!base.getSkills().miniondraw.IsDead)
                {
                    Render.Circle.DrawCircle(base.getSkills().miniondraw.Position, 100, System.Drawing.Color.Azure, 1);
                    Render.Circle.DrawCircle(base.getSkills().miniondraw.Position, 500, System.Drawing.Color.Red, 1);
                }
            }
            /*     foreach (BuffInstance b in ObjectManager.Player.Buffs)
            {
                Drawing.DrawText(players[0] - 35, players[1] + 10+x*10,System.Drawing.Color.Yellow, "value : " + b.DisplayName);
                x++;
            }
            x = 0;*/

        }

        public void load()
        {
               
            player = ObjectManager.Player;
            if (player.ChampionName != "Amumu") return;
            Utils.ShowNotification(getName() + " load good luck ;) " + getVersion(), System.Drawing.Color.White, 100);
            loadMenu();
            Drawing.OnDraw += draw;
            Game.OnUpdate += update;
        }
        public void update(EventArgs args)
        {
            if (getPlayer().IsDead) return;
            updateModes();
        }
        public void updateModes()
        {
            if (getMenu().Item("combokey").GetValue<KeyBind>().Active) base.combo(base.getTarget());  
             if (getMenu().Item("jungleclearkey").GetValue<KeyBind>().Active) base.jungleClear(); 
            if (getMenu().Item("laneclearkey").GetValue<KeyBind>().Active) base.laneClear();
            if (getMenu().Item("Harrash key").GetValue<KeyBind>().Active) base.harrash(base.getTarget());
            if (!getMenu().Item("combokey").GetValue<KeyBind>().Active)
            {
                base.getSkills().min=0;
            }
            if (!getMenu().Item("combokey").GetValue<KeyBind>().Active && !getMenu().Item("jungleclearkey").GetValue<KeyBind>().Active
                && !getMenu().Item("laneclearkey").GetValue<KeyBind>().Active && !getMenu().Item("Harrash key").GetValue<KeyBind>().Active)
            { base.getSkills().wDeCast(); }
        }

        public static void Main()
        {
            Program p = new Program();
            p.load(p);
            p.load();
        }


    }
}