using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Akali.Utility
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Enumerations;
    using System;
    using System.Collections.Generic;

    internal static class Defensive
    {
        private static int UseCleanTime, CleanID;
        private static int Dervish = 3137, Mercurial = 3139, Quicksilver = 3140, Mikaels = 3222;
        private static List<BuffType> DebuffTypes = new List<BuffType>();

        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        internal static void Inject()
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
            }

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Defensive"]["Cleanse"]["CleanEnable"].GetValue<MenuBool>())
            {
                Cleanse1();
            }
        }

        private static void Cleanse1()
        {
            if (Menu["Defensive"]["Cleanse"]["CleanOnlyKey"].GetValue<MenuBool>() && !Menu["Defensive"]["Cleanse"]["CleanActiveKey"].GetValue<MenuKeyBind>().Active)
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

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanblind"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Blind);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleancharm"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Charm);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanfear"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Fear);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanflee"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Flee);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanstun"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Stun);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleansnare"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Snare);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleantaunt"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Taunt);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleansuppression"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Suppression);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanpolymorph"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Polymorph);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleansilence"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Blind);

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleansilence"].GetValue<MenuBool>())
                DebuffTypes.Add(BuffType.Silence);

            foreach (var buff in hero.Buffs)
            {
                if (DebuffTypes.Contains(buff.Type) && (buff.EndTime - Game.Time) * 1000 >= Menu["Defensive"]["Cleanse"]["CleanBuffTime"].GetValue<MenuSlider>().Value && buff.IsActive)
                {
                    CanUse = true;
                }
            }

            if (Menu["Defensive"]["Cleanse"]["Debuffs"]["Cleanexhaust"].GetValue<MenuBool>() && hero.HasBuff("CleanSummonerExhaust"))
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