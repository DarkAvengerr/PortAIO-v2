using EloBuddy; 
using LeagueSharp.Common; 
namespace myAurelionSol.Manager.Events
{
    using System;
    using LeagueSharp;

    internal class CreateManager : Logic
    {
        internal static void Init(GameObject sender, EventArgs Args)
        {
            var SpellQ = sender as MissileClient;

            if (SpellQ != null)
            {
                if (SpellQ.IsValid && SpellQ.SpellCaster.IsMe && SpellQ.SpellCaster.IsValid &&
                    SpellQ.SData.Name.Contains("AurelionSolQMissile"))
                {
                    qMillile = SpellQ;
                }
            }
        }
    }
}