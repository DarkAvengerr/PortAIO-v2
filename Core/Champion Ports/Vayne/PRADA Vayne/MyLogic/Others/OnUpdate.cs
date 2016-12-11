using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using PRADA_Vayne_Old.MyUtils;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PRADA_Vayne_Old.MyLogic.Others
{
    public static partial class Events
    {
        public static void OnUpdate(EventArgs args)
        {/*
            if (LeagueSharp.Common.Utility.Map.GetMap().Type != LeagueSharp.Common.Utility.Map.MapType.SummonersRift) return;
            if (Heroes.Player.HasBuff("rengarralertsound"))
            {
                if (Items.HasItem((int) ItemId.Oracle_Alteration, Heroes.Player) &&
                    Items.CanUseItem((int) ItemId.Oracle_Alteration))
                {
                    Items.UseItem((int) ItemId.Oracle_Alteration, Heroes.Player.Position);
                }
                else if (Items.HasItem((int) ItemId.Control_Ward, Heroes.Player))
                {
                    Items.UseItem((int) ItemId.Control_Ward, Heroes.Player.Position.Randomize(0, 125));
                }
            }

            var enemyVayne = Heroes.EnemyHeroes.FirstOrDefault(e => e.CharData.BaseSkinName == "Vayne");
            if (enemyVayne != null && enemyVayne.ServerPosition.Distance(Heroes.Player.ServerPosition) < 700 && enemyVayne.HasBuff("VayneInquisition"))
            {
                if (Items.HasItem((int) ItemId.Oracle_Alteration, Heroes.Player) &&
                    Items.CanUseItem((int) ItemId.Oracle_Alteration))
                {
                    Items.UseItem((int) ItemId.Oracle_Alteration, Heroes.Player.Position);
                }
                else if (Items.HasItem((int) ItemId.Control_Ward, Heroes.Player))
                {
                    Items.UseItem((int) ItemId.Control_Ward, Heroes.Player.Position.Randomize(0, 125));
                }
            }
            if (Game.MapId == GameMapId.SummonersRift)
            {
                if (ObjectManager.Player.InFountain() && Shop.IsOpen)
                {
                    if (Heroes.Player.InFountain() && Program.ComboMenu.Item("AutoBuy").GetValue<bool>() &&
                        !Items.HasItem((int) ItemId.Oracle_Alteration, Heroes.Player) && Heroes.Player.Level > 6 &&
                        HeroManager.Enemies.Any(
                            h =>
                                h.CharData.BaseSkinName == "Rengar" || h.CharData.BaseSkinName == "Talon" ||
                                h.CharData.BaseSkinName == "Vayne"))
                    {
                        Heroes.Shop.BuyItem(ItemId.Sweeping_Lens_Trinket);
                    }
                    if (Heroes.Player.InFountain() && Program.ComboMenu.Item("AutoBuy").GetValue<bool>() &&
                        Heroes.Player.Level >= 9 && Items.HasItem((int) ItemId.Sweeping_Lens_Trinket))
                    {
                        Heroes.Shop.BuyItem(ItemId.Oracle_Alteration);
                    }
                }
            }*/
        }

        public static void OnUpdateVHRPlugin(EventArgs args)
        {/*
            if (Heroes.Player.HasBuff("rengarralertsound"))
            {
                if (Items.HasItem((int) ItemId.Oracle_Alteration, Heroes.Player) &&
                    Items.CanUseItem((int) ItemId.Oracle_Alteration))
                {
                    Items.UseItem((int) ItemId.Oracle_Alteration, Heroes.Player.Position);
                }
                else if (Items.HasItem((int) ItemId.Control_Ward, Heroes.Player))
                {
                    Items.UseItem((int) ItemId.Control_Ward, Heroes.Player.Position.Randomize(0, 125));
                }
            }

            var enemyVayne = Heroes.EnemyHeroes.FirstOrDefault(e => e.CharData.BaseSkinName == "Vayne");
            if (enemyVayne != null && enemyVayne.Distance(Heroes.Player) < 700 && enemyVayne.HasBuff("VayneInquisition"))
            {
                if (Items.HasItem((int) ItemId.Oracle_Alteration, Heroes.Player) &&
                    Items.CanUseItem((int) ItemId.Oracle_Alteration))
                {
                    Items.UseItem((int) ItemId.Oracle_Alteration, Heroes.Player.Position);
                }
                else if (Items.HasItem((int) ItemId.Control_Ward, Heroes.Player))
                {
                    Items.UseItem((int) ItemId.Control_Ward, Heroes.Player.Position.Randomize(0, 125));
                }
            }*/
        }
    }
}
