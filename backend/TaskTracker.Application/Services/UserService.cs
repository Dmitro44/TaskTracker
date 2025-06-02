using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Auth;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IGenericMapper<UserDto, User> _userMapper;

    public UserService(IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IGenericMapper<UserDto, User> userMapper)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _userMapper = userMapper;
    }
    
    public async Task Register(UserDto dto, string password, CancellationToken ct)
    {
        var hashedPassword = _passwordHasher.Generate(password);

        var user = new User
        {
            Username = dto.Username,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = hashedPassword
        };
        
        await _userRepository.AddAsync(user, ct);
    }

    public async Task<UserDto> ValidateCredentials(string email, string password, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(email, ct);
        if (user is null)
        {
            throw new ArgumentNullException($"User with email {email} not found. Invalid email");
        }
        
        var result = _passwordHasher.Verify(password, user.PasswordHash);
        if (!result)
        {
            throw new ArgumentException("Invalid password");
        }

        return _userMapper.ToDto(user);
    }

    public async Task<UserDto> GetById(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null) throw new InvalidOperationException($"User with ID {userId} not found");

        return _userMapper.ToDto(user);
    }
}