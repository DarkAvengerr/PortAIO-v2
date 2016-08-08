using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace HeavenStrikeAzir
{
    public static class AzirCombo
    {
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static void Initialize()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program._orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (Soldiers.enemies.Any() && OrbwalkCommands.CanDoAttack())
            {
                var target = Soldiers.enemies.OrderByDescending(x => x.Health).LastOrDefault();
                OrbwalkCommands.AttackTarget(target);
            }
            if (Soldiers.splashautoattackchampions.Any() && OrbwalkCommands.CanDoAttack())
            {
                var splashAutoAttackChampion = Soldiers.splashautoattackchampions
                    .OrderByDescending(x => x.SplashAutoAttackChampions.MinOrDefault(y => y.Health).Health).LastOrDefault();
                if (splashAutoAttackChampion != null)
                {
                    var target = splashAutoAttackChampion.MainMinion;
                    if (target.LSIsValidTarget())
                        OrbwalkCommands.AttackTarget(target);
                }
            }
            if (!Soldiers.enemies.Any() && OrbwalkCommands.CanDoAttack())
            {
                var target = Program._orbwalker.GetTarget();
                if (target.LSIsValidTarget() && !target.IsZombie)
                {
                    OrbwalkCommands.AttackTarget(target);
                }
            }
            if (Program._q.LSIsReady() && OrbwalkCommands.CanMove() && Program.qcombo && (!Program.donotqcombo || (!Soldiers.enemies.Any() && !Soldiers.splashautoattackchampions.Any())))
            {
                var target = TargetSelector.GetTarget(Program._q.Range, TargetSelector.DamageType.Magical);
                foreach (var obj in Soldiers.soldier)
                {
                    Program._q.SetSkillshot(0.0f, 65f, 1500f, false, SkillshotType.SkillshotLine, obj.Position, Player.Position);
                    Program._q.Cast(target);
                }
            }
            if (Program._w.LSIsReady() && OrbwalkCommands.CanMove() && Program.wcombo)
            {
                var target = TargetSelector.GetTarget(Program._w.Range + 300, TargetSelector.DamageType.Magical);
                if (target.LSIsValidTarget() && !target.IsZombie && (!Soldiers.enemies.Contains(target) || Player.LSCountEnemiesInRange(1000) >= 2))
                {
                    var x = Player.LSDistance(target.Position) > Program._w.Range ? Player.Position.LSExtend(target.Position, Program._w.Range)
                        : target.Position;
                    Program._w.Cast(x);
                }
            }
            if (Program._w.LSIsReady() && OrbwalkCommands.CanMove() && !Soldiers.soldier.Any() && Program.wcombo && Program.Qisready())
            {
                var target = TargetSelector.GetTarget(Program._w.Range + 300, TargetSelector.DamageType.Magical);
                if (target == null || !target.LSIsValidTarget() || target.IsZombie)
                {
                    var tar = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Program._q.Range) && !x.IsZombie).OrderByDescending(x => Player.LSDistance(x.Position)).LastOrDefault();
                    if (tar.LSIsValidTarget() && !tar.IsZombie)
                    {
                        var x = Player.LSDistance(tar.Position) > Program._w.Range ? Player.Position.LSExtend(tar.Position, Program._w.Range)
                            : tar.Position;
                        Program._w.Cast(x);
                    }
                }
            }
        }
    }
}
