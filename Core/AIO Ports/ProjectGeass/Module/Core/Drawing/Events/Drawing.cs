using System;
using LeagueSharp.Common;
using _Project_Geass.Functions;
using _Project_Geass.Functions.Objects;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Module.Core.Drawing.Events
{

    internal class Drawing
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Drawing" /> class.
        /// </summary>
        public Drawing() {EloBuddy.Drawing.OnDraw+=OnDraw;}

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Raises the <see cref="E:Draw" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        public void OnDraw(EventArgs args)
        {
            if (StaticObjects.Player.IsDead)
                return;

            if (StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+".Minion."+"Circle.LastHitHelper").GetValue<Circle>().Active)
                foreach (var minion in Minions.GetEnemyMinions(StaticObjects.Player.AttackRange+150))
                    if (StaticObjects.Player.GetAutoAttackDamage(minion)-5>minion.Health) // Is killable
                        Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius-10, StaticObjects.ProjectMenu.Item(Names.Menu.DrawingItemBase+".Minion."+"Circle.LastHitHelper").GetValue<Circle>().Color, 3);
        }

        #endregion Public Methods
    }

}