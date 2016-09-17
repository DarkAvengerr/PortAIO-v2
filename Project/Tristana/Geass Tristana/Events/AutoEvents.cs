using System;
using System.Linq;
using Geass_Tristana.Misc;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Events
{
    internal class AutoEvents : Core
    {
        public const string MenuItemBase = ".Auto.";
        public const string MenuNameBase = ".Auto Menu";

        private readonly Misc.Damage _damageLib = new Misc.Damage();
        public void OnUpdate(EventArgs args)
        {
            AutoKS();
        }
        // ReSharper disable once InconsistentNaming
        void AutoKS()
        {
            if (!DelayHandler.CheckKS())return;
            DelayHandler.UseKS();

            if (!SMenu.Item(MenuItemBase + "Boolean.AutoRKS.Use").GetValue<bool>())return;
            
            if (!Champion.GetSpellR.IsReady()) return;

            foreach (var enemy in (ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(Champion.GetSpellR.Range)).OrderBy(hp => hp.Health)))
            {
                if (_damageLib.CalcDamage(enemy) < enemy.Health) continue;
                Logger.WriteLog($"Auto KS on {enemy}");
                Champion.GetSpellR.Cast(enemy);
                return;
            }
        }
        public void AntiGapClose(ActiveGapcloser user)
        {
            if (!DelayHandler.CheckGapClose()) return;
            DelayHandler.UseGapClose();

            if (!SMenu.Item(MenuItemBase + "Boolean.Interruption.Use.On." + user.Sender.ChampionName).GetValue<bool>()) return;
            if (!Champion.GetSpellR.IsReady()) return;

            if (user.Sender.Distance(Champion.Player) > Champion.GetSpellR.Range) return;

            Logger.WriteLog($"Interrupt Gap R on : {user.Sender}");
            Champion.GetSpellR.Cast(user.Sender);
        }

        public void AutoInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!DelayHandler.CheckInterrption()) return;
            DelayHandler.UseInterrption();
            // Chat.Print($"{sender.ChampionName}");

            if (!SMenu.Item(MenuItemBase + "Boolean.AntiGapClose.Use.On." + sender.ChampionName).GetValue<bool>()) return;

            if (!Champion.GetSpellR.IsReady()) return;

            if (sender.Distance(Champion.Player) > Champion.GetSpellR.Range) return;

             Logger.WriteLog($"Interrupt Cast R on : {sender}");
            Champion.GetSpellR.Cast(sender);
        }
    }
}