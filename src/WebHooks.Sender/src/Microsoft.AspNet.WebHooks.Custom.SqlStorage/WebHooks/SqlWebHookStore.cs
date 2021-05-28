// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information

using System;
using System.Globalization;
using Microsoft.AspNet.WebHooks.Config;
using Microsoft.AspNet.WebHooks.Custom.SqlStorage.Properties;
using Microsoft.AspNet.WebHooks.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNet.WebHooks
{
    /// <summary>
    /// Provides an implementation of <see cref="IWebHookStore"/> storing registered WebHooks in Microsoft SQL Server.
    /// </summary>
    [CLSCompliant(false)]
    public class SqlWebHookStore : DbWebHookStore<WebHookStoreContext, Registration>
    {
        private readonly string _nameOrConnectionString;
        private readonly string _schemaName;
        private readonly string _tableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWebHookStore"/> class with the given <paramref name="settings"/>
        /// and <paramref name="logger"/>.
        /// Using this constructor, the data will not be encrypted while persisted to the database.
        /// </summary>
        public SqlWebHookStore(SettingsDictionary settings, ILogger<SqlWebHookStore> logger)
            : base(logger)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            CheckSqlStorageConnectionString(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWebHookStore"/> class with the given <paramref name="settings"/>
        /// <paramref name="logger"/>, <paramref name="nameOrConnectionString"/>, <paramref name="schemaName"/> and <paramref name="tableName"/>.
        /// Using this constructor, the data will not be encrypted while persisted to the database.
        /// </summary>
        public SqlWebHookStore(SettingsDictionary settings, ILogger<SqlWebHookStore> logger, string nameOrConnectionString, string schemaName, string tableName)
            : base(logger)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                CheckSqlStorageConnectionString(settings);
            }

            _nameOrConnectionString = nameOrConnectionString;
            _schemaName = schemaName;
            _tableName = tableName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWebHookStore"/> class with the given <paramref name="settings"/>,
        /// <paramref name="protector"/>, and <paramref name="logger"/>.
        /// Using this constructor, the data will be encrypted using the provided <paramref name="protector"/>.
        /// </summary>
        public SqlWebHookStore(SettingsDictionary settings, IDataProtector protector, ILogger<SqlWebHookStore> logger)
            : base(protector, logger)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            CheckSqlStorageConnectionString(settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWebHookStore"/> class with the given <paramref name="settings"/>,
        /// <paramref name="protector"/>, <paramref name="logger"/>, <paramref name="nameOrConnectionString"/>, <paramref name="schemaName"/> and <paramref name="tableName"/>.
        /// Using this constructor, the data will be encrypted using the provided <paramref name="protector"/>.
        /// </summary>
        public SqlWebHookStore(
            SettingsDictionary settings,
            IDataProtector protector,
            ILogger<SqlWebHookStore> logger,
            string nameOrConnectionString,
            string schemaName,
            string tableName) : base(protector, logger)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(nameOrConnectionString))
            {
                CheckSqlStorageConnectionString(settings);
            }

            _nameOrConnectionString = nameOrConnectionString;
            _schemaName = schemaName;
            _tableName = tableName;
        }

        internal static string CheckSqlStorageConnectionString(SettingsDictionary settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!settings.Connections.TryGetValue(WebHookStoreContext.ConnectionStringName, out var connection) ||
                connection == null ||
                string.IsNullOrEmpty(connection.ConnectionString))
            {
                var message = string.Format(CultureInfo.CurrentCulture, SqlStorageResources.SqlStore_NoConnectionString, WebHookStoreContext.ConnectionStringName);
                throw new InvalidOperationException(message);
            }
            return connection.ConnectionString;
        }

        /// <inheritdoc />
        protected override WebHookStoreContext GetContext()
        {
            if (string.IsNullOrEmpty(_nameOrConnectionString))
            {
                return new WebHookStoreContext();
            }
            if (string.IsNullOrEmpty(_schemaName) && string.IsNullOrEmpty(_tableName))
            {
                return new WebHookStoreContext(_nameOrConnectionString);
            }
            return new WebHookStoreContext(_nameOrConnectionString, _schemaName, _tableName);
        }
    }
}
