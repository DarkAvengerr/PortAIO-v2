using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Menu.Interfaces
{
    using LeagueSharp.Common;

    public interface IMenuSet
    {
        Menu Menu { get; set; }

        void Generate();
    }
}
