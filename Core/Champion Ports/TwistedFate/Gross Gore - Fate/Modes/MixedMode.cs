#region Use
using System;
using LeagueSharp;
using LeagueSharp.Common;

#endregion
using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using Config = GrossGoreTwistedFate.Config;

    internal static class MixedMode
    {
        #region Methods

        internal static void Execute()
        {
            var wName = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name;
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
            //CardsLock
            var blueLock = wName.Equals("BlueCardLock", StringComparison.InvariantCultureIgnoreCase);
            var redLock = wName.Equals("RedCardLock", StringComparison.InvariantCultureIgnoreCase);
            var goldLock = wName.Equals("GoldCardLock", StringComparison.InvariantCultureIgnoreCase);

            if (!Config.Rotate)
            {
                return;
            }

            foreach (var enemy in HeroManager.Enemies)
            {
                if (!enemy.IsDead && enemy != null)
                {
                    if(enemy.IsValidTarget(Spells._q.Range))
                    {
                        if (Spells._w.IsReadyPerfectly())
                        {
                            if (enemy.Distance(ObjectManager.Player) <= (ObjectManager.Player.AttackRange + Config.RotateRange))
                            {
                                if (ObjectManager.Player.ManaPercent >= Config.RotateMana)
                                {
                                    CardSelector.RotateCards();
                                }
                            }
                        }

                        if (enemy.Distance(ObjectManager.Player) <= (ObjectManager.Player.AttackRange + Config.RotateLock))
                        {
                            if(CardSelector.Status == SelectStatus.Selecting)
                            {
                                switch (Config.Prioritize)
                                {
                                    case 0:
                                    {
                                        //Smart (W->Q)
                                        if (ObjectManager.Player.Mana - wMana - qMana >= wMana )
                                        {
                                            if(Spells._q.IsReadyPerfectly())
                                            {
                                                if (blueLock)
                                                {
                                                    CardSelector.JumpToCard(Cards.Red);

                                                }
                                                else if (goldLock)
                                                {
                                                    CardSelector.LockCard();
                                                }
                                                else if (redLock)
                                                {
                                                    CardSelector.LockCard();
                                                }
                                            }

                                            CardSelector.LockCard();
                                        }

                                        CardSelector.LockCard();

                                        return;
                                    }
                                    case 1:
                                    {
                                        //Prioritize BLUE-GOLD-RED;
                                        if (blueLock)
                                        {
                                            CardSelector.LockCard();

                                        }
                                        else if (goldLock)
                                        {
                                            CardSelector.JumpToCard(Cards.Blue);
                                        }
                                        else if (redLock)
                                        {
                                            CardSelector.LockCard();
                                        }
                                        return;
                                    }
                                    case 2:
                                    {
                                        //Prioritize RED-BLUE-GOLD;
                                        if (blueLock)
                                        {
                                            CardSelector.LockCard();

                                        }
                                        else if (goldLock)
                                        {
                                            CardSelector.JumpToCard(Cards.Red);
                                        }
                                        else if (redLock)
                                        {
                                            CardSelector.LockCard();
                                        }
                                        return;
                                    }
                                    case 3:
                                    {
                                        //Prioritize GOLD-BLUE-RED;
                                        if (blueLock)
                                        {
                                            CardSelector.LockCard();

                                        }
                                        else if (goldLock)
                                        {
                                            CardSelector.LockCard();
                                        }
                                        else if (redLock)
                                        {
                                            CardSelector.JumpToCard(Cards.Yellow);
                                        }
                                        return;
                                    }
                                    case 4:
                                    {
                                        //Prioritize GOLD-RED-BLUE;
                                        if (blueLock)
                                        {
                                            CardSelector.JumpToCard(Cards.Red);

                                        }
                                        else if (goldLock)
                                        {
                                            CardSelector.LockCard();
                                        }
                                        else if (redLock)
                                        {
                                            CardSelector.LockCard();
                                        }
                                        return;
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