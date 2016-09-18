using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Toolss
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    using System;
    using System.Collections.Generic;

    using Config;

    using Menu = LeagueSharp.SDK.UI.Menu;

    internal static class QSS
    {
        private static int UseCleanTime;
        private static List<BuffType> DebuffTypes = new List<BuffType>();

        private static AIHeroClient Player => PlaySharp.Player;

        private static Menu Menu => Tools.Menu;

        internal static void Init()
        {
            var QSSMenu = Menu.Add(new Menu("QSS", "QSS Set"));
            {
                var CleanseMenu = QSSMenu.Add(new Menu("CleanseBuffs", "Cleanse Buffs"));
                {
                    CleanseMenu.GetSeparator("Buffs");
                    CleanseMenu.GetBool("blind", "Blind", false);
                    CleanseMenu.GetBool("charm", "Charm", false);
                    CleanseMenu.GetBool("fear", "Fear", false);
                    CleanseMenu.GetBool("flee", "Flee", false);
                    CleanseMenu.GetBool("stun", "Stun", false);
                    CleanseMenu.GetBool("snare", "Snare", false);
                    CleanseMenu.GetBool("taunt", "Taunt", false);
                    CleanseMenu.GetBool("exhaust", "Exhaust", false);
                    CleanseMenu.GetBool("suppression", "Suppression");
                    CleanseMenu.GetBool("polymorph", "Polymorph");
                    CleanseMenu.GetBool("silence", "Silence");
                }
                QSSMenu.GetSeparator("Mode");
                QSSMenu.GetBool("CleanEnable", "Enable", false);
                QSSMenu.GetSlider("CleanDelay", "Clean Delay(ms)", 1000, 0, 2000);
                QSSMenu.GetSlider("CleanBuffTime", "Debuff Less End Times(ms)", 800, 0, 1000);
            }
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (Menu["QSS"]["CleanEnable"].GetValue<MenuBool>())
            {
                if (UseQSS(Player))
                {
                    UseCleanTime = Variables.TickCount + 2000;
                }                 
            }
        }

        private static bool UseQSS(AIHeroClient hero)
        {
            bool CanUse = false;

            if (UseCleanTime > Variables.TickCount)
            {
                return CanUse;
            }

            if (Menu["QSS"]["CleanseBuffs"]["blind"] && Player.HasBuffOfType(BuffType.Blind))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["charm"] && Player.HasBuffOfType(BuffType.Charm))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["fear"] && Player.HasBuffOfType(BuffType.Fear))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["stun"] && Player.HasBuffOfType(BuffType.Stun))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["snare"] && Player.HasBuffOfType(BuffType.Snare))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["taunt"] && Player.HasBuffOfType(BuffType.Taunt))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["suppression"] && Player.HasBuffOfType(BuffType.Suppression))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["polymorph"] && Player.HasBuffOfType(BuffType.Polymorph))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["silence"] && Player.HasBuffOfType(BuffType.Silence))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }

            foreach (var buff in hero.Buffs)
            {
                if (DebuffTypes.Contains(buff.Type) && Menu["QSS"]["CleanBuffTime"].GetValue<MenuSlider>().Value >= (buff.EndTime - Game.Time) * 1000)
                {
                    CanUse = true;
                }
            }
            if (Menu["QSS"]["CleanseBuffs"]["exhaust"].GetValue<MenuBool>() && hero.HasBuff("CleanSummonerExhaust"))
            {
                CanUse = true;
            }

            Menu["QSS"]["CleanDelay"].GetValue<MenuSlider>().Value += UseCleanTime = Variables.TickCount;
            return CanUse;
        }
    }
}