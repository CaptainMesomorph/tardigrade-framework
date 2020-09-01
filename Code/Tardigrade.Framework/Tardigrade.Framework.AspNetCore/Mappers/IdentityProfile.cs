using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Tardigrade.Framework.AspNetCore.Mappers
{
    /// <summary>
    /// AutoMapper mapping profile for types associated with the application user identity framework.
    /// </summary>
    public class IdentityProfile : Profile
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        public IdentityProfile()
        {
            CreateMap<IdentityError, Framework.Models.Errors.IdentityError>(MemberList.Source)
                .ConstructUsing(x => new Framework.Models.Errors.IdentityError(x.Code, x.Description))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Description))
                .ReverseMap();
        }
    }
}