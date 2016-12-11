using EloBuddy; 
using LeagueSharp.Common; 
namespace ADCCOMMON
{
    using System;
    using System.Collections.Generic;
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class CleaneseManager
    {
        private static int useCleanTime;
        private static Menu cleanMenu;
        private static readonly List<BuffType> debuffTypes = new List<BuffType>();

        public static void AddToMenu(Menu mainMenu)
        {
            cleanMenu = mainMenu;

            var Debuff = cleanMenu.AddSubMenu(new Menu("Debuffs", "Debuffs"));
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
            cleanMenu.AddItem(new MenuItem("CleanEnable", "Enabled", true).SetValue(true));
            cleanMenu.AddItem(new MenuItem("CleanDelay", "Clean Delay(ms)", true).SetValue(new Slider(0, 0, 2000)));
            cleanMenu.AddItem(
                new MenuItem("CleanBuffTime", "Debuff Less End Times(ms)", true).SetValue(new Slider(800, 0, 1000)));
            cleanMenu.AddItem(new MenuItem("CleanOnlyKey", "Only Combo Mode Active?", true).SetValue(true));

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (cleanMenu.Item("CleanEnable", true).GetValue<bool>())
            {
                if (cleanMenu.Item("CleanOnlyKey", true).GetValue<bool>() &&
                    !Orbwalking.isCombo)
                {
                    return;
                }

                if (CanClean(ObjectManager.Player) && Utils.TickCount > useCleanTime)
                {
                    if (Items.HasItem(3140, ObjectManager.Player) && Items.CanUseItem(3140))
                    {
                        Items.UseItem(3140, ObjectManager.Player);
                        useCleanTime = Utils.TickCount + 3000;
                    }
                    else if (Items.HasItem(3139, ObjectManager.Player) && Items.CanUseItem(3139))
                    {
                        Items.UseItem(3139, ObjectManager.Player);
                        useCleanTime = Utils.TickCount + 3000;
                    }
                    else if (Items.HasItem(3222, ObjectManager.Player) && Items.CanUseItem(3222))
                    {
                        Items.UseItem(3222, ObjectManager.Player);
                        useCleanTime = Utils.TickCount + 3000;
                    }
                    else if (Items.HasItem(3137, ObjectManager.Player) && Items.CanUseItem(3137))
                    {
                        Items.UseItem(3137, ObjectManager.Player);
                        useCleanTime = Utils.TickCount + 3000;
                    }
                }
            }
        }

        private static bool CanClean(AIHeroClient hero)
        {
            var CanUse = false;

            if (useCleanTime > Utils.TickCount)
            {
                return false;
            }

            if (cleanMenu.Item("Cleanblind", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Blind);
            }

            if (cleanMenu.Item("Cleancharm", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Charm);
            }

            if (cleanMenu.Item("Cleanfear", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Fear);
            }

            if (cleanMenu.Item("Cleanflee", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Flee);
            }

            if (cleanMenu.Item("Cleanstun", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Stun);
            }

            if (cleanMenu.Item("Cleansnare", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Snare);
            }

            if (cleanMenu.Item("Cleantaunt", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Taunt);
            }

            if (cleanMenu.Item("Cleansuppression", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Suppression);
            }

            if (cleanMenu.Item("Cleanpolymorph", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Polymorph);
            }

            if (cleanMenu.Item("Cleansilence", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Blind);
            }

            if (cleanMenu.Item("Cleansilence", true).GetValue<bool>())
            {
                debuffTypes.Add(BuffType.Silence);
            }

            foreach (var buff in hero.Buffs)
            {
                if (debuffTypes.Contains(buff.Type) &&
                    (buff.EndTime - Game.Time) * 1000 >= cleanMenu.Item("CleanBuffTime", true).GetValue<Slider>().Value &&
                    buff.IsActive)
                {
                    CanUse = true;
                }
            }

            if (cleanMenu.Item("Cleanexhaust", true).GetValue<bool>() && hero.HasBuff("CleanSummonerExhaust"))
            {
                CanUse = true;
            }

            useCleanTime = Utils.TickCount + cleanMenu.Item("CleanDelay", true).GetValue<Slider>().Value;

            return CanUse;
        }
    }
}
