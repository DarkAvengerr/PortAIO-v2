using Dark_Star_Thresh.Core;
using LeagueSharp.Common;
using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp.Common.Data;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Update
{
    class Mode : Core.Core
    {
        public static Vector3 qPred(AIHeroClient Target)
        {
            var pos = Spells.Q.GetPrediction(Target).CastPosition.To2D();

            if(Spells.Q.MinHitChance >= HitChance.High)
            {
                return pos.To3D2();
            }
           
            return pos.To3D2();
        }

        public static void GetActiveMode(EventArgs args)
        {
            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.None:
                    FlashCombo();
                    Flee();
                    _orbwalker.SetAttack(true);
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        public static bool ThreshQ(Obj_AI_Base t)
        {
            return t.HasBuff("ThreshQ");
        }

        public static void Combo()
        {
            var qTarget = TargetSelector.GetTarget(MenuConfig.ComboQ * 10, TargetSelector.DamageType.Physical);

            var eTarget = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical);

            var rTarget = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Physical);

            // Credits to DanZ for this line of code.
            var wAlly = Player.GetAlliesInRange(Spells.W.Range).Where(x => !x.IsMe).Where(x => !x.IsDead).Where(x => x.Distance(Player.Position) <= Spells.W.Range + 250).FirstOrDefault();

            if (Spells.E.IsReady())
            {
                if (eTarget != null && !eTarget.IsDashing() && !eTarget.IsDead && eTarget.IsValidTarget(Spells.E.Range))
                {
                    if(eTarget.Distance(Player) <= Spells.E.Range)
                    {
                        if (wAlly == null && !eTarget.UnderAllyTurret())
                        {
                            if(MenuConfig.Debug)
                            {
                                Chat.Print("Pushing");
                            }
                            Spells.E.Cast(eTarget.Position);
                        }
                        else
                        {
                            if (MenuConfig.Debug)
                            {
                                Chat.Print("Pulling");
                            }
                            // Might extend it to wAlly
                            Spells.E.Cast(eTarget.Position.Extend(Player.Position, Vector3.Distance(eTarget.Position, Player.Position) + 400));
                        }
                    }
                }
            }

            if(Spells.Q.IsReady())
            {
                if (qTarget != null && !qTarget.IsDashing() && !qTarget.IsDead && qTarget.IsValidTarget())
                {
                    var qPrediction = Spells.Q.GetPrediction(qTarget);

                    if (Spells.Q.WillHit(qTarget, qPrediction.CastPosition))
                    {
                        Spells.Q.Cast(qPred(qTarget));
                    }
                }

                if(MenuConfig.ComboTaxi && Spells.E.IsReady())
                {
                    if(qTarget != null && qTarget.IsValidTarget())
                    {
                        var qPrediction = Spells.Q.GetPrediction(qTarget);

                        if (Player.ManaPercent >= 55 && !Spells.Q.WillHit(qTarget, qPrediction.CastPosition))
                        {
                            var minions = MinionManager.GetMinions(Player.Position, Spells.Q.Range + Spells.E.Range);

                            foreach (var m in minions)
                            {
                                if (m != null && m.IsValidTarget() && m.Health > Spells.Q.GetDamage(m) && qTarget.IsFacing(Player))
                                {
                                    if(ThreshQ(m) || m.Distance(Player) >= 900f)
                                    {
                                        if (m.Distance(Player) <= Spells.Q.Range && qTarget.Distance(Player) <= Spells.Q.Range + Spells.E.Range - 50)
                                        {
                                            if (MenuConfig.Debug) Chat.Print("Taxi Mode Active...");

                                            Spells.Q.Cast(m.ServerPosition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (wAlly != null)
            {
                if(qTarget.IsValidTarget() && qTarget != null && ThreshQ(qTarget))
                {
                    if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(wAlly);
                    }
                }
                else if (eTarget.IsValidTarget() && eTarget != null && eTarget.Distance(Player) <= Spells.E.Range)
                {
                    if(Spells.W.IsReady())
                    {
                        Spells.W.Cast(wAlly);
                    }
                }
            }

          if(Spells.R.IsReady())
            {
                if (rTarget != null && !rTarget.IsDead && rTarget.IsValidTarget())
                {
                    if (Player.CountEnemiesInRange(Spells.R.Range - 45) >= MenuConfig.ComboR)
                    {
                        Spells.R.Cast();
                    }
                }
            }
        }

        public static void Harass()
        {
            if(_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && MenuConfig.HarassAA)
            {
                _orbwalker.SetAttack(false);
            }
            else // Not needed, but you'll never know.
            {
                _orbwalker.SetAttack(true);
            }

            var qTarget = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Physical);

            var eTarget = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Physical);

            if(MenuConfig.HarassE)
            {
                if (Spells.E.IsReady())
                {
                    if (eTarget != null && !eTarget.IsDashing() && !eTarget.IsDead && eTarget.IsValidTarget())
                    {
                        if (eTarget.Distance(Player.Position) <= Spells.E.Range)
                        {
                            Spells.E.Cast(eTarget.Position);
                        }
                    }
                }
            }

            // The reason we can return here is because we wont go further. Better code and we don't have to put unecessary if statements
            if (!MenuConfig.HarassQ) return;
            
            if (qTarget == null || qTarget.IsDashing() || qTarget.IsDead || !qTarget.IsValidTarget() || !Spells.Q.IsReady()) return;

            var qPrediction = Spells.Q.GetPrediction(qTarget);

            if (Spells.Q.WillHit(qTarget, qPrediction.CastPosition))
            {
                Spells.Q.Cast(qPred(qTarget));
            }
        }

        public static void LastHit()
        {
            var minions = MinionManager.GetMinions(1050f);
            if (Dmg.TalentReaper == 0) return;

            foreach (var m in minions)
            {
                if (m.IsValidTarget(1050) && m != null && !m.IsDead)
                {
                    var range = Player.GetAlliesInRange(Spells.W.Range).Where(x => !x.IsMe).Where(x => !x.IsDead).Where(x => x.Distance(Player.Position) <= 1050).FirstOrDefault();

                    if (Player.GetAutoAttackDamage(m, true) > m.Health && range != null)
                    {
                        if (MenuConfig.Debug)
                        {
                            Chat.Print("Damage = " + (float)Player.GetAutoAttackDamage(m, true) + " | Minion Hp = " + m.Health);

                            if (m.Distance(Player) <= 225f)
                            {
                                Render.Circle.DrawCircle(m.Position, 75, System.Drawing.Color.Green);
                            }
                            else
                            {
                                Render.Circle.DrawCircle(m.Position, 75, System.Drawing.Color.Red);
                            }
                        }
                    }
                }
            }
        }

        public static void FlashCombo()
        {
            if (!MenuConfig.ComboFlash) return;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (!Spells.Q.IsReady()) return;

            var qTarget = TargetSelector.GetTarget(Spells.Q.Range + 300, TargetSelector.DamageType.Physical);

            if(qTarget != null && qTarget.IsValidTarget())
            {
                var qPrediction = Spells.Q.GetPrediction(qTarget);
               
                var wAlly = Player.GetAlliesInRange(Spells.W.Range).Where(x => !x.IsMe).Where(x => !x.IsDead).Where(x => x.Distance(Player.Position) <= Spells.W.Range + 250).FirstOrDefault();

                if (wAlly != null)
                {
                    Spells.W.Cast(wAlly);
                }
                if (qPrediction.Hitchance == HitChance.Collision) return;

                if (Spells.Flash != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(Spells.Flash) == SpellState.Ready)
                {
                    Player.Spellbook.CastSpell(Spells.Flash, qPrediction.CastPosition);
                    Spells.Q.Cast(qPrediction.CastPosition);
                } 
            }   
        }
        public static void Flee() // Snippet From Nechrito Diana
        {
            if (!MenuConfig.Flee) return;

            var jump = JumpPos.Where(x => x.Value.Distance(ObjectManager.Player.Position) < 300f && x.Value.Distance(Game.CursorPos) < Spells.Q.Range).FirstOrDefault();
            var monster = MinionManager.GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health).FirstOrDefault();
            var mobs = MinionManager.GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            
            if(jump.Value.IsValid() && Spells.Q.IsReady())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, jump.Value);


                foreach (var pos in JunglePos)
                {
                    if (Game.CursorPos.Distance(pos) <= 350 && ObjectManager.Player.Position.Distance(pos) <= Spells.Q.Range && Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(pos);
                    }
                }
            }
            else
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }


            if (!mobs.Any() || !Spells.Q.IsReady()) return;

            var m = mobs.MaxOrDefault(x => x.MaxHealth);

            if(m.Distance(Game.CursorPos) <= Spells.Q.Range && m.Distance(Player) >= 475)
            {
                if(m.Health > Spells.Q.GetDamage(m))
                {
                    Spells.Q.Cast(m.Position);
                }
            }
        }
        public static readonly Dictionary<String, Vector3> JumpPos = new Dictionary<String, Vector3>()
        {
            { "mid_Dragon" , new Vector3 (9122f, 4058f, 53.95995f) },
            { "left_dragon" , new Vector3 (9088f, 4544f, 52.24316f) },
            { "baron" , new Vector3 (5774f, 10706f, 55.77578F) },
            { "red_wolves" , new Vector3 (11772f, 8856f, 50.30728f) },
            { "blue_wolves" , new Vector3 (3046f, 6132f, 57.04655f) },
        };

        public static readonly List<Vector3> JunglePos = new List<Vector3>()
        {
          new Vector3(6271.479f, 12181.25f, 56.47668f),
           new Vector3(6971.269f, 10839.12f, 55.2f),
           new Vector3(8006.336f, 9517.511f, 52.31763f),
           new Vector3(10995.34f, 8408.401f, 61.61731f),
          new Vector3(10895.08f, 7045.215f, 51.72278f),
           new Vector3(12665.45f, 6466.962f, 51.70544f),
           //pos of baron
           new Vector3(5048f, 10460f, -71.2406f),
           new Vector3(39000.529f, 7901.832f, 51.84973f),
          new Vector3(2106.111f, 8388.643f, 51.77686f),
           new Vector3(3753.737f, 6454.71f, 52.46301f),
           new Vector3(6776.247f, 5542.872f, 55.27625f),
           new Vector3(7811.688f, 4152.602f, 53.79456f),
          new Vector3(8528.921f, 2822.875f, 50.92188f),
          //pos of dragon
           new Vector3(9802f, 4366f, -71.2406f),
           new Vector3(3926f, 7918f, 51.74162f)
        };
    }
}
