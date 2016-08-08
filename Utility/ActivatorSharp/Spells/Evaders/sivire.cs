using System;
using Activator.Base;

using EloBuddy; namespace Activator.Spells.Evaders
{
    class sivire : CoreSpell
    {
        internal override string Name => "sivire";
        internal override string DisplayName => "Spell Shield | E";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SpellShield, MenuType.Zhonyas };
        internal override int DefaultHP => 30;
        internal override int DefaultMP => 40;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId != Player.NetworkId)
                    continue;

                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (Menu.Item("ss" + Name + "all").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                        UseSpell();
                }

                if (Menu.Item("ss" + Name + "cc").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                        UseSpell();
                }

                if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        UseSpell();
                }

                if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        UseSpell();  
                    }
                }
            }
        }
    }
}
