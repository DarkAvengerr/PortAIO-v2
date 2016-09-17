using System;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Nechrito_Rengar.Handlers;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Rengar.Main
{
    class Modes : Core
    {
        public static void Combo()
        {
            var hasPassive = Player.HasBuff("RengarRBuff") || Player.HasBuff("RengarPassiveBuff");

            var target = TargetSelector.GetTarget(Player.AttackRange, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget() && !target.IsZombie && target.Distance(Player) <= Champion.E.Range)
            {
                if(Player.Mana == 5)
                {
                    if(Champion.E.IsReady() && target.Distance(Player) <= 600)
                    {
                        if(MenuConfig.UseItem)
                        {
                            ITEMS.CastYoumoo();
                        }
                        Champion.E.Cast(target);
                    }
                   if(Champion.Q.IsReady())
                    {
                        Champion.Q.Cast(target);
                    }
                   if (Champion.W.IsReady())
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(target);
                    }
                }
                if (Player.Mana < 5)
                {
                    if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast(target);
                    }
                   if (Champion.W.IsReady())
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(target);
                    }
                   if (Champion.E.IsReady())
                    {
                        Champion.E.Cast(target);
                    }
                }
            }
        }
        public static void ApCombo()
        {
            var target = TargetSelector.GetTarget(Player.AttackRange, TargetSelector.DamageType.Magical);
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if(Player.Mana == 5)
                {
                    if (Champion.W.IsReady())
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastYoumoo();
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(target);
                        Champion.W.Cast(target);
                    }
                    if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast(target);
                    }
                    if (Champion.E.IsReady())
                    {
                        Champion.E.Cast(target);
                    }
                }
                if(Player.Mana <= 4)
                {
                    if(Champion.Q.IsReady())
                    {
                        Champion.Q.Cast(target);
                    }
                    if (Champion.W.IsReady())
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(target);
                        Champion.W.Cast(target);
                    }
                    if (Champion.E.IsReady())
                    {
                        Champion.E.Cast(target);
                    }
                }
            }
        }
        public static void TripleQ()
        {
            var target = TargetSelector.GetTarget(Player.AttackRange, TargetSelector.DamageType.Physical);
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if (Player.Mana == 5)
                {
                    if (Champion.Q.IsReady())
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastYoumoo();
                        }
                        Champion.Q.Cast();
                    }
                    
                  if (Champion.E.IsReady())
                    {
                        Champion.E.Cast(target);
                    }
                  if (Champion.W.IsReady())
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(target.ServerPosition);
                    }
                   if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast();
                    }
                }
                if (Player.Mana < 5)
                {
                    if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast();
                    }

                    if (Champion.E.IsReady())
                    {
                        Champion.E.Cast(target);
                    }
                    if (Champion.W.IsReady())
                    {
                        if(MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(target.ServerPosition);
                    }
                    if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast();
                    }
                }
            }
        }
        public static void Lane()
        {
            var minions = MinionManager.GetMinions(Player.AttackRange);
            if (minions == null || Player.Mana == 5 && MenuConfig.Passive)
                return;

            var hasPassive = Player.HasBuff("RengarRBuff") || Player.HasBuff("RengarPassiveBuff");

            foreach (var m in minions)
            {
                if(Player.Mana == 5)
                {
                    if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast(m);
                    }
                   if (Champion.E.IsReady() && !hasPassive)
                    {
                        Champion.E.Cast(m);
                    }
                   if (Champion.W.IsReady() && m.Distance(Player) <= Champion.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(m);
                    }

                }
                if (Player.Mana < 5)
                {
                    if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast(m);
                    }
                    if (Champion.E.IsReady() && !hasPassive)
                    {
                        Champion.E.Cast(m);
                    }
                   if (Champion.W.IsReady() && m.Distance(Player) <= Champion.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(m);
                    }
                }
            }
        }
        public static void Jungle()
        {
            var mobs = MinionManager.GetMinions(Player.AttackRange + 75, MinionTypes.All, MinionTeam.Neutral,
          MinionOrderTypes.MaxHealth);
            if (mobs.Count == 0 || Player.Mana == 5 && MenuConfig.Passive)
                return;

            var hasPassive = Player.HasBuff("RengarRBuff") || Player.HasBuff("RengarPassiveBuff");

            foreach (var m in mobs)
            {
                if (Player.Mana == 5)
                {
                    if (Champion.W.IsReady() && m.Distance(Player) <= Champion.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(m);
                    }
                    if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast(m);
                    }
                   else if(Champion.E.IsReady() && !hasPassive)
                    {
                        Champion.E.Cast(m);
                    }
                }
                if(Player.Mana < 5)
                {
                    if (Champion.W.IsReady() && m.Distance(Player) <= Champion.W.Range)
                    {
                        if (MenuConfig.UseItem)
                        {
                            ITEMS.CastHydra();
                        }
                        Champion.W.Cast(m);
                    }
                    if (Champion.Q.IsReady())
                    {
                        Champion.Q.Cast(m);
                    }
                   else if (Champion.E.IsReady() && !hasPassive)
                    {
                        Champion.E.Cast(m);
                    }
                }
            }
        }
        public static void Game_OnUpdate(EventArgs args)
        {
        }
    }
}
