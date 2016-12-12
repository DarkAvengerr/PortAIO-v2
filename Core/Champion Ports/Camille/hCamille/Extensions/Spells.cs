using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hCamille.Extensions
{
    public static class Spells
    {
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }


        public static void Initializer()
        {
            Q = new Spell(SpellSlot.Q, 325);
            W = new Spell(SpellSlot.W, 580);
            R = new Spell(SpellSlot.R, 475);
            E = new Spell(SpellSlot.E, 865);
            
            E.SetSkillshot(0.3f,30,500,false,SkillshotType.SkillshotLine);
            W.SetSkillshot(0.195f,100,1750,false,SkillshotType.SkillshotCone);

        }
    }
}
