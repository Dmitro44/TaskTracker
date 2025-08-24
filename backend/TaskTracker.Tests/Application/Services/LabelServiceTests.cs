using Moq;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Interfaces.Repositories;

namespace TaskTracker.Tests.Application.Services
{
    public class LabelServiceTests
    {
        private readonly Mock<ILabelRepository> _mockLabelRepository;
        private readonly LabelService _labelService;

        public LabelServiceTests()
        {
            _mockLabelRepository = new Mock<ILabelRepository>();

            _labelService = new LabelService(
                _mockLabelRepository.Object);
        }

        [Fact]
        public async Task Create_ShouldAddLabelToRepository()
        {
            // Arrange
            var labelDto = new LabelDto 
            { 
                Id = Guid.NewGuid(), 
                Name = "Test Label", 
                Color = "#FF0000",
                BoardId = Guid.NewGuid()
            };
            
            var label = new Label 
            { 
                Id = labelDto.Id, 
                Name = "Test Label", 
                Color = "#FF0000",
                BoardId = labelDto.BoardId
            };

            // Act
            await _labelService.Create(labelDto, CancellationToken.None);

            // Assert
            _mockLabelRepository.Verify(r => r.AddAsync(label, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetLabels_ShouldReturnLabelsFromRepository()
        {
            // Arrange
            var boardId = Guid.NewGuid();
            var labels = new List<Label>
            {
                new Label { Id = Guid.NewGuid(), Name = "Label 1", Color = "#FF0000", BoardId = boardId },
                new Label { Id = Guid.NewGuid(), Name = "Label 2", Color = "#00FF00", BoardId = boardId }
            };
            
            var labelDtos = labels.Select(l => new LabelDto 
            { 
                Id = l.Id, 
                Name = l.Name, 
                Color = l.Color,
                BoardId = l.BoardId 
            }).ToList();

            _mockLabelRepository.Setup(r => r.GetAllByBoardAsync(boardId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(labels);

            // Act
            var result = await _labelService.GetLabels(boardId, CancellationToken.None);

            // Assert
            Assert.Equal(labelDtos.Count, result.Count());
            Assert.Equal(labelDtos, result);
            _mockLabelRepository.Verify(r => r.GetAllByBoardAsync(boardId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AttachLabelToCard_ShouldCallRepositoryMethod()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var labelId = Guid.NewGuid();

            _mockLabelRepository.Setup(r => r.AttachLabelToCardAsync(cardId, labelId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _labelService.AttachLabelToCard(cardId, labelId, CancellationToken.None);

            // Assert
            _mockLabelRepository.Verify(r => r.AttachLabelToCardAsync(cardId, labelId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveLabelFromCard_ShouldCallRepositoryMethod()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var labelId = Guid.NewGuid();

            _mockLabelRepository.Setup(r => r.RemoveLabelFromCardAsync(cardId, labelId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _labelService.RemoveLabelFromCard(cardId, labelId, CancellationToken.None);

            // Assert
            _mockLabelRepository.Verify(r => r.RemoveLabelFromCardAsync(cardId, labelId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}