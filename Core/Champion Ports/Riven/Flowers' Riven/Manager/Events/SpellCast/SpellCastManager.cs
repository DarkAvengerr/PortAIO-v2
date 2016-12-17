using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events
{
    using Spells;
    using System.Linq;
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;


    internal class SpellCastManager : Logic
    {
        internal static void InitSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsMe || Args.SData == null)
            {
                return;
            }

            switch (Args.SData.Name)
            {
                case "ItemTiamatCleave":
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            if (!HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= W.Range))
                            {
                                return;
                            }

                            if (Menu.GetBool("ComboW") && W.IsReady())
                            {
                                W.Cast();
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            var ForcusTarget = TargetSelector.GetSelectedTarget();
                            var fortarget = ForcusTarget;

                            if (fortarget != null && !fortarget.IsDead && !fortarget.IsZombie)
                            {
                                if (W.IsReady() && fortarget.IsValidTarget(W.Range))
                                {
                                    W.Cast(true);
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Mixed:
                            if (!HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= W.Range))
                            {
                                return;
                            }

                            if (Menu.GetBool("HarassW") && W.IsReady())
                            {
                                W.Cast(true);
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.LaneClear:
                            if (Menu.GetBool("JungleClearW") && W.IsReady())
                            {
                                var mobs = MinionManager.GetMinions(Me.Position, W.Range, MinionTypes.All,
                                    MinionTeam.Neutral);

                                if (mobs.Any())
                                {
                                    W.Cast(true);
                                }
                            }
                            break;
                    }
                    break;
                case "RivenTriCleave":
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            SpellManager.CastItem(true);

                            if (Menu.GetBool("ComboR") && R.IsReady() && R.Instance.Name == "RivenFengShuiEngine")
                            {
                                var target = TargetSelector.GetSelectedTarget() ??
                                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                                if (target.IsValidTarget(R.Range))
                                {
                                    SpellManager.R2Logic(target);
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            SpellManager.CastItem(true);

                            if (R.IsReady() && R.Instance.Name == "RivenFengShuiEngine")
                            {
                                var ForcusTarget = TargetSelector.GetSelectedTarget();

                                if (ForcusTarget.IsValidTarget(R.Range))
                                {
                                    R.Cast(ForcusTarget.Position, true);
                                }
                            }
                            break;
                    }
                    break;
                case "RivenTriCleaveBuffer":
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            if (Menu.GetBool("ComboR") && R.IsReady() && R.Instance.Name == "RivenFengShuiEngine")
                            {
                                var target = TargetSelector.GetSelectedTarget() ??
                                             TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                                if (target.IsValidTarget(R.Range))
                                {
                                    SpellManager.R2Logic(target);
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            if (R.IsReady() && R.Instance.Name == "RivenFengShuiEngine")
                            {
                                var ForcusTarget = TargetSelector.GetSelectedTarget();

                                if (ForcusTarget.IsValidTarget(R.Range))
                                {
                                    R.Cast(ForcusTarget.Position, true);
                                }
                            }
                            break;
                    }
                    break;
                case "RivenMartyr":
                    if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst) &&
                        HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= 400))
                    {
                        SpellManager.CastItem(true);
                    }
                    break;
                case "RivenFeint":
                    if (!R.IsReady())
                    {
                        return;
                    }

                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            var target = TargetSelector.GetSelectedTarget() ??
                                         TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                            if (target.IsValidTarget(R.Range))
                            {
                                switch (R.Instance.Name)
                                {
                                    case "RivenFengShuiEngine":
                                        if (Menu.GetKey("R1Combo"))
                                        {
                                            if (target.DistanceToPlayer() <= 500 &&
                                                HeroManager.Enemies.Any(x => x.DistanceToPlayer() <= 500))
                                            {
                                                R.Cast(true);
                                            }
                                        }
                                        break;
                                    case "RivenIzunaBlade":
                                        SpellManager.R2Logic(target);
                                        break;
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            if (TargetSelector.GetSelectedTarget().IsValidTarget(R.Range))
                            {
                                switch (R.Instance.Name)
                                {
                                    case "RivenFengShuiEngine":
                                        R.Cast();
                                        break;
                                    case "RivenIzunaBlade":
                                        var ForcusTarget = TargetSelector.GetSelectedTarget();

                                        if (ForcusTarget.IsValidTarget(R.Range))
                                        {
                                            R.Cast(ForcusTarget.Position, true);
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                    break;
                case "RivenIzunaBlade":
                    switch (Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            var target = TargetSelector.GetSelectedTarget() ??
                                         TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

                            if (target != null && target.IsValidTarget())
                            {
                                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                                {
                                    SpellManager.CastQ(target);
                                }
                                else if (W.IsReady() && target.IsValidTarget(W.Range))
                                {
                                    W.Cast();
                                }
                            }
                            break;
                        case Orbwalking.OrbwalkingMode.Burst:
                            var ForcusTarget = TargetSelector.GetSelectedTarget();

                            if (ForcusTarget.IsValidTarget())
                            {
                                if (ForcusTarget.IsValidTarget(Q.Range))
                                {
                                    SpellManager.CastQ(ForcusTarget);
                                }
                                else if (ForcusTarget.IsValidTarget(W.Range) && W.IsReady())
                                {
                                    W.Cast();
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        internal static void InitSpellShield(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsEnemy || sender.Type != GameObjectType.AIHeroClient || Args.SData == null ||
                Args.Target == null || !Args.Target.IsMe)
            {
                return;
            }

            if (Menu.GetBool("EShielddogde") && E.IsReady() && Me.CanMoveMent())
            {
                if (Args.SData.Name.Contains("DariusR"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeDariusR"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("GarenQ"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeGarenQ"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("GarenR"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeGarenR"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("IreliaE"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeIreliaE"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("LeeSinR"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeLeeSinR"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("OlafE"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeOlafE"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("RenektonW"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeRenektonW"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("RenektonPreExecute"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgePantheonW"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("RengarQ"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeRengarQ"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("VeigarR"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeVeigarR"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("VolibearW"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeVolibearW"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("XenZhaoThrust3"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeXenZhaoQ3"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }

                if (Args.SData.Name.Contains("attack") && Args.Target.IsMe &&
                    sender.Buffs.Any(
                        buff =>
                            buff.Name == "BlueCardAttack" || buff.Name == "GoldCardAttack" ||
                            buff.Name == "RedCardAttack"))
                {
                    if (E.IsReady() && Menu.GetBool("EDodgeTwistedFateW"))
                    {
                        E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    }
                }
            }
        }
    }
}