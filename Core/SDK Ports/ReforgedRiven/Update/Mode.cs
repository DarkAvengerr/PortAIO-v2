#region

using System;
using System.Linq;
using System.Windows.Forms;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using Reforged_Riven.Extras;
using Reforged_Riven.Menu;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Update
{
    internal class Mode : Core
    {
        public static void Burst()
        {
            if(!MenuConfig.BurstKeyBind.Active) return;

            if (!MenuConfig.FnoR)
            {
                if (!Spells.R.IsReady() || !Spells.W.IsReady() || !Spells.Q.IsReady() ||
                    Spells.Flash == SpellSlot.Unknown || !Spells.Flash.IsReady())
                {
                    return;
                }
            }

            var target = Variables.TargetSelector.GetSelectedTarget();

            if (target == null || !target.IsValidTarget(425 + Spells.W.Range - 35) || target.Distance(Player) <= 375) return;

            if ((MenuConfig.Flash || MenuConfig.FnoR) && target.Health > Dmg.GetComboDamage(target))
            {
                return;
            }

            Spells.E.Cast(target.Position);
            Logic.ForceR();
            DelayAction.Add(190, ()=> Player.Spellbook.CastSpell(Spells.Flash, target));
            DelayAction.Add(191, () => Spells.W.Cast());
        }

        public static void Combo()
        {
            var target = Variables.TargetSelector.GetTarget(Player.AttackRange + 310, DamageType.Physical);

            if (target == null || !target.IsValid) return;

            if (Spells.R.IsReady()
                && Spells.R.Instance.Name == IsFirstR 
                && MenuConfig.ForceR 
                && (Spells.Q.IsReady() || Spells.E.IsReady()))
            {
                Logic.ForceR();
            }

            if (Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR)
            {
                var pred = Spells.R.GetPrediction(target);

                if (pred.Hitchance > HitChance.High)
                {
                    if (Qstack > 2 && !MenuConfig.RKillable)
                    {
                        Spells.R.Cast(pred.CastPosition);
                    }
                    if (MenuConfig.RKillable && Qstack == 1 && !Spells.Q.IsReady())
                    {
                        Spells.R.Cast(pred.CastPosition);
                    }
                }
            }

            if (Spells.W.IsReady() && Spells.Q.IsReady() && Spells.E.IsReady())
            {
                Spells.E.Cast(target.ServerPosition);
                Logic.CastYomu();

                if (Spells.R.IsReady()
                    && Spells.R.Instance.Name == IsFirstR
                    && MenuConfig.ForceR 
                    && !(Dmg.GetComboDamage(target) < target.Health))
                {
                    Logic.ForceR();
                }

                DelayAction.Add(30, Logic.ForceItem);

                //if (Logic.InWRange(target) && Qstack >= 2)
                //{
                //    DelayAction.Add(130, () => Spells.W.Cast());
                //}


                //if (Qstack != 1)
                //{
                //    return;
                //}
                //    DelayAction.Add(160, () => Logic.ForceCastQ(target));
                //{
                //    return;
                //}
            }

            if (Spells.E.IsReady())
            {
                var epred = Spells.E.GetPrediction(target);

                Spells.E.Cast(epred.CastPosition);
            }

            if (Spells.Q.IsReady() && Player.Level == 1 && MenuConfig.EngageQ)
            {
                if(Qstack != 1) return;

                Logic.ForceCastQ(target);
            }

            if (Spells.W.IsReady() && Logic.InWRange(target) && Qstack > 1)
            {
                Spells.W.Cast(target);
            }
        }

        public static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Player.AttackRange + 380));
            foreach (var m in minions)
            {
                //if (m.IsUnderEnemyTurret()) return;

                if (Spells.E.IsReady() && MenuConfig.LaneE)
                {
                    Spells.E.Cast(m);
                }

                if (!Spells.W.IsReady() || !MenuConfig.LaneW) continue;

                if (!Logic.InWRange(m)) continue;

                if (m.Health < Spells.W.GetDamage(m) && !Player.Spellbook.IsAutoAttacking)
                {
                    Spells.W.Cast(m);
                }
            }
        }

       
        public static void Jungle()
        {
            var mobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Player.GetRealAutoAttackRange(Player) + Spells.E.Range));

            foreach (var m in mobs)
            {
                if (!m.IsValid) return;

                if (Spells.E.IsReady() && MenuConfig.JungleE && !Player.Spellbook.IsAutoAttacking)
                {
                    Spells.E.Cast(m.Position);
                }
            }
        }

        public static void Harass()
        {
            var target = Variables.TargetSelector.GetTarget(400, DamageType.Physical);
            if (Spells.Q.IsReady() && Spells.W.IsReady() && Spells.E.IsReady() && Qstack == 1)
            {
                if (target.IsValidTarget())
                {
                    Logic.ForceCastQ(target);
                }
            }

            if (!Spells.Q.IsReady() || !Spells.E.IsReady() || Qstack < 3) return;

            var epos = Player.ServerPosition + (Player.ServerPosition - target.ServerPosition).Normalized() * 300;

            Spells.E.Cast(epos);

            if (Logic.InWRange(target))
            {
                Logic.ForceW();
            }

            DelayAction.Add(190, () => Spells.Q.Cast(epos));
        }

        public static void QMove()
        {
            if (!MenuConfig.QMove.Active)
            {
                return;
            }
        
            if (!Spells.Q.IsReady()) return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
             
            DelayAction.Add(Game.Ping + 2, () => Spells.Q.Cast(Player.Position - 15));
            
        }

        public static void Flee()
        {
            if(!MenuConfig.FleeKey.Active) return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (MenuConfig.WallFlee)
            {
                var end = Player.ServerPosition.Extend(Game.CursorPos, Spells.Q.Range);
                var isWallDash = FleeLogic.IsWallDash(end, Spells.Q.Range);

                var eend = Player.ServerPosition.Extend(Game.CursorPos, Spells.E.Range);
                var wallE = FleeLogic.GetFirstWallPoint(Player.ServerPosition, eend);
                var wallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);

                Player.GetPath(wallPoint);

                if (Spells.Q.IsReady() && Qstack < 3)
                {
                    Spells.Q.Cast(Game.CursorPos);
                }


                if (!isWallDash || Qstack != 3 || !(wallPoint.Distance(Player.ServerPosition) <= 800)) return;

                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, wallPoint);

                if (!(wallPoint.Distance(Player.ServerPosition) <= 600)) return;

                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, wallPoint);

                if (!(wallPoint.Distance(Player.ServerPosition) <= 45)) return;

                if (Spells.E.IsReady())
                {
                    Spells.E.Cast(wallE);
                }

                if (Qstack != 3 || !(end.Distance(Player.ServerPosition) <= 260) || !wallPoint.IsValid()) return;


                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, wallPoint);

                Spells.Q.Cast(wallPoint);
            }

            else
            {
                var enemy =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(hero => hero.IsValidTarget(Player.HasBuff("RivenFengShuiEngine")
                            ? 70 + 195 + Player.BoundingRadius
                            : 70 + 120 + Player.BoundingRadius) && Spells.W.IsReady());

                var x = Player.Position.Extend(Game.CursorPos, 300);

                var objAiHeroes = enemy as AIHeroClient[] ?? enemy.ToArray();

                if (Spells.W.IsReady() && objAiHeroes.Any())
                {
                    foreach (var target in objAiHeroes.Where(Logic.InWRange))
                    {
                        Spells.W.Cast();
                    }
                }

                if (Spells.Q.IsReady() && !Player.IsDashing()) Spells.Q.Cast(Game.CursorPos);
                Logic.CastYomu();
                if (Spells.E.IsReady() && !Player.IsDashing()) Spells.E.Cast(x);
            }
        }
    }
}