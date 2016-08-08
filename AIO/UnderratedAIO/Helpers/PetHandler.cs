using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace UnderratedAIO.Helpers
{
    internal class PetHandler
    {
        private static int LastAATick;
        public static Obj_AI_Base Pet;
        private static readonly AIHeroClient player = ObjectManager.Player;
        private static Spell R, FarmL;
        private static readonly int range = 2000;
        private static bool debug = false, PetDelay = false;
        private static Vector3 movePos;

        internal static Menu addItemOptons(Menu menuM)
        {
            Menu menuP = new Menu("Pet ", "Psettings");
            menuP.AddItem(new MenuItem("petTarget", "Pet target priority", true))
                .SetValue(new StringList(new[] { "Targetselector", "Lowest health", "Closest to you" }, 0));
            menuP.AddItem(new MenuItem("petMovementType", "Pet movement type", true))
                .SetValue(new StringList(new[] { "Orbwalking", "Basic" }, 0));
            menuP.AddItem(new MenuItem("petOrbPos", "   Orbwalking Pos", true))
                .SetValue(new StringList(new[] { "Between you and target", "Less enemy" }, 0));
            menuP.AddItem(new MenuItem("petMovement", "Pet movement", true))
                .SetValue(new StringList(new[] { "Enable", "Disable" }, 0));
            menuP.AddItem(new MenuItem("petFollow", "Move without target", true)).SetValue(true);
            menuM.AddSubMenu(menuP);
            R = new Spell(SpellSlot.R);
            FarmL = new Spell(SpellSlot.Unknown, 1000);
            FarmL.SetSkillshot(0, 300, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Game.OnUpdate += Game_OnGameUpdate;

            return menuM;
        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (args == null || Pet == null)
            {
                return;
            }
            if (sender.NetworkId != Pet.NetworkId)
            {
                return;
            }
            if (args.Animation.ToLower().Contains("attack"))
            {
                LastAATick = Utils.GameTimeTickCount;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Pet = (Obj_AI_Base) ObjectManager.Player.Pet;
            if (Pet != null && !Pet.IsValid)
            {
                Pet = null;
            }
            if (debug)
            {
                Render.Circle.DrawCircle(movePos, 60, Color.Aqua, 7);
            }
        }

        public static void MovePet(Menu options, Orbwalking.OrbwalkingMode orbwalkingMode)
        {
            var isCombo = Orbwalking.OrbwalkingMode.Combo == orbwalkingMode;
            var isFarm = Orbwalking.OrbwalkingMode.LaneClear == orbwalkingMode;
            var isLastHit = Orbwalking.OrbwalkingMode.LastHit == orbwalkingMode ||
                            Orbwalking.OrbwalkingMode.Freeze == orbwalkingMode;
            if (PetDelay || Pet == null)
            {
                return;
            }
            if (options.Item("petMovement", true).GetValue<StringList>().SelectedIndex == 0 &&
                (isCombo || isFarm || isLastHit))
            {
                AttackableUnit gtarget = GetTarget(
                    isCombo, isFarm, isLastHit, options.Item("petTarget", true).GetValue<StringList>().SelectedIndex);
                if (gtarget == null)
                {
                    if (debug)
                    {
                        Console.WriteLine("follow");
                    }
                    if (player.IsMoving && options.Item("petFollow", true).GetValue<bool>())
                    {
                        var movePos = player.Position.LSExtend(Prediction.GetPrediction(player, 0.5f).UnitPosition, -250);
                        MoveTo(movePos);
                        SetPetDelay();
                    }
                }
                else if (gtarget.IsValid && !Pet.Spellbook.IsAutoAttacking)
                {
                    if (CanPetAttack() ||
                        options.Item("petMovementType", true).GetValue<StringList>().SelectedIndex == 1 ||
                        player.HealthPercent < 25)
                    {
                        if (Pet.LSDistance(gtarget) < Pet.AttackRange + gtarget.BoundingRadius + Pet.BoundingRadius)
                        {
                            Attack(gtarget);
                        }
                        else
                        {
                            MoveTo(gtarget.Position);
                        }
                    }
                    else
                    {
                        var pos = GetMovementPos(
                            gtarget, options.Item("petOrbPos", true).GetValue<StringList>().SelectedIndex == 1);
                        if (isLastHit || isFarm)
                        {
                            pos = gtarget.Position.LSExtend(Pet.Position, Pet.AttackRange);
                        }
                        MoveTo(pos);
                    }
                    SetPetDelay();
                }
            }
        }

        private static AttackableUnit GetTarget(bool isCombo, bool isFarm, bool isLastHit, int targetMode)
        {
            AttackableUnit gtarget = null;
            if (isCombo)
            {
                switch (targetMode)
                {
                    case 0:
                        gtarget = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);
                        break;
                    case 1:
                        gtarget =
                            HeroManager.Enemies.Where(i => player.LSDistance(i) <= range)
                                .OrderBy(i => i.Health)
                                .FirstOrDefault();
                        break;
                    case 2:
                        gtarget =
                            HeroManager.Enemies.Where(i => player.LSDistance(i) <= range)
                                .OrderBy(i => player.LSDistance(i))
                                .FirstOrDefault();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                List<AttackableUnit> otherTarget =
                    MinionManager.GetMinions(1000, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(
                            m =>
                                HealthPrediction.GetHealthPrediction(m, 2000) > Pet.LSGetAutoAttackDamage(m) ||
                                m.Health < Pet.LSGetAutoAttackDamage(m))
                        .Select(m => m as AttackableUnit)
                        .ToList()
                        .Concat(
                            ObjectManager.Get<Obj_AI_Turret>()
                                .Where(t => t.LSIsValidTarget() && t.Position.LSDistance(player.Position) < range))
                        .ToList();
                if (isFarm)
                {
                    gtarget =
                        otherTarget.OrderByDescending(m => Pet.LSGetAutoAttackDamage((Obj_AI_Base) m) > m.Health)
                            .ThenByDescending(m => m.MaxHealth)
                            .ThenByDescending(m => player.LSDistance(m))
                            .FirstOrDefault();
                }
                if (isLastHit)
                {
                    gtarget =
                        otherTarget.Where(m => Pet.LSGetAutoAttackDamage((Obj_AI_Base) m) > m.Health)
                            .OrderByDescending(m => player.LSDistance(m))
                            .FirstOrDefault();
                }
            }
            return gtarget;
        }


        private static Vector3 GetMovementPos(AttackableUnit Gtarget, bool IsSafe)
        {
            Vector3 pos = Vector3.Zero;
            Vector3 predictionPos = Vector3.Zero;
            try
            {
                predictionPos = Prediction.GetPrediction((Obj_AI_Base) Gtarget, 2).UnitPosition;
            }
            catch (Exception)
            {
                predictionPos = Gtarget.Position;
            }

            if (!Pet.IsMelee)
            {
                if (Pet.AttackRange < player.LSDistance(Gtarget) && !IsSafe)
                {
                    pos = Gtarget.Position.LSExtend(player.Position, Pet.AttackRange);
                }
                else
                {
                    var safePos =
                        CombatHelper.PointsAroundTheTargetOuterRing(
                            Gtarget.Position, Pet.AttackRange + Gtarget.BoundingRadius)
                            .Where(p => !p.LSIsWall() && p.LSIsValid())
                            .OrderByDescending(p => !p.LSUnderTurret(true))
                            .ThenBy(p => p.LSCountEnemiesInRange(700))
                            .FirstOrDefault();
                    pos = Gtarget.Position.LSExtend(safePos, Pet.AttackRange);
                }
            }
            else
            {
                if (IsSafe)
                {
                    pos = Gtarget.Position.LSExtend(predictionPos, Pet.AttackRange);
                }
                else
                {
                    pos = Gtarget.Position.LSExtend(player.Position, Pet.AttackRange);
                }
            }
            return pos;
        }

        private static bool CanPetAttack()
        {
            if (Pet != null)
            {
                if (Pet.CharData.BaseSkinName == "Graves")
                {
                    var attackDelay = 1.0740296828d * 1000 * Pet.AttackDelay - 716.2381256175d;
                    if (Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= LastAATick + attackDelay &&
                        Pet.HasBuff("GravesBasicAttackAmmo1"))
                    {
                        return true;
                    }
                    return false;
                }
                if (Pet.CharData.BaseSkinName == "Jhin")
                {
                    if (Pet.HasBuff("JhinPassiveReload"))
                    {
                        return false;
                    }
                }
                return Utils.GameTimeTickCount >=
                       LastAATick + Game.Ping / 2 + 25 + (Pet.AttackDelay - Pet.AttackCastDelay) * 1000;
            }
            return false;
        }

        private static void Attack(AttackableUnit gtarget)
        {
            try
            {
                if (debug)
                {
                    Console.WriteLine("attack " + gtarget.Name);
                }
                R.CastOnUnit(gtarget as Obj_AI_Base);
            }
            catch (Exception)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttackPet, gtarget.Position);
            }
        }

        private static void MoveTo(Vector3 pos)
        {
            if ((Pet.Path.LastOrDefault().LSDistance(pos) > 50 || !Pet.IsMoving) && pos.LSIsValid())
            {
                if (debug)
                {
                    Console.WriteLine("move");
                }
                EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, pos);
            }
        }

        private static void SetPetDelay()
        {
            PetDelay = true;
            var rnd = new Random();
            LeagueSharp.Common.Utility.DelayAction.Add(rnd.Next(220, 300), () => PetDelay = false);
        }
    }
}