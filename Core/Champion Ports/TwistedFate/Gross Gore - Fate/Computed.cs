#region Use
using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common; 
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
                if (sender.IsValidTarget(Spells._w.Range))
                {
                    switch(CardSelector.Status)
                    {
                        case SelectStatus.Selecting:
                        {
                            CardSelector.JumpToCard(Cards.Yellow);
                            return;
                        }
                        case SelectStatus.Ready:
                        {
                            CardSelector.StartSelecting(Cards.Yellow);
                            return;
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
                if (gapcloser.Sender.IsValidTarget(Spells._w.Range))
                {
                    switch (CardSelector.Status)
                    {
                        case SelectStatus.Selecting:
                        {
                            CardSelector.JumpToCard(Cards.Yellow);
                            return;
                        }
                        case SelectStatus.Ready:
                        {
                            CardSelector.StartSelecting(Cards.Yellow);
                            return;
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

            if (args.Target is AIHeroClient)
            {
                args.Process = CardSelector.Status != SelectStatus.Selecting
                               && Environment.TickCount - CardSelector.LastWSent > 300;
            }

            if (CardSelector.Status == SelectStatus.Selecting)
            {
                if(Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                    || Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
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
                        var targetDis = TargetSelector.GetTarget(Spells._q.Range, Spells._q.DamageType);

                        if (targetDis.IsValidTarget(Spells._q.Range))
                        {
                            if(!targetDis.IsZombie)
                            {
                                if((ObjectManager.Player.Distance(targetDis) < Orbwalking.GetAttackRange(ObjectManager.Player) + 175))
                                {
                                    args.Process = false;

                                    var target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), Spells._w.DamageType);

                                    if(target.IsValidTarget(Spells._w.Range))
                                    {
                                        if(!target.IsZombie)
                                        {
                                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                        }
                                    }
                                }
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
                            switch (CardSelector.Status)
                            {
                                case SelectStatus.Selecting:
                                {
                                    CardSelector.JumpToCard(Cards.Yellow);
                                    return;
                                }
                                case SelectStatus.Ready:
                                {
                                    CardSelector.StartSelecting(Cards.Yellow);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void YellowIntoQ(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;

            if (ObjectManager.Player.IsDead || (ObjectManager.Player.ManaPercent - qMana < wMana))
            {
                return;
            }

            if(sender.IsMe)
            {
                if(args.SData.Name.ToLower() == "goldcardpreattack")
                {
                    if(Spells._q.IsReadyPerfectly())
                    {
                        if(ObjectManager.Player.ManaPercent >= Config.AutoqMana)
                        {
                            var canWKill = HeroManager.Enemies.FirstOrDefault(
                                   h => !h.IsDead&& h.IsValidTarget(Spells._q.Range)
                                   && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.W));

                            if (canWKill == null)
                            {
                                var targetDis = TargetSelector.GetTarget(Spells._q.Range, Spells._q.DamageType);

                                if(targetDis != null)
                                {
                                    if(targetDis.IsValidTarget(Spells._q.Range / 2))
                                    {
                                        if(Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                                            || Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                                        {
                                            if (targetDis.IsValidTarget(Spells._q.Range))
                                            {
                                                var qPred = Spells._q.GetPrediction(targetDis);

                                                if (qPred.Hitchance >= HitChance.VeryHigh)
                                                {
                                                    Spells._q.Cast(qPred.CastPosition);
                                                }
                                            }
                                        }
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
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;

            if (ObjectManager.Player.IsDead || (ObjectManager.Player.ManaPercent - qMana < wMana))
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
                            var canWKill = HeroManager.Enemies.FirstOrDefault(
                                   h => !h.IsDead && h.IsValidTarget(Spells._q.Range)
                                   && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.W));

                            if (canWKill == null)
                            {
                                var targetDis = TargetSelector.GetTarget(Spells._q.Range, Spells._q.DamageType);

                                if (targetDis != null)
                                {
                                    if (targetDis.IsValidTarget(Spells._q.Range / 2))
                                    {
                                        if (Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                                            || Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                                        {
                                            if (targetDis.IsValidTarget(Spells._q.Range))
                                            {
                                                var qPred = Spells._q.GetPrediction(targetDis);

                                                if (qPred.Hitchance >= HitChance.VeryHigh)
                                                {
                                                    Spells._q.Cast(qPred.CastPosition);
                                                }
                                            }
                                        }
                                    }
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