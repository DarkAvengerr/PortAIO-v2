namespace SkyLv_Taric
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Interrupter
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
        }
        #endregion

        static Interrupter()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("Misc").AddItem(new MenuItem("Taric.AutoEInterrupt", "Auto E On Interruptable").SetValue(true));

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();

            if (Player.IsRecalling()) return;

            if (SkyLv_Taric.Menu.Item("Taric.AutoEInterrupt").GetValue<bool>() && E.IsReady() && sender.IsValidTarget(E.Range))
                E.Cast(sender, PacketCast);

            if (SkyLv_Taric.Menu.Item("Taric.UseEFromAlly").GetValue<bool>() && E.IsReady() && Player.Mana >= E.ManaCost)
            {
                foreach (var AllyHero in ObjectManager.Get<AIHeroClient>().Where(a => !a.IsMe && !a.IsDead && a.Team == ObjectManager.Player.Team && Player.Distance(a) < 1600 && (a.HasBuff("TaricWAllyBuff") || a.HasBuff("TaricW"))))
                {
                    var Allytarget = ObjectManager.Get<AIHeroClient>().Where(t => !t.IsDead && t.Team != ObjectManager.Player.Team && AllyHero.Distance(t) < E.Range).FirstOrDefault();

                    if (SkyLv_Taric.Menu.Item(AllyHero.ChampionName + "TargetInterruptEComboFromAlly", true).GetValue<bool>() && Allytarget.NetworkId == sender.NetworkId)
                    {
                        E.Cast(sender.ServerPosition, PacketCast);
                        return;
                    }
                }
            }
        }
    }
}
