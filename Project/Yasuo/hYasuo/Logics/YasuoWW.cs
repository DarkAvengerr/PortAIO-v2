using hYasuo.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hYasuo.Logics
{
    internal static class YasuoWW
    {
        public static void YasuoWindWallProtector(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient
                && args.End.Distance(ObjectManager.Player.Position) < 400f && Menus.Config.Item("w.protect."+args.SData.Name) != null
                && Menus.Config.Item("w.protect." + args.SData.Name).GetValue<bool>()
                && Spells.W.IsReady())
            {
                Spells.W.Cast(ObjectManager.Player.Position.Extend(args.Start, Spells.W.Range));
            }

            if (sender is AIHeroClient && sender.IsEnemy && args.SData.TargettingType == SpellDataTargetType.Unit
                && sender.Type == GameObjectType.AIHeroClient
                && args.End.Distance(ObjectManager.Player.Position) < 400f && Menus.Config.Item("windwall.targetted." + args.SData.Name) != null
                && Menus.Config.Item("windwall.targetted." + args.SData.Name).GetValue<bool>()
                && Spells.W.IsReady())
            {
                Spells.W.Cast(ObjectManager.Player.Position.Extend(args.Start, Spells.W.Range));
            }

        }

        public static void YasuoTargettedProtector(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsEnemy && args.SData.TargettingType == SpellDataTargetType.Unit
                && sender.Type == GameObjectType.AIHeroClient && args.End.Distance(ObjectManager.Player.Position) < 400f 
                && Menus.Config.Item("q3.target." + args.SData.Name) != null
                && Menus.Config.Item("q3.target." + args.SData.Name).GetValue<bool>()
                && Spells.Q3.IsReady() && Spells.Q.Empowered())
            {
                Spells.Q3.Cast(ObjectManager.Player.Position.Extend(args.End , 500f));
            }
        }
    }
}
