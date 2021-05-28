﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.AspNet.WebHooks.Custom.AzureStorage.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AzureStorageResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AzureStorageResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.AspNet.WebHooks.Custom.AzureStorage.Properties.AzureStorageResources", typeof(AzureStorageResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not serialize message: {0}.
        /// </summary>
        internal static string AzureSender_SerializeFailure {
            get {
                return ResourceManager.GetString("AzureSender_SerializeFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; could not be retrieved from Azure Table store: {1}.
        /// </summary>
        internal static string AzureStore_BadWebHook {
            get {
                return ResourceManager.GetString("AzureStore_BadWebHook", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please provide a Microsoft Azure Storage connection string with name &apos;{0}&apos; in the configuration string section of the &apos;Web.Config&apos; file..
        /// </summary>
        internal static string AzureStore_NoConnectionString {
            get {
                return ResourceManager.GetString("AzureStore_NoConnectionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No entry was found with partition key &apos;{0}&apos; and row key &apos;{1}&apos;..
        /// </summary>
        internal static string AzureStore_NotFound {
            get {
                return ResourceManager.GetString("AzureStore_NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encountered an error when dequeueing messages from the Azure Queue &apos;{0}&apos;: &apos;{1}&apos;.
        /// </summary>
        internal static string DequeueManager_ErrorDequeueing {
            get {
                return ResourceManager.GetString("DequeueManager_ErrorDequeueing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Giving up sending WebHook &apos;{0}&apos; after &apos;{1}&apos; attempts..
        /// </summary>
        internal static string DequeueManager_GivingUp {
            get {
                return ResourceManager.GetString("DequeueManager_GivingUp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not get the property with key &apos;{0}&apos; from the work item with Id &apos;{1}&apos;..
        /// </summary>
        internal static string DequeueManager_NoProperty {
            get {
                return ResourceManager.GetString("DequeueManager_NoProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not send HTTP request to &apos;{0}&apos;: &apos;{1}&apos;..
        /// </summary>
        internal static string DequeueManager_SendFailure {
            get {
                return ResourceManager.GetString("DequeueManager_SendFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This &apos;{0}&apos; instance has already been started. It can only be started once..
        /// </summary>
        internal static string DequeueManager_Started {
            get {
                return ResourceManager.GetString("DequeueManager_Started", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to WebHook &apos;{0}&apos; resulted in status code &apos;{1}&apos; on attempt &apos;{2}&apos;..
        /// </summary>
        internal static string DequeueManager_WebHookStatus {
            get {
                return ResourceManager.GetString("DequeueManager_WebHookStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid continuation token: &apos;{0}&apos;..
        /// </summary>
        internal static string StorageManager_BadContinuationToken {
            get {
                return ResourceManager.GetString("StorageManager_BadContinuationToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not get the list of containers for this Microsoft Azure Storage account: {0}.
        /// </summary>
        internal static string StorageManager_ContainerListError {
            get {
                return ResourceManager.GetString("StorageManager_ContainerListError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating item in Azure table &apos;{0}&apos; failed with error code: &apos;{1}&apos;..
        /// </summary>
        internal static string StorageManager_CreateFailed {
            get {
                return ResourceManager.GetString("StorageManager_CreateFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not retrieve the given record with the given partition key and row key: {0}.
        /// </summary>
        internal static string StorageManager_ErrorRetrieving {
            get {
                return ResourceManager.GetString("StorageManager_ErrorRetrieving", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not initialize connection to Microsoft Azure Storage: {0}.
        /// </summary>
        internal static string StorageManager_InitializationFailure {
            get {
                return ResourceManager.GetString("StorageManager_InitializationFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection string is invalid. Please provide a valid Microsoft Azure Storage connection string. You can get Microsoft Azure Storage connection string from the Azure portal..
        /// </summary>
        internal static string StorageManager_InvalidConnectionString {
            get {
                return ResourceManager.GetString("StorageManager_InvalidConnectionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value &apos;{0}&apos; for query option &apos;{1}&apos;. Please provide a valid value..
        /// </summary>
        internal static string StorageManager_InvalidQueryOptionValue {
            get {
                return ResourceManager.GetString("StorageManager_InvalidQueryOptionValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not create a &apos;{0}&apos; instance from the connection string..
        /// </summary>
        internal static string StorageManager_NoCloudStorageAccount {
            get {
                return ResourceManager.GetString("StorageManager_NoCloudStorageAccount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation failed with status code {0} and error: &apos;{1}&apos;..
        /// </summary>
        internal static string StorageManager_OperationFailed {
            get {
                return ResourceManager.GetString("StorageManager_OperationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Query operation failed with error: &apos;{0}&apos;..
        /// </summary>
        internal static string StorageManager_QueryFailed {
            get {
                return ResourceManager.GetString("StorageManager_QueryFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not get the list of tables for this Microsoft Azure Storage account: {0}.
        /// </summary>
        internal static string StorageManager_TableListError {
            get {
                return ResourceManager.GetString("StorageManager_TableListError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected result of type &apos;{0}&apos; but received type &apos;{1}&apos;..
        /// </summary>
        internal static string StorageManager_UnexpectedTableResult {
            get {
                return ResourceManager.GetString("StorageManager_UnexpectedTableResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The query option &apos;{0}&apos; is not allowed. Please use only the supported query options: {1}..
        /// </summary>
        internal static string StorageManager_UnsupportedQueryOption {
            get {
                return ResourceManager.GetString("StorageManager_UnsupportedQueryOption", resourceCulture);
            }
        }
    }
}
