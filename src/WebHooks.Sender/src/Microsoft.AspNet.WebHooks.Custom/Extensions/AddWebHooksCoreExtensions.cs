using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNet.WebHooks.Custom.Extensions
{
    public static class AddWebHooksCoreExtensions
    {
        public static void AddWebHooksCore(this IServiceCollection services, Action<IServiceCollection> overrideTypes = null)
        {
            services.AddSingleton<IWebHookSender, DataflowWebHookSender>();
            services.AddScoped<IWebHookUser, WebHookUser>();
            services.AddScoped<IWebHookRegistrationsManager, WebHookRegistrationsManager>();
            services.AddScoped<IWebHookFilterManager, WebHookFilterManager>();
            services.AddScoped<IWebHookFilterProvider, WildcardWebHookFilterProvider>();
            services.AddScoped<IWebHookManager, WebHookManager>();
            services.AddSingleton<IWebHookStore, MemoryWebHookStore>();
            services.AddSingleton(c=> new HttpClient());

            overrideTypes?.Invoke(services);
        }
    }
}
