using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Feedlesticks.Core
{
    class Helper
    {
        /// <summary>
        /// w buff checker
        /// </summary>
        public static bool IsWActive
        {
            get { return ObjectManager.Player.HasBuff("fiddlebuff"); }
        }
        /// <summary>
        /// Menu Checker
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns></returns>
        public static bool Enabled (string menuName)
        {
            return Menus.Config.Item(menuName).GetValue<bool>();
        }
        /// <summary>
        /// Menu Slider
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns></returns>
        public static int Slider(string menuName)
        {
            return Menus.Config.Item(menuName).GetValue<Slider>().Value;
        }
        /// <summary>
        /// Draw Color
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns></returns>
        public static Color Color(string menuName)
        {
            return Menus.Config.Item(menuName).GetValue<Circle>().Color;
        }
        /// <summary>
        /// Draw Circle
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="skillRange"></param>
        public static void Circle(string menuName,float skillRange)
        {
            Render.Circle.DrawCircle(ObjectManager.Player.Position, skillRange,Color(menuName));
        }
        /// <summary>
        /// Circle is Active
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns></returns>
        public static bool Active(string menuName)
        {
            return Menus.Config.Item("q.draw").GetValue<Circle>().Active;
        }
        /// <summary>
        /// Enemy Immobile
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsEnemyImmobile(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || target.IsChannelingImportantSpell())
            {
                return true;
            }
            else
            {
                return false;
            }
                
        }
        
        /// <summary>
        /// Process spell cast. thats need for last w game time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Spells.Wable && IsWActive && args.Slot == SpellSlot.W)
            {
                Spells.LastW = Game.Time;
            }
        }
        /// <summary>
        /// W Lock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (IsWActive && sender.Owner.IsMe && Spells.Wable)
            {
                if (args.Slot == SpellSlot.W)
                {
                    args.Process = false;
                    Menus.Orbwalker.SetAttack(false);
                    Menus.Orbwalker.SetMovement(false);
                }
                else
                {
                    args.Process = true;
                    Menus.Orbwalker.SetAttack(true);
                    Menus.Orbwalker.SetMovement(true);
                }
            }
        }
    }
}
