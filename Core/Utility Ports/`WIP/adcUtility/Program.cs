using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;



using EloBuddy; 
 using LeagueSharp.Common; 
 namespace adcUtility
{
    class Program
    {
        public static Menu Config;
        public static SpellSlot Heal;
        public static SpellSlot Barrier;

        
		//test
        static void Main(string[] args)
        {
            Game_OnGameLoad();
        }
        private static void Game_OnGameLoad()
        {
            //start activator
            Activator.Bilgewater_Cutlass.hikiBilgewater = true;
            Activator.Blade_of_Ruined_King.hikiBOTRK = true;
            Activator.Ghostblade.hikiGhostBlade = true;
            Activator.Potion.hikiPotion = true;
            Activator.Quick_Silver_Sash.hikiQSS = true;
            Activator.Barrier.hikiBarrier = true;
            Activator.Heal.hikiHeal = true;
            //start range
            Range.Ally_AA.hikiAllyAA = true;
            Range.Enemy_AA.hikiEnemyAA = true;
            //start turret
            Turret.Ally_Turret.hikiAllyTurret = true;
            Turret.Enemy_Turret.hikiEnemyTurret = true;
            //start dragon
            Dragon.Buff_Drawer.hikiBuffDrawer = true;
            //start plugins
            Plugins.Anti_Rengar.hikiAntiRengar = true;

            Heal = ObjectManager.Player.GetSpellSlot("summonerheal");
            Barrier = ObjectManager.Player.GetSpellSlot("summonerbarrier");

            Config = new Menu("adcUtility", "adcUtility", true);

            var activatorMenu = new Menu("Activator Settings", "Activator Settings");
            {
                var summonerMenu = new Menu("Summoner Spells", "Summoner Spells");
                {
                    var healMenu = new Menu("Heal Settings", "Heal Settings");
                    {
                        healMenu.AddItem(new MenuItem("use.heal", "Use Heal").SetValue(true));
                        healMenu.AddItem(new MenuItem("heal.myhp", "Use if my HP < %").SetValue(new Slider(10, 0, 100)));
                        summonerMenu.AddSubMenu(healMenu);
                    }
                    var barrierMenu = new Menu("Barrier Settings", "Barrier Settings");
                    {
                        barrierMenu.AddItem(new MenuItem("use.barrier", "Use Heal").SetValue(true));
                        barrierMenu.AddItem(new MenuItem("barrier.myhp", "Use if my HP < %").SetValue(new Slider(10, 0, 100)));
                        summonerMenu.AddSubMenu(barrierMenu);
                    }
                    activatorMenu.AddSubMenu(summonerMenu);
                }
                var qssMenu = new Menu("QSS Settings", "QSS Settings");
                {
                    qssMenu.AddItem(new MenuItem("use.qss", "Use QSS").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.ignite", "Clear Ignite").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.exhaust", "Clear Exhaust").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.zedult", "Clear Zed R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.fizzult", "Clear Fizz R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.malzaharult", "Clear Malzahar R").SetValue(true));
                    qssMenu.AddItem(new MenuItem("clear.vladulti", "Clear Vladimir R").SetValue(true));
                    activatorMenu.AddSubMenu(qssMenu);
                }

                var botrk = new Menu("BOTRK Settings", "BOTRK Settings");
                {
                    botrk.AddItem(new MenuItem("useBOTRK", "Use BOTRK").SetValue(true));
                    botrk.AddItem(new MenuItem("myhp", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    botrk.AddItem(new MenuItem("theirhp", "Use if enemy HP < %").SetValue(new Slider(20, 0, 100)));
                    activatorMenu.AddSubMenu(botrk);
                }

                var ghostBlade = new Menu("GhostBlade Settings", "GhostBlade Settings");
                {
                    ghostBlade.AddItem(new MenuItem("gBlade", "Use GhostBlade").SetValue(true));
                    activatorMenu.AddSubMenu(ghostBlade);
                }

                var bilgewater = new Menu("Bilgewater Settings", "Bilgewater Settings");
                {
                    bilgewater.AddItem(new MenuItem("useBilge", "Use BOTRK").SetValue(true));
                    bilgewater.AddItem(new MenuItem("myhpbilge", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    bilgewater.AddItem(new MenuItem("theirhpbilge", "Use if enemy HP < %").SetValue(new Slider(20, 0, 100)));
                    activatorMenu.AddSubMenu(bilgewater);
                }
                var health = new Menu("Health Potion Settings", "Health Potion Settings");
                {
                    health.AddItem(new MenuItem("useHealth", "Use Health Potion").SetValue(true));
                    health.AddItem(new MenuItem("myhp", "Use if my HP < %").SetValue(new Slider(20, 0, 100)));
                    activatorMenu.AddSubMenu(health);
                }
                var mana = new Menu("Mana Potion Settings", "Mana Potion Settings");
                {
                    mana.AddItem(new MenuItem("useMana", "Use Mana Potion").SetValue(true));
                    mana.AddItem(new MenuItem("mymana", "Use if my mana < %").SetValue(new Slider(20, 0, 100)));
                    activatorMenu.AddSubMenu(mana);
                }
                Config.AddSubMenu(activatorMenu);
            }

            var turretMenu = new Menu("Turret Settings", "Turret Settings");
            {
                var allyTurret = new Menu("Ally Turret Settings", "Ally Turret Settings");
                {
                    allyTurret.AddItem(new MenuItem("ally.turret", "Ally Turret Range").SetValue(new Circle(true, Color.Yellow)));
                    allyTurret.AddItem(new MenuItem("ally.turret.distance", "Turret Distance").SetValue(new Slider(1000, 0, 2000)));
                    allyTurret.AddItem(new MenuItem("ally.turret.thickness", "Thickness").SetValue(new Slider(5, 0, 5)));
                    turretMenu.AddSubMenu(allyTurret);
                }
                var enemyTurret = new Menu("Enemy Turret Settings", "Enemy Turret Settings");
                {
                    enemyTurret.AddItem(new MenuItem("enemy.turret.set", "Enemy Turret Range").SetValue(new Circle(true, Color.Green)));
                    enemyTurret.AddItem(new MenuItem("enemy.turret.distance", "Range Distance").SetValue(new Slider(1000, 0, 2000)));
                    enemyTurret.AddItem(new MenuItem("enemy.turret.thickness", "Range Thickness").SetValue(new Slider(5, 0, 5)));
                    turretMenu.AddSubMenu(enemyTurret);
                }
                Config.AddSubMenu(turretMenu);
            }
            
            var rangeMenu = new Menu("Range Settings","Range Settings");
            {
                var allyRange = new Menu("Ally Range Settings", "Ally Range Settings");
                {
                    allyRange.AddItem(new MenuItem("ally.range", "Ally Range").SetValue(new Circle(true, Color.Gold)));
                    allyRange.AddItem(new MenuItem("ally.distance", "Range Distance").SetValue(new Slider(1000, 0, 2000)));
                    allyRange.AddItem(new MenuItem("ally.thickness", "Range Thickness").SetValue(new Slider(5, 0, 5)));
                    rangeMenu.AddSubMenu(allyRange);
                }
                var enemyRange = new Menu("Enemy Range Settings", "Enemy Range Settings");
                {
                    enemyRange.AddItem(new MenuItem("enemy.range", "Enemy AA Range").SetValue(new Circle(true, Color.Red)));
                    enemyRange.AddItem(new MenuItem("enemy.distance", "Range Distance").SetValue(new Slider(1000, 0, 2000)));
                    enemyRange.AddItem(new MenuItem("enemy.thickness", "Range Thickness").SetValue(new Slider(5, 0, 5)));
                    rangeMenu.AddSubMenu(enemyRange);
                }

                Config.AddSubMenu(rangeMenu);
            }
            var dragonBuff = new Menu("Dragon Settings", "Dragon Settings");
            {
                dragonBuff.AddItem(new MenuItem("enemy.dragon.buff.count", "Enemy Dragon Buff Count").SetValue(new Circle(true, Color.White)));
                dragonBuff.AddItem(new MenuItem("ally.dragon.buff.count", "Ally Dragon Buff Count").SetValue(new Circle(true, Color.White)));
                Config.AddSubMenu(dragonBuff);
            }
            var antiRengar = new Menu("Anti-Rengar Settings", "Anti-Rengar Settings");
            {
                antiRengar.AddItem(new MenuItem("anti.rengar", "Use Anti-Rengar").SetValue(true));
                var antirengarSub = new Menu("Supported Champion & Spells", "Supported Champion & Spells");
                {
                    antirengarSub.AddItem(new MenuItem("vayne.E", "Vayne[E]"));
                    antirengarSub.AddItem(new MenuItem("tristana.R", "Tristana [R]"));
                    antirengarSub.AddItem(new MenuItem("draven.E", "Draven [E]"));
                    antirengarSub.AddItem(new MenuItem("ashe.R", "Ashe [R]"));
                    antirengarSub.AddItem(new MenuItem("jinx.E", "Jinx [E]"));
                    antirengarSub.AddItem(new MenuItem("urgot.R", "Urgot [R]"));
                    antiRengar.AddSubMenu(antirengarSub);
                }
                Config.AddSubMenu(antiRengar);
            }

            Config.AddToMainMenu();
        }
    }
}
