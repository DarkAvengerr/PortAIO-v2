using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Utility
{
    using System;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;

    internal class Cleaness : Program
    {
        private static int UseCleanTime, CleanID;

        private static readonly int Dervish = 3137;
        private static readonly int Mercurial = 3139;
        private static readonly int Quicksilver = 3140;
        private static readonly int Mikaels = 3222;

        private static readonly List<BuffType> DebuffTypes = new List<BuffType>();

        private new static readonly Menu Menu = Utilitymenu;

        public static void Init()
        {
            var CleanseMenu = Menu.AddSubMenu(new Menu("Cleanse", "Cleanse"));
            {
                var Debuff = CleanseMenu.AddSubMenu(new Menu("Debuffs", "Debuffs"));
                {
                    Debuff.AddItem(new MenuItem("Cleanblind", "Blind", true).SetValue(true));
                    Debuff.AddItem(new MenuItem("Cleancharm", "Charm", true).SetValue(true));
                    Debuff.AddItem(new MenuItem("Cleanfear", "Fear", true).SetValue(true));
                    Debuff.AddItem(new MenuItem("Cleanflee", "Flee", true).SetValue(true));
                    Debuff.AddItem(new MenuItem("Cleanstun", "Stun", true).SetValue(true));
                    Debuff.AddItem(new MenuItem("Cleansnare", "Snare", true).SetValue(true));
                    Debuff.AddItem(new MenuItem("Cleantaunt", "Taunt", true).SetValue(true));
                    Debuff.AddItem(new MenuItem("Cleansuppression", "Suppression", true).SetValue(true));
                    Debuff.AddItem(new MenuItem("Cleanpolymorph", "Polymorph", true).SetValue(false));
                    Debuff.AddItem(new MenuItem("Cleansilence", "Silence", true).SetValue(false));
                    Debuff.AddItem(new MenuItem("Cleanexhaust", "Exhaust", true).SetValue(true));
                }
                CleanseMenu.AddItem(new MenuItem("CleanEnable", "Enabled", true).SetValue(true));
                CleanseMenu.AddItem(new MenuItem("CleanDelay", "Clean Delay(ms)", true).SetValue(new Slider(0, 0, 2000)));
                CleanseMenu.AddItem(
                    new MenuItem("CleanBuffTime", "Debuff Less End Times(ms)", true).SetValue(new Slider(800, 0, 1000)));
                CleanseMenu.AddItem(new MenuItem("CleanOnlyKey", "Only Combo Mode Active?", true).SetValue(true));
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu.Item("CleanEnable", true).GetValue<bool>())
            {
                if (Menu.Item("CleanOnlyKey", true).GetValue<bool>() &&
                    Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                {
                    return;
                }

                if (CanClean(Me))
                {
                    if (CanUseDervish() || CanUseMercurial() || CanUseMikaels() || CanUseQuicksilver())
                    {
                        LeagueSharp.Common.Items.UseItem(CleanID);
                        UseCleanTime = Utils.TickCount + 2500;
                    }
                }
            }
        }

        private static bool CanClean(AIHeroClient hero)
        {
            var CanUse = false;

            if (UseCleanTime > Utils.TickCount)
            {
                return false;
            }

            if (Menu.Item("Cleanblind", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Blind);
            }

            if (Menu.Item("Cleancharm", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Charm);
            }

            if (Menu.Item("Cleanfear", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Fear);
            }

            if (Menu.Item("Cleanflee", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Flee);
            }

            if (Menu.Item("Cleanstun", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Stun);
            }

            if (Menu.Item("Cleansnare", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Snare);
            }

            if (Menu.Item("Cleantaunt", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Taunt);
            }

            if (Menu.Item("Cleansuppression", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Suppression);
            }

            if (Menu.Item("Cleanpolymorph", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Polymorph);
            }

            if (Menu.Item("Cleansilence", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Blind);
            }

            if (Menu.Item("Cleansilence", true).GetValue<bool>())
            {
                DebuffTypes.Add(BuffType.Silence);
            }

            foreach (var buff in hero.Buffs)
            {
                if (DebuffTypes.Contains(buff.Type) &&
                    (buff.EndTime - Game.Time)*1000 >= Menu.Item("CleanBuffTime", true).GetValue<Slider>().Value &&
                    buff.IsActive)
                {
                    CanUse = true;
                }
            }

            if (Menu.Item("Cleanexhaust", true).GetValue<bool>() && hero.HasBuff("CleanSummonerExhaust"))
            {
                CanUse = true;
            }

            UseCleanTime = Utils.TickCount + Menu.Item("CleanDelay", true).GetValue<Slider>().Value;

            return CanUse;
        }

        private static bool CanUseQuicksilver()
        {
            if (LeagueSharp.Common.Items.HasItem(Quicksilver) && LeagueSharp.Common.Items.CanUseItem(Quicksilver))
            {
                CleanID = Quicksilver;
                return true;
            }

            CleanID = 0;
            return false;
        }

        private static bool CanUseMikaels()
        {
            if (LeagueSharp.Common.Items.HasItem(Mikaels) && LeagueSharp.Common.Items.CanUseItem(Mikaels))
            {
                CleanID = Mikaels;
                return true;
            }

            CleanID = 0;
            return false;
        }

        private static bool CanUseMercurial()
        {
            if (LeagueSharp.Common.Items.HasItem(Mercurial) && LeagueSharp.Common.Items.CanUseItem(Mercurial))
            {
                CleanID = Mercurial;
                return true;
            }

            CleanID = 0;
            return false;
        }

        private static bool CanUseDervish()
        {
            if (LeagueSharp.Common.Items.HasItem(Dervish) && LeagueSharp.Common.Items.CanUseItem(Dervish))
            {
                CleanID = Dervish;
                return true;
            }

            CleanID = 0;
            return false;
        }
    }
}
