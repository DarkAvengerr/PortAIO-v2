using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.OrbwalkingModes
{
    #region

    using Core;

    using LeagueSharp.Common;

    using Menus;

    #endregion

    internal class JungleClearMode : Core
    {
        #region Public Methods and Operators

        public static void Jungleclear()
        {
            var mobs = MinionManager.GetMinions(
                Player.Position,
                Player.AttackRange + Spells.E.Range,
                MinionTypes.All,
                MinionTeam.Neutral);

            if (mobs == null) return;

            foreach (var m in mobs)
            {
                if (!m.IsValid
                    || !Spells.E.IsReady() 
                    || !MenuConfig.JnglE 
                    || Player.Spellbook.IsAutoAttacking)
                {
                    return;
                }

                Spells.E.Cast(m.Position);
                LeagueSharp.Common.Utility.DelayAction.Add(10, Usables.CastHydra);
            }
        }

        #endregion
    }
}
