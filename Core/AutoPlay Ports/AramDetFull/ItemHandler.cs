using System;
using System.Collections.Generic;
using ARAMDetFull;
using LeagueSharp;
using DetuksSharp;
using LeagueSharp.Common;
using ARAMDetFull = ARAMDetFull.ARAMDetFull;
using Items = EloBuddy.SDK.Item;

using EloBuddy;
namespace ARAMDetFull.Helpers
{

    public class ItemHandler
    {
        public static AIHeroClient player = ObjectManager.Player;
        public static EloBuddy.SDK.Item botrk;
        public static EloBuddy.SDK.Item tiamat;
        public static EloBuddy.SDK.Item hydra;
        public static EloBuddy.SDK.Item randuins;
        public static EloBuddy.SDK.Item odins;
        public static EloBuddy.SDK.Item bilgewater;
        public static EloBuddy.SDK.Item hexgun;
        public static EloBuddy.SDK.Item Dfg;
        public static EloBuddy.SDK.Item Bft;
        public static EloBuddy.SDK.Item sheen;
        public static EloBuddy.SDK.Item gaunlet;
        public static EloBuddy.SDK.Item trinity;
        public static EloBuddy.SDK.Item lich;
        public static EloBuddy.SDK.Item youmuu;
        public static EloBuddy.SDK.Item Qss;
        public static EloBuddy.SDK.Item Mercurial;
        public static EloBuddy.SDK.Item Dervish;
        public static EloBuddy.SDK.Item Zhonya;
        public static EloBuddy.SDK.Item Wooglet;
        public static EloBuddy.SDK.Item locket;

        static ItemHandler()
        {
            botrk = new EloBuddy.SDK.Item(3153, 450);
            tiamat = new EloBuddy.SDK.Item(3077, 400);
            hydra = new EloBuddy.SDK.Item(3074, 400);
            randuins = new EloBuddy.SDK.Item(3143, 500);
            odins = new EloBuddy.SDK.Item(3180, 520);
            bilgewater = new EloBuddy.SDK.Item(3144, 450);
            hexgun = new EloBuddy.SDK.Item(3146, 700);
            Dfg = new EloBuddy.SDK.Item(3128, 750);
            Bft = new EloBuddy.SDK.Item(3188, 750);
            sheen = new EloBuddy.SDK.Item(3057, player.AttackRange);
            gaunlet = new EloBuddy.SDK.Item(3025, player.AttackRange);
            trinity = new EloBuddy.SDK.Item(3078, player.AttackRange);
            lich = new EloBuddy.SDK.Item(3100, player.AttackRange);
            youmuu = new EloBuddy.SDK.Item(3142, player.AttackRange);
            Qss = new EloBuddy.SDK.Item(3140, 0);
            Mercurial = new EloBuddy.SDK.Item(3139, 0);
            Dervish = new EloBuddy.SDK.Item(3137, 0);
            Zhonya = new EloBuddy.SDK.Item(3157, 0);
            Wooglet = new EloBuddy.SDK.Item(3090, 0);
            locket = new EloBuddy.SDK.Item((int)ItemId.Locket_of_the_Iron_Solari, 0);
        }


