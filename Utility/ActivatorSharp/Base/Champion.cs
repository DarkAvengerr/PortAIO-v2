#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Base/Champion.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System.Collections.Generic;
using LeagueSharp;

using EloBuddy; namespace Activator.Base
{
    public class Champion
    {
        public float TowerDamage;
        public float IncomeDamage;
        public float MinionDamage;

        public AIHeroClient Player;
        public Obj_AI_Base Attacker;

        public bool ForceQSS;
        public bool Immunity;
        public bool WalkedInTroy;
        public bool HasRecentAura;
        public string LastDebuff;

        public int DotTicks;
        public int TroyTicks;
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

        public List<HitType> HitTypes = new List<HitType>();
        public Champion(AIHeroClient player, float incdmg)
        {
            Player = player;
            IncomeDamage = incdmg;
        }
    }
}
