using AutoMapper;
using MimeKit;
using Tardigrade.Framework.Emails;

namespace Tardigrade.Framework.MailKit.Profiles
{
    /// <summary>
    /// AutoMapper profile for MailKit model objects.
    /// </summary>
    public class MailKitProfile : Profile
    {
        /// <summary>
        /// Create an instance of this profile and define the appropriate mappings.
        /// </summary>
        public MailKitProfile()
        {
            CreateMap<EmailAddress, MailboxAddress>().ReverseMap();
        }
    }
}