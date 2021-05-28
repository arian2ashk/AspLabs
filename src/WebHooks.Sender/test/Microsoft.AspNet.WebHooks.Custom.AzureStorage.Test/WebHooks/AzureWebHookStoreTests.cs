// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.WebHooks.Config;
using Microsoft.AspNet.WebHooks.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.AspNet.WebHooks
{
    [Collection("StoreCollection")]
    public class AzureWebHookStoreTests : WebHookStoreTest
    {
        public AzureWebHookStoreTests() : base(CreateStore())
        {
        }

        public static IWebHookStore CreateStore()
        {
            var storageManagerLoggerMock = new Mock<ILogger<StorageManager>>();
            var azureWebHookStoreLoggerMock = new Mock<ILogger<AzureWebHookStore>>();
            var settingsDictionary = new SettingsDictionary( new Dictionary<string, ConnectionSettings> { ["MS_AzureStoreConnectionString"] = new("MS_AzureStoreConnectionString","UseDevelopmentStorage=true;") }) ;
            return new AzureWebHookStore(new StorageManager(storageManagerLoggerMock.Object), settingsDictionary, azureWebHookStoreLoggerMock.Object);
        }

        [Fact]
        public void CreateStore_Succeeds()
        {
            // Arrange
            ILogger<AzureWebHookStore> logger = new Mock<ILogger<AzureWebHookStore>>().Object;
            SettingsDictionary settingsDictionary = new Mock<SettingsDictionary>().Object;
            IStorageManager storageManager = new Mock<IStorageManager>().Object;
            IDataProtector dataProtector = new Mock<IDataProtector>().Object;

            // Act
            IWebHookStore actual = new AzureWebHookStore(storageManager, settingsDictionary, dataProtector, logger);

            // Assert
            Assert.IsType<AzureWebHookStore>(actual);
        }
    }
}
