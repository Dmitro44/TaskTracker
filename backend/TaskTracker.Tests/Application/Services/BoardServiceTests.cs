using Moq;
using TaskTracker.Application.DTOs.Board;
using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.Column;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Tests.Application.Services;

public class BoardServiceTests
{
    private readonly Mock<IGenericMapper<BoardShortDto, Board>> _mockBoardMapper;
    private readonly Mock<IBoardRepository> _mockBoardRepository;
    private readonly Mock<IColumnService> _mockColumnService;
    private readonly Mock<ICardService> _mockCardService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly BoardService _boardService;

    public BoardServiceTests()
    {
        _mockBoardRepository = new Mock<IBoardRepository>();
        _mockBoardMapper = new Mock<IGenericMapper<BoardShortDto, Board>>();
        _mockColumnService = new Mock<IColumnService>();
        _mockCardService = new Mock<ICardService>();
        _mockUserService = new Mock<IUserService>();
        
        _boardService = new BoardService(
            _mockBoardRepository.Object,
            _mockBoardMapper.Object,
            _mockColumnService.Object,
            _mockCardService.Object,
            _mockUserService.Object);
    }

    [Fact]
    public async Task Create_ShouldMapToDtoAndAddToRepository()
    {
        // Arrange
        var boardDto = new BoardShortDto
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

        var expectedDtos = new List<BoardShortDto>
        {
            new BoardShortDto { Id = userBoards[0].Id, Title = "User's board DTO" },
            new BoardShortDto { Id = userBoards[1].Id, Title = "Another user board DTO" }
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
    
    [Fact]
    public async Task GetBoardWithColumnsAndCards_ShouldReturnBoardWithColumnsAndCards()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var board = new Board
        {
            Id = boardId,
            Title = "Test Board",
            IsPublic = true
        };

        var columns = new List<ColumnShortDto>
        {
            new ColumnShortDto { Id = Guid.NewGuid(), Title = "Column 1", Position = 1 },
            new ColumnShortDto { Id = Guid.NewGuid(), Title = "Column 2", Position = 2 }
        };

        var cardsCol1 = new List<CardDto>
        {
            new CardDto { Id = Guid.NewGuid(), Title = "Card 1", Position = 1 },
            new CardDto { Id = Guid.NewGuid(), Title = "Card 2", Position = 2 }
        };

        var cardsCol2 = new List<CardDto>
        {
            new CardDto { Id = Guid.NewGuid(), Title = "Card 3", Position = 1 }
        };

        var cardsByColumns = new Dictionary<Guid, IEnumerable<CardDto>>
        {
            [columns[0].Id] = cardsCol1,
            [columns[1].Id] = cardsCol2
        };

        _mockBoardRepository
            .Setup(repo => repo.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);

        _mockColumnService
            .Setup(service => service.GetColumns(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(columns);

        _mockCardService
            .Setup(service => service.GetCardsByColumns(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cardsByColumns);

        // Act
        var result = await _boardService.GetBoardWithColumnsAndCards(boardId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(board.Id, result.Id);
        Assert.Equal(board.Title, result.Title);
        Assert.Equal(board.IsPublic, result.IsPublic);

        var resultColumns = result.Columns.ToList();
        Assert.Equal(2, resultColumns.Count);

        Assert.Equal(columns[0].Id, resultColumns[0].Id);
        Assert.Equal(columns[0].Title, resultColumns[0].Title);
        Assert.Equal(columns[0].Position, resultColumns[0].Position);
        Assert.Equal(cardsCol1.Count, resultColumns[0].Cards.ToList().Count);

        Assert.Equal(columns[1].Id, resultColumns[1].Id);
        Assert.Equal(columns[1].Title, resultColumns[1].Title);
        Assert.Equal(columns[1].Position, resultColumns[1].Position);
        Assert.Equal(cardsCol2.Count, resultColumns[1].Cards.ToList().Count);
    }

    [Fact]
    public async Task GetBoardWithColumnsAndCards_ShouldThrowIfBoardNotFound()
    {
        // Arrange
        var boardId = Guid.NewGuid();

        _mockBoardRepository
            .Setup(repo => repo.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _boardService.GetBoardWithColumnsAndCards(boardId, CancellationToken.None));
    }
    
    [Fact]
    public async Task ArchiveBoard_ShouldSetArchiveFlagAndMetadata()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var board = new Board { Id = boardId, Title = "Test Board" };
        var user = new User { Id = userId, FirstName = "Ivan", LastName = "Petrov" };
        
        _mockBoardRepository.Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);
        
        _mockUserService.Setup(s => s.GetById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        // Act
        await _boardService.ArchiveBoard(boardId, userId, CancellationToken.None);
        
        // Assert
        Assert.True(board.IsArchived);
        Assert.NotNull(board.ArchivedAt);
        Assert.Equal("Ivan Petrov", board.ArchivedBy);
        
        _mockBoardRepository.Verify(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserService.Verify(s => s.GetById(userId, It.IsAny<CancellationToken>()), Times.Once);
        _mockBoardRepository.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ArchiveBoard_ShouldThrowInvalidOperationException_WhenBoardNotFound()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        _mockBoardRepository.Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Board);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _boardService.ArchiveBoard(boardId, userId, CancellationToken.None));
        
        _mockBoardRepository.Verify(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserService.Verify(s => s.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockBoardRepository.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RestoreBoard_ShouldClearArchiveFlag()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var board = new Board 
        { 
            Id = boardId, 
            Title = "Test Board", 
            IsArchived = true, 
            ArchivedAt = DateTime.UtcNow, 
            ArchivedBy = "Ivan Petrov" 
        };
        
        _mockBoardRepository.Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(board);
        
        // Act
        await _boardService.RestoreBoard(boardId, CancellationToken.None);
        
        // Assert
        Assert.False(board.IsArchived);
        Assert.Null(board.ArchivedAt);
        Assert.Null(board.ArchivedBy);
        
        _mockBoardRepository.Verify(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), Times.Once);
        _mockBoardRepository.Verify(r => r.UpdateAsync(board, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RestoreBoard_ShouldThrowInvalidOperationException_WhenBoardNotFound()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        
        _mockBoardRepository.Setup(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Board);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _boardService.RestoreBoard(boardId, CancellationToken.None));
        
        _mockBoardRepository.Verify(r => r.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), Times.Once);
        _mockBoardRepository.Verify(r => r.UpdateAsync(It.IsAny<Board>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}