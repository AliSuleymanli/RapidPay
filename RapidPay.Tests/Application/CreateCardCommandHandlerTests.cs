using Moq;
using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.CreateCard;

namespace RapidPay.Tests.Application;

public class CreateCardCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsExpectedCardDto()
    {
        // Arrange
        var expectedDto = new CardDto(Guid.NewGuid(), "123456789012345", 500m, 100m, "Active");
        var cardServiceMock = new Mock<ICardService>();
        var idempotencyKey = Guid.NewGuid().ToString();
        cardServiceMock
            .Setup(s => s.CreateCardAsync(
                It.IsAny<decimal?>(),
                It.IsAny<CancellationToken>(),
                idempotencyKey))
            .ReturnsAsync(expectedDto);

        var handler = new CreateCardCommandHandler(cardServiceMock.Object);
        var command = new CreateCardCommand(100m);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(expectedDto, result);
        cardServiceMock.Verify(s => s.CreateCardAsync(100m, cancellationToken, idempotencyKey), Times.Once);
    }
}
