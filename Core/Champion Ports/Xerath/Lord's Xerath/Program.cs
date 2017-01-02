using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lords_Xerath
{
    public class Program
    {
        private Items.Item
        FarsightOrb = new Items.Item(3342, 4000f),
        ScryingOrb = new Items.Item(3363, 3500f);
        private static string News = "Welcome to Copy Pasted Series";

        public Vector3 Rtarget;
        public const string CHAMP_NAME = "Xerath";
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static bool HasIgnite { get; private set; }

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            // Validate champ
            if (player.ChampionName != CHAMP_NAME)
                return;
            Chat.Print("<font size='30'>Lord's Xerath</font> <font color='#b756c5'>by LordZEDith</font>");
            Chat.Print("<font color='#b756c5'>NEWS: </font>" + Program.News);
            // Clear the console
            Utils.ClearConsole();

            // Initialize classes
            SpellManager.Initialize();
            Config.Initialize();

            // Check if the player has ignite
            HasIgnite = player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown;

            // Initialize damage indicator
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit = Damages.GetTotalDamage;
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Color = System.Drawing.Color.Aqua;
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = true;

            // Listend to some other events
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            //Spellbook.OnCastSpell += Spellbook_OnCastSpell;

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            // Always active stuff, ignite and stuff :P
            ActiveModes.OnPermaActive();

            if (Config.KeyLinks["comboActive"].Value.Active)
                ActiveModes.OnCombo();
            if (Config.KeyLinks["harassActive"].Value.Active)
                ActiveModes.OnHarass();
            if (Config.KeyLinks["waveActive"].Value.Active)
                ActiveModes.OnWaveClear();
            if (Config.KeyLinks["jungleActive"].Value.Active)
                ActiveModes.OnJungleClear();
            if (Config.KeyLinks["fleeActive"].Value.Active)
                ActiveModes.OnFlee();
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.WM_KEYUP && Config.BoolLinks["castEnabled"].Value)
            {
                // Single Spell Casts
                if (args.WParam == Config.KeyLinks["castW"].Value.Key && SpellManager.W.IsReady())
                {
                    // Cast W on best target
                    SpellManager.W.CastOnBestTarget();
                }
                else if (args.WParam == Config.KeyLinks["castE"].Value.Key && SpellManager.E.IsReady())
                {
                    // Cast E on best target
                    foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(SpellManager.E.Range) && !OktwCommon.CanMove(enemy)))
                        SpellManager.E.Cast(enemy);
                }
            }
        }
                
            
        

        private static void Drawing_OnDraw(EventArgs args)
        {
            // Draw all circles except for R
            foreach (var circleLink in Config.CircleLinks)
            {
                if (circleLink.Value.Value.Active && circleLink.Key != "drawRangeR")
                    Render.Circle.DrawCircle(player.Position, circleLink.Value.Value.Radius, circleLink.Value.Value.Color);
            }

            // Draw Q while charging
            if (Config.CircleLinks["drawRangeQ"].Value.Active && SpellManager.Q.IsCharging && SpellManager.Q.Range < SpellManager.Q.ChargedMaxRange)
                Render.Circle.DrawCircle(player.Position, SpellManager.Q.Range, Config.CircleLinks["drawRangeQ"].Value.Color);

            // Draw R range
            if (Config.CircleLinks["drawRangeR"].Value.Active)
                Render.Circle.DrawCircle(player.Position, SpellManager.R.Range, Config.CircleLinks["drawRangeR"].Value.Color);
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            // Draw R on minimap
            if (Config.CircleLinks["drawRangeR"].Value.Active && SpellManager.R.Level > 0)
                LeagueSharp.Common.Utility.DrawCircle(player.Position, SpellManager.R.Range, Config.CircleLinks["drawRangeR"].Value.Color, 5, 30, true);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.BoolLinks["miscGapcloseE"].Value && SpellManager.E.IsReady() && SpellManager.E.IsInRange(gapcloser.End))
            {
                if (ObjectManager.Player.Distance(gapcloser.Sender.Position) < SpellManager.E.Range)
                {
                    SpellManager.E.Cast(gapcloser.Sender);
                }
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel == Interrupter2.DangerLevel.High && Config.BoolLinks["miscInterruptE"].Value && SpellManager.E.IsReady() && SpellManager.E.IsInRange(sender))
            {
                if (ObjectManager.Player.Distance(sender) < SpellManager.E.Range)
                {
                    SpellManager.E.Cast(sender);
                }
            }
        }
        public void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                var t = TargetSelector.GetTarget(SpellManager.R.Range, TargetSelector.DamageType.Magical);
                Rtarget = SpellManager.R.GetPrediction(t).CastPosition;
                if (args.Slot == SpellSlot.R)
                {
                    if ((Config.BoolLinks["itemsOrb"].Value) && !IsCastingR)
                    {
                        if (ObjectManager.Player.Level < 9)
                            ScryingOrb.Range = 2500;
                        else
                            ScryingOrb.Range = 3500;

                        if (ScryingOrb.IsReady())
                            ScryingOrb.Cast(Rtarget);
                        if (FarsightOrb.IsReady())
                            FarsightOrb.Cast(Rtarget);
                    }
                }
            }
        }
        public bool IsCastingR
        {
            get
            {
                return ObjectManager.Player.HasBuff("XerathLocusOfPower2", true) ||
                       (ObjectManager.Player.LastCastedSpellName() == "XerathLocusOfPower2" &&
                        Utils.TickCount - ObjectManager.Player.LastCastedSpellT() < 500);
            }
        }
    }
}
