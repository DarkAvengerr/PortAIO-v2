#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Base/Champion.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Activator.Items;
using Activator.Spells;
using Activator.Summoners;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Base
{
    public class Champion
    {
        public float BuffDamage;
        public float TroyDamage;
        public float ItemDamage;
        public float TowerDamage;
        public float AbilityDamage;
        public float MinionDamage;

        public AIHeroClient Player;
        public Obj_AI_Base Attacker;

        public bool ForceQSS;

        public int QSSBuffCount;
        public int CleanseBuffCount;
        public int DervishBuffCount;
        public int MercurialBuffCount;
        public int MikaelsBuffCount;

        public int QSSHighestBuffTime;
        public int CleanseHighestBuffTime;
        public int DervishHighestBuffTime;
        public int MercurialHighestBuffTime;
        public int MikaelsHighestBuffTime;
        public int LastDebuffTimestamp;

        public int BuffCount => HitTypes.Count(t => t == HitType.Buff);
        public int TroyCount => HitTypes.Count(t => t == HitType.Troy);
        public int SpellCount => HitTypes.Count(t => t == HitType.Spell) + TroyCount;
        public int DangerCount => HitTypes.Count(t => t == HitType.Danger);
        public int CrowdControlCount => HitTypes.Count(t => t == HitType.CrowdControl);

        public float IncomeDamage
        {
            get { return TroyDamage + AbilityDamage + BuffDamage + ItemDamage; }
        }

        public List<HitType> HitTypes = new List<HitType>();
        public Champion(AIHeroClient player, float incdmg)
        {
            Player = player;
        }
    }

    public struct Priority
    {

        public int ItemId;
        public int Position;
        public object Type;

        public bool Needed()
        {
            var item = Type as CoreItem;
            if (item != null)
                return item.Needed;

            var spell = Type as CoreSpell;
            if (spell != null)
                return spell.Needed;

            var sum = Type as CoreSum;
            if (sum != null)
                return sum.Needed;

            return false;
        }

        public Menu Menu()
        {
            var item = Type as CoreItem;
            if (item != null)
                return item.Menu;

            var spell = Type as CoreSpell;
            if (spell != null)
                return spell.Menu;

            var sum = Type as CoreSum;
            if (sum != null)
                return sum.Menu;

            return null;
        }

        public string Name()
        {
            var item = Type as CoreItem;
            if (item != null)
                return item.Name;

            var spell = Type as CoreSpell;
            if (spell != null)
                return spell.Name;

            var sum = Type as CoreSum;
            if (sum != null)
                return sum.Name;

            return string.Empty;
        }

        public bool Ready()
        {
            var item = Type as CoreItem;
            if (item != null)
                return item.IsReady();

            var spell = Type as CoreSpell;
            if (spell != null)
                return spell.IsReady();

            var sum = Type as CoreSum;
            if (sum != null)
                return sum.IsReady();

            return false;
        }
    }
}
