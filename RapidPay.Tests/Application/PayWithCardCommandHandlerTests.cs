using Moq;
using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.PayWithCard;

namespace RapidPay.Tests.Application;

public class PayWithCardCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsExpectedPaymentTransactionDto()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var paymentAmount = 200m;
        var expectedDto = new PaymentTransactionDto(
            TransactionId: Guid.NewGuid(),
            CardId: cardId,
            PaymentAmount: paymentAmount,
            Fee: 10m,
            NewBalance: 790m,
            Timestamp: DateTime.UtcNow
        );

        var cardServiceMock = new Mock<ICardService>();
        cardServiceMock
            .Setup(s => s.PayWithCardAsync(cardId, paymentAmount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        var handler = new PayWithCardCommandHandler(cardServiceMock.Object);
        var command = new PayWithCardCommand(cardId, paymentAmount);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(expectedDto, result);
        cardServiceMock.Verify(s => s.PayWithCardAsync(cardId, paymentAmount, cancellationToken), Times.Once);
    }
}