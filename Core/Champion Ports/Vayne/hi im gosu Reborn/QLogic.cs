using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hi_im_gosu_Reborn
{
    class QLogic
    {
        private void QLogics(AIHeroClient target)
        {
            if (!Vayne.Q.IsReady())
            {
                return;
            }

            var qPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, Vayne.Q.Range);
            var targetDisQ = target.ServerPosition.Distance(qPosition);
            var canQ = false;

            if (Vayne.qmenu.Item("QTurret", true).GetValue<bool>() && qPosition.UnderTurret(true))
            {
                canQ = false;
            }

            if (Vayne.qmenu.Item("QCheck", true).GetValue<bool>())
            {
                if (qPosition.CountEnemiesInRange(300f) >= 3)
                {
                    canQ = false;
                }

                //Catilyn W
                if (ObjectManager
                        .Get<Obj_GeneralParticleEmitter>()
                        .FirstOrDefault(
                            x =>
                                x != null && x.IsValid &&
                                x.Name.ToLower().Contains("yordletrap_idle_red.troy") &&
                                x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }

                //Jinx E
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "k" &&
                                             x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }

                //Teemo R
                if (ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(x => x.IsValid && x.IsEnemy && x.Name == "Noxious Trap" &&
                                             x.Position.Distance(qPosition) <= 100) != null)
                {
                    canQ = false;
                }
            }

            if (targetDisQ >= Vayne.Q.Range && targetDisQ <= Vayne.Q.Range * 2)
            {
                canQ = true;
            }

            if (canQ)
            {
                Vayne.Q.Cast(qPosition, true);
                canQ = false;
            }
        }
    }
}