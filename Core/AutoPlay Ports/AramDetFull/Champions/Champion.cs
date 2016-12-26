using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace ARAMDetFull.Champions
{
    abstract class Champion
    {
        protected Champion()
        {
            Chat.Print(player.ChampionName+" plugin loaded!");
        }

        public bool safeGap(Obj_AI_Base target)
        {
            return MapControl.safeGap(target);
        }

        public bool safeGap(Vector2 position)
        {
            return MapControl.safeGap(position);
        }

        public static AIHeroClient player = ObjectManager.Player;

        public Spell Q = new Spell(SpellSlot.Q), W = new Spell(SpellSlot.W), E = new Spell(SpellSlot.E), R = new Spell(SpellSlot.R);

        /* Skill Use */
        public abstract void useQ(Obj_AI_Base target);
        public abstract void useW(Obj_AI_Base target);
        public abstract void useE(Obj_AI_Base target);
        public abstract void useR(Obj_AI_Base target);

        public abstract void setUpSpells();

        public abstract void useSpells();

        public virtual void escape(){ }

        public virtual void farm()
        {
            MapControl.myControler.useSpellsOnMinions();
        }
        public virtual void killSteal() { }
        public virtual void alwaysCheck() { }

        public virtual void kiteBack(Vector2 pos) { }
    }
}
