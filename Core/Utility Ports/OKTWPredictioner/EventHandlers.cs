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

namespace OKTWPredictioner
{
    public class EventHandlers
    {
        private static bool[] handleEvent = { true, true, true, true };

        public static void Game_OnGameLoad()
        {
            OKTWPredictioner.Initialize();
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
                    if (OKTWPredictioner.Spells[(int)slot] != null)
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
                if (OKTWPredictioner.Config.Item("ENABLED").GetValue<bool>() && (OKTWPredictioner.Config.Item("COMBOKEY").GetValue<KeyBind>().Active || OKTWPredictioner.Config.Item("HARASSKEY").GetValue<KeyBind>().Active))
                {
                    if (!Utility.IsValidSlot(args.Slot))
                        return;

                    if (OKTWPredictioner.Spells[(int)args.Slot] == null)
                        return;

                    if (!OKTWPredictioner.Config.Item(String.Format("{0}{1}", ObjectManager.Player.ChampionName, args.Slot)).GetValue<bool>())
                        return;

                    if (handleEvent[(int)args.Slot])
                    {
                        args.Process = false;

                        var enemy = TargetSelector.GetTarget(OKTWPredictioner.Spells[(int)args.Slot].Range, TargetSelector.DamageType.Physical);

                        var QWER = OKTWPredictioner.Spells[(int)args.Slot];

                        if (enemy != null)
                        {
                            SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
                            bool aoe2 = false;

                            if (QWER.Type == SkillshotType.SkillshotCircle)
                            {
                                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                                aoe2 = true;
                            }

                            if (QWER.Width > 80 && !QWER.Collision)
                                aoe2 = true;

                            var predInput2 = new SebbyLib.Prediction.PredictionInput
                            {
                                Aoe = aoe2,
                                Collision = QWER.Collision,
                                Speed = QWER.Speed,
                                Delay = QWER.Delay,
                                Range = QWER.Range,
                                From = Player.ServerPosition,
                                Radius = QWER.Width,
                                Unit = enemy,
                                Type = CoreType2
                            };
                            
                            var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                            if (QWER.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                                return;

                            if (ShineCommon.Utility.HitchanceArray[OKTWPredictioner.Config.Item("SPREDHITC").GetValue<StringList>().SelectedIndex] == HitChance.VeryHigh)
                            {
                                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                                {
                                    if (QWER.Cast(poutput2.CastPosition))
                                        handleEvent[(int)args.Slot] = false;
                                }
                                else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                                {
                                    if (QWER.Cast(poutput2.CastPosition))
                                        handleEvent[(int)args.Slot] = false;
                                }
                            }
                            else if (ShineCommon.Utility.HitchanceArray[OKTWPredictioner.Config.Item("SPREDHITC").GetValue<StringList>().SelectedIndex] == HitChance.High)
                            {
                                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                                {
                                    if (QWER.Cast(poutput2.CastPosition))
                                        handleEvent[(int)args.Slot] = false;
                                }
                            }
                            else if (ShineCommon.Utility.HitchanceArray[OKTWPredictioner.Config.Item("SPREDHITC").GetValue<StringList>().SelectedIndex] == HitChance.Medium)
                            {
                                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                                {
                                    if (QWER.Cast(poutput2.CastPosition))
                                        handleEvent[(int)args.Slot] = false;
                                }
                            }
                            else if (ShineCommon.Utility.HitchanceArray[OKTWPredictioner.Config.Item("SPREDHITC").GetValue<StringList>().SelectedIndex] == HitChance.Low)
                            {
                                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Low)
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
