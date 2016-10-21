#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;
using PrideStalker_Rengar.Main;
using LeagueSharp.SDK.Enumerations;
using SharpDX;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace PrideStalker_Rengar.Handlers
{
    internal class Mode : Core
    {

        //public static bool HasPassive => Player.Buffs.Any(x => x.Name.ToLower().Contains("RengarPassiveBuff")) ||
        //    Player.Buffs.Any(x => x.Name.ToLower().Contains("RengarRBuff"));

        public static bool HasPassive()
        {
            return Player.Buffs.Any(x => x.Name.ToLower().Contains("rengarpassivebuff")) || Player.HasBuff("RengarRBuff");
        }

        #region Combo
        public static void Combo()
        {   
            var target = Variables.TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget() || target.IsZombie) return;

            if(Player.Mana == 5)
            {
                if(MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && HasPassive())
                {
                    Item.CastYomu();
                }

                if(Spells.E.IsReady() && target.Distance(Player) < Player.AttackRange)
                {
                    if (Spells.E.IsReady() && target.Distance(Player) <= Spells.E.Range - 350)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                }

                if (Spells.Q.IsReady() && target.Distance(Player) < Player.AttackRange && !Spells.E.IsReady())
                {
                    Spells.Q.Cast(target);
                }
            }

            if (!(Player.Mana < 5)) return;

            if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && HasPassive())
            {
                Item.CastYomu();
            }

            if (Spells.E.IsReady() && target.Distance(Player) <= Spells.E.Range - 200)
            {
                Spells.E.CastOnUnit(target);
            }

            if (!(target.Distance(Player) <= Spells.W.Range)) return;

            if (Spells.Q.IsReady())
            {
                Spells.Q.Cast(target);
            }
            if (MenuConfig.UseItem)
            {
                Item.CastHydra();
            }
            if (Spells.W.IsReady())
            {
                Spells.W.Cast(target);
            }
        }
        #endregion

        #region ApCombo
        public static void ApCombo()
        {
            var target = Variables.TargetSelector.GetTarget(Spells.E.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget() || target.IsZombie) return;

            if (Player.Mana == 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && HasPassive())
                {
                    Item.CastYomu();
                }
                if(target.Distance(Player) <= Spells.W.Range)
                {
                    if (MenuConfig.UseItem && Spells.W.IsReady())
                    {
                        Item.CastHydra();
                    }
                    if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(target);
                    }
                }
            }
            if (!(Player.Mana < 5)) return;

            if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady() && HasPassive())
            {
                Item.CastYomu();
            }

            if(MenuConfig.UseItem && !HasPassive())
            {
                Item.CastProtobelt();
            }

            if (!(target.Distance(Player) <= Spells.W.Range)) return;

            if (MenuConfig.UseItem && Spells.W.IsReady())
            {
                Item.CastHydra();
            }
            if (Spells.W.IsReady())
            {
                Spells.W.Cast(target);
                Spells.W.Cast(target);
            }

           else if (Spells.Q.IsReady())
            {
                Spells.Q.Cast(target);
            }

            else if (Spells.E.IsReady() && !Spells.W.IsReady() && target.Distance(Player) <= float.MaxValue)
            {
                if (Spells.E.IsReady() && target.Distance(Player) <= Spells.E.Range - 200)
                {
                    Spells.E.CastOnUnit(target);
                }
            }
        }
        #endregion

        #region TripleQ
        public static void TripleQ()
        {
            var target = Variables.TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget() || target.IsZombie) return;

            if (Player.Mana >= 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && HasPassive())
                {
                    Item.CastYomu();
                }

                if (Spells.Q.IsReady() && target.Distance(Player) <= Spells.W.Range)
                {
                    Spells.Q.Cast();
                }
            }

            if (!(Player.Mana < 5)) return;

            
            if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady())
            {
                Item.CastYomu();
            }

            if (Spells.Q.IsReady() && target.Distance(Player) <= Spells.W.Range)
            {
                Spells.Q.Cast();
            }

            if (Spells.E.IsReady() && target.Distance(Player) <= Spells.E.Range - 200)
            {
                Spells.E.CastOnUnit(target);
            }

            if (!Spells.W.IsReady() || Spells.Q.IsReady() || !(Player.Distance(target) <= Spells.W.Range)) return;

            if (MenuConfig.UseItem)
            {
                Item.CastHydra();
            }

            Spells.W.Cast(target);
        }
        #endregion

        #region OneShot
        public static void OneShot()
        {
            var target = Variables.TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget() || target.IsInvulnerable) return;

            if (Player.Mana >= 5)
            {
                if (MenuConfig.UseItem && Spells.Q.IsReady() && HasPassive())
                {
                    Item.CastYomu();
                }
                if (Spells.Q.IsReady())
                {
                    if (HasPassive())
                    {
                        Spells.Q.Cast();
                    }
                    if (!HasPassive() && target.Distance(Player) <= Player.GetRealAutoAttackRange())
                    {
                        Spells.Q.Cast(target);
                    }  
                }
            }

            if (!(Player.Mana < 5)) return;

            if (MenuConfig.UseItem && Spells.Q.IsReady() && Spells.W.IsReady())
            {
                Item.CastYomu();
            }

            if (Spells.E.IsReady() && target.Distance(Player) <= Spells.E.Range - 200)
            {
                Spells.E.CastOnUnit(target);
            }

            if (Spells.W.IsReady() && Player.Distance(target) <= Spells.W.Range)
            {
                if (MenuConfig.UseItem)
                {
                    Item.CastHydra();
                }
                Spells.W.Cast(target);
            }

            if (Spells.Q.IsReady() && target.Distance(Player) <= Spells.W.Range)
            {
                Spells.Q.Cast();
            }
        }
        
        #endregion


        #region Lane
        public static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(Spells.W.Range)).ToList();

            if (Player.Mana == 5 && MenuConfig.Passive.Active)
            {
                return;
            }

            foreach(var m in minions)
            {
                if (Player.Mana >= 5)
                {
                    if (MenuConfig.ComboMode.SelectedValue == "Ap Combo")
                    {
                        if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                        {
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                    else
                    {
                        if (Spells.Q.IsReady() && m.Distance(Player) < Player.AttackRange)
                        {
                            if (MenuConfig.UseItem)
                            {
                                Item.CastHydra();
                            }
                            Spells.Q.Cast(m);
                        }
                    }
                }

                if (!(Player.Mana < 5)) continue;

                if (Spells.Q.IsReady())
                {
                    Spells.Q.Cast(m);
                }

                if (Spells.E.IsReady() && !HasPassive())
                {
                    Spells.E.Cast(m);
                }

                if (!Spells.W.IsReady() || !(m.Distance(Player) <= Spells.W.Range)) continue;

                if (MenuConfig.UseItem)
                {
                    Item.CastHydra();
                }

                Spells.W.Cast(m);
            }
              
        }
        #endregion

        #region Jungle
        public static void Jungle()
        {
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.IsValidTarget(Spells.W.Range)).ToList();

            if (Player.Mana == 5 && MenuConfig.Passive.Active)
            {
                return;
            }

            foreach (var m in mob)
            {
                if (Player.Mana == 5)
                {
                    if(MenuConfig.ComboMode.SelectedValue == "Ap Combo")
                    {
                        if(Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range)
                        {
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                    else
                    {
                        if (Spells.W.IsReady() && m.Distance(Player) <= Spells.W.Range && Player.HealthPercent <= 15)
                        {
                            if (MenuConfig.UseItem)
                            {
                                Item.CastHydra();
                            }
                            Spells.W.Cast(m.ServerPosition);
                        }
                    }
                }

                if (!(Player.Mana < 5)) continue;

                if (Spells.E.IsReady())
                {
                    Spells.E.Cast(m.ServerPosition);
                }

                if (!Spells.W.IsReady() || !(m.Distance(Player) <= Spells.W.Range)) continue;

                if (MenuConfig.UseItem)
                {
                    Item.CastHydra();
                }

                Spells.W.Cast(m.ServerPosition);
            }
        }
        #endregion


        #region LastHit
        public static void LastHit()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(Player.AttackRange)).ToList();
            

            if (Player.Mana == 5 && MenuConfig.Passive.Active)
            {
                return;
            }
            if (!MenuConfig.StackLastHit) return;
            {
                foreach (var m in minions)
                {
                    if (m.Health < Spells.Q.GetDamage(m) + (float)Player.GetAutoAttackDamage(m))
                    {
                        Spells.Q.Cast();
                    }
                }
            }
        }
        #endregion

        #region ComboMode
        public static void ChangeComboMode()
        {
            if(MenuConfig.ChangeComboMode.Active)
            {
                switch(MenuConfig.ComboMode.SelectedValue)
                {
                    case "Gank":
                        DelayAction.Add(200, () => MenuConfig.ComboMode.SelectedValue = "Triple Q");
                        break;
                    case "Triple Q":
                        DelayAction.Add(200, () => MenuConfig.ComboMode.SelectedValue = "Ap Combo");
                        break;
                    case "Ap Combo":
                        DelayAction.Add(200, () => MenuConfig.ComboMode.SelectedValue = "OneShot");
                        break;
                    case "OneShot":
                        DelayAction.Add(200, () => MenuConfig.ComboMode.SelectedValue = "Gank");
                        break;
                }
            }
        }
        #endregion

        #region Skin
        public static void Skin()
        {
           if(MenuConfig.UseSkin)
           {
               var skin = MenuConfig.SkinChanger.Index;
               if (skin == 0)
               {
               }
               else if (skin == 1)
               {
               }
               else if (skin == 2)
               {
               }
               else if (skin == 3)
               {
               }
           }
           else
            {
            }
        }
        #endregion
    }
}
