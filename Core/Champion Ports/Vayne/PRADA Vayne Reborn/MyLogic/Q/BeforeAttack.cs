using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using PRADA_Vayne.MyUtils;

using EloBuddy; 
using LeagueSharp.Common; 
namespace PRADA_Vayne.MyLogic.Q
{
    public static partial class Events
    {
        public static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe && Program.Q.IsReady() && Program.ComboMenu.Item("QCombo").GetValue<bool>())
            {
                if (args.Target.IsValid<AIHeroClient>())
                {
                    var target = (AIHeroClient) args.Target;
                    if (Program.ComboMenu.Item("RCombo").GetValue<bool>() && Program.R.IsReady() &&
                        Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        if (!target.UnderTurret(true))
                        {
                            Program.R.Cast();
                        }
                    }
                    if (target.IsMelee && target.IsFacing(Heroes.Player))
                    {
                        if (target.Distance(Heroes.Player.ServerPosition) < 325)
                        {
                            var tumblePosition = target.GetTumblePos();
                            args.Process = false;
                            Tumble.Cast(tumblePosition);
                        }
                    }
                    var closestJ4Wall = ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            m =>
                                m.BaseSkinName == "jarvanivwall" &&
                                ObjectManager.Player.ServerPosition.Distance(m.Position) < 100);
                    if (closestJ4Wall != null)
                    {
                        args.Process = false;
                        Program.Q.Cast(ObjectManager.Player.ServerPosition.Extend(closestJ4Wall.Position, 300));
                    }
                }
            }
        }
    }
}
