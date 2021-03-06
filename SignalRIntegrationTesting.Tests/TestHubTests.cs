﻿using Moq;
using SignalRIntegrationTesting.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SignalRIntegrationTesting.Tests
{
    public class TestHubTests
    {
        private readonly AppFixture _fixture;

        public TestHubTests()
        {
            _fixture = new AppFixture();
        }

        [Fact]
        public async Task ShouldNotifySubscribers()
        {
            // Arrange
            var notificationToSend = new Notification { Message = "test message" };

            var connection = new TestHubConnectionBuilder()
                .OnHub(_fixture.GetCompleteServerUrl("/testHub"))
                .WithExpectedEvent<Notification>(nameof(Notification))
                .Build();

            await connection.StartAsync();

            // Act
            await _fixture.ExecuteHttpClientAsync(httpClient =>
                httpClient.PostAsJsonAsync("/hub/test", notificationToSend));

            // Assert
            await connection.VerifyMessageReceived<Notification>(
                n => n.Message == notificationToSend.Message,
                Times.Once());
        }
    }
}
