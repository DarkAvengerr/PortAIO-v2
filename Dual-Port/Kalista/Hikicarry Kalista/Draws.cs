using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Kalista
{
    class Draws
    {
        public static void EPercentOnEnemy(Color color)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsHPBarRendered && o.IsEnemy && o.LSIsValidTarget(1000)))
            {
                float getTotalDamage = Calculators.ChampionTotalDamage(enemy);
                float tDamage = getTotalDamage * 100 / enemy.Health;
                int totalDamage = (int)Math.Ceiling(tDamage);

                if (totalDamage > 0)
                {
                    Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y, color, string.Format("{0}%", totalDamage));
                }
            }
        }
        public static void EPercentOnJungleMobs(Color color)
        {
            foreach (
                var jungleMobs in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            o =>
                                o.LSIsValidTarget(Program.E.Range) && o.Team == GameObjectTeam.Neutral && o.IsVisible && !o.IsDead)
                )
            {
                float tDamage = Calculators.JungleTotalDamage(jungleMobs) * 100 / jungleMobs.Health;
                int totalDamage = (int)Math.Ceiling(tDamage);
                if (totalDamage >= 0)
                {
                    switch (jungleMobs.CharData.BaseSkinName)
                    {
                        case "SRU_Razorbeak":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X + 50, jungleMobs.HPBarPosition.Y, color,
                                string.Format("{0}%", totalDamage));
                            break;
                        case "SRU_Red":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y - 3, color,
                                string.Format("{0}%", totalDamage));
                            break;
                        case "SRU_Blue":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y, color,
                                string.Format("{0}%", totalDamage));
                            break;
                        case "SRU_Dragon":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y, color,
                                string.Format("{0}%", totalDamage));
                            break;
                        case "SRU_Baron":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y, color,
                                string.Format("{0}%", totalDamage));
                            break;
                        case "SRU_Gromp":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X, jungleMobs.HPBarPosition.Y, color,
                                string.Format("{0}%", totalDamage));
                            break;
                        case "SRU_Krug":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X + 53, jungleMobs.HPBarPosition.Y, color,
                                string.Format("{0}%", totalDamage));
                            break;
                        case "SRU_Murkwolf":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X + 50, jungleMobs.HPBarPosition.Y, color,
                                string.Format("{0}%", totalDamage));
                            break;
                        case "Sru_Crab":
                            Drawing.DrawText(jungleMobs.HPBarPosition.X + 50, jungleMobs.HPBarPosition.Y + 20, color,
                                string.Format("{0}%", totalDamage));
                            break;
                    }
                }
            }
           
        }
        public static void SkillDraw(Spell spell, Color color,int width)
        {
            Render.Circle.DrawCircle(new Vector3(ObjectManager.Player.Position.X, ObjectManager.Player.Position.Y, ObjectManager.Player.Position.Z), spell.Range, color,width);
        }
        public static void ConnectionSignal(Color color)
        {
            var heroPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var kalistaSupport = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsAlly && x.HasBuff("kalistacoopstrikeally"));

            if (kalistaSupport.LSDistance(ObjectManager.Player.Position) < 500 && !ObjectManager.Player.IsDead)
            {
                Drawing.DrawText(heroPos.X - 100, heroPos.Y + 25, color, "Support Connection Signal: Good");
            }
            if (kalistaSupport.LSDistance(ObjectManager.Player.Position) > 500 && kalistaSupport.LSDistance(ObjectManager.Player.Position) < 1000
                && !ObjectManager.Player.IsDead)
            {
                Drawing.DrawText(heroPos.X - 100, heroPos.Y + 25, color, "Support Connection Signal: Medium");
            }
            if (kalistaSupport.LSDistance(ObjectManager.Player.Position) > 1000 && kalistaSupport.LSDistance(ObjectManager.Player.Position) < 1500
                && !ObjectManager.Player.IsDead)
            {
                Drawing.DrawText(heroPos.X - 100, heroPos.Y + 25, color, "Support Connection Signal: Low");
            }
            if (kalistaSupport.LSDistance(ObjectManager.Player.Position) > 1500 && !ObjectManager.Player.IsDead)
            {
                Drawing.DrawText(heroPos.X - 100, heroPos.Y + 25, color, "Support Connection Signal: Missed");
            }
        }
        public static void CircleOnSupport(Color color)
        {
            var kalistaSupport = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsAlly && x.HasBuff("kalistacoopstrikeally"));
            if (kalistaSupport == null)
            {
                return;
            }
            Render.Circle.DrawCircle(kalistaSupport.Position + new Vector3(0, 0, 15), 100, color, 5, true);
        }
    }
}
