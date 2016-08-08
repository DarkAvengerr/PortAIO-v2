using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SAutoCarry.Champions.Helpers
{
    public static class Target
    {
        private static AIHeroClient s_Target;
        private static bool s_Flashed;

        public static AIHeroClient Get(float inRange, bool locked = false, LeagueSharp.Common.TargetSelector.DamageType dtype = LeagueSharp.Common.TargetSelector.DamageType.Physical)
        {
            if (Program.Champion.ConfigMenu.Item("CSHYKEY").GetValue<KeyBind>().Active || Program.Champion.ConfigMenu.Item("CFLASHKEY").GetValue<KeyBind>().Active)
                return TargetSelector.SelectedTarget;

            if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.LSIsValidTarget(inRange))
            {
                Set(TargetSelector.SelectedTarget);
                return TargetSelector.SelectedTarget;
            }
            if (s_Target != null)
            {
                if (!s_Target.LSIsValidTarget())
                {
                    s_Target = null;
                }
                else
                {
                    if (s_Target.IsDead || !s_Target.IsTargetable)
                        s_Target = null;
                    else if (s_Target.LSDistance(ObjectManager.Player.ServerPosition) > inRange)
                    {
                        if (locked)
                            return s_Target;
                        else
                            s_Target = null;
                    }
                }
            }

            if (s_Target == null)
                Set(TargetSelector.GetTarget(inRange, dtype));
            else
            {
                if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget != s_Target && TargetSelector.SelectedTarget.LSIsValidTarget(inRange))
                    Set(TargetSelector.SelectedTarget);
            }

            return s_Target;
        }

        public static void Set(AIHeroClient t)
        {
            if (s_Target != t)
                s_Flashed = false;

            s_Target = t;
        }

        public static void SetFlashed(bool val = true)
        {
            s_Flashed = val;
        }

        public static bool IsTargetFlashed()
        {
            if (s_Target == null)
                return false;

            return s_Flashed;
        }

        public static bool HasTarget()
        {
            return s_Target != null;
        }
    }
}
