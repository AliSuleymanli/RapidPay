using Moq;
using RapidPay.Application.Features.Auth;
using RapidPay.Application.Features.Auth.Login;

namespace RapidPay.Tests.Application;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResultDto()
    {
        // Arrange
        var testUser = new UserDto(Guid.NewGuid(), "testuser", "testuser@example.com");
        var expectedToken = "test_jwt_token";

        // Create mocks for IUserService and IJwtTokenGenerator.
        var userServiceMock = new Mock<IUserService>();
        var tokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        // Setup IUserService to return the test user.
        userServiceMock
            .Setup(s => s.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser);

        // Setup IJwtTokenGenerator to return the expected token.
        tokenGeneratorMock
            .Setup(t => t.GenerateToken(It.IsAny<UserDto>()))
            .Returns(expectedToken);

        var handler = new LoginCommandHandler(userServiceMock.Object, tokenGeneratorMock.Object);
        var loginCommand = new LoginCommand("testuser", "correctpassword");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(loginCommand, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.Token);
        Assert.Equal(testUser, result.User);

        // Verify that the service methods were called exactly once.
        userServiceMock.Verify(s => s.ValidateUserAsync("testuser", "correctpassword", cancellationToken), Times.Once);
        tokenGeneratorMock.Verify(t => t.GenerateToken(testUser), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var tokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        // Setup IUserService to return null to simulate invalid credentials.
        userServiceMock
            .Setup(s => s.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto?)null);

        var handler = new LoginCommandHandler(userServiceMock.Object, tokenGeneratorMock.Object);
        var loginCommand = new LoginCommand("testuser", "wrongpassword");
        var cancellationToken = CancellationToken.None;

        // Act & Assert: Expect UnauthorizedAccessException.
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.Handle(loginCommand, cancellationToken));

        // Verify that ValidateUserAsync was called.
        userServiceMock.Verify(s => s.ValidateUserAsync("testuser", "wrongpassword", cancellationToken), Times.Once);

        // Verify that token generation was never called.
        tokenGeneratorMock.Verify(t => t.GenerateToken(It.IsAny<UserDto>()), Times.Never);
    }
}
