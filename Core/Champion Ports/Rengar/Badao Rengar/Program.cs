using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoRengar
{
    // rengarpassivebuff , RengarR
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static void Main()
        {
            Game_OnGameLoad();
        }

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Rengar")
                return;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Game.OnUpdate += Game_OnUpdate;

            Config.BadaoActivate();
            Combo.BadaoActivate();
            Clear.BadaoActivate();
            Assasinate.BadaoActivate();
            Magnet.BadaoActivate();
            Auto.BadaoActivate();
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                args.Process = false;
                Player.Spellbook.CastSpell(SpellSlot.Q, Player.Position.Extend(args.StartPosition, Variables.Q.Range), false);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            //if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            //    return;
            //string buffs = "";
            //foreach (var buff in Player.Buffs)
            //{
            //    buffs = buffs + buff.Name + " ; ";
            //}
            //Chat.Print(buffs);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Slot == SpellSlot.Q)
                Orbwalking.ResetAutoAttackTimer();
        }
    }
}
