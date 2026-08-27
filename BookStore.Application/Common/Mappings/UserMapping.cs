using AutoMapper;
using FixHub.Application.Features.Auth.Commands.Login;
using FixHub.Application.Features.Auth.Commands.Register;
using FixHub.Application.Features.Users.Commands.UpdateInfo;
using FixHub.Application.Features.Users.Queries.GetAllUsers;
using FixHub.Application.Features.Users.Queries.GetProfile;
using FixHub.Domain.Entities;

namespace FixHub.Application.Common.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, RegisterResponse>();
        CreateMap<User, UserResponse>();
        CreateMap<User, UserLoginResponse>();
        CreateMap<RegisterCommand, User>();
        CreateMap<User, UpdateProfileResponse>();
        CreateMap<User, GetProfileResponse>();
    }
}