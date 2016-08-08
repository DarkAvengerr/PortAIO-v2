using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ARAMDetFull.Champions;
using DetuksSharp;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using KeyBindType = LeagueSharp.Common.KeyBindType;

using EloBuddy; namespace ARAMDetFull
{
    class ARAMDetFull
    {
        /* TODO:
         * ##- Tower range higher dives a lot
         * ##- before level 6/7 play safer dont go so close stay behind other players or 800/900 units away from closest enemy champ
         * ##- Target selector based on invincible enemies
         * ##- IF invincible or revive go full in
         * ##- if attacking enemy and it is left 3 or less aa to kill then follow to kill (check movespeed dif)
         *  - bush invis manager player death
         *  - fixx gankplank plays like retard
         *  - this weeks customs
         *  - WPF put to allways take mark
         * ##- nami auto level
         *  - Some skills make aggresivity for time and how much to put in balance ignore minsions on/off
         * ## - LeeSin
         * ## - Nocturn
         *  - Gnar
         *  -Katarina error
         *  - Gangplank error
         *  ##- healing relics
         *  -Make velkoz
         */
        public static TextWriter defaultOut;


        public ARAMDetFull()
        {
            Console.WriteLine("Aram det full started!");
            CustomEvents.Game.OnGameLoad += onLoad;
        }
        
        public static int gameStart = 0;

        public static Menu Config;

        public static int now
        {
            get { return (int)DateTime.Now.TimeOfDay.TotalMilliseconds; }
        }

        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        private static void onLoad(object sender)
        {
            gameStart = now;

            Chat.Print("ARAm - Sharp by DeTuKs");
            try
            {
                defaultOut = System.Console.Out;
                System.Console.SetOut(new ErrorLogger(defaultOut));

                Config = new Menu("ARAM", "aramBot", true);

                var orbwalkerMenu = new Menu("Orbwalker", "my_Orbwalker");
                DeathWalker.AddToMenu(orbwalkerMenu);
                Config.AddSubMenu(orbwalkerMenu);

                //Extra
                Config.AddSubMenu(new Menu("Extra Sharp", "extra"));
                Config.SubMenu("extra").AddItem(new MenuItem("debugDraw", "Debug draw")).SetValue(false);
                Config.SubMenu("extra").AddItem(new MenuItem("dataGathering", "Send errors to server")).SetValue(false);


                //Debug
                Config.AddSubMenu(new Menu("Debug", "debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "Debug Target")).SetValue(new KeyBind('T', KeyBindType.Press, false));


                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnUpdate += OnGameUpdate;
                Game.OnEnd += OnGameEnd;
                ARAMSimulator.setupARMASimulator();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        

        private static void OnGameEnd(EventArgs args)
        {
            DataGathering.sendEndGame(ARAMSimulator.toNex.Health<ARAMSimulator.fromNex.Health);
            Thread.Sleep(5000);//Stole from myo lol
            Game.QuitGame();
        }
        

        private static void onDraw(EventArgs args)
        {
            try
            {
                if (!Config.Item("debugDraw").GetValue<bool>())
                    return;
                Drawing.DrawText(100, 100, Color.Red, "2bal: " + ARAMSimulator.balance+ " fear: "+MapControl.fearDistance );
               //return;
               // ((Jayce)ARAMSimulator.champ).drawCD();
                foreach (var hel in ObjectManager.Get<Obj_AI_Minion>().Where(r => r.IsValid && !r.IsDead && r.Name.ToLower().Contains("blobdrop")))
                {
                    var spos = Drawing.WorldToScreen(hel.Position);
                    Drawing.DrawText(spos.X, spos.Y, Color.Brown, " : " + hel.Name);
                    Drawing.DrawText(spos.X, spos.Y+25, Color.Brown, hel.IsDead + " : " + hel.Type+ " : " + hel.IsValid+ " : " + hel.CharacterState);
                }
                var tar = ARAMTargetSelector.getBestTarget(5100);
                if (tar != null)
                    LeagueSharp.Common.Utility.DrawCircle(tar.Position, 150, Color.Violet);

                foreach (var sec in ARAMSimulator.sectors)
                {
                    sec.draw();
                }

                foreach (var ene in MapControl.enemy_champions)
                {
                    var spos = Drawing.WorldToScreen(ene.hero.Position);
                    LeagueSharp.Common.Utility.DrawCircle(ene.hero.Position, ene.reach , Color.Green);
                
                    Drawing.DrawText(spos.X, spos.Y, Color.Green,"Gold: "+ene.hero.Gold);
                }
                return;

                foreach (var ene in MapControl.enemy_champions)
                {
                    LeagueSharp.Common.Utility.DrawCircle(ene.hero.Position, ene.reach, Color.Violet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void getAllBuffs()
        {
            foreach (var aly in HeroManager.Enemies)
            {
                foreach (var buffs in aly.Buffs)
                {
                    Console.WriteLine(aly.ChampionName + " - Buf: " + buffs.Name);
                }
            }
        }

        private static int lastTick = now;

        private static int tickTimeRng = 77;
        private static Random rng = null;
        private static void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Config.Item("db_targ").GetValue<KeyBind>().Active)
                {
                    foreach (var buf in ObjectManager.Player.Buffs)
                    {
                        Console.WriteLine(buf.Name);
                    }
                    
                    /*var player = HeroManager.Player;
                    foreach (var spell in
                   SpellDatabase.Spells.Where(
                       s =>
                           s.ChampionName.Equals(player.ChampionName)))
                    {
                        Console.WriteLine("-----Spell: " + spell.Slot + "---------------- ");
                        if(spell.SpellTags != null)
                            foreach (var tag in spell.SpellTags)
                            {
                                Console.WriteLine("--tag: " + tag);
                            }
                        if (spell.CastType != null)
                            foreach (var ctype in spell.CastType)
                            {
                                Console.WriteLine("--casttype: " + ctype);
                            }
                    }*/

                }
                //if (lastTick + tickTimeRng > now)
                //    return;

                //if(rng == null)
                //   rng = new Random();

                //tickTimeRng = rng.Next(70, 140);
                lastTick = now;
                ARAMSimulator.updateArmaPlay();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
