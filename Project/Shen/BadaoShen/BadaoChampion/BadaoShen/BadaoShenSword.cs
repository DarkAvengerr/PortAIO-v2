using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoShen
{
    public static class BadaoShenSword
    {
        private static int CheckTick;
        public static void BadaoActivate()
        {
            if (HeroManager.Enemies.Any(x => x.ChampionName == "Shen"))
                Game.OnUpdate += Game_OnUpdate1;
            else
                Game.OnUpdate += Game_OnUpdate2;
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            if (CheckTick >= Utils.GameTimeTickCount - 200)
                return;
            CheckTick = Utils.GameTimeTickCount;
            var ShenBeam = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.IsAlly && x.Name == "ShenThingUnit");
            if (ShenBeam != null)
                BadaoShenVariables.SwordPos = ShenBeam.Position.To2D();
        }

        private static void Game_OnUpdate2(EventArgs args)
        {
            if (CheckTick >= Utils.GameTimeTickCount - 200)
                return;
            CheckTick = Utils.GameTimeTickCount;
            var ShenBeam = ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(x => x.Name == "Shen_Base_Q_beam.troy");
            if (ShenBeam != null)
                BadaoShenVariables.SwordPos = ShenBeam.Position.To2D();
        }
    }
}
