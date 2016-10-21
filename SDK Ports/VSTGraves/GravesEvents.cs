using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    internal class GravesEvents : Graves
    {
        internal static void Init()
        {
            Game.OnUpdate += GravesOnTick.Init;
            Events.OnGapCloser += GravesGapCloser.Init;
            //Variables.Orbwalker.OnAction += GravesAfterAttack.Init;
            Obj_AI_Base.OnSpellCast += GravesDoCast.Init;
            Drawing.OnDraw += GravesOnDraw.Init;
        }
    }
}