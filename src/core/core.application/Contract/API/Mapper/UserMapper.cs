using core.application.Contract.API.DTO.Party.User;
using core.domain.entity.enums;
using core.domain.entity.structureModels;
using IdentityProvider.Infrastructure.Services.Persistence;
using System.Security.Claims;

namespace core.application.Contract.API.Mapper;

public static class UserMapper
{
    public static UserGetResponseDTO MapUserModelToUserGetResponse(this UserModel value)
    {

        return new()
        {
            Address = value.Address,
            Age = value.Age,
            Email = value.Email,
            FirstName = value.FirstName,
            Gender = value.Gender,
            Id = value.Id,
            LastName = value.LastName,
            NationalID = value.NationalID,
            PhoneNumber = value.PhoneNumber
            
        };
    }

    public static UserModel MapUserCreateRequestToUserModel(this UserCreateRequest value)
    {
        return new()
        {
            Address = value.Address,
            Age = value.Age,
            Email = value.Email,
            FirstName = value.FirstName,
            Gender = value.Gender,
            LastName = value.LastName,
            NationalID = value.NationalID,
            PhoneNumber = value.PhoneNumber
        };
    }

    public static UserModel MapUserUpdateDTOToUserModel(this UserUpdateRequest value)
    {
        return new()
        {
            Address = value.Address,
            Age = value.Age,
            Email = value.Email,
            FirstName = value.FirstName,
            Gender = value.Gender,
            Id = value.Id,
            LastName = value.LastName,
            NationalID = value.NationalID,
            PhoneNumber = value.PhoneNumber
        };
    }
}
