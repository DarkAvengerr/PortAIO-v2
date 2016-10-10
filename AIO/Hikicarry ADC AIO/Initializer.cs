using System;
using System.Drawing;
using System.Linq;
using HikiCarry.Core.Activator;
using HikiCarry.Core.Plugins;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry
{
    internal static class Initializer
    {

        internal static Menu Config;
        internal static Menu Activator;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static AIHeroClient Player = ObjectManager.Player;

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static string[] SupportedChampions =
        {
            "Ashe", "Caitlyn", "Draven" , "Ezreal" , "Jhin" ,"Jinx", "Kalista" , "Lucian",
                "Quinn", "Sivir", "Tristana", "Varus", "Vayne"
        };

        public static Cleanser Cleanser = new Cleanser();
        public static Youmuu Youmuu = new Youmuu();
        public static Bilgewater Bilgewater = new Bilgewater();
        public static Botrk Botrk = new Botrk();


        internal static void Load()
        {
            try
            {
                if (SupportedChampions.Contains(ObjectManager.Player.ChampionName))
                {
                    Chat.Print("<font color = \"#FFFF33\">HikiCarry: " + 
                        ObjectManager.Player.ChampionName + " Loaded</font>");
                }

                else
                {
                    Chat.Print("<font color = \"#FFFF33\">HikiCarry: " + 
                        ObjectManager.Player.ChampionName + " Not Supported </font>");
                }

                Config = new Menu("HikiCarry: " + Player.ChampionName, "HikiCarry", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.GreenYellow); 
                {
                    Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));


                    var type = Type.GetType("HikiCarry.Champions." + Player.ChampionName);
                    if (type != null)
                    {
                        DynamicInitializer.NewInstance(type);
                    }

                    if (ObjectManager.Player.ChampionName != "Vayne" || ObjectManager.Player.ChampionName != "Tristana")
                    {
                        Config.AddItem(
                        new MenuItem("hitchance", "Hit Chance", true).SetValue(new StringList(HitchanceNameArray, 2)))
                        .SetFontStyle(FontStyle.Italic, SharpDX.Color.Gold);
                        Config.AddItem(new MenuItem("prediction", "Prediction").SetValue(new StringList(new[] { "Common", "Sebby", "sPrediction", "SDK" }, 1)).SetFontStyle(FontStyle.Italic, SharpDX.Color.Gold))
                        .ValueChanged += (s, ar) =>
                        {
                            Config.Item("pred.info").Show(ar.GetNewValue<StringList>().SelectedIndex == 2);
                        };
                        Config.AddItem(new MenuItem("pred.info", "                 PRESS F5 FOR LOAD SPREDICTION").SetFontStyle(FontStyle.Bold))
                            .Show(Config.Item("prediction").GetValue<StringList>().SelectedIndex == 2);

                        if (Config.Item("prediction").GetValue<StringList>().SelectedIndex == 2)
                        {
                            SPrediction.Prediction.Initialize(Config, ":: SPREDICTION");
                        }
                    }

                    Config.AddItem(
                        new MenuItem("credits.x1", "                          Developed by Hikigaya").SetFontStyle(
                            FontStyle.Bold, SharpDX.Color.DodgerBlue));

                    Config.AddToMainMenu();
                }

                Activator = new Menu("HikiCarry: Activator", "HikiCarry: Activator", true).SetFontStyle(FontStyle.Bold,SharpDX.Color.GreenYellow);
                {

                    var bilgewater = new Menu("Bilgewater Settings", "Bilgewater Settings");
                    {
                        bilgewater.AddItem(new MenuItem("bilgewater", "Use (Bilgewater)").SetValue(true));
                        bilgewater.AddItem(new MenuItem("bilgewater.adc.hp", "Min ADC HP %").SetValue(new Slider(20, 1, 99)));
                        bilgewater.AddItem(new MenuItem("bilgewater.enemy.hp", "Minimum Enemy HP %").SetValue(new Slider(20, 1, 99)));
                        Activator.AddSubMenu(bilgewater);
                    }

                    var botrk = new Menu("Blade of Ruined King Settings", "Blade of Ruined King Settings");
                    {
                        botrk.AddItem(new MenuItem("botrk", "Use (BotRK)").SetValue(true));
                        botrk.AddItem(new MenuItem("botrk.adc.hp", "Min ADC HP %").SetValue(new Slider(20, 1, 99)));
                        botrk.AddItem(new MenuItem("botrk.enemy.hp", "Minimum Enemy HP %").SetValue(new Slider(20, 1, 99)));
                        Activator.AddSubMenu(botrk);
                    }

                    var youmuu = new Menu("Youmuu Settings", "Youmuu Settings");
                    {
                        youmuu.AddItem(new MenuItem("youmuu", "Use (Youmuu)").SetValue(true));
                        Activator.AddSubMenu(youmuu);
                    }

                    var cleanse = new Menu("Cleanse Settings", "Cleanse Settings");
                    {
                        var cleansedebuffs = new Menu("Cleanse Debuffs", "Cleanse Debuffs");
                        {
                            cleansedebuffs.AddItem(new MenuItem("qss.charm", "Charm").SetValue(true));
                            cleansedebuffs.AddItem(new MenuItem("qss.flee", "Flee").SetValue(true));
                            cleansedebuffs.AddItem(new MenuItem("qss.snare", "Snare").SetValue(true));
                            cleansedebuffs.AddItem(new MenuItem("qss.polymorph", "Polymorph").SetValue(true));
                            cleansedebuffs.AddItem(new MenuItem("qss.stun", "Stun").SetValue(true));
                            cleansedebuffs.AddItem(new MenuItem("qss.suppression", "Suppression").SetValue(true));
                            cleansedebuffs.AddItem(new MenuItem("qss.taunt", "Taunt").SetValue(true));
                            cleanse.AddSubMenu(cleansedebuffs);
                        }
                        cleanse.AddItem(new MenuItem("use.cleanse", "Use Cleanser Item").SetValue(true));
                        cleanse.AddItem(new MenuItem("cleanse.delay", "Max. Cleanse Delay").SetValue(new Slider(1000, 1, 2500)));
                        Activator.AddSubMenu(cleanse);
                    }

                    Activator.AddToMainMenu();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}