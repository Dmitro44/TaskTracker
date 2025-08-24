using Moq;
using TaskTracker.Application.DTOs.CheckList;
using TaskTracker.Application.DTOs.CheckListItem;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Tests.Application.Services
{
    public class CheckListServiceTests
    {
        private readonly Mock<ICheckListRepository> _mockCheckListRepository;
        private readonly CheckListService _checkListService;

        public CheckListServiceTests()
        {
            _mockCheckListRepository = new Mock<ICheckListRepository>();

            _checkListService = new CheckListService(
                _mockCheckListRepository.Object);
        }

        [Fact]
        public async Task CreateCheckList_ShouldAddCheckListToRepository()
        {
            // Arrange
            var checkListDto = new CheckListDto 
            { 
                Id = Guid.NewGuid(), 
                Title = "Test CheckList", 
                CardId = Guid.NewGuid() 
            };
            
            var checkList = new CheckList 
            { 
                Id = checkListDto.Id, 
                Title = "Test CheckList", 
                CardId = checkListDto.CardId.Value 
            };

            // Act
            await _checkListService.CreateCheckList(checkListDto, CancellationToken.None);

            // Assert
            _mockCheckListRepository.Verify(r => r.AddAsync(checkList, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCheckListItem_ShouldAddCheckListItemToRepository()
        {
            // Arrange
            var checkListItemDto = new CheckListItemDto 
            { 
                Id = Guid.NewGuid(), 
                Text = "Test Item", 
                CheckListId = Guid.NewGuid(),
                IsCompleted = false
            };
            
            var checkListItem = new CheckListItem 
            { 
                Id = checkListItemDto.Id, 
                Text = "Test Item", 
                CheckListId = checkListItemDto.CheckListId.Value,
                IsCompleted = false
            };

            // Act
            await _checkListService.CreateCheckListItem(checkListItemDto, CancellationToken.None);

            // Assert
            _mockCheckListRepository.Verify(r => r.AddItemAsync(checkListItem, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCheckListItem_ShouldUpdateItemInRepository_AndReturnUpdatedItemDto()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var checkListItemDto = new CheckListItemDto 
            { 
                Id = itemId, 
                Text = "Updated Item",
                IsCompleted = true,
                CheckListId = Guid.NewGuid()
            };
            
            var existingItem = new CheckListItem 
            { 
                Id = itemId, 
                Text = "Original Item",
                IsCompleted = false,
                CheckListId = checkListItemDto.CheckListId.Value
            };

            _mockCheckListRepository.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingItem);

            // Act
            var result = await _checkListService.UpdateCheckListItem(itemId, checkListItemDto, CancellationToken.None);

            // Assert
            Assert.Equal(checkListItemDto, result);
            Assert.Equal("Updated Item", existingItem.Text);
            Assert.True(existingItem.IsCompleted);
            
            _mockCheckListRepository.Verify(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
            _mockCheckListRepository.Verify(r => r.UpdateCheckListItemAsync(existingItem, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCheckListItem_ShouldThrowInvalidOperationException_WhenItemNotFound()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var checkListItemDto = new CheckListItemDto 
            { 
                Id = itemId, 
                Text = "Updated Item" 
            };

            _mockCheckListRepository.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as CheckListItem);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _checkListService.UpdateCheckListItem(itemId, checkListItemDto, CancellationToken.None));

            _mockCheckListRepository.Verify(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
            _mockCheckListRepository.Verify(r => r.UpdateCheckListItemAsync(It.IsAny<CheckListItem>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}