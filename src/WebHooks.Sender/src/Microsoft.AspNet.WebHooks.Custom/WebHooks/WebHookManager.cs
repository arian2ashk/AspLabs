// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks.Custom.Properties;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNet.WebHooks
{
    /// <summary>
    /// Provides an implementation of <see cref="IWebHookManager"/> for managing notifications and mapping
    /// them to registered WebHooks.
    /// </summary>
    public class WebHookManager : IWebHookManager
    {
        internal const string NoEchoParameter = "noecho";
        internal const string EchoParameter = "echo";

        private readonly IWebHookStore _webHookStore;
        private readonly IWebHookSender _webHookSender;
        private readonly IWebHookUser _webHookUser;
        private readonly ILogger<WebHookManager> _logger;
        private readonly HttpClient _httpClient;


        /// <summary>
        /// Initialize a new instance of the <see cref="WebHookManager"/> with a default retry policy.
        /// </summary>
        /// <param name="webHookStore">The current <see cref="IWebHookStore"/>.</param>
        /// <param name="webHookSender">The current <see cref="IWebHookSender"/>.</param>
        /// <param name="webHookUser"></param>
        /// <param name="logger">The current <see cref="ILogger"/>.</param>
        /// <param name="httpClient"></param>
        public WebHookManager(IWebHookStore webHookStore, IWebHookSender webHookSender, IWebHookUser webHookUser, ILogger<WebHookManager> logger, HttpClient httpClient)
        {
            _webHookStore = webHookStore ?? throw new ArgumentNullException(nameof(webHookStore));
            _webHookSender = webHookSender ?? throw new ArgumentNullException(nameof(webHookSender));
            _webHookUser = webHookUser ?? throw new ArgumentNullException(nameof(webHookUser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc />
        public virtual async Task VerifyWebHookAsync(WebHook webHook)
        {
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }

            VerifySecret(webHook.Secret);

            VerifyUri(webHook.WebHookUri);

            await VerifyEchoAsync(webHook);
        }

        /// <inheritdoc />
        public async Task<int> NotifyAsync(IPrincipal user, string action, object data, Func<WebHook, string, bool> predicate = null)
        {
            var notifications = new[] { new NotificationDictionary(action, data) };
            return await NotifyAsync(user, notifications, predicate);
        }

        /// <inheritdoc />
        public async Task<int> NotifyAsync(IPrincipal user, params NotificationDictionary[] notifications)
        {
            return await NotifyAsync(user, notifications, predicate: null);
        }

        /// <inheritdoc />
        public async Task<int> NotifyAsync(IPrincipal user, IEnumerable<NotificationDictionary> notifications, Func<WebHook, string, bool> predicate = null)
        {
            if (notifications == null)
            {
                throw new ArgumentNullException(nameof(notifications));
            }
            if (!notifications.Any())
            {
                return 0;
            }

            // Get the User ID from the User principal
            string userId = await _webHookUser.GetUserIdAsync(user);

            // Send a notification to registered WebHooks with matching filters
            return await NotifyAsync(userId, notifications, predicate);
        }

        /// <inheritdoc />
        public async Task<int> NotifyAsync(string user, IEnumerable<NotificationDictionary> notifications, Func<WebHook, string, bool> predicate)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (notifications == null)
            {
                throw new ArgumentNullException(nameof(notifications));
            }
            if (!notifications.Any())
            {
                return 0;
            }

            // Get all actions in this batch
            ICollection<NotificationDictionary> nots = notifications.ToArray();
            var actions = nots.Select(n => n.Action).ToArray();

            // Find all active WebHooks that matches at least one of the actions
            var webHooks = await _webHookStore.QueryWebHooksAsync(user, actions, predicate);

            // For each WebHook set up a work item with the right set of notifications
            var workItems = GetWorkItems(webHooks, nots);

            // Start sending WebHooks
            await _webHookSender.SendWebHookWorkItemsAsync(workItems);
            return webHooks.Count;
        }

        /// <inheritdoc />
        public async Task<int> NotifyAsync(IEnumerable<string> users, IEnumerable<NotificationDictionary> notifications, Func<WebHook, string, bool> predicate)
        {
            if (users == null || !users.Any())
            {
                throw new ArgumentNullException(nameof(users));
            }
            if (notifications == null)
            {
                throw new ArgumentNullException(nameof(notifications));
            }

            // Get all actions in this batch
            ICollection<NotificationDictionary> nots = notifications.ToArray();
            var actions = nots.Select(n => n.Action).ToArray();

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<int> NotifyAllAsync(string action, object data, Func<WebHook, string, bool> predicate = null)
        {
            var notifications = new[] { new NotificationDictionary(action, data) };
            return await NotifyAllAsync(notifications, predicate);
        }

        /// <inheritdoc />
        public async Task<int> NotifyAllAsync(params NotificationDictionary[] notifications)
        {
            return await NotifyAllAsync(notifications, predicate: null);
        }

        /// <inheritdoc />
        public async Task<int> NotifyAllAsync(IEnumerable<NotificationDictionary> notifications, Func<WebHook, string, bool> predicate)
        {
            if (notifications == null)
            {
                throw new ArgumentNullException(nameof(notifications));
            }

            // Get all actions in this batch
            ICollection<NotificationDictionary> nots = notifications.ToArray();
            var actions = nots.Select(n => n.Action).ToArray();

            // Find all active WebHooks that matches at least one of the actions
            var webHooks = await _webHookStore.QueryWebHooksAcrossAllUsersAsync(actions, predicate);

            // For each WebHook set up a work item with the right set of notifications
            var workItems = GetWorkItems(webHooks, nots);

            // Start sending WebHooks
            await _webHookSender.SendWebHookWorkItemsAsync(workItems);
            return webHooks.Count;
        }

        internal static IEnumerable<WebHookWorkItem> GetWorkItems(ICollection<WebHook> webHooks, ICollection<NotificationDictionary> notifications)
        {
            var workItems = new List<WebHookWorkItem>();
            foreach (var webHook in webHooks)
            {
                ICollection<NotificationDictionary> webHookNotifications;

                // Pick the notifications that apply for this particular WebHook. If we only got one notification
                // then we know that it applies to all WebHooks. Otherwise each notification may apply only to a subset.
                if (notifications.Count == 1)
                {
                    webHookNotifications = notifications;
                }
                else
                {
                    webHookNotifications = notifications.Where(n => webHook.MatchesAction(n.Action)).ToArray();
                    if (webHookNotifications.Count == 0)
                    {
                        continue;
                    }
                }

                var workItem = new WebHookWorkItem(webHook, webHookNotifications);
                workItems.Add(workItem);
            }
            return workItems;
        }

        /// <summary>
        /// Verifies the WebHook by submitting a GET request with a query token intended by the echoed back.
        /// </summary>
        /// <param name="webHook">The <see cref="WebHook"/> to verify.</param>
        protected virtual async Task VerifyEchoAsync(WebHook webHook)
        {
            // Create the echo query parameter that we want returned in response body as plain text.
            var echo = Guid.NewGuid().ToString("N");

            HttpResponseMessage response;
            try
            {
                // If WebHook URI contains a "NoEcho" query parameter then we don't verify the URI using a GET request
                if (webHook.WebHookUri.Query.IndexOf(NoEchoParameter, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    var message = string.Format(CultureInfo.CurrentCulture, CustomResources.Manager_NoEcho);
                    _logger.LogInformation(message);
                    return;
                }

                // Get request URI with echo query parameter
                var webHookUri = new UriBuilder(webHook.WebHookUri)
                {
                    Query = EchoParameter + "=" + echo
                };

                // Create request adding any additional request headers (not entity headers) from Web Hook
                var hookRequest = new HttpRequestMessage(HttpMethod.Get, webHookUri.Uri);
                foreach (var kvp in webHook.Headers)
                {
                    hookRequest.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }

                response = await _httpClient.SendAsync(hookRequest);
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, CustomResources.Manager_VerifyFailure, ex.Message);
                _logger.LogError(message, ex);
                throw new InvalidOperationException(message);
            }

            if (!response.IsSuccessStatusCode)
            {
                var message = string.Format(CultureInfo.CurrentCulture, CustomResources.Manager_VerifyFailure, response.StatusCode);
                _logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            // Verify response body
            if (response.Content == null)
            {
                var message = CustomResources.Manager_VerifyNoBody;
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            var actualEcho = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(actualEcho))
            {
                var message = CustomResources.Manager_VerifyNoBody;
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            if (!string.Equals(actualEcho, echo, StringComparison.Ordinal))
            {
                var message = CustomResources.Manager_VerifyBadEcho;
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Verifies that the <paramref name="webHookUri"/> has either an 'http' or 'https' scheme.
        /// </summary>
        /// <param name="webHookUri">The URI to verify.</param>
        protected virtual void VerifyUri(Uri webHookUri)
        {
            // Check that WebHook URI scheme is either '<c>http</c>' or '<c>https</c>'.
            if (!(webHookUri.IsHttp() || webHookUri.IsHttps()))
            {
                var message = string.Format(CultureInfo.CurrentCulture, CustomResources.Manager_NoHttpUri, webHookUri);
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Verifies that the <see cref="WebHook"/> secret is between 32 and 64 characters long.
        /// </summary>
        /// <param name="secret">The <see cref="WebHook"/> secret to validate.</param>
        protected virtual void VerifySecret(string secret)
        {
            // Check that we have a valid secret
            if (string.IsNullOrEmpty(secret) || secret.Length < 32 || secret.Length > 64)
            {
                throw new InvalidOperationException(CustomResources.WebHook_InvalidSecret);
            }
        }
    }
}