        public static void useItems()
        {
            try
            {
                var target = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
                if (target != null && target.IsValidTarget())
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

            if (Zhonya.IsOwned() && Zhonya.IsReady())
            {
                if (player.HealthPercent < 30 && ARAMSimulator.balance < -60)
                {
                    Zhonya.Cast();
                }
            }

            if (locket.IsOwned() && locket.IsReady())
            {
                if (player.HealthPercent < 80 && player.CountEnemiesInRange(550) > 0)
                {
                    locket.Cast();
                }
            }

            if (randuins.IsOwned() && randuins.IsReady())
            {
                if (player.Distance(target) < randuins.Range && player.Distance(target) > player.AttackRange + 100)
                {
                    randuins.Cast();
                }
            }

            if (odins.IsReady() && odins.IsOwned())
            {
                if (player.Distance(target) < odins.Range &&
                    (player.CountEnemiesInRange(odins.Range) > 1 ||
                     target.Health < Damage.GetItemDamage(player, target, Damage.DamageItems.OdingVeils)))
                {
                    odins.Cast();
                }
            }

            if (bilgewater.IsOwned() && bilgewater.IsReady())
            {
                bilgewater.Cast(target);
            }

            if (botrk.IsOwned() && botrk.IsReady() && (player.Health < player.MaxHealth / 2 || Damage.GetItemDamage(player, target, Damage.DamageItems.Botrk) < target.Health))
            {
                botrk.Cast(target);
            }

            if (hexgun.IsOwned() && hexgun.IsReady())
            {
                hexgun.Cast(target);
            }

            if (Dfg.IsOwned() && Dfg.IsReady())
            {
                Dfg.Cast(target);
            }

            if (Bft.IsOwned() && Bft.IsReady())
            {
                Bft.Cast(target);
            }

            if (youmuu.IsOwned() && youmuu.IsReady() && player.Distance(target) < player.AttackRange + 50)
            {
                youmuu.Cast();
            }
        }

        public static void castHydra(AIHeroClient target)
        {
            if (player.Distance(target) < hydra.Range && !LeagueSharp.Common.Orbwalking.CanAttack())
            {
                if (tiamat.IsReady() && tiamat.IsOwned())
                {
                    tiamat.Cast();
                }
                if (hydra.IsReady() && hydra.IsOwned())
                {
                    hydra.Cast();
                }
            }
        }

        public static float GetItemsDamage(AIHeroClient target)
        {
            double damage = 0;
            //if (EloBuddy.SDK.Item.HasItem(odins.Id) && EloBuddy.SDK.Item.CanUseItem(odins.Id))
            //{
            //    damage += Damage.GetItemDamage(player, target, Damage.DamageItems.OdingVeils);
            //}
            //if (EloBuddy.SDK.Item.HasItem(hexgun.Id) && EloBuddy.SDK.Item.CanUseItem(hexgun.Id))
            //{
            //    damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Hexgun);
            //}
            //if (EloBuddy.SDK.Item.HasItem(lich.Id) && EloBuddy.SDK.Item.CanUseItem(lich.Id))
            //{
            //    damage += player.CalcDamage(target, Damage.DamageType.Magical, player.BaseAttackDamage * 0.75 + player.FlatMagicDamageMod * 0.5);
            //}
            //if (EloBuddy.SDK.Item.HasItem(Dfg.Id) && EloBuddy.SDK.Item.CanUseItem(Dfg.Id))
            //{
            //    damage = damage * 1.2;
            //    damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Dfg);
            //}
            //if (EloBuddy.SDK.Item.HasItem(Bft.Id) && EloBuddy.SDK.Item.CanUseItem(Bft.Id))
            //{
            //    damage = damage * 1.2;
            //    damage += Damage.GetItemDamage(player, target, Damage.DamageItems.BlackFireTorch);
            //}
            //if (EloBuddy.SDK.Item.HasItem(tiamat.Id) && EloBuddy.SDK.Item.CanUseItem(tiamat.Id))
            //{
            //    damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Tiamat);
            //}
            //if (EloBuddy.SDK.Item.HasItem(hydra.Id) && EloBuddy.SDK.Item.CanUseItem(hydra.Id))
            //{
            //    damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Hydra);
            //}
            //if (EloBuddy.SDK.Item.HasItem(bilgewater.Id) && EloBuddy.SDK.Item.CanUseItem(bilgewater.Id))
            //{
            //    damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Bilgewater);
            //}
            //if (EloBuddy.SDK.Item.HasItem(botrk.Id) && EloBuddy.SDK.Item.CanUseItem(botrk.Id))
            //{
            //    damage += Damage.GetItemDamage(player, target, Damage.DamageItems.Botrk);
            //}
            //if (EloBuddy.SDK.Item.HasItem(sheen.Id) && (EloBuddy.SDK.Item.CanUseItem(sheen.Id) || player.HasBuff("sheen", true)))
            //{
            //    damage += player.CalcDamage(target, Damage.DamageType.Physical, player.BaseAttackDamage);
            //}
            //if (EloBuddy.SDK.Item.HasItem(gaunlet.Id) && EloBuddy.SDK.Item.CanUseItem(gaunlet.Id))
            //{
            //    damage += player.CalcDamage(target, Damage.DamageType.Physical, player.BaseAttackDamage * 1.25);
            //}
            //if (EloBuddy.SDK.Item.HasItem(trinity.Id) && EloBuddy.SDK.Item.CanUseItem(trinity.Id))
            //{
            //    damage += player.CalcDamage(target, Damage.DamageType.Physical, player.BaseAttackDamage * 2);
            //}
            return (float)damage;
        }

    }
}
