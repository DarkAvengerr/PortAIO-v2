using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using System.Linq;

    internal class GravesDoCast : Graves //need to test
    {
        internal static void Init(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target is AIHeroClient && Menu["Key"]["Combo"].GetValue<MenuKeyBind>().Active)
            {
                var t = args.Target as AIHeroClient;

                if (E.IsReady() && t.DistanceToPlayer() <= E.Range)
                {
                    if (Menu["E"]["Combo"].GetValue<MenuBool>() && E.IsReady())
                    {
                        if (CanCaseE(t, Game.CursorPos))
                        {
                            E.Cast(Game.CursorPos);
                            Variables.Orbwalker.ResetSwingTimer();
                        }
                    }
                }
            }

            if (args.Target is Obj_AI_Minion && Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active)
            {
                var Mobs = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.IsValidTarget(E.Range)).ToList();

                if (Mobs.Count() > 0 && Menu["E"]["Jungle"].GetValue<MenuBool>() && Me.ManaPercent >= Menu["E"]["JungleMana"].GetValue<MenuSlider>().Value && E.IsReady())
                {
                    if (CanCaseE(Mobs[0], Game.CursorPos) && Me.Spellbook.IsAutoAttacking && !Me.Spellbook.IsCastingSpell)
                    {
                        E.Cast(Game.CursorPos);
                        Variables.Orbwalker.ResetSwingTimer();
                    }
                }
            }
        }
    }
}