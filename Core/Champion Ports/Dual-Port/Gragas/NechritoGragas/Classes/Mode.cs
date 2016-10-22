using LeagueSharp.Common;
using LeagueSharp;
using SPrediction;
using System;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Gragas
{
    class Mode
    {
        private static AIHeroClient Player => ObjectManager.Player;

        public static Vector3 rpred(AIHeroClient Target)
        {

            var pos = Spells.R.GetVectorSPrediction(Target, -(Player.MoveSpeed - Target.MoveSpeed)).CastTargetPosition;

            if (Target == null) return pos.To3D();


            if (Target.IsFacing(Player))
            {
                if (Target.IsMoving)
                {
                    pos = pos.Extend(Player.Position.To2D(), -90);
                }
                pos = pos.Extend(Player.Position.To2D(), -110);
            }

            if (!Target.IsFacing(Player))
            {
                pos = pos.Extend(Player.Position.To2D(), -150);
            }
            
            return pos.To3D2();
        }

        public static Vector3 qpred(AIHeroClient Target)
        {
            var pos = Spells.Q.GetVectorSPrediction(Target, - (Player.MoveSpeed - Target.MoveSpeed)).CastTargetPosition;

            pos = pos.Extend(Player.Position.To2D(), + Spells.R.Range);

            if (Target != null && !pos.IsWall())
            {
                if (Target.IsFacing(Player))
                {
                    if (Target.IsMoving)
                    {
                        pos = pos.Extend(Player.Position.To2D(), 90);
                    }
                    pos = pos.Extend(Player.Position.To2D(), 110);
                }

                if (!Target.IsFacing(Player))
                {
                    pos = pos.Extend(Player.Position.To2D(), 150);
                }
            }

            return pos.To3D2();
        }

      
        public static void ComboLogic()
        {
            var Target = TargetSelector.GetSelectedTarget();
            if (Target != null && !Target.IsZombie)
            {

                if (Target.Distance(Player) <= 1000f)
                {
                    if (Target.IsDashing()) return;

                    if (Spells.Q.IsReady() && Spells.R.IsReady())
                    {

                        if (Program.GragasQ == null)
                        {
                            Spells.Q.Cast(qpred(Target), true);
                        }

                        if (Spells.R.IsReady())
                        {
                            Spells.R.Cast(rpred(Target), true);
                        }

                        if (Program.GragasQ != null && Target.Distance(Program.GragasQ.Position) <= 250f)
                        {
                            Spells.Q.Cast(true);

                            if (Spells.E.IsReady())
                            {
                                Spells.E.Cast(Target.Position, true);
                            }
                        }
                    }
                }
                if (!Spells.Q.IsReady() && !Spells.R.IsReady())
                {
                    if (Spells.W.IsReady())
                    {
                        Spells.W.Cast();
                    }
                }
            }

            var target = TargetSelector.GetTarget(700f, TargetSelector.DamageType.Magical);
            
             if (target != null && target.IsValidTarget() && !target.IsZombie)
             {

                if (Spells.Q.IsReady())
                {
                    if (!Spells.R.IsReady())
                    {

                        if (Program.GragasQ == null)
                        {
                            Spells.Q.Cast(target, true);
                        }
                        if (Program.GragasQ != null && target.Distance(Program.GragasQ.Position) <= 250f)
                        {
                            Spells.Q.Cast(true);
                        }
                    }
                }
                
                // Smite
                if (Spells.Smite != SpellSlot.Unknown && Spells.R.IsReady() && Player.Spellbook.CanUseSpell(Spells.Smite) == SpellState.Ready && !target.IsZombie)
                {
                    Player.Spellbook.CastSpell(Spells.Smite, target);
                }

                else if (Spells.W.IsReady() && !Spells.R.IsReady())
                {
                    Spells.W.Cast();
                }

                // E
                else if (Spells.E.IsReady() && !Spells.W.IsReady())
                {
                    var pos = Spells.E.GetVectorSPrediction(target, Spells.E.Range).CastTargetPosition;

                    if (!Spells.E.CheckMinionCollision(pos))
                    {
                        Spells.E.Cast(pos);
                    }
                }
            }
        }
        
        public static void JungleLogic()
        {
            var mobs = MinionManager.GetMinions(Player.Position, Spells.W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (MenuConfig._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {   
                if (mobs.Count == 0 || mobs == null || Player.Spellbook.IsAutoAttacking)
                    return;
               
                foreach(var m in mobs)
                {
                    if(m.Distance(Player) <= 300f)
                    {
                        if (Spells.W.IsReady())
                        {
                            Spells.W.Cast();
                        }

                        if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(m);
                        }

                        if (Program.GragasQ == null)
                        {
                            Spells.Q.Cast(m, true);
                        }
                        if (Program.GragasQ != null && m.Distance(Program.GragasQ.Position) <= 250)
                        {
                            Spells.Q.Cast(true);
                        }
                    }
                }
            }
        }
        public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(Spells.R.Range - 50, TargetSelector.DamageType.Magical);
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if (Spells.E.IsReady() && MenuConfig.harassE)
                {
                    Spells.E.Cast(target);
                }

                if (Program.GragasQ == null)
                {
                    Spells.Q.Cast(target, true);
                }
                if (Program.GragasQ != null && target.Distance(Program.GragasQ.Position) <= 250)
                {
                    Spells.Q.Cast(true);
                }

                if (Spells.W.IsReady())
                {
                    if(target.Distance(Player) <= Player.AttackRange)
                    {
                        Spells.W.Cast();
                    }
                }
            }
        }
        public static void Game_OnUpdate(EventArgs args)
        {
        }
    }
}
