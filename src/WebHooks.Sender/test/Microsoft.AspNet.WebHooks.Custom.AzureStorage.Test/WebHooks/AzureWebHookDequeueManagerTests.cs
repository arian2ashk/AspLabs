// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks.Mocks;
using Microsoft.AspNet.WebHooks.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.TestUtilities.Mocks;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Xunit;

namespace Microsoft.AspNet.WebHooks
{
    public class AzureWebHookDequeueManagerTests : IDisposable
    {
        private const int MaxAttempts = 0;
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        private TimeSpan _pollingFrequency = TimeSpan.FromMilliseconds(10);
        private TimeSpan _messageTimeout = TimeSpan.FromMilliseconds(10);
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private ILogger<AzureWebHookDequeueManager> _logger;
        private ILogger<WebHookSender> _webHookSenderLogger;
        private Mock<IStorageManager> _storageMock;
        private Mock<WebHookSender> _senderMock;
        private Mock<HttpClient> _httpClientMock;
        private AzureWebHookDequeueManagerMock _dequeueManager;
        private HttpMessageHandlerMock _handlerMock;

        public AzureWebHookDequeueManagerTests()
        {
            _logger = new Mock<ILogger<AzureWebHookDequeueManager>>().Object;
            _webHookSenderLogger = new Mock<ILogger<WebHookSender>>().Object;
            _storageMock = StorageManagerMock.Create();
            _senderMock = new Mock<WebHookSender>(_webHookSenderLogger);
            _handlerMock = new HttpMessageHandlerMock();
            _httpClientMock = new Mock<HttpClient>(_handlerMock);
        }

        public static TheoryData<int[]> DequeueData
        {
            get
            {
                return new TheoryData<int[]>
                {
                    new[] { 0 },
                    new[] { 0, -1, 0, -1, 0 },
                    new[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 },
                    new[] { 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 100 },
                    new[] { 0, 1, 4, -1, 8, 36, 0, -1, 0, 1, 31, 64, 100, 0, 1 },
                };
            }
        }

        public static TheoryData<int[]> SenderData
        {
            get
            {
                return new TheoryData<int[]>
                {
                    new[] { 200, 200, 200 },
                    new[] { -1, -1, -1 },
                    new[] { 400, 500, 404, 401, 503, 302 },
                    new[] { 200, 201, 404, 500, 202, -1, -1 },
                    new[] { -1, 404, 200, 410, 200, 410, -1 },
                };
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(250)]
        public async Task Dispose_CancelsStartTask(int millisecondDelay)
        {
            // Arrange
            _dequeueManager = new AzureWebHookDequeueManagerMock(this, _httpClientMock.Object, _storageMock.Object);

            // Act
            var actual = _dequeueManager.Start(_tokenSource.Token);
            await Task.Delay(millisecondDelay);
            _dequeueManager.Dispose();
            await actual;

            // Assert
            Assert.True(actual.IsCompleted);
        }

        [Fact]
        public async Task Start_Throws_IfCalledMoreThanOnce()
        {
            // Arrange
            _dequeueManager = new AzureWebHookDequeueManagerMock(this, _httpClientMock.Object, _storageMock.Object);
            var start = _dequeueManager.Start(_tokenSource.Token);

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _dequeueManager.Start(_tokenSource.Token));

            // Assert
            Assert.Contains("This 'AzureWebHookDequeueManagerMock' instance has already been started. It can only be started once.", ex.Message);
        }

