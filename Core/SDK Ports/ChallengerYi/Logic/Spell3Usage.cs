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
    internal class Spell3Usage
    {
        private static Spell E = new Spell(SpellSlot.E);
        internal Spell3Usage()
        {
            Variables.Orbwalker.OnAction += (sender, args) =>
            {
                if (!E.IsReady()) return;
                if (args.Type == OrbwalkingType.BeforeAttack)
                {
                    if (args.Target is AIHeroClient && Spell3Menu.UseECombo)
                    {
                        E.Cast();
                    }
                    if (args.Target is Obj_AI_Minion && Spell3Menu.UseEFarm)
                    {
                        var minion = args.Target as Obj_AI_Minion;
                        if ((minion.CharData.BaseSkinName.Contains("SRU") || minion.CharData.BaseSkinName.Contains("TT")) &&
                            !minion.CharData.BaseSkinName.ToLower().Contains("minion") && minion.Health > ObjectManager.Player.GetAutoAttackDamage(minion) * 2)
                        {
                            E.Cast();
                        }
                    }
                }
            };
        }
    }
}
