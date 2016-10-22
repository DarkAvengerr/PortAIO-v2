using System;
using System.Drawing;
using LeagueSharp.Common;
using VayneHunter_Reborn.External;
using VayneHunter_Reborn.External.Cleanser;
using VayneHunter_Reborn.External.ProfileSelector;
using VayneHunter_Reborn.External.Translation;
using Activator = VayneHunter_Reborn.External.Activator.Activator;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Utility.MenuUtility
{
    using LeagueSharp;

    class MenuGenerator
    {
        public static void OnLoad()
        {
            var RootMenu = Variables.Menu;

            var OWMenu = new Menu("[VHR] Orbwalker", "dz191.vhr.orbwalker");
            {
                Variables.Orbwalker = new Orbwalking.Orbwalker(OWMenu);
                RootMenu.AddSubMenu(OWMenu);
            }

            var TSMenu = new Menu("[VHR] TS", "dz191.vhr.ts");
            {
                TargetSelector.AddToMenu(TSMenu);
                RootMenu.AddSubMenu(TSMenu);
            }

            var comboMenu = new Menu("[VHR] Combo", "dz191.vhr.combo");
            {
                var manaMenu = new Menu("Mana Manager", "dz191.vhr.combo.mm");
                {
                    manaMenu.AddManaLimiter(Enumerations.Skills.Q, Orbwalking.OrbwalkingMode.Combo);
                    manaMenu.AddManaLimiter(Enumerations.Skills.E, Orbwalking.OrbwalkingMode.Combo);
                    manaMenu.AddManaLimiter(Enumerations.Skills.R, Orbwalking.OrbwalkingMode.Combo);
                    
                    comboMenu.AddSubMenu(manaMenu);
                }

                comboMenu.AddSkill(Enumerations.Skills.Q, Orbwalking.OrbwalkingMode.Combo);
                comboMenu.AddSkill(Enumerations.Skills.E, Orbwalking.OrbwalkingMode.Combo);
                comboMenu.AddSkill(Enumerations.Skills.R, Orbwalking.OrbwalkingMode.Combo, false);

                comboMenu.AddSlider("dz191.vhr.combo.r.minenemies", "Min. R Enemies", new Tuple<int, int, int>(2, 1, 5)).SetTooltip("Minimum enemies in range for R");
                comboMenu.AddBool("dz191.vhr.combo.q.2wstacks", "Only Q if 2W Stacks on Target").SetTooltip("Will Q for 3rd proc only. Enable if you want AA AA Q AA");

                RootMenu.AddSubMenu(comboMenu);
            }

            var harassMenu = new Menu("[VHR] Harass", "dz191.vhr.mixed");
            {
                var manaMenu = new Menu("Mana Manager", "dz191.vhr.mixed.mm");
                {
                    manaMenu.AddManaLimiter(Enumerations.Skills.Q, Orbwalking.OrbwalkingMode.Mixed);
                    manaMenu.AddManaLimiter(Enumerations.Skills.E, Orbwalking.OrbwalkingMode.Mixed);
                    
                    harassMenu.AddSubMenu(manaMenu);
                }

                harassMenu.AddSkill(Enumerations.Skills.Q, Orbwalking.OrbwalkingMode.Mixed);
                harassMenu.AddSkill(Enumerations.Skills.E, Orbwalking.OrbwalkingMode.Mixed);

                harassMenu.AddBool("dz191.vhr.mixed.q.2wstacks", "Only Q if 2W Stacks on Target").SetTooltip("Will Q for 3rd proc only. Enable if you want AA AA Q AA");
                harassMenu.AddBool("dz191.vhr.mixed.ethird", "Use E for Third Proc").SetTooltip("Uses E for 3rd W proc. Enable if you want AA Q AA E");

                RootMenu.AddSubMenu(harassMenu);
            }

            var farmMenu = new Menu("[VHR] Farm", "dz191.vhr.farm");
            {
                farmMenu.AddSkill(Enumerations.Skills.Q, Orbwalking.OrbwalkingMode.LaneClear).SetTooltip("Q Laneclear");
                farmMenu.AddManaLimiter(Enumerations.Skills.Q, Orbwalking.OrbwalkingMode.LaneClear, 45, true);
                farmMenu.AddSkill(Enumerations.Skills.Q, Orbwalking.OrbwalkingMode.LastHit).SetTooltip("Q Lasthit");
                farmMenu.AddManaLimiter(Enumerations.Skills.Q, Orbwalking.OrbwalkingMode.LastHit, 45, true);
                farmMenu.AddBool("dz191.vhr.farm.condemnjungle", "Use E to condemn jungle mobs", true).SetTooltip("Use Condemn against jungle creeps");
                farmMenu.AddBool("dz191.vhr.farm.qjungle", "Use Q against jungle mobs", true).SetTooltip("Use Tumble in the Jungle");

                RootMenu.AddSubMenu(farmMenu);
            }

            var miscMenu = new Menu("[VHR] Misc", "dz191.vhr.misc");
            {
                var miscQMenu = new Menu("Misc - Q (Tumble)", "dz191.vhr.misc.tumble");
                {
                    miscQMenu.AddStringList("dz191.vhr.misc.condemn.qlogic", "Q Logic", new[] { "Reborn", "Normal", "Kite melees", "Kurisu" }).SetTooltip("The Tumble Method. Reborn = Safest & Besto");
                    miscQMenu.AddBool("dz191.vhr.mixed.mirinQ", "Q to Wall when Possible (Mirin Mode)", true).SetTooltip("Will Q to walls when possible for really fast bursts!");
                    miscQMenu.AddBool("dz191.vhr.misc.tumble.smartq", "Try to QE when possible").SetTooltip("Will try to do the Tumble + Condemn combo when possible"); //Done
                    miscQMenu.AddKeybind("dz191.vhr.misc.tumble.noaastealthex", "Don't AA while stealthed", new Tuple<uint, KeyBindType>('K', KeyBindType.Toggle)).SetTooltip("Will not AA while you are in Ult+Q"); //Done
                    miscQMenu.AddSlider("dz191.vhr.misc.tumble.noaastealthex.hp", "^ Only if HP % < x", new Tuple<int, int, int>(35, 0, 100)).SetTooltip("If true it will not Q into 2 or more enemies"); //done
                    miscQMenu.AddBool("dz191.vhr.misc.tumble.ijava", "iJava Stealth").SetTooltip("If you are not iJava, Don't press me :^)"); //Done
                    miscQMenu.AddSlider("dz191.vhr.misc.tumble.noaastealth.duration", "Duration to wait (iJava Only)", new Tuple<int, int, int>(700, 0, 1000));
                    miscQMenu.AddBool("dz191.vhr.misc.tumble.noqenemies", "Don't Q into enemies").SetTooltip("If true it will not Q into 2 or more enemies"); //done
                    miscQMenu.AddBool("dz191.vhr.misc.tumble.noqenemies.old", "Use Old Don't Q into enemies").SetTooltip("Uses the old algorithm."); //done
                    miscQMenu.AddBool("dz191.vhr.misc.tumble.dynamicqsafety", "Use dynamic Q Safety Distance").SetTooltip("Use the enemy AA range as the 'Don't Q into enemies' safety distance?"); //done
                    miscQMenu.AddBool("dz191.vhr.misc.tumble.qspam", "Ignore Q checks").SetTooltip("Ignores 'Safe Q' and 'Don't Q into enemies' checks"); //Done
                    miscQMenu.AddBool("dz191.vhr.misc.tumble.qinrange", "Q For KS", true).SetTooltip("Uses Q to KS by Qing in range if you can kill with Q + AA"); //Done
                    miscQMenu.AddSlider(
                        "dz191.vhr.misc.tumble.noaa.enemies", "Min Enemies for No AA Stealth",
                        new Tuple<int, int, int>(3, 2, 5));
                    miscQMenu.AddBool("dz191.vhr.misc.tumble.autoQR", "Automatically use Q after R", true).SetTooltip("Automatically uses Q after R if enabled"); //Done

                    miscMenu.AddSubMenu(miscQMenu);
                }

                var miscEMenu = new Menu("Misc - E (Condemn)", "dz191.vhr.misc.condemn");
                {
                    miscEMenu.AddStringList("dz191.vhr.misc.condemn.condemnmethod", "Condemn Method",
                        new[] { "VH Revolution", "VH Reborn", "Marksman/Gosu", "Shine#" }).SetTooltip("The condemn method. Recommended: Revolution > Shine/Reborn > Marksman");

                    miscEMenu.AddSlider("dz191.vhr.misc.condemn.pushdistance", "E Push Distance",
                        new Tuple<int, int, int>(420, 350, 470)).SetTooltip("The E Knockback distance the script uses. Recommended: 400-430");

                    miscEMenu.AddSlider("dz191.vhr.misc.condemn.accuracy", "Accuracy (Revolution Only)",
                        new Tuple<int, int, int>(45, 1, 65)).SetTooltip("The Condemn Accuracy. Recommended value: 40-45");

                    miscEMenu.AddItem(
                        new MenuItem("dz191.vhr.misc.condemn.enextauto", "E Next Auto").SetValue(
                            new KeyBind('T', KeyBindType.Toggle))).SetTooltip("If On it will fire E after the next Auto Attack is landed");

                    miscEMenu.AddItem(
                        new MenuItem("dz191.vhr.misc.condemn.flashcondemn", "Condemn -> Flash").SetValue(
                            new KeyBind('W', KeyBindType.Press)))
                        .SetTooltip("Uses the Condemn -> Flash pro play on an enemy on which it is possible to do so.");

                    miscEMenu.AddBool("dz191.vhr.misc.condemn.onlystuncurrent", "Only stun current target").SetTooltip("Only uses E on the current orbwalker target"); //done
                    miscEMenu.AddBool("dz191.vhr.misc.condemn.autoe", "Auto E").SetTooltip("Uses E whenever possible"); //Done
                    miscEMenu.AddBool("dz191.vhr.misc.condemn.eks", "Smart E KS").SetTooltip("Uses E to KS when they have 2 W Stacks and they can be killed by W + E"); //Done
                    miscEMenu.AddSlider("dz191.vhr.misc.condemn.noeaa", "Don't E if Target can be killed in X AA",
                        new Tuple<int, int, int>(1, 0, 4)).SetTooltip("Does not condemn if you can kill the target in X Auto Attacks"); //Done

                    miscEMenu.AddBool("dz191.vhr.misc.condemn.trinketbush", "Trinket Bush on Condemn", true).SetTooltip("Uses Blue / Yellow trinket on bush if you condemn in there.");
                    miscEMenu.AddBool("dz191.vhr.misc.condemn.lowlifepeel", "Peel with E when low health").SetTooltip("Uses E on melee enemies if your health < 15%");
                    miscEMenu.AddBool("dz191.vhr.misc.condemn.condemnflag", "Condemn to J4 flag", true).SetTooltip("Tries to make the assembly condemn on J4 Flags");
                    miscEMenu.AddBool("dz191.vhr.misc.condemn.noeturret", "No E Under enemy turret").SetTooltip("Does not condemn if you are under their turret");
                    miscEMenu.AddBool("dz191.vhr.misc.condemn.repelflash", "Use E on Enemy Flashes").SetTooltip("Uses E on enemy flashes that get too close.");
                    miscEMenu.AddBool("dz191.vhr.misc.condemn.repelkindred", "Use E to push enemies out of kindred ult").SetTooltip("Uses E on enemies inside Kindred's ult.");

                    miscMenu.AddSubMenu(miscEMenu);
                }

                var miscGeneralSubMenu = new Menu("Misc - General", "dz191.vhr.misc.general"); //Done
                {
                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.antigp", "Anti Gapcloser").SetTooltip("Uses E to stop gapclosers");
                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.interrupt", "Interrupter", true).SetTooltip("Uses E to interrupt skills");
                    miscGeneralSubMenu.AddSlider("dz191.vhr.misc.general.antigpdelay", "Anti Gapcloser Delay (ms)",
                        new Tuple<int, int, int>(0, 0, 1000)).SetTooltip("Sets a delay before the Condemn for Antigapcloser is casted.");

                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.specialfocus", "Focus targets with 2 W marks").SetTooltip("Tries to focus targets that have 2W Rings on them");
                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.reveal", "Stealth Reveal (Pink Ward / Lens)").SetTooltip("Reveals stealthed champions using Pink Wards / Lenses");

                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.disablemovement", "Disable Orbwalker Movement").SetTooltip("Disables the Orbwalker movements as long as it's active");
                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.disableattk", "Disable Orbwalker Attack").SetTooltip("Disables the Orbwalker attacks as long as it's active");
                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.turnaround", "Use Turnaround against Cassio R / Trynda W", true).SetTooltip("Turns around to prevent Slows / Stuns from Cassio R and Trynda W");
                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.threshCatch", "Catch Thresh Lantern", true).SetTooltip("Autocatches Thresh Lantern");
                    miscGeneralSubMenu.AddSlider("dz191.vhr.misc.general.hpThresh", "^ When HP Below %", new Tuple<int, int, int>(20, 0, 100)).SetTooltip("Autocatches Thresh Lantern at % HP < Value only");
                    miscGeneralSubMenu.AddBool("dz191.vhr.misc.general.blueTrinket", "Buy Blue Trinket").SetTooltip("Buys Blue trinket automagically at Level 9");
                    miscMenu.AddSubMenu(miscGeneralSubMenu);
                }

                RootMenu.AddSubMenu(miscMenu);
            }

            var drawMenu = new Menu("[VHR] Drawings", "dz191.vhr.draw");
            {
                drawMenu.AddBool("dz191.vhr.draw.spots", "Draw Spots", true);
                drawMenu.AddBool("dz191.vhr.draw.range", "Draw Enemy Ranges", true);
                drawMenu.AddBool("dz191.vhr.draw.condemn", "Draw Condemn Rectangles", true);
                drawMenu.AddBool("dz191.vhr.draw.qpos", "Reborn Q Position (Debug)");
                drawMenu.AddBool("dz191.vhr.draw.trapDraw", "Draw Traps (Teemo / Jinx / Cait)").SetTooltip("Draws Traps for Teemo / Jinx / Cait");

                RootMenu.AddSubMenu(drawMenu);
            }

            //CustomAntigapcloser.BuildMenu(RootMenu);
            DZAntigapcloserVHR.BuildMenu(RootMenu, "[VHR] AntiGapclosers List", "dz191.vhr.agplist");
            External.Activator.Activator.LoadMenu();
            Cleanser.LoadMenu(RootMenu);
            ProfileSelector.OnLoad(RootMenu);
            TranslationInterface.OnLoad(RootMenu);

            RootMenu.AddToMainMenu();
        }
    }
}