        [Theory]
        [MemberData(nameof(DequeueData))]
        public async Task DequeueAndSendWebHooks_GetsMessagesAndSubmitsToSender(int[] data)
        {
            // Arrange
            var index = 0;
            _storageMock.Setup(s => s.GetMessagesAsync(StorageManagerMock.CloudQueue, AzureWebHookDequeueManager.MaxDequeuedMessages, _messageTimeout))
                .Returns(() =>
                {
                    var count = index > data.Length ? 0 : data[index++];
                    if (count < 0)
                    {
                        throw new Exception("Catch this!");
                    }
                    var result = StorageManagerMock.CreateQueueMessages(count);
                    return Task.FromResult(result);
                })
                .Callback(() =>
                {
                    if (index > data.Length)
                    {
                        _tokenSource.Cancel();
                    }
                })
                .Verifiable();
            _dequeueManager =
                new AzureWebHookDequeueManagerMock(this, _httpClientMock.Object, storageManager: _storageMock.Object)
                {
                    _sender = _senderMock.Object
                };

            // Act
            await _dequeueManager.DequeueAndSendWebHooks(_tokenSource.Token);

            // Assert
            var expected = data.Count(i => i > 0);
            _senderMock.Verify(s => s.SendWebHookWorkItemsAsync(It.IsAny<IEnumerable<WebHookWorkItem>>()), Times.Exactly(expected));
        }

        [Theory]
        [MemberData(nameof(SenderData))]
        public async Task QueuedSender_Deletes_AllCompletedResponses(int[] statusCodes)
        {
            // Arrange
            _handlerMock.Handler = (req, index) =>
            {
                if (statusCodes[index] < 0)
                {
                    throw new Exception("Catch this!");
                }

                var response = new HttpResponseMessage((HttpStatusCode) statusCodes[index]) {RequestMessage = req};
                return Task.FromResult(response);
            };
            var client = new HttpClient(_handlerMock);
            _dequeueManager = new AzureWebHookDequeueManagerMock(this, httpClient: client, storageManager: _storageMock.Object);
            var workItems = StorageManagerMock.CreateWorkItems(statusCodes.Length);

            // Act
            await _dequeueManager.WebHookSender.SendWebHookWorkItemsAsync(workItems);

            // Assert
            _storageMock.Verify(s => s.DeleteMessagesAsync(StorageManagerMock.CloudQueue, It.Is<IEnumerable<CloudQueueMessage>>(m => m.Count() == statusCodes.Length)), Times.Once());
        }

        [Theory]
        [MemberData(nameof(SenderData))]
        public async Task QueuedSender_Deletes_SuccessAndGoneResponses(int[] statusCodes)
        {
            // Arrange
            _handlerMock.Handler = (req, index) =>
            {
                if (statusCodes[index] < 0)
                {
                    throw new Exception("Catch this!");
                }
                var response = new HttpResponseMessage((HttpStatusCode)statusCodes[index]) { RequestMessage = req };
                return Task.FromResult(response);
            };
            var client = new HttpClient(_handlerMock);
            _dequeueManager = new AzureWebHookDequeueManagerMock(this, httpClient: client, storageManager: _storageMock.Object, maxAttempts: 1);
            var workItems = StorageManagerMock.CreateWorkItems(statusCodes.Length);

            // Act
            await _dequeueManager.WebHookSender.SendWebHookWorkItemsAsync(workItems);

            // Assert
            var expected = statusCodes.Count(i => (i is >= 200 and <= 299) || i == 410);
            _storageMock.Verify(s => s.DeleteMessagesAsync(StorageManagerMock.CloudQueue, It.Is<IEnumerable<CloudQueueMessage>>(m => m.Count() == expected)), Times.Once());
        }

        public void Dispose()
        {
            if (_dequeueManager != null)
            {
                _dequeueManager.Dispose();
            }
            if (_handlerMock != null)
            {
                _handlerMock.Dispose();
            }
            if (_tokenSource != null)
            {
                _tokenSource.Dispose();
            }
        }

        private class AzureWebHookDequeueManagerMock : AzureWebHookDequeueManager
        {
            public AzureWebHookDequeueManagerMock(AzureWebHookDequeueManagerTests parent, HttpClient httpClient,
                IStorageManager storageManager, int maxAttempts = MaxAttempts)
                : base(ConnectionString, parent._pollingFrequency, parent._messageTimeout, maxAttempts, httpClient, storageManager, parent._logger, parent._webHookSenderLogger)
            {
            }

            public new Task DequeueAndSendWebHooks(CancellationToken cancellationToken)
            {
                return base.DequeueAndSendWebHooks(cancellationToken);
            }
        }
    }
}
