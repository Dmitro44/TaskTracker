using Moq;
using TaskTracker.Application.DTOs.Column;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Tests.Application.Services;

public class ColumnServiceTests
{
    private readonly Mock<IGenericMapper<ColumnShortDto, Column>> _mockColumnMapper;
    private readonly Mock<IColumnRepository> _mockColumnRepository;
    private readonly Mock<IUserService> _mockUserService;
    private readonly ColumnService _columnService;

    public ColumnServiceTests()
    {
        _mockColumnRepository = new Mock<IColumnRepository>();
        _mockColumnMapper = new Mock<IGenericMapper<ColumnShortDto, Column>>();
        _mockUserService = new Mock<IUserService>();
        
        
        _columnService = new ColumnService(
            _mockColumnMapper.Object,
            _mockColumnRepository.Object,
            _mockUserService.Object);
    }
    
    [Fact]
    public async Task Create_Should_Map_And_Add_Column()
    {
        // Arrange
        var dto = new ColumnShortDto { Id = Guid.NewGuid(), Title = "Col", BoardId = Guid.NewGuid(), Position = 1 };
        var entity = new Column { Id = dto.Id, Title = dto.Title, BoardId = dto.BoardId.Value, Position = dto.Position!.Value };

        _mockColumnMapper.Setup(m => m.ToEntity(dto)).Returns(entity);

        // Act
        var result = await _columnService.Create(dto, CancellationToken.None);

        // Assert
        _mockColumnMapper.Verify(m => m.ToEntity(dto), Times.Once);
        _mockColumnRepository.Verify(r => r.AddAsync(entity, CancellationToken.None), Times.Once);
        Assert.Equal(entity, result);
    }

    [Fact]
    public async Task GetColumns_Should_Return_Mapped_Dtos()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var columns = new List<Column>
        {
            new Column { Id = Guid.NewGuid(), BoardId = boardId, Title = "Col1", Position = 1 },
            new Column { Id = Guid.NewGuid(), BoardId = boardId, Title = "Col2", Position = 2 }
        };
        var dtos = columns.Select(c => new ColumnShortDto { Id = c.Id, BoardId = boardId, Title = c.Title, Position = c.Position }).ToList();

        _mockColumnRepository.Setup(r => r.GetAllByBoardAsync(boardId, It.IsAny<CancellationToken>())).ReturnsAsync(columns);
        for (int i = 0; i < columns.Count; ++i)
            _mockColumnMapper.Setup(m => m.ToDto(columns[i])).Returns(dtos[i]);

        // Act
        var result = (await _columnService.GetColumns(boardId, CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(dtos.Count, result.Count);
        for (int i = 0; i < dtos.Count; ++i)
        {
            Assert.Equal(dtos[i].Id, result[i].Id);
            Assert.Equal(dtos[i].Title, result[i].Title);
            Assert.Equal(dtos[i].Position, result[i].Position);
        }
    }

    [Fact]
    public async Task MoveColumns_Should_Throw_If_Column_Not_Found()
    {
        // Arrange
        var dto = new ColumnShortDto { Id = Guid.NewGuid(), Position = 2 };
        _mockColumnRepository.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Column?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _columnService.MoveColumns(dto, CancellationToken.None));
    }

