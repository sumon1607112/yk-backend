using AutoMapper;
using YK.Application.UseCases.Users;
using YK.Domain.Entities.Users;


namespace YK.Application.Mappings.Users
{
    public class UserProfile: Profile
    {
        public UserProfile() 
        {
            CreateMap<UserRequestDto, User>();
            CreateMap<User, UserResponseDto>();
        }

    }
}
