
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Sivir
{
    using System;
    using System.Linq;

    using ExorAIO.Utilities;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() && !Bools.HasSheenBuff()
                && Vars.Menu["spells"]["q"]["logical"].GetValue<MenuBool>().Value)
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t => Bools.IsImmobile(t) && !Invulnerable.Check(t) && t.IsValidTarget(Vars.Q.Range - 50f)))
                {
                    Vars.Q.Cast(target.ServerPosition);
                }
            }

            /// <summary>
            ///     Block Special AoE.
            /// </summary>
            foreach (var target in GameObjects.EnemyHeroes)
            {
                var buff1 = target.GetBuff("jaxcounterstrike");
                var buff2 = target.GetBuff("kogmawicathiansurprise");
                switch (target.ChampionName)
                {
                    case "Jax":
                        if (target.HasBuff("jaxcounterstrike")
                            && target.IsValidTarget(355 + GameObjects.Player.BoundingRadius)
                            && buff1.EndTime - Game.Time > buff1.EndTime - buff1.StartTime - 1
                            && Vars.Menu["spells"]["e"]["whitelist"][$"{target.ChampionName.ToLower()}.jaxcounterstrike"
                                   ].GetValue<MenuBool>().Value)
                        {
                            Vars.E.Cast();
                        }
                        break;
                    case "KogMaw":
                        if (target.HasBuff("kogmawicathiansurprise")
                            && target.IsValidTarget(355 + GameObjects.Player.BoundingRadius)
                            && buff2.EndTime - Game.Time > buff2.EndTime - buff2.StartTime - 4
                            && Vars.Menu["spells"]["e"]["whitelist"][
                                $"{target.ChampionName.ToLower()}.kogmawicathiansurprise"].GetValue<MenuBool>().Value)
                        {
                            Vars.E.Cast();
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///     Called while processing Spellcasting operations.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        public static void AutoShield(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsEnemy)
            {
                return;
            }

            switch (sender.Type)
            {
                case GameObjectType.AIHeroClient:
                    if (Invulnerable.Check(GameObjects.Player, DamageType.Magical, false))
                    {
                        return;
                    }

                    var hero = sender as AIHeroClient;
                    if (hero != null)
                    {
                        /// <summary>
                        ///     Block Gangplank's Barrels.
                        /// </summary>
                        if (hero.ChampionName.Equals("Gangplank"))
                        {
                            if (!(args.Target is Obj_AI_Minion))
                            {
                                return;
                            }

                            if (AutoAttack.IsAutoAttack(args.SData.Name) || args.SData.Name.Equals("GangplankQProceed"))
                            {
                                var target = (Obj_AI_Minion)args.Target;
                                if ((int)target.Health == 1 && target.DistanceToPlayer() < 450
                                    && target.CharData.BaseSkinName.Equals("gangplankbarrel"))
                                {
                                    Vars.E.Cast();
                                    return;
                                }
                            }
                        }

                        var spellMenu =
                            Vars.Menu["spells"]["e"]["whitelist"][
                                $"{hero.ChampionName.ToLower()}.{args.SData.Name.ToLower()}"];

                        var resetMenu = hero.Buffs.Any(b => AutoAttack.IsAutoAttackReset(b.Name))
                                            ? Vars.Menu["spells"]["e"]["whitelist"][
                                                $"{hero.ChampionName.ToLower()}.{hero.Buffs.First(b => AutoAttack.IsAutoAttackReset(b.Name)).Name.ToLower()}"
                                                  ]
                                            : null;

                        /// <summary>
                        ///     Check for Special On-Hit CC AutoAttacks & Melee AutoAttack Resets.
                        /// </summary>
                        if (AutoAttack.IsAutoAttack(args.SData.Name))
                        {
                            if (!args.Target.IsMe)
                            {
                                return;
                            }

                            switch (args.SData.Name)
                            {
                                case "UdyrBearAttack":
                                case "BraumBasicAttackPassiveOverride":
                                case "GoldCardPreAttack":
                                case "RedCardPreAttack":
                                case "BlueCardPreAttack":
                                    if (spellMenu == null || !spellMenu.GetValue<MenuBool>().Value
                                        || (hero.ChampionName.Equals("Udyr")
                                            && GameObjects.Player.HasBuff("udyrbearstuncheck")))
                                    {
                                        return;
                                    }

                                    Vars.E.Cast();
                                    break;
                                default:
                                    if (!hero.Buffs.Any(b => AutoAttack.IsAutoAttackReset(b.Name)) || resetMenu == null
                                        || !resetMenu.GetValue<MenuBool>().Value)
                                    {
                                        return;
                                    }

                                    Vars.E.Cast();
                                    break;
                            }
                        }

                        /// <summary>
                        ///     Shield all the Targetted Spells.
                        /// </summary>
                        else if (SpellDatabase.GetByName(args.SData.Name) != null)
                        {
                            if (spellMenu == null || !spellMenu.GetValue<MenuBool>().Value)
                            {
                                return;
                            }

                            switch (SpellDatabase.GetByName(args.SData.Name).SpellType)
                            {
                                /// <summary>
                                ///     Check for Targetted Spells.
                                /// </summary>
                                case SpellType.Targeted:
                                case SpellType.TargetedMissile:
                                    if (!args.Target.IsMe)
                                    {
                                        return;
                                    }

                                    var delay = Vars.Menu["spells"]["e"]["delay"].GetValue<MenuSlider>().Value;
                                    switch (hero.ChampionName)
                                    {
                                        case "Caitlyn":
                                            delay = 1050;
                                            break;
                                        case "Nocturne":
                                            delay = 350;
                                            break;
                                        case "Zed":
                                            delay = 200;
                                            break;
                                    }

                                    DelayAction.Add(delay, () => { Vars.E.Cast(); });
                                    break;

                                /// <summary>
                                ///     Check for AoE Spells.
                                /// </summary>
                                case SpellType.SkillshotCircle:
                                    switch (hero.ChampionName)
                                    {
                                        case "Alistar":
                                            if (hero.DistanceToPlayer() < 355 + GameObjects.Player.BoundingRadius)
                                            {
                                                Vars.E.Cast();
                                            }
                                            break;
                                    }
                                    break;
                            }
                        }
                    }

                    break;
                case GameObjectType.obj_AI_Minion:
                    if (args.Target == null || !args.Target.IsMe)
                    {
                        return;
                    }

                    /// <summary>
                    ///     Block Dragon/Baron/RiftHerald's AutoAttacks.
                    /// </summary>
                    if (sender.CharData.BaseSkinName.Contains("SRU_Dragon")
                        && Vars.Menu["spells"]["e"]["whitelist"]["minions"].GetValue<MenuBool>().Value)
                    {
                        Vars.E.Cast();
                    }
                    break;
            }
        }

        #endregion
    }
}