using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.Events
{
    class Game_SpellProcess
    {
        public static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            
            if (hero != null)
                if (args.Target != null)
                    if (args.Target.IsMe)
                        if (hero.Type == GameObjectType.AIHeroClient)
                            if (hero.IsEnemy)
                                if (hero.IsMelee)
                                    if (args.SData.IsAutoAttack())
                                        if (Program.qmenu.Item("AntiMQ").GetValue<bool>())
                                            if (Program.Q.IsReady())
                                                Program.Q.Cast(ObjectManager.Player.Position.Extend(hero.Position, -Program.Q.Range));

        }
    }
}
