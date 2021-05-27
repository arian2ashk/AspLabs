﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.AspNet.WebHooks.Common.Properties {
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
    internal class CommonResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CommonResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.AspNet.WebHooks.Common.Properties.CommonResources", typeof(CommonResources).Assembly);
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
        ///   Looks up a localized string similar to WebHooks support has not been initialized correctly. Please call the initializer &apos;{0}&apos; on startup. See &apos;http://go.microsoft.com/fwlink/?LinkId=799408&apos; for details on how to initialize WebHooks..
        /// </summary>
        internal static string Config_NotInitialized {
            get {
                return ResourceManager.GetString("Config_NotInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot read value &apos;{0}&apos; as type &apos;{1}&apos;..
        /// </summary>
        internal static string DateTime_BadFormat {
            get {
                return ResourceManager.GetString("DateTime_BadFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot convert null value to type &apos;{0}&apos;..
        /// </summary>
        internal static string DateTime_NullError {
            get {
                return ResourceManager.GetString("DateTime_NullError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Input is not a valid hex-encoded string: &apos;{0}&apos;. Please provide a valid hex-encoded string..
        /// </summary>
        internal static string EncodingUtils_InvalidHexValue {
            get {
                return ResourceManager.GetString("EncodingUtils_InvalidHexValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not verify the validity of the encrypted data..
        /// </summary>
        internal static string Encryptor_InvalidHash {
            get {
                return ResourceManager.GetString("Encryptor_InvalidHash", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The encryption key provided by setting &apos;{0}&apos; must be exactly 32 bytes long when encoded as UTF-8 but was found to be {1} bytes long..
        /// </summary>
        internal static string Encryptor_InvalidKey {
            get {
                return ResourceManager.GetString("Encryptor_InvalidKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No WebHook setting was found with key &apos;{0}&apos;. Please ensure that the WebHooks module is initialized with the correct application settings..
        /// </summary>
        internal static string Settings_KeyNotFound {
            get {
                return ResourceManager.GetString("Settings_KeyNotFound", resourceCulture);
            }
        }
    }
}
