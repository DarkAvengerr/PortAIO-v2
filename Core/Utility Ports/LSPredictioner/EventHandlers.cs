using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;

using EloBuddy;
using LeagueSharp.Common;
using SebbyLib;

namespace LSPredictioner
{
    public class EventHandlers
    {
        private static bool[] handleEvent = { true, true, true, true };

        public static void Game_OnGameLoad()
        {
            LSPredictioner.Initialize();
        }

        public static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                SpellSlot slot = ObjectManager.Player.GetSpellSlot(args.SData.Name);

                if (!Utility.IsValidSlot(slot))
                    return;

                if (!handleEvent[(int)slot])
                {
                    if (LSPredictioner.Spells[(int)slot] != null)
                    {
                        handleEvent[(int)slot] = true;
                    }
                }
            }
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                if (LSPredictioner.Config.Item("ENABLED").GetValue<bool>() && (LSPredictioner.Config.Item("COMBOKEY").GetValue<KeyBind>().Active || LSPredictioner.Config.Item("HARASSKEY").GetValue<KeyBind>().Active))
                {
                    if (!Utility.IsValidSlot(args.Slot))
                        return;

                    if (LSPredictioner.Spells[(int)args.Slot] == null)
                        return;

                    if (!LSPredictioner.Config.Item(String.Format("{0}{1}", ObjectManager.Player.ChampionName, args.Slot)).GetValue<bool>())
                        return;

                    if (handleEvent[(int)args.Slot])
                    {
                        args.Process = false;

                        var enemy = TargetSelector.GetTarget(LSPredictioner.Spells[(int)args.Slot].Range, TargetSelector.DamageType.Physical);

                        var QWER = LSPredictioner.Spells[(int)args.Slot];

                        if (enemy != null)
                        {
                            var predInput2 = new PortAIO.PredictionInput
                            {
                                Collision = QWER.Collision,
                                Speed = QWER.Speed,
                                Delay = QWER.Delay,
                                Range = QWER.Range,
                                From = Player.ServerPosition,
                                Radius = QWER.Width,
                                Unit = enemy,
                            };
                            
                            var poutput2 = PortAIO.Prediction.GetPrediction(predInput2);

                            if (ShineCommon.Utility.HitchanceArray[LSPredictioner.Config.Item("SPREDHITC").GetValue<StringList>().SelectedIndex] == HitChance.VeryHigh)
                            {
                                if (poutput2.Hitchance >= PortAIO.HitChance.VeryHigh)
                                {
                                    if (QWER.Cast(poutput2.CastPosition))
                                        handleEvent[(int)args.Slot] = false;
                                }
                            }
                            else if (ShineCommon.Utility.HitchanceArray[LSPredictioner.Config.Item("SPREDHITC").GetValue<StringList>().SelectedIndex] == HitChance.High)
                            {
                                if (poutput2.Hitchance >= PortAIO.HitChance.High)
                                {
                                    if (QWER.Cast(poutput2.CastPosition))
                                        handleEvent[(int)args.Slot] = false;
                                }
                            }
                            else if (ShineCommon.Utility.HitchanceArray[LSPredictioner.Config.Item("SPREDHITC").GetValue<StringList>().SelectedIndex] == HitChance.Medium)
                            {
                                if (poutput2.Hitchance >= PortAIO.HitChance.Medium)
                                {
                                    if (QWER.Cast(poutput2.CastPosition))
                                        handleEvent[(int)args.Slot] = false;
                                }
                            }
                            else if (ShineCommon.Utility.HitchanceArray[LSPredictioner.Config.Item("SPREDHITC").GetValue<StringList>().SelectedIndex] == HitChance.Low)
                            {
                                if (poutput2.Hitchance >= PortAIO.HitChance.Low)
                                {
                                    if (QWER.Cast(poutput2.CastPosition))
                                        handleEvent[(int)args.Slot] = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