    [Fact]
    public async Task MoveColumns_Should_Throw_If_BoardColumns_Not_Found()
    {
        // Arrange
        var column = new Column { Id = Guid.NewGuid(), BoardId = Guid.NewGuid(), Position = 1 };
        var dto = new ColumnShortDto { Id = column.Id, Position = 2 };
        _mockColumnRepository.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(column);
        _mockColumnRepository.Setup(r => r.GetAllByBoardAsync(column.BoardId, It.IsAny<CancellationToken>())).ReturnsAsync((IEnumerable<Column>?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _columnService.MoveColumns(dto, CancellationToken.None));
    }

    [Fact]
    public async Task MoveColumns_Should_Update_Positions_And_MapPartial()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var column1 = new Column { Id = Guid.NewGuid(), BoardId = boardId, Position = 1, Title = "Col1" };
        var column2 = new Column { Id = Guid.NewGuid(), BoardId = boardId, Position = 2, Title = "Col2" };
        var column3 = new Column { Id = Guid.NewGuid(), BoardId = boardId, Position = 3, Title = "Col3" };
        var columns = new List<Column> { column1, column2, column3 };

        var moveDto = new ColumnShortDto { Id = column1.Id, BoardId = boardId, Position = 3 };

        _mockColumnRepository.Setup(r => r.GetByIdAsync(column1.Id, It.IsAny<CancellationToken>())).ReturnsAsync(column1);
        _mockColumnRepository.Setup(r => r.GetAllByBoardAsync(boardId, It.IsAny<CancellationToken>())).ReturnsAsync(columns);

        _mockColumnMapper.Setup(m => m.MapPartial(moveDto, column1));
        _mockColumnMapper.Setup(m => m.ToDto(column1)).Returns(new ColumnShortDto
        {
            Id = column1.Id,
            BoardId = boardId,
            Title = column1.Title,
            Position = moveDto.Position
        });

        // Act
        var result = await _columnService.MoveColumns(moveDto, CancellationToken.None);

        // Assert
        // Column2 and Column3 should be moved up
        _mockColumnRepository.Verify(r => r.UpdateAsync(column2, It.IsAny<CancellationToken>()), Times.Once);
        _mockColumnRepository.Verify(r => r.UpdateAsync(column3, It.IsAny<CancellationToken>()), Times.Once);
        // The main column should be updated last
        _mockColumnRepository.Verify(r => r.UpdateAsync(column1, It.IsAny<CancellationToken>()), Times.Once);

        _mockColumnMapper.Verify(m => m.MapPartial(moveDto, column1), Times.Once);
        Assert.Equal(column1.Id, result.Id);
        Assert.Equal(moveDto.Position, result.Position);
    }

