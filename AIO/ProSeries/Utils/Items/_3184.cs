using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Utils.Items
{
    internal class _3184 : Item
    {
        internal override int Id
        {
            get { return 3184; }
        }

        internal override string Name
        {
            get { return "Entropy"; }
        }

        public override void Use()
        {
            var target = ProSeries.Orbwalker.GetTarget();

            if (!target.IsValid<AIHeroClient>())
            {
                return;
            }

            var targetHero = (AIHeroClient) target;

            if (targetHero.LSIsValidTarget())
            {
                LeagueSharp.Common.Items.UseItem(Id, ProSeries.Player);
            }
        }
    }
}