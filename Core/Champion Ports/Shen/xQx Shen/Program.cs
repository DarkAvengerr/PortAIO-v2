#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Shen.Champion;
using Color = System.Drawing.Color;
using Shen.Common;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
namespace Shen
{
    internal class Program
    {
        public const string ChampionName = "Shen";
        //Orbwalker instance
        
        private static SpellSlot TeleportSlot = ObjectManager.Player.GetSpellSlot("SummonerTeleport");
        //Menu

        private static float EManaCost => ObjectManager.Player.GetSpell(SpellSlot.E).SData.Mana;

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                return;
            }

            Shen.Modes.MenuConfig.Initialize();
            //Shen.Champion.Drawings.Initialize();

            //Create the menu

            Game.OnUpdate += Game_OnUpdate;
            //Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            //Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

            Chat.Print($"<font color='#FFFFFF'>{ChampionName}</font> <font color='#70DBDB'> Loaded!</font>");

        }

        public static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe && sender.IsEnemy && sender is AIHeroClient && args.Target.IsMe)
            {

                Chat.Print(sender.BaseSkinName + " Attacking!!!");
            }

            return;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            //if (Shen.Champion.Spells.E.IsReady() && gapcloser.Sender.IsValidTarget(Shen.Champion.Spells.E.Range) && Shen.Champion.Menu.MenuMisc.Item("GapCloserE").GetValue<bool>())
            //{
            //    Shen.Champion.Spells.E.Cast(gapcloser.Sender.Position);
            //}
        }
        


        public static bool InShopRange(AIHeroClient xAlly)
        {
            return
                (from shop in ObjectManager.Get<Obj_Shop>() where shop.IsAlly select shop).Any<Obj_Shop>(
                    shop => Vector2.Distance(xAlly.Position.To2D(), shop.Position.To2D()) < 1250f);
        }

        public static int CountAlliesInRange(float range, Vector3 point)
        {
            return
                (from units in ObjectManager.Get<AIHeroClient>()
                    where units.IsAlly && units.IsVisible && !units.IsDead
                    select units).Count<AIHeroClient>(
                        units => Vector2.Distance(point.To2D(), units.Position.To2D()) <= range);
        }

        public static int CountEnemysInRange(float range, Vector3 point)
        {
            return
                (from units in ObjectManager.Get<AIHeroClient>() where units.IsValidTarget() select units)
                    .Count<AIHeroClient>(units => Vector2.Distance(point.To2D(), units.Position.To2D()) <= range);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
           
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            //var x = ObjectManager.Player.PassiveCooldownEndTime - Game.Time;
            //Chat.Print(ObjectManager.Player.PassiveCooldownEndTime + "  :  " + (Game.Time - 17));
            //foreach (var buff in ObjectManager.Player.Buffs)
            //{
            //    Console.WriteLine(buff.DisplayName + " : " + buff.StartTime + " : " + buff.EndTime);
            //}
            return;
            //if (ObjectManager.Player.HasPassive())
            //{
            //    Chat.Print("Passive Ok!");
            //}
            //else
            //{
            //    Chat.Print("Don't have Passive");
            //}
            //return;
            ////Chat.Print("------------------------------");
            //if (ObjectManager.Player.Distance(Shen.Champion.SpiritUnit.SwordUnit.Position) < 350f)
            //{
            //    Chat.Print("You are In");
            //}
            //else
            //{
            //    Chat.Print("You are Out");

            //}

        }
        
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t,
            Interrupter2.InterruptableTargetEventArgs args)
        {
       
        }

    }
}
