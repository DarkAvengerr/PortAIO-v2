using System;
using System.Collections.Generic;

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace grabEvade
{
    class Program
    {
        private static List<string> supportedChamp = new List<string> { "Heimerdinger", "Zyra", "Shaco", "Yorick", "Yasuo", "Sivir",
                                                                        "Nocturne", "Morgana", "Fizz", "Annie", "Blitzcrank", "Thresh", 
                                                                        "Ezreal", "Trundle" };

        private static List<string> adcDataBase = new List<string> { "Vayne", "Varus", "Urgot", "Twitch", "Tristana", "Sivir", "Miss Fortune",
                                                                     "Lucian", "Kog'Maw", "Kalista", "Jinx", "Graves", "Ezreal", "Draven",
                                                                     "Corki", "Caitlyn", "Ashe" };
        private const string scriptName = "grabEvade";
        private static AIHeroClient champion = ObjectManager.Player;

        private static Spell Q, W, E, R;

        public static List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        private static Menu generalMenu;

        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs argv)
        {
            if (supportedChamp.Contains(champion.ChampionName))
            {
                generalMenu = new Menu(scriptName, "general", true);
                generalMenu.AddItem(new MenuItem("enable", "Enable", false)).SetValue(true);
                Menu adcModeSettingsMenu = new Menu("ADCMode", "admenu");
                adcModeSettingsMenu.AddItem(new MenuItem("adcmode", "ADC Mode", false)).SetValue(false);
                generalMenu.AddSubMenu(adcModeSettingsMenu);

                Menu champSupportedMenu = new Menu("grabToEvade", "champ");
                champSupportedMenu.AddItem(new MenuItem("blitzok", "Blitzcrank", false)).SetValue(true);
                champSupportedMenu.AddItem(new MenuItem("threshok", "Thresh", false)).SetValue(true);
                champSupportedMenu.AddItem(new MenuItem("morganaok", "Morgana", false)).SetValue(true);
                champSupportedMenu.AddItem(new MenuItem("amumuok", "Amumu", false)).SetValue(true);
                generalMenu.AddSubMenu(champSupportedMenu);
                generalMenu.AddToMainMenu();

                Q = new Spell(SpellSlot.Q);
                W = new Spell(SpellSlot.W);
                E = new Spell(SpellSlot.E);
                R = new Spell(SpellSlot.R);

                Game.OnUpdate += OnGameUpdate;
                SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
           }
        }   

        private static void OnGameUpdate(EventArgs argv)
        {
            if (generalMenu.Item("enable").GetValue<bool>())
            {
                foreach (var shot in DetectedSkillshots)
                {
                    if (generalMenu.Item("adcmode").GetValue<bool>())
                    {
                        foreach (AIHeroClient champ in champion.GetAlliesInRange(1000))
                        {
                            if ((champ.IsAlly) & (adcDataBase.Contains(champ.ChampionName)))
                            {
                                evadeGrabADCMode(shot, champ);
                            }
                        }
                    }
                    else
                    {
                        evadeGrabSelfMode(shot);
                    }
                }
            }
        }

        //Credits to detuks and his Yasuo script for some part of this
        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            var alreadyAdded = false;

            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    (item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                     (skillshot.Direction).AngleBetween(item.Direction) < 5 &&
                     (skillshot.Start.Distance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0)))
                {
                    alreadyAdded = true;
                }
            }

            //Check if the skillshot is from an ally.
            if (skillshot.Unit.Team == champion.Team)
            {
                return;
            }

            //Check if the skillshot is too far away.
            if (skillshot.Start.Distance(champion.ServerPosition.To2D()) >
                (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
            {
                return;
            }

            //Add the skillshot to the detected skillshot list.
            if (!alreadyAdded)
            {
                DetectedSkillshots.Add(skillshot);
            }
        }

        private static void evadeGrabADCMode(Skillshot skillShot, AIHeroClient mate)
        {
            if (((skillShot.SpellData.MissileSpellName == "RocketGrabMissile") & (generalMenu.Item("blitzok").GetValue<bool>())) |
                ((skillShot.SpellData.MissileSpellName == "ThreshQMissile") & (generalMenu.Item("threshok").GetValue<bool>())) |
                ((skillShot.SpellData.MissileSpellName == "DarkBindingMissile") & (generalMenu.Item("morganaok").GetValue<bool>())) |
                ((skillShot.SpellData.MissileSpellName == "SadMummyBandageToss") & (generalMenu.Item("amumuok").GetValue<bool>())))
            {
                if (!skillShot.IsSafe(mate.ServerPosition.To2D()))
                {
                    if (champion.ChampionName == "Zyra")
                    {
                        W.Cast(mate.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                        E.Cast(mate.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Heimerdinger")
                    {
                        Q.Cast(mate.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Shaco")
                    {
                        W.Cast(mate.Position.Extend(skillShot.Unit.Position, +50), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Yorick")
                    {
                        W.Cast(mate.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Yasuo")
                    {
                        W.Cast(mate.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Morgana")
                    {
                        E.CastOnUnit(mate);
                    }
                    else if (champion.ChampionName == "Annie")
                    {
                        R.Cast(mate.Position.Extend(skillShot.Unit.Position, +50), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Thresh")
                    {
                        var prediction = Q.GetPrediction(skillShot.Unit);
                        Q.Cast(prediction.CastPosition);
                    }
                    else if (champion.ChampionName == "Blitzcrank")
                    {
                        var prediction = Q.GetPrediction(skillShot.Unit);
                        Q.Cast(prediction.CastPosition);
                    }
                }
                DetectedSkillshots.Clear();
            }
        }

        private static void evadeGrabSelfMode(Skillshot skillShot)
        {
            if (((skillShot.SpellData.MissileSpellName == "RocketGrabMissile") & (generalMenu.Item("blitzok").GetValue<bool>())) | 
                ((skillShot.SpellData.MissileSpellName == "ThreshQMissile") & (generalMenu.Item("threshok").GetValue<bool>())) |
                ((skillShot.SpellData.MissileSpellName == "DarkBindingMissile") & (generalMenu.Item("morganaok").GetValue<bool>())) |
                ((skillShot.SpellData.MissileSpellName == "SadMummyBandageToss") & (generalMenu.Item("amumuok").GetValue<bool>())))
            {
                if (!skillShot.IsSafe(champion.ServerPosition.To2D()))
                {
                    if (champion.ChampionName == "Zyra")
                    {
                        W.Cast(champion.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                        E.Cast(champion.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                    }
                    else if(champion.ChampionName == "Heimerdinger")
                    {
                        Q.Cast(champion.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Shaco")
                    {
                        W.Cast(champion.Position.Extend(skillShot.Unit.Position, +50), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Yorick")
                    {
                        W.Cast(champion.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Yasuo")
                    {
                        W.Cast(champion.Position.Extend(skillShot.Unit.Position, +75), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Sivir")
                    {
                        E.Cast();
                    }
                    else if (champion.ChampionName == "Nocturne")
                    {
                        W.Cast();
                    }
                    else if (champion.ChampionName == "Morgana")
                    {
                        E.CastOnUnit(champion);
                    }
                    else if (champion.ChampionName == "Fizz")
                    {
                        E.Cast(Game.CursorPos);
                    }
                    else if (champion.ChampionName == "Annie")
                    {
                        R.Cast(champion.Position.Extend(skillShot.Unit.Position, +50), skillShot.Unit.Position);
                    }
                    else if (champion.ChampionName == "Thresh")
                    {
                        var prediction = Q.GetPrediction(skillShot.Unit);
                        Q.Cast(prediction.CastPosition);
                    }
                    else if (champion.ChampionName == "Blitzcrank")
                    {
                        var prediction = Q.GetPrediction(skillShot.Unit);
                        Q.Cast(prediction.CastPosition);
                    }
                    else if (champion.ChampionName == "Ezreal")
                    {
                        E.Cast(Game.CursorPos);
                    }
                    else if (champion.ChampionName == "Trundle")
                    {
                        Console.Write("gogogogogogogog");
                        LeagueSharp.Common.Utility.DelayAction.Add((int)champion.Distance(skillShot.Unit) / 2, () => E.Cast(champion.Position.Extend(skillShot.Unit.Position, +50), skillShot.Unit.Position));
                    }
                }
                DetectedSkillshots.Clear();
            }
        }
    }
}