    [Fact]
    public async Task MoveColumns_Should_Return_If_Old_And_New_Positions_Are_Same()
    {
        // Arrange
        var column = new Column { Id = Guid.NewGuid(), BoardId = Guid.NewGuid(), Position = 2, Title = "Col" };
        var dto = new ColumnShortDto { Id = column.Id, BoardId = column.BoardId, Position = 2 };

        _mockColumnRepository.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(column);
        _mockColumnRepository.Setup(r => r.GetAllByBoardAsync(column.BoardId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Column> { column });
        _mockColumnMapper.Setup(m => m.ToDto(column)).Returns(dto);

        // Act
        var result = await _columnService.MoveColumns(dto, CancellationToken.None);

        // Assert
        _mockColumnRepository.Verify(r => r.UpdateAsync(It.IsAny<Column>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Position, result.Position);
    }

    [Fact]
    public async Task UpdateColumn_Should_Update_And_MapPartial()
    {
        // Arrange
        var column = new Column { Id = Guid.NewGuid(), BoardId = Guid.NewGuid(), Position = 1, Title = "Col" };
        var dto = new ColumnShortDto { Id = column.Id, BoardId = column.BoardId, Position = 2, Title = "Updated" };

        _mockColumnRepository.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(column);

        // Act
        await _columnService.UpdateColumn(dto, CancellationToken.None);

        // Assert
        _mockColumnMapper.Verify(m => m.MapPartial(dto, column), Times.Once);
        _mockColumnRepository.Verify(r => r.UpdateAsync(column, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateColumn_Should_Throw_If_Column_Not_Found()
    {
        // Arrange
        var dto = new ColumnShortDto { Id = Guid.NewGuid() };
        _mockColumnRepository.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Column?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _columnService.UpdateColumn(dto, CancellationToken.None));
    }
    
    [Fact]
    public async Task MoveColumns_Should_ShiftColumnsLeft_WhenMovingLeft()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        // Допустим у нас 3 колонки: позиции 1, 2, 3 (col1, col2, col3)
        var col1 = new Column { Id = Guid.NewGuid(), BoardId = boardId, Position = 1, Title = "Col1" };
        var col2 = new Column { Id = Guid.NewGuid(), BoardId = boardId, Position = 2, Title = "Col2" };
        var col3 = new Column { Id = Guid.NewGuid(), BoardId = boardId, Position = 3, Title = "Col3" };
        var columns = new List<Column> { col1, col2, col3 };

        // будем двигать col3 с позиции 3 на позицию 1 (oldPosition=3, newPosition=1)
        var moveDto = new ColumnShortDto { Id = col3.Id, BoardId = boardId, Position = 1 };

        _mockColumnRepository.Setup(r => r.GetByIdAsync(col3.Id, It.IsAny<CancellationToken>())).ReturnsAsync(col3);
        _mockColumnRepository.Setup(r => r.GetAllByBoardAsync(boardId, It.IsAny<CancellationToken>())).ReturnsAsync(columns);

        _mockColumnMapper.Setup(m => m.MapPartial(moveDto, col3))
            .Callback(() => {
                col3.Position = moveDto.Position.Value;
            });
        _mockColumnMapper.Setup(m => m.ToDto(col3)).Returns(new ColumnShortDto
        {
            Id = col3.Id,
            BoardId = boardId,
            Title = col3.Title,
            Position = moveDto.Position
        });

        // Act
        var result = await _columnService.MoveColumns(moveDto, CancellationToken.None);

        // Assert
        // col1 и col2 должны получить позиции +1 (т.е. col1: 2, col2: 3)
        Assert.Equal(2, col1.Position);
        Assert.Equal(3, col2.Position);
        Assert.Equal(1, col3.Position); // Новая позиция перемещаемой

        _mockColumnRepository.Verify(r => r.UpdateAsync(col1, It.IsAny<CancellationToken>()), Times.Once);
        _mockColumnRepository.Verify(r => r.UpdateAsync(col2, It.IsAny<CancellationToken>()), Times.Once);
        _mockColumnRepository.Verify(r => r.UpdateAsync(col3, It.IsAny<CancellationToken>()), Times.Once);
        _mockColumnMapper.Verify(m => m.MapPartial(moveDto, col3), Times.Once);
        Assert.Equal(col3.Id, result.Id);
        Assert.Equal(moveDto.Position, result.Position);
    }
    
    [Fact]
    public async Task ArchiveColumn_ShouldSetArchiveFlagAndMetadata()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var column = new Column { Id = columnId, Title = "Test Column" };
        var user = new User { Id = userId, FirstName = "Ivan", LastName = "Petrov" };
        
        _mockColumnRepository.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(column);
        
        _mockUserService.Setup(s => s.GetById(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        // Act
        await _columnService.ArchiveColumn(columnId, userId, CancellationToken.None);
        
        // Assert
        Assert.True(column.IsArchived);
        Assert.NotNull(column.ArchivedAt);
        Assert.Equal("Ivan Petrov", column.ArchivedBy);
        
        _mockColumnRepository.Verify(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserService.Verify(s => s.GetById(userId, It.IsAny<CancellationToken>()), Times.Once);
        _mockColumnRepository.Verify(r => r.UpdateAsync(column, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ArchiveColumn_ShouldThrowInvalidOperationException_WhenColumnNotFound()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        _mockColumnRepository.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Column);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _columnService.ArchiveColumn(columnId, userId, CancellationToken.None));
        
        _mockColumnRepository.Verify(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserService.Verify(s => s.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockColumnRepository.Verify(r => r.UpdateAsync(It.IsAny<Column>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RestoreColumn_ShouldClearArchiveFlag()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        var column = new Column 
        { 
            Id = columnId, 
            Title = "Test Column", 
            IsArchived = true, 
            ArchivedAt = DateTime.UtcNow, 
            ArchivedBy = "Ivan Petrov" 
        };
        
        _mockColumnRepository.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(column);
        
        // Act
        await _columnService.RestoreColumn(columnId, CancellationToken.None);
        
        // Assert
        Assert.False(column.IsArchived);
        Assert.Null(column.ArchivedAt);
        Assert.Null(column.ArchivedBy);
        
        _mockColumnRepository.Verify(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()), Times.Once);
        _mockColumnRepository.Verify(r => r.UpdateAsync(column, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RestoreColumn_ShouldThrowInvalidOperationException_WhenColumnNotFound()
    {
        // Arrange
        var columnId = Guid.NewGuid();
        
        _mockColumnRepository.Setup(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Column);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _columnService.RestoreColumn(columnId, CancellationToken.None));
        
        _mockColumnRepository.Verify(r => r.GetByIdAsync(columnId, It.IsAny<CancellationToken>()), Times.Once);
        _mockColumnRepository.Verify(r => r.UpdateAsync(It.IsAny<Column>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}