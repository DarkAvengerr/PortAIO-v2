using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using System.Text.RegularExpressions;
using Color = System.Drawing.Color;
using EloBuddy;

namespace HeavenStrikeAzir
{
    public static class AzirHarass
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void Initialize()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
                return;
            if (OrbwalkCommands.CanDoAttack())
            {
                var minion = OrbwalkCommands.GetClearMinionsAndBuildings();
                if (minion.IsValidTarget())
                {
                    OrbwalkCommands.AttackTarget(minion);
                }
            }
            if (Program._q.IsReady() && OrbwalkCommands.CanMove() && Program.qharass && (!Program.donotqharass || (!Soldiers.enemies.Any() && !Soldiers.splashautoattackchampions.Any())))
            {
                var target = TargetSelector.GetTarget(Program._q.Range, TargetSelector.DamageType.Magical);
                foreach (var obj in Soldiers.soldier)
                {
                    Program._q.SetSkillshot(0.0f, 65f, 1500f, false, SkillshotType.SkillshotLine, obj.Position, Player.Position);
                    Program._q.Cast(target);
                }
            }
            if (Program._w.IsReady() && OrbwalkCommands.CanMove() && !Soldiers.enemies.Any() && Program.wharass)
            {
                var target = TargetSelector.GetTarget(Program._w.Range + 300, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget() && !target.IsZombie && !Soldiers.enemies.Contains(target))
                {
                    var x = Player.Distance(target.Position) > Program._w.Range ? Player.Position.Extend(target.Position, Program._w.Range)
                        : target.Position;
                    Program._w.Cast(x);
                }
            }
            if (Program._w.IsReady() && OrbwalkCommands.CanMove() && !Soldiers.enemies.Any() && !Soldiers.soldier.Any() && Program.wharass && Program.Qisready())
            {
                var target = TargetSelector.GetTarget(Program._w.Range + 300, TargetSelector.DamageType.Magical);
                if (target == null || !target.IsValidTarget() || target.IsZombie)
                {
                    var tar = HeroManager.Enemies.Where(x => x.IsValidTarget(Program._q.Range) && !x.IsZombie).OrderByDescending(x => Player.Distance(x.Position)).LastOrDefault();
                    if (tar.IsValidTarget() && !tar.IsZombie)
                    {
                        var x = Player.Distance(tar.Position) > Program._w.Range ? Player.Position.Extend(tar.Position, Program._w.Range)
                            : tar.Position;
                        Program._w.Cast(x);
                    }
                }
            }
        }
    }
}
