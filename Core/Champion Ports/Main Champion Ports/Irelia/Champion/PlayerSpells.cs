using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Common;
using SharpDX;
using EloBuddy;

namespace Irelia.Champion
{
    public static class PlayerSpells
    {
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q, W, E, R;

        public static int LastAutoAttackTick;

        public static int LastQCastTick;

        public static int LastECastTick;

        public static int LastSpellCastTick;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 660f);
            Q.SetTargetted(0f, 2200);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E, 325);
            E.SetSkillshot(0.15f, 75f, 1500f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 1000f);
            R.SetSkillshot(0.15f, 120f, 1600f, false, SkillshotType.SkillshotLine);

            SpellList.AddRange(new[] { Q, W, E, R });

            Game.OnUpdate += GameOnOnUpdate;
            Obj_AI_Base.OnSpellCast += Game_OnProcessSpell;
        }

        public static void Game_OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!unit.IsMe)
            {
                return;
            }
            if (spell.SData.Name.Contains("summoner"))
            {
                return;
            }
            //Chat.Print(spell.SData.Name);
            if (spell.SData.Name.ToLower().Contains("attack"))
            {
                LastAutoAttackTick = Environment.TickCount;
                
            }

            //if (!spell.SData.Name.ToLower().Contains("attack"))
            //{
            //    LastSpellCastTick = Environment.TickCount;
            //    Orbwalking.ResetAutoAttackTimer();
            //}


            switch (spell.Slot)
            {
                case SpellSlot.Q:
                {
                    LastQCastTick = Environment.TickCount;
                    LastSpellCastTick = Environment.TickCount;
                    Orbwalking.ResetAutoAttackTimer();
                    break;
                }

                case SpellSlot.E:
                {
                    LastECastTick = Environment.TickCount;
                    LastSpellCastTick = Environment.TickCount;
                    Orbwalking.ResetAutoAttackTimer();
                    break;
                }

                case SpellSlot.R:
                {
                    LastQCastTick = Environment.TickCount;
                    LastSpellCastTick = Environment.TickCount;
                    Orbwalking.ResetAutoAttackTimer();
                    break;
                }
            }
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (Modes.ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                if (Modes.ModeSettings.MenuSettingE.Item("Settings.E.Auto").GetValue<StringList>().SelectedIndex == 1)
                {
                    var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget())
                    {
                        CastECombo(t);
                    }
                }
            }
        }

        public static void CastQObjects(Obj_AI_Base t)
        {
            if (!Q.CanCast(t))
            {
                return;
            }

            if (Environment.TickCount - LastQCastTick >= (Modes.ModeSettings.MenuSettingQ.Item("Settings.Q.CastDelay").GetValue<StringList>().SelectedIndex + 1) * 250)
            {
                Q.CastOnUnit(t);
            }
        }

        public static void CastQCombo(Obj_AI_Base t)
        {
            //if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            //{
            //    Chat.Print("Shen Active!");
            //    return;
            //}

            if (!Q.CanCast(t))
            {
                return;
            }

            if (Modes.ModeCombo.MenuLocal.Item("Combo.Mode").GetValue<StringList>().SelectedIndex == 1 && LastAutoAttackTick < LastSpellCastTick && t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            {
                return;
            }

            if (Environment.TickCount - LastQCastTick < (Modes.ModeSettings.MenuSettingQ.Item("Settings.Q.CastDelay").GetValue<StringList>().SelectedIndex + 1)* 250)
            {
                return;
            }

            foreach (var enemy in Common.AutoBushHelper.EnemyInfo.Where(
                x =>
                    Q.CanCast(x.Player) 
                    && Environment.TickCount - x.LastSeenForE >= (Modes.ModeSettings.MenuSettingQ.Item("Settings.Q.VisibleDelay").GetValue<StringList>().SelectedIndex + 1) * 250 
                    && x.Player.NetworkId == t.NetworkId).Select(x => x.Player).Where(enemy => enemy != null))
            {
                Q.CastOnUnit(t);
                LastQCastTick = Environment.TickCount;
            }
        }

        public static void CastECombo(Obj_AI_Base t)
        {
            //if (!Common.CommonHelper.ShouldCastSpell(CommonTargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65)))
            //{
            //    Chat.Print("Shen Active!");
            //    return;
            //}

            if (!E.CanCast(t))
            {
                return;
            }

            foreach (var enemy in Common.AutoBushHelper.EnemyInfo.Where(
                x =>
                    E.CanCast(x.Player) &&
                    Environment.TickCount - x.LastSeenForE >= (Modes.ModeSettings.MenuSettingE.Item("Settings.E.VisibleDelay").GetValue<StringList>().SelectedIndex + 1) * 250 &&
                    x.Player.NetworkId == t.NetworkId).Select(x => x.Player))
            {
                if (enemy != null)
                {
                    E.CastOnUnit(t);
                    LastECastTick = Environment.TickCount;
                }
            }
        }
    }
}
