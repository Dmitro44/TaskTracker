using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Auth;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IJwtProvider jwtProvider)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }
    
    public async Task Register(UserDto dto, string password, CancellationToken ct)
    {
        var hashedPassword = _passwordHasher.Generate(password);

        var user = new User
        {
            Username = dto.UserName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = hashedPassword
        };
        
        await _userRepository.AddAsync(user, ct);
    }

    public async Task<string> Login(string email, string password, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(email, ct);
        if (user is null)
        {
            throw new ArgumentNullException($"User with email {email} not found");
        }
        
        var result = _passwordHasher.Verify(password, user.PasswordHash);
        if (!result)
        {
            throw new ArgumentException("Invalid password");
        }

        var token = _jwtProvider.GenerateToken(user);

        return token;
    }
}