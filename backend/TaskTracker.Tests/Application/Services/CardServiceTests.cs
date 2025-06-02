using Moq;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Card;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Interfaces.Mapping;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Tests.Application.Services
{
    public class CardServiceTests
    {
        private readonly Mock<ICardRepository> _mockCardRepository;
        private readonly Mock<IGenericMapper<CardDto, Card>> _mockCardMapper;
        private readonly Mock<ILabelService> _mockLabelService;
        private readonly Mock<ICheckListService> _mockCheckListService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly CardService _cardService;

        public CardServiceTests()
        {
            _mockCardRepository = new Mock<ICardRepository>();
            _mockCardMapper = new Mock<IGenericMapper<CardDto, Card>>();
            _mockLabelService = new Mock<ILabelService>();
            _mockCheckListService = new Mock<ICheckListService>();
            _mockUserService = new Mock<IUserService>();

            _cardService = new CardService(
                _mockCardRepository.Object,
                _mockCardMapper.Object,
                _mockLabelService.Object,
                _mockCheckListService.Object,
                _mockUserService.Object);
        }

        [Fact]
        public async Task CreateCard_ShouldAddCardToRepository_AndReturnCreatedCard()
        {
            // Arrange
            var cardDto = new CardDto { Id = Guid.NewGuid(), Title = "Test Card" };
            var card = new Card { Id = cardDto.Id, Title = cardDto.Title };

            _mockCardMapper.Setup(m => m.ToEntity(cardDto))
                .Returns(card);

            // Act
            var result = await _cardService.CreateCard(cardDto, CancellationToken.None);

            // Assert
            Assert.Equal(card, result);
            _mockCardRepository.Verify(r => r.AddAsync(card, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllCards_ShouldReturnAllCardsFromRepository()
        {
            // Arrange
            var boardId = Guid.NewGuid();
            var cards = new List<Card>
            {
                new Card { Id = Guid.NewGuid(), Title = "Card 1" },
                new Card { Id = Guid.NewGuid(), Title = "Card 2" }
            };
            var cardDtos = cards.Select(c => new CardDto { Id = c.Id, Title = c.Title }).ToList();

            _mockCardRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(cards);

            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var cardDto = cardDtos[i];
                _mockCardMapper.Setup(m => m.ToDto(card))
                    .Returns(cardDto);
            }

            // Act
            var result = (await _cardService.GetAllCards(boardId, CancellationToken.None)).ToList();

            // Assert
            Assert.Equal(cardDtos.Count, result.Count);
            Assert.Equal(cardDtos, result);
            _mockCardRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCard_ShouldUpdateCardInRepository_AndReturnUpdatedCardDto()
        {
            // Arrange
            var cardDto = new CardDto { Id = Guid.NewGuid(), Title = "Updated Card" };
            var card = new Card { Id = cardDto.Id, Title = "Original Card" };

            _mockCardRepository.Setup(r => r.GetByIdAsync(cardDto.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(card);

            _mockCardMapper.Setup(m => m.MapPartial(cardDto, card))
                .Callback(() => card.Title = cardDto.Title); // Simulate the mapping

            _mockCardMapper.Setup(m => m.ToDto(card))
                .Returns(cardDto);

            // Act
            var result = await _cardService.UpdateCard(cardDto, CancellationToken.None);

            // Assert
            Assert.Equal(cardDto, result);
            Assert.Equal("Updated Card", card.Title);
            _mockCardRepository.Verify(r => r.GetByIdAsync(cardDto.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mockCardRepository.Verify(r => r.UpdateAsync(card, It.IsAny<CancellationToken>()), Times.Once);
            _mockCardMapper.Verify(m => m.MapPartial(cardDto, card), Times.Once);
            _mockCardMapper.Verify(m => m.ToDto(card), Times.Once);
        }

        [Fact]
        public async Task UpdateCard_ShouldThrowInvalidOperationException_WhenCardNotFound()
        {
            // Arrange
            var cardDto = new CardDto { Id = Guid.NewGuid(), Title = "Updated Card" };

            _mockCardRepository.Setup(r => r.GetByIdAsync(cardDto.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Card);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _cardService.UpdateCard(cardDto, CancellationToken.None));

            _mockCardRepository.Verify(r => r.GetByIdAsync(cardDto.Id, It.IsAny<CancellationToken>()), Times.Once);
            _mockCardRepository.Verify(r => r.UpdateAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddLabelToCard_ShouldCallLabelService()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var labelId = Guid.NewGuid();

            _mockLabelService.Setup(s => s.AttachLabelToCard(cardId, labelId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cardService.AddLabelToCard(cardId, labelId, CancellationToken.None);

            // Assert
            _mockLabelService.Verify(s => s.AttachLabelToCard(cardId, labelId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveLabelFromCard_ShouldCallLabelService()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var labelId = Guid.NewGuid();

            _mockLabelService.Setup(s => s.RemoveLabelFromCard(cardId, labelId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cardService.RemoveLabelFromCard(cardId, labelId, CancellationToken.None);

            // Assert
            _mockLabelService.Verify(s => s.RemoveLabelFromCard(cardId, labelId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddCheckList_ShouldCallCheckListService()
        {
            // Arrange
            var checkListDto = new CheckListDto { Id = Guid.NewGuid(), Title = "Test CheckList" };

            _mockCheckListService.Setup(s => s.CreateCheckList(checkListDto, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cardService.AddCheckList(checkListDto, CancellationToken.None);

            // Assert
            _mockCheckListService.Verify(s => s.CreateCheckList(checkListDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddCheckListItem_ShouldCallCheckListService()
        {
            // Arrange
            var checkListItemDto = new CheckListItemDto { Id = Guid.NewGuid(), Text = "Test Item" };

            _mockCheckListService.Setup(s => s.CreateCheckListItem(checkListItemDto, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cardService.AddCheckListItem(checkListItemDto, CancellationToken.None);

            // Assert
            _mockCheckListService.Verify(s => s.CreateCheckListItem(checkListItemDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCheckListItem_ShouldCallCheckListService_AndReturnResult()
        {
            // Arrange
            var checkListItemId = Guid.NewGuid();
            var checkListItemDto = new CheckListItemDto { Id = checkListItemId, Text = "Updated Item" };
            var updatedDto = new CheckListItemDto { Id = checkListItemId, Text = "Updated Item" };

            _mockCheckListService.Setup(s => s.UpdateCheckListItem(checkListItemId, checkListItemDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedDto);

            // Act
            var result = await _cardService.UpdateCheckListItem(checkListItemId, checkListItemDto, CancellationToken.None);

            // Assert
            Assert.Equal(updatedDto, result);
            _mockCheckListService.Verify(s => s.UpdateCheckListItem(checkListItemId, checkListItemDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCardsByColumns_ShouldReturnCardsDictionaryGroupedByColumnId()
        {
            // Arrange
            var columnIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            
            var cards = new List<Card>
            {
                new Card { Id = Guid.NewGuid(), Title = "Card 1", ColumnId = columnIds[0] },
                new Card { Id = Guid.NewGuid(), Title = "Card 2", ColumnId = columnIds[0] },
                new Card { Id = Guid.NewGuid(), Title = "Card 3", ColumnId = columnIds[1] }
            };
            
            var cardDtos = cards.Select(c => new CardDto { Id = c.Id, Title = c.Title, ColumnId = c.ColumnId }).ToList();

            _mockCardRepository.Setup(r => r.GetCardsByColumns(columnIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cards);

            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var cardDto = cardDtos[i];
                _mockCardMapper.Setup(m => m.ToDto(card))
                    .Returns(cardDto);
            }

            // Act
            var result = await _cardService.GetCardsByColumns(columnIds, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(2, result[columnIds[0]].Count());
            Assert.Single(result[columnIds[1]]);
            Assert.Contains(cardDtos[0], result[columnIds[0]]);
            Assert.Contains(cardDtos[1], result[columnIds[0]]);
            Assert.Contains(cardDtos[2], result[columnIds[1]]);
            _mockCardRepository.Verify(r => r.GetCardsByColumns(columnIds, It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task ArchiveCard_ShouldSetArchiveFlagAndMetadata()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var card = new Card { Id = cardId, Title = "Test Card" };
            var user = new UserDto { Id = userId, FirstName = "Ivan", LastName = "Petrov" };
            
            _mockCardRepository.Setup(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(card);
            
            _mockUserService.Setup(s => s.GetById(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            
            // Act
            await _cardService.ArchiveCard(cardId, userId, CancellationToken.None);
            
            // Assert
            Assert.True(card.IsArchived);
            Assert.NotNull(card.ArchivedAt);
            Assert.Equal("Ivan Petrov", card.ArchivedBy);
            
            _mockCardRepository.Verify(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()), Times.Once);
            _mockUserService.Verify(s => s.GetById(userId, It.IsAny<CancellationToken>()), Times.Once);
            _mockCardRepository.Verify(r => r.UpdateAsync(card, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ArchiveCard_ShouldThrowInvalidOperationException_WhenCardNotFound()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            _mockCardRepository.Setup(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Card);
            
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _cardService.ArchiveCard(cardId, userId, CancellationToken.None));
            
            _mockCardRepository.Verify(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()), Times.Once);
            _mockUserService.Verify(s => s.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockCardRepository.Verify(r => r.UpdateAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RestoreCard_ShouldClearArchiveFlag()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var card = new Card 
            { 
                Id = cardId, 
                Title = "Test Card", 
                IsArchived = true, 
                ArchivedAt = DateTime.Parse("2025-05-16 21:58:31"), 
                ArchivedBy = "Ivan Petrov" 
            };
            
            _mockCardRepository.Setup(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(card);
            
            // Act
            await _cardService.RestoreCard(cardId, CancellationToken.None);
            
            // Assert
            Assert.False(card.IsArchived);
            Assert.Null(card.ArchivedAt);
            Assert.Null(card.ArchivedBy);
            
            _mockCardRepository.Verify(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()), Times.Once);
            _mockCardRepository.Verify(r => r.UpdateAsync(card, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RestoreCard_ShouldThrowInvalidOperationException_WhenCardNotFound()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            
            _mockCardRepository.Setup(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Card);
            
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _cardService.RestoreCard(cardId, CancellationToken.None));
            
            _mockCardRepository.Verify(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()), Times.Once);
            _mockCardRepository.Verify(r => r.UpdateAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}