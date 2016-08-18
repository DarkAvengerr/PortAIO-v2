using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoTheLastMemebender.Skills
{
    static class WindWall
    {

        public static Spell W = new Spell(SpellSlot.W, 400);
        public static void  Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {

            try
            {
                if (!args.Target.IsMe || args.SData.MissileUnblockable || !Config.Param<bool>(string.Format("ylm.windwall.spells.{0}", args.SData.Name)))
                {
                    return;
                }
                W.Cast(args.Start);

            }
            catch (Exception)
            {
                throw ;
            }
            Console.WriteLine(args.SData.Name);


        }
    }
}
