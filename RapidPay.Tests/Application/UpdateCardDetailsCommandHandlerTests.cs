using Moq;
using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.CreateCard;
using RapidPay.Application.Features.CardManagement.UpdateCardDetails;

namespace RapidPay.Tests.Application;

public class UpdateCardDetailsCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsExpectedCardDto()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var expectedDto = new CardDto(
            cardId,
            "123456789012345",
            800m,
            1000m,
            "Active"
        );

        var cardServiceMock = new Mock<ICardService>();
        cardServiceMock
            .Setup(s => s.UpdateCardDetailsAsync(It.IsAny<UpdateCardDetailsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        var handler = new UpdateCardDetailsCommandHandler(cardServiceMock.Object);
        var command = new UpdateCardDetailsCommand(cardId, 800m, 1000m, "Active");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(expectedDto, result);
        cardServiceMock.Verify(s => s.UpdateCardDetailsAsync(command, cancellationToken), Times.Once);
    }
}
