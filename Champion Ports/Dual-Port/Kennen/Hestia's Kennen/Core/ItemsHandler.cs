using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
namespace Kennen.Core
{
    internal class ItemsHandler
    {
        public static Items.Item Zhonya = new Items.Item(3157);
        public static Items.Item Wooglet = new Items.Item(3090);
        public static Items.Item ProtoBelt = new Items.Item(3152, 800f);

        public static void UseProtobelt(Obj_AI_Base target)
        {
            if (Configs.config.Item("useProto").GetValue<bool>() && Items.HasItem(ProtoBelt.Id) && Items.CanUseItem(ProtoBelt.Id) && target != null)
            {
                var protoPred = LeagueSharp.Common.Prediction.GetPrediction(target, 0.25f, 100, 2000,
                    new[]
                    {
                        CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.Walls,
                        CollisionableObjects.YasuoWall
                    });
                if (protoPred.Hitchance >= HitChance.Medium)
                {
                    ProtoBelt.Cast(protoPred.CastPosition);
                }
            }
        }


    }
}
