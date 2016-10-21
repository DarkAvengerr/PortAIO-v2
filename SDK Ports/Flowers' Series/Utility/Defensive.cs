using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Utility
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Enumerations;
    using System;
    using System.Collections.Generic;

    public static class Defensive
    {
        private static int UseCleanTime, CleanID;
        private static int Dervish = 3137, Mercurial = 3139, Quicksilver = 3140, Mikaels = 3222, RanduinsOmen = 3143;
        private static List<BuffType> DebuffTypes = new List<BuffType>();

        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        public static void Inject()
        {
            var DefensiveMenu = Menu.Add(new Menu("Defensive", "Defensive"));
            {
                var CleanseMenu = DefensiveMenu.Add(new Menu("Cleanse", "Cleanse"));
                {
                    var Debuff = CleanseMenu.Add(new Menu("Debuffs", "Buffs"));
                    {
                        Debuff.Add(new MenuBool("Cleanblind", "Blind", true));
                        Debuff.Add(new MenuBool("Cleancharm", "Charm", true));
                        Debuff.Add(new MenuBool("Cleanfear", "Fear", true));
                        Debuff.Add(new MenuBool("Cleanflee", "Flee", true));
                        Debuff.Add(new MenuBool("Cleanstun", "Stun", true));
                        Debuff.Add(new MenuBool("Cleansnare", "Snare", true));
                        Debuff.Add(new MenuBool("Cleantaunt", "Taunt", true));
                        Debuff.Add(new MenuBool("Cleansuppression", "Suppression"));
                        Debuff.Add(new MenuBool("Cleanpolymorph", "Polymorph"));
                        Debuff.Add(new MenuBool("Cleansilence", "Silence"));
                        Debuff.Add(new MenuBool("Cleanexhaust", "Exhaust", true));
                    }
                    CleanseMenu.Add(new MenuBool("CleanEnable", "Enable!", true));
                    CleanseMenu.Add(new MenuSlider("CleanDelay", "Clean Delay(ms)", 0, 0, 2000));
                    CleanseMenu.Add(new MenuSlider("CleanBuffTime", "Debuff Less End Times(ms)", 800, 0, 1000));
                    CleanseMenu.Add(new MenuBool("CleanOnlyKey", "Only Key Active", true));
                    CleanseMenu.Add(new MenuKeyBind("CleanActiveKey", "Keys!", System.Windows.Forms.Keys.Space, KeyBindType.Press));
                }

                var Randuin = DefensiveMenu.Add(new Menu("Randuin", "Randuin's Omen"));
                {
                    Randuin.Add(new MenuBool("Enable", "Enabled", Tools.EnableActivator));
                    Randuin.Add(new MenuSlider("Counts", "When Player Counts Enemies >=",3, 1, 5));
                    Randuin.Add(new MenuBool("AntiGap", "Anti GapCloser", true));
                }
            }

            Common.Manager.WriteConsole("DefensiveMenu Load!");

            Game.OnUpdate += Game_OnUpdate;
            Events.OnGapCloser += OnGapCloser;
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            AIHeroClient target = Args.Sender;

            if (target != null && Menu["Defensive"]["Randuin"]["Enable"] && Items.HasItem(RanduinsOmen) && Items.CanUseItem(RanduinsOmen))
            {
                foreach (var hero in GameObjects.AllyHeroes)
                {
                    if (!hero.IsMe)
                        continue;

                    if (!target.IsMelee())
                        continue;

                    if (target.Distance(hero) <= 500 / 2f)
                    {
                        Items.UseItem(RanduinsOmen);
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Defensive"]["Cleanse"]["CleanEnable"])
            {
                Cleanse1();
            }

            if (Menu["Defensive"]["Randuin"]["Enable"] && Items.HasItem(RanduinsOmen) && Items.CanUseItem(RanduinsOmen))
            {
                Randuin();
            }
        }

        private static void Randuin()
        {
            if (Me.CountEnemyHeroesInRange(500) >= Menu["Defensive"]["Randuin"]["Counts"].GetValue<MenuSlider>().Value)
            {
                Items.UseItem(RanduinsOmen);
            }
        }

        private static void Cleanse1()
        {
            if (Menu["Defensive"]["Cleanse"]["CleanOnlyKey"] && !Menu["Defensive"]["Cleanse"]["CleanActiveKey"].GetValue<MenuKeyBind>().Active)
            {
                return;
            }

            if (CanClean(Me))
            {
                if (CanUseDervish() || CanUseMercurial() || CanUseMikaels() || CanUseQuicksilver())
                {
                    Items.UseItem(CleanID);
                    UseCleanTime = Variables.TickCount + 2500;
                }
            }
        }

        private static bool CanClean(AIHeroClient hero)
        {
            bool CanUse = false;

            if (UseCleanTime > Variables.TickCount)
            {
                return CanUse;
            }

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanblind"])
                DebuffTypes.Add(BuffType.Blind);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleancharm"])
                DebuffTypes.Add(BuffType.Charm);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanfear"])
                DebuffTypes.Add(BuffType.Fear);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanflee"])
                DebuffTypes.Add(BuffType.Flee);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanstun"])
                DebuffTypes.Add(BuffType.Stun);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleansnare"])
                DebuffTypes.Add(BuffType.Snare);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleantaunt"])
                DebuffTypes.Add(BuffType.Taunt);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleansuppression"])
                DebuffTypes.Add(BuffType.Suppression);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanpolymorph"])
                DebuffTypes.Add(BuffType.Polymorph);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleansilence"])
                DebuffTypes.Add(BuffType.Blind);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleansilence"])
                DebuffTypes.Add(BuffType.Silence);

            foreach (var buff in hero.Buffs)
            {
                if (DebuffTypes.Contains(buff.Type) && (buff.EndTime - Game.Time) * 1000 >= Menu["Defensive"]["Cleanse"]["CleanBuffTime"].GetValue<MenuSlider>().Value && buff.IsActive)
                {
                    CanUse = true;
                }
            }

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanexhaust"] && hero.HasBuff("CleanSummonerExhaust"))
            {
                CanUse = true;
            }

            UseCleanTime = Variables.TickCount + Menu["Defensive"]["Cleanse"]["CleanDelay"].GetValue<MenuSlider>().Value;

            return CanUse;
        }

        private static bool CanUseQuicksilver()
        {
            if (Items.HasItem(Quicksilver) && Items.CanUseItem(Quicksilver))
            {
                CleanID = Quicksilver;
                return true;
            }
            else
            {
                CleanID = 0;
                return false;
            }
        }

        private static bool CanUseMikaels()
        {
            if (Items.HasItem(Mikaels) && Items.CanUseItem(Mikaels))
            {
                CleanID = Mikaels;
                return true;
            }
            else
            {
                CleanID = 0;
                return false;
            }
        }

        private static bool CanUseMercurial()
        {
            if (Items.HasItem(Mercurial) && Items.CanUseItem(Mercurial))
            {
                CleanID = Mercurial;
                return true;
            }
            else
            {
                CleanID = 0;
                return false;
            }
        }

        private static bool CanUseDervish()
        {
            if (Items.HasItem(Dervish) && Items.CanUseItem(Dervish))
            {
                CleanID = Dervish;
                return true;
            }
            else
            {
                CleanID = 0;
                return false;
            }
        }
    }
}