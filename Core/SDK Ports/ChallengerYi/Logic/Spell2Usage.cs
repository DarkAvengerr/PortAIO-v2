using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallengerYi.Backbone.Menu;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Logic
{
    internal class Spell2Usage
    {
        private static Spell W = new Spell(SpellSlot.W);
        internal Spell2Usage()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private void OnProcessSpellCast(Obj_AI_Base caster, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (caster.IsEnemy &&
                    (args.Target.IsMe ||
                     (args.End != null && args.End.Distance(ObjectManager.Player.ServerPosition) < 200)))
                {
                    if (caster is AIHeroClient && caster.Distance(ObjectManager.Player) < 900)
                    {
                        var enemy = caster as AIHeroClient;
                        if (Damage.GetSpellDamage(enemy, ObjectManager.Player, args.Slot)/ObjectManager.Player.Health*
                            100 >
                            Spell2Menu.UseWOnDangerousSpell)
                        {
                            W.Cast();
                        }
                    }
                    /*if (caster is Obj_AI_Turret && Spell2Menu.UseWOnTowerShots)
                    {
                        DelayAction.Add(350, () =>
                        {
                            if (!ObjectManager.Player.IsUnderEnemyTurret())
                            {
                                Chat.Print(caster.Name + args.Slot);
                                W.Cast();
                            }
                        });
                    }*/
                }
            }
            catch (NullReferenceException ex)
            {
                
            }
        }
    }
}
