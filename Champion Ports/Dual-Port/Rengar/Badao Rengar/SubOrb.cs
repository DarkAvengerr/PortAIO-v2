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
    public static class SubOrb
    {
        public static int Qtick , QcastTick, EcastTick, WcastTick , FirstBlockTick;
        public static bool FirstBlock;
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            //var pos = Prediction.GetPrediction(Player, 0.25f).UnitPosition;
            //Render.Circle.DrawCircle(pos, 50, Color.Red);
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            //if(Variables.E.IsReady())
            //{
            //    var targetE2 = MinionManager.GetMinions(Player.Position, Variables.E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            //    if (targetE2 != null)
            //    {
            //        Helper.CastE(targetE2);
            //    }
            //}
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Buff.Name == "rengarqbase" || args.Buff.Name == "rengarqemp")
            {
                if (Environment.TickCount - Qtick <= 1500 && Orbwalking.CanAttack())
                    Orbwalking.LastAATick = Utils.GameTimeTickCount - Game.Ping / 2 - (int)(ObjectManager.Player.AttackCastDelay * 1000);
                if (Environment.TickCount - Qtick <= 1500)
                    Orbwalking.FireAfterAttack(Player, Orbwalking._lastTarget);
            }
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Buff.Name == "rengarqbase" || args.Buff.Name == "rengarqemp")
            {
                //Chat.Print(args.Buff.Name);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Slot == SpellSlot.Q)
                Qtick = Environment.TickCount;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - FirstBlockTick > 3000 && FirstBlock == true)
            {
                FirstBlock = false;
            }
        }
        private static int PlayerMana()
        {
            int mana = (int)Player.Mana;
            if (Environment.TickCount - QcastTick < 1500 + Game.Ping)
                mana++;
            if (Environment.TickCount - WcastTick < 1500 + Game.Ping)
                mana++;
            if (Environment.TickCount - EcastTick < 1500 + Game.Ping)
                mana++;
            return mana;
        }
        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
                return;
            if (Player.Mana == 5 && FirstBlock == false && Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None
                && (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E))
            {
                if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Variables.AssassinateKey.GetValue<KeyBind>().Active)
                {
                    int index = Variables.ComboMode.GetValue<StringList>().SelectedIndex;
                    if ((index == 0 || index == 4) && args.Slot == SpellSlot.Q)
                    {
                        args.Process = false;
                        FirstBlockTick = Environment.TickCount;
                        FirstBlock = true;
                        return;
                    }
                    if (args.Slot == SpellSlot.E && (index == 3 || index == 4) )
                    {
                        args.Process = false;
                        FirstBlockTick = Environment.TickCount;
                        FirstBlock = true;
                        return;
                    }
                    if (args.Slot == SpellSlot.W && index != 4 && !(Player.Health * 100 / Player.MaxHealth <= Variables.AutoWHeal.GetValue<Slider>().Value
                        && (Player.Health * 100 / Player.MaxHealth <= 10 || Player.CountEnemiesInRange(1000) > 0)))
                    {
                        args.Process = false;
                        FirstBlockTick = Environment.TickCount;
                        FirstBlock = true;
                        return;
                    }
                }
                if (Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (args.Slot == SpellSlot.E)
                    {
                        args.Process = false;
                        FirstBlockTick = Environment.TickCount;
                        FirstBlock = true;
                        return;
                    }
                    if (args.Slot == SpellSlot.W && !(Player.Health * 100 / Player.MaxHealth <= Variables.AutoWHeal.GetValue<Slider>().Value
                        && (Player.Health * 100 / Player.MaxHealth <= 10 || Player.CountEnemiesInRange(1000) > 0)))
                    {
                        args.Process = false;
                        FirstBlockTick = Environment.TickCount;
                        FirstBlock = true;
                        return;
                    }
                }
            }
            return;

            if (args.Slot == SpellSlot.Q)
            {
                QcastTick = Environment.TickCount;
                if (PlayerMana() > 5)
                {
                    args.Process = false;
                    QcastTick = 0;
                }
            }
            if (args.Slot == SpellSlot.W)
            {
                WcastTick = Environment.TickCount;
                if (PlayerMana() > 5)
                {
                    args.Process = false;
                    WcastTick = 0;
                }
            }
            if (args.Slot == SpellSlot.E)
            {
                EcastTick = Environment.TickCount;
                if (PlayerMana() > 5)
                {
                    args.Process = false;
                    EcastTick = 0;
                }
            }

        }
    }
}
