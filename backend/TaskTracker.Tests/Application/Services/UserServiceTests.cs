using Moq;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Auth;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Tests.Application.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGenericMapper<UserDto, User>> _mockUserMapper;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserMapper = new Mock<IGenericMapper<UserDto, User>>();

            _userService = new UserService(
                _mockPasswordHasher.Object,
                _mockUserRepository.Object,
                _mockUserMapper.Object);
        }

        [Fact]
        public async Task Register_ShouldHashPasswordAndAddUserToRepository()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com"
            };
            
            string password = "password123";
            string hashedPassword = "hashedpassword123";

            _mockPasswordHasher.Setup(h => h.Generate(password))
                .Returns(hashedPassword);

            // Act
            await _userService.Register(userDto, password, CancellationToken.None);

            // Assert
            _mockPasswordHasher.Verify(h => h.Generate(password), Times.Once);
            
            _mockUserRepository.Verify(r => r.AddAsync(
                It.Is<User>(u => 
                    u.Username == userDto.Username && 
                    u.FirstName == userDto.FirstName && 
                    u.LastName == userDto.LastName && 
                    u.Email == userDto.Email && 
                    u.PasswordHash == hashedPassword),
                It.IsAny<CancellationToken>()),
            Times.Once);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password123";
            string hashedPassword = "hashedpassword123";
            string token = "jwt_token_123";
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = email,
                PasswordHash = hashedPassword,
                FirstName = "Test",
                LastName = "User"
            };

            var userDto = new UserDto
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = email,
                FirstName = "Test",
                LastName = "User"
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(h => h.Verify(password, hashedPassword))
                .Returns(true);

            // _mockJwtProvider.Setup(j => j.GenerateToken(user))
            //     .Returns(token);

            _mockUserMapper.Setup(m => m.ToDto(user)).Returns(userDto);

            // Act
            var result = await _userService.ValidateCredentials(email, password, CancellationToken.None);

            // Assert
            Assert.Equal(userDto, result);
            _mockUserRepository.Verify(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
            _mockPasswordHasher.Verify(h => h.Verify(password, hashedPassword), Times.Once);
            // _mockJwtProvider.Verify(j => j.GenerateToken(user), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldThrowArgumentNullException_WhenUserNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string password = "password123";

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _userService.ValidateCredentials(email, password, CancellationToken.None));
            
            _mockUserRepository.Verify(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
            _mockPasswordHasher.Verify(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_ShouldThrowArgumentException_WhenPasswordIsInvalid()
        {
            // Arrange
            string email = "test@example.com";
            string password = "wrongpassword";
            string hashedPassword = "hashedpassword123";
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = email,
                PasswordHash = hashedPassword,
                FirstName = "Test",
                LastName = "User"
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(h => h.Verify(password, hashedPassword))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await _userService.ValidateCredentials(email, password, CancellationToken.None));
            
            _mockUserRepository.Verify(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
            _mockPasswordHasher.Verify(h => h.Verify(password, hashedPassword), Times.Once);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User 
            { 
                Id = userId, 
                Username = "Dmitro44",
                FirstName = "Dmitro",
                LastName = "Test",
                Email = "dmitro@example.com"
            };

            var userDto = new UserDto
            {
                Id = userId,
                Username = "Dmitro44",
                FirstName = "Dmitro",
                LastName = "Test",
                Email = "dmitro@example.com"
            };
    
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockUserMapper.Setup(m => m.ToDto(user))
                .Returns(userDto);
    
            // Act
            var result = await _userService.GetById(userId, CancellationToken.None);
    
            // Assert
            Assert.Equal(userDto, result);
            _mockUserRepository.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
    
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);
    
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _userService.GetById(userId, CancellationToken.None));
    
            Assert.Equal($"User with ID {userId} not found", exception.Message);
            _mockUserRepository.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}