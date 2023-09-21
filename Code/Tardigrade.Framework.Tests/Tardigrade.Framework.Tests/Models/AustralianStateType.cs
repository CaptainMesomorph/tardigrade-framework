using System.ComponentModel;

namespace Tardigrade.Framework.Tests.Models
{
    /// <summary>
    /// Enumeration of Australian states.
    /// </summary>
    public enum AustralianStateType
    {
        [Description("Australian Capital Territory")]
        ACT,

        [Description("New South Wales")]
        NSW,

        [Description("Northern Territory")]
        NT,

        [Description("South Australia")]
        SA,

        [Description("Tasmania")]
        TAS,

        [Description("Victoria")]
        VIC,

        [Description("Western Australia")]
        WA
    }
}