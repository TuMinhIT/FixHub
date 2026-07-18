using AutoMapper;
using BookStore.Application.Features.Auth.Commands.Login;
using BookStore.Application.Features.Auth.Commands.Register;
using BookStore.Application.Features.Users.Queries.GetAllUsers;
using BookStore.Domain.Entities;

namespace BookStore.Application.Common.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, RegisterResponse>();
        CreateMap<User, UserResponse>();
        CreateMap<User, UserLoginResponse>();
        CreateMap<RegisterCommand, User>();
    }
}