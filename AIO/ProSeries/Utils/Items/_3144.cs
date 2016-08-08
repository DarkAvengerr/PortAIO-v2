using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Utils.Items
{
    internal class _3144 : Item
    {
        internal override int Id
        {
            get { return 3144; }
        }

        internal override string Name
        {
            get { return "Bilgewater Cutlass"; }
        }

        internal override float Range
        {
            get { return 550; }
        }

        public override void Use()
        {
            var target = ProSeries.Orbwalker.GetTarget();

            if (!target.IsValid<AIHeroClient>())
            {
                return;
            }

            var targetHero = (AIHeroClient) target;

            if (targetHero.LSIsValidTarget(Range))
            {
                LeagueSharp.Common.Items.UseItem(Id, targetHero);
            }
        }
    }
}