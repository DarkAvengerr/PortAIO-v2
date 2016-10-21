using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.Perma_Active.AntiGapcloser
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using Lucian.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

   internal sealed class GapcloseFastAsFuckBOi : OrbwalkingChild
    {
        public override string Name { get; set; } = "Anti-Gapcloser";

       private readonly ESpell eSpell;

       public GapcloseFastAsFuckBOi(ESpell eSpell)
       {
           this.eSpell = eSpell;
       }

       public void GapcloseFastAsFuckBoi(ActiveGapcloser gapcloser)
       {
           var target = gapcloser.Sender;

           if (!target.IsEnemy || !target.IsValidTarget(600))
           {
               return;
           }

           eSpell.Spell.Cast(Game.CursorPos);
       }
    }
}
