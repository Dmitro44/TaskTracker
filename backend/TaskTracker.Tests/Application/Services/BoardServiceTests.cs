using Moq;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;
using Xunit;

namespace TaskTracker.Tests.Application.Services;

public class BoardServiceTests
{
    private readonly Mock<IGenericMapper<BoardDto, Board>> _mockBoardMapper;
    private readonly Mock<IBoardRepository> _mockBoardRepository;
    private readonly BoardService _boardService;

    public BoardServiceTests()
    {
        _mockBoardRepository = new Mock<IBoardRepository>();
        _mockBoardMapper = new Mock<IGenericMapper<BoardDto, Board>>();
        
        _boardService = new BoardService(
            _mockBoardRepository.Object,
            _mockBoardMapper.Object);
    }

    [Fact]
    public async Task Create_ShouldMapToDtoAndAddToRepository()
    {
        // Arrange
        var boardDto = new BoardDto
        {
            Id = Guid.NewGuid(),
            Title = "Board Title",
            IsPublic = true
        };

        var boardEntity = new Board
        {
            Id = boardDto.Id,
            Title = boardDto.Title,
            IsPublic = boardDto.IsPublic
        };
        
        _mockBoardMapper
            .Setup(mapper => mapper.ToEntity(boardDto))
            .Returns(boardEntity);
        
        // Act
        await _boardService.Create(boardDto, CancellationToken.None);
        
        // Assert
        _mockBoardMapper.Verify(mapper => mapper.ToEntity(boardDto), Times.Once);
        
        _mockBoardRepository.Verify(repo => repo.AddAsync(boardEntity, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetBoards_ShouldReturnMappedDtosForUserBoards()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var userBoards = new List<Board>
        {
            new Board { Id = Guid.NewGuid(), Title = "User's board", IsPublic = true, OwnerId = userId },
            new Board { Id = Guid.NewGuid(), Title = "Another user board", IsPublic = false, OwnerId = userId },
        };

        var expectedDtos = new List<BoardDto>
        {
            new BoardDto { Id = userBoards[0].Id, Title = "User's board DTO" },
            new BoardDto { Id = userBoards[1].Id, Title = "Another user board DTO" }
        };
        
        _mockBoardRepository
            .Setup(repo => repo.GetAllByUserAsync(userId, CancellationToken.None))
            .ReturnsAsync(userBoards);

        for (int i = 0; i < userBoards.Count; ++i)
        {
            _mockBoardMapper
                .Setup(mapper => mapper.ToDto(userBoards[i]))
                .Returns(expectedDtos[i]);
        }
        
        // Act
        var result = await _boardService.GetBoards(userId, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(userBoards.Count, result.Count());
        
        var resultList = result.ToList();
        for (int i = 0; i < expectedDtos.Count; ++i)
        {
            Assert.Equal(expectedDtos[i].Id, resultList[i].Id);
            Assert.Equal(expectedDtos[i].Title, resultList[i].Title);
        }
    }
}