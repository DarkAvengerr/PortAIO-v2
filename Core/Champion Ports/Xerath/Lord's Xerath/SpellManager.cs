using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

using SharpDX;
using Xerath;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lords_Xerath
{
    public static class SpellManager
    {
        private static readonly AIHeroClient player = ObjectManager.Player;

        public delegate void TapKeyPressedEventHandler();
        public static event TapKeyPressedEventHandler OnTapKeyPressed;

        public static Spell Q { get; private set; }
        public static Spell W { get; private set; }
        public static Spell E { get; private set; }
        public static Spell R { get; private set; }


        public static Spell.CastStates[] Spells { get; private set; }
        public static Dictionary<SpellSlot, Color> ColorTranslation { get; private set; }

        public static bool IsCastingUlt
        {
            get { return player.Buffs.Any(b => b.Caster.IsMe && b.IsValidBuff() && b.DisplayName == "XerathR"); }
        }
        public static int LastChargeTime { get; private set; }
        public static Vector3 LastChargePosition { get; private set; }

        public static int MaxCharges
        {
            get { return !R.IsSkillshot ? 3 : 2 + R.Level; }
        }

        public static int ChargesRemaining { get; private set; }

        public static bool TapKeyPressed { get; private set; }

        public static void Initialize()
        {
            // Initialize spells
            Q = new Spell(SpellSlot.Q, 1550);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 1050);
            R = new Spell(SpellSlot.R, 675);

            // Finetune spells
            Q.SetSkillshot(0.6f, 95f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 130f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Q.SetCharged(750, 1550, 1.5f);

            // Setup ult management
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnWndProc += Game_OnWndProc;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        static void Game_OnWndProc(WndEventArgs args)
        {
            if (IsCastingUlt && args.Msg == (uint)WindowsMessages.WM_KEYUP && args.WParam == Config.KeyLinks["ultSettingsKeyPress"].Value.Key)
            {
                // Only handle the tap key if the mode is set to tap key
                switch (Config.StringListLinks["ultSettingsMode"].Value.SelectedIndex)
                {
                    // Auto
                    case 3:
                    // Near mouse
                    case 4:

                        // Tap key has been pressed
                        if (OnTapKeyPressed != null)
                            OnTapKeyPressed();
                        TapKeyPressed = true;
                        break;
                }
            }
        }

        private static float previousLevel = 0;
        private static void Game_OnGameUpdate(EventArgs args)
        {
            // Adjust R range
            if (previousLevel < R.Level)
            {
                R.Range = 2000 + 1200 * R.Level;
                previousLevel = R.Level;
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                // Ult activation
                if (args.SData.Name == "XerathLocusOfPower2")
                {
                    LastChargePosition = Vector3.Zero;
                    LastChargeTime = 0;
                    ChargesRemaining = 3;
                    TapKeyPressed = false;
                }
                // Ult charge usage
                else if (args.SData.Name == "xerathlocuspulse")
                {
                    LastChargePosition = args.End;
                    LastChargeTime = Environment.TickCount;
                    ChargesRemaining--;
                    TapKeyPressed = false;
                }
            }
        }

        public static bool IsEnabled(this Spell spell, string mode)
        {
            return Config.BoolLinks[string.Concat(mode, "Use", spell.Slot.ToString())].Value;
        }

        public static bool IsEnabledAndReady(this Spell spell, string mode)
        {
            return spell.IsEnabled(mode) && spell.IsReady();
        }

        public static AIHeroClient GetTarget(this Spell spell, IEnumerable<AIHeroClient> excludeTargets = null)
        {
            return TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.Magical, true, excludeTargets);
        }

        public static bool CastOnBestTarget(this Spell spell)
        {
            var target = spell.GetTarget();
            return target != null && spell.Cast(target) == Spell.CastStates.SuccessfullyCasted;
        }

        public static MinionManager.FarmLocation? GetFarmLocation(this Spell spell, MinionTeam team = MinionTeam.Enemy, List<Obj_AI_Base> targets = null)
        {
            // Get minions if not set
            if (targets == null)
                targets = MinionManager.GetMinions(spell.Range, MinionTypes.All, team, MinionOrderTypes.MaxHealth);
            // Validate
            if (!spell.IsSkillshot || targets.Count == 0)
                return null;
            // Predict minion positions
            var positions = MinionManager.GetMinionsPredictedPositions(targets, spell.Delay, spell.Width, spell.Speed, spell.From, spell.Range, spell.Collision, spell.Type);
            // Get best location to shoot for those positions
            var farmLocation = MinionManager.GetBestLineFarmLocation(positions, spell.Width, spell.Range);
            if (farmLocation.MinionsHit == 0)
                return null;
            return farmLocation;
        }
    }
}
