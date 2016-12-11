using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Events
{
    class AntiGapCloser
    {
        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Program.E.IsReady())
            {
                if (Program.imenu.Item("AntiAlistar", true).GetValue<bool>() && gapcloser.Sender.ChampionName == "Alistar" &&
                    gapcloser.SkillType == GapcloserType.Targeted)
                {
                    Program.E.Cast(gapcloser.Sender, true);

                    if (Program.gmenu.Item("Gapcloser", true).GetValue<bool>() &&
                        Program.gmenu.Item("AntiGapcloser" + gapcloser.Sender.ChampionName.ToLower(), true).GetValue<bool>())
                    {
                        if (gapcloser.Sender.DistanceToPlayer() <= 200 && gapcloser.Sender.IsValid)
                        {
                            Program.E.CastOnUnit(gapcloser.Sender, true);
                        }
                    }
                }
            }
        }

        public static bool CheckTarget(Obj_AI_Base target, float range = float.MaxValue)
        {
            if (target == null)
            {
                return false;
            }

            if (target.DistanceToPlayer() > range)
            {
                return false;
            }

            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return false;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("ShroudofDarkness"))
            {
                return false;
            }

            if (target.HasBuff("SivirShield"))
            {
                return false;
            }

            return !target.HasBuff("FioraW");
        }
    }
}