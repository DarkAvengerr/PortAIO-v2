namespace SkyLv_Taric
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class OnProcessSpellCast
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Taric.W;
            }
        }
        #endregion

        static OnProcessSpellCast()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if (Player.LSIsRecalling()) return;

            if ((sender.IsValid<AIHeroClient>() || sender.IsValid<Obj_AI_Turret>()) && sender.IsEnemy)
            {
                var PacketCast = SkyLv_Taric.Menu.Item("Taric.UsePacketCast").GetValue<bool>();


                if (SkyLv_Taric.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && SkyLv_Taric.Menu.Item("Taric.UseWIncomingDamageCombo").GetValue<bool>() && W.LSIsReady() && W.ManaCost <= Player.Mana)
                {
                    if (Player.LSDistance(args.End) <= Player.BoundingRadius && sender.LSGetSpellDamage(Player, args.SData.Name.ToString()) > 0)
                    {
                        W.CastOnUnit(Player, PacketCast);
                    }
                }

                if (SkyLv_Taric.Menu.Item("Taric.UseWAlly").GetValue<bool>())
                {

                    switch (SkyLv_Taric.Menu.Item("Taric.UseWAllyMode").GetValue<StringList>().SelectedIndex)
                    {

                        case 0:
                            {
                                if (SkyLv_Taric.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                                {
                                    if (W.LSIsReady() && Player.Mana >= W.ManaCost)
                                    {
                                        foreach (var ally in HeroManager.Allies.Where(x => x.LSIsValidTarget(W.Range, false) && !x.IsMe && Player.LSDistance(x) <= W.Range && SkyLv_Taric.Menu.Item(x.ChampionName + "IncomingDamageWAlly", true).GetValue<bool>() && x.HealthPercent <= SkyLv_Taric.Menu.Item(x.ChampionName + "MinimumHpWAlly").GetValue<Slider>().Value))
                                        {
                                            foreach (var SendingUnit in HeroManager.Enemies.Where(x => x.NetworkId == sender.NetworkId))
                                            {
                                                if (ally.LSDistance(args.End) <= ally.BoundingRadius && SendingUnit.LSGetSpellDamage(ally, args.SData.Name.ToString()) > 0)
                                                {
                                                    W.CastOnUnit(ally, PacketCast);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }

                        case 1:
                            {
                                if (W.LSIsReady() && Player.Mana >= W.ManaCost)
                                {
                                    foreach (var ally in HeroManager.Allies.Where(x => x.LSIsValidTarget(W.Range, false) && !x.IsMe && Player.LSDistance(x) <= W.Range && SkyLv_Taric.Menu.Item(x.ChampionName + "IncomingDamageWAlly", true).GetValue<bool>() && x.HealthPercent <= SkyLv_Taric.Menu.Item(x.ChampionName + "MinimumHpWAlly").GetValue<Slider>().Value))
                                    {
                                        foreach (var SendingUnit in HeroManager.Enemies.Where(x => x.NetworkId == sender.NetworkId))
                                        {
                                            if (ally.LSDistance(args.End) <= ally.BoundingRadius)
                                            {
                                                W.CastOnUnit(ally, PacketCast);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
        }
    }
}
