using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events
{
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;
    

    internal class SpellCastManager : Logic
    {
        internal static void Init(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Args.SData.Name.Contains("DianaTeleport"))
            {
                lastRCast = Utils.TickCount;
            }

            if (!Args.SData.Name.Contains("DianaTeleport") || !W.IsReady())
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    {
                        var target = (AIHeroClient)Args.Target;

                        if (target != null && !target.IsDead && !target.IsZombie)
                        {
                            if (Menu.GetBool("ComboW") && target.IsValidTarget(W.Range))
                            {
                                W.Cast(true);
                            }
                        }
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    {
                        if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearmana")))
                        {
                            var mob = (Obj_AI_Base)Args.Target;
                            var mobs = MinionManager.GetMinions(Me.Position, R.Range, MinionTypes.All,
                                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                            if (mob != null && mobs.Contains(mob))
                            {
                                if (Menu.GetBool("JungleClearW") && mob.IsValidTarget(W.Range))
                                {
                                    W.Cast(true);
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}