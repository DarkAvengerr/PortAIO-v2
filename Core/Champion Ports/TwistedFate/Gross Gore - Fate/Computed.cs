#region Use
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using System.Collections.Generic;
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{
    internal static class Computed

    {
        #region Properties

        internal static bool HasBlue { get { return ObjectManager.Player.HasBuff("bluecardpreattack"); } }
        internal static bool HasRed { get { return ObjectManager.Player.HasBuff("redcardpreattack"); } }
        internal static bool HasGold { get { return ObjectManager.Player.HasBuff("goldcardpreattack"); } }
        internal static string HasACard
        {
            get
            {
                if (ObjectManager.Player.HasBuff("bluecardpreattack"))
                    return "blue";
                if (ObjectManager.Player.HasBuff("goldcardpreattack"))
                    return "gold";
                if (ObjectManager.Player.HasBuff("redcardpreattack"))
                    return "red";
                return "empty";
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void InterruptableSpell_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Config.UseInterrupter)
            {
                var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;

                if (sender.IsValidTarget(Spells._w.Range))
                {
                    switch(CardSelector.Status)
                    {
                        case SelectStatus.Selecting:
                        {
                            CardSelector.JumpToCard(Cards.Yellow);
                            break;
                        }
                        case SelectStatus.Ready:
                        {
                            if(ObjectManager.Player.ManaPercent >= wMana)
                            {
                                CardSelector.StartSelecting(Cards.Yellow);
                            }
                            break;
                        }
                    }

                    if(HasGold)
                    {
                        if (Orbwalking.InAutoAttackRange(sender))
                        {
                            if (Orbwalking.CanAttack())
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                            }
                        }
                    }
                }
            }
        }

        public static void Gapcloser_OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Config.UseAntiGapCloser)
            {
                var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;

                if (gapcloser.Sender.IsValidTarget(Spells._w.Range))
                {
                    switch(CardSelector.Status)
                    {
                        case SelectStatus.Selecting:
                        {
                            CardSelector.JumpToCard(Cards.Yellow);
                            break;
                        }
                        case SelectStatus.Ready:
                        {
                            if (ObjectManager.Player.ManaPercent >= wMana)
                            {
                                CardSelector.StartSelecting(Cards.Yellow);
                            }
                            break;
                        }
                    }

                    if (HasGold)
                    {
                        if (Orbwalking.InAutoAttackRange(gapcloser.Sender))
                        {
                            if (Orbwalking.CanAttack())
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
                            }
                        }
                    }
                }
            }
        }

        public static void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if(Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if(CardSelector.Status == SelectStatus.Selecting 
                    || CardSelector.Status == SelectStatus.Ready)
                {
                    args.Process = false;
                }

            }else if(Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (CardSelector.Status == SelectStatus.Selecting)
                {
                    args.Process = false;
                }
            }

            if(Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if(HasACard != "empty")
                {
                    if(!HeroManager.Enemies.Contains(args.Target))
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(e => !e.IsDead))
                        {
                            if (enemy.IsValidTarget(Spells._q.Range))
                            {
                                if((ObjectManager.Player.Distance(enemy) <= Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100))
                                {
                                    args.Process = false;

                                    if(Orbwalking.InAutoAttackRange(enemy))
                                    {
                                        if (Orbwalking.CanAttack())
                                        {
                                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, enemy);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if(Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if(HasACard != "none" && HasRed)
                {
                    args.Process = false;

                    IDictionary<Obj_AI_Minion, int> creeps = new Dictionary<Obj_AI_Minion, int>();

                    foreach (var x in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Team != ObjectManager.Player.Team && x.Team != GameObjectTeam.Neutral && Orbwalking.InAutoAttackRange(x)))
                    {
                        creeps.Add(x, ObjectManager.Get<Obj_AI_Minion>().Count(y => y.Team != ObjectManager.Player.Team && y.Team != GameObjectTeam.Neutral && y.IsValidTarget() && y.Distance(x.Position) <= 300));
                    }

                    foreach (var x in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Team == GameObjectTeam.Neutral && Orbwalking.InAutoAttackRange(x)))
                    {
                        creeps.Add(x, ObjectManager.Get<Obj_AI_Minion>().Count(y => y.Team == GameObjectTeam.Neutral && y.IsValidTarget() && y.Distance(x.Position) <= 300));
                    }

                    var sbire = creeps.OrderByDescending(x => x.Value).FirstOrDefault();

                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sbire.Key);
                }
                else if(args.Target is Obj_AI_Turret)
                {
                    if(ObjectManager.Player.CountEnemiesInRange(900) == 0)
                    {
                        if(Spells._w.IsReadyPerfectly() && CardSelector.Status == SelectStatus.Ready)
                        {
                            CardSelector.StartSelecting(Cards.Blue);

                            if (CardSelector.Status == SelectStatus.Selected && Orbwalking.InAutoAttackRange(args.Target))
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, args.Target);
                            }
                        }
                    }
                }
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if(ObjectManager.Player.IsDead)
            {
                return;
            }

            if (sender != null)
            {
                if (sender.IsMe)
                {
                    if (args.Slot == SpellSlot.R)
                    {
                        if (args.SData.Name.ToLowerInvariant() == "gate")
                        {
                            switch(CardSelector.Status)
                            {
                                case SelectStatus.Selecting:
                                {
                                    CardSelector.JumpToCard(Cards.Yellow);
                                    break;
                                }
                                case SelectStatus.Ready:
                                {
                                    CardSelector.StartSelecting(Cards.Yellow);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void YellowIntoQ(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Config.PredictQ || ObjectManager.Player.IsDead
                || (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                && Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed))
            {
                return;
            }

            if (sender.IsMe)
            {
                if (args.SData.Name.ToLower() == "goldcardpreattack")
                {
                    if (Spells._q.IsReadyPerfectly())
                    {
                        if (ObjectManager.Player.ManaPercent >= Config.AutoqMana)
                        {
                            foreach (var enemy in HeroManager.Enemies.Where(e => !e.IsDead))
                            {
                                if (!enemy.IsKillableAndValidTarget(Spells._w.GetDamage(enemy), Spells._w.DamageType, Spells._q.Range))
                                {
                                    if (enemy.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 50))
                                    {
                                        Pred.CastSebbyPredict(Spells._q, enemy, Spells._q.MinHitChance);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void RedIntoQ(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Config.PredictQ || ObjectManager.Player.IsDead
                || (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                && Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed))
            {
                return;
            }

            if (sender.IsMe)
            {
                if (args.SData.Name.ToLower() == "redcardpreattack")
                {
                    if (Spells._q.IsReadyPerfectly())
                    {
                        if (ObjectManager.Player.ManaPercent >= Config.AutoqMana)
                        {
                            foreach (var enemy in HeroManager.Enemies.Where(e => !e.IsDead))
                            {
                                if (!enemy.IsKillableAndValidTarget(Spells._w.GetDamage(enemy), Spells._w.DamageType, Spells._q.Range))
                                {
                                    if (enemy.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 50))
                                    {
                                        Pred.CastSebbyPredict(Spells._q, enemy, Spells._q.MinHitChance);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (!ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                damage += (float)ObjectManager.Player.GetAutoAttackDamage(enemy, true);
            }

            if (Spells._q.IsReadyPerfectly())
            {
                damage += Spells._q.GetDamage(enemy);
            }

            if (Spells._w.IsReadyPerfectly())
            {
                damage += Spells._w.GetDamage(enemy, 3);
            }

            if (ObjectManager.Player.HasBuff("cardmasterstackparticle"))
            {
                damage += Spells._e.GetDamage(enemy, 1);
            }

            /*
             * Luden
             * */
            if (Items.HasItem(ItemData.Ludens_Echo.Id))
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, 100 + ObjectManager.Player.FlatMagicDamageMod * 0.1);
            /*
            * Sheen
            * */
            if (Items.HasItem(ItemData.Sheen.Id))
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical, 0.5 * ObjectManager.Player.BaseAttackDamage);
            /*
            * Lich
            * */
            if (Items.HasItem(ItemData.Lich_Bane.Id))
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, 0.5 * ObjectManager.Player.FlatMagicDamageMod + 0.75 * ObjectManager.Player.BaseAttackDamage);
            /*
            * IB Gauntlet
            * */
            if (Items.HasItem(ItemData.Iceborn_Gauntlet.Id))
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, 1.25 * ObjectManager.Player.BaseAttackDamage);
            /*
            * Trinity Force
            * */
            if (Items.HasItem(ItemData.Trinity_Force.Id))
                damage += (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, 2 * ObjectManager.Player.BaseAttackDamage);

            return (float)damage;
        }

        #endregion
    }
}