using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Core.Condemn_Logic
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal sealed class Condemn
    {
        public ICondemnType CondemnType { get; set; }

        public Condemn(ICondemnType condemnType = null)
        {
            this.CondemnType = condemnType;
        }

        public void Execute(Obj_AI_Base target, float range, Spell spell)
        {
            this.CondemnType.Execute(target, range, spell);
        }
    }
}
