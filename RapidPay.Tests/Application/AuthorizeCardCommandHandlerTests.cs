using Moq;
using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.AuthorizeCard;

namespace RapidPay.Tests.Application;

public class AuthorizeCardCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCard_ReturnsSuccessfulAuthorization()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var expectedResult = new AuthorizationResultDto(cardId, true, "Authorization successful.");

        var cardServiceMock = new Mock<ICardService>();
        cardServiceMock
            .Setup(s => s.AuthorizeCardAsync(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var handler = new AuthorizeCardCommandHandler(cardServiceMock.Object);
        var command = new AuthorizeCardCommand(cardId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(expectedResult, result);
        cardServiceMock.Verify(s => s.AuthorizeCardAsync(cardId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCard_ReturnsFailedAuthorization()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var expectedResult = new AuthorizationResultDto(cardId, false, "Card not found.");

        var cardServiceMock = new Mock<ICardService>();
        cardServiceMock
            .Setup(s => s.AuthorizeCardAsync(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var handler = new AuthorizeCardCommandHandler(cardServiceMock.Object);
        var command = new AuthorizeCardCommand(cardId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(expectedResult, result);
        cardServiceMock.Verify(s => s.AuthorizeCardAsync(cardId, cancellationToken), Times.Once);
    }
}
