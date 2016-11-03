using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallengerYi.Backbone.Menu;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Logic
{
    internal class Spell4Usage
    {
        private static Spell R = new Spell(SpellSlot.R);
        internal Spell4Usage()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            var target = Variables.TargetSelector.GetTarget(1400);
            if (Spell4Menu.UseR && Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo && target != null)
            {
                if (target.Distance(Game.CursorPos) < 300 && target.Distance(ObjectManager.Player) > 600)
                {
                    R.Cast();
                }
                if (Items.CanUseItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
        }
    }
}
