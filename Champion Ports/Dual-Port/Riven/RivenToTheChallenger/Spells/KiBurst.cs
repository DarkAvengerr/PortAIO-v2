using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RivenToTheChallenger.Spells
{
    static class KiBurst
    {
        public static event Action OnWCasted;
        public static Spell W { get; }

        static KiBurst()
        {
            W = new Spell(SpellSlot.W);
            W.SetSkillshot(250, 250, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.Range = 0;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || OnWCasted == null)
            {
                return;
            }
            if (args.Slot == SpellSlot.W)
            {
                LeagueSharp.Common.Utility.DelayAction.Add((int)W.Delay + Game.Ping, () => { OnWCasted(); });
            }
        }
    }
}
