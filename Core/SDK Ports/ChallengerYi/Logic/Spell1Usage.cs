using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallengerYi.Backbone.Menu;
using ChallengerYi.Backbone.Utils;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Logic
{
    internal class Spell1Usage
    {
        private static Spell Q = new Spell(SpellSlot.Q, 600);

        internal Spell1Usage()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Q.IsReady())
            {
                return;
            }
            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
            {
                var target = Variables.TargetSelector.GetTarget(600, DamageType.Physical);
                if (target != null)
                {
                    if (Spell1Menu.QCombo.SelectedValue == "ALWAYS" || ObjectManager.Player.IsUnderEnemyTurret() || target.CharData.BaseSkinName == "Jinx" || target.CharData.BaseSkinName == "Jhin")
                    {
                        Q.Cast(target);
                    }
                    else
                    {
                        if (target.Health < Q.GetDamage(target) + ObjectManager.Player.GetAutoAttackDamage(target))
                        {
                            Q.Cast(target);
                        }
                        var championData =
                            GameData.Champions.FirstOrDefault(
                                champ => champ.ChampionName == target.CharData.BaseSkinName);
                        if (championData != null)
                        {
                            var gapclosers =
                                target.Spellbook.Spells.Where(
                                    spell => championData.Gapclosers.Any(slot => spell.Slot == slot));
                            if (gapclosers != null && gapclosers.Any())
                            {
                                if (gapclosers.Any(s => s.IsReady()))
                                {
                                    return;
                                }
                                // no gapclosers? no problem!
                                Q.Cast(target);
                            }
                        }
                        else
                        {
                            if (!ObjectManager.Player.HasBuff("Highlander"))
                            {
                                Q.Cast(target);
                            }
                        }
                    }
                }
            }
            var minion =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(m => m.IsHPBarRendered && m.Distance(ObjectManager.Player) < 600 && m.IsTargetable);
            if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear && minion != null &&
                minion.Health > Q.GetDamage(minion))
            {
                if (ObjectManager.Player.ManaPercent > Spell1Menu.QFarmMana)
                {
                    if (Spell1Menu.QFarm.SelectedValue == "ALWAYS")
                    {
                        Q.Cast(minion);
                    }
                    else
                    {
                        if (
                            !GameObjects.EnemyHeroes.Any(
                                enemy => enemy.IsHPBarRendered && enemy.Distance(ObjectManager.Player) < 1250))
                        {
                            Q.Cast(minion);
                        }
                    }
                }
                if ((minion.CharData.BaseSkinName.Contains("SRU") || minion.CharData.BaseSkinName.Contains("TT")) &&
                    !minion.CharData.BaseSkinName.ToLower().Contains("minion"))
                {
                    Q.Cast(minion);
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base caster, GameObjectProcessSpellCastEventArgs args)
        {
            if (Q.IsReady() && caster.IsEnemy)
            {
                if (caster is AIHeroClient && caster.Distance(ObjectManager.Player) < 600)
                {
                    if (Damage.GetSpellDamage((AIHeroClient) caster, ObjectManager.Player, SpellSlot.Q)/
                        ObjectManager.Player.Health*100 > Spell1Menu.QHighDamageSpell.Value)
                    {
                        Q.Cast(caster);
                    }
                    var champData = GameData.Champions.FirstOrDefault(
                        champion =>
                            champion.ChampionName == caster.CharData.BaseSkinName);
                    if (champData != null)
                    {
                        if (champData.Crowdcontrol.Any(slot => slot == args.Slot) &&
                            (args.Target.IsMe || caster.Distance(ObjectManager.Player) < 400))
                        {
                            Q.Cast(caster);
                        }
                        if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo &&
                            champData.Gapclosers.Any(slot => slot == args.Slot))
                        {
                            Q.Cast(caster);
                        }
                    }
                }
            }
        }
    }
}