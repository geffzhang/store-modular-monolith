using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using OnlineStore.Modules.Identity.Domain.Users;
using Role = VaultSharp.V1.SecretsEngines.Database.Role;

namespace OnlineStore.Modules.Identity.Application.Context
{
    public class StoreContext
    {
         /// <summary>
    /// Main working context contains all data which could be used in presentation logic
    /// </summary>
    public partial class OnlineStoreContext : IDisposable, ICloneable
    {
        public OnlineStoreContext()
        {
            ApplicationSettings = new Dictionary<string, object>();
        }

        public string Template { get; set; }

        /// <summary>
        /// Layout which will be used for rendering current view
        /// </summary>
        public string Layout { get; set; }
        /// <summary>
        /// Current request url example: http:/host/app/store/en-us/search?page=2
        /// </summary>
        public Uri RequestUrl { get; set; }

        public NameValueCollection QueryString { get; set; }

        /// <summary>
        /// Current user
        /// </summary>
        public User CurrentUser { get; set; }
        public User Customer => CurrentUser;

        /// <summary>
        /// List of all available roles
        /// </summary>
        public IEnumerable<Role> AvailableRoles { get; set; }

        /// <summary>
        /// List of all b2b roles
        /// </summary>
        public IEnumerable<Role> BusinessToBusinessRoles { get; set; }

        public string ErrorMessage { get; set; }
        /// <summary>
        /// List of active pricelists

        private DateTime? _utcNow;
        /// <summary>
        /// Represent current storefront datetime in UTC (may be changes to test in future or past events)
        /// </summary>
        public DateTime StorefrontUtcNow
        {
            get
            {
                return _utcNow ?? DateTime.UtcNow;
            }
            set
            {
                _utcNow = value;
            }
        }

        /// <summary>
        /// Gets or sets the dictionary of application settings
        /// </summary>
        public IDictionary<string, object> ApplicationSettings { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int? PageNumber { get; set; } = 1;
        /// <summary>
        /// Current page size
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// Settings defined in theme
        /// </summary>
        public IDictionary<string, object> Settings { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// Checks if the current hosting environment name is Microsoft.AspNetCore.Hosting.EnvironmentName.Development.
        /// </summary>
        public bool IsDevelopment { get; set; }

        /// <summary>
        /// The flag that indicates that themes resources won't use cache to be able to preview changes frequently during development and designing.
        /// This setting will be helpful especially when the AzureBlobStorage provider is used because of unable to monitor blob changes as well as for file system
        /// </summary>
        public bool IsPreviewMode { get; set; }


        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public object Clone()
        {
            //TODO: Implement deep clone
            var result = MemberwiseClone() as OnlineStoreContext;
            return result;
        }

        #endregion
    }
    }
}