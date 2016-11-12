using System;
using Activator.Base;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Spells.Evaders
{
    internal class hallucinatefull : CoreSpell
    {
        internal override string Name => "hallucinatefull";
        internal override string DisplayName => "Hallucinate | R";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas };
        internal override int Priority => 5;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            UseSpell();

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            UseSpell();
                }
            }
        }
    }
}
