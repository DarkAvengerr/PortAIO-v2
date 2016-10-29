using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
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
                        handleEvent[(int)slot] = true;
                }
            }
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
                        handleEvent[(int)args.Slot] = false;
                        var enemy = args.EndPosition.GetEnemiesInRange(200f).OrderByDescending(p => Utility.GetPriority(p.ChampionName)).FirstOrDefault();
                        if (enemy == null)
                            enemy = TargetSelector.GetTarget(OKTWPredictioner.Spells[(int)args.Slot].Range, TargetSelector.DamageType.Physical);



                        if (enemy != null)
                        {
                            OKTWPredictioner.Spells[(int)args.Slot].CastSebby(enemy, Utility.HitchanceArray[OKTWPredictioner.Config.Item("HITCHANCE").GetValue<StringList>().SelectedIndex]);
                        }
                    }
                }
            }
        }
    }
}