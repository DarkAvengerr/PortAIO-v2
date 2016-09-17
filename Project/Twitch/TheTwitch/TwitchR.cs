using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheTwitch.Commons.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch
{
    class TwitchR : Skill
    {
        public int MinEnemies;
        public Circle DrawRange;

        public TwitchR(SpellSlot spell)
            : base(spell)
        {
            HarassEnabled = false;
        }

        public override void Execute(AIHeroClient target)
        {
            if (HeroManager.Enemies.Count(hero => hero.IsValidTarget(1000)) >= MinEnemies)
                Cast();
        }

        public override int GetPriority()
        {
            return 1;
        }

        public override void Draw()
        {
            if (DrawRange.Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 975, DrawRange.Color);
        }
    }
}
