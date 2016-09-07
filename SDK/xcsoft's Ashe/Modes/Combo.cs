using System.Linq;

using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;

using Settings = xcAshe.Config.Modes.Combo;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcAshe.Modes
{
    internal sealed class Combo : ModeBase
    {
        internal override bool ShouldBeExecuted()
        {
            return Config.Keys.ComboActive;
        }

        internal override void Execute()
        {
            if (!Variables.Orbwalker.CanMove)
            {//평타모션중엔 리턴
                return;
            }

            if (Settings.UseW && W.IsReady())
            {
                var target = Variables.TargetSelector.GetTargetNoCollision(W);
                if (target != null)
                {//충돌없는 적 챔프있으면 W맞추기
                    W.Cast(target);
                }
            }

            if (Settings.UseR && R.IsReady())
            {
                foreach (var hero in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && !Invulnerable.Check(x, DamageType.Magical, false)))
                {
                    var theDistance = hero.Distance(GameObjects.Player);

                    if (theDistance <= 400)
                    {//적과의 거리가 400이하면 궁극기 맞추기
                        if (R.Cast(hero).HasFlag(CastStates.SuccessfullyCasted))
                        {//궁 사용했으면 foreach문 탈출
                            break;
                        }
                    }

                    if (theDistance <= 1000 && hero.IsKillableWithR(false, (float)GameObjects.Player.GetAutoAttackDamage(hero) * 2))
                    {//적과의 거리가 1000이하고 궁x2+평타로 죽일 수 있으면 궁극기 맞추기
                        if (R.Cast(hero).HasFlag(CastStates.SuccessfullyCasted))
                        {//궁 사용했으면 foreach문 탈출
                            break;
                        }
                    }

                    if (theDistance <= 2500 && hero.IsImmobileUntil() > theDistance / R.Speed)
                    {//적과의 거리가 2500이하고 궁극기가 날라가서 맞기전까지 적이 CC기에 걸려있을거라면 궁극기 시전
                        if (R.Cast(hero).HasFlag(CastStates.SuccessfullyCasted))
                        {//궁 사용했으면 foreach문 탈출
                            break;
                        }
                    }
                }
            }
        }
    }
}
