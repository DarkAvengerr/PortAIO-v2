using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace SergixBrand
{
    public class Damage
    {
        private System.Drawing.Color _color;

        private string _id;
        private float _damageFloat;
        private LeagueSharp.Common.Spell spell;
        public LeagueSharp.Common.Spell getSpell() => spell;
        public System.Drawing.Color getColor()
        {
            return _color;
        }
        public float getDamageValue(AIHeroClient target)
        {
            return spell.GetDamage(target);
        }
        public Damage(string id, LeagueSharp.Common.Spell spell, System.Drawing.Color color)
        {
            this._color = color;
            this.spell =  spell;
            this._id = id;
        }
    }
}
