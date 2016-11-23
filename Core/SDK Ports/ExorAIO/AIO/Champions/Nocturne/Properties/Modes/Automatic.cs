
#pragma warning disable 1587

using EloBuddy; 
using LeagueSharp.SDK; 
 namespace ExorAIO.Champions.Nocturne
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
                            Vars.W.Cast();
                        }
                        break;
                    case "KogMaw":
                        if (target.HasBuff("kogmawicathiansurprise")
                            && target.IsValidTarget(355 + GameObjects.Player.BoundingRadius)
                            && buff2.EndTime - Game.Time > buff2.EndTime - buff2.StartTime - 4
                            && Vars.Menu["spells"]["e"]["whitelist"][
                                $"{target.ChampionName.ToLower()}.kogmawicathiansurprise"].GetValue<MenuBool>().Value)
                        {
                            Vars.W.Cast();
                        }
                        break;
                }
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() && Vars.Menu["spells"]["r"]["bool"].GetValue<MenuBool>().Value
                && Vars.Menu["spells"]["r"]["key"].GetValue<MenuKeyBind>().Active)
            {
                var target =
                    GameObjects.EnemyHeroes.Where(
                        t =>
                        !Invulnerable.Check(t) && t.IsValidTarget(Vars.R.Range)
                        && Vars.Menu["spells"]["r"]["whitelist"][t.ChampionName.ToLower()].GetValue<MenuBool>().Value)
                        .OrderBy(o => o.Health)
                        .FirstOrDefault();
                if (target != null)
                {
                    Vars.R.Cast();
                    Vars.R.CastOnUnit(target);
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
                    if (Invulnerable.Check(GameObjects.Player, DamageType.True, false))
                    {
                        return;
                    }

                    /// <summary>
                    ///     Block Gangplank's Barrels.
                    /// </summary>
                    var hero = sender as AIHeroClient;
                    if (hero != null && hero.ChampionName.Equals("Gangplank"))
                    {
                        if (!(args.Target is Obj_AI_Minion))
                        {
                            return;
                        }

                        if (AutoAttack.IsAutoAttack(args.SData.Name) || args.SData.Name.Equals("GangplankQProceed"))
                        {
                            if (Math.Abs(((Obj_AI_Minion)args.Target).Health - 1) <= 0
                                && ((Obj_AI_Minion)args.Target).DistanceToPlayer() < 450
                                && ((Obj_AI_Minion)args.Target).BaseSkinName.Equals("gangplankbarrel"))
                            {
                                Vars.W.Cast();
                            }
                        }
                    }

                    if (hero != null)
                    {
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

                                    /// <summary>
                                    ///     Whitelist Block.
                                    /// </summary>
                                    if (
                                        Vars.Menu["spells"]["e"]["whitelist"][
                                            $"{hero.ChampionName.ToLower()}.{args.SData.Name.ToLower()}"] == null
                                        || !Vars.Menu["spells"]["e"]["whitelist"][
                                            $"{hero.ChampionName.ToLower()}.{args.SData.Name.ToLower()}"]
                                                .GetValue<MenuBool>().Value)
                                    {
                                        return;
                                    }
                                    if (GameObjects.Player.HasBuff("udyrbearstuncheck")
                                        && hero.ChampionName.Equals("Udyr"))
                                    {
                                        return;
                                    }

                                    Vars.W.Cast();
                                    break;
                                default:
                                    if (!hero.Buffs.Any(b => AutoAttack.IsAutoAttackReset(b.Name))
                                        || Vars.Menu["spells"]["e"]["whitelist"][
                                            $"{hero.ChampionName.ToLower()}.{hero.Buffs.First(b => AutoAttack.IsAutoAttackReset(b.Name)).Name.ToLower()}"
                                               ] == null
                                        || !Vars.Menu["spells"]["e"]["whitelist"][
                                            $"{hero.ChampionName.ToLower()}.{hero.Buffs.First(b => AutoAttack.IsAutoAttackReset(b.Name)).Name.ToLower()}"
                                                ].GetValue<MenuBool>().Value)
                                    {
                                        return;
                                    }

                                    Vars.W.Cast();
                                    break;
                            }
                        }

                        /// <summary>
                        ///     Shield all the Targetted Spells.
                        /// </summary>
                        else if (SpellDatabase.GetByName(args.SData.Name) != null)
                        {
                            /// <summary>
                            ///     Whitelist Block.
                            /// </summary>
                            if (
                                Vars.Menu["spells"]["e"]["whitelist"][
                                    $"{hero.ChampionName.ToLower()}.{args.SData.Name.ToLower()}"] == null
                                || !Vars.Menu["spells"]["e"]["whitelist"][
                                    $"{hero.ChampionName.ToLower()}.{args.SData.Name.ToLower()}"].GetValue<MenuBool>()
                                        .Value)
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
                                    if (!args.Target.IsMe
                                        || !SpellDatabase.GetByName(args.SData.Name)
                                                .CastType.Contains(CastType.EnemyChampions))
                                    {
                                        return;
                                    }

                                    switch (hero.ChampionName)
                                    {
                                        case "Caitlyn":
                                            DelayAction.Add(1050, () => { Vars.W.Cast(); });
                                            break;
                                        case "Nocturne":
                                            DelayAction.Add(350, () => { Vars.W.Cast(); });
                                            break;
                                        case "Zed":
                                            DelayAction.Add(200, () => { Vars.W.Cast(); });
                                            break;
                                        default:
                                            DelayAction.Add(
                                                Vars.Menu["spells"]["e"]["delay"].GetValue<MenuSlider>().Value,
                                                () => { Vars.W.Cast(); });
                                            break;
                                    }

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
                                                Vars.W.Cast();
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
                    if (Vars.Menu["spells"]["e"]["whitelist"]["minions"].GetValue<MenuBool>().Value)
                    {
                        if (sender.BaseSkinName.Equals("SRU_Baron")
                            || sender.BaseSkinName.Contains("SRU_Dragon")
                            || sender.BaseSkinName.Equals("SRU_RiftHerald"))
                        {
                            Vars.W.Cast();
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}