// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.WebHooks
{
    [Collection("StoreCollection")]
    public abstract class WebHookStoreTest
    {
        private const string OtherUser = "OtherUser";
        private const int WebHookCount = 8;
        private const int TestId = 32;

        private readonly IWebHookStore _store;

        protected WebHookStoreTest(IWebHookStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        protected string TestUser => "TestUser";

        protected IWebHookStore Store => _store;

        [Fact]
        public async Task GetAllWebHooksAsync_ReturnsExpectedItems()
        {
            // Arrange
            await Initialize();

            // Act
            var actual = await _store.GetAllWebHooksAsync(TestUser);

            // Assert
            Assert.Equal(WebHookCount, actual.Count);
            Assert.Equal(WebHookCount, actual.Count(h => h.Description == TestUser));
        }

        [Theory]
        [InlineData("a1", true)]
        [InlineData("A1", true)]
        [InlineData("b1", false)]
        [InlineData("", false)]
        public async Task QueryWebHooksAsync_ReturnsExpectedItems(string action, bool present)
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, filter: "nomatch");
            await _store.InsertWebHookAsync(TestUser, w1);

            // Act
            var actual = await _store.QueryWebHooksAsync(TestUser, new[] { action }, null);

            // Assert
            var expectedCount = present ? WebHookCount : 0;
            Assert.Equal(expectedCount, actual.Count);
            Assert.Equal(expectedCount, actual.Count(h => h.Description == TestUser));
        }

        [Fact]
        public async Task QueryWebHooksAsync_SkipsPausedWebHooks()
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, isPaused: true);
            await _store.InsertWebHookAsync(TestUser, w1);
            var w2 = CreateWebHook(TestUser, TestId + 1, isPaused: true, hasWildcard: true);
            await _store.InsertWebHookAsync(TestUser, w2);

            // Act
            var actual = await _store.QueryWebHooksAsync(TestUser, new[] { "a1" }, null);

            // Assert
            Assert.Equal(WebHookCount, actual.Count);
            Assert.Equal(WebHookCount, actual.Count(h => h.Description == TestUser));
        }

        [Fact]
        public async Task QueryWebHooksAsync_FindsWildcards()
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, filter: "nomatch", hasWildcard: true);
            await _store.InsertWebHookAsync(TestUser, w1);

            // Act
            var actual = await _store.QueryWebHooksAsync(TestUser, new[] { "a1" }, null);

            // Assert
            Assert.Equal(WebHookCount + 1, actual.Count);
            Assert.Equal(WebHookCount, actual.Count(h => h.Filters.Contains("a1")));
            Assert.Equal(1, actual.Count(h => h.Filters.Contains("*")));
        }

        [Theory]
        [InlineData("a1", true)]
        [InlineData("A1", true)]
        [InlineData("b1", false)]
        [InlineData("", false)]
        public async Task QueryWebHooksAsync_ReturnsExpectedItemsWithPredicate(string action, bool present)
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, filter: "nomatch");
            await _store.InsertWebHookAsync(TestUser, w1);

            // Act
            var actual = await _store.QueryWebHooksAsync(TestUser, new[] { action }, (w, u) => u == TestUser.ToLowerInvariant());

            // Assert
            var expectedCount = present ? WebHookCount : 0;
            Assert.Equal(expectedCount, actual.Count);
            Assert.Equal(expectedCount, actual.Count(h => h.Description == TestUser));
        }

        [Fact]
        public async Task QueryWebHooksAsync_SkipsPausedWebHooksWithPredicate()
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, isPaused: true);
            await _store.InsertWebHookAsync(TestUser, w1);
            var w2 = CreateWebHook(TestUser, TestId + 1, isPaused: true, hasWildcard: true);
            await _store.InsertWebHookAsync(TestUser, w2);

            // Act
            var actual = await _store.QueryWebHooksAsync(TestUser, new[] { "a1" }, (w, u) => u == TestUser.ToLowerInvariant());

            // Assert
            Assert.Equal(WebHookCount, actual.Count);
            Assert.Equal(WebHookCount, actual.Count(h => h.Description == TestUser));
        }

        [Fact]
        public async Task QueryWebHooksAsync_FindsWildcardsWithPredicate()
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, filter: "nomatch", hasWildcard: true);
            await _store.InsertWebHookAsync(TestUser, w1);

            // Act
            var actual = await _store.QueryWebHooksAsync(TestUser, new[] { "a1" }, (w, u) => u == TestUser.ToLowerInvariant());

            // Assert
            Assert.Equal(WebHookCount + 1, actual.Count);
            Assert.Equal(WebHookCount, actual.Count(h => h.Filters.Contains("a1")));
            Assert.Equal(1, actual.Count(h => h.Filters.Contains("*")));
        }

        [Theory]
        [InlineData("a1", true)]
        [InlineData("A1", true)]
        [InlineData("b1", false)]
        [InlineData("", false)]
        public async Task QueryWebHooksAcrossAllUsers_ReturnsExpectedItems(string action, bool present)
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, filter: "nomatch");
            await _store.InsertWebHookAsync(TestUser, w1);
            var w2 = CreateWebHook(OtherUser, TestId + 1, filter: "nomatch");
            await _store.InsertWebHookAsync(TestUser, w2);

            // Act
            var all = await _store.QueryWebHooksAcrossAllUsersAsync(new[] { action }, null);
            ICollection<WebHook> actual = all.Where(w => w.Description == TestUser || w.Description == OtherUser).ToArray();

            // Assert
            var expectedCount = present ? WebHookCount : 0;
            Assert.Equal(2 * expectedCount, actual.Count);
            Assert.Equal(expectedCount, actual.Count(h => h.Description == TestUser));
            Assert.Equal(expectedCount, actual.Count(h => h.Description == OtherUser));
        }

        [Fact]
        public async Task QueryWebHooksAcrossAllUsers_SkipsPausedWebHooks()
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, isPaused: true);
            await _store.InsertWebHookAsync(TestUser, w1);
            var w2 = CreateWebHook(OtherUser, TestId + 1, isPaused: true);
            await _store.InsertWebHookAsync(TestUser, w2);

            // Act
            var all = await _store.QueryWebHooksAcrossAllUsersAsync(new[] { "a1" }, null);
            ICollection<WebHook> actual = all.Where(w => w.Description == TestUser || w.Description == OtherUser).ToArray();

            // Assert
            Assert.Equal(2 * WebHookCount, actual.Count);
            Assert.Equal(WebHookCount, actual.Count(h => h.Description == TestUser));
            Assert.Equal(WebHookCount, actual.Count(h => h.Description == OtherUser));
        }

        [Fact]
        public async Task QueryWebHooksAcrossAllUsers_FindsWildcards()
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, filter: "nomatch", hasWildcard: true);
            await _store.InsertWebHookAsync(TestUser, w1);
            var w2 = CreateWebHook(OtherUser, TestId + 1, filter: "nomatch", hasWildcard: true);
            await _store.InsertWebHookAsync(TestUser, w2);

            // Act
            var all = await _store.QueryWebHooksAcrossAllUsersAsync(new[] { "a1" }, null);
            ICollection<WebHook> actual = all.Where(w => w.Description == TestUser || w.Description == OtherUser).ToArray();

            // Assert
            Assert.Equal((2 * WebHookCount) + 2, actual.Count);
            Assert.Equal(2 * WebHookCount, actual.Count(h => h.Filters.Contains("a1")));
            Assert.Equal(2, actual.Count(h => h.Filters.Contains("*")));
        }

        [Theory]
        [InlineData("a1", true)]
        [InlineData("A1", true)]
        [InlineData("b1", false)]
        [InlineData("", false)]
        public async Task QueryWebHooksAcrossAllUsers_ReturnsExpectedItemsWithPredicate(string action, bool present)
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, filter: "nomatch");
            await _store.InsertWebHookAsync(TestUser, w1);
            var w2 = CreateWebHook(OtherUser, TestId + 1, filter: "nomatch");
            await _store.InsertWebHookAsync(TestUser, w2);

            // Act
            var all = await _store.QueryWebHooksAcrossAllUsersAsync(new[] { action }, (w, u) => u == TestUser.ToLowerInvariant());
            ICollection<WebHook> actual = all.Where(w => w.Description == TestUser || w.Description == OtherUser).ToArray();

            // Assert
            var expectedCount = present ? WebHookCount : 0;
            Assert.Equal(expectedCount, actual.Count);
            Assert.Equal(expectedCount, actual.Count(h => h.Description == TestUser));
            Assert.Equal(0, actual.Count(h => h.Description == OtherUser));
        }

        [Fact]
        public async Task QueryWebHooksAcrossAllUsers_SkipsPausedWebHooksWithPredicate()
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, isPaused: true);
            await _store.InsertWebHookAsync(TestUser, w1);
            var w2 = CreateWebHook(OtherUser, TestId + 1, isPaused: true);
            await _store.InsertWebHookAsync(TestUser, w2);

            // Act
            var all = await _store.QueryWebHooksAcrossAllUsersAsync(new[] { "a1" }, (w, u) => u == TestUser.ToLowerInvariant());
            ICollection<WebHook> actual = all.Where(w => w.Description == TestUser || w.Description == OtherUser).ToArray();

            // Assert
            Assert.Equal(WebHookCount, actual.Count);
            Assert.Equal(WebHookCount, actual.Count(h => h.Description == TestUser));
            Assert.Equal(0, actual.Count(h => h.Description == OtherUser));
        }

        [Fact]
        public async Task QueryWebHooksAcrossAllUsers_FindsWildcardsWithPredicate()
        {
            // Arrange
            await Initialize();
            var w1 = CreateWebHook(TestUser, TestId, filter: "nomatch", hasWildcard: true);
            await _store.InsertWebHookAsync(TestUser, w1);
            var w2 = CreateWebHook(OtherUser, TestId + 1, filter: "nomatch", hasWildcard: true);
            await _store.InsertWebHookAsync(OtherUser, w2);

            // Act
            var all = await _store.QueryWebHooksAcrossAllUsersAsync(new[] { "a1" }, (w, u) => u == TestUser.ToLowerInvariant());
            ICollection<WebHook> actual = all.Where(w => w.Description == TestUser || w.Description == OtherUser).ToArray();

            // Assert
            Assert.Equal(WebHookCount + 1, actual.Count);
            Assert.Equal(WebHookCount, actual.Count(h => h.Filters.Contains("a1")));
            Assert.Equal(1, actual.Count(h => h.Filters.Contains("*")));
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("7", true)]
        [InlineData("unknown", false)]
        public async Task LookupWebHooksAsync_ReturnsExpectedItem(string id, bool present)
        {
            // Arrange
            await Initialize();

            // Act
            var actual = await _store.LookupWebHookAsync(TestUser, id);

            // Assert
            if (present)
            {
                Assert.NotNull(actual);
                Assert.Equal(id, actual.Id);
            }
            else
            {
                Assert.Null(actual);
            }
        }

        [Fact]
        public async Task InsertWebHookAsync_InsertsItem()
        {
            // Arrange
            await Initialize();
            var webHook = CreateWebHook(TestUser, TestId);

            // Act
            var actual = await _store.InsertWebHookAsync(TestUser, webHook);

            // Assert
            Assert.Equal(StoreResult.Success, actual);
        }

        [Fact]
        public async Task InsertWebHookAsync_DetectsConflictOnInsertingAlreadyExistingItem()
        {
            // Arrange
            await Initialize();
            var webHook = CreateWebHook(TestUser, TestId);

            // Act
            var actual1 = await _store.InsertWebHookAsync(TestUser, webHook);
            var actual2 = await _store.InsertWebHookAsync(TestUser, webHook);

            // Assert
            Assert.Equal(StoreResult.Success, actual1);
            Assert.Equal(StoreResult.Conflict, actual2);
        }

        [Fact]
        public async Task UpdateWebHookAsync_UpdatesItem()
        {
            // Arrange
            await Initialize();
            var webHook = CreateWebHook(TestUser, 0);
            webHook.Description = "Updated!";

            // Act
            var actual1 = await _store.UpdateWebHookAsync(TestUser, webHook);
            var actual2 = await _store.LookupWebHookAsync(TestUser, "0");

            // Assert
            Assert.Equal(StoreResult.Success, actual1);
            Assert.Equal("Updated!", actual2.Description);
        }

        [Fact]
        public async Task UpdateWebHookAsync_DetectsWhenUpdatedItemDoesNotExist()
        {
            // Arrange
            await Initialize();
            var webHook = CreateWebHook(TestUser, TestId);

            // Act
            var actual = await _store.UpdateWebHookAsync(TestUser, webHook);

            // Assert
            Assert.Equal(StoreResult.NotFound, actual);
        }

        [Fact]
        public async Task DeleteWebHookAsync_DeletesItem()
        {
            // Arrange
            await Initialize();

            // Act
            var actual = await _store.DeleteWebHookAsync(TestUser, "0");

            // Assert
            Assert.Equal(StoreResult.Success, actual);
        }

        [Fact]
        public async Task DeleteWebHookAsync_DetectsWhenDeletedItemDoesNotExist()
        {
            // Arrange
            await Initialize();

            // Act
            var actual1 = await _store.DeleteWebHookAsync(TestUser, "0");
            var actual2 = await _store.DeleteWebHookAsync(TestUser, "0");

            // Assert
            Assert.Equal(StoreResult.Success, actual1);
            Assert.Equal(StoreResult.NotFound, actual2);
        }

        [Fact]
        public async Task DeleteAllWebHooksAsync_DeletesAllItems()
        {
            // Arrange
            await Initialize();

            // Act
            await _store.DeleteAllWebHooksAsync(TestUser);
            var actual1 = await _store.GetAllWebHooksAsync(TestUser);
            var actual2 = await _store.GetAllWebHooksAsync(OtherUser);

            // Assert
            Assert.Empty(actual1);
            Assert.Equal(WebHookCount, actual2.Count);
        }

        [Fact]
        public async Task DeleteAllWebHooksAsync_WorksIfAlreadyEmpty()
        {
            // Arrange
            await Initialize();

            // Act
            await _store.DeleteAllWebHooksAsync(TestUser);
            var actual1 = await _store.GetAllWebHooksAsync(TestUser);
            var actual2 = await _store.GetAllWebHooksAsync(TestUser);

            // Assert
            Assert.Empty(actual1);
            Assert.Empty(actual2);
        }

        protected static WebHook CreateWebHook(string user, int offset, string filter = "a1", bool isPaused = false, bool hasWildcard = false)
        {
            var hook = new WebHook
            {
                Id = offset.ToString(),
                IsPaused = isPaused,
                Description = user,
                Secret = "123456789012345678901234567890123456789012345678",
                WebHookUri = new Uri("http://localhost/hook/" + offset)
            };
            hook.Headers.Add("h1", "hv1");
            hook.Properties.Add("p1", "pv1");
            hook.Filters.Add(filter);
            if (hasWildcard)
            {
                hook.Filters.Add(WildcardWebHookFilterProvider.Name);
            }
            return hook;
        }

        protected virtual async Task Initialize()
        {
            // Reset items for test user
            await _store.DeleteAllWebHooksAsync(TestUser);
            for (var i = 0; i < WebHookCount; i++)
            {
                var webHook = CreateWebHook(TestUser, i);
                await _store.InsertWebHookAsync(TestUser, webHook);
            }

            // Insert items for other user
            await _store.DeleteAllWebHooksAsync(OtherUser);
            for (var i = 0; i < WebHookCount; i++)
            {
                var webHook = CreateWebHook(OtherUser, i);
                await _store.InsertWebHookAsync(OtherUser, webHook);
            }
        }
    }
}
