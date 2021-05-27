// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Example.Controllers
{
    /// <summary>
    /// The <see cref="WebHookNotificationsController"/> allows the caller to trigger a notification to WebHooks
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookNotificationsController : ControllerBase
    {
        private readonly IWebHookManager _webHookManager;
        private readonly IEnumerable<IWebHookRegistrar> _webHookRegistrars;
        private readonly ILogger<WebHookNotificationsController> _logger;

        public WebHookNotificationsController(IWebHookManager webHookManager, IEnumerable<IWebHookRegistrar> webHookRegistrars, ILogger<WebHookNotificationsController> logger)
        {
            _webHookManager = webHookManager ?? throw new ArgumentNullException(nameof(webHookManager));
            _logger = logger;
            _webHookRegistrars = webHookRegistrars;
        }

        /// <summary>
        /// Triggers a notification that will call related webhooks for current user.
        /// </summary>
        /// <param name="action">The action that will trigger webhooks call</param>
        /// <param name="data">The data that will be send to webhooks</param>
        /// <returns>The number of <see cref="WebHook"/> instances that were selected and subsequently notified about the actions.</returns>
        [HttpPost]
        public async Task<IActionResult> Notify(string action, IDictionary<string, dynamic> data)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return BadRequest();
            }

            try
            {
                var numberOfNotifiedWebHooks= await _webHookManager.NotifyAsync(User, action, data);
                return Ok(numberOfNotifiedWebHooks);
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, "", ex.Message);
                _logger.LogError(ex, message);
                return BadRequest(new
                {
                    detail = message,
                    instance = nameof(WebHookNotificationsController)
                });
            }
        }

        /// <summary>
        /// Triggers a notification that will call related webhooks for all users.
        /// </summary>
        /// <param name="action">The action that will trigger webhooks call</param>
        /// <param name="data">The data that will be send to webhooks</param>
        /// <returns>The number of <see cref="WebHook"/> instances that were selected and subsequently notified about the actions.</returns>
        [HttpPost]
        [Route("all")]
        public async Task<IActionResult> NotifyAll(string action, IDictionary<string, object> data)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return BadRequest();
            }

            try
            {
                var numberOfNotifiedWebHooks = await _webHookManager.NotifyAllAsync(action, data);
                return Ok(numberOfNotifiedWebHooks);
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, "", ex.Message);
                _logger.LogError(ex, message);
                return BadRequest(new
                {
                    detail = message,
                    instance = nameof(WebHookNotificationsController)
                });
            }
        }
    }
}
