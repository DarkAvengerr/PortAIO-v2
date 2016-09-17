using System.ComponentModel;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloDZLib.Logging
{
    /// <summary>
    /// The Log Severity enum
    /// </summary>
    [DefaultValue(Medium)]
    enum LogSeverity
    {
        Warning, Error, Low, Medium, Severe
    }
}
