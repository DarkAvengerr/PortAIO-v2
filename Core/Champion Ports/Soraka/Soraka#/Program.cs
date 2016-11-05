using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SorakaSharp.Source.Handler;
using SorakaSharp.Source.Manager.Heal;
using SorakaSharp.Source.Manager.Heal.Ultimate;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SorakaSharp.Source
{
    internal static class Program
    {
        // Initialize Champion
        internal const string ChampionName = "Soraka";

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }
        
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            // Validate Champion
            if (Player.ChampionName != ChampionName)
                return;

            //Load Handlers
            CConfig.Initialize();
            CSpell.Initialize();

            //Listen to Additional Events
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!CConfig.ConfigMenu.Item("AntiGapCloser").GetValue<bool>() || !CSpell.E.CanCast(gapcloser.Sender))
                return;

            CSpell.E.Cast(gapcloser.Sender);
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!CConfig.ConfigMenu.Item("InterruptSpells").GetValue<bool>() || !CSpell.E.CanCast(sender) ||
                args.DangerLevel != Interrupter2.DangerLevel.High)
                return;

            CSpell.E.Cast(sender);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            CHeal.AutoHeal();
            CUltimate.SmartSave();
            CUltimate.TeamfightUltimate();

            if (CConfig.ConfigMenu.Item("comboActive").GetValue<KeyBind>().Active)
            {
                CCombo.Combo();
            }

            foreach (var enemy in ObjectHandler.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(CSpell.E.Range)))
            {
                if (enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                    enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                    enemy.HasBuffOfType(BuffType.Taunt) || enemy.HasBuffOfType(BuffType.Suppression) ||
                    enemy.IsStunned || enemy.HasBuff("Recall"))
                {
                    CSpell.E.Cast(enemy);
                }
                else
                {
                    CSpell.E.CastIfHitchanceEquals(enemy, HitChance.Immobile, true);
                }
            }
        }
    }
}
