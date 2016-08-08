using System;
using System.Collections.Generic;
using ARAMDetFull;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using ARAMDetFull = ARAMDetFull.ARAMDetFull;
using Items = LeagueSharp.Common.Items;

using EloBuddy; namespace ARAMDetFull.Helpers
{

    public class ItemHandler
    {
        public static AIHeroClient player = ObjectManager.Player;
        public static LeagueSharp.Common.Items.Item botrk = new LeagueSharp.Common.Items.Item(3153, 450);
        public static LeagueSharp.Common.Items.Item tiamat = new LeagueSharp.Common.Items.Item(3077, 400);
        public static LeagueSharp.Common.Items.Item hydra = new LeagueSharp.Common.Items.Item(3074, 400);
        public static LeagueSharp.Common.Items.Item randuins = new LeagueSharp.Common.Items.Item(3143, 500);
        public static LeagueSharp.Common.Items.Item odins = new LeagueSharp.Common.Items.Item(3180, 520);
        public static LeagueSharp.Common.Items.Item bilgewater = new LeagueSharp.Common.Items.Item(3144, 450);
        public static LeagueSharp.Common.Items.Item hexgun = new LeagueSharp.Common.Items.Item(3146, 700);
        public static LeagueSharp.Common.Items.Item Dfg = new LeagueSharp.Common.Items.Item(3128, 750);
        public static LeagueSharp.Common.Items.Item Bft = new LeagueSharp.Common.Items.Item(3188, 750);
        public static LeagueSharp.Common.Items.Item sheen = new LeagueSharp.Common.Items.Item(3057, player.AttackRange);
        public static LeagueSharp.Common.Items.Item gaunlet = new LeagueSharp.Common.Items.Item(3025, player.AttackRange);
        public static LeagueSharp.Common.Items.Item trinity = new LeagueSharp.Common.Items.Item(3078, player.AttackRange);
        public static LeagueSharp.Common.Items.Item lich = new LeagueSharp.Common.Items.Item(3100, player.AttackRange);
        public static LeagueSharp.Common.Items.Item youmuu = new LeagueSharp.Common.Items.Item(3142, player.AttackRange);

        public static LeagueSharp.Common.Items.Item Qss = new LeagueSharp.Common.Items.Item(3140, 0);
        public static LeagueSharp.Common.Items.Item Mercurial = new LeagueSharp.Common.Items.Item(3139, 0);
        public static LeagueSharp.Common.Items.Item Dervish = new LeagueSharp.Common.Items.Item(3137, 0);
        public static LeagueSharp.Common.Items.Item Zhonya = new LeagueSharp.Common.Items.Item(3157, 0);
        public static LeagueSharp.Common.Items.Item Wooglet = new LeagueSharp.Common.Items.Item(3090, 0);
        public static LeagueSharp.Common.Items.Item locket = new LeagueSharp.Common.Items.Item((int)ItemId.Locket_of_the_Iron_Solari, 0);


        public static void useItems()
        {
            try
            {

                var target = ARAMTargetSelector.getBestTarget(500);
                if(target != null)
                    UseItems(target);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void UseItems(AIHeroClient target)
        {
            if (player.BaseSkinName != "Renekton")
            {
                castHydra(target);
            }

            if (LeagueSharp.Common.Items.HasItem(Zhonya.Id) && LeagueSharp.Common.Items.CanUseItem(Zhonya.Id))
            {
                if (player.HealthPercent<30 && ARAMSimulator.balance<-60)
                {
                    LeagueSharp.Common.Items.UseItem(Zhonya.Id);
                }
            }
            if (LeagueSharp.Common.Items.HasItem(locket.Id) && LeagueSharp.Common.Items.CanUseItem(locket.Id))
            {
                if (player.HealthPercent < 80 && player.LSCountEnemiesInRange(550)>0)
                {
                    LeagueSharp.Common.Items.UseItem(locket.Id);
                }
            }
            if (LeagueSharp.Common.Items.HasItem(randuins.Id) && LeagueSharp.Common.Items.CanUseItem(randuins.Id))
            {
                if (player.LSDistance(target) < randuins.Range && player.LSDistance(target) > player.AttackRange + 100)
                {
                    LeagueSharp.Common.Items.UseItem(randuins.Id);
                }
            }
            if (LeagueSharp.Common.Items.HasItem(odins.Id) && LeagueSharp.Common.Items.CanUseItem(odins.Id))
            {
                if (player.LSDistance(target) < odins.Range &&
                    (player.LSCountEnemiesInRange(odins.Range) > 1 ||
                     target.Health < Damage.GetItemDamage(player, target, Damage.DamageItems.OdingVeils)))
                {
                    LeagueSharp.Common.Items.UseItem(odins.Id);
                }
            }
            if (LeagueSharp.Common.Items.HasItem(bilgewater.Id) && LeagueSharp.Common.Items.CanUseItem(bilgewater.Id))
            {
                bilgewater.Cast(target);
            }
            if (LeagueSharp.Common.Items.HasItem(botrk.Id) && LeagueSharp.Common.Items.CanUseItem(botrk.Id) && (player.Health < player.MaxHealth / 2 || Damage.GetItemDamage(player, target, Damage.DamageItems.Botrk) < target.Health))
            {
                botrk.Cast(target);
            }
            if (LeagueSharp.Common.Items.HasItem(hexgun.Id) && LeagueSharp.Common.Items.CanUseItem(hexgun.Id))
            {
                hexgun.Cast(target);
            }
            if (LeagueSharp.Common.Items.HasItem(Dfg.Id) && LeagueSharp.Common.Items.CanUseItem(Dfg.Id))
            {
                Dfg.Cast(target);
            }
            if (LeagueSharp.Common.Items.HasItem(Bft.Id) && LeagueSharp.Common.Items.CanUseItem(Bft.Id))
            {
                Bft.Cast(target);
            }
            if (LeagueSharp.Common.Items.HasItem(youmuu.Id) && LeagueSharp.Common.Items.CanUseItem(youmuu.Id) && player.LSDistance(target) < player.AttackRange + 50)
            {
                youmuu.Cast();
            }
        }

        public static void castHydra(AIHeroClient target)
        {
            if (player.LSDistance(target) < hydra.Range && !LeagueSharp.Common.Orbwalking.CanAttack())
            {
                if (LeagueSharp.Common.Items.HasItem(tiamat.Id) && LeagueSharp.Common.Items.CanUseItem(tiamat.Id))
                {
                    LeagueSharp.Common.Items.UseItem(tiamat.Id);
                }
                if (LeagueSharp.Common.Items.HasItem(hydra.Id) && LeagueSharp.Common.Items.CanUseItem(hydra.Id))
                {
                    LeagueSharp.Common.Items.UseItem(hydra.Id);
                }
            }
        }

        public static float GetItemsDamage(AIHeroClient target)
        {
            double damage = 0;
            if (LeagueSharp.Common.Items.HasItem(odins.Id) && LeagueSharp.Common.Items.CanUseItem(odins.Id))
            {
                damage += Damage.GetItemDamage(player, target, Damage.DamageItems.OdingVeils);
            }
            if (LeagueSharp.Common.Items.HasItem(hexgun.Id) && LeagueSharp.Common.Items.CanUseItem(hexgun.Id))
            {
                damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Hexgun);
            }
            if (LeagueSharp.Common.Items.HasItem(lich.Id) && LeagueSharp.Common.Items.CanUseItem(lich.Id))
            {
                damage += player.CalcDamage(target, Damage.DamageType.Magical, player.BaseAttackDamage * 0.75 + player.FlatMagicDamageMod * 0.5);
            }
            if (LeagueSharp.Common.Items.HasItem(Dfg.Id) && LeagueSharp.Common.Items.CanUseItem(Dfg.Id))
            {
                damage = damage * 1.2;
                damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Dfg);
            }
            if (LeagueSharp.Common.Items.HasItem(Bft.Id) && LeagueSharp.Common.Items.CanUseItem(Bft.Id))
            {
                damage = damage * 1.2;
                damage += Damage.GetItemDamage(player, target, Damage.DamageItems.BlackFireTorch);
            }
            if (LeagueSharp.Common.Items.HasItem(tiamat.Id) && LeagueSharp.Common.Items.CanUseItem(tiamat.Id))
            {
                damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Tiamat);
            }
            if (LeagueSharp.Common.Items.HasItem(hydra.Id) && LeagueSharp.Common.Items.CanUseItem(hydra.Id))
            {
                damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Hydra);
            }
            if (LeagueSharp.Common.Items.HasItem(bilgewater.Id) && LeagueSharp.Common.Items.CanUseItem(bilgewater.Id))
            {
                damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Bilgewater);
            }
            if (LeagueSharp.Common.Items.HasItem(botrk.Id) && LeagueSharp.Common.Items.CanUseItem(botrk.Id))
            {
                damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Botrk);
            }
            if (LeagueSharp.Common.Items.HasItem(sheen.Id) && (LeagueSharp.Common.Items.CanUseItem(sheen.Id) || player.LSHasBuff("sheen", true)))
            {
                damage += player.CalcDamage(target, Damage.DamageType.Physical, player.BaseAttackDamage);
            }
            if (LeagueSharp.Common.Items.HasItem(gaunlet.Id) && LeagueSharp.Common.Items.CanUseItem(gaunlet.Id))
            {
                damage += player.CalcDamage(target, Damage.DamageType.Physical, player.BaseAttackDamage * 1.25);
            }
            if (LeagueSharp.Common.Items.HasItem(trinity.Id) && LeagueSharp.Common.Items.CanUseItem(trinity.Id))
            {
                damage += player.CalcDamage(target, Damage.DamageType.Physical, player.BaseAttackDamage * 2);
            }
            return (float)damage;
        }

    }
}
