using Moq;
using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.GetCardBalance;

namespace RapidPay.Tests.Application;

public class GetCardBalanceQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidQuery_ReturnsExpectedCardBalanceDto()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var expectedDto = new CardBalanceDto(cardId, 500m, 1000m, 1500m);

        var cardServiceMock = new Mock<ICardService>();
        cardServiceMock
            .Setup(s => s.GetCardBalanceAsync(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        var handler = new GetCardBalanceQueryHandler(cardServiceMock.Object);
        var query = new GetCardBalanceQuery(cardId);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.Equal(expectedDto, result);
        cardServiceMock.Verify(s => s.GetCardBalanceAsync(cardId, cancellationToken), Times.Once);
    }
}
