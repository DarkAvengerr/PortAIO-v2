using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheTwitch.Commons;
using TheTwitch.Commons.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch
{
    class TwitchQ : Skill
    {
        public Circle DrawRange;
        public MenuItem StealthRecall;
        private float _stealthRecallTime;

        public TwitchQ(SpellSlot spell)
            : base(spell)
        {
            OnlyUpdateIfTargetValid = false;
            HarassEnabled = false;
            ComboEnabled = false;
            Spellbook.OnCastSpell += OnCastSpell;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        void Drawing_OnEndScene(EventArgs args)
        {
            if (DrawRange.Active)
#pragma warning disable 618
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, GetRemainingTime() * ObjectManager.Player.MoveSpeed, DrawRange.Color, 1, onMinimap: true);
#pragma warning restore 618
        }

        private void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && ObjectManager.Player.GetSpell(args.Slot).Name == "recall" && StealthRecall.GetValue<KeyBind>().Active && _stealthRecallTime + 5 < Game.Time)
            {
                args.Process = ObjectManager.Player.InFountain();
            }
        }

        public override void Update(Orbwalking.OrbwalkingMode mode, ComboProvider combo, AIHeroClient target)
        {
            if (StealthRecall.GetValue<KeyBind>().Active && _stealthRecallTime + 5 < Game.Time && !ObjectManager.Player.InFountain())
            {
                _stealthRecallTime = Game.Time;
                Cast();
                ObjectManager.Player.Spellbook.CastSpell(ObjectManager.Player.Spellbook.Spells.First(spell => spell.Name == "recall").Slot);
            }
            base.Update(mode, combo, target);
        }

        public override void Execute(AIHeroClient target)
        {
            Cast();
        }

        public override void Draw()
        {

            if (!DrawRange.Active) return;
            var stealthTime = GetRemainingTime();
            if (stealthTime > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, stealthTime * ObjectManager.Player.MoveSpeed, DrawRange.Color);

            }
        }

        private float GetRemainingTime()
        {
            var buff = ObjectManager.Player.GetBuff("TwitchHideInShadows");
            if (buff == null && Instance.State == SpellState.Ready) return Level + 3 + 1.5f + 1f;
            if (buff == null) return 0;
            return buff.EndTime - Game.Time;
        }

        public override int GetPriority()
        {
            return 2;
        }
    }
}
