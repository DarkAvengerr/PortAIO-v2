#region

using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using Swiftly_Teemo.Menu;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Swiftly_Teemo.Handler
{
    internal class Mode : Core
    {
        public static void Combo()
        {
            var targets = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.R.Range)).ToList();

            foreach (var target in targets)
            {
                if (MenuConfig.TowerCheck && target.IsUnderEnemyTurret() || !target.IsFacing(Player)) return;

                var ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;
                var rPrediction = Spells.R.GetPrediction(target).CastPosition;
              //  var newPos = Player.ServerPosition.Extend(rPrediction, Spells.R.Range);

                if (Spells.R.IsReady())
                {
                    if (target.Distance(Player) <= Spells.R.Range)
                    {
                        Spells.R.Cast(rPrediction);
                    }
                }

                if (Spells.W.IsReady() && Player.ManaPercent >= 22.5)
                {
                    Spells.W.Cast();
                }
            }
        }
       
        public static void Jungle()
        {
            var mob = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.R.Range) && !GameObjects.JungleSmall.Contains(m)).ToList();
            var ammo = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo;

            foreach (var m in from m in mob where Spells.R.IsReady() &&
                              m.Health > Spells.R.GetDamage(m)*2 where
                              !m.BaseSkinName.Contains("Sru_Crab") where
                              ammo >= 3 select m)
            {
                Spells.R.Cast(m);
            }
        }

        public static void Flee()
        {
            if (!MenuConfig.Flee.Active)
            {
                return;
            }

            var targets = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.R.Range)).ToList();

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Spells.W.IsReady())
            {
                Spells.W.Cast();
            }

            if (!Spells.R.IsReady()) return;

            foreach (var target in targets)
            {
                if(target.Distance(Player) > Spells.R.Range) return;

                Spells.R.Cast(Player.Position);
            }
        }

        public static void Skin()
        {
        }
    }
}
