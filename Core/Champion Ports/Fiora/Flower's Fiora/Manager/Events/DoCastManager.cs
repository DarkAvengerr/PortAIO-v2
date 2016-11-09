using System.Linq;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Fiora.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class DoCastManager : Logic
    {
        internal static void Init(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name) || !E.IsReady())
            {
                return;
            }

            switch(Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ItemsUse(Menu.Item("ComboYoumuu", true).GetValue<bool>(),
                        Menu.Item("ComboTiamat", true).GetValue<bool>(),
                        Menu.Item("ComboHydra", true).GetValue<bool>());

                    if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
                    {
                        var target = Args.Target as AIHeroClient;

                        if (target != null && Orbwalking.InAutoAttackRange(target))
                        {
                            E.Cast();
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (Me.ManaPercent >= Menu.Item("HarassMana", true).GetValue<Slider>().Value)
                    {
                        ItemsUse(false,
                            Menu.Item("HarassTiamat", true).GetValue<bool>(),
                            Menu.Item("HarassHydra", true).GetValue<bool>());

                        if (Menu.Item("HarassE", true).GetValue<bool>() && E.IsReady())
                        {
                            var target = Args.Target as AIHeroClient;

                            if (target != null && Orbwalking.InAutoAttackRange(target))
                            {
                                E.Cast();
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (Me.ManaPercent >= Menu.Item("JungleClearMana", true).GetValue<Slider>().Value)
                    {
                        var mobs = MinionManager.GetMinions(Me.Position, Orbwalking.GetRealAutoAttackRange(Me),
                            MinionTypes.All, MinionTeam.Neutral,
                            MinionOrderTypes.MaxHealth);

                        if (mobs.Any())
                        {
                            if (Menu.Item("JungleClearE", true).GetValue<bool>() && E.IsReady())
                            {
                                var mob = mobs.FirstOrDefault(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)));

                                if (mob != null)
                                {
                                    E.Cast();
                                }
                            }

                            ItemsUse(false, true, true, true);
                        }
                    }
                    break;
            }
        }
    }
}