using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappers;

public class UserMapper : IGenericMapper<UserDto, User>
{
    public User ToEntity(UserDto source)
    {
        return new User
        {
            Id = source.Id,
            Username = source.Username,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email
        };
    }

    public UserDto ToDto(User source)
    {
        return new UserDto
        {
            Id = source.Id,
            Username = source.Username,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email
        };
    }

    public void MapPartial(UserDto source, User destination)
    {
        throw new NotImplementedException();
    }
}