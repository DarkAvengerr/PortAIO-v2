using EloBuddy;
namespace SkyLv_Taric
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class CastOnDash
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

        static CastOnDash()
        {
            //Menu
            SkyLv_Taric.Menu.SubMenu("Combo").SubMenu("E Settings Combo").AddItem(new MenuItem("Taric.EOnDashendPosition", "Use E On Enemy Dash End Position").SetValue(true));

            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        #region On Dash
        static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {

            var target = TargetSelector.GetTarget(E.Range * 2, TargetSelector.DamageType.Magical);
            var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();

            var td = sender as AIHeroClient;

            if (!td.IsEnemy || td == null || Player.LSIsRecalling())
            {
                return;
            }

            if (SkyLv_Taric.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (sender.NetworkId == target.NetworkId)
                {
                    if (SkyLv_Taric.Menu.Item("Taric.EOnDashendPosition").GetValue<bool>() && E.LSIsReady() && Player.LSDistance(args.EndPos) < E.Range)
                    {
                        var delay = (int)(args.EndTick - Game.Time - E.Delay - 0.1f);
                        if (delay > 0)
                        {
                            Utility.DelayAction.Add(delay * 1000, () => E.Cast(args.EndPos, PacketCast));
                        }
                        else
                        {
                            E.Cast(args.EndPos, PacketCast);
                        }
                    }
                }

                if (SkyLv_Taric.Menu.Item("Taric.UseEFromAlly").GetValue<bool>() && E.LSIsReady() && Player.Mana >= E.ManaCost)
                {
                    foreach (var AllyHero in ObjectManager.Get<AIHeroClient>().Where(a => !a.IsMe && a.IsDead && a.Team == ObjectManager.Player.Team && Player.LSDistance(a) < 1600 && (a.HasBuff("TaricWAllyBuff") || a.HasBuff("TaricW"))))
                    {
                        var Allytarget = ObjectManager.Get<AIHeroClient>().Where(t => !t.IsDead && t.Team != ObjectManager.Player.Team && AllyHero.LSDistance(args.EndPos) < E.Range).FirstOrDefault();

                        if (sender.NetworkId == Allytarget.NetworkId)
                        {
                            if (SkyLv_Taric.Menu.Item(AllyHero.ChampionName + "TargetDashEPEComboFromAlly", true).GetValue<bool>())
                            {
                                var delay = (int)(args.EndTick - Game.Time - E.Delay - 0.1f);
                                if (delay > 0)
                                {
                                    Utility.DelayAction.Add(delay * 1000, () => E.Cast(args.EndPos, PacketCast));
                                }
                                else
                                {
                                    E.Cast(args.EndPos, PacketCast);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
