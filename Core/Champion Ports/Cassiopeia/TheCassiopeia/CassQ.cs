using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TheCassiopeia.Commons;
using TheCassiopeia.Commons.ComboSystem;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace TheCassiopeia
{
    class CassQ : Skill
    {
        public MenuItem StackTear;
        public int MinTearStackMana;
        public int FarmIfHigherThan;
        public int FarmIfMoreOrEqual;
        public bool Farm;
        private CassE _e;
        public MenuItem LanepressureMenu;
        public bool AutoHarass;
        public int AutoHarassMana;
        public bool OnlyQWhenNotPoisoned;

        public CassQ(SpellSlot slot)
            : base(slot)
        {

            Range = Instance.SData.CastRange;
            SetSkillshot(0.75f, Instance.SData.CastRadius - 50, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        //public override void Draw()
        //{
        //    var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.NotAlly).Select(minion => minion.Position.To2D()).ToList(), Instance.SData.CastRadius, 850);
        //    Drawing.DrawText(200, 200, Color.Red, farmLocation.MinionsHit.ToString() + " / " + FarmIfMoreOrEqual);

        //}

        public override void Initialize(ComboProvider combo)
        {
            _e = combo.GetSkill<CassE>();
            base.Initialize(combo);

            float tickLimiter = 0;
            //float mana = 0;
            Game.OnUpdate += (args) =>
            {
                if (tickLimiter > Game.Time) return;
                tickLimiter = Game.Time + 0.25f;

                if (ObjectManager.Player.Spellbook.Spells.Any(spell => spell.Name == "ItemSeraphsEmbrace")) return;
                //if (mana == ObjectManager.Player.MaxMana)
                //{
                //    StackTear.SetValue(new KeyBind(StackTear.GetValue<KeyBind>().Key, KeyBindType.Toggle));
                //    mana = 0f;
                //}


                if (combo.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && CanBeCast() && ObjectManager.Player.CountEnemiesInRange(2000) == 0 && MinTearStackMana < ObjectManager.Player.ManaPercent && !ObjectManager.Player.IsRecalling() && StackTear.IsActive())
                {
                    if (ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.IsAlly && turret.Distance(ObjectManager.Player) < 1000) || ObjectManager.Player.NearFountain(3500))
                    {
                        var tear = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "TearsDummySpell" || spell.Name == "ArchAngelsDummySpell");

                        if (tear != null && tear.CooldownExpires < Game.Time)
                            Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, 500));
                        //mana = ObjectManager.Player.MaxMana;
                    }
                }

            };
        }

        public override void Update(Orbwalking.OrbwalkingMode mode, ComboProvider combo, AIHeroClient target)
        {
            if (AutoHarass && ObjectManager.Player.ManaPercent > AutoHarassMana && target.IsValidTarget(Range))
            {
                Cast(target);
            }

            base.Update(mode, combo, target);
        }



        public override void Lasthit()
        {
            if (!Farm || !_e.CanBeCast() || !LanepressureMenu.IsActive()) return;
            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.NotAlly).Select(minion => minion.Position.To2D()).ToList(), Instance.SData.CastRadius, 850);
            if (farmLocation.MinionsHit >= FarmIfMoreOrEqual && ObjectManager.Player.ManaPercent > FarmIfHigherThan)
            {
                Cast(farmLocation.Position);
            }
        }

        public override void LaneClear()
        {
            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.NotAlly).Select(minion => minion.Position.To2D()).ToList(), Instance.SData.CastRadius, 850);
            if (farmLocation.MinionsHit > 0)
            {
                Cast(farmLocation.Position);
            }
        }


        public bool OnCooldown()
        {
            return Instance.CooldownExpires - Game.Time < Instance.Cooldown - Delay;
        }

        public override void Execute(AIHeroClient target)
        {
            if (_e.CanBeCast() && _e.IsKillable(target))
                return;

            if (!OnlyQWhenNotPoisoned || !target.IsPoisoned())
                Cast(target);
        }

        public override int GetPriority()
        {
            return 2;
        }
    }
}
