using System.ComponentModel.DataAnnotations;

namespace Tardigrade.Framework.Tests.Models
{
    /// <summary>
    /// Enumeration of locales.
    /// </summary>
    public enum LocaleType
    {
        /// <summary>
        /// Australia.
        /// </summary>
        [Display(Name = "en-AU", Description = "Australia")]
        EnAu = 0,

        /// <summary>
        /// The United Kingdom.
        /// </summary>
        [Display(Name = "en-GB", Description = "United Kingdom")]
        EnGb = 1
    }
}