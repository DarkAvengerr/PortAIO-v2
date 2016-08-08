using System;
using System.Linq;
using Azir_Creator_of_Elo;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Azir_Free_elo_Machine
{

    class JumpLogic
    {
        AzirMain azir;
        Obj_AI_Minion soldier;

        public JumpLogic(AzirMain azir)
        {
            this.azir = azir;
        }

        public void updateLogic(Vector3 position)
        {
            if (azir.Spells.W.LSIsReady() && azir.Spells.Q.LSIsReady() && azir.Spells.E.LSIsReady()) //&&R.LSIsReady())
            {
                //   if (azir.soldierManager.ActiveSoldiers.Count == 0|| azir.soldierManager.ActiveSoldiers.Min(t=>t.LSDistance(Game.CursorPos))>azir.Hero.LSDistance(Game.CursorPos) )
                // {
                azir.Spells.W.Cast(HeroManager.Player.Position.LSExtend(position, 450));
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 150,
                    () =>
                        azir.Spells.E.Cast(
                            azir.SoldierManager.Soldiers[azir.SoldierManager.Soldiers.Count - 1].ServerPosition));
                //}
                //else
                //{
                //    LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 150, () => azir.Spells.E.Cast(azir.soldierManager.Soldiers[azir.soldierManager.Soldiers.Count - 1].ServerPosition));
                // }

                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 400, () => fleeq());
            }
        }
        public void updateLogicJumpInsec(Vector3 position)
        {
            if (azir.Spells.W.LSIsReady() && azir.Spells.Q.LSIsReady() && azir.Spells.E.LSIsReady()) //&&R.LSIsReady())
            {
                //   if (azir.soldierManager.ActiveSoldiers.Count == 0|| azir.soldierManager.ActiveSoldiers.Min(t=>t.LSDistance(Game.CursorPos))>azir.Hero.LSDistance(Game.CursorPos) )
                // {
                azir.Spells.W.Cast(HeroManager.Player.Position.LSExtend(position, 450));
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 150,
                    () =>
                        azir.Spells.E.Cast(
                            azir.SoldierManager.Soldiers[azir.SoldierManager.Soldiers.Count - 1].ServerPosition));
                //}
                //else
                //{
                //    LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 150, () => azir.Spells.E.Cast(azir.soldierManager.Soldiers[azir.soldierManager.Soldiers.Count - 1].ServerPosition));
                // }

                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 400, () => fleeqToInsec(position));
            }
        }

        private void fleeqToInsec(Vector3 position)
        {
            if (Vector2.Distance(HeroManager.Player.ServerPosition.LSTo2D(), position.LSTo2D()) < azir.Spells.Q.Range)
            {
                azir.Spells.Q.Cast(position);
            }
            else
            {
                azir.Spells.Q.Cast(HeroManager.Player.Position.LSExtend(position, 1150));
            }
        }

        public void fleeTopos(Vector3 position)
        {
            if (azir.Spells.W.LSIsReady() && azir.Spells.Q.LSIsReady() && azir.Spells.E.LSIsReady()) //&&R.LSIsReady())
            {
                azir.Spells.W.Cast(HeroManager.Player.Position.LSExtend(position, 450));
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 150,
                    () =>
                        azir.Spells.E.Cast(
                            azir.SoldierManager.Soldiers[azir.SoldierManager.Soldiers.Count - 1].ServerPosition));
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 400, () => fleeq(position));
            }
        }

        public void fleeq()
        {
            if (Vector2.Distance(HeroManager.Player.ServerPosition.LSTo2D(), Game.CursorPos.LSTo2D()) < azir.Spells.Q.Range)
            {
                azir.Spells.Q.Cast(Game.CursorPos);
            }
            else
            {
                azir.Spells.Q.Cast(HeroManager.Player.Position.LSExtend(Game.CursorPos, 1150));
            }
        }

        public void fleeq(Vector3 position)
        {
            azir.Spells.Q.Cast(position);
        }

        public void insec(AIHeroClient target)
        {

            if (azir.Hero.LSDistance(target) <= azir.Spells.R.Range)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target);
                if (azir.Hero.LSDistance(target) < 220)
                {
                    var tower =
                        ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(it => it.IsAlly && it.LSIsValidTarget(1000));

                    if (tower != null)
                    {
                        if (azir.Spells.R.Cast(tower.ServerPosition)) return;
                    }

                    if (azir.Spells.R.Cast(Game.CursorPos)) return;
                }


            }
            else
            {
                var pos = Game.CursorPos.LSExtend(target.Position, Game.CursorPos.LSDistance(target.Position) - 250);

                fleeTopos(pos);
            }

        }
    }
}
