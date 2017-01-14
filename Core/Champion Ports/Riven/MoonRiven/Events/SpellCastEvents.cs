using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellCastEvents : Logic
    {
        internal static void Init()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || Args.SData == null)
            {
                return;
            }

            if (sender.IsMe)
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo(Args);
                        break;
                    case Orbwalking.OrbwalkingMode.Burst:
                        Burst(Args);
                        break;
                }
            }
            else if (sender.IsEnemy)
            {
                Dodge(sender, Args);
            }
        }

        private static void Combo(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null)
            {
                return;
            }

            AIHeroClient target = null;

            target = myTarget.IsValidTarget() ? myTarget : TargetSelector.GetTarget(600f, TargetSelector.DamageType.Physical);

            if (target != null && target.IsValidTarget(600f))
            {
                if (Args.SData.Name == "RivenFeint")
                {
                    if (MenuInit.ComboR && R.IsReady() && !isRActive && target.IsValidTarget(600f))
                    {
                        Riven.R1Logic(target);
                    }
                }
            }
        }

        private static void Burst(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null)
            {
                return;
            }

            var target = TargetSelector.GetSelectedTarget();

            if (target != null && target.IsValidTarget())
            {
                if (Args.SData.Name == "RivenFeint")
                {
                    if (Items.HasItem(3142) && Items.CanUseItem(3142))
                    {
                        Items.UseItem(3142);
                    }

                    if (R.IsReady() && !isRActive)
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void Dodge(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || !sender.IsEnemy || !(sender is AIHeroClient) || Args.SData == null)
            {
                return;
            }

            if (MenuInit.DodgeE && E.IsReady())
            {
                if (Args.SData.Name.Contains("DariusR"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("GarenQ"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("GarenR"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                }

                if (Args.SData.Name.Contains("IreliaE"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("LeeSinR"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("OlafE"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("RenektonW"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("RenektonPreExecute"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("RengarQ"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("VeigarR"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("VolibearW"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("XenZhaoThrust3"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                    return;
                }

                if (Args.SData.Name.Contains("attack") && Args.Target.IsMe &&
                    sender.Buffs.Any(
                        buff =>
                            buff.Name == "BlueCardAttack" || buff.Name == "GoldCardAttack" ||
                            buff.Name == "RedCardAttack"))
                {
                    E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
                }
            }
        }
    }
}