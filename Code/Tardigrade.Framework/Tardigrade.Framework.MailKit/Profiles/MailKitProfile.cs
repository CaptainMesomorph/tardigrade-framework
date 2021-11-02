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
        // Create an instance of this profile and define the appropriate mappings.
        public MailKitProfile()
        {
            CreateMap<EmailAddress, MailboxAddress>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
                .ReverseMap();
        }
    }
}