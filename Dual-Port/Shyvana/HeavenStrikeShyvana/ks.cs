using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeShyvana
{
    using static extension;
    using static Program;
    class ks
    {
        public static void UpdateKs()
        {
            foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsZombie))
            {
                if (EKs && E.GetDamage(hero) >= hero.Health)
                {
                    E.Cast(hero);
                }
                if (RKs && R.GetDamage(hero) >= hero.Health)
                {
                    var pred = R.GetPrediction(hero);
                    if (!pred.CastPosition.LSIsWall() && pred.Hitchance >= HitChance.Medium)
                    {
                        R.Cast(pred.CastPosition);
                    }
                }
                if (BotrkKs && ItemData.Blade_of_the_Ruined_King.GetItem().IsReady() 
                    && ItemData.Blade_of_the_Ruined_King.GetItem().IsInRange(hero) &&
                    (Player.CalcDamage(hero, Damage.DamageType.Physical, hero.MaxHealth * 0.1) >= hero.Health 
                    || Player.CalcDamage(hero, Damage.DamageType.Physical, 100) >= hero.Health))
                {
                    ItemData.Blade_of_the_Ruined_King.GetItem().Cast(hero);
                }
                if (CutlassKs && ItemData.Bilgewater_Cutlass.GetItem().IsReady()
                    && ItemData.Bilgewater_Cutlass.GetItem().IsInRange(hero)
                    && (Player.CalcDamage(hero, Damage.DamageType.Magical, 100) >= hero.Health))
                {
                    ItemData.Bilgewater_Cutlass.GetItem().Cast(hero);
                }
                if (TiamatKs && (ItemData.Tiamat_Melee_Only.GetItem().IsReady()
                    || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady()))

                {
                    var pred = Prediction.GetPrediction(hero, Player.AttackCastDelay).UnitPosition;
                    if ((ItemData.Tiamat_Melee_Only.GetItem().IsInRange(pred)
                        || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsInRange(pred))
                        && Player.CalcDamage(hero, Damage.DamageType.Physical,
                        Player.TotalAttackDamage * (1-(Player.Position.LSTo2D().LSDistance(pred.LSTo2D()) - hero.BoundingRadius)/1000))
                        >= hero.Health)
                    {
                        if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                        {
                            ItemData.Tiamat_Melee_Only.GetItem().Cast();
                        }
                        if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                        {
                            ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
                        }
                    }
                }
                if (KsSmite && Smite.LSIsReady() && GetSmiteDamage(hero) >= hero.Health
                    && Player.Position.LSTo2D().LSDistance(hero.Position.LSTo2D()) <= 500 + Player.BoundingRadius + hero.BoundingRadius)
                {
                    Player.Spellbook.CastSpell(Smite, hero);
                }
            }
            //Q ks
            //E ks
            //R ks
            //item ks
            //smite ks
        }
    }
}
