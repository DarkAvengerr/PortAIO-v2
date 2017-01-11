using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Core.Spells
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Library.Geometry.Insec;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal sealed class WSpell : SpellChild
    {
        public override string Name { get; set; } = "Safeguard";

        public override Spell Spell { get; set; }

        public int PassiveStacks => ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic")
                                        ? ObjectManager.Player.GetBuff("blindmonkpassive_cosmetic").Count
                                        : 0;

        public LeeSinHelper Insec;

        public int wardTick;

        public int jumpTick;

        public bool W1 => Spell.Instance.SData.Name.ToLower().Contains("one");

        public bool Trinket => Items.GetWardSlot() != null && Items.GetWardSlot().Stacks >= 1;

    //    private static bool CanUseItemWard => Items.CanUseItem(Item) && Item != 0;

        // Wards, ordered by most common item
        //private static int Item => Items.CanUseItem(3340) && Items.HasItem(3340)
        //                    ? 3340
        //                    : Items.CanUseItem(3711) && Items.HasItem(3711)
        //                    ? 3711
        //                    : Items.CanUseItem(2055) && Items.HasItem(2055)
        //                    ? 2055
        //                    : Items.CanUseItem(2049) && Items.HasItem(2049)
        //                    ? 2049
        //                    : Items.CanUseItem(2045) && Items.HasItem(2045)
        //                    ? 2045
        //                    : Items.CanUseItem(2301) && Items.HasItem(2301)
        //                    ? 2301
        //                    : Items.CanUseItem(2302) && Items.HasItem(2302)
        //                    ? 2302
        //                    : Items.CanUseItem(2303) && Items.HasItem(2303)
        //                    ? 2303
        //                    : 0;

        /// <summary>
        /// Gets what kind of position that should be accounted for in the insec process
        /// </summary>
        /// <param name="target"></param>
        /// <param name="turret"></param>
        /// <param name="ally"></param>
        /// <param name="reverted"></param>
        /// <returns></returns>
        public Vector3 InsecPositioner(Obj_AI_Base target, bool turret = false, bool ally = false)
        {
            var allies = Insec.Ally(1700, target.Position);
            var allyTurret = Insec.AllyTurret(1600, target.Position);

            if (turret && allyTurret != null)
            {
                return InsecPosition(target, allyTurret.Position);
            }

            if (ally && allies != null)
            {
                return InsecPosition(target, allies.Position);
            }

            return InsecPosition(target, ObjectManager.Player.Position);
        }

        /// <summary>
        /// Gets where to jump
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Minions"></param>
        /// <param name="Allies"></param>
        /// <param name="Ward"></param>
        public void Jump(Vector3 Position, bool Minions, bool Allies, bool Ward)
        {
            if (500 > Utils.TickCount - jumpTick || !W1 || !Spell.IsReady())
            {
                return;
            }

            const int Range = 600;

            var ally = Insec.Ally(Range, Position);
            var allyMinion = Insec.Minions(Range, Position);
            var wardObject = Insec.Ward(Range, Position);

            if (Allies && ally != null)
            {
                Cast(ally);
            }

            if (Minions && allyMinion != null)
            {
                Cast(allyMinion);
            }

            if (!Ward)
            {
                return;
            }

            if (wardObject != null)
            {
                Cast(wardObject);
            }
            else if (Trinket)
            {
                ObjectManager.Player.Spellbook.CastSpell(Items.GetWardSlot().SpellSlot, Position);
                wardTick = Utils.TickCount;
            }
        }

        /// <summary>
        /// Casts W To Obj_AI_Base 
        /// </summary>
        /// <param name="target"></param>
        public void Cast(Obj_AI_Base target)
        {
            Spell.CastOnUnit(target);
            jumpTick = Utils.TickCount;
        }

        /// <summary>
        /// Gets best insec position & distance
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Object"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public Vector3 InsecPosition(Obj_AI_Base target, Vector3 Object, bool cursor = false)
        {
            var amount = 190 + target.BoundingRadius; 

           // Console.WriteLine("[DEBUG] Range Behind Target: " + amount);

            var cursorPos = target.Position + (Object - target.Position).Normalized() * amount;

            var pos = target.Position + (target.Position - Object).Normalized() * amount;

            return cursor ? cursorPos : pos;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.W, 700);

            Insec = new LeeSinHelper();
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
