using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nasus
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;


    public class Standards{

        protected static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>{

            {
                Spells.Q,
                new Spell(SpellSlot.Q, Orbwalking.GetRealAutoAttackRange(Player)+ 50)
            },
            {
                Spells.W,
                new Spell(SpellSlot.W, 600)
            },
            {
                Spells.E,
                new Spell(SpellSlot.E, 650)
            },
            {
                Spells.R,
                new Spell(SpellSlot.R, 20)
            }
    };

        #region Public Properties
        public static Orbwalking.Orbwalker Orbwalker;
        public static AIHeroClient Player => ObjectManager.Player;
        public static bool IsActive(string menuItem) => MenuInit.Menu.Item(menuItem).IsActive();
        #endregion
    }
}
